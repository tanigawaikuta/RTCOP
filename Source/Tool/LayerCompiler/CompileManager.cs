using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using LayerCompiler.CodeGeneration;
using LayerCompiler.CodeGeneration.Model;

namespace LayerCompiler
{
    /// <summary>
    /// コンパイルマネージャ。
    /// </summary>
    class CompileManager
    {
        #region フィールド
        /// <summary>
        /// RTCOPプリプロセッサ
        /// </summary>
        private RTCOPPreprocessor _RTCOPPreprocessor;

        /// <summary>
        /// RTCOPコンパイラ
        /// </summary>
        private RTCOPCompiler _RTCOPCompiler;

        /// <summary>
        /// コードジェネレータ
        /// </summary>
        private RTCOPCodeGenerator _RTCOPCodeGenerator;

        #endregion

        #region プロパティ
        /// <summary>
        /// 入力ファイル
        /// .lobjファイルか.lcppファイルを指定
        /// </summary>
        public List<string> SourceFiles { get; protected set; } = new List<string>();

        /// <summary>
        /// 出力ファイル。.lobjファイルかディレクトリパスを指定する
        /// ディレクトリパスの場合、その場所にC++ソースコードを出力する
        /// </summary>
        public string OutputFile { get; protected set; } = "./";

        /// <summary>
        /// ヘッダファイルのディレクトリパス
        /// </summary>
        public List<string> IncludePaths { get; protected set; } = new List<string>() { "./" };

        /// <summary>
        /// ソースファイルのルートパス
        /// </summary>
        public string SourceRootPath { get; protected set; } = "./";

        /// <summary>
        /// レイヤクラスのデフォルトの名前空間 (現時点では変更不可)
        /// </summary>
        public string Namespace { get; protected set; } = "RTCOP::Generated";

        /// <summary>
        /// レイヤコンパイラに与えるマクロ定義
        /// </summary>
        public List<string> Macros { get; protected set; } = new List<string>();

        /// <summary>
        /// 入出力ファイルの文字コード
        /// </summary>
        public Encoding Encoding { get; protected set; } = new UTF8Encoding(false);

        /// <summary>
        /// 改行文字
        /// </summary>
        public string LineTerminator { get; protected set; } = "\r\n";

        /// <summary>
        /// 開発対象
        /// </summary>
        public DevelopmentTarget Target { get; protected set; } = DevelopmentTarget.None;

        /// <summary>
        /// 開発環境
        /// </summary>
        public DevelopmentEnvironment Environment { get; protected set; } = DevelopmentEnvironment.None;

        #endregion

        #region コンストラクタ
        /// <summary>
        /// コンパイルマネージャ。
        /// </summary>
        /// <param name="args">コマンドライン引数。</param>
        public CompileManager(string[] args)
        {
            Namespace = Properties.Settings.Default.NameSpace;
            string newLine = Properties.Settings.Default.LineTerminator.ToLower();
            if ((newLine == "\\r") || (newLine == "cr"))
            {
                LineTerminator = "\r";
            }
            else if ((newLine == "\\n") || (newLine == "lf"))
            {
                LineTerminator = "\n";
            }
            else
            {
                LineTerminator = "\r\n";
            }
            string encoding = Properties.Settings.Default.Encoding;
            if (encoding.ToLower() == "utf-8n")
            {
                Encoding = new UTF8Encoding(false);
            }
            else
            {
                Encoding enc = Encoding;
                try { enc = Encoding.GetEncoding(encoding); } catch (Exception) { }
                Encoding = enc;
            }
            // コンパイルオプションのチェック
            CheckCompileOptions(args);

            // プリプロセッサ・コンパイラの生成
            _RTCOPPreprocessor = new RTCOPPreprocessor(Macros, IncludePaths, Encoding);
            _RTCOPCompiler = new RTCOPCompiler();
            _RTCOPCodeGenerator = new RTCOPCodeGenerator(Namespace, LineTerminator, Target, Environment, IncludePaths);
        }

        #endregion

        #region コンパイルオプション関連
        /// <summary>
        /// コンパイルオプションのチェック。
        /// </summary>
        /// <param name="args"></param>
        private void CheckCompileOptions(string[] args)
        {
            // オプションの読み取り
            Func<string, string> getPath = (s) =>
            {
                string result = s;
                if (!(s.EndsWith("/") || s.EndsWith("\\")))
                {
                    result = s + "/";
                }
                return result;
            };
            int offset = 1;
            for (int i = 0; i < args.Length; i += offset)
            {
                string arg = args[i];
                switch (arg)
                {
                    // 出力ファイル
                    case "-o":
                    case "-O":
                        if ((i + 1) < args.Length)
                        {
                            OutputFile = args[i + 1];
                        }
                        offset = 2;
                        break;
                    // ヘッダファイルのディレクトリパス
                    case "-i":
                    case "-I":
                        if ((i + 1) < args.Length)
                        {
                            string arg2 = args[i + 1];
                            string[] includes = arg2.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string inc in includes)
                            {
                                IncludePaths.Add(getPath(inc));
                            }
                        }
                        offset = 2;
                        break;
                    // マクロ定義
                    case "-d":
                    case "-D":
                        if ((i + 1) < args.Length)
                        {
                            string arg2 = args[i + 1];
                            string[] macros = arg2.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string macro in macros)
                            {
                                Macros.Add(macro);
                            }
                        }
                        offset = 2;
                        break;
                    // 開発対象
                    case "-t":
                    case "-T":
                        if ((i + 1) < args.Length)
                        {
                            string arg2 = args[i + 1].ToLower();
                            switch (arg2)
                            {
                                case "win32":
                                case "win_32":
                                case "windows32":
                                case "windows_32":
                                case "winx86":
                                case "win_x86":
                                case "windowsx86":
                                case "windows_x86":
                                case "x86win":
                                case "x86_win":
                                case "x86windows":
                                case "x86_windows":
                                    Target = DevelopmentTarget.WindowsX86;
                                    break;
                                case "win64":
                                case "win_64":
                                case "windows64":
                                case "windows_64":
                                case "winx64":
                                case "win_x64":
                                case "windowsx64":
                                case "windows_x64":
                                case "x64win":
                                case "x64_win":
                                case "x64windows":
                                case "x64_windows":
                                    Target = DevelopmentTarget.WindowsX64;
                                    break;
                                case "linux32":
                                case "linux_32":
                                case "linuxx86":
                                case "linux_x86":
                                case "x86linux":
                                case "x86_linux":
                                    Target = DevelopmentTarget.LinuxX86;
                                    break;
                                case "linux64":
                                case "linux_64":
                                case "linuxx64":
                                case "linux_x64":
                                case "x64linux":
                                case "x64_linux":
                                    Target = DevelopmentTarget.LinuxX64;
                                    break;
                                case "linuxarm":
                                case "linux_arm":
                                case "armlinux":
                                case "arm_linux":
                                    Target = DevelopmentTarget.LinuxARM;
                                    break;
                                case "linuxarm64":
                                case "linux_arm64":
                                case "arm64linux":
                                case "arm64_linux":
                                    Target = DevelopmentTarget.LinuxARM64;
                                    break;
                                case "mac":
                                case "macos":
                                case "mac64":
                                case "macos64":
                                case "mac_64":
                                case "macos_64":
                                case "macx64":
                                case "macosx64":
                                case "mac_x64":
                                case "macos_x64":
                                case "x64mac":
                                case "x64macos":
                                case "x64_mac":
                                case "x64_macos":
                                    Target = DevelopmentTarget.macOSX64;
                                    break;
                                default:
                                    break;
                            }
                        }
                        offset = 2;
                        break;
                    // 開発環境
                    case "-e":
                    case "-E":
                        if ((i + 1) < args.Length)
                        {
                            string arg2 = args[i + 1].ToLower();
                            switch (arg2)
                            {
                                case "vs":
                                case "visualstudio":
                                case "visual_studio":
                                case "vc":
                                case "vc++":
                                case "visualc":
                                case "visualc++":
                                case "visual_c":
                                case "visual_c++":
                                    Environment = DevelopmentEnvironment.VisualStudio;
                                    break;
                                case "gcc":
                                case "g++":
                                case "mingw":
                                    Environment = DevelopmentEnvironment.GCC;
                                    break;
                                case "clang":
                                case "llvm":
                                    Environment = DevelopmentEnvironment.Clang;
                                    break;
                                default:
                                    break;
                            }
                        }
                        offset = 2;
                        break;
                    // ソースのルートフォルダ
                    case "-r":
                    case "-R":
                        if ((i + 1) < args.Length)
                        {
                            string arg2 = args[i + 1];
                            SourceRootPath = getPath(arg2);
                        }
                        offset = 2;
                        break;
                    // ヘルプ
                    case "--help":
                        break;
                    // バージョン
                    case "--virsion":
                        break;
                    // 入力ファイル
                    default:
                        // 未知のオプションでないかチェック
                        if (arg[0] != '-')
                        {
                            // 入力ファイルの追加
                            SourceFiles.Add(arg);
                        }
                        offset = 1;
                        break;
                }
            }
        }

        #endregion

        #region コンパイル関連のメソッド
        /// <summary>
        /// コンパイル実行。
        /// </summary>
        public void Compile()
        {
            // ソースファイルが無い場合
            if (SourceFiles.Count <= 0)
            {
                // エラー
                throw new Exception("ソースファイルが1つも無いです。");
            }

            // ソースファイルの読み込み
            List<LayerStructureFile> lstrs = new List<LayerStructureFile>();
            foreach (string fileName in SourceFiles)
            {
                if (Path.GetExtension(fileName) == ".lcpp")
                {
                    // .lcppをコンパイル
                    LayerStructureFile result = CompileLCppToObjectFile(fileName);
                    lstrs.Add(result);
                }
                else
                {
                    // レイヤ構造ファイルを読み込み
                    LayerStructureFile result = LayerStructureFile.LoadFile(fileName);
                    lstrs.Add(result);
                }
            }
            // レイヤ構造ファイルのマージ
            LayerStructureFile mergedLSFile = _RTCOPCompiler.MergeObjectFiles(lstrs);
            // 出力ファイルがC++のソースコードか、レイヤ構造ファイルかを確認
            bool outputIsCpp = OutputFile.EndsWith("/") || OutputFile.EndsWith("\\");
            if (!outputIsCpp && (Path.GetExtension(OutputFile) == ""))
            {
                // 拡張子無しの場合フォルダと判断
                outputIsCpp = true;
                OutputFile += "/";
            }
            // ソースコード出力
            if (outputIsCpp)
            {
                GenerateCode(mergedLSFile);
            }
            // レイヤ構造出力
            else
            {
                LayerStructureFile.SaveFile(OutputFile, mergedLSFile);
            }
        }

        /// <summary>
        /// .lcppからレイヤ構造ファイルにコンパイル。
        /// </summary>
        /// <param name="fileName">ファイル名。</param>
        /// <returns>コンパイル結果。</returns>
        private LayerStructureFile CompileLCppToObjectFile(string fileName)
        {
            // ファイルチェック
            string fileName2 = fileName;
            if (!File.Exists(fileName2))
            {
                fileName2 = SourceRootPath + fileName;
                if (!File.Exists(fileName2))
                {
                    throw new FileNotFoundException(fileName + " が見つかりません", fileName);
                }
            }
            // ファイルオープン
            RTCOPSourceFile src = null;
            using (StreamReader sr = new StreamReader(fileName2, Encoding))
            {
                string text = sr.ReadToEnd();
                src = new RTCOPSourceFile(fileName2, text);
            }
            // プリプロセス
            var src2 = _RTCOPPreprocessor.Run(src);
            // コンパイル
            var result = _RTCOPCompiler.Compile(src2);
            return result;
        }

        /// <summary>
        /// コード生成
        /// </summary>
        /// <param name="mergedLSFile">マージ済みのレイヤ構造ファイル</param>
        private void GenerateCode(LayerStructureFile mergedLSFile)
        {
            // ソースコードディレクトリのルートパスを取得
            string includeFilePath = OutputFile;
            if (SourceRootPath != "/")
            {
                string generateCodePath = Path.GetFullPath(OutputFile);
                string sourceRootPath = Path.GetFullPath(SourceRootPath);
                if (generateCodePath.StartsWith(sourceRootPath))
                {
                    includeFilePath = generateCodePath.Substring(sourceRootPath.Length);
                    includeFilePath = includeFilePath.Replace('\\', '/');
                    includeFilePath = "./" + includeFilePath;
                }
            }
            // コードジェネレート
            var result = _RTCOPCodeGenerator.GenerateCode(mergedLSFile, includeFilePath);
            foreach (string filename in result.CodeDictionary.Keys)
            {
                string filepath = OutputFile + filename;
                string dirpath = Path.GetDirectoryName(filepath);
                if (!Directory.Exists(dirpath))
                {
                    Directory.CreateDirectory(dirpath);
                }
                using (StreamWriter sw = new StreamWriter(filepath, false, Encoding))
                {
                    sw.Write(result.CodeDictionary[filename]);
                }
            }
        }

        #endregion

    }
}
