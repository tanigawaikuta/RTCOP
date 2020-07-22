using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Sprache;

using LayerCompiler.Model;

namespace LayerCompiler.Parsers
{
    /// <summary>
    /// RTCOPのパーサ
    /// </summary>
    static class RTCOPParser
    {
        #region 読み飛ばし
        /// <summary>
        /// 無視するトークン
        /// </summary>
        public static readonly Parser<Token> ToIgnoringToken =
                                                    from token in TokenParser.Token
                                                    where !((token is Identifier) && (TokenParser.RTCOPKeywordStrings.Contains(token.Text)))    // RTCOPのキーワードと一致しない
                                                    where !((token is Keyword) && (token.Text == "#" || token.Text == "##"))                    // #か##ではない
                                                    select token;

        /// <summary>
        /// 無視するディレクティブ
        /// </summary>
        public static readonly Parser<PreprocessDirective> ToIgnoringDirective =
                                                    from directive in PreprocessParser.Directive
                                                    select directive;

        /// <summary>
        /// 無視するトークンまたはディレクティブ
        /// </summary>
        public static readonly Parser<object> ToIgnoringTokenAndDirective =
                                                    PreprocessParser.Directive
                                                    .Or<object>(ToIgnoringToken);

        #endregion

        #region レイヤ
        /// <summary>
        /// レイヤ定義
        /// </summary>
        public static readonly Parser<LayerDefinition> LayerDefinition =
                                                    from keyword in Parse.String("layer").Text().TokenWithSkipComment()
                                                    from identifier in TokenParser.RTCOPIdentifierString.TokenWithSkipComment()
                                                    from beginblock in Parse.String("{").Text().TokenWithSkipComment()
                                                        /*中身*/
                                                    from endblock in Parse.String("}").Text().TokenWithSkipComment()
                                                    select new LayerDefinition(identifier);

        #endregion

    }
}
