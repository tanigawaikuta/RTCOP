using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Sprache;
using System.Security.Policy;

namespace LayerCompiler.Parsers
{
    /// <summary>
    /// RTCOPのパーサ
    /// </summary>
    static class RTCOPParser
    {
        #region レイヤードメソッド
        /// <summary>
        /// レイヤードなメソッドのブロック
        /// </summary>
        public static readonly Parser<Model.IgnoreObjectBlock> LayerdMethodBlock =
                                                    from beginblock in Parse.String("{").Text().TokenWithSkipComment()
                                                    from contents in TokenParser.RTCOPKeyword.Where((keywood) => 
                                                                        (keywood.Text == "proceed") || (keywood.Text == "layer_members"))
                                                                    .Or<object>(IgnoreParser.IgnoreObject)
                                                                    .TokenWithSkipComment().Many()
                                                    from endblock in Parse.String("}").Text().TokenWithSkipComment()
                                                    from semicolon in Parse.String(";").XOr(Parse.Return("")).Text().TokenWithSkipComment()
                                                    select new Model.IgnoreObjectBlock(
                                                                    new Model.IgnoreObject(new Model.OperatorOrPunctuator("{")),
                                                                    new Model.IgnoreObject(new Model.OperatorOrPunctuator("}")), contents);

        /// <summary>
        /// 純粋仮想関数
        /// </summary>
        private static readonly Parser<string> PureVirtualFunction =
                                                    from eq in Parse.String("=").Text().TokenWithSkipComment()
                                                    from zero in Parse.String("0").Text().TokenWithSkipComment()
                                                    from semicolon in Parse.String(";").Text().TokenWithSkipComment()
                                                    select " = 0 ;";

        /// <summary>
        /// メソッドの定義
        /// </summary>
        public static readonly Parser<Model.LayerdMethodDefinition> MethodDefinition =
                                                    from modifiers in Parse.String("inline")
                                                                    .Or(Parse.String("virtual"))
                                                                    .Or(Parse.String("static"))
                                                                    .Text().TokenWithSkipComment().Many()
                                                    from returnType in CommonParser.VariableType.TokenWithSkipComment()
                                                    from identifier in TokenParser.RTCOPIdentifierString.TokenWithSkipComment()
                                                    from beginparentheses in Parse.String("(").Text().TokenWithSkipComment()
                                                    from parameters in CommonParser.ParameterDeclarations
                                                                    .Or(Parse.String("void").Return(new Model.VariableDeclaration[] { }))
                                                                    .XOr(Parse.Return(new Model.VariableDeclaration[] { }))
                                                                    .TokenWithSkipComment()
                                                    from endparentheses in Parse.String(")").Text().TokenWithSkipComment()
                                                    from thismodifiers in Parse.String("const")
                                                                    .Or(Parse.String("volatile"))
                                                                    .Text().TokenWithSkipComment().Many()
                                                    from noexceptkeyword in Parse.String("noexcept").XOr(Parse.Return("")).Text().TokenWithSkipComment()
                                                    from overridekeyword in Parse.String("override").XOr(Parse.Return("")).Text().TokenWithSkipComment()
                                                    from contents in PureVirtualFunction
                                                                    .Or(Parse.String(";").Text())
                                                                    .Or<object>(LayerdMethodBlock)
                                                                    .TokenWithSkipComment()
                                                    let result = new Model.LayerdMethodDefinition(identifier, returnType, parameters, contents, modifiers, thismodifiers, (noexceptkeyword != ""), null)
                                                    where !(!result.IsVirtual && result.IsPureVirtual)
                                                    select result;

        /// <summary>
        /// パーシャルメソッドの定義
        /// </summary>
        public static readonly Parser<Model.LayerdMethodDefinition> PartialMethodDefinition =
                                                    from copkeyword in Parse.String("partial").Text().TokenWithSkipComment()
                                                    from method in MethodDefinition.TokenWithSkipComment()
                                                    select method.SetIsBase(false);

        /// <summary>
        /// ベースメソッドの定義
        /// </summary>
        public static readonly Parser<Model.LayerdMethodDefinition> BaseMethodDefinition =
                                                    from copkeyword in Parse.String("base").Text().TokenWithSkipComment()
                                                    from method in MethodDefinition.TokenWithSkipComment()
                                                    select method.SetIsBase(true);

        /// <summary>
        /// メソッドの実装
        /// </summary>
        public static readonly Parser<Model.MethodImplementation> MethodImplementation =
                                                    from returnType in CommonParser.VariableType.TokenWithSkipComment()
                                                    from identifier in CommonParser.NestedName.TokenWithSkipComment()
                                                    from beginparentheses in Parse.String("(").Text().TokenWithSkipComment()
                                                    from parameters in CommonParser.ParameterDeclarations
                                                                    .Or(Parse.String("void").Return(new Model.VariableDeclaration[] { }))
                                                                    .XOr(Parse.Return(new Model.VariableDeclaration[] { }))
                                                                    .TokenWithSkipComment()
                                                    from endparentheses in Parse.String(")").Text().TokenWithSkipComment()
                                                    from thismodifiers in Parse.String("const")
                                                                    .Or(Parse.String("volatile"))
                                                                    .Text().TokenWithSkipComment().Many()
                                                    from noexceptkeyword in Parse.String("noexcept").XOr(Parse.Return("")).Text().TokenWithSkipComment()
                                                    from contents in LayerdMethodBlock.TokenWithSkipComment()
                                                    select new Model.MethodImplementation(identifier, returnType, parameters, contents, thismodifiers, (noexceptkeyword != ""));

        #endregion

        #region レイヤードクラス
        /// <summary>
        /// アクセス修飾子
        /// </summary>
        public static readonly Parser<Model.AccessModifier> AccessModifier =
                                                    from kind in Parse.String("public").Or(Parse.String("protected")).Or(Parse.String("private")).Text().TokenWithSkipComment()
                                                    from colon in Parse.String(":").Text().TokenWithSkipComment()
                                                    select new Model.AccessModifier(kind);

        /// <summary>
        /// レイヤ固有メンバ
        /// </summary>
        public static readonly Parser<Model.VariableDeclaration> LayerMemberDeclaration =
                                                    from variable in CommonParser.VariableDeclaration.TokenWithSkipComment()
                                                    from semicolon in Parse.String(";").Text().TokenWithSkipComment()
                                                    select variable;

        /// <summary>
        /// クラス定義
        /// </summary>
        public static readonly Parser<Model.LayerdClassDefinition> ClassDefinition =
                                                    from classkey in Parse.String("class").Or(Parse.String("struct")).Text().TokenWithSkipComment()
                                                    from identifier in TokenParser.RTCOPIdentifierString.TokenWithSkipComment()
                                                    from supers in CommonParser.SuperClassDefinition.XOr(Parse.Return(new Model.SuperClassDefinition[] { })).TokenWithSkipComment()
                                                    from beginblock in Parse.String("{").Text().TokenWithSkipComment()
                                                    from members in BaseClassDefinition
                                                                   .Or<object>(PartialClassDefinition)
                                                                   .Or(NormalClassDefinition)
                                                                   .Or(BaseMethodDefinition)
                                                                   .Or(PartialMethodDefinition)
                                                                   .Or(MethodDefinition)
                                                                   .Or(LayerMemberDeclaration)
                                                                   .Or(AccessModifier)
                                                                   .Or(IgnoreParser.IgnoreObject)
                                                                   .TokenWithSkipComment().Many()
                                                    from endblock in Parse.String("}").Text().TokenWithSkipComment()
                                                    from semicolon in Parse.String(";").Text().TokenWithSkipComment()
                                                    select new Model.LayerdClassDefinition(identifier, classkey, supers, members, true);

        /// <summary>
        /// パーシャルクラスの定義
        /// </summary>
        public static readonly Parser<Model.LayerdClassDefinition> PartialClassDefinition =
                                                    from copkeyword in Parse.String("partial").Text().TokenWithSkipComment()
                                                    from classdef in ClassDefinition.TokenWithSkipComment()
                                                    let result = classdef.SetIsBase(false)
                                                    where CheckClassDefinition(result)
                                                    select result;

        /// <summary>
        /// ベースクラスの定義
        /// </summary>
        public static readonly Parser<Model.LayerdClassDefinition> BaseClassDefinition =
                                                    from copkeyword in Parse.String("base").Text().TokenWithSkipComment()
                                                    from classdef in ClassDefinition.TokenWithSkipComment()
                                                    let result = classdef.SetIsBase(true)
                                                    where CheckClassDefinition(result)
                                                    select result;

        /// <summary>
        /// 通常クラスの定義
        /// </summary>
        public static readonly Parser<Model.LayerdClassDefinition> NormalClassDefinition =
                                                    from classdef in ClassDefinition.TokenWithSkipComment()
                                                    let result = classdef.SetIsBase(null)
                                                    where CheckClassDefinition(result)
                                                    select result;

        /// <summary>
        /// クラス定義が正しいかチェックする
        /// </summary>
        /// <param name="layerdClass">レイヤードなクラス</param>
        /// <returns>クラス定義が正しいかどうか</returns>
        private static bool CheckClassDefinition(Model.LayerdClassDefinition layerdClass)
        {
            // クラスを調べる
            Func<Model.LayerdClassDefinition, bool> checker = null;
            checker = new Func<Model.LayerdClassDefinition, bool>((lc) =>
            {
                var classes = lc.LayerdClassDefinitions;
                foreach (var c in classes)
                {
                    // ネストされたクラスを調べる
                    bool result = checker(c);
                    if (!result) return false;
                    // クラスチェック
                    if ((lc.IsBase != null) && (c.IsBase != null))
                    {
                        if (lc.IsBase.Value && !c.IsBase.Value) return false;
                        else if (!lc.IsBase.Value && c.IsBase.Value) return false;
                    }
                    else if ((lc.IsBase == null) && (c.IsBase != null))
                    {
                        return false;
                    }
                }
                // クラス内のメソッドを調べる
                var methods = lc.MethodDefinitions;
                foreach (var m in methods)
                {
                    if ((lc.IsBase != null) && (m.IsBase != null))
                    {
                        if (lc.IsBase.Value && !m.IsBase.Value) return false;
                        else if (!lc.IsBase.Value && m.IsBase.Value) return false;
                    }
                    else if ((lc.IsBase == null) && (m.IsBase != null))
                    {
                        return false;
                    }
                }
                // ここまで来たら成功
                return true;
            });
            // 結果を返す
            return checker(layerdClass);
        }

        #endregion

        #region 名前空間
        /// <summary>
        /// 名前空間定義
        /// </summary>
        public static readonly Parser<Model.NamespaceDefinition> NamespaceDefinition =
                                                    from inline in Parse.String("inline").XOr(Parse.Return("")).Text().TokenWithSkipComment()
                                                    from keyword in Parse.String("namespace").Text().TokenWithSkipComment()
                                                    from identifier in TokenParser.RTCOPIdentifierString.TokenWithSkipComment()
                                                    from beginblock in Parse.String("{").Text().TokenWithSkipComment()
                                                    from objs in NamespaceDefinition
                                                                   .Or<object>(PartialClassDefinition)
                                                                   .Or(BaseClassDefinition)
                                                                   .Or(ClassDefinition)
                                                                   .Or(MethodImplementation)
                                                                   .Or(IgnoreParser.IgnoreObject)
                                                                   .TokenWithSkipComment().Many()
                                                    from endblock in Parse.String("}").Text().TokenWithSkipComment()
                                                    select new Model.NamespaceDefinition(identifier, objs, (inline != ""));

        #endregion

        #region レイヤ
        /// <summary>
        /// レイヤ定義
        /// </summary>
        public static readonly Parser<Model.LayerDefinition> LayerDefinition =
                                                    from keyword in Parse.String("layer").Text().TokenWithSkipComment()
                                                    from identifier in TokenParser.RTCOPIdentifierString.TokenWithSkipComment()
                                                    from beginblock in Parse.String("{").Text().TokenWithSkipComment()
                                                    from objs in NamespaceDefinition
                                                                   .Or<object>(PartialClassDefinition)
                                                                   .Or(ClassDefinition)
                                                                   .Or(MethodImplementation)
                                                                   .Or(IgnoreParser.IgnoreObject)
                                                                   .TokenWithSkipComment().Many()
                                                    from endblock in Parse.String("}").Text().TokenWithSkipComment()
                                                    let result = new Model.LayerDefinition(identifier, objs)
                                                    where CheckLayerNamespace(result)
                                                    select result;

        /// <summary>
        /// ベースレイヤ定義
        /// </summary>
        public static readonly Parser<Model.LayerDefinition> BaseLayerDefinition =
                                                    from keyword in Parse.String("baselayer").Text().TokenWithSkipComment()
                                                    from beginblock in Parse.String("{").Text().TokenWithSkipComment()
                                                    from objs in NamespaceDefinition
                                                                   .Or<object>(BaseClassDefinition)
                                                                   .Or(ClassDefinition)
                                                                   .Or(MethodImplementation)
                                                                   .Or(IgnoreParser.IgnoreObject)
                                                                   .TokenWithSkipComment().Many()
                                                    from endblock in Parse.String("}").Text().TokenWithSkipComment()
                                                    let result = new Model.LayerDefinition("baselayer", objs)
                                                    where CheckLayerNamespace(result)
                                                    select result;

        /// <summary>
        /// レイヤ内の名前空間が正しいか調べる
        /// </summary>
        /// <param name="layer">レイヤ</param>
        /// <returns>レイヤ内の名前空間が正しいかどうか</returns>
        private static bool CheckLayerNamespace(Model.LayerDefinition layer)
        {
            bool isBaseLayer = (layer.Name == "baselayer");
            // 名前空間を調べる
            Func<Model.NamespaceDefinition, bool> checker = null;
            checker = new Func<Model.NamespaceDefinition, bool>((names) => 
            {
                var nss = names.Namespaces;
                // ネストされた名前空間を調べる
                foreach (var ns in nss)
                {
                    bool result = checker(ns);
                    if (!result) return false;
                }
                // 名前空間内のクラスを調べる
                var classes = names.LayerdClassDefinitions;
                foreach (var c in classes)
                {
                    if (c.IsBase != null)
                    {
                        // ベースレイヤ内にパーシャルクラスがいる場合
                        if (isBaseLayer && !c.IsBase.Value) return false;
                        // レイヤ内にベースクラスがいる場合
                        else if (!isBaseLayer && c.IsBase.Value) return false;
                    }
                }
                // ここまで来たら成功
                return true;
            });
            // レイヤ内の名前空間の取得
            var namespaces = layer.Namespaces;
            foreach (var ns in namespaces)
            {
                bool result = checker(ns);
                if (!result) return false;
            }
            // ここまで来たらOK
            return true;
        }

        #endregion

    }
}
