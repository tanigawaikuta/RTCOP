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
    /// プリプロセッサのパーサ
    /// </summary>
    static class PreprocessParser
    {
        #region define関連パーサ
        /// <summary>
        /// define用の識別子、\で改行できる
        /// </summary>
        private static readonly Parser<string> IdentifierForDefine =
                                                    from first in Parse.Letter.Or(Parse.Chars('_'))
                                                    from then in Parse.Regex(@"((\\(\n|\r\n))|\w)*").Text()
                                                    select first + then;

        /// <summary>
        /// define用の式\で改行できる
        /// </summary>
        private static readonly Parser<string> ExpressionForDefine =
                                                    from first in Parse.Regex(new Regex(@"[^\r\n]")).Text()
                                                    from then in Parse.Regex(new Regex(@"((\\(\n|\r\n))|([^\r\n]))*")).Text()
                                                    select first + then;

        /// <summary>
        /// 読み取り: #ifdef Aho
        /// </summary>
        public static readonly Parser<Model.PreprocessDirective> Ifdef = 
                                                    from sharp in Parse.String("#").TokenWithSkipCommentForPreprocessParser()
                                                    from keyword in Parse.String("ifdef").TokenWithSkipCommentForPreprocessParser()
                                                    from name in IdentifierForDefine.TokenWithSkipCommentForPreprocessParser()
                                                    from lineend in Parse.LineTerminator
                                                    select new Model.PreprocessDirective(Model.DirectiveKind.Ifdef, name);

        /// <summary>
        /// 読み取り: #ifndef Aho
        /// </summary>
        public static readonly Parser<Model.PreprocessDirective> Ifndef =
                                                    from sharp in Parse.String("#").TokenWithSkipCommentForPreprocessParser()
                                                    from keyword in Parse.String("ifndef").TokenWithSkipCommentForPreprocessParser()
                                                    from name in IdentifierForDefine.TokenWithSkipCommentForPreprocessParser()
                                                    from lineend in Parse.LineTerminator
                                                    select new Model.PreprocessDirective(Model.DirectiveKind.Ifndef, name);

        /// <summary>
        /// 読み取り: #if
        /// </summary>
        public static readonly Parser<Model.PreprocessDirective> If =
                                                    from sharp in Parse.String("#").TokenWithSkipCommentForPreprocessParser()
                                                    from keyword in Parse.String("if").TokenWithSkipCommentForPreprocessParser()
                                                    from expression in ExpressionForDefine.TokenWithSkipCommentForPreprocessParser()
                                                    from lineend in Parse.LineTerminator
                                                    select new Model.PreprocessDirective(Model.DirectiveKind.If, expression);

        /// <summary>
        /// 読み取り: #elif
        /// </summary>
        public static readonly Parser<Model.PreprocessDirective> Elif =
                                                    from sharp in Parse.String("#").TokenWithSkipCommentForPreprocessParser()
                                                    from keyword in Parse.String("elif").TokenWithSkipCommentForPreprocessParser()
                                                    from expression in ExpressionForDefine.TokenWithSkipCommentForPreprocessParser()
                                                    from lineend in Parse.LineTerminator
                                                    select new Model.PreprocessDirective(Model.DirectiveKind.Elif, expression);

        /// <summary>
        /// 読み取り: #else
        /// </summary>
        public static readonly Parser<Model.PreprocessDirective> Else =
                                                    from sharp in Parse.String("#").TokenWithSkipCommentForPreprocessParser()
                                                    from keyword in Parse.String("else").TokenWithSkipCommentForPreprocessParser()
                                                    from lineend in Parse.LineTerminator
                                                    select new Model.PreprocessDirective(Model.DirectiveKind.Else);

        /// <summary>
        /// 読み取り: #endif
        /// </summary>
        public static readonly Parser<Model.PreprocessDirective> Endif =
                                                    from sharp in Parse.String("#").TokenWithSkipCommentForPreprocessParser()
                                                    from keyword in Parse.String("endif").TokenWithSkipCommentForPreprocessParser()
                                                    from lineend in Parse.LineTerminator
                                                    select new Model.PreprocessDirective(Model.DirectiveKind.Endif);

        /// <summary>
        /// 読み取り: #define Aho
        /// </summary>
        public static readonly Parser<Model.PreprocessDirective> Define =
                                                    from sharp in Parse.String("#").TokenWithSkipCommentForPreprocessParser()
                                                    from keyword in Parse.String("define").TokenWithSkipCommentForPreprocessParser()
                                                    from name in IdentifierForDefine.TokenWithSkipCommentForPreprocessParser()
                                                    from expression in ExpressionForDefine.XOr(Parse.Return("")).TokenWithSkipCommentForPreprocessParser()
                                                    from lineend in Parse.LineTerminator
                                                    select new Model.PreprocessDirective(Model.DirectiveKind.Define, name, expression);

        /// <summary>
        /// 読み取り: #undef Aho
        /// </summary>
        public static readonly Parser<Model.PreprocessDirective> UnDef =
                                                    from sharp in Parse.String("#").TokenWithSkipCommentForPreprocessParser()
                                                    from keyword in Parse.String("undef").TokenWithSkipCommentForPreprocessParser()
                                                    from name in IdentifierForDefine.TokenWithSkipCommentForPreprocessParser()
                                                    from lineend in Parse.LineTerminator
                                                    select new Model.PreprocessDirective(Model.DirectiveKind.Undef, name);

        #endregion

        #region include関連パーサ
        /// <summary>
        /// include用のファイル名\で改行できる
        /// </summary>
        private static readonly Parser<string> FilenameForInclude =
                                                    from first in Parse.Regex(new Regex(@"[^<>"" \t\r\n]")).Text()
                                                    from then in Parse.Regex(new Regex(@"((\\(\n|\r\n))|([^<>"" \t\r\n]))*")).Text()
                                                    select first + then;

        /// <summary>
        /// 読み取り: #include "Hello.h"
        /// </summary>
        public static readonly Parser<Model.PreprocessDirective> Include =
                                                    from sharp in Parse.String("#").TokenWithSkipCommentForPreprocessParser()
                                                    from keyword in Parse.String("include").TokenWithSkipCommentForPreprocessParser()
                                                    from begin in Parse.Char('"').Or(Parse.Char('<'))
                                                    from space1 in Parse.Regex(@"(\\(\n|\r\n))|[ \t]").Many()
                                                    from filename in FilenameForInclude
                                                    from space2 in Parse.Regex(@"(\\(\n|\r\n))|[ \t]").Many()
                                                    from end in Parse.Char('"').Or(Parse.Char('>'))
                                                    from lineend in Parse.LineTerminator.TokenWithSkipCommentForPreprocessParser()
                                                    where (begin == '"' && end == '"') || (begin == '<' && end == '>')
                                                    select new Model.PreprocessDirective(Model.DirectiveKind.Include, filename);

        #endregion

        #region その他
        /// <summary>
        /// line用の番号\で改行できる
        /// </summary>
        private static readonly Parser<string> NumberForLine =
                                                    from first in Parse.Regex(new Regex(@"[1-9]")).Text()
                                                    from then in Parse.Regex(new Regex(@"((\\(\n|\r\n))|([0-9]))*")).Text()
                                                    select first + then;

        /// <summary>
        /// line用のファイル名\で改行できる
        /// </summary>
        private static readonly Parser<string> FilenameForLine =
                                                    from begin in Parse.Char('"')
                                                    from space1 in Parse.Regex(@"(\\(\n|\r\n))|[ \t]").Many()
                                                    from first in Parse.Regex(new Regex(@"[^<>"" \t\r\n]")).Text()
                                                    from then in Parse.Regex(new Regex(@"((\\(\n|\r\n))|([^<>"" \t\r\n]))*")).Text()
                                                    from space2 in Parse.Regex(@"(\\(\n|\r\n))|[ \t]").Many()
                                                    from end in Parse.Char('"')
                                                    select first + then;

        /// <summary>
        /// 読み取り: #line
        /// </summary>
        public static readonly Parser<Model.PreprocessDirective> Line =
                                                    from sharp in Parse.String("#").TokenWithSkipCommentForPreprocessParser()
                                                    from keyword in Parse.String("line").TokenWithSkipCommentForPreprocessParser()
                                                    from num in NumberForLine.TokenWithSkipCommentForPreprocessParser()
                                                    from filename in FilenameForLine.XOr(Parse.Return("")).TokenWithSkipCommentForPreprocessParser()
                                                    from lineend in Parse.LineTerminator
                                                    select new Model.PreprocessDirective(Model.DirectiveKind.Line, num, filename);

        /// <summary>
        /// 読み取り: #error
        /// </summary>
        public static readonly Parser<Model.PreprocessDirective> Error =
                                                    from sharp in Parse.String("#").TokenWithSkipCommentForPreprocessParser()
                                                    from keyword in Parse.String("error").TokenWithSkipCommentForPreprocessParser()
                                                    from message in ExpressionForDefine.XOr(Parse.Return("")).TokenWithSkipCommentForPreprocessParser()
                                                    from lineend in Parse.LineTerminator
                                                    select new Model.PreprocessDirective(Model.DirectiveKind.Error, message);

        /// <summary>
        /// 読み取り: #pragma
        /// </summary>
        public static readonly Parser<Model.PreprocessDirective> Pragma =
                                                    from sharp in Parse.String("#").TokenWithSkipCommentForPreprocessParser()
                                                    from keyword in Parse.String("pragma").TokenWithSkipCommentForPreprocessParser()
                                                    from param in ExpressionForDefine.XOr(Parse.Return("")).TokenWithSkipCommentForPreprocessParser()
                                                    from lineend in Parse.LineTerminator
                                                    select new Model.PreprocessDirective(Model.DirectiveKind.Pragma, param);

        /// <summary>
        /// 読み取り: 非標準
        /// </summary>
        public static readonly Parser<Model.PreprocessDirective> NonStandard =
                                                    from sharp in Parse.String("#").TokenWithSkipCommentForPreprocessParser()
                                                    from keyword in IdentifierForDefine.TokenWithSkipCommentForPreprocessParser()
                                                    from param in ExpressionForDefine.XOr(Parse.Return("")).TokenWithSkipCommentForPreprocessParser()
                                                    from lineend in Parse.LineTerminator
                                                    select new Model.PreprocessDirective(Model.DirectiveKind.NonStandard, keyword, param);

        /// <summary>
        /// 読み取り: #
        /// </summary>
        public static readonly Parser<Model.PreprocessDirective> None =
                                                    from sharp in Parse.String("#").TokenWithSkipCommentForPreprocessParser()
                                                    from lineend in Parse.LineTerminator
                                                    select new Model.PreprocessDirective(Model.DirectiveKind.None);

        #endregion

        #region 組み合わせ
        /// <summary>
        /// 何らかのディレクティブ
        /// </summary>
        public static readonly Parser<Model.PreprocessDirective> Directive =
                                                    Ifdef
                                                    .Or(Ifndef)
                                                    .Or(If)
                                                    .Or(Elif)
                                                    .Or(Else)
                                                    .Or(Endif)
                                                    .Or(Define)
                                                    .Or(UnDef)
                                                    .Or(Include)
                                                    .Or(Line)
                                                    .Or(Error)
                                                    .Or(Pragma)
                                                    .Or(NonStandard)
                                                    .Or(None);

        #endregion

    }
}
