using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using LayerCompiler.Parsers;
using LayerCompiler.Parsers.Model;
using LayerCompiler.CodeGeneration.Model;

namespace LayerCompiler.CodeGeneration
{
    /// <summary>
    /// RTCOPのコードジェネレータ
    /// </summary>
    partial class RTCOPCodeGenerator
    {
        #region プロパティ
        /// <summary>
        /// レイヤクラスのデフォルトの名前空間
        /// </summary>
        public string Namespace { get; protected set; }

        /// <summary>
        /// 改行文字
        /// </summary>
        public string LineTerminator { get; protected set; }

        /// <summary>
        /// 開発対象
        /// </summary>
        public DevelopmentTarget Target { get; protected set; }

        /// <summary>
        /// 開発環境
        /// </summary>
        public DevelopmentEnvironment Environment { get; protected set; }

        /// <summary>
        /// ヘッダファイルのディレクトリパス
        /// </summary>
        public List<string> IncludePaths { get; private set; }

        #endregion

        #region コンストラクタ
        /// <summary>
        /// RTCOPのコードジェネレータ
        /// </summary>
        /// <param name="nspace">レイヤクラスのデフォルトの名前空間</param>
        /// <param name="lineTerminator">改行文字</param>
        /// <param name="target">開発対象</param>
        /// <param name="environment">開発環境</param>
        /// <param name="includePaths">ヘッダファイルのディレクトリパス</param>
        public RTCOPCodeGenerator(string nspace, string lineTerminator, DevelopmentTarget target, DevelopmentEnvironment environment, List<string> includePaths)
        {
            Namespace = nspace;
            LineTerminator = lineTerminator;
            Target = target;
            Environment = environment;
            IncludePaths = includePaths;
        }

        #endregion

        #region メソッド
        /// <summary>
        /// コード生成
        /// </summary>
        /// <param name="mergedLSFile">マージ済みのレイヤ構造ファイル</param>
        /// <param name="includeFilePath">インクルードファイルパス</param>
        /// <returns>生成後のソースコード</returns>
        public GeneratedCodes GenerateCode(LayerStructureFile mergedLSFile, string includeFilePath)
        {
            GeneratedCodes result = new GeneratedCodes();
            // ライブラリのヘッダファイルを追加
            result.CodeDictionary[@"Layer_Private.h"] = Properties.Resources.Layer_Private_Header;
            result.CodeDictionary[@"LayerdObject_Private.h"] = Properties.Resources.LayerdObject_Private_Header;
            // ベースクラスを調べる
            LayerStructure baselayerStructure = mergedLSFile.BaseLayerStructure;
            if (baselayerStructure == null)
            {
                // ベースレイヤは定義されていなくても、自動的に生成される
                baselayerStructure = new LayerStructure("baselayer");
                mergedLSFile.BaseLayerStructure = baselayerStructure;
            }
            List<string> baseClassNameList = new List<string>();
            List<List<LayerdMethodDefinition>> baseMethodLists = new List<List<LayerdMethodDefinition>>();
            List<List<ConstructorDefinition>> constructorLists = new List<List<ConstructorDefinition>>();
            Action<IEnumerable<object>, string, int> createBaseClassAndMethodList = null;
            createBaseClassAndMethodList = (items, nspace, classId) =>
            {
                foreach (object item in items)
                {
                    if (item is LayerDefinition)
                    {
                        var ld = (LayerDefinition)item;
                        string nspace2 = "baselayer";
                        createBaseClassAndMethodList(ld.Contents, nspace2, -1);
                    }
                    else if (item is NamespaceDefinition)
                    {
                        var nd = (NamespaceDefinition)item;
                        string nspace2 = nd.Name;
                        if (nspace != "") nspace2 = nspace + "::" + nspace2;
                        createBaseClassAndMethodList(nd.Contents, nspace2, -1);
                    }
                    else if (item is LayerdClassDefinition)
                    {
                        var lcd = (LayerdClassDefinition)item;
                        string classname = lcd.Name;
                        if (nspace != "") classname = nspace + "::" + classname;
                        if (!baseClassNameList.Contains(classname))
                        {
                            baseClassNameList.Add(classname);
                            baseMethodLists.Add(new List<LayerdMethodDefinition>());
                            constructorLists.Add(new List<ConstructorDefinition>());
                            int id = baseClassNameList.IndexOf(classname);
                            if (lcd.SuperClasses.Count >= 1)
                            {
                                string superclassFullname = lcd.SuperClasses[0].ClassName;
                                if (nspace != "") superclassFullname = nspace + "::" + classname;
                                string[] superclassNspaces = superclassFullname.Split(new string[] { "::" }, StringSplitOptions.None);
                                string sname = superclassFullname;
                                for (int i = 0; i < superclassNspaces.Length; ++i)
                                {
                                    int id2 = baseClassNameList.IndexOf(lcd.SuperClasses[0].ClassName);
                                    if (id2 != -1)
                                    {
                                        baseMethodLists[id].AddRange(baseMethodLists[id2]);
                                        break;
                                    }
                                    sname = sname.Remove(0, (superclassNspaces[i].Length - 2));
                                }
                            }
                            createBaseClassAndMethodList(lcd.Contents, classname, id);
                            // デストラクタ無ければ追加
                            var baseMethodList = baseMethodLists[id];
                            if (!baseMethodList.Exists((obj) => obj == null))
                            {
                                baseMethodList.Add(null);
                            }
                        }
                        else
                        {
                            throw new Exception("クラス" + classname + "の定義が重複しています");
                        }
                    }
                    else if (item is ConstructorDefinition)
                    {
                        var cd = (ConstructorDefinition)item;
                        var list = constructorLists[classId];
                        list.Add(cd);
                    }
                    else if (item is DestructorDefinition)
                    {
                        var dd = (DestructorDefinition)item;
                        if (dd.IsVirtual || dd.IsPureVirtual)
                        {
                            var methodList = baseMethodLists[classId];
                            if (!methodList.Exists((obj) => obj == null))
                            {
                                methodList.Add(null);
                            }
                        }
                    }
                    else if (item is LayerdMethodDefinition)
                    {
                        if (classId == -1)
                            continue;
                        var lmd = (LayerdMethodDefinition)item;
                        if (((lmd.IsBase != null) && lmd.IsBase.Value) || lmd.IsVirtual || lmd.IsPureVirtual)
                        {
                            var methodList = baseMethodLists[classId];
                            if (!methodList.Exists((obj) => obj.Equals(lmd)))
                            {
                                methodList.Add(lmd);
                            }
                        }
                    }
                }
            };
            createBaseClassAndMethodList(baselayerStructure.HeaderFileItems, "", -1);
            createBaseClassAndMethodList(baselayerStructure.SourceFileItems, "", -1);
            createBaseClassAndMethodList(mergedLSFile.BaseClassHeaderItems, "", -1);
            // レイヤ、クラス、メソッドの数を調べる
            int numOfLayers = mergedLSFile.LayerStructures.Count + 1;
            int numOfClasses = baseClassNameList.Count;
            int[] numOfMethods = new int[numOfClasses];
            for (int i = 0; i < numOfClasses; ++i)
            {
                numOfMethods[i] = baseMethodLists[i].Count;
            }

            //=======================================
            // ↓これらは別ファイルで定義
            //=======================================
            // RTCOPAppInitializerの生成
            GenerateRTCOPAppInitializer(result, numOfLayers, numOfClasses, numOfMethods, mergedLSFile.LayerStructures, includeFilePath);
            // API.h、COPNewForApp.h、ActivationForApp.hの生成
            GenerateAPIHeaders(result, mergedLSFile.LayerStructures, baseClassNameList, includeFilePath);
            // ベースレイヤの生成
            GenerateBaseLayer(result, baselayerStructure, mergedLSFile.LayerStructures, baseClassNameList, includeFilePath);
            // 各レイヤの生成
            GenerateLayers(result, baselayerStructure, mergedLSFile.LayerStructures, baseClassNameList, baseMethodLists, constructorLists, includeFilePath);
            // 環境依存コードの生成
            GenerateDependentCode(result, baselayerStructure, mergedLSFile.LayerStructures, baseClassNameList, baseMethodLists, includeFilePath);

            // 結果を返す
            return result;
        }

        #endregion

    }
}
