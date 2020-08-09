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
    /// 各パーサで共有されるパーサ
    /// </summary>
    static class CommonParser
    {
        /// <summary>
        /// ネストされた名前
        /// </summary>
        public static readonly Parser<string> NestedName =
                                                    from first in TokenParser.RTCOPIdentifierString.TokenWithSkipComment()
                                                    from afters in NestedNameAfter.TokenWithSkipComment().Many()
                                                    select first + string.Join("", afters);

        /// <summary>
        /// ネストされた名前の後ろの部分
        /// </summary>
        private static readonly Parser<string> NestedNameAfter =
                                                    from doublecolon in Parse.String("::").Text().TokenWithSkipComment()
                                                    from name in TokenParser.RTCOPIdentifierString.TokenWithSkipComment()
                                                    select doublecolon + name;

        /// <summary>
        /// スーパークラス名の指定
        /// </summary>
        private static readonly Parser<Model.SuperClassDefinition> SuperClassName =
                                                    from access in Parse.String("public")
                                                                     .Or(Parse.String("protected"))
                                                                     .Or(Parse.String("private"))
                                                                     .Text().TokenWithSkipComment()
                                                    from name in NestedName.TokenWithSkipComment()
                                                    select new Model.SuperClassDefinition(name, access);

        /// <summary>
        /// スーパークラスの定義の後ろの部分
        /// </summary>
        private static readonly Parser<Model.SuperClassDefinition> SuperClassDefinitionAfter =
                                                    from colon in Parse.String(",").Text().TokenWithSkipComment()
                                                    from super in SuperClassName.TokenWithSkipComment()
                                                    select super;

        /// <summary>
        /// スーパークラスの定義
        /// </summary>
        public static readonly Parser<IEnumerable<Model.SuperClassDefinition>> SuperClassDefinition =
                                                    from colon in Parse.String(":").Text().TokenWithSkipComment()
                                                    from first in SuperClassName.TokenWithSkipComment()
                                                    from afters in SuperClassDefinitionAfter.TokenWithSkipComment().Many()
                                                    let result = new List<Model.SuperClassDefinition>() { first }.Concat(afters)
                                                    select result;

        /// <summary>
        /// ユーザ定義型
        /// </summary>
        public static readonly Parser<Model.UserDefinedType> UserDefinedType =
                                                    from enumkey in Parse.String("enum").XOr(Parse.Return("")).Text().TokenWithSkipComment()
                                                    from classkey in Parse.String("class")
                                                                    .Or(Parse.String("struct"))
                                                                    .Or(Parse.String("union"))
                                                                    .XOr(Parse.Return(""))
                                                                    .Text().TokenWithSkipComment()
                                                    from name in NestedName.TokenWithSkipComment()
                                                    where !((enumkey == "enum") && (classkey == "union"))
                                                    select new Model.UserDefinedType(name, enumkey, classkey);

        /// <summary>
        /// ポインタ
        /// </summary>
        public static readonly Parser<Model.Pointer> Pointer =
                                                    from op in Parse.String("*").Text().TokenWithSkipComment()
                                                    from constkeyword in Parse.String("const").XOr(Parse.Return("")).Text().TokenWithSkipComment()
                                                    select new Model.Pointer(constkeyword != "");

        /// <summary>
        /// 変数の型
        /// </summary>
        public static readonly Parser<Model.VariableType> VariableType =
                                                    from premodifiers in Parse.String("signed")
                                                                    .Or(Parse.String("unsigned"))
                                                                    .Or(Parse.String("long"))
                                                                    .Or(Parse.String("short"))
                                                                    .Or(Parse.String("const"))
                                                                    .Or(Parse.String("volatile"))
                                                                    .Text().TokenWithSkipComment().Many()
                                                    from type in UserDefinedType
                                                                    .Or<object>(Parse.String("char").Text())
                                                                    .Or(Parse.String("char16_t").Text())
                                                                    .Or(Parse.String("char32_t").Text())
                                                                    .Or(Parse.String("wchar_t").Text())
                                                                    .Or(Parse.String("bool").Text())
                                                                    .Or(Parse.String("int").Text())
                                                                    .Or(Parse.String("float").Text())
                                                                    .Or(Parse.String("double").Text())
                                                                    .Or(Parse.String("void").Text())
                                                                    .XOr(Parse.Return("").Text())
                                                                    .TokenWithSkipComment()
                                                    from postmodifiers in Parse.String("signed")
                                                                    .Or(Parse.String("unsigned"))
                                                                    .Or(Parse.String("long"))
                                                                    .Or(Parse.String("short"))
                                                                    .Or(Parse.String("const"))
                                                                    .Or(Parse.String("volatile"))
                                                                    .Text().TokenWithSkipComment().Many()
                                                    from pointers in Pointer.TokenWithSkipComment().Many()
                                                    from reference in Parse.String("&&").Or(Parse.String("&")).XOr(Parse.Return("")).Text().TokenWithSkipComment()
                                                    from referenceConst in Parse.String("const").XOr(Parse.Return("")).Text().TokenWithSkipComment()
                                                    select new Model.VariableType(type, premodifiers, postmodifiers, pointers, reference, referenceConst);

        /// <summary>
        /// 変数宣言の式の括弧
        /// </summary>
        private static readonly Parser<Model.IgnoreObjectBlock> VariableDeclarationExpressionParentheses =
                                                    from begin in Parse.String("(").Text().TokenWithSkipComment()
                                                    from objs in IgnoreParser.TokenForExpression
                                                        .Where((obj) =>
                                                            (obj.Content.ToString() != ",")
                                                            && (obj.Content.ToString() != ";")
                                                        )
                                                        .Or(VariableDeclarationExpressionParentheses).TokenWithSkipComment().Many()
                                                    from end in Parse.String(")").Text().TokenWithSkipComment()
                                                    select new Model.IgnoreObjectBlock(
                                                        new Model.IgnoreObject(new Model.OperatorOrPunctuator("(")),
                                                        new Model.IgnoreObject(new Model.OperatorOrPunctuator(")")), objs);
        /// <summary>
        /// 変数宣言の式
        /// </summary>
        private static readonly Parser<IEnumerable<Model.IgnoreObject>> VariableDeclarationExpression =
                                                    from eq in Parse.String("=").Text().TokenWithSkipComment()
                                                    from objs in IgnoreParser.TokenForExpression
                                                        .Where((obj) => 
                                                            (obj.Content.ToString() != ",")
                                                            && (obj.Content.ToString() != ";")
                                                        )
                                                        .Or(VariableDeclarationExpressionParentheses).TokenWithSkipComment().AtLeastOnce()
                                                    select objs;

        /// <summary>
        /// 変数宣言の配列
        /// </summary>
        private static readonly Parser<long> VariableDeclarationArray =
                                                    from begin in Parse.String("[").Text().TokenWithSkipComment()
                                                    from num in TokenParser.IntegerLiteral.XOr(Parse.Return<object>(null)).TokenWithSkipComment()
                                                    from end in Parse.String("]").Text().TokenWithSkipComment()
                                                    let result = (num == null ? -1 : ((Model.IntegerLiteral)num).Value)
                                                    select result;

        /// <summary>
        /// 変数宣言
        /// </summary>
        public static readonly Parser<Model.VariableDeclaration> VariableDeclaration =
                                                    from type in VariableType.TokenWithSkipComment()
                                                    from name in TokenParser.RTCOPIdentifierString.XOr(Parse.Return("")).TokenWithSkipComment()
                                                    from arrays in VariableDeclarationArray.TokenWithSkipComment().Many()
                                                    from dexpression in VariableDeclarationExpression.XOr(Parse.Return(new Model.IgnoreObject[]{ })).TokenWithSkipComment()
                                                    where !((type.Type.ToString() == "void") && (!type.IsPointer))
                                                    select new Model.VariableDeclaration(name, type, arrays, dexpression);

        /// <summary>
        /// パラメータ宣言(複数)
        /// </summary>
        public static readonly Parser<IEnumerable<Model.VariableDeclaration>> ParameterDeclarations =
                                                    from first in VariableDeclaration.TokenWithSkipComment()
                                                    from afters in ParameterDeclarationsAfter.TokenWithSkipComment().Many()
                                                    let result = new List<Model.VariableDeclaration>() { first }.Concat(afters)
                                                    where CheckParameterDefaultExpression(result)
                                                    select result;

        /// <summary>
        /// パラメータ宣言(複数)の後ろの部分
        /// </summary>
        private static readonly Parser<Model.VariableDeclaration> ParameterDeclarationsAfter =
                                                    from comma in Parse.String(",").Text().TokenWithSkipComment()
                                                    from parameter in VariableDeclaration.TokenWithSkipComment()
                                                    select parameter;

        /// <summary>
        /// パラメータの入力が正しいか調べる
        /// </summary>
        /// <param name="paras">パラメータのリスト</param>
        /// <returns>パラメータの入力が正しいかどうか</returns>
        private static bool CheckParameterDefaultExpression(IEnumerable<Model.VariableDeclaration> paras)
        {
            var parameters = paras.ToList();
            bool flag1 = true;
            int n = parameters.Count;
            for (int i = (n - 1); i >= 0; --i)
            {
                bool flag2 = (parameters[i].DefaultExpression.Count > 0);
                // 不正な場合
                if (!flag1 && flag2)
                {
                    // 正しくない
                    return false;
                }
                // 途中でデフォルト値が無くなった場合
                else if (flag1 && !flag2)
                {
                    flag1 = false;
                }
            }
            // ここまで来たら正しい
            return true;
        }

    }
}
