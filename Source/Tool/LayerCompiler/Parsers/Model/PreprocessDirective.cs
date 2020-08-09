using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayerCompiler.Parsers.Model
{
    /// <summary>
    /// プリプロセッサへの指令
    /// </summary>
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
        public string Param2 { get; protected set; }

        #endregion

        #region コンストラクタ
        /// <summary>
        /// プリプロセッサへの指令
        /// </summary>
        /// <param name="kind">種類</param>
        /// <param name="param1">パラメータ1 マクロの名前など</param>
        /// <param name="param2">パラメータ2 マクロの値など</param>
        public PreprocessDirective(
            DirectiveKind kind = DirectiveKind.None, string param1 = "", string param2 = "")
        {
            Kind = kind;
            param1 = param1.Replace("\\\n", "");
            param1 = param1.Replace("\\\r\n", "");
            Param1 = param1;
            param2 = param2.Replace("\\\n", "");
            param2 = param2.Replace("\\\r\n", "");
            Param2 = param2;
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
                    if (Param2 != "")
                    {
                        result += (" " + Param2);
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
                    if (Param2 != "")
                    {
                        result += (" \"" + Param2 + "\"");
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
                    if (Param2 != "")
                    {
                        result += (" " + Param2);
                    }
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
    }
}
