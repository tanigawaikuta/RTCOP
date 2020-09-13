using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Sprache;
using LayerCompiler.Parsers.Model;
using System.IO;
using System.ComponentModel.Design;

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
        /// defineのパラメータ
        /// </summary>
        private static readonly Parser<string> DefineFuncParameters_After =
                                                    from colon in Parse.String(",").TokenWithSkipCommentForPreprocessParser()
                                                    from parameter in IdentifierForDefine.TokenWithSkipCommentForPreprocessParser()
                                                    select parameter;

        /// <summary>
        /// defineのパラメータ
        /// </summary>
        private static readonly Parser<IEnumerable<string>> DefineFuncParameters =
                                                    from begin in Parse.String("(").TokenWithSkipCommentForPreprocessParser()
                                                    from first in IdentifierForDefine.TokenWithSkipCommentForPreprocessParser()
                                                    from then in DefineFuncParameters_After.TokenWithSkipCommentForPreprocessParser().Many()
                                                    from end in Parse.String(")").TokenWithSkipCommentForPreprocessParser()
                                                    select new List<string> { first }.Concat(then);

        /// <summary>
        /// 読み取り: #define Aho
        /// </summary>
        public static readonly Parser<Model.PreprocessDirective> Define =
                                                    from sharp in Parse.String("#").TokenWithSkipCommentForPreprocessParser()
                                                    from keyword in Parse.String("define").TokenWithSkipCommentForPreprocessParser()
                                                    from name in IdentifierForDefine.TokenWithSkipCommentForPreprocessParser()
                                                    from parameters in DefineFuncParameters.Or(Parse.Return(new string[0])).TokenWithSkipCommentForPreprocessParser()
                                                    from expression in ExpressionForDefine.Or(Parse.Return("")).TokenWithSkipCommentForPreprocessParser()
                                                    from lineend in Parse.LineTerminator
                                                    let param2 = new List<string>(parameters).Concat(new string[] { expression })
                                                    select new Model.PreprocessDirective(Model.DirectiveKind.Define, name, param2);

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

        /// <summary>
        /// 読み取り: #import_lh "Hello.lh"
        /// RTCOP用のディレクティブ
        /// </summary>
        public static readonly Parser<Model.PreprocessDirective> ImportLayerHeader =
                                                    from sharp in Parse.String("#").TokenWithSkipCommentForPreprocessParser()
                                                    from keyword in Parse.String("import_lh").Text().TokenWithSkipCommentForPreprocessParser()
                                                    from begin in Parse.Char('"').Or(Parse.Char('<'))
                                                    from space1 in Parse.Regex(@"(\\(\n|\r\n))|[ \t]").Many()
                                                    from filename in FilenameForInclude
                                                    from space2 in Parse.Regex(@"(\\(\n|\r\n))|[ \t]").Many()
                                                    from end in Parse.Char('"').Or(Parse.Char('>'))
                                                    from lineend in Parse.LineTerminator.TokenWithSkipCommentForPreprocessParser()
                                                    where (begin == '"' && end == '"') || (begin == '<' && end == '>')
                                                    where Path.GetExtension(filename) == ".lh"
                                                    select new Model.PreprocessDirective(Model.DirectiveKind.ImportLayerHeader, filename);

        /// <summary>
        /// 読み取り: #import_baseclass "Hello.h"
        /// RTCOP用のディレクティブ
        /// </summary>
        public static readonly Parser<Model.PreprocessDirective> ImportBaseClassHeader =
                                                    from sharp in Parse.String("#").TokenWithSkipCommentForPreprocessParser()
                                                    from keyword in Parse.String("import_baseclass").Text().TokenWithSkipCommentForPreprocessParser()
                                                    from begin in Parse.Char('"').Or(Parse.Char('<'))
                                                    from space1 in Parse.Regex(@"(\\(\n|\r\n))|[ \t]").Many()
                                                    from filename in FilenameForInclude
                                                    from space2 in Parse.Regex(@"(\\(\n|\r\n))|[ \t]").Many()
                                                    from end in Parse.Char('"').Or(Parse.Char('>'))
                                                    from lineend in Parse.LineTerminator.TokenWithSkipCommentForPreprocessParser()
                                                    where (begin == '"' && end == '"') || (begin == '<' && end == '>')
                                                    where Path.GetExtension(filename) != ".lh"
                                                    select new Model.PreprocessDirective(Model.DirectiveKind.ImportBaseClassHeader, filename);

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
                                                    from filename in FilenameForLine.Or(Parse.Return("")).TokenWithSkipCommentForPreprocessParser()
                                                    from lineend in Parse.LineTerminator
                                                    select new Model.PreprocessDirective(Model.DirectiveKind.Line, num, new string[] { filename });

        /// <summary>
        /// 読み取り: #error
        /// </summary>
        public static readonly Parser<Model.PreprocessDirective> Error =
                                                    from sharp in Parse.String("#").TokenWithSkipCommentForPreprocessParser()
                                                    from keyword in Parse.String("error").TokenWithSkipCommentForPreprocessParser()
                                                    from message in ExpressionForDefine.Or(Parse.Return("")).TokenWithSkipCommentForPreprocessParser()
                                                    from lineend in Parse.LineTerminator
                                                    select new Model.PreprocessDirective(Model.DirectiveKind.Error, message);

        /// <summary>
        /// 読み取り: #pragma
        /// </summary>
        public static readonly Parser<Model.PreprocessDirective> Pragma =
                                                    from sharp in Parse.String("#").TokenWithSkipCommentForPreprocessParser()
                                                    from keyword in Parse.String("pragma").TokenWithSkipCommentForPreprocessParser()
                                                    from param in ExpressionForDefine.Or(Parse.Return("")).TokenWithSkipCommentForPreprocessParser()
                                                    from lineend in Parse.LineTerminator
                                                    select new Model.PreprocessDirective(Model.DirectiveKind.Pragma, param);

        /// <summary>
        /// 読み取り: 非標準
        /// </summary>
        public static readonly Parser<Model.PreprocessDirective> NonStandard =
                                                    from sharp in Parse.String("#").TokenWithSkipCommentForPreprocessParser()
                                                    from keyword in IdentifierForDefine.TokenWithSkipCommentForPreprocessParser()
                                                    from param in ExpressionForDefine.Or(Parse.Return("")).TokenWithSkipCommentForPreprocessParser()
                                                    from lineend in Parse.LineTerminator
                                                    select new Model.PreprocessDirective(Model.DirectiveKind.NonStandard, keyword, new string[] { param });

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

        /// <summary>
        /// RTCOPのディレクティブ
        /// </summary>
        public static readonly Parser<Model.PreprocessDirective> RTCOPDirective =
                                                    ImportLayerHeader
                                                    .Or(ImportBaseClassHeader)
                                                    .Or(Directive);

        #endregion

        #region 行
        /// <summary>
        /// 非ディレクティブ行
        /// </summary>
        public static readonly Parser<Model.NonDirectiveLine> NonDirectiveLine =
                                                    from tokens in TokenParser.RTCOPToken.TokenWithSkipCommentForPreprocessParser().Many()
                                                    from lineend in Parse.LineTerminator
                                                    select new Model.NonDirectiveLine(tokens);

        /// <summary>
        /// ディレクティブか行
        /// </summary>
        public static readonly Parser<object> DirectiveOrLine =
                                                    RTCOPDirective
                                                    .Or<object>(NonDirectiveLine);

        #endregion

        #region ifセクション
        /// <summary>
        /// ディレクティブか行(ifセクション用)
        /// </summary>
        public static readonly Parser<object> DirectiveOrLineForIfSection =
                                                    from item in DirectiveOrLine
                                                    let kind = item is Model.PreprocessDirective ? ((Model.PreprocessDirective)item).Kind : DirectiveKind.None
                                                    where !((kind == DirectiveKind.If) || (kind == DirectiveKind.Ifdef) || (kind == DirectiveKind.Ifndef) || (kind == DirectiveKind.Elif) || (kind == DirectiveKind.Else) || (kind == DirectiveKind.Endif))
                                                    select item;

        /// <summary>
        /// ifセクション
        /// </summary>
        public static readonly Parser<IfSection> IfSection =
                                                    from ifdirective in Ifdef.Or(Ifndef).Or(If).TokenWithSkipCommentForPreprocessParser()
                                                    from children in IfSection.Or(DirectiveOrLineForIfSection).TokenWithSkipCommentForPreprocessParser().Many()
                                                    from elifgroups in ElifGroup.TokenWithSkipCommentForPreprocessParser().Many()
                                                    from elsegroup in ElseGroup.Or(Parse.Return<IEnumerable<object>>(null)).TokenWithSkipCommentForPreprocessParser()
                                                    from endifdirective in Endif.TokenWithSkipCommentForPreprocessParser()
                                                    let ifgroup = new List<object>() { ifdirective }.Concat(children)
                                                    select new IfSection(ifgroup, elifgroups, elsegroup);

        /// <summary>
        /// elifグループ
        /// </summary>
        private static readonly Parser<IEnumerable<object>> ElifGroup =
                                                    from elifdirective in Elif.TokenWithSkipCommentForPreprocessParser()
                                                    from children in IfSection.Or(DirectiveOrLineForIfSection).TokenWithSkipCommentForPreprocessParser().Many()
                                                    let elifgroup = new List<object>() { elifdirective }.Concat(children)
                                                    select elifgroup;

        /// <summary>
        /// elseグループ
        /// </summary>
        private static readonly Parser<IEnumerable<object>> ElseGroup =
                                                    from elsedirective in Else.TokenWithSkipCommentForPreprocessParser()
                                                    from children in IfSection.Or(DirectiveOrLineForIfSection).TokenWithSkipCommentForPreprocessParser().Many()
                                                    let elsegroup = new List<object>() { elsedirective }.Concat(children)
                                                    select elsegroup;

        #endregion

        #region ifディレクティブの式読み取り用のパーサ
        private static readonly Parser<Model.Leaf_IfDirectiveExpression> Leaf_IfDirectiveExpression =
                                                    from unarys in Parse.String("!").Or(Parse.String("-")).Or(Parse.String("+")).Text().TokenWithSkipComment().Many()
                                                    from literal in TokenParser.Literal.TokenWithSkipComment()
                                                    where !(literal is StringLiteral)
                                                    select new Model.Leaf_IfDirectiveExpression(literal, unarys);

        private static readonly Parser<Model.IfDirectiveExpression> Parentheses_IfDirectiveExpression =
                                                    from unarys in Parse.String("!").Or(Parse.String("-")).Or(Parse.String("+")).Text().TokenWithSkipComment().Many()
                                                    from begin in Parse.String("(").TokenWithSkipComment()
                                                    from expression in IfDirectiveExpression.TokenWithSkipComment()
                                                    from end in Parse.String(")").TokenWithSkipComment()
                                                    select new Model.IfDirectiveExpression(expression, unarys);

        private static readonly Parser<Model.IfDirectiveExpression> LeafOrParentheses_IfDirectiveExpression =
                                                    Parentheses_IfDirectiveExpression
                                                    .Or(Leaf_IfDirectiveExpression);

        private static readonly Parser<Model.IfDirectiveExpression.OperatorAndRightExpression> MultiplicativeExpression_After =
                                                    from op in Parse.String("*").Or(Parse.String("/")).Or(Parse.String("%")).Text().TokenWithSkipComment()
                                                    from right in LeafOrParentheses_IfDirectiveExpression.TokenWithSkipComment()
                                                    select new Model.IfDirectiveExpression.OperatorAndRightExpression(op, right);

        private static readonly Parser<Model.IfDirectiveExpression> MultiplicativeExpression =
                                                    from left in LeafOrParentheses_IfDirectiveExpression.TokenWithSkipComment()
                                                    from afters in MultiplicativeExpression_After.TokenWithSkipComment().Many()
                                                    let afterexpressions = afters.Select((obj) => obj.RightExpression)
                                                    let operators = afters.Select((obj) => obj.Operator)
                                                    select new Model.IfDirectiveExpression(operators, new List<Model.IfDirectiveExpression>() { left }.Concat(afterexpressions));

        private static readonly Parser<Model.IfDirectiveExpression.OperatorAndRightExpression> AdditiveExpression_After =
                                                    from op in Parse.String("+").Or(Parse.String("-")).Text().TokenWithSkipComment()
                                                    from right in MultiplicativeExpression.TokenWithSkipComment()
                                                    select new Model.IfDirectiveExpression.OperatorAndRightExpression(op, right);

        private static readonly Parser<Model.IfDirectiveExpression> AdditiveExpression =
                                                   from left in MultiplicativeExpression.TokenWithSkipComment()
                                                   from afters in AdditiveExpression_After.TokenWithSkipComment().Many()
                                                   let afterexpressions = afters.Select((obj) => obj.RightExpression)
                                                   let operators = afters.Select((obj) => obj.Operator)
                                                   select new Model.IfDirectiveExpression(operators, new List<Model.IfDirectiveExpression>() { left }.Concat(afterexpressions));

        private static readonly Parser<Model.IfDirectiveExpression.OperatorAndRightExpression> ShiftExpression_After =
                                                    from op in Parse.String("<<").Or(Parse.String(">>")).Text().TokenWithSkipComment()
                                                    from right in AdditiveExpression.TokenWithSkipComment()
                                                    select new Model.IfDirectiveExpression.OperatorAndRightExpression(op, right);

        private static readonly Parser<Model.IfDirectiveExpression> ShiftExpression =
                                                   from left in AdditiveExpression.TokenWithSkipComment()
                                                   from afters in ShiftExpression_After.TokenWithSkipComment().Many()
                                                   let afterexpressions = afters.Select((obj) => obj.RightExpression)
                                                   let operators = afters.Select((obj) => obj.Operator)
                                                   select new Model.IfDirectiveExpression(operators, new List<Model.IfDirectiveExpression>() { left }.Concat(afterexpressions));

        private static readonly Parser<Model.IfDirectiveExpression.OperatorAndRightExpression> RelationalExpression_After =
                                                    from op in Parse.String("<=").Or(Parse.String(">=")).Or(Parse.String("<")).Or(Parse.String(">")).Text().TokenWithSkipComment()
                                                    from right in ShiftExpression.TokenWithSkipComment()
                                                    select new Model.IfDirectiveExpression.OperatorAndRightExpression(op, right);

        private static readonly Parser<Model.IfDirectiveExpression> RelationalExpression =
                                                   from left in ShiftExpression.TokenWithSkipComment()
                                                   from afters in RelationalExpression_After.TokenWithSkipComment().Many()
                                                   let afterexpressions = afters.Select((obj) => obj.RightExpression)
                                                   let operators = afters.Select((obj) => obj.Operator)
                                                   select new Model.IfDirectiveExpression(operators, new List<Model.IfDirectiveExpression>() { left }.Concat(afterexpressions));

        private static readonly Parser<Model.IfDirectiveExpression.OperatorAndRightExpression> EqualityExpression_After =
                                                    from op in Parse.String("==").Or(Parse.String("!=")).Text().TokenWithSkipComment()
                                                    from right in RelationalExpression.TokenWithSkipComment()
                                                    select new Model.IfDirectiveExpression.OperatorAndRightExpression(op, right);

        private static readonly Parser<Model.IfDirectiveExpression> EqualityExpression =
                                                   from left in RelationalExpression.TokenWithSkipComment()
                                                   from afters in EqualityExpression_After.TokenWithSkipComment().Many()
                                                   let afterexpressions = afters.Select((obj) => obj.RightExpression)
                                                   let operators = afters.Select((obj) => obj.Operator)
                                                   select new Model.IfDirectiveExpression(operators, new List<Model.IfDirectiveExpression>() { left }.Concat(afterexpressions));

        private static readonly Parser<Model.IfDirectiveExpression.OperatorAndRightExpression> AndExpression_After =
                                                    from op in Parse.String("&").Text().TokenWithSkipComment()
                                                    from right in EqualityExpression.TokenWithSkipComment()
                                                    select new Model.IfDirectiveExpression.OperatorAndRightExpression(op, right);

        private static readonly Parser<Model.IfDirectiveExpression> AndExpression =
                                                   from left in EqualityExpression.TokenWithSkipComment()
                                                   from afters in AndExpression_After.TokenWithSkipComment().Many()
                                                   let afterexpressions = afters.Select((obj) => obj.RightExpression)
                                                   let operators = afters.Select((obj) => obj.Operator)
                                                   select new Model.IfDirectiveExpression(operators, new List<Model.IfDirectiveExpression>() { left }.Concat(afterexpressions));

        private static readonly Parser<Model.IfDirectiveExpression.OperatorAndRightExpression> ExclusiveOrExpression_After =
                                                    from op in Parse.String("^").Text().TokenWithSkipComment()
                                                    from right in AndExpression.TokenWithSkipComment()
                                                    select new Model.IfDirectiveExpression.OperatorAndRightExpression(op, right);

        private static readonly Parser<Model.IfDirectiveExpression> ExclusiveOrExpression =
                                                   from left in AndExpression.TokenWithSkipComment()
                                                   from afters in ExclusiveOrExpression_After.TokenWithSkipComment().Many()
                                                   let afterexpressions = afters.Select((obj) => obj.RightExpression)
                                                   let operators = afters.Select((obj) => obj.Operator)
                                                   select new Model.IfDirectiveExpression(operators, new List<Model.IfDirectiveExpression>() { left }.Concat(afterexpressions));

        private static readonly Parser<Model.IfDirectiveExpression.OperatorAndRightExpression> InclusiveOrExpression_After =
                                                    from op in Parse.String("|").Text().TokenWithSkipComment()
                                                    from right in ExclusiveOrExpression.TokenWithSkipComment()
                                                    select new Model.IfDirectiveExpression.OperatorAndRightExpression(op, right);

        private static readonly Parser<Model.IfDirectiveExpression> InclusiveOrExpression =
                                                   from left in ExclusiveOrExpression.TokenWithSkipComment()
                                                   from afters in InclusiveOrExpression_After.TokenWithSkipComment().Many()
                                                   let afterexpressions = afters.Select((obj) => obj.RightExpression)
                                                   let operators = afters.Select((obj) => obj.Operator)
                                                   select new Model.IfDirectiveExpression(operators, new List<Model.IfDirectiveExpression>() { left }.Concat(afterexpressions));

        private static readonly Parser<Model.IfDirectiveExpression.OperatorAndRightExpression> LogicalAndExpression_After =
                                                    from op in Parse.String("&&").Text().TokenWithSkipComment()
                                                    from right in InclusiveOrExpression.TokenWithSkipComment()
                                                    select new Model.IfDirectiveExpression.OperatorAndRightExpression(op, right);

        private static readonly Parser<Model.IfDirectiveExpression> LogicalAndExpression =
                                                   from left in InclusiveOrExpression.TokenWithSkipComment()
                                                   from afters in LogicalAndExpression_After.TokenWithSkipComment().Many()
                                                   let afterexpressions = afters.Select((obj) => obj.RightExpression)
                                                   let operators = afters.Select((obj) => obj.Operator)
                                                   select new Model.IfDirectiveExpression(operators, new List<Model.IfDirectiveExpression>() { left }.Concat(afterexpressions));

        private static readonly Parser<Model.IfDirectiveExpression.OperatorAndRightExpression> LogicalOrExpression_After =
                                                    from op in Parse.String("||").Text().TokenWithSkipComment()
                                                    from right in LogicalAndExpression.TokenWithSkipComment()
                                                    select new Model.IfDirectiveExpression.OperatorAndRightExpression(op, right);

        private static readonly Parser<Model.IfDirectiveExpression> LogicalOrExpression =
                                                   from left in LogicalAndExpression.TokenWithSkipComment()
                                                   from afters in LogicalOrExpression_After.TokenWithSkipComment().Many()
                                                   let afterexpressions = afters.Select((obj) => obj.RightExpression)
                                                   let operators = afters.Select((obj) => obj.Operator)
                                                   select new Model.IfDirectiveExpression(operators, new List<Model.IfDirectiveExpression>() { left }.Concat(afterexpressions));

        /// <summary>
        /// ifディレクティブの式
        /// </summary>
        public static readonly Parser<Model.IfDirectiveExpression> IfDirectiveExpression = LogicalOrExpression;

        #endregion

    }
}
