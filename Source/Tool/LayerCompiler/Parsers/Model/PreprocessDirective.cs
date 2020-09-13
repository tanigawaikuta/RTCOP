using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LayerCompiler.Parsers.Model
{
    /// <summary>
    /// プリプロセッサへの指令
    /// </summary>
    [Serializable]
    class PreprocessDirective
    {
        #region プロパティ
        /// <summary>
        /// 種類
        /// </summary>
        public DirectiveKind Kind { get; protected set; }

        /// <summary>
        /// パラメータ1 マクロの名前など
        /// </summary>
        public string Param1 { get; protected set; }

        /// <summary>
        /// パラメータ2 マクロの値など
        /// </summary>
        public List<string> Param2 { get; private set; }

        #endregion

        #region コンストラクタ
        /// <summary>
        /// プリプロセッサへの指令
        /// </summary>
        /// <param name="kind">種類</param>
        /// <param name="param1">パラメータ1 マクロの名前など</param>
        /// <param name="param2">パラメータ2 マクロの値など</param>
        public PreprocessDirective(DirectiveKind kind = DirectiveKind.None, string param1 = "", IEnumerable<string> param2 = null)
        {
            Kind = kind;
            param1 = param1.Replace("\\\n", "");
            param1 = param1.Replace("\\\r\n", "");
            Param1 = param1;
            if (param2 != null)
            {
                Param2 = new List<string>(param2);
                for (int i = 0; i < Param2.Count; ++i)
                {
                    var param = Param2[i];
                    param = param.Replace("\\\n", "");
                    param = param.Replace("\\\r\n", "");
                    Param2[i] = param;
                }
            }
            else
            {
                Param2 = new List<string>(0);
            }
        }

        #endregion

        #region メソッド
        /// <summary>
        /// 文字列を返す
        /// </summary>
        /// <returns>文字列</returns>
        public override string ToString()
        {
            string result = "#";
            switch (Kind)
            {
                case DirectiveKind.Ifdef:
                    result += ("ifdef " + Param1);
                    break;
                case DirectiveKind.Ifndef:
                    result += ("ifndef " + Param1);
                    break;
                case DirectiveKind.If:
                    result += ("if " + Param1);
                    break;
                case DirectiveKind.Elif:
                    result += ("elif " + Param1);
                    break;
                case DirectiveKind.Else:
                    result += ("else");
                    break;
                case DirectiveKind.Endif:
                    result += ("endif");
                    break;
                case DirectiveKind.Define:
                    result += ("define " + Param1);
                    if (Param2.Count >= 2)
                    {
                        result += "(";
                        result += string.Join(", ", Param2.ToArray(), 0, (Param2.Count - 1));
                        result += ")";
                    }
                    if (Param2.Last() != "")
                    {
                        result += (" " + Param2.Last());
                    }
                    break;
                case DirectiveKind.Undef:
                    result += ("undef " + Param1);
                    break;
                case DirectiveKind.Include:
                    result += ("include \"" + Param1 + "\"");
                    break;
                case DirectiveKind.Line:
                    result += ("line " + Param1);
                    if (Param2.Count >= 1)
                    {
                        result += (" \"" + Param2[0] + "\"");
                    }
                    break;
                case DirectiveKind.Error:
                    result += "error";
                    if (Param1 != "")
                    {
                        result += (" " + Param1);
                    }
                    break;
                case DirectiveKind.Pragma:
                    result += "pragma";
                    if (Param1 != "")
                    {
                        result += (" " + Param1);
                    }
                    break;
                case DirectiveKind.NonStandard:
                    result += Param1;
                    foreach (var param in Param2)
                    {
                        result += (" " + param);
                    }
                    break;
                case DirectiveKind.ImportLayerHeader:
                    result += ("import_lh \"" + Param1 + "\"");
                    break;
                case DirectiveKind.ImportBaseClassHeader:
                    result += ("import_baseclass \"" + Param1 + "\"");
                    break;
                default:
                    break;
            }
            return result;
        }

        #endregion

    }

    /// <summary>
    /// プリプロセッサへの指令の種類
    /// </summary>
    [Serializable]
    enum DirectiveKind
    {
        /// <summary>#のみ</summary>
        None,
        /// <summary>#ifdef</summary>
        Ifdef,
        /// <summary>#ifndef</summary>
        Ifndef,
        /// <summary>#if</summary>
        If,
        /// <summary>#elif</summary>
        Elif,
        /// <summary>#else</summary>
        Else,
        /// <summary>#endif</summary>
        Endif,
        /// <summary>#define</summary>
        Define,
        /// <summary>#undef</summary>
        Undef,
        /// <summary>#include</summary>
        Include,
        /// <summary>#line</summary>
        Line,
        /// <summary>#error</summary>
        Error,
        /// <summary>#pragma</summary>
        Pragma,
        /// <summary>非標準</summary>
        NonStandard,
        /// <summary>#import_lh</summary>
        ImportLayerHeader,
        /// <summary>#import_baseclass</summary>
        ImportBaseClassHeader,
    }

    /// <summary>
    /// 非ディレクティブ行
    /// </summary>
    [Serializable]
    class NonDirectiveLine
    {
        #region プロパティ
        /// <summary>
        /// 内容
        /// </summary>
        public List<Token> Contents { get; protected set; }

        #endregion

        #region コンストラクタ
        /// <summary>
        /// 非ディレクティブ行
        /// </summary>
        /// <param name="contents">内容</param>
        public NonDirectiveLine(IEnumerable<Token> contents)
        {
            Contents = new List<Token>(contents);
        }

        #endregion

        #region メソッド
        /// <summary>
        /// 文字列を返す
        /// </summary>
        /// <returns>文字列</returns>
        public override string ToString()
        {
            return string.Join(" ", Contents);
        }

        #endregion

    }

    /// <summary>
    /// ifセクション
    /// </summary>
    [Serializable]
    class IfSection
    {
        #region プロパティ
        /// <summary>
        /// ifセクション内のグループのリスト
        /// </summary>
        public List<IEnumerable<object>> Groups { get; private set; } = new List<IEnumerable<object>>();

        #endregion

        #region コンストラクタ
        /// <summary>
        /// ifセクション
        /// </summary>
        /// <param name="ifgroup">ifグループ</param>
        /// <param name="elifgroups">elifグループ</param>
        /// <param name="elsegroup">elseグループ</param>
        public IfSection(IEnumerable<object> ifgroup, IEnumerable<IEnumerable<object>> elifgroups, IEnumerable<object> elsegroup)
        {
            Groups.Add(ifgroup);
            Groups.AddRange(elifgroups);
            if (elsegroup != null)
            {
                Groups.Add(elsegroup);
            }
        }

        #endregion

        #region メソッド
        /// <summary>
        /// 文字列を返す
        /// </summary>
        /// <returns>文字列</returns>
        public override string ToString()
        {
            string result = "";
            foreach (var group in Groups)
            {
                result += (string.Join("\r\n", group) + "\r\n");
            }
            result += "#endif";
            return result;
        }

        #endregion

    }

    /// <summary>
    /// ifディレクティブの式
    /// </summary>
    [Serializable]
    class IfDirectiveExpression
    {
        #region プロパティ
        /// <summary>
        /// 演算子
        /// </summary>
        public IEnumerable<string> Operators { get; protected set; }

        /// <summary>
        /// 単項演算子
        /// </summary>
        public IEnumerable<string> UnaryOperators { get; protected set; }

        /// <summary>
        /// 子の式
        /// </summary>
        public IEnumerable<IfDirectiveExpression> SubExpressions { get; protected set; }

        #endregion

        #region コンストラクタ
        /// <summary>
        /// ifディレクティブの式
        /// </summary>
        /// <param name="operators">演算子</param>
        /// <param name="expressions">子の式</param>
        /// <param name="unaryOperators">単項演算子</param>
        public IfDirectiveExpression(IEnumerable<string> operators, IEnumerable<IfDirectiveExpression> expressions, IEnumerable<string> unaryOperators = null)
        {
            Operators = operators;
            SubExpressions = expressions;
            if (unaryOperators != null)
            {
                UnaryOperators = unaryOperators;
            }
            else
            {
                UnaryOperators = new string[0];
            }
        }

        /// <summary>
        /// ifディレクティブの式
        /// </summary>
        /// <param name="expression">式</param>
        /// <param name="unaryOperators">単項演算子</param>
        public IfDirectiveExpression(IfDirectiveExpression expression, IEnumerable<string> unaryOperators)
        {
            Operators = expression.Operators;
            SubExpressions = expression.SubExpressions;
            UnaryOperators = unaryOperators;
        }

        #endregion

        #region メソッド
        /// <summary>
        /// 式の評価
        /// </summary>
        /// <returns>評価結果</returns>
        public virtual Literal Evaluate()
        {
            var values = from expression in SubExpressions
                         select expression.Evaluate();
            List<Literal> vlist = new List<Literal>(values);
            Literal result = vlist[0];
            int ct = 1;
            foreach (var op in Operators)
            {
                switch (op)
                {
                    case "*":
                        result = Mult(result, vlist[ct++]);
                        break;
                    case "/":
                        result = Div(result, vlist[ct++]);
                        break;
                    case "%":
                        result = Mod(result, vlist[ct++]);
                        break;
                    case "+":
                        result = Plus(result, vlist[ct++]);
                        break;
                    case "-":
                        result = Minus(result, vlist[ct++]);
                        break;
                    case "<<":
                        result = LShift(result, vlist[ct++]);
                        break;
                    case ">>":
                        result = RShift(result, vlist[ct++]);
                        break;
                    case "<":
                        result = Less(result, vlist[ct++]);
                        break;
                    case ">":
                        result = Greater(result, vlist[ct++]);
                        break;
                    case "<=":
                        result = LessEq(result, vlist[ct++]);
                        break;
                    case ">=":
                        result = GreaterEq(result, vlist[ct++]);
                        break;
                    case "==":
                        result = Equal(result, vlist[ct++]);
                        break;
                    case "!=":
                        result = NotEqual(result, vlist[ct++]);
                        break;
                    case "&":
                        result = And(result, vlist[ct++]);
                        break;
                    case "^":
                        result = XOr(result, vlist[ct++]);
                        break;
                    case "|":
                        result = Or(result, vlist[ct++]);
                        break;
                    case "&&":
                        result = AndAnd(result, vlist[ct++]);
                        break;
                    case "||":
                        result = OrOr(result, vlist[ct++]);
                        break;
                }
            }
            foreach (var op in UnaryOperators)
            {
                switch (op)
                {
                    case "-":
                        result = UnaryMinus(result);
                        break;
                    case "!":
                        result = Not(result);
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// 加算
        /// </summary>
        protected Literal Plus(Literal lvalue, Literal rvalue)
        {
            Literal result = null;
            if ((lvalue is IntegerLiteral) || (lvalue is CharacterLiteral) || (lvalue is BooleanLiteral))
            {
                // 左の値
                long left = 0;
                if (lvalue is IntegerLiteral) left = ((IntegerLiteral)lvalue).Value;
                if (lvalue is CharacterLiteral) left = ((CharacterLiteral)lvalue).Value;
                if (lvalue is BooleanLiteral) left = ((BooleanLiteral)lvalue).Value ? 1 : 0;
                // 右の値
                if ((rvalue is IntegerLiteral) || (rvalue is CharacterLiteral) || (rvalue is BooleanLiteral))
                {
                    long right = 0;
                    if (rvalue is IntegerLiteral) right = ((IntegerLiteral)rvalue).Value;
                    if (rvalue is CharacterLiteral) right = ((CharacterLiteral)rvalue).Value;
                    if (rvalue is BooleanLiteral) right = ((BooleanLiteral)rvalue).Value ? 1 : 0;
                    long calc = left + right;
                    string text = calc.ToString();
                    result = new IntegerLiteral(text, text, "", "");
                }
                else if (rvalue is FloatingLiteral)
                {
                    double right = ((FloatingLiteral)rvalue).Value;
                    double calc = left + right;
                    string text = calc.ToString();
                    result = new FloatingLiteral(text, text, "", "");
                }
                else
                {
                    throw new Exception("nullptrで演算子+は使えない");
                }
            }
            else if (lvalue is FloatingLiteral)
            {
                // 左の値
                double left = ((FloatingLiteral)lvalue).Value;
                // 右の値
                if ((rvalue is IntegerLiteral) || (rvalue is CharacterLiteral) || (rvalue is BooleanLiteral))
                {
                    long right = 0;
                    if (rvalue is IntegerLiteral) right = ((IntegerLiteral)rvalue).Value;
                    if (rvalue is CharacterLiteral) right = ((CharacterLiteral)rvalue).Value;
                    if (rvalue is BooleanLiteral) right = ((BooleanLiteral)rvalue).Value ? 1 : 0;
                    double calc = left + right;
                    string text = calc.ToString();
                    result = new IntegerLiteral(text, text, "", "");
                }
                else if (rvalue is FloatingLiteral)
                {
                    double right = ((FloatingLiteral)rvalue).Value;
                    double calc = left + right;
                    string text = calc.ToString();
                    result = new FloatingLiteral(text, text, "", "");
                }
                else
                {
                    throw new Exception("nullptrで演算子+は使えない");
                }
            }
            else
            {
                throw new Exception("nullptrで演算子+は使えない");
            }
            return result;
        }

        /// <summary>
        /// 減算
        /// </summary>
        protected Literal Minus(Literal lvalue, Literal rvalue)
        {
            Literal result = null;
            if ((lvalue is IntegerLiteral) || (lvalue is CharacterLiteral) || (lvalue is BooleanLiteral))
            {
                // 左の値
                long left = 0;
                if (lvalue is IntegerLiteral) left = ((IntegerLiteral)lvalue).Value;
                if (lvalue is CharacterLiteral) left = ((CharacterLiteral)lvalue).Value;
                if (lvalue is BooleanLiteral) left = ((BooleanLiteral)lvalue).Value ? 1 : 0;
                // 右の値
                if ((rvalue is IntegerLiteral) || (rvalue is CharacterLiteral) || (rvalue is BooleanLiteral))
                {
                    long right = 0;
                    if (rvalue is IntegerLiteral) right = ((IntegerLiteral)rvalue).Value;
                    if (rvalue is CharacterLiteral) right = ((CharacterLiteral)rvalue).Value;
                    if (rvalue is BooleanLiteral) right = ((BooleanLiteral)rvalue).Value ? 1 : 0;
                    long calc = left - right;
                    string text = calc.ToString();
                    result = new IntegerLiteral(text, text, "", "");
                }
                else if (rvalue is FloatingLiteral)
                {
                    double right = ((FloatingLiteral)rvalue).Value;
                    double calc = left - right;
                    string text = calc.ToString();
                    result = new FloatingLiteral(text, text, "", "");
                }
                else
                {
                    throw new Exception("nullptrで演算子-は使えない");
                }
            }
            else if (lvalue is FloatingLiteral)
            {
                // 左の値
                double left = ((FloatingLiteral)lvalue).Value;
                // 右の値
                if ((rvalue is IntegerLiteral) || (rvalue is CharacterLiteral) || (rvalue is BooleanLiteral))
                {
                    long right = 0;
                    if (rvalue is IntegerLiteral) right = ((IntegerLiteral)rvalue).Value;
                    if (rvalue is CharacterLiteral) right = ((CharacterLiteral)rvalue).Value;
                    if (rvalue is BooleanLiteral) right = ((BooleanLiteral)rvalue).Value ? 1 : 0;
                    double calc = left - right;
                    string text = calc.ToString();
                    result = new IntegerLiteral(text, text, "", "");
                }
                else if (rvalue is FloatingLiteral)
                {
                    double right = ((FloatingLiteral)rvalue).Value;
                    double calc = left - right;
                    string text = calc.ToString();
                    result = new FloatingLiteral(text, text, "", "");
                }
                else
                {
                    throw new Exception("nullptrで演算子-は使えない");
                }
            }
            else
            {
                throw new Exception("nullptrで演算子-は使えない");
            }
            return result;
        }

        /// <summary>
        /// 乗算
        /// </summary>
        protected Literal Mult(Literal lvalue, Literal rvalue)
        {
            Literal result = null;
            if ((lvalue is IntegerLiteral) || (lvalue is CharacterLiteral) || (lvalue is BooleanLiteral))
            {
                // 左の値
                long left = 0;
                if (lvalue is IntegerLiteral) left = ((IntegerLiteral)lvalue).Value;
                if (lvalue is CharacterLiteral) left = ((CharacterLiteral)lvalue).Value;
                if (lvalue is BooleanLiteral) left = ((BooleanLiteral)lvalue).Value ? 1 : 0;
                // 右の値
                if ((rvalue is IntegerLiteral) || (rvalue is CharacterLiteral) || (rvalue is BooleanLiteral))
                {
                    long right = 0;
                    if (rvalue is IntegerLiteral) right = ((IntegerLiteral)rvalue).Value;
                    if (rvalue is CharacterLiteral) right = ((CharacterLiteral)rvalue).Value;
                    if (rvalue is BooleanLiteral) right = ((BooleanLiteral)rvalue).Value ? 1 : 0;
                    long calc = left * right;
                    string text = calc.ToString();
                    result = new IntegerLiteral(text, text, "", "");
                }
                else if (rvalue is FloatingLiteral)
                {
                    double right = ((FloatingLiteral)rvalue).Value;
                    double calc = left * right;
                    string text = calc.ToString();
                    result = new FloatingLiteral(text, text, "", "");
                }
                else
                {
                    throw new Exception("nullptrで演算子*は使えない");
                }
            }
            else if (lvalue is FloatingLiteral)
            {
                // 左の値
                double left = ((FloatingLiteral)lvalue).Value;
                // 右の値
                if ((rvalue is IntegerLiteral) || (rvalue is CharacterLiteral) || (rvalue is BooleanLiteral))
                {
                    long right = 0;
                    if (rvalue is IntegerLiteral) right = ((IntegerLiteral)rvalue).Value;
                    if (rvalue is CharacterLiteral) right = ((CharacterLiteral)rvalue).Value;
                    if (rvalue is BooleanLiteral) right = ((BooleanLiteral)rvalue).Value ? 1 : 0;
                    double calc = left * right;
                    string text = calc.ToString();
                    result = new IntegerLiteral(text, text, "", "");
                }
                else if (rvalue is FloatingLiteral)
                {
                    double right = ((FloatingLiteral)rvalue).Value;
                    double calc = left * right;
                    string text = calc.ToString();
                    result = new FloatingLiteral(text, text, "", "");
                }
                else
                {
                    throw new Exception("nullptrで演算子*は使えない");
                }
            }
            else
            {
                throw new Exception("nullptrで演算子*は使えない");
            }
            return result;
        }

        /// <summary>
        /// 除算
        /// </summary>
        protected Literal Div(Literal lvalue, Literal rvalue)
        {
            Literal result = null;
            if ((lvalue is IntegerLiteral) || (lvalue is CharacterLiteral) || (lvalue is BooleanLiteral))
            {
                // 左の値
                long left = 0;
                if (lvalue is IntegerLiteral) left = ((IntegerLiteral)lvalue).Value;
                if (lvalue is CharacterLiteral) left = ((CharacterLiteral)lvalue).Value;
                if (lvalue is BooleanLiteral) left = ((BooleanLiteral)lvalue).Value ? 1 : 0;
                // 右の値
                if ((rvalue is IntegerLiteral) || (rvalue is CharacterLiteral) || (rvalue is BooleanLiteral))
                {
                    long right = 0;
                    if (rvalue is IntegerLiteral) right = ((IntegerLiteral)rvalue).Value;
                    if (rvalue is CharacterLiteral) right = ((CharacterLiteral)rvalue).Value;
                    if (rvalue is BooleanLiteral) right = ((BooleanLiteral)rvalue).Value ? 1 : 0;
                    long calc = left / right;
                    string text = calc.ToString();
                    result = new IntegerLiteral(text, text, "", "");
                }
                else if (rvalue is FloatingLiteral)
                {
                    double right = ((FloatingLiteral)rvalue).Value;
                    double calc = left / right;
                    string text = calc.ToString();
                    result = new FloatingLiteral(text, text, "", "");
                }
                else
                {
                    throw new Exception("nullptrで演算子/は使えない");
                }
            }
            else if (lvalue is FloatingLiteral)
            {
                // 左の値
                double left = ((FloatingLiteral)lvalue).Value;
                // 右の値
                if ((rvalue is IntegerLiteral) || (rvalue is CharacterLiteral) || (rvalue is BooleanLiteral))
                {
                    long right = 0;
                    if (rvalue is IntegerLiteral) right = ((IntegerLiteral)rvalue).Value;
                    if (rvalue is CharacterLiteral) right = ((CharacterLiteral)rvalue).Value;
                    if (rvalue is BooleanLiteral) right = ((BooleanLiteral)rvalue).Value ? 1 : 0;
                    double calc = left / right;
                    string text = calc.ToString();
                    result = new IntegerLiteral(text, text, "", "");
                }
                else if (rvalue is FloatingLiteral)
                {
                    double right = ((FloatingLiteral)rvalue).Value;
                    double calc = left / right;
                    string text = calc.ToString();
                    result = new FloatingLiteral(text, text, "", "");
                }
                else
                {
                    throw new Exception("nullptrで演算子/は使えない");
                }
            }
            else
            {
                throw new Exception("nullptrで演算子/は使えない");
            }
            return result;
        }

        /// <summary>
        /// 剰余
        /// </summary>
        protected Literal Mod(Literal lvalue, Literal rvalue)
        {
            Literal result = null;
            if ((lvalue is IntegerLiteral) || (lvalue is CharacterLiteral) || (lvalue is BooleanLiteral))
            {
                // 左の値
                long left = 0;
                if (lvalue is IntegerLiteral) left = ((IntegerLiteral)lvalue).Value;
                if (lvalue is CharacterLiteral) left = ((CharacterLiteral)lvalue).Value;
                if (lvalue is BooleanLiteral) left = ((BooleanLiteral)lvalue).Value ? 1 : 0;
                // 右の値
                if ((rvalue is IntegerLiteral) || (rvalue is CharacterLiteral) || (rvalue is BooleanLiteral))
                {
                    long right = 0;
                    if (rvalue is IntegerLiteral) right = ((IntegerLiteral)rvalue).Value;
                    if (rvalue is CharacterLiteral) right = ((CharacterLiteral)rvalue).Value;
                    if (rvalue is BooleanLiteral) right = ((BooleanLiteral)rvalue).Value ? 1 : 0;
                    long calc = left % right;
                    string text = calc.ToString();
                    result = new IntegerLiteral(text, text, "", "");
                }
                else if (rvalue is FloatingLiteral)
                {
                    throw new Exception("浮動小数のリテラルで演算子%は使えない");
                }
                else
                {
                    throw new Exception("nullptrで演算子%は使えない");
                }
            }
            else if (lvalue is FloatingLiteral)
            {
                throw new Exception("浮動小数のリテラルで演算子%は使えない");
            }
            else
            {
                throw new Exception("nullptrで演算子%は使えない");
            }
            return result;
        }

        /// <summary>
        /// 左シフト
        /// </summary>
        protected Literal LShift(Literal lvalue, Literal rvalue)
        {
            Literal result = null;
            if ((lvalue is IntegerLiteral) || (lvalue is CharacterLiteral) || (lvalue is BooleanLiteral))
            {
                // 左の値
                long left = 0;
                if (lvalue is IntegerLiteral) left = ((IntegerLiteral)lvalue).Value;
                if (lvalue is CharacterLiteral) left = ((CharacterLiteral)lvalue).Value;
                if (lvalue is BooleanLiteral) left = ((BooleanLiteral)lvalue).Value ? 1 : 0;
                // 右の値
                if ((rvalue is IntegerLiteral) || (rvalue is CharacterLiteral) || (rvalue is BooleanLiteral))
                {
                    long right = 0;
                    if (rvalue is IntegerLiteral) right = ((IntegerLiteral)rvalue).Value;
                    if (rvalue is CharacterLiteral) right = ((CharacterLiteral)rvalue).Value;
                    if (rvalue is BooleanLiteral) right = ((BooleanLiteral)rvalue).Value ? 1 : 0;
                    long calc = left << (int)right;
                    string text = calc.ToString();
                    result = new IntegerLiteral(text, text, "", "");
                }
                else if (rvalue is FloatingLiteral)
                {
                    throw new Exception("浮動小数のリテラルで演算子<<は使えない");
                }
                else
                {
                    throw new Exception("nullptrで演算子<<は使えない");
                }
            }
            else if (lvalue is FloatingLiteral)
            {
                throw new Exception("浮動小数のリテラルで演算子<<は使えない");
            }
            else
            {
                throw new Exception("nullptrで演算子<<は使えない");
            }
            return result;
        }

        /// <summary>
        /// 右シフト
        /// </summary>
        protected Literal RShift(Literal lvalue, Literal rvalue)
        {
            Literal result = null;
            if ((lvalue is IntegerLiteral) || (lvalue is CharacterLiteral) || (lvalue is BooleanLiteral))
            {
                // 左の値
                long left = 0;
                if (lvalue is IntegerLiteral) left = ((IntegerLiteral)lvalue).Value;
                if (lvalue is CharacterLiteral) left = ((CharacterLiteral)lvalue).Value;
                if (lvalue is BooleanLiteral) left = ((BooleanLiteral)lvalue).Value ? 1 : 0;
                // 右の値
                if ((rvalue is IntegerLiteral) || (rvalue is CharacterLiteral) || (rvalue is BooleanLiteral))
                {
                    long right = 0;
                    if (rvalue is IntegerLiteral) right = ((IntegerLiteral)rvalue).Value;
                    if (rvalue is CharacterLiteral) right = ((CharacterLiteral)rvalue).Value;
                    if (rvalue is BooleanLiteral) right = ((BooleanLiteral)rvalue).Value ? 1 : 0;
                    long calc = left >> (int)right;
                    string text = calc.ToString();
                    result = new IntegerLiteral(text, text, "", "");
                }
                else if (rvalue is FloatingLiteral)
                {
                    throw new Exception("浮動小数のリテラルで演算子>>は使えない");
                }
                else
                {
                    throw new Exception("nullptrで演算子>>は使えない");
                }
            }
            else if (lvalue is FloatingLiteral)
            {
                throw new Exception("浮動小数のリテラルで演算子>>は使えない");
            }
            else
            {
                throw new Exception("nullptrで演算子>>は使えない");
            }
            return result;
        }

        /// <summary>
        /// 小なり
        /// </summary>
        protected Literal Less(Literal lvalue, Literal rvalue)
        {
            Literal result = null;
            if ((lvalue is IntegerLiteral) || (lvalue is CharacterLiteral) || (lvalue is BooleanLiteral) || (lvalue is PointerLiteral))
            {
                // 左の値
                long left = 0;
                if (lvalue is IntegerLiteral) left = ((IntegerLiteral)lvalue).Value;
                if (lvalue is CharacterLiteral) left = ((CharacterLiteral)lvalue).Value;
                if (lvalue is BooleanLiteral) left = ((BooleanLiteral)lvalue).Value ? 1 : 0;
                if (lvalue is PointerLiteral) left = 0;
                // 右の値
                if ((rvalue is IntegerLiteral) || (rvalue is CharacterLiteral) || (rvalue is BooleanLiteral) || (rvalue is PointerLiteral))
                {
                    long right = 0;
                    if (rvalue is IntegerLiteral) right = ((IntegerLiteral)rvalue).Value;
                    if (rvalue is CharacterLiteral) right = ((CharacterLiteral)rvalue).Value;
                    if (rvalue is BooleanLiteral) right = ((BooleanLiteral)rvalue).Value ? 1 : 0;
                    if (rvalue is PointerLiteral) right = 0;
                    long calc = left < right ? 1 : 0;
                    string text = calc.ToString();
                    result = new IntegerLiteral(text, text, "", "");
                }
                else if (rvalue is FloatingLiteral)
                {
                    double right = ((FloatingLiteral)rvalue).Value;
                    long calc = left < right ? 1 : 0;
                    string text = calc.ToString();
                    result = new IntegerLiteral(text, text, "", "");
                }
            }
            else if (lvalue is FloatingLiteral)
            {
                // 左の値
                double left = ((FloatingLiteral)lvalue).Value;
                // 右の値
                if ((rvalue is IntegerLiteral) || (rvalue is CharacterLiteral) || (rvalue is BooleanLiteral) || (rvalue is PointerLiteral))
                {
                    long right = 0;
                    if (rvalue is IntegerLiteral) right = ((IntegerLiteral)rvalue).Value;
                    if (rvalue is CharacterLiteral) right = ((CharacterLiteral)rvalue).Value;
                    if (rvalue is BooleanLiteral) right = ((BooleanLiteral)rvalue).Value ? 1 : 0;
                    if (rvalue is PointerLiteral) right = 0;
                    long calc = left < right ? 1 : 0;
                    string text = calc.ToString();
                    result = new IntegerLiteral(text, text, "", "");
                }
                else if (rvalue is FloatingLiteral)
                {
                    double right = ((FloatingLiteral)rvalue).Value;
                    long calc = left < right ? 1 : 0;
                    string text = calc.ToString();
                    result = new IntegerLiteral(text, text, "", "");
                }
            }
            return result;
        }

        /// <summary>
        /// 大なり
        /// </summary>
        protected Literal Greater(Literal lvalue, Literal rvalue)
        {
            Literal result = null;
            if ((lvalue is IntegerLiteral) || (lvalue is CharacterLiteral) || (lvalue is BooleanLiteral) || (lvalue is PointerLiteral))
            {
                // 左の値
                long left = 0;
                if (lvalue is IntegerLiteral) left = ((IntegerLiteral)lvalue).Value;
                if (lvalue is CharacterLiteral) left = ((CharacterLiteral)lvalue).Value;
                if (lvalue is BooleanLiteral) left = ((BooleanLiteral)lvalue).Value ? 1 : 0;
                if (lvalue is PointerLiteral) left = 0;
                // 右の値
                if ((rvalue is IntegerLiteral) || (rvalue is CharacterLiteral) || (rvalue is BooleanLiteral) || (rvalue is PointerLiteral))
                {
                    long right = 0;
                    if (rvalue is IntegerLiteral) right = ((IntegerLiteral)rvalue).Value;
                    if (rvalue is CharacterLiteral) right = ((CharacterLiteral)rvalue).Value;
                    if (rvalue is BooleanLiteral) right = ((BooleanLiteral)rvalue).Value ? 1 : 0;
                    if (rvalue is PointerLiteral) right = 0;
                    long calc = left > right ? 1 : 0;
                    string text = calc.ToString();
                    result = new IntegerLiteral(text, text, "", "");
                }
                else if (rvalue is FloatingLiteral)
                {
                    double right = ((FloatingLiteral)rvalue).Value;
                    long calc = left > right ? 1 : 0;
                    string text = calc.ToString();
                    result = new IntegerLiteral(text, text, "", "");
                }
            }
            else if (lvalue is FloatingLiteral)
            {
                // 左の値
                double left = ((FloatingLiteral)lvalue).Value;
                // 右の値
                if ((rvalue is IntegerLiteral) || (rvalue is CharacterLiteral) || (rvalue is BooleanLiteral) || (rvalue is PointerLiteral))
                {
                    long right = 0;
                    if (rvalue is IntegerLiteral) right = ((IntegerLiteral)rvalue).Value;
                    if (rvalue is CharacterLiteral) right = ((CharacterLiteral)rvalue).Value;
                    if (rvalue is BooleanLiteral) right = ((BooleanLiteral)rvalue).Value ? 1 : 0;
                    if (rvalue is PointerLiteral) right = 0;
                    long calc = left > right ? 1 : 0;
                    string text = calc.ToString();
                    result = new IntegerLiteral(text, text, "", "");
                }
                else if (rvalue is FloatingLiteral)
                {
                    double right = ((FloatingLiteral)rvalue).Value;
                    long calc = left > right ? 1 : 0;
                    string text = calc.ToString();
                    result = new IntegerLiteral(text, text, "", "");
                }
            }
            return result;
        }

        /// <summary>
        /// 小なりイコール
        /// </summary>
        protected Literal LessEq(Literal lvalue, Literal rvalue)
        {
            Literal result = null;
            if ((lvalue is IntegerLiteral) || (lvalue is CharacterLiteral) || (lvalue is BooleanLiteral) || (lvalue is PointerLiteral))
            {
                // 左の値
                long left = 0;
                if (lvalue is IntegerLiteral) left = ((IntegerLiteral)lvalue).Value;
                if (lvalue is CharacterLiteral) left = ((CharacterLiteral)lvalue).Value;
                if (lvalue is BooleanLiteral) left = ((BooleanLiteral)lvalue).Value ? 1 : 0;
                if (lvalue is PointerLiteral) left = 0;
                // 右の値
                if ((rvalue is IntegerLiteral) || (rvalue is CharacterLiteral) || (rvalue is BooleanLiteral) || (rvalue is PointerLiteral))
                {
                    long right = 0;
                    if (rvalue is IntegerLiteral) right = ((IntegerLiteral)rvalue).Value;
                    if (rvalue is CharacterLiteral) right = ((CharacterLiteral)rvalue).Value;
                    if (rvalue is BooleanLiteral) right = ((BooleanLiteral)rvalue).Value ? 1 : 0;
                    if (rvalue is PointerLiteral) right = 0;
                    long calc = left <= right ? 1 : 0;
                    string text = calc.ToString();
                    result = new IntegerLiteral(text, text, "", "");
                }
                else if (rvalue is FloatingLiteral)
                {
                    double right = ((FloatingLiteral)rvalue).Value;
                    long calc = left <= right ? 1 : 0;
                    string text = calc.ToString();
                    result = new IntegerLiteral(text, text, "", "");
                }
            }
            else if (lvalue is FloatingLiteral)
            {
                // 左の値
                double left = ((FloatingLiteral)lvalue).Value;
                // 右の値
                if ((rvalue is IntegerLiteral) || (rvalue is CharacterLiteral) || (rvalue is BooleanLiteral) || (rvalue is PointerLiteral))
                {
                    long right = 0;
                    if (rvalue is IntegerLiteral) right = ((IntegerLiteral)rvalue).Value;
                    if (rvalue is CharacterLiteral) right = ((CharacterLiteral)rvalue).Value;
                    if (rvalue is BooleanLiteral) right = ((BooleanLiteral)rvalue).Value ? 1 : 0;
                    if (rvalue is PointerLiteral) right = 0;
                    long calc = left <= right ? 1 : 0;
                    string text = calc.ToString();
                    result = new IntegerLiteral(text, text, "", "");
                }
                else if (rvalue is FloatingLiteral)
                {
                    double right = ((FloatingLiteral)rvalue).Value;
                    long calc = left <= right ? 1 : 0;
                    string text = calc.ToString();
                    result = new IntegerLiteral(text, text, "", "");
                }
            }
            return result;
        }

        /// <summary>
        /// 大なりイコール
        /// </summary>
        protected Literal GreaterEq(Literal lvalue, Literal rvalue)
        {
            Literal result = null;
            if ((lvalue is IntegerLiteral) || (lvalue is CharacterLiteral) || (lvalue is BooleanLiteral) || (lvalue is PointerLiteral))
            {
                // 左の値
                long left = 0;
                if (lvalue is IntegerLiteral) left = ((IntegerLiteral)lvalue).Value;
                if (lvalue is CharacterLiteral) left = ((CharacterLiteral)lvalue).Value;
                if (lvalue is BooleanLiteral) left = ((BooleanLiteral)lvalue).Value ? 1 : 0;
                if (lvalue is PointerLiteral) left = 0;
                // 右の値
                if ((rvalue is IntegerLiteral) || (rvalue is CharacterLiteral) || (rvalue is BooleanLiteral) || (rvalue is PointerLiteral))
                {
                    long right = 0;
                    if (rvalue is IntegerLiteral) right = ((IntegerLiteral)rvalue).Value;
                    if (rvalue is CharacterLiteral) right = ((CharacterLiteral)rvalue).Value;
                    if (rvalue is BooleanLiteral) right = ((BooleanLiteral)rvalue).Value ? 1 : 0;
                    if (rvalue is PointerLiteral) right = 0;
                    long calc = left >= right ? 1 : 0;
                    string text = calc.ToString();
                    result = new IntegerLiteral(text, text, "", "");
                }
                else if (rvalue is FloatingLiteral)
                {
                    double right = ((FloatingLiteral)rvalue).Value;
                    long calc = left >= right ? 1 : 0;
                    string text = calc.ToString();
                    result = new IntegerLiteral(text, text, "", "");
                }
            }
            else if (lvalue is FloatingLiteral)
            {
                // 左の値
                double left = ((FloatingLiteral)lvalue).Value;
                // 右の値
                if ((rvalue is IntegerLiteral) || (rvalue is CharacterLiteral) || (rvalue is BooleanLiteral) || (rvalue is PointerLiteral))
                {
                    long right = 0;
                    if (rvalue is IntegerLiteral) right = ((IntegerLiteral)rvalue).Value;
                    if (rvalue is CharacterLiteral) right = ((CharacterLiteral)rvalue).Value;
                    if (rvalue is BooleanLiteral) right = ((BooleanLiteral)rvalue).Value ? 1 : 0;
                    if (rvalue is PointerLiteral) right = 0;
                    long calc = left >= right ? 1 : 0;
                    string text = calc.ToString();
                    result = new IntegerLiteral(text, text, "", "");
                }
                else if (rvalue is FloatingLiteral)
                {
                    double right = ((FloatingLiteral)rvalue).Value;
                    long calc = left >= right ? 1 : 0;
                    string text = calc.ToString();
                    result = new IntegerLiteral(text, text, "", "");
                }
            }
            return result;
        }

        /// <summary>
        /// 一致
        /// </summary>
        protected Literal Equal(Literal lvalue, Literal rvalue)
        {
            Literal result = null;
            if ((lvalue is IntegerLiteral) || (lvalue is CharacterLiteral) || (lvalue is BooleanLiteral) || (lvalue is PointerLiteral))
            {
                // 左の値
                long left = 0;
                if (lvalue is IntegerLiteral) left = ((IntegerLiteral)lvalue).Value;
                if (lvalue is CharacterLiteral) left = ((CharacterLiteral)lvalue).Value;
                if (lvalue is BooleanLiteral) left = ((BooleanLiteral)lvalue).Value ? 1 : 0;
                if (lvalue is PointerLiteral) left = 0;
                // 右の値
                if ((rvalue is IntegerLiteral) || (rvalue is CharacterLiteral) || (rvalue is BooleanLiteral) || (rvalue is PointerLiteral))
                {
                    long right = 0;
                    if (rvalue is IntegerLiteral) right = ((IntegerLiteral)rvalue).Value;
                    if (rvalue is CharacterLiteral) right = ((CharacterLiteral)rvalue).Value;
                    if (rvalue is BooleanLiteral) right = ((BooleanLiteral)rvalue).Value ? 1 : 0;
                    if (rvalue is PointerLiteral) right = 0;
                    long calc = left == right ? 1 : 0;
                    string text = calc.ToString();
                    result = new IntegerLiteral(text, text, "", "");
                }
                else if (rvalue is FloatingLiteral)
                {
                    double right = ((FloatingLiteral)rvalue).Value;
                    long calc = left == right ? 1 : 0;
                    string text = calc.ToString();
                    result = new IntegerLiteral(text, text, "", "");
                }
            }
            else if (lvalue is FloatingLiteral)
            {
                // 左の値
                double left = ((FloatingLiteral)lvalue).Value;
                // 右の値
                if ((rvalue is IntegerLiteral) || (rvalue is CharacterLiteral) || (rvalue is BooleanLiteral) || (rvalue is PointerLiteral))
                {
                    long right = 0;
                    if (rvalue is IntegerLiteral) right = ((IntegerLiteral)rvalue).Value;
                    if (rvalue is CharacterLiteral) right = ((CharacterLiteral)rvalue).Value;
                    if (rvalue is BooleanLiteral) right = ((BooleanLiteral)rvalue).Value ? 1 : 0;
                    if (rvalue is PointerLiteral) right = 0;
                    long calc = left == right ? 1 : 0;
                    string text = calc.ToString();
                    result = new IntegerLiteral(text, text, "", "");
                }
                else if (rvalue is FloatingLiteral)
                {
                    double right = ((FloatingLiteral)rvalue).Value;
                    long calc = left == right ? 1 : 0;
                    string text = calc.ToString();
                    result = new IntegerLiteral(text, text, "", "");
                }
            }
            return result;
        }

        /// <summary>
        /// 一致しない
        /// </summary>
        protected Literal NotEqual(Literal lvalue, Literal rvalue)
        {
            Literal result = null;
            if ((lvalue is IntegerLiteral) || (lvalue is CharacterLiteral) || (lvalue is BooleanLiteral) || (lvalue is PointerLiteral))
            {
                // 左の値
                long left = 0;
                if (lvalue is IntegerLiteral) left = ((IntegerLiteral)lvalue).Value;
                if (lvalue is CharacterLiteral) left = ((CharacterLiteral)lvalue).Value;
                if (lvalue is BooleanLiteral) left = ((BooleanLiteral)lvalue).Value ? 1 : 0;
                if (lvalue is PointerLiteral) left = 0;
                // 右の値
                if ((rvalue is IntegerLiteral) || (rvalue is CharacterLiteral) || (rvalue is BooleanLiteral) || (rvalue is PointerLiteral))
                {
                    long right = 0;
                    if (rvalue is IntegerLiteral) right = ((IntegerLiteral)rvalue).Value;
                    if (rvalue is CharacterLiteral) right = ((CharacterLiteral)rvalue).Value;
                    if (rvalue is BooleanLiteral) right = ((BooleanLiteral)rvalue).Value ? 1 : 0;
                    if (rvalue is PointerLiteral) right = 0;
                    long calc = left != right ? 1 : 0;
                    string text = calc.ToString();
                    result = new IntegerLiteral(text, text, "", "");
                }
                else if (rvalue is FloatingLiteral)
                {
                    double right = ((FloatingLiteral)rvalue).Value;
                    long calc = left != right ? 1 : 0;
                    string text = calc.ToString();
                    result = new IntegerLiteral(text, text, "", "");
                }
            }
            else if (lvalue is FloatingLiteral)
            {
                // 左の値
                double left = ((FloatingLiteral)lvalue).Value;
                // 右の値
                if ((rvalue is IntegerLiteral) || (rvalue is CharacterLiteral) || (rvalue is BooleanLiteral) || (rvalue is PointerLiteral))
                {
                    long right = 0;
                    if (rvalue is IntegerLiteral) right = ((IntegerLiteral)rvalue).Value;
                    if (rvalue is CharacterLiteral) right = ((CharacterLiteral)rvalue).Value;
                    if (rvalue is BooleanLiteral) right = ((BooleanLiteral)rvalue).Value ? 1 : 0;
                    if (rvalue is PointerLiteral) right = 0;
                    long calc = left != right ? 1 : 0;
                    string text = calc.ToString();
                    result = new IntegerLiteral(text, text, "", "");
                }
                else if (rvalue is FloatingLiteral)
                {
                    double right = ((FloatingLiteral)rvalue).Value;
                    long calc = left != right ? 1 : 0;
                    string text = calc.ToString();
                    result = new IntegerLiteral(text, text, "", "");
                }
            }
            return result;
        }

        /// <summary>
        /// and
        /// </summary>
        protected Literal And(Literal lvalue, Literal rvalue)
        {
            Literal result = null;
            if ((lvalue is IntegerLiteral) || (lvalue is CharacterLiteral) || (lvalue is BooleanLiteral))
            {
                // 左の値
                long left = 0;
                if (lvalue is IntegerLiteral) left = ((IntegerLiteral)lvalue).Value;
                if (lvalue is CharacterLiteral) left = ((CharacterLiteral)lvalue).Value;
                if (lvalue is BooleanLiteral) left = ((BooleanLiteral)lvalue).Value ? 1 : 0;
                // 右の値
                if ((rvalue is IntegerLiteral) || (rvalue is CharacterLiteral) || (rvalue is BooleanLiteral))
                {
                    long right = 0;
                    if (rvalue is IntegerLiteral) right = ((IntegerLiteral)rvalue).Value;
                    if (rvalue is CharacterLiteral) right = ((CharacterLiteral)rvalue).Value;
                    if (rvalue is BooleanLiteral) right = ((BooleanLiteral)rvalue).Value ? 1 : 0;
                    long calc = left & right;
                    string text = calc.ToString();
                    result = new IntegerLiteral(text, text, "", "");
                }
                else if (rvalue is FloatingLiteral)
                {
                    throw new Exception("浮動小数のリテラルで演算子&は使えない");
                }
                else
                {
                    throw new Exception("nullptrで演算子&は使えない");
                }
            }
            else if (lvalue is FloatingLiteral)
            {
                throw new Exception("浮動小数のリテラルで演算子&は使えない");
            }
            else
            {
                throw new Exception("nullptrで演算子&は使えない");
            }
            return result;
        }

        /// <summary>
        /// xor
        /// </summary>
        protected Literal XOr(Literal lvalue, Literal rvalue)
        {
            Literal result = null;
            if ((lvalue is IntegerLiteral) || (lvalue is CharacterLiteral) || (lvalue is BooleanLiteral))
            {
                // 左の値
                long left = 0;
                if (lvalue is IntegerLiteral) left = ((IntegerLiteral)lvalue).Value;
                if (lvalue is CharacterLiteral) left = ((CharacterLiteral)lvalue).Value;
                if (lvalue is BooleanLiteral) left = ((BooleanLiteral)lvalue).Value ? 1 : 0;
                // 右の値
                if ((rvalue is IntegerLiteral) || (rvalue is CharacterLiteral) || (rvalue is BooleanLiteral))
                {
                    long right = 0;
                    if (rvalue is IntegerLiteral) right = ((IntegerLiteral)rvalue).Value;
                    if (rvalue is CharacterLiteral) right = ((CharacterLiteral)rvalue).Value;
                    if (rvalue is BooleanLiteral) right = ((BooleanLiteral)rvalue).Value ? 1 : 0;
                    long calc = left ^ right;
                    string text = calc.ToString();
                    result = new IntegerLiteral(text, text, "", "");
                }
                else if (rvalue is FloatingLiteral)
                {
                    throw new Exception("浮動小数のリテラルで演算子^は使えない");
                }
                else
                {
                    throw new Exception("nullptrで演算子^は使えない");
                }
            }
            else if (lvalue is FloatingLiteral)
            {
                throw new Exception("浮動小数のリテラルで演算子^は使えない");
            }
            else
            {
                throw new Exception("nullptrで演算子^は使えない");
            }
            return result;
        }

        /// <summary>
        /// or
        /// </summary>
        protected Literal Or(Literal lvalue, Literal rvalue)
        {
            Literal result = null;
            if ((lvalue is IntegerLiteral) || (lvalue is CharacterLiteral) || (lvalue is BooleanLiteral))
            {
                // 左の値
                long left = 0;
                if (lvalue is IntegerLiteral) left = ((IntegerLiteral)lvalue).Value;
                if (lvalue is CharacterLiteral) left = ((CharacterLiteral)lvalue).Value;
                if (lvalue is BooleanLiteral) left = ((BooleanLiteral)lvalue).Value ? 1 : 0;
                // 右の値
                if ((rvalue is IntegerLiteral) || (rvalue is CharacterLiteral) || (rvalue is BooleanLiteral))
                {
                    long right = 0;
                    if (rvalue is IntegerLiteral) right = ((IntegerLiteral)rvalue).Value;
                    if (rvalue is CharacterLiteral) right = ((CharacterLiteral)rvalue).Value;
                    if (rvalue is BooleanLiteral) right = ((BooleanLiteral)rvalue).Value ? 1 : 0;
                    long calc = left | right;
                    string text = calc.ToString();
                    result = new IntegerLiteral(text, text, "", "");
                }
                else if (rvalue is FloatingLiteral)
                {
                    throw new Exception("浮動小数のリテラルで演算子|は使えない");
                }
                else
                {
                    throw new Exception("nullptrで演算子|は使えない");
                }
            }
            else if (lvalue is FloatingLiteral)
            {
                throw new Exception("浮動小数のリテラルで演算子|は使えない");
            }
            else
            {
                throw new Exception("nullptrで演算子|は使えない");
            }
            return result;
        }

        /// <summary>
        /// 論理積
        /// </summary>
        protected Literal AndAnd(Literal lvalue, Literal rvalue)
        {
            bool left;
            bool right;
            // 左の値
            if (lvalue is IntegerLiteral) left = ((IntegerLiteral)lvalue).Value != 0;
            else if (lvalue is CharacterLiteral) left = ((CharacterLiteral)lvalue).Value != 0;
            else if (lvalue is BooleanLiteral) left = ((BooleanLiteral)lvalue).Value;
            else if (lvalue is PointerLiteral) left = false;
            else left = ((FloatingLiteral)lvalue).Value != 0;
            // 右の値
            if (rvalue is IntegerLiteral) right = ((IntegerLiteral)rvalue).Value != 0;
            else if (rvalue is CharacterLiteral) right = ((CharacterLiteral)rvalue).Value != 0;
            else if (rvalue is BooleanLiteral) right = ((BooleanLiteral)rvalue).Value;
            else if (rvalue is PointerLiteral) right = false;
            else right = ((FloatingLiteral)rvalue).Value != 0;
            // 結果
            Literal result = null;
            if (left && right)
            {
                result = new IntegerLiteral("1", "1", "", "");
            }
            else
            {
                result = new IntegerLiteral("0", "0", "", "");
            }
            return result;
        }

        /// <summary>
        /// 論理和
        /// </summary>
        protected Literal OrOr(Literal lvalue, Literal rvalue)
        {
            bool left;
            bool right;
            // 左の値
            if (lvalue is IntegerLiteral) left = ((IntegerLiteral)lvalue).Value != 0;
            else if (lvalue is CharacterLiteral) left = ((CharacterLiteral)lvalue).Value != 0;
            else if (lvalue is BooleanLiteral) left = ((BooleanLiteral)lvalue).Value;
            else if (lvalue is PointerLiteral) left = false;
            else left = ((FloatingLiteral)lvalue).Value != 0;
            // 右の値
            if (rvalue is IntegerLiteral) right = ((IntegerLiteral)rvalue).Value != 0;
            else if (rvalue is CharacterLiteral) right = ((CharacterLiteral)rvalue).Value != 0;
            else if (rvalue is BooleanLiteral) right = ((BooleanLiteral)rvalue).Value;
            else if (rvalue is PointerLiteral) right = false;
            else right = ((FloatingLiteral)rvalue).Value != 0;
            // 結果
            Literal result = null;
            if (left || right)
            {
                result = new IntegerLiteral("1", "1", "", "");
            }
            else
            {
                result = new IntegerLiteral("0", "0", "", "");
            }
            return result;
        }

        /// <summary>
        /// 単項演算子のマイナス
        /// </summary>
        protected Literal UnaryMinus(Literal rvalue)
        {
            IntegerLiteral lvalue = new IntegerLiteral("0", "0", "", "");
            Literal result = Minus(lvalue, rvalue);
            return result;
        }

        /// <summary>
        /// 否定
        /// </summary>
        protected Literal Not(Literal rvalue)
        {
            bool right;
            if (rvalue is IntegerLiteral) right = ((IntegerLiteral)rvalue).Value != 0;
            else if (rvalue is CharacterLiteral) right = ((CharacterLiteral)rvalue).Value != 0;
            else if (rvalue is BooleanLiteral) right = ((BooleanLiteral)rvalue).Value;
            else if (rvalue is PointerLiteral) right = false;
            else right = ((FloatingLiteral)rvalue).Value != 0;
            IntegerLiteral result = null;
            if (right)
            {
                result = new IntegerLiteral("0", "0", "", "");
            }
            else
            {
                result = new IntegerLiteral("1", "1", "", "");
            }
            return result;
        }

        #endregion

        #region 型
        /// <summary>
        /// 演算子と左の式
        /// </summary>
        public class OperatorAndRightExpression
        {
            public string Operator { get; protected set; }
            public IfDirectiveExpression RightExpression { get; protected set; }
            public OperatorAndRightExpression(string op, IfDirectiveExpression rightExpression)
            {
                Operator = op;
                RightExpression = rightExpression;
            }
        }

        #endregion

    }

    /// <summary>
    /// ifディレクティブの式の末端
    /// </summary>
    [Serializable]
    class Leaf_IfDirectiveExpression : IfDirectiveExpression
    {
        #region プロパティ
        /// <summary>
        /// リテラル
        /// </summary>
        public Literal Literal { get; protected set; }

        #endregion

        #region コンストラクタ
        /// <summary>
        /// ifディレクティブの式の末端
        /// </summary>
        /// <param name="literal">リテラル</param>
        /// <param name="unaryOperators">単項演算子</param>
        public Leaf_IfDirectiveExpression(Literal literal, IEnumerable<string> unaryOperators)
            : base(new string[0], new IfDirectiveExpression[0], unaryOperators)
        {
            Literal = literal;
        }

        #endregion

        #region メソッド
        /// <summary>
        /// 式の評価
        /// </summary>
        /// <returns>評価結果</returns>
        public override Literal Evaluate()
        {
            Literal result = Literal;
            foreach (var op in UnaryOperators)
            {
                switch (op)
                {
                    case "-":
                        result = UnaryMinus(result);
                        break;
                    case "!":
                        result = Not(result);
                        break;
                }
            }

            return result;
        }

        #endregion

    }

}
