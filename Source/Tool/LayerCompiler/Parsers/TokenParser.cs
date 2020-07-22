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
    static class TokenParser
    {
        #region 識別子
        /// <summary>
        /// C++の識別子となる文字列
        /// </summary>
        public static readonly Parser<string> IdentifierString =
                                                    from identifier in Parse.Identifier(
                                                                       Parse.Letter.Or(Parse.Char('_')),
                                                                       Parse.LetterOrDigit.Or(Parse.Char('_')))
                                                    where !(KeywordStrings.Contains(identifier) || OperatorOrPunctuatorStrings.Contains(identifier))
                                                    select identifier;

        /// <summary>
        /// C++の識別子の読み取り
        /// </summary>
        public static readonly Parser<Identifier> Identifier =
                                                    from identifier in IdentifierString
                                                    select new Identifier(identifier);

        /// <summary>
        /// RTCOPの識別子となる文字列
        /// </summary>
        public static readonly Parser<string> RTCOPIdentifierString =
                                                    from identifier in Parse.Identifier(
                                                                       Parse.Letter.Or(Parse.Char('_')),
                                                                       Parse.LetterOrDigit.Or(Parse.Char('_')))
                                                    where !(KeywordStrings.Contains(identifier) || RTCOPKeywordStrings.Contains(identifier) || OperatorOrPunctuatorStrings.Contains(identifier))
                                                    select identifier;

        /// <summary>
        /// RTCOPの識別子の読み取り
        /// </summary>
        public static readonly Parser<Identifier> RTCOPIdentifier =
                                                    from identifier in RTCOPIdentifierString
                                                    select new Identifier(identifier);

        #endregion

        #region リテラル
        /// <summary>
        /// 整数リテラル
        /// </summary>
        public static readonly Parser<IntegerLiteral> IntegerLiteral =
                                                    from num in Parse.Regex(@"((0x|0X)[0-9a-fA-F]+)|(0[0-7]*)|([1-9][0-9]*)")
                                                    from suffix in Parse.Regex(@"((u|U)((ll|LL)|(l|L))?)|((ll|LL)(u|U)?)|((l|L)(u|U)?)").XOr(Parse.Return(""))
                                                    from usersuffix in IdentifierString.XOr(Parse.Return(""))
                                                    select new IntegerLiteral(num + suffix + usersuffix, num, suffix, usersuffix);

        /// <summary>
        /// 文字リテラル
        /// </summary>
        public static readonly Parser<CharacterLiteral> CharacterLiteral =
                                                    from prefix in Parse.Regex(@"u|U|L").XOr(Parse.Return(""))
                                                    from begin in Parse.Char('\'')
                                                    from character in Parse.Regex(@"([^\r\n\'\\])|(\\(\'|""|\?|\\|a|b|f|n|r|t|v))|(\\[0-7]{1,3})|(\\x[0-9a-fA-F]{1,2})|(\\(u[0-9a-fA-F]{4}|U[0-9a-fA-F]{8}))")
                                                    from end in Parse.Char('\'')
                                                    from usersuffix in IdentifierString.XOr(Parse.Return(""))
                                                    select new CharacterLiteral(prefix + begin + character + end + usersuffix, prefix, character, usersuffix);

        /// <summary>
        /// 浮動小数リテラル
        /// </summary>
        public static readonly Parser<FloatingLiteral> FloatingLiteral =
                                                    from value in Parse.Regex(@"(([.][0-9]+)|([0-9]+[.][0-9]*))((e|E)(\+|\-)?([0-9])+)?").Or(Parse.Regex(@"[0-9]+(e|E)(\+|\-)?[0-9]+"))
                                                    from suffix in Parse.Regex(@"(f|l|F|L)").XOr(Parse.Return(""))
                                                    from usersuffix in IdentifierString.XOr(Parse.Return(""))
                                                    select new FloatingLiteral(value + suffix + usersuffix, value, suffix, usersuffix);

        /// <summary>
        /// 通常の文字列リテラル
        /// </summary>
        public static readonly Parser<StringLiteral> NormalStringLiteral =
                                                    from prefix in Parse.Regex(@"((u8)|u|U|L)").XOr(Parse.Return(""))
                                                    from begin in Parse.Char('"')
                                                    from str in Parse.Regex(@"(([^\r\n""\\])|(\\(\'|""|\?|\\|a|b|f|n|r|t|v))|(\\[0-7]{1,3})|(\\x[0-9a-fA-F]{1,2})|(\\(u[0-9a-fA-F]{4}|U[0-9a-fA-F]{8})))*")
                                                    from end in Parse.Char('"')
                                                    from usersuffix in IdentifierString.XOr(Parse.Return(""))
                                                    select new StringLiteral(prefix + begin + str + end + usersuffix, str, prefix, usersuffix);

        /// <summary>
        /// 文字列リテラル (生文字列)
        /// 実際のC++よりマッチするケースが多くなってしまっているが、面倒なのでこのままでいく
        /// </summary>
        public static readonly Parser<StringLiteral> RawStringLiteral =
                                                    from prefix in Parse.Regex(@"((u8)|u|U|L)").XOr(Parse.Return(""))
                                                    from r in Parse.Char('R')
                                                    from begin1 in Parse.Regex(@"("")")
                                                    from begin2 in Parse.Regex(@"[^ \(\)\t\v\f\r\n\\]*")
                                                    from begin3 in Parse.Regex(@"\(")
                                                    from str in Parse.Regex(@"(.|\r\n|\n)*(?=(\)" + begin2 + @"""))")
                                                    from end in Parse.Regex(@"\)" + begin2 + @"""")
                                                    from usersuffix in IdentifierString.XOr(Parse.Return(""))
                                                    select new StringLiteral(prefix + r + begin1 + begin2 + begin3 + str + end + usersuffix, str, prefix, usersuffix);

        /// <summary>
        /// 文字列リテラル
        /// </summary>
        public static readonly Parser<StringLiteral> StringLiteral =
                                                    NormalStringLiteral
                                                    .Or(RawStringLiteral);

        /// <summary>
        /// 真偽値
        /// </summary>
        public static readonly Parser<BooleanLiteral> BooleanLiteral =
                                                    from boolean in Parse.String("true").Or(Parse.String("false")).Text()
                                                    select new BooleanLiteral(boolean);

        /// <summary>
        /// ポインタリテラル
        /// </summary>
        public static readonly Parser<PointerLiteral> PointerLiteral =
                                                    from ptr in Parse.String("nullptr").Text()
                                                    select new PointerLiteral(ptr);

        /// <summary>
        /// リテラル
        /// </summary>
        public static readonly Parser<Literal> Literal =
                                                    FloatingLiteral
                                                    .Or<Literal>(IntegerLiteral)
                                                    .Or<Literal>(StringLiteral)
                                                    .Or<Literal>(CharacterLiteral)
                                                    .Or<Literal>(BooleanLiteral)
                                                    .Or<Literal>(PointerLiteral);

        #endregion

        #region 演算子・区切り文字
        /// <summary>
        /// 演算子・区切り文字となる文字列
        /// </summary>
        public static readonly string[] OperatorOrPunctuatorStrings = new string[]
            { "{", "}", "[", "]", "#", "##", "(", ")", "<:", ":>", "<%", "%>", "%:", "%:%:", ";", ":", "...",
              "?", "::", ".", ".*", "+", "-", "*", "/", "%", "?", "&", "|", "~", "!", "=", "<", ">", "+=", "-=",
              "*=", "/=", "%=", "?=", "&=", "|=", "<<", ">>", ">>=", "<<=", "==", "!=", "<=", ">=", "&&", "||",
              "++", "--", ",", "->*", "->", "new", "delete", "and", "and_eq", "bitand", "bitor", "compl", "not",
              "not_eq", "or", "or_eq", "xor", "xor_eq"
            };

        /// <summary>
        /// 演算子・区切り文字であるかチェック
        /// </summary>
        public static readonly Func<Parser<string>> IsOperatorOrPunctuator = () =>
        {
            var parse = Parse.String(OperatorOrPunctuatorStrings[0]);
            for (int i = 1; i < OperatorOrPunctuatorStrings.Length; ++i)
            {
                parse = parse.Or(Parse.String(OperatorOrPunctuatorStrings[i]));
            }
            return parse.Text();
        };

        /// <summary>
        /// 演算子・区切り文字
        /// </summary>
        public static readonly Parser<OperatorOrPunctuator> OperatorOrPunctuator =
                                                    from op in IsOperatorOrPunctuator()
                                                    select new OperatorOrPunctuator(op);

        #endregion

        #region キーワード
        /// <summary>
        /// C++11のキーワードとなる文字列
        /// </summary>
        public static readonly string[] KeywordStrings = new string[]
            {
              "alignas", "alignof", "and", "and_eq", "asm", "auto", "bitand", "bitor", "bool", "break", "case",
              "catch", "char", "char16_t", "char32_t", "class", "compl", "const", "constexpr", "const_cast",
              "continue", "decltype", "default", "delete", "do", "double", "dynamic_cast", "else", "enum",
              "explicit", "export", "extern", "false", "float", "for", "friend", "goto", "if", "inline", "int",
              "long", "mutable", "namespace", "new", "noexcept", "not", "not_eq", "nullptr", "operator", "or",
              "or_eq", "private", "protected", "public", "register", "reinterpret_cast", "return", "short",
              "signed", "sizeof", "static", "static_assert", "static_cast", "struct", "switch", "template",
              "this", "thread_local", "throw", "true", "try", "typedef", "typeid", "typename", "union",
              "unsigned", "using", "virtual", "void", "volatile", "wchar_t", "while", "xor", "xor_eq"
            };

        /// <summary>
        /// C++11のキーワードであるかチェック
        /// </summary>
        private static readonly Func<Parser<string>> IsKeyword = () =>
        {
            var parse = Parse.String(KeywordStrings[0]);
            for (int i = 1; i < KeywordStrings.Length; ++i)
            {
                parse = parse.Or(Parse.String(KeywordStrings[i]));
            }
            return parse.Text();
        };

        /// <summary>
        /// C++11のキーワード
        /// </summary>
        public static readonly Parser<Keyword> Keyword =
                                                    from keyword in IsKeyword()
                                                    select new Keyword(keyword);

        /// <summary>
        /// RTCOPのキーワードとなる文字列
        /// </summary>
        public static readonly string[] RTCOPKeywordStrings = new string[]
            {
              "layer", "baselayer", "base", "partial", "proceed"
            };

        /// <summary>
        /// RTCOPのキーワードであるかチェック
        /// </summary>
        private static readonly Func<Parser<string>> IsRTCOPKeyword = () =>
        {
            var parse = Parse.String(RTCOPKeywordStrings[0]);
            for (int i = 1; i < RTCOPKeywordStrings.Length; ++i)
            {
                parse = parse.Or(Parse.String(RTCOPKeywordStrings[i]));
            }
            return parse.Text();
        };

        /// <summary>
        /// RTCOPのキーワード
        /// </summary>
        public static readonly Parser<Keyword> RTCOPKeyword =
                                                    from keyword in IsRTCOPKeyword()
                                                    select new Keyword(keyword);

        #endregion



        #region 組み合わせ (順番重要)
        /// <summary>
        /// 不明なトークン
        /// </summary>
        public static readonly Parser<Token> Unknown =
                                                    from str in Parse.Regex(@"\S*")
                                                    select new Token(str);

        /// <summary>
        /// トークン
        /// </summary>
        public static readonly Parser<Token> Token =
                                                    Literal
                                                    .Or<Token>(Identifier)
                                                    .Or<Token>(OperatorOrPunctuator)
                                                    .Or<Token>(Keyword);

        /// <summary>
        /// RTCOPのトークン
        /// </summary>
        public static readonly Parser<Token> RTCOPToken =
                                                    Literal
                                                    .Or<Token>(RTCOPIdentifier)
                                                    .Or<Token>(OperatorOrPunctuator)
                                                    .Or<Token>(Keyword)
                                                    .Or<Token>(RTCOPKeyword);

        #endregion

    }
}
