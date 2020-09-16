using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Sprache;

namespace LayerCompiler.Parsers
{
    /// <summary>
    /// RTCOPに関係ない無視するもののパーサ
    /// </summary>
    static class IgnoreParser
    {
        /// <summary>
        /// 無視するトークン
        /// </summary>
        public static readonly Parser<Model.IgnoreObject> Token =
                                                    from token in TokenParser.Token
                                                    where !((token is Model.Identifier) && (token.Text != "baselayer") &&                             // baselayerは特別にOK
                                                            (TokenParser.RTCOPKeywordStrings.Contains(token.Text)))                                   // RTCOPのキーワードと一致しない
                                                    where !((token is Model.OperatorOrPunctuator) && (token.Text == "#" || token.Text == "##"))       // #か##ではない
                                                    where !((token is Model.OperatorOrPunctuator) && (token.Text == "{" || token.Text == "}"))        // ブロックではない
                                                    select new Model.IgnoreObject(token);

        /// <summary>
        /// 無視するブロック
        /// </summary>
        public static readonly Parser<Model.IgnoreObjectBlock> Block =
                                                    from beginblock in Parse.String("{").Text().TokenWithSkipComment()
                                                    from contents in Token.Or(Block).TokenWithSkipComment().Many()
                                                    from endblock in Parse.String("}").Text().TokenWithSkipComment()
                                                    select new Model.IgnoreObjectBlock(
                                                        new Model.IgnoreObject(new Model.OperatorOrPunctuator("{")), 
                                                        new Model.IgnoreObject(new Model.OperatorOrPunctuator("}")), contents);

        /// <summary>
        /// 無視するトークン(式用)
        /// </summary>
        public static readonly Parser<Model.IgnoreObject> TokenForExpression =
                                                    from token in TokenParser.Token
                                                    where !((token is Model.Identifier) && (TokenParser.RTCOPKeywordStrings.Contains(token.Text)))    // RTCOPのキーワードと一致しない
                                                    where !((token is Model.Keyword) && (token.Text == "#" || token.Text == "##"))                    // #か##ではない
                                                    where !((token is Model.OperatorOrPunctuator) && (token.Text == "(" || token.Text == ")"))        // ブロックではない
                                                    select new Model.IgnoreObject(token);

        /// <summary>
        /// 無視するブロック(式用)
        /// </summary>
        public static readonly Parser<Model.IgnoreObjectBlock> BlockForExpression =
                                                    from beginblock in Parse.String("(").Text().TokenWithSkipComment()
                                                    from contents in Token.Or(Block).TokenWithSkipComment().Many()
                                                    from endblock in Parse.String(")").Text().TokenWithSkipComment()
                                                    select new Model.IgnoreObjectBlock(
                                                        new Model.IgnoreObject(new Model.OperatorOrPunctuator("(")),
                                                        new Model.IgnoreObject(new Model.OperatorOrPunctuator(")")), contents);

        /// <summary>
        /// 無視するディレクティブ
        /// </summary>
        public static readonly Parser<Model.IgnoreObject> Directive =
                                                    from directive in PreprocessParser.RTCOPDirective
                                                    select new Model.IgnoreObject(directive);

        /// <summary>
        /// 無視するトークン、ブロック、ディレクティブ
        /// </summary>
        public static readonly Parser<Model.IgnoreObject> IgnoreObject =
                                                    Directive
                                                    .Or(Block)
                                                    .Or(Token);

    }
}
