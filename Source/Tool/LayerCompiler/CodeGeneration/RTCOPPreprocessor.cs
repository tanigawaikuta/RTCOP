using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using LayerCompiler.Parsers;
using LayerCompiler.Parsers.Model;
using LayerCompiler.CodeGeneration.Model;
using Sprache;

namespace LayerCompiler.CodeGeneration
{
    /// <summary>
    /// RTCOPファイルのプリプロセッサ
    /// </summary>
    class RTCOPPreprocessor
    {
        #region フィールド
        private List<string> _ImportedFileNames = new List<string>();

        #endregion

        #region プロパティ
        /// <summary>
        /// レイヤコンパイラに与えるマクロ定義
        /// </summary>
        public List<PreprocessDirective> DefinedMacros { get; private set; }

        /// <summary>
        /// ヘッダファイルのディレクトリパス
        /// </summary>
        public List<string> IncludePaths { get; private set; }

        /// <summary>
        /// 入出力ファイルの文字コード
        /// </summary>
        public Encoding Encoding { get; protected set; }

        #endregion

        #region コンストラクタ
        /// <summary>
        /// RTCOPファイルのプリプロセッサ
        /// </summary>
        /// <param name="macros">レイヤコンパイラに与えるマクロ定義</param>
        /// <param name="includePaths">ヘッダファイルのディレクトリパス</param>
        /// <param name="encoding">入出力ファイルの文字コード</param>
        public RTCOPPreprocessor(IEnumerable<string> macros, List<string> includePaths, Encoding encoding)
        {
            DefinedMacros = new List<PreprocessDirective>(macros.Count());
            foreach (var macro in macros)
            {
                var item = new PreprocessDirective(DirectiveKind.Define, macro);
                DefinedMacros.Add(item);
            }
            IncludePaths = includePaths;
            Encoding = encoding;
        }

        #endregion

        /// <summary>
        /// プリプロセス実行
        /// </summary>
        /// <param name="inputFile">入力ファイル</param>
        /// <returns>実行後のファイル</returns>
        public RTCOPSourceFile Run(RTCOPSourceFile inputFile)
        {
            var macros = new List<PreprocessDirective>(DefinedMacros);
            _ImportedFileNames.Clear();
            return Run_Private(inputFile, macros);
        }

        #region メソッド
        /// <summary>
        /// プリプロセス実行
        /// </summary>
        /// <param name="inputFile">入力ファイル</param>
        /// <param name="macros">ファイル内で定義されたマクロ</param>
        /// <returns>実行後のファイル</returns>
        private RTCOPSourceFile Run_Private(RTCOPSourceFile inputFile, List<PreprocessDirective> macros)
        {
            // ファイルから全てのディレクティブ、行を取得
            var allObjs = PreprocessParser.IfSection.Or(PreprocessParser.DirectiveOrLineForIfSection).TokenWithSkipCommentForPreprocessParser().Many().Parse(inputFile.Text);
            var importingFiles = new List<RTCOPSourceFile>();

            // 実際のプリプロセス処理を行う
            Func<IEnumerable<object>, IEnumerable<object>> runPreprocess = null;
            runPreprocess = (objs) =>
            {
                var newObjs = new List<object>();
                foreach (object obj in objs)
                {
                    // ifセクション
                    if (obj is IfSection)
                    {
                        var ifSection = (IfSection)obj;
                        foreach (var group in ifSection.Groups)
                        {
                            // 条件が合えば追加
                            bool isMatch = EvaluateIfDirectiveExpression(group.First() as PreprocessDirective, macros);
                            if (isMatch)
                            {
                                var additionalGroup = runPreprocess(group.Skip(1));
                                newObjs.AddRange(additionalGroup);
                                break;
                            }
                        }
                    }
                    // ディレクティブ
                    else if (obj is PreprocessDirective)
                    {
                        var directive = (PreprocessDirective)obj;
                        switch (directive.Kind)
                        {
                            case DirectiveKind.Define:
                                macros.Add(directive);
                                //newObjs.Add(directive);
                                break;
                            case DirectiveKind.Undef:
                                var removingMacro = macros.Find((macro) => macro.Param1 == directive.Param1);
                                if (removingMacro != null) macros.Remove(removingMacro);
                                //newObjs.Add(directive);
                                break;
                            case DirectiveKind.ImportLayerHeader:
                            case DirectiveKind.ImportBaseClassHeader:
                                RTCOPSourceFile importingFile = null;
                                foreach (var incpath in IncludePaths)
                                {
                                    string path = incpath + directive.Param1;
                                    string fullpath = Path.GetFullPath(path);
                                    if (File.Exists(fullpath))
                                    {
                                        if (_ImportedFileNames.Contains(fullpath))
                                        {
                                            // 同じパスのファイルは二度読み込まないようにする
                                            goto EndImport;
                                        }
                                        else
                                        {
                                            _ImportedFileNames.Add(fullpath);
                                        }
                                        // ファイルオープン
                                        RTCOPSourceFile src = null;
                                        using (StreamReader sr = new StreamReader(fullpath, Encoding))
                                        {
                                            string text = sr.ReadToEnd();
                                            src = new RTCOPSourceFile(fullpath, text);
                                        }
                                        importingFile = Run_Private(src, macros);
                                        break;
                                    }
                                }
                                if (importingFile == null)
                                {
                                    throw new Exception("ファイル: " + directive.Param1 + "が見つかりません");
                                }
                                importingFiles.Add(importingFile);
                                if (directive.Kind == DirectiveKind.ImportBaseClassHeader)
                                {
                                    // includeに変換して追加
                                    PreprocessDirective incdirective = new PreprocessDirective(DirectiveKind.Include, directive.Param1);
                                    newObjs.Add(incdirective);
                                }
                                else
                                {
                                    newObjs.Add(directive);
                                }
                                EndImport:
                                break;
                            default:
                                newObjs.Add(directive);
                                break;
                        }
                    }
                    // 通常の行
                    else
                    {
                        // マクロ展開して追加
                        var line = (NonDirectiveLine)obj;
                        var newLine = ExpandMacro(line.Contents, macros, false);
                        newObjs.Add(newLine);
                    }
                }
                return newObjs;
            };
            // 実行開始
            var result = runPreprocess(allObjs);

            // 結果を返す
            StringBuilder sb = new StringBuilder();
            foreach (object obj in result)
            {
                sb.Append(obj.ToString());
                sb.Append("\r\n");
            }
            RTCOPSourceFile outputFile = new RTCOPSourceFile(inputFile.FilePath, sb.ToString(), importingFiles);

            return outputFile;
        }

        /// <summary>
        /// 行内のマクロを展開する
        /// </summary>
        /// <param name="tokens">トークンの集合</param>
        /// <param name="macros">defineマクロ</param>
        /// <param name="isIfDirective">ifディレクティブ内かどうか</param>
        /// <returns>展開後の行</returns>
        private NonDirectiveLine ExpandMacro(List<Token> tokens, List<PreprocessDirective> macros, bool isIfDirective)
        {
            Func<List<Token>, List<Token>> expantionFunc = null;
            expantionFunc = (atokens) =>
            {
                var newTokens = new List<Token>();
                for (int i = 0; i < atokens.Count; ++i)
                {
                    var token = atokens[i];
                    // 識別子ならマクロの可能性あり
                    if (token is Identifier)
                    {
                        var macro = macros.Find((mcr) => mcr.Param1 == token.ToString());
                        if (macro != null)
                        {
                            // 実引数の確認
                            int paramNum = (macro.Param2.Count - 1);
                            List<Token>[] parameters = new List<Token>[paramNum];
                            if (paramNum > 0)
                            {
                                if (atokens[i + 1].ToString() != "(")
                                {
                                    throw new Exception("マクロ関数の構文エラー: " + macro.Param1);
                                }
                                int paramCount = 0;
                                int skipTokenNum = 1;
                                int parenthesesCount = 1;
                                while (parenthesesCount > 0)
                                {
                                    // パラメータ用のリスト生成
                                    if (parameters[paramCount] == null)
                                    {
                                        parameters[paramCount] = new List<Token>();
                                    }
                                    // トークンチェック
                                    ++skipTokenNum;
                                    var st = atokens[i + skipTokenNum];
                                    if ((st.ToString() == ",") && (parenthesesCount == 1))
                                    {
                                        ++paramCount;
                                    }
                                    else if (st.ToString() == "(")
                                    {
                                        ++parenthesesCount;
                                        parameters[paramCount].Add(st);
                                    }
                                    else if ((st.ToString() == ")") && (parenthesesCount > 1))
                                    {
                                        --parenthesesCount;
                                        parameters[paramCount].Add(st);
                                    }
                                    else if (st.ToString() == ")")
                                    {
                                        ++paramCount;
                                        --parenthesesCount;
                                    }
                                    else
                                    {
                                        parameters[paramCount].Add(st);
                                    }
                                }
                                if (paramCount != paramNum)
                                {
                                    throw new Exception("マクロ関数の実引数の数が合っていません: " + macro.Param1);
                                }
                                i += skipTokenNum;
                            }
                            // パラメータ置き換え
                            var macroTokens = new List<Token>(TokenParser.RTCOPToken.TokenWithSkipComment().Many().Parse(macro.Param2.Last()));
                            var macroTokens2 = new List<Token>();
                            Token pre = null;
                            for (int j = 0; j < macroTokens.Count; ++j)
                            {
                                var mt = macroTokens[j];
                                if (mt.ToString() == "#")
                                {
                                    var mt2 = macroTokens[j + 1];
                                    int index = macro.Param2.FindIndex(0, (macro.Param2.Count - 1), (ms) => mt2.ToString() == ms);
                                    if (index == -1)
                                    {
                                        throw new Exception("#演算子の後はマクロ関数の引数である必要があります: " + macro.Param1);
                                    }
                                    var objs = expantionFunc(parameters[index]);
                                    string text = string.Join(" ", objs);
                                    var sl = new StringLiteral("\"" + text + "\"", text, "", "");
                                    macroTokens2.Add(sl);
                                    ++j;
                                }
                                else if (mt.ToString() == "##")
                                {
                                    var mt2 = macroTokens[j + 1];
                                    if (mt2 is Identifier)
                                    {
                                        int index = macro.Param2.FindIndex(0, (macro.Param2.Count - 1), (ms) => mt2.ToString() == ms);
                                        if (index != -1)
                                        {
                                            var objs = expantionFunc(parameters[index]);
                                            string text = pre.ToString() + objs[0].ToString();
                                            objs[0] = TokenParser.RTCOPToken.TokenWithSkipComment().Parse(text);
                                            macroTokens2.RemoveAt(macroTokens2.Count - 1);
                                            macroTokens2.AddRange(objs);
                                        }
                                        else
                                        {
                                            string text = pre.ToString() + mt2.ToString();
                                            var mt3 = TokenParser.RTCOPToken.TokenWithSkipComment().Parse(text);
                                            macroTokens2.RemoveAt(macroTokens2.Count - 1);
                                            macroTokens2.Add(mt3);
                                        }
                                    }
                                    else
                                    {
                                        string text = pre.ToString() + mt2.ToString();
                                        var mt3 = TokenParser.RTCOPToken.TokenWithSkipComment().Parse(text);
                                        macroTokens2.RemoveAt(macroTokens2.Count - 1);
                                        macroTokens2.Add(mt3);
                                    }
                                    ++j;
                                }
                                else if (mt is Identifier)
                                {
                                    int index = macro.Param2.FindIndex(0, (macro.Param2.Count - 1), (ms) => mt.ToString() == ms);
                                    if (index != -1)
                                    {
                                        var objs = expantionFunc(parameters[index]);
                                        macroTokens2.AddRange(objs);
                                    }
                                    else
                                    {
                                        macroTokens2.Add(mt);
                                    }
                                }
                                else
                                {
                                    macroTokens2.Add(mt);
                                }
                                pre = macroTokens2[macroTokens2.Count - 1];
                            }
                            // さらに深く見る
                            var macroTokens3 = expantionFunc(macroTokens2);
                            // ifディレクティブで定義無しなら0として扱う
                            if (isIfDirective && (macroTokens3.Count == 0))
                            {
                                var zero = new IntegerLiteral("0", "0", "", "");
                                newTokens.Add(zero);
                            }
                            else
                            {
                                newTokens.AddRange(macroTokens3);
                            }
                        }
                        else
                        {
                            // マクロではない
                            newTokens.Add(token);
                        }
                    }
                    // 違っていればとりあえず追加
                    else
                    {
                        newTokens.Add(token);
                    }
                }
                return newTokens;
            };
            var ts = expantionFunc(tokens);
            return new NonDirectiveLine(ts);
        }

        /// <summary>
        /// ifディレクティブの評価
        /// </summary>
        /// <param name="directive">ディレクティブ</param>
        /// <param name="macros">defineマクロ</param>
        /// <returns>条件を満たしているかどうか</returns>
        private bool EvaluateIfDirectiveExpression(PreprocessDirective directive, List<PreprocessDirective> macros)
        {
            bool result = false;
            switch (directive.Kind)
            {
                case DirectiveKind.If:
                case DirectiveKind.Elif:
                    // 式のトークン取得
                    var tokens = new List<Token>(TokenParser.RTCOPToken.TokenWithSkipComment().Many().Parse(directive.Param1));
                    // definedを置き換える
                    var newTokens = new List<Token>();
                    for (int i = 0; i < tokens.Count; ++i)
                    {
                        var token = tokens[i];
                        if (token.ToString() == "defined")
                        {
                            if ((tokens[i + 1].ToString() == "(") && (tokens[i + 2] is Identifier) && (tokens[i + 3].ToString() == ")"))
                            {
                                IntegerLiteral il = null;
                                if (macros.Exists((macro) => macro.Param1 == tokens[i + 2].ToString()))
                                {
                                    il = new IntegerLiteral("1", "1", "", "");
                                }
                                else
                                {
                                    il = new IntegerLiteral("0", "0", "", "");
                                }
                                newTokens.Add(il);
                                i += 3;
                            }
                            else if (tokens[i + 1] is Identifier)
                            {
                                IntegerLiteral il = null;
                                if (macros.Exists((macro) => macro.Param1 == tokens[i + 1].ToString()))
                                {
                                    il = new IntegerLiteral("1", "1", "", "");
                                }
                                else
                                {
                                    il = new IntegerLiteral("0", "0", "", "");
                                }
                                newTokens.Add(il);
                                i += 1;
                            }
                            else
                            {
                                throw new Exception("definedの書き方が間違っています");
                            }
                        }
                        else
                        {
                            newTokens.Add(token);
                        }
                    }
                    // マクロ展開
                    var newLine = ExpandMacro(newTokens, macros, true);
                    // 式を評価
                    IfDirectiveExpression expression = PreprocessParser.IfDirectiveExpression.TokenWithSkipComment().Parse(newLine.ToString());
                    Literal exResult = expression.Evaluate();
                    if (exResult is IntegerLiteral) result = ((IntegerLiteral)exResult).Value != 0;
                    else if (exResult is CharacterLiteral) result = ((CharacterLiteral)exResult).Value != 0;
                    else if (exResult is BooleanLiteral) result = ((BooleanLiteral)exResult).Value;
                    else if (exResult is PointerLiteral) result = false;
                    else result = ((FloatingLiteral)exResult).Value != 0;
                    break;
                case DirectiveKind.Ifdef:
                    result = macros.Exists((macro) => macro.Param1 == directive.Param1);
                    break;
                case DirectiveKind.Ifndef:
                    result = !macros.Exists((macro) => macro.Param1 == directive.Param1);
                    break;
                case DirectiveKind.Else:
                    result = true;
                    break;
                default:
                    break;
            }
            return result;
        }

        #endregion

    }
}
