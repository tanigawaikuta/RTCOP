using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Sprache;

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
        public static readonly Parser<Model.Identifier> Identifier =
                                                    from identifier in IdentifierString
                                                    select new Model.Identifier(identifier);

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
        public static readonly Parser<Model.Identifier> RTCOPIdentifier =
                                                    from identifier in RTCOPIdentifierString
                                                    select new Model.Identifier(identifier);

        #endregion

        #region リテラル
        /// <summary>
        /// 整数リテラル
        /// </summary>
        public static readonly Parser<Model.IntegerLiteral> IntegerLiteral =
                                                    from num in Parse.Regex(@"((0x|0X)[0-9a-fA-F]+)|(0[0-7]*)|([1-9][0-9]*)")
                                                    from suffix in Parse.Regex(@"((u|U)((ll|LL)|(l|L))?)|((ll|LL)(u|U)?)|((l|L)(u|U)?)").Or(Parse.Return(""))
                                                    from usersuffix in IdentifierString.Or(Parse.Return(""))
                                                    select new Model.IntegerLiteral(num + suffix + usersuffix, num, suffix, usersuffix);

        /// <summary>
        /// 文字リテラル
        /// </summary>
        public static readonly Parser<Model.CharacterLiteral> CharacterLiteral =
                                                    from prefix in Parse.Regex(@"u|U|L").Or(Parse.Return(""))
                                                    from begin in Parse.Char('\'')
                                                    from character in Parse.Regex(@"([^\r\n\'\\])|(\\(\'|""|\?|\\|a|b|f|n|r|t|v))|(\\[0-7]{1,3})|(\\x[0-9a-fA-F]{1,2})|(\\(u[0-9a-fA-F]{4}|U[0-9a-fA-F]{8}))")
                                                    from end in Parse.Char('\'')
                                                    from usersuffix in IdentifierString.Or(Parse.Return(""))
                                                    select new Model.CharacterLiteral(prefix + begin + character + end + usersuffix, prefix, character, usersuffix);

        /// <summary>
        /// 浮動小数リテラル
        /// </summary>
        public static readonly Parser<Model.FloatingLiteral> FloatingLiteral =
                                                    from value in Parse.Regex(@"(([.][0-9]+)|([0-9]+[.][0-9]*))((e|E)(\+|\-)?([0-9])+)?").Or(Parse.Regex(@"[0-9]+(e|E)(\+|\-)?[0-9]+"))
                                                    from suffix in Parse.Regex(@"(f|l|F|L)").Or(Parse.Return(""))
                                                    from usersuffix in IdentifierString.Or(Parse.Return(""))
                                                    select new Model.FloatingLiteral(value + suffix + usersuffix, value, suffix, usersuffix);

        /// <summary>
        /// 通常の文字列リテラル
        /// </summary>
        public static readonly Parser<Model.StringLiteral> NormalStringLiteral =
                                                    from prefix in Parse.Regex(@"((u8)|u|U|L)").Or(Parse.Return(""))
                                                    from begin in Parse.Char('"')
                                                    from str in Parse.Regex(@"(([^\r\n""\\])|(\\(\'|""|\?|\\|a|b|f|n|r|t|v))|(\\[0-7]{1,3})|(\\x[0-9a-fA-F]{1,2})|(\\(u[0-9a-fA-F]{4}|U[0-9a-fA-F]{8})))*")
                                                    from end in Parse.Char('"')
                                                    from usersuffix in IdentifierString.Or(Parse.Return(""))
                                                    select new Model.StringLiteral(prefix + begin + str + end + usersuffix, str, prefix, usersuffix);

        /// <summary>
        /// 文字列リテラル (生文字列)
        /// 実際のC++よりマッチするケースが多くなってしまっているが、面倒なのでこのままでいく
        /// </summary>
        public static readonly Parser<Model.StringLiteral> RawStringLiteral =
                                                    from prefix in Parse.Regex(@"((u8)|u|U|L)").Or(Parse.Return(""))
                                                    from r in Parse.Char('R')
                                                    from begin1 in Parse.Regex(@"("")")
                                                    from begin2 in Parse.Regex(@"[^ \(\)\t\v\f\r\n\\]*")
                                                    from begin3 in Parse.Regex(@"\(")
                                                    from str in Parse.Regex(@"(.|\r\n|\n)*(?=(\)" + begin2 + @"""))")
                                                    from end in Parse.Regex(@"\)" + begin2 + @"""")
                                                    from usersuffix in IdentifierString.Or(Parse.Return(""))
                                                    select new Model.StringLiteral(prefix + r + begin1 + begin2 + begin3 + str + end + usersuffix, str, prefix, usersuffix);

        /// <summary>
        /// 文字列リテラル
        /// </summary>
        public static readonly Parser<Model.StringLiteral> StringLiteral =
                                                    NormalStringLiteral
                                                    .Or(RawStringLiteral);

        /// <summary>
        /// 真偽値
        /// </summary>
        public static readonly Parser<Model.BooleanLiteral> BooleanLiteral =
                                                    from boolean in Parse.String("true").Or(Parse.String("false")).Text()
                                                    select new Model.BooleanLiteral(boolean);

        /// <summary>
        /// ポインタリテラル
        /// </summary>
        public static readonly Parser<Model.PointerLiteral> PointerLiteral =
                                                    from ptr in Parse.String("nullptr").Text()
                                                    select new Model.PointerLiteral(ptr);

        /// <summary>
        /// リテラル
        /// </summary>
        public static readonly Parser<Model.Literal> Literal =
                                                    FloatingLiteral
                                                    .Or<Model.Literal>(IntegerLiteral)
                                                    .Or<Model.Literal>(StringLiteral)
                                                    .Or<Model.Literal>(CharacterLiteral)
                                                    .Or<Model.Literal>(BooleanLiteral)
                                                    .Or<Model.Literal>(PointerLiteral);

        #endregion

        #region 演算子・区切り文字
        /// <summary>
        /// 演算子・区切り文字となる文字列
        /// </summary>
        public static readonly string[] OperatorOrPunctuatorStrings = new string[]
            { "new", "delete", "and_eq", "and", "bitand", "bitor", "compl", "not_eq", "not", "or_eq", "or", "xor_eq", "xor",
              "%:%:", "...", ">>=", "<<=", "->*", "+=", "-=", "*=", "/=", "%=", "&=", "|=", "^=", "<<", ">>", "==", "!=", 
              "<=", ">=", "&&", "||", "++", "--", "->", "<:", ":>", "<%", "%>", "%:", "::", ".*", "##", "#", ",", "{", "}", 
              "[", "]", "(", ")",  ";", ":", "?", ".", "+", "-", "*", "/", "%", "?", "&", "|", "^", "!", "=", "<", ">",  
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
        public static readonly Parser<Model.OperatorOrPunctuator> OperatorOrPunctuator =
                                                    from op in IsOperatorOrPunctuator()
                                                    select new Model.OperatorOrPunctuator(op);

        #endregion

        #region キーワード
        /// <summary>
        /// C++11のキーワードとなる文字列
        /// </summary>
        public static readonly string[] KeywordStrings = new string[]
            {
              "alignas", "alignof", "and_eq", "and", "asm", "auto", "bitand", "bitor", "bool", "break", "case",
              "catch", "char16_t", "char32_t", "char", "class", "compl", "constexpr", "const_cast", "const",
              "continue", "decltype", "default", "delete", "double", "do", "dynamic_cast", "else", "enum",
              "explicit", "export", "extern", "false", "float", "for", "friend", "goto", "if", "inline", "int",
              "long", "mutable", "namespace", "new", "noexcept", "not_eq", "not", "nullptr", "operator", 
              "or_eq", "or", "private", "protected", "public", "register", "reinterpret_cast", "return", "short",
              "signed", "sizeof", "static_assert", "static_cast", "static", "struct", "switch", "template",
              "this", "thread_local", "throw", "true", "try", "typedef", "typeid", "typename", "union",
              "unsigned", "using", "virtual", "void", "volatile", "wchar_t", "while", "xor_eq", "xor",
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
        public static readonly Parser<Model.Keyword> Keyword =
                                                    from keyword in IsKeyword()
                                                    select new Model.Keyword(keyword);

        /// <summary>
        /// RTCOPのキーワードとなる文字列
        /// </summary>
        public static readonly string[] RTCOPKeywordStrings = new string[]
            {
              "layer_members", "layer", "baselayer", "base", "partial", "proceed"
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
        public static readonly Parser<Model.Keyword> RTCOPKeyword =
                                                    from keyword in IsRTCOPKeyword()
                                                    select new Model.Keyword(keyword);

        #endregion



        #region 組み合わせ (順番重要)
        /// <summary>
        /// 不明なトークン
        /// </summary>
        public static readonly Parser<Model.Token> Unknown =
                                                    from str in Parse.Regex(@"\S*")
                                                    select new Model.Token(str);

        /// <summary>
        /// トークン
        /// </summary>
        public static readonly Parser<Model.Token> Token =
                                                    Literal
                                                    .Or<Model.Token>(Identifier)
                                                    .Or<Model.Token>(OperatorOrPunctuator)
                                                    .Or<Model.Token>(Keyword);

        /// <summary>
        /// RTCOPのトークン
        /// </summary>
        public static readonly Parser<Model.Token> RTCOPToken =
                                                    Literal
                                                    .Or<Model.Token>(RTCOPIdentifier)
                                                    .Or<Model.Token>(OperatorOrPunctuator)
                                                    .Or<Model.Token>(Keyword)
                                                    .Or<Model.Token>(RTCOPKeyword);

        #endregion

    }
}
