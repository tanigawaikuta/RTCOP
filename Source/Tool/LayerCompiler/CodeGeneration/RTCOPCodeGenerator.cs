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
    class RTCOPCodeGenerator
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

        #endregion

        #region コンストラクタ
        /// <summary>
        /// RTCOPのコードジェネレータ
        /// </summary>
        /// <param name="nspace">レイヤクラスのデフォルトの名前空間</param>
        /// <param name="lineTerminator">改行文字</param>
        /// <param name="target">開発対象</param>
        /// <param name="environment">開発環境</param>
        public RTCOPCodeGenerator(string nspace, string lineTerminator, DevelopmentTarget target, DevelopmentEnvironment environment)
        {
            Namespace = nspace;
            LineTerminator = lineTerminator;
            Target = target;
            Environment = environment;
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

        #region 非公開メソッド
        /// <summary>
        /// RTCOPAppInitializerの生成
        /// </summary>
        private void GenerateRTCOPAppInitializer(GeneratedCodes result, int numOfLayers, int numOfClasses, int[] numOfMethods, List<LayerStructure> layerStructures, string includeFilePath)
        {
            // RTCOPAppInitializer.h
            StringBuilder stringBuilderForHeader = new StringBuilder();
            stringBuilderForHeader.AppendLine(@"#ifndef __RTCOP_GENERATED_RTCOPAPPINITIALIZER__");
            stringBuilderForHeader.AppendLine(@"#define __RTCOP_GENERATED_RTCOPAPPINITIALIZER__");
            stringBuilderForHeader.AppendLine();
            stringBuilderForHeader.AppendLine(@"#include ""RTCOP/Core/Initializer.h""");
            stringBuilderForHeader.AppendLine();
            stringBuilderForHeader.AppendLine(@"namespace RTCOP {");
            stringBuilderForHeader.AppendLine();
            stringBuilderForHeader.AppendLine(@"namespace Core {");
            stringBuilderForHeader.AppendLine(@"class RTCOPManager;");
            stringBuilderForHeader.AppendLine(@"class LayerdObjectInitializer;");
            stringBuilderForHeader.AppendLine(@"class LayerActivater;");
            stringBuilderForHeader.AppendLine(@"}");
            stringBuilderForHeader.AppendLine();
            stringBuilderForHeader.AppendLine(@"namespace Generated {");
            stringBuilderForHeader.AppendLine(@"class RTCOPAppInitializer : public Core::Initializer");
            stringBuilderForHeader.AppendLine(@"{");
            stringBuilderForHeader.AppendLine(@"public:");
            stringBuilderForHeader.Append("\t");
            stringBuilderForHeader.AppendLine(@"explicit RTCOPAppInitializer();");
            stringBuilderForHeader.Append("\t");
            stringBuilderForHeader.AppendLine(@"virtual ~RTCOPAppInitializer();");
            stringBuilderForHeader.Append("\t");
            stringBuilderForHeader.AppendLine(@"virtual Core::RTCOPManager* InitializeRTCOPManager() override;");
            stringBuilderForHeader.Append("\t");
            stringBuilderForHeader.AppendLine(@"virtual Core::LayerdObjectInitializer* InitializeLayerdObjectInitializer(Core::RTCOPManager* manager) override;");
            stringBuilderForHeader.Append("\t");
            stringBuilderForHeader.AppendLine(@"virtual Core::LayerActivater* InitializeLayerActivater(Core::RTCOPManager* manager) override;");
            stringBuilderForHeader.Append("\t");
            stringBuilderForHeader.AppendLine(@"virtual void RegisterLayers(Core::RTCOPManager* manager) override;");
            stringBuilderForHeader.Append("\t");
            stringBuilderForHeader.AppendLine(@"virtual void FirstLayerActivation(Core::LayerActivater* activater) override;");
            stringBuilderForHeader.AppendLine(@"};");
            stringBuilderForHeader.AppendLine();
            stringBuilderForHeader.AppendLine(@"} // namespace Generated {}");
            stringBuilderForHeader.AppendLine(@"} // namespace RTCOP {}");
            stringBuilderForHeader.AppendLine(@"#endif // !__RTCOP_GENERATED_RTCOPAPPINITIALIZER__");
            result.CodeDictionary[@"RTCOPAppInitializer.h"] = stringBuilderForHeader.ToString();

            // RTCOPAppInitializer.cpp
            StringBuilder stringBuilderForSource = new StringBuilder();
            stringBuilderForSource.Append(@"#include """);
            stringBuilderForSource.Append(includeFilePath);
            stringBuilderForSource.AppendLine(@"RTCOPAppInitializer.h""");
            stringBuilderForSource.AppendLine(@"#include ""RTCOP/Core/RTCOPManager.h""");
            stringBuilderForSource.AppendLine(@"#include ""RTCOP/Core/LayerdObjectInitializer.h""");
            stringBuilderForSource.AppendLine(@"#include ""RTCOP/Core/LayerActivater.h""");
            stringBuilderForSource.Append(@"#include """);
            stringBuilderForSource.Append(includeFilePath);
            stringBuilderForSource.AppendLine(@"BaseLayer.h""");
            foreach (LayerStructure ls in layerStructures)
            {
                stringBuilderForSource.Append(@"#include """);
                stringBuilderForSource.Append(includeFilePath);
                stringBuilderForSource.Append(ls.LayerName);
                stringBuilderForSource.AppendLine(@".h""");
            }
            stringBuilderForSource.AppendLine();
            stringBuilderForSource.AppendLine(@"namespace RTCOP {");
            stringBuilderForSource.AppendLine(@"namespace Generated {");
            stringBuilderForSource.AppendLine();
            stringBuilderForSource.AppendLine(@"RTCOPAppInitializer::RTCOPAppInitializer()");
            stringBuilderForSource.AppendLine(@"{");
            stringBuilderForSource.AppendLine(@"}");
            stringBuilderForSource.AppendLine();
            stringBuilderForSource.AppendLine(@"RTCOPAppInitializer::~RTCOPAppInitializer()");
            stringBuilderForSource.AppendLine(@"{");
            stringBuilderForSource.AppendLine(@"}");
            stringBuilderForSource.AppendLine();
            stringBuilderForSource.AppendLine(@"Core::RTCOPManager* RTCOPAppInitializer::InitializeRTCOPManager()");
            stringBuilderForSource.AppendLine(@"{");
            stringBuilderForSource.Append("\t");
            stringBuilderForSource.Append(@"int numOfLayers = ");
            stringBuilderForSource.Append(numOfLayers);
            stringBuilderForSource.Append(@";");
            stringBuilderForSource.Append("\t");
            stringBuilderForSource.Append(@"int numOfClasses = ");
            stringBuilderForSource.Append(numOfClasses);
            stringBuilderForSource.Append(@";");
            stringBuilderForSource.Append("\t");
            stringBuilderForSource.Append(@"int* numOfMethods = new int[numOfClasses] {");
            for (int i = 0; i < numOfMethods.Length; ++i)
            {
                int num = numOfMethods[i];
                if (Environment != DevelopmentEnvironment.VisualStudio)
                {
                    num += 1;
                }
                stringBuilderForSource.Append(num);
                if (i < (numOfMethods.Length - 1))
                {
                    stringBuilderForSource.Append(@", ");
                }
            }
            stringBuilderForSource.AppendLine(@"};");
            stringBuilderForSource.Append("\t");
            stringBuilderForSource.AppendLine(@"return new Core::RTCOPManager(numOfLayers, numOfClasses, numOfMethods);");
            stringBuilderForSource.AppendLine(@"}");
            stringBuilderForSource.AppendLine();
            stringBuilderForSource.AppendLine(@"Core::LayerdObjectInitializer* RTCOPAppInitializer::InitializeLayerdObjectInitializer(Core::RTCOPManager* manager)");
            stringBuilderForSource.AppendLine(@"{");
            stringBuilderForSource.Append("\t");
            stringBuilderForSource.AppendLine(@"return new Core::LayerdObjectInitializer(manager);");
            stringBuilderForSource.AppendLine(@"}");
            stringBuilderForSource.AppendLine();
            stringBuilderForSource.AppendLine(@"Core::LayerActivater* RTCOPAppInitializer::InitializeLayerActivater(Core::RTCOPManager* manager)");
            stringBuilderForSource.AppendLine(@"{");
            stringBuilderForSource.Append("\t");
            stringBuilderForSource.AppendLine(@"return new Core::LayerActivater(manager);");
            stringBuilderForSource.AppendLine(@"}");
            stringBuilderForSource.AppendLine();
            stringBuilderForSource.AppendLine(@"void RTCOPAppInitializer::RegisterLayers(Core::RTCOPManager* manager)");
            stringBuilderForSource.AppendLine(@"{");
            stringBuilderForSource.Append("\t");
            stringBuilderForSource.AppendLine(@"int numOfBaseClasses = manager->GetNumOfBaseClasses();");
            stringBuilderForSource.Append("\t");
            stringBuilderForSource.AppendLine(@"int* numOfBaseMethods = manager->GetNumOfBaseMethods();");
            stringBuilderForSource.Append("\t");
            stringBuilderForSource.AppendLine(@"manager->RegisterLayer(new BaseLayer(0, ""baselayer"", numOfBaseClasses, numOfBaseMethods));");
            int layercount = 1;
            foreach (LayerStructure ls in layerStructures)
            {
                stringBuilderForSource.Append("\t");
                stringBuilderForSource.Append(@"manager->RegisterLayer(new ");
                stringBuilderForSource.Append(ls.LayerName);
                stringBuilderForSource.Append(@"(");
                stringBuilderForSource.Append(layercount);
                stringBuilderForSource.Append(@", """);
                stringBuilderForSource.Append(ls.LayerName);
                stringBuilderForSource.AppendLine(@""", numOfBaseClasses, numOfBaseMethods));");
                ++layercount;
            }
            stringBuilderForSource.AppendLine(@"}");
            stringBuilderForSource.AppendLine();
            stringBuilderForSource.AppendLine(@"void RTCOPAppInitializer::FirstLayerActivation(Core::LayerActivater* activater)");
            stringBuilderForSource.AppendLine(@"{");
            stringBuilderForSource.Append("\t");
            stringBuilderForSource.AppendLine(@"activater->ActivateLayer(0);");
            stringBuilderForSource.AppendLine(@"}");
            stringBuilderForSource.AppendLine();
            stringBuilderForSource.AppendLine(@"Core::Initializer* _GetDefaultInitializer_()");
            stringBuilderForSource.AppendLine(@"{");
            stringBuilderForSource.Append("\t");
            stringBuilderForSource.AppendLine(@"return new RTCOPAppInitializer();");
            stringBuilderForSource.AppendLine(@"}");
            stringBuilderForSource.AppendLine();
            stringBuilderForSource.AppendLine(@"} // namespace Generated {}");
            stringBuilderForSource.AppendLine(@"} // namespace RTCOP {}");
            result.CodeDictionary[@"RTCOPAppInitializer.cpp"] = stringBuilderForSource.ToString();
        }

        /// <summary>
        /// API.h、COPNewForApp.h、ActivationForApp.hの生成
        /// </summary>
        private void GenerateAPIHeaders(GeneratedCodes result, List<LayerStructure> layerStructures, List<string> baseClassNameList, string includeFilePath)
        {
            // API.h
            StringBuilder stringBuilderForAPI = new StringBuilder();
            stringBuilderForAPI.AppendLine(@"#ifndef __RTCOP_GENERATED_API__");
            stringBuilderForAPI.AppendLine(@"#define __RTCOP_GENERATED_API__");
            stringBuilderForAPI.AppendLine();
            stringBuilderForAPI.Append(@"#include """);
            stringBuilderForAPI.Append(includeFilePath);
            stringBuilderForAPI.AppendLine(@"COPNewForApp.h""");
            stringBuilderForAPI.Append(@"#include """);
            stringBuilderForAPI.Append(includeFilePath);
            stringBuilderForAPI.AppendLine(@"ActivationForApp.h""");
            stringBuilderForAPI.AppendLine();
            stringBuilderForAPI.AppendLine(@"#endif // !__RTCOP_GENERATED_API__");
            result.CodeDictionary[@"API.h"] = stringBuilderForAPI.ToString();

            // COPNewForApp.h
            StringBuilder stringBuilderForNew = new StringBuilder();
            stringBuilderForNew.AppendLine(@"#ifndef __RTCOP_GENERATED_COPNEWFORAPP__");
            stringBuilderForNew.AppendLine(@"#define __RTCOP_GENERATED_COPNEWFORAPP__");
            stringBuilderForNew.AppendLine();
            stringBuilderForNew.AppendLine(@"#include ""RTCOP/COPNew.h""");
            stringBuilderForNew.AppendLine();
            foreach (string classname in baseClassNameList)
            {
                var nss = classname.Split(new string[] { "::" }, StringSplitOptions.None);
                string nscheck = "";
                for (int i = 0; i < (nss.Length - 1); ++i)
                {
                    if (nscheck == "") nscheck = nss[i];
                    else nscheck += ("::" + nss[i]);
                    if (baseClassNameList.Contains(nscheck))
                    {
                        stringBuilderForNew.Append(@"class ");
                    }
                    else
                    {
                        stringBuilderForNew.Append(@"namespace ");
                    }
                    stringBuilderForNew.Append(nss[i]);
                    stringBuilderForNew.Append(@" { ");
                }
                stringBuilderForNew.Append(@"class ");
                stringBuilderForNew.Append(nss[nss.Length - 1]);
                stringBuilderForNew.Append(@";");
                for (int i = 0; i < (nss.Length - 1); ++i)
                {
                    stringBuilderForNew.Append(@" } ");
                }
                stringBuilderForNew.AppendLine();
            }
            stringBuilderForNew.AppendLine(@"namespace RTCOP {");
            stringBuilderForNew.AppendLine(@"namespace Generated {");
            stringBuilderForNew.AppendLine();
            stringBuilderForNew.AppendLine(@"template<class Base>");
            stringBuilderForNew.AppendLine(@"inline const int GetBaseClassID() { return -1; }");
            int classcount = 0;
            foreach (string classname in baseClassNameList)
            {
                stringBuilderForNew.AppendLine(@"template<>");
                stringBuilderForNew.Append(@"inline const int GetBaseClassID<");
                stringBuilderForNew.Append(classname);
                stringBuilderForNew.Append(@">() { return ");
                stringBuilderForNew.Append(classcount);
                stringBuilderForNew.AppendLine(@"; }");
                ++classcount;
            }
            stringBuilderForNew.AppendLine();
            stringBuilderForNew.AppendLine(@"} // namespace Generated {}");
            stringBuilderForNew.AppendLine();
            stringBuilderForNew.AppendLine(@"template<class Base, class... ArgTypes>");
            stringBuilderForNew.AppendLine(@"inline Core::LayerdObject<Base>* copnew(ArgTypes... args)");
            stringBuilderForNew.AppendLine(@"{");
            stringBuilderForNew.Append("\t");
            stringBuilderForNew.AppendLine(@"const int classId = Generated::GetBaseClassID<Base>();");
            stringBuilderForNew.AppendLine(@"return COPNew<Base>(classId, args...);");
            stringBuilderForNew.AppendLine(@"}");
            stringBuilderForNew.AppendLine();
            stringBuilderForNew.AppendLine(@"} // namespace RTCOP {}");
            stringBuilderForNew.AppendLine(@"#endif // !__RTCOP_GENERATED__COPNEW__");
            result.CodeDictionary[@"COPNewForApp.h"] = stringBuilderForNew.ToString();

            // ActivationForApp.h
            StringBuilder stringBuilderForActivation = new StringBuilder();
            stringBuilderForActivation.AppendLine(@"#ifndef __RTCOP_GENERATED_ACTIVATIONFORAPP__");
            stringBuilderForActivation.AppendLine(@"#define __RTCOP_GENERATED_ACTIVATIONFORAPP__");
            stringBuilderForActivation.AppendLine();
            stringBuilderForActivation.AppendLine(@"#include ""RTCOP/Activation.h""");
            stringBuilderForActivation.AppendLine();
            stringBuilderForActivation.AppendLine(@"namespace RTCOP {");
            stringBuilderForActivation.AppendLine(@"namespace Generated {");
            stringBuilderForActivation.AppendLine();
            stringBuilderForActivation.AppendLine(@"enum class LayerID : int");
            stringBuilderForActivation.AppendLine(@"{");
            stringBuilderForActivation.Append("\t");
            stringBuilderForActivation.AppendLine(@"baselayer = 0,");
            int layercount = 1;
            foreach (LayerStructure ls in layerStructures)
            {
                stringBuilderForActivation.Append("\t");
                stringBuilderForActivation.Append(ls.LayerName);
                stringBuilderForActivation.Append(@" = ");
                stringBuilderForActivation.Append(layercount);
                stringBuilderForActivation.Append(@",");
                ++layercount;
            }
            stringBuilderForActivation.AppendLine(@"};");
            stringBuilderForActivation.AppendLine(@"} // namespace Generated {}");
            stringBuilderForActivation.AppendLine();
            stringBuilderForActivation.AppendLine(@"inline void activate(Generated::LayerID layerid)");
            stringBuilderForActivation.AppendLine(@"{");
            stringBuilderForActivation.AppendLine("\t");
            stringBuilderForActivation.AppendLine(@"Activate((int)layerid);");
            stringBuilderForActivation.AppendLine(@"}");
            stringBuilderForActivation.AppendLine();
            stringBuilderForActivation.AppendLine(@"inline void deactivate(Generated::LayerID layerid)");
            stringBuilderForActivation.AppendLine(@"{");
            stringBuilderForActivation.AppendLine("\t");
            stringBuilderForActivation.AppendLine(@"Deactivate((int)layerid);");
            stringBuilderForActivation.AppendLine(@"}");
            stringBuilderForActivation.AppendLine();
            stringBuilderForActivation.AppendLine(@"} // namespace RTCOP {}");
            stringBuilderForActivation.AppendLine(@"#endif // !__RTCOP_GENERATED_ACTIVATION__");
            result.CodeDictionary[@"ActivationForApp.h"] = stringBuilderForActivation.ToString();
        }

        /// <summary>
        /// ベースレイヤ
        /// </summary>
        private void GenerateBaseLayer(GeneratedCodes result, LayerStructure baseLayerStructure, List<LayerStructure> layerStructures, List<string> baseClassNameList, string includeFilePath)
        {
            // 共通処理
            var headerIncludeFiles = from item in baseLayerStructure.HeaderFileItems
                                     where (item is PreprocessDirective) && (((PreprocessDirective)item).Kind == DirectiveKind.Include)
                                     select (PreprocessDirective)item;
            var headerImportFiles = from item in baseLayerStructure.HeaderFileItems
                                    where (item is PreprocessDirective) && (((PreprocessDirective)item).Kind == DirectiveKind.ImportLayerHeader)
                                    select (PreprocessDirective)item;
            var srcIncludeFiles = from item in baseLayerStructure.SourceFileItems
                                  where (item is PreprocessDirective) && (((PreprocessDirective)item).Kind == DirectiveKind.Include)
                                  select (PreprocessDirective)item;
            var srcImportFiles = from item in baseLayerStructure.SourceFileItems
                                 where (item is PreprocessDirective) && (((PreprocessDirective)item).Kind == DirectiveKind.ImportLayerHeader)
                                 select (PreprocessDirective)item;
            // ヘッダファイル
            StringBuilder stringBuilderForHeader = new StringBuilder();
            stringBuilderForHeader.AppendLine(@"#ifndef __RTCOP_GENERATED_BASELAYER__");
            stringBuilderForHeader.AppendLine(@"#define __RTCOP_GENERATED_BASELAYER__");
            stringBuilderForHeader.AppendLine();
            stringBuilderForHeader.AppendLine(@"#include ""RTCOP/Core/Layer.h""");
            stringBuilderForHeader.AppendLine(@"#include ""RTCOP/Core/LayerdObject.h""");
            stringBuilderForHeader.AppendLine();
            var headerIncludeFilesArray = headerIncludeFiles.ToArray();
            foreach (var incFile in headerIncludeFilesArray)
            {
                stringBuilderForHeader.AppendLine(incFile.ToString());
                baseLayerStructure.HeaderFileItems.Remove(incFile);
            }
            stringBuilderForHeader.AppendLine();
            var headerImportFilesArray = headerImportFiles.ToArray();
            List<string> layerNamesH = new List<string>();
            foreach (var impFile in headerImportFilesArray)
            {
                string fullPath = Path.GetFullPath(impFile.Param1);
                foreach (var ls in layerStructures)
                {
                    if (layerNamesH.Contains(ls.LayerName))
                    {
                        continue;
                    }
                    if (ls.ImportedLhInfomation.Exists((obj) => obj.FilePath == fullPath))
                    {
                        stringBuilderForHeader.Append(@"#include """);
                        stringBuilderForHeader.Append(includeFilePath);
                        stringBuilderForHeader.Append(ls.LayerName);
                        stringBuilderForHeader.AppendLine(@".h""");
                        layerNamesH.Add(ls.LayerName);
                    }
                }
                baseLayerStructure.HeaderFileItems.Remove(impFile);
            }
            stringBuilderForHeader.AppendLine();
            stringBuilderForHeader.AppendLine(@"namespace RTCOP {");
            stringBuilderForHeader.AppendLine(@"namespace Generated {");
            stringBuilderForHeader.AppendLine();
            stringBuilderForHeader.AppendLine(@"class BaseLayer : public Core::Layer");
            stringBuilderForHeader.AppendLine(@"{");
            stringBuilderForHeader.AppendLine(@"public:");
            stringBuilderForHeader.Append("\t");
            stringBuilderForHeader.AppendLine(@"static BaseLayer* GetInstance();");
            stringBuilderForHeader.Append("\t");
            stringBuilderForHeader.AppendLine(@"explicit BaseLayer(const int id, const char* const name, int numOfBaseClasses, int* numOfBaseMethods);");
            stringBuilderForHeader.Append("\t");
            stringBuilderForHeader.AppendLine(@"virtual ~BaseLayer() override;");
            stringBuilderForHeader.AppendLine(@"protected:");
            stringBuilderForHeader.Append("\t");
            stringBuilderForHeader.AppendLine(@"virtual void* InitializeLayerdObject(void* obj, int classID) override;");
            stringBuilderForHeader.AppendLine(@"};");
            stringBuilderForHeader.AppendLine();
            stringBuilderForHeader.AppendLine(@"} // namespace Generated {}");
            stringBuilderForHeader.AppendLine(@"} // namespace RTCOP {}");
            stringBuilderForHeader.AppendLine();
            stringBuilderForHeader.AppendLine(ItemsToStringForBaseLayer(baseLayerStructure.HeaderFileItems, 0));
            stringBuilderForHeader.AppendLine();
            stringBuilderForHeader.AppendLine(@"#endif //!__RTCOP_GENERATED_JAPANESELAYER__");
            result.CodeDictionary[@"BaseLayer.h"] = stringBuilderForHeader.ToString();

            // ソースファイル
            StringBuilder stringBuilderForSource = new StringBuilder();
            stringBuilderForSource.Append(@"#include """);
            stringBuilderForSource.Append(includeFilePath);
            stringBuilderForSource.AppendLine(@"API.h""");
            stringBuilderForSource.Append(@"#include """);
            stringBuilderForSource.Append(includeFilePath);
            stringBuilderForSource.AppendLine(@"Layer_Private.h""");
            stringBuilderForSource.Append(@"#include """);
            stringBuilderForSource.Append(includeFilePath);
            stringBuilderForSource.AppendLine(@"LayerdObject_Private.h""");
            stringBuilderForSource.Append(@"#include """);
            stringBuilderForSource.Append(includeFilePath);
            stringBuilderForSource.AppendLine(@"DependentCode.h""");
            stringBuilderForSource.AppendLine(@"#include ""RTCOP/Framework.h""");
            stringBuilderForSource.AppendLine(@"#include ""RTCOP/Core/RTCOPManager.h""");
            stringBuilderForSource.AppendLine(@"#include ""RTCOP/Core/PartialClassMembers.h""");
            stringBuilderForSource.AppendLine(@"#include <cstring>");
            stringBuilderForSource.AppendLine();
            var srcIncludeFilesArray = srcIncludeFiles.ToArray();
            foreach (var incFile in srcIncludeFilesArray)
            {
                stringBuilderForSource.AppendLine(incFile.ToString());
                baseLayerStructure.SourceFileItems.Remove(incFile);
            }
            stringBuilderForSource.AppendLine();
            stringBuilderForSource.Append(@"#include """);
            stringBuilderForSource.Append(includeFilePath);
            stringBuilderForSource.AppendLine(@"BaseLayer.h""");
            var srcImportFilesArray = srcImportFiles.ToArray();
            List<string> layerNamesS = new List<string>();
            foreach (var impFile in srcImportFilesArray)
            {
                string fullPath = Path.GetFullPath(impFile.Param1);
                foreach (var ls in layerStructures)
                {
                    if (layerNamesS.Contains(ls.LayerName))
                    {
                        continue;
                    }
                    if (ls.ImportedLhInfomation.Exists((obj) => obj.FilePath == fullPath))
                    {
                        stringBuilderForSource.Append(@"#include """);
                        stringBuilderForSource.Append(includeFilePath);
                        stringBuilderForSource.Append(ls.LayerName);
                        stringBuilderForSource.AppendLine(@".h""");
                        layerNamesS.Add(ls.LayerName);
                    }
                }
                baseLayerStructure.SourceFileItems.Remove(impFile);
            }
            stringBuilderForSource.AppendLine();
            stringBuilderForSource.AppendLine(@"namespace RTCOP {");
            stringBuilderForSource.AppendLine(@"namespace Generated {");
            stringBuilderForSource.AppendLine();
            stringBuilderForSource.AppendLine(@"BaseLayer* BaseLayer::GetInstance()");
            stringBuilderForSource.AppendLine(@"{");
            stringBuilderForSource.Append("\t");
            stringBuilderForSource.AppendLine(@"return (BaseLayer*)RTCOP::Framework::Instance->GetRTCOPManager()->GetLayer((int)LayerID::baselayer);");
            stringBuilderForSource.AppendLine(@"}");
            stringBuilderForSource.AppendLine();
            stringBuilderForSource.AppendLine(@"BaseLayer::BaseLayer(const int id, const char* const name, int numOfBaseClasses, int* numOfBaseMethods)");
            stringBuilderForSource.Append("\t");
            stringBuilderForSource.AppendLine(@":Core::Layer(id, name, numOfBaseClasses, numOfBaseMethods)");
            stringBuilderForSource.AppendLine(@"{");
            int classcount = 0;
            foreach (string classname in baseClassNameList)
            {
                stringBuilderForSource.Append("\t");
                stringBuilderForSource.Append(@"int size");
                stringBuilderForSource.Append(classcount);
                stringBuilderForSource.Append(@" = sizeof(volatile void*) * numOfBaseMethods[");
                stringBuilderForSource.Append(classcount);
                stringBuilderForSource.AppendLine(@"];");
                stringBuilderForSource.Append("\t");
                stringBuilderForSource.Append(@"volatile void** virtualFunctionTable");
                stringBuilderForSource.Append(classcount);
                stringBuilderForSource.Append(@" = DependentCode::");
                stringBuilderForSource.Append(classname);
                stringBuilderForSource.AppendLine(@"::GetVirtualFunctionTable(this);");
                stringBuilderForSource.Append("\t");
                stringBuilderForSource.Append(@"std::memcpy(_Private->_VirtualFunctionTables[");
                stringBuilderForSource.Append(classcount);
                stringBuilderForSource.Append(@"], virtualFunctionTable");
                stringBuilderForSource.Append(classcount);
                stringBuilderForSource.Append(@", size");
                stringBuilderForSource.Append(classcount);
                stringBuilderForSource.AppendLine(@");");
                ++classcount;
            }
            stringBuilderForSource.AppendLine(@"}");
            stringBuilderForSource.AppendLine();
            stringBuilderForSource.AppendLine(@"BaseLayer::~BaseLayer()");
            stringBuilderForSource.AppendLine(@"{");
            stringBuilderForSource.AppendLine(@"}");
            stringBuilderForSource.AppendLine();
            stringBuilderForSource.AppendLine(@"void* BaseLayer::InitializeLayerdObject(void* obj, int classID)");
            stringBuilderForSource.AppendLine(@"{");
            stringBuilderForSource.Append("\t");
            stringBuilderForSource.AppendLine(@"return obj;");
            stringBuilderForSource.AppendLine(@"}");
            stringBuilderForSource.AppendLine();
            stringBuilderForSource.AppendLine(@"} // namespace Generated {}");
            stringBuilderForSource.AppendLine(@"} // namespace RTCOP {}");
            stringBuilderForSource.AppendLine();
            stringBuilderForSource.AppendLine(ItemsToStringForBaseLayer(baseLayerStructure.SourceFileItems, 0));
            stringBuilderForSource.AppendLine();
            result.CodeDictionary[@"BaseLayer.cpp"] = stringBuilderForSource.ToString();
        }

        /// <summary>
        /// 要素の集合を文字列に変換
        /// </summary>
        private string ItemsToStringForBaseLayer(IEnumerable<object> items, int indent)
        {
            StringBuilder stringBuilder = new StringBuilder();
            bool indentFlag = true;
            Action<int> addIndent = (indentCount) =>
            {
                for (int i = 0; i < indentCount; ++i)
                {
                    stringBuilder.Append("\t");
                }
            };
            foreach (var item in items)
            {
                if (item is LayerDefinition)
                {
                    var ld = (LayerDefinition)item;
                    addIndent(indent);
                    stringBuilder.Append(@"namespace ");
                    stringBuilder.AppendLine(ld.Name);
                    addIndent(indent);
                    stringBuilder.AppendLine(@"{");
                    stringBuilder.AppendLine(ItemsToStringForBaseLayer(ld.Contents, indent));
                    addIndent(indent);
                    stringBuilder.AppendLine(@"}");
                    indentFlag = true;
                }
                else if (item is NamespaceDefinition)
                {
                    var nd = (NamespaceDefinition)item;
                    addIndent(indent);
                    if (nd.IsInline) stringBuilder.Append(@"inline ");
                    stringBuilder.Append(@"namespace ");
                    stringBuilder.AppendLine(nd.Name);
                    addIndent(indent);
                    stringBuilder.AppendLine(@"{");
                    stringBuilder.AppendLine(ItemsToStringForBaseLayer(nd.Contents, indent));
                    addIndent(indent);
                    stringBuilder.AppendLine(@"}");
                    indentFlag = true;
                }
                else if (item is LayerdClassDefinition)
                {
                    var lcd = (LayerdClassDefinition)item;
                    addIndent(indent);
                    stringBuilder.Append(lcd.ClassKey);
                    stringBuilder.Append(" ");
                    stringBuilder.Append(lcd.Name);
                    if (lcd.SuperClasses.Count > 0)
                    {
                        stringBuilder.Append(" : ");
                        stringBuilder.Append(string.Join(", ", lcd.SuperClasses));
                    }
                    stringBuilder.AppendLine();
                    addIndent(indent);
                    stringBuilder.AppendLine(@"{");
                    if (!lcd.Contents.Exists((obj) => obj is ConstructorDefinition))
                    {
                        ConstructorDefinition constructor = new ConstructorDefinition(lcd.Name, new VariableDeclaration[] { }, new IgnoreObjectBlock("{", "}", new object[] { }), new string[] { }, false);
                        lcd.Contents.Add(constructor);
                    }
                    stringBuilder.AppendLine(ItemsToStringForBaseLayer(lcd.Contents, indent + 1));
                    addIndent(indent);
                    stringBuilder.AppendLine(@"};");
                    indentFlag = true;
                }
                else if (item is LayerdMethodDefinition)
                {
                    if (item is EventHandlerDefinition)
                    {
                    }
                    var lmd = (LayerdMethodDefinition)item;
                    addIndent(indent);
                    if (lmd.IsBase != null && !lmd.IsVirtual)
                    {
                        stringBuilder.Append(@"virtual ");
                    }
                    foreach (string modifier in lmd.Modifiers)
                    {
                        stringBuilder.Append(modifier);
                        stringBuilder.Append(@" ");
                    }
                    stringBuilder.Append(lmd.ReturnType);
                    stringBuilder.Append(@" ");
                    stringBuilder.Append(lmd.Name);
                    stringBuilder.Append(@" (");
                    stringBuilder.Append(string.Join(", ", lmd.Parameters));
                    stringBuilder.Append(@")");
                    foreach (string modifier in lmd.ThisModifiers)
                    {
                        stringBuilder.Append(@" ");
                        stringBuilder.Append(modifier);
                    }
                    if (lmd.IsNoexcept) stringBuilder.Append(@" noexcept");
                    if (lmd.Contents is IgnoreObjectBlock)
                    {
                        stringBuilder.AppendLine();
                        stringBuilder.AppendLine(ItemsToStringForBaseLayer(((IgnoreObjectBlock)lmd.Contents).Contents, indent));
                    }
                    else
                    {
                        stringBuilder.AppendLine(lmd.Contents.ToString());
                    }
                    indentFlag = true;
                }
                else if (item is MethodImplementation)
                {
                    var mi = (MethodImplementation)item;
                    addIndent(indent);
                    stringBuilder.Append(mi.ReturnType);
                    stringBuilder.Append(@" ");
                    stringBuilder.Append(mi.FullName);
                    stringBuilder.Append(@" (");
                    stringBuilder.Append(string.Join(@", ", mi.Parameters));
                    stringBuilder.Append(@")");
                    foreach (string modifier in mi.ThisModifiers)
                    {
                        stringBuilder.Append(@" ");
                        stringBuilder.Append(modifier);
                    }
                    if (mi.IsNoexcept) stringBuilder.Append(@" noexcept");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine(ItemsToStringForBaseLayer(mi.Contents.Contents, indent));
                    indentFlag = true;
                }
                else if (item is VariableDeclaration)
                {
                    addIndent(indent);
                    stringBuilder.AppendLine(item.ToString());
                    indentFlag = true;
                }
                else if (item is IgnoreObjectBlock)
                {
                    var iob = (IgnoreObjectBlock)item;
                    stringBuilder.AppendLine(ItemsToStringForBaseLayer(iob.Contents, indent));
                    indentFlag = true;
                }
                else if (item is IgnoreObject)
                {
                    var io = (IgnoreObject)item;
                    if (indentFlag) addIndent(indent);
                    if (io.Content is PreprocessDirective)
                    {
                        var directive = io.Content as PreprocessDirective;
                        if (directive.Kind == DirectiveKind.ImportBaseClassHeader || directive.Kind == DirectiveKind.ImportLayerHeader)
                        {
                            // 読み飛ばす
                        }
                        else
                        {
                            stringBuilder.AppendLine(io.Content.ToString());
                            indentFlag = true;
                        }
                    }
                    else
                    {
                        string text = io.Content.ToString();
                        if (text == "{")
                        {
                            stringBuilder.AppendLine(text);
                            indent += 1;
                            indentFlag = true;
                        }
                        else if (text == "}")
                        {
                            stringBuilder.AppendLine(text);
                            indent -= 1;
                            indentFlag = true;
                        }
                        else if ((text == ";") || (text == ":"))
                        {
                            stringBuilder.AppendLine(text);
                            indentFlag = true;
                        }
                        else
                        {
                            stringBuilder.Append(text);
                            stringBuilder.Append(" ");
                            indentFlag = false;
                        }
                    }
                }
                else
                {
                    addIndent(indent);
                    stringBuilder.AppendLine(item.ToString());
                }
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 各種レイヤ
        /// </summary>
        private void GenerateLayers(GeneratedCodes result, LayerStructure baseLayerStructure, List<LayerStructure> layerStructures, List<string> baseClassNameList, List<List<LayerdMethodDefinition>> baseMethodLists, List<List<ConstructorDefinition>> constructorLists, string includeFilePath)
        {
            int layerID = 1;
            foreach (LayerStructure layerStructure in layerStructures)
            {
                // 共通
                var headerIncludeFiles = from item in layerStructure.HeaderFileItems
                                         where (item is PreprocessDirective) && (((PreprocessDirective)item).Kind == DirectiveKind.Include)
                                         select (PreprocessDirective)item;
                var headerImportFiles = from item in layerStructure.HeaderFileItems
                                        where (item is PreprocessDirective) && (((PreprocessDirective)item).Kind == DirectiveKind.ImportLayerHeader)
                                        select (PreprocessDirective)item;
                var srcIncludeFiles = from item in layerStructure.SourceFileItems
                                      where (item is PreprocessDirective) && (((PreprocessDirective)item).Kind == DirectiveKind.Include)
                                      select (PreprocessDirective)item;
                var srcImportFiles = from item in layerStructure.SourceFileItems
                                     where (item is PreprocessDirective) && (((PreprocessDirective)item).Kind == DirectiveKind.ImportLayerHeader)
                                     select (PreprocessDirective)item;

                // ヘッダファイル
                StringBuilder stringBuilderForHeader = new StringBuilder();
                stringBuilderForHeader.Append(@"#ifndef __RTCOP_GENERATED_");
                stringBuilderForHeader.Append(layerStructure.LayerName.ToUpper());
                stringBuilderForHeader.AppendLine(@"__");
                stringBuilderForHeader.Append(@"#define __RTCOP_GENERATED_");
                stringBuilderForHeader.Append(layerStructure.LayerName.ToUpper());
                stringBuilderForHeader.AppendLine(@"__");
                stringBuilderForHeader.AppendLine();
                stringBuilderForHeader.AppendLine(@"#include ""RTCOP/Core/Layer.h""");
                stringBuilderForHeader.AppendLine(@"#include ""RTCOP/Core/LayerdObject.h""");
                stringBuilderForHeader.AppendLine(@"#include ""RTCOP/Core/PartialClassMembers.h""");
                stringBuilderForHeader.AppendLine();
                var headerIncludeFilesArray = headerIncludeFiles.ToArray();
                foreach (var incFile in headerIncludeFilesArray)
                {
                    stringBuilderForHeader.AppendLine(incFile.ToString());
                    layerStructure.HeaderFileItems.Remove(incFile);
                }
                stringBuilderForHeader.AppendLine();
                var headerImportFilesArray = headerImportFiles.ToArray();
                List<string> layerNamesH = new List<string>() { layerStructure.LayerName };
                stringBuilderForHeader.Append(@"#include """);
                stringBuilderForHeader.Append(includeFilePath);
                stringBuilderForHeader.AppendLine(@"BaseLayer.h""");
                layerNamesH.Add("baselayer");
                foreach (var impFile in headerImportFilesArray)
                {
                    string fullPath = Path.GetFullPath(impFile.Param1);
                    foreach (var ls in layerStructures)
                    {
                        if (layerNamesH.Contains(ls.LayerName))
                        {
                            continue;
                        }
                        if (ls.ImportedLhInfomation.Exists((obj) => obj.FilePath == fullPath))
                        {
                            stringBuilderForHeader.Append(@"#include """);
                            stringBuilderForHeader.Append(includeFilePath);
                            stringBuilderForHeader.Append(ls.LayerName);
                            stringBuilderForHeader.AppendLine(@".h""");
                            layerNamesH.Add(ls.LayerName);
                        }
                    }
                    layerStructure.HeaderFileItems.Remove(impFile);
                }
                stringBuilderForHeader.AppendLine();
                stringBuilderForHeader.AppendLine(@"namespace RTCOP {");
                stringBuilderForHeader.AppendLine(@"namespace Generated {");
                stringBuilderForHeader.AppendLine();
                stringBuilderForHeader.Append(@"class ");
                stringBuilderForHeader.Append(layerStructure.LayerName);
                stringBuilderForHeader.AppendLine(@" : public RTCOP::Core::Layer");
                stringBuilderForHeader.AppendLine(@"{");
                stringBuilderForHeader.AppendLine(@"public:");
                stringBuilderForHeader.Append("\t");
                stringBuilderForHeader.Append(@"static ");
                stringBuilderForHeader.Append(layerStructure.LayerName);
                stringBuilderForHeader.AppendLine(@"* GetInstance();");
                stringBuilderForHeader.Append("\t");
                stringBuilderForHeader.Append(@"explicit ");
                stringBuilderForHeader.Append(layerStructure.LayerName);
                stringBuilderForHeader.AppendLine(@"(const int id, const char* const name, int numOfBaseClasses, int* numOfBaseMethods);");
                stringBuilderForHeader.Append("\t");
                stringBuilderForHeader.Append(@"virtual ~");
                stringBuilderForHeader.Append(layerStructure.LayerName);
                stringBuilderForHeader.AppendLine(@"() override;");
                stringBuilderForHeader.AppendLine(@"protected:");
                stringBuilderForHeader.Append("\t");
                stringBuilderForHeader.AppendLine(@"virtual void* InitializeLayerdObject(void* obj, int classID) override;");
                stringBuilderForHeader.Append("\t");
                stringBuilderForHeader.AppendLine(@"virtual void _RTCOP_OnActivating() override;");
                stringBuilderForHeader.Append("\t");
                stringBuilderForHeader.AppendLine(@"virtual void _RTCOP_OnActivated() override;");
                stringBuilderForHeader.Append("\t");
                stringBuilderForHeader.AppendLine(@"virtual void _RTCOP_OnDeactivating() override;");
                stringBuilderForHeader.Append("\t");
                stringBuilderForHeader.AppendLine(@"virtual void _RTCOP_OnDeactivated() override;");
                stringBuilderForHeader.AppendLine(@"};");
                stringBuilderForHeader.AppendLine();
                stringBuilderForHeader.AppendLine(@"} // namespace Generated {}");
                stringBuilderForHeader.AppendLine(@"} // namespace RTCOP {}");
                stringBuilderForHeader.AppendLine();
                stringBuilderForHeader.AppendLine(ItemsToStringForLayers(layerStructure.HeaderFileItems, layerStructure, layerID, baseClassNameList, constructorLists, 0, "", false));
                stringBuilderForHeader.AppendLine();
                stringBuilderForHeader.AppendLine(@"#endif");
                result.CodeDictionary[layerStructure.LayerName + @".h"] = stringBuilderForHeader.ToString();

                // ソースファイル
                StringBuilder stringBuilderForSource = new StringBuilder();
                stringBuilderForSource.Append(@"#include """);
                stringBuilderForSource.Append(includeFilePath);
                stringBuilderForSource.AppendLine(@"API.h""");
                stringBuilderForSource.Append(@"#include """);
                stringBuilderForSource.Append(includeFilePath);
                stringBuilderForSource.AppendLine(@"Layer_Private.h""");
                stringBuilderForSource.Append(@"#include """);
                stringBuilderForSource.Append(includeFilePath);
                stringBuilderForSource.AppendLine(@"LayerdObject_Private.h""");
                stringBuilderForSource.AppendLine(@"#include ""RTCOP/Framework.h""");
                stringBuilderForSource.AppendLine(@"#include ""RTCOP/Core/RTCOPManager.h""");
                stringBuilderForSource.Append(@"#include """);
                stringBuilderForSource.Append(includeFilePath);
                stringBuilderForSource.AppendLine(@"DependentCode.h""");
                stringBuilderForSource.AppendLine(@"#include <iostream>");
                stringBuilderForSource.AppendLine(@"#include <cstring>");
                stringBuilderForSource.AppendLine();
                var srcIncludeFilesArray = srcIncludeFiles.ToArray();
                foreach (var incFile in srcIncludeFilesArray)
                {
                    stringBuilderForSource.AppendLine(incFile.ToString());
                    layerStructure.SourceFileItems.Remove(incFile);
                }
                stringBuilderForSource.AppendLine();
                var srcImportFilesArray = srcImportFiles.ToArray();
                List<string> layerNamesS = new List<string>() { layerStructure.LayerName };
                foreach (var impFile in srcImportFilesArray)
                {
                    string fullPath = Path.GetFullPath(impFile.Param1);
                    if (!layerNamesS.Contains("baselayer") && baseLayerStructure.ImportedLhInfomation.Exists((obj) => obj.FilePath == fullPath))
                    {
                        stringBuilderForSource.Append(@"#include """);
                        stringBuilderForSource.Append(includeFilePath);
                        stringBuilderForSource.AppendLine(@"BaseLayer.h""");
                        layerNamesS.Add("baselayer");
                    }
                    foreach (var ls in layerStructures)
                    {
                        if (layerNamesS.Contains(ls.LayerName))
                        {
                            continue;
                        }
                        if (ls.ImportedLhInfomation.Exists((obj) => obj.FilePath == fullPath))
                        {
                            stringBuilderForSource.Append(@"#include """);
                            stringBuilderForSource.Append(includeFilePath);
                            stringBuilderForSource.Append(ls.LayerName);
                            stringBuilderForSource.AppendLine(@".h""");
                            layerNamesS.Add(ls.LayerName);
                        }
                    }
                    layerStructure.SourceFileItems.Remove(impFile);
                }
                stringBuilderForSource.Append(@"#include """);
                stringBuilderForSource.Append(includeFilePath);
                stringBuilderForSource.Append(layerStructure.LayerName);
                stringBuilderForSource.AppendLine(@".h""");
                stringBuilderForSource.AppendLine();
                stringBuilderForSource.AppendLine(@"namespace RTCOP {");
                stringBuilderForSource.AppendLine(@"namespace Generated {");
                stringBuilderForSource.AppendLine();
                stringBuilderForSource.Append(layerStructure.LayerName);
                stringBuilderForSource.Append(@"* ");
                stringBuilderForSource.Append(layerStructure.LayerName);
                stringBuilderForSource.AppendLine(@"::GetInstance()");
                stringBuilderForSource.AppendLine(@"{");
                stringBuilderForSource.Append("\t");
                stringBuilderForSource.Append(@"return (");
                stringBuilderForSource.Append(layerStructure.LayerName);
                stringBuilderForSource.Append(@"*)RTCOP::Framework::Instance->GetRTCOPManager()->GetLayer((int)LayerID::");
                stringBuilderForSource.Append(layerStructure.LayerName);
                stringBuilderForSource.AppendLine(@");");
                stringBuilderForSource.AppendLine(@"}");
                stringBuilderForSource.AppendLine();
                stringBuilderForSource.Append(layerStructure.LayerName);
                stringBuilderForSource.Append(@"::");
                stringBuilderForSource.Append(layerStructure.LayerName);
                stringBuilderForSource.AppendLine(@"(const int id, const char* const name, int numOfBaseClasses, int* numOfBaseMethods)");
                stringBuilderForSource.Append("\t");
                stringBuilderForSource.AppendLine(@":RTCOP::Core::Layer(id, name, numOfBaseClasses, numOfBaseMethods)");
                stringBuilderForSource.AppendLine(@"{");
                int classCount = 0;
                List<LayerdClassDefinition> findedPartialClasses = new List<LayerdClassDefinition>();
                foreach (string baseClassName in baseClassNameList)
                {
                    stringBuilderForSource.Append("\t");
                    stringBuilderForSource.Append(@"int size");
                    stringBuilderForSource.Append(classCount);
                    stringBuilderForSource.Append(@" = sizeof(volatile void*) * numOfBaseMethods[");
                    stringBuilderForSource.Append(classCount);
                    stringBuilderForSource.AppendLine(@"];");
                    stringBuilderForSource.Append(@"volatile void** virtualFunctionTable");
                    stringBuilderForSource.Append(classCount);
                    stringBuilderForSource.Append("\t");
                    stringBuilderForSource.Append(@" = DependentCode::");
                    stringBuilderForSource.Append(baseClassName);
                    stringBuilderForSource.AppendLine(@"::GetVirtualFunctionTable(this);");
                    stringBuilderForSource.Append("\t");
                    stringBuilderForSource.Append(@"std::memcpy(_Private->_VirtualFunctionTables[");
                    stringBuilderForSource.Append(classCount);
                    stringBuilderForSource.Append(@"], virtualFunctionTable");
                    stringBuilderForSource.Append(classCount);
                    stringBuilderForSource.Append(@", size");
                    stringBuilderForSource.Append(classCount);
                    stringBuilderForSource.AppendLine(@");");
                    string[] nss = baseClassName.Split(new string[] { "::" }, StringSplitOptions.None);
                    var lds = new List<LayerDefinition>(layerStructure.HeaderFileItems.FindAll((obj) => obj is LayerDefinition).Cast<LayerDefinition>());
                    lds.AddRange(layerStructure.SourceFileItems.FindAll((obj) => obj is LayerDefinition).Cast<LayerDefinition>());
                    Func<object, int, LayerdClassDefinition> FindPartialClass = null;
                    FindPartialClass = (layerOrNamespaceOrClass, nsindex) =>
                    {
                        if (nsindex >= nss.Length)
                        {
                            return null;
                        }
                        string ns = nss[nsindex];
                        if (layerOrNamespaceOrClass is LayerDefinition)
                        {
                            var lditem = layerOrNamespaceOrClass as LayerDefinition;
                            var cditem = lditem.LayerdClassDefinitions.SingleOrDefault((obj) => obj.Name == ns);
                            if (cditem != null)
                            {
                                if (nsindex == (nss.Length - 1))
                                {
                                    return cditem;
                                }
                                else
                                {
                                    var resultOfFindPartialClass = FindPartialClass(cditem, nsindex + 1);
                                    if (resultOfFindPartialClass != null)
                                    {
                                        return resultOfFindPartialClass;
                                    }
                                }
                            }
                            var nsitems = from nsitem in lditem.Namespaces
                                          where nsitem.Name == ns
                                          select nsitem;
                            foreach (var nsitem in nsitems)
                            {
                                var resultOfFindPartialClass = FindPartialClass(nsitem, nsindex + 1);
                                if (resultOfFindPartialClass != null)
                                {
                                    return resultOfFindPartialClass;
                                }
                            }
                        }
                        else if (layerOrNamespaceOrClass is NamespaceDefinition)
                        {
                            var nsitem = layerOrNamespaceOrClass as NamespaceDefinition;
                            var cditem = nsitem.LayerdClassDefinitions.SingleOrDefault((obj) => obj.Name == ns);
                            if (cditem != null)
                            {
                                if (nsindex == (nss.Length - 1))
                                {
                                    return cditem;
                                }
                                else
                                {
                                    var resultOfFindPartialClass = FindPartialClass(cditem, nsindex + 1);
                                    if (resultOfFindPartialClass != null)
                                    {
                                        return resultOfFindPartialClass;
                                    }
                                }
                            }
                            var nsitems2 = from nsitem2 in nsitem.Namespaces
                                           where nsitem2.Name == ns
                                           select nsitem2;
                            foreach (var nsitem2 in nsitems2)
                            {
                                var resultOfFindPartialClass = FindPartialClass(nsitem2, nsindex + 1);
                                if (resultOfFindPartialClass != null)
                                {
                                    return resultOfFindPartialClass;
                                }
                            }
                        }
                        else if (layerOrNamespaceOrClass is LayerdClassDefinition)
                        {
                            var cditem = layerOrNamespaceOrClass as LayerdClassDefinition;
                            var cditem2 = cditem.LayerdClassDefinitions.SingleOrDefault((obj) => obj.Name == ns);
                            if (cditem2 != null)
                            {
                                if (nsindex == (nss.Length - 1))
                                {
                                    return cditem2;
                                }
                                else
                                {
                                    var resultOfFindPartialClass = FindPartialClass(cditem2, nsindex + 1);
                                    if (resultOfFindPartialClass != null)
                                    {
                                        return resultOfFindPartialClass;
                                    }
                                }
                            }
                        }
                        return null;
                    };
                    LayerdClassDefinition findedPartialClass = null;
                    int nsi = 0;
                    if (nss[0] == "baselayer") nsi = 1;
                    foreach (var ld in lds)
                    {
                        findedPartialClass = FindPartialClass(ld, nsi);
                        if (findedPartialClass != null)
                        {
                            break;
                        }
                    }
                    if (findedPartialClass != null)
                    {
                        var baseMethodList = baseMethodLists[classCount];
                        int methodCount = 0;
                        int offset = 0;
                        foreach (var baseMethod in baseMethodList)
                        {
                            if (baseMethod == null)
                            {
                                stringBuilderForSource.Append("\t");
                                stringBuilderForSource.Append(@"_Private->_VirtualFunctionTables[");
                                stringBuilderForSource.Append(classCount);
                                stringBuilderForSource.Append(@"][");
                                stringBuilderForSource.Append(methodCount + offset);
                                stringBuilderForSource.AppendLine(@"] = 0;");
                                if (Environment != DevelopmentEnvironment.VisualStudio)
                                {
                                    offset = 1;
                                    stringBuilderForSource.Append("\t");
                                    stringBuilderForSource.Append(@"_Private->_VirtualFunctionTables[");
                                    stringBuilderForSource.Append(classCount);
                                    stringBuilderForSource.Append(@"][");
                                    stringBuilderForSource.Append(methodCount + offset);
                                    stringBuilderForSource.AppendLine(@"] = 0;");
                                }
                                continue;
                            }
                            var partialMethod = findedPartialClass.MethodDefinitions.SingleOrDefault((obj) => obj.Equals(baseMethod));
                            if (partialMethod == null)
                            {
                                stringBuilderForSource.Append("\t");
                                stringBuilderForSource.Append(@"_Private->_VirtualFunctionTables[");
                                stringBuilderForSource.Append(classCount);
                                stringBuilderForSource.Append(@"][");
                                stringBuilderForSource.Append(methodCount + offset);
                                stringBuilderForSource.AppendLine(@"] = 0;");
                            }
                            ++methodCount;
                        }
                    }
                    findedPartialClasses.Add(findedPartialClass);
                    ++classCount;
                }
                stringBuilderForSource.AppendLine(@"}");
                stringBuilderForSource.AppendLine();
                stringBuilderForSource.Append(layerStructure.LayerName);
                stringBuilderForSource.Append(@"::~");
                stringBuilderForSource.Append(layerStructure.LayerName);
                stringBuilderForSource.AppendLine(@"()");
                stringBuilderForSource.AppendLine(@"{");
                stringBuilderForSource.AppendLine(@"}");
                stringBuilderForSource.AppendLine();
                stringBuilderForSource.Append(@"void* ");
                stringBuilderForSource.Append(layerStructure.LayerName);
                stringBuilderForSource.AppendLine(@"::InitializeLayerdObject(void* obj, int classID)");
                stringBuilderForSource.AppendLine(@"{");
                stringBuilderForSource.Append("\t");
                stringBuilderForSource.AppendLine(@"int layerID = _Private->_ID;");
                for (int i = 0; i < baseClassNameList.Count; ++i)
                {
                    string baseClassName = baseClassNameList[i];
                    if (baseClassName.StartsWith("baselayer::"))
                    {
                        baseClassName = baseClassName.Substring("baselayer::".Length);
                    }
                    stringBuilderForSource.Append("\t");
                    if (i == 0)
                    {
                        stringBuilderForSource.AppendLine(@"if (classID == 0)");
                    }
                    else
                    {
                        stringBuilderForSource.Append(@"else if (classID == ");
                        stringBuilderForSource.Append(i);
                        stringBuilderForSource.AppendLine(@")");
                    }
                    stringBuilderForSource.Append("\t");
                    stringBuilderForSource.AppendLine(@"{");
                    stringBuilderForSource.Append("\t\t");
                    stringBuilderForSource.Append(@"::");
                    stringBuilderForSource.Append(layerStructure.LayerName);
                    stringBuilderForSource.Append(@"::");
                    stringBuilderForSource.Append(baseClassName);
                    stringBuilderForSource.Append(@"* layerdObject = reinterpret_cast<::");
                    stringBuilderForSource.Append(layerStructure.LayerName);
                    stringBuilderForSource.Append(@"::");
                    stringBuilderForSource.Append(baseClassName);
                    stringBuilderForSource.AppendLine(@"*>(obj);");
                    stringBuilderForSource.Append("\t\t");
                    stringBuilderForSource.Append(@"layerdObject->_Private->_PartialClassMembers[layerID] = new ::");
                    stringBuilderForSource.Append(layerStructure.LayerName);
                    stringBuilderForSource.Append(@"::");
                    stringBuilderForSource.Append(baseClassName);
                    stringBuilderForSource.AppendLine(@"::PartialClassMembers();");
                    stringBuilderForSource.Append("\t\t");
                    stringBuilderForSource.AppendLine(@"layerdObject->_Private->_PartialClassMembers[layerID]->_Layer = this;");
                    stringBuilderForSource.Append("\t\t");
                    stringBuilderForSource.AppendLine(@"layerdObject->_Private->_PartialClassMembers[layerID]->_VirtualFunctionTableForProceeding = _Private->_VirtualFunctionTablesForProceeding[classID];");
                    stringBuilderForSource.Append("\t\t");
                    stringBuilderForSource.AppendLine(@"volatile void* vfp = DependentCode::GetLayerdObjectFinalizer(layerdObject);");
                    stringBuilderForSource.Append("\t\t");
                    stringBuilderForSource.AppendLine(@"layerdObject->_Private->_PartialClassMembers[layerID]->_Finalizer = vfp;");
                    stringBuilderForSource.Append("\t\t");
                    stringBuilderForSource.AppendLine(@"layerdObject->_RTCOP_InitializePartialClass();");
                    stringBuilderForSource.Append("\t");
                    stringBuilderForSource.AppendLine(@"}");
                }
                stringBuilderForSource.Append("\t");
                stringBuilderForSource.AppendLine(@"return obj;");
                stringBuilderForSource.AppendLine(@"}");
                stringBuilderForSource.AppendLine();
                // 未実装のイベントハンドラ
                stringBuilderForSource.Append(@"void ");
                stringBuilderForSource.Append(layerStructure.LayerName);
                stringBuilderForSource.AppendLine(@"::_RTCOP_OnActivating()");
                stringBuilderForSource.AppendLine(@"{");
                stringBuilderForSource.AppendLine(@"}");
                stringBuilderForSource.AppendLine();
                stringBuilderForSource.Append(@"void ");
                stringBuilderForSource.Append(layerStructure.LayerName);
                stringBuilderForSource.AppendLine(@"::_RTCOP_OnActivated()");
                stringBuilderForSource.AppendLine(@"{");
                stringBuilderForSource.AppendLine(@"}");
                stringBuilderForSource.AppendLine();
                stringBuilderForSource.Append(@"void ");
                stringBuilderForSource.Append(layerStructure.LayerName);
                stringBuilderForSource.AppendLine(@"::_RTCOP_OnDeactivating()");
                stringBuilderForSource.AppendLine(@"{");
                stringBuilderForSource.AppendLine(@"}");
                stringBuilderForSource.AppendLine();
                stringBuilderForSource.Append(@"void ");
                stringBuilderForSource.Append(layerStructure.LayerName);
                stringBuilderForSource.AppendLine(@"::_RTCOP_OnDeactivated()");
                stringBuilderForSource.AppendLine(@"{");
                stringBuilderForSource.AppendLine(@"}");
                stringBuilderForSource.AppendLine();
                stringBuilderForSource.AppendLine(@"} // namespace Generated {}");
                stringBuilderForSource.AppendLine(@"} // namespace RTCOP {}");
                stringBuilderForSource.AppendLine();
                stringBuilderForSource.AppendLine(ItemsToStringForLayers(layerStructure.SourceFileItems, layerStructure, layerID, baseClassNameList, constructorLists, 0, "", false));
                stringBuilderForSource.AppendLine();
                stringBuilderForSource.Append(@"namespace ");
                stringBuilderForSource.Append(layerStructure.LayerName);
                stringBuilderForSource.AppendLine(@"{");
                for (int i = 0; i < baseClassNameList.Count; ++i)
                {
                    string baseClassName = baseClassNameList[i];
                    LayerdClassDefinition findedPartialClass = findedPartialClasses[i];
                    if (findedPartialClass != null)
                    {
                        ConstructorDefinition constructor = constructorLists[i].Last();
                        string[] nss = baseClassName.Split(new string[] { "::" }, StringSplitOptions.None);
                        string nscheck = "";
                        string className = "";
                        int nsCount = 0;
                        for (int j = 0; j < nss.Length; ++j)
                        {
                            if (nscheck == "") nscheck = nss[j];
                            else nscheck += ("::" + nss[j]);
                            if (baseClassNameList.Contains(nscheck))
                            {
                                if (className == "") className = nss[j];
                                else className += ("::" + nss[j]);
                            }
                            else
                            {
                                if (className != "") throw new Exception();
                                if (nscheck == "baselayer")
                                {
                                    continue;
                                }
                                stringBuilderForSource.Append(@"namespace ");
                                stringBuilderForSource.Append(nss[j]);
                                stringBuilderForSource.Append(@" { ");
                                ++nsCount;
                            }
                        }
                        stringBuilderForSource.AppendLine();
                        stringBuilderForSource.Append(className);
                        stringBuilderForSource.Append(@"::");
                        stringBuilderForSource.Append(className);
                        stringBuilderForSource.Append(@"(");
                        stringBuilderForSource.Append(string.Join(", ", constructor.Parameters));
                        stringBuilderForSource.AppendLine(@")");
                        stringBuilderForSource.Append("\t");
                        stringBuilderForSource.Append(@": RTCOP::Core::LayerdObject<");
                        if (nss[0] != "baselayer")
                        {
                            stringBuilderForSource.Append(@"::");
                        }
                        stringBuilderForSource.Append(baseClassName);
                        stringBuilderForSource.Append(@">(");
                        var paramNames = from param in constructor.Parameters
                                         select param.Name;
                        stringBuilderForSource.Append(string.Join(", ", paramNames));
                        stringBuilderForSource.AppendLine(@")");
                        stringBuilderForSource.AppendLine(@"{");
                        stringBuilderForSource.AppendLine(@"}");
                        stringBuilderForSource.AppendLine();
                        stringBuilderForSource.Append(@"void ");
                        stringBuilderForSource.Append(className);
                        stringBuilderForSource.AppendLine(@"::_RTCOP_InitializePartialClass()");
                        stringBuilderForSource.AppendLine(@"{");
                        var initializers = from eventHandler in findedPartialClass.EventHandlerDefinitions
                                           where eventHandler.EventName == "Initialize"
                                           select eventHandler;
                        foreach (var initializer in initializers)
                        {
                            stringBuilderForSource.Append("\t");
                            stringBuilderForSource.Append(initializer.Name);
                            stringBuilderForSource.AppendLine(@"();");
                        }
                        stringBuilderForSource.AppendLine(@"}");
                        stringBuilderForSource.AppendLine();
                        stringBuilderForSource.Append(@"void ");
                        stringBuilderForSource.Append(className);
                        stringBuilderForSource.AppendLine(@"::_RTCOP_FinalizePartialClass()");
                        stringBuilderForSource.AppendLine(@"{");
                        var finalizers = from eventHandler in findedPartialClass.EventHandlerDefinitions
                                         where eventHandler.EventName == "Finalize"
                                         select eventHandler;
                        foreach (var finalizer in finalizers)
                        {
                            stringBuilderForSource.Append("\t");
                            stringBuilderForSource.Append(finalizer.Name);
                            stringBuilderForSource.AppendLine(@"();");
                        }
                        stringBuilderForSource.AppendLine(@"}");
                        stringBuilderForSource.AppendLine();
                        for (int k = 0; k < nsCount; ++k)
                        {
                            stringBuilderForSource.Append(@" } ");
                        }
                        stringBuilderForSource.AppendLine();
                    }
                }
                stringBuilderForSource.AppendLine(@"}");
                stringBuilderForSource.AppendLine();
                result.CodeDictionary[layerStructure.LayerName + @".cpp"] = stringBuilderForSource.ToString();

                layerStructure._FindedPartialClasses = findedPartialClasses;
                ++layerID;
            }
        }

        /// <summary>
        /// 要素の集合を文字列に変換
        /// </summary>
        private string ItemsToStringForLayers(IEnumerable<object> items, LayerStructure layerStructure, int layerId, List<string> baseClassNameList, List<List<ConstructorDefinition>> constructorLists, int indent, string ns, bool inLayer)
        {
            StringBuilder stringBuilder = new StringBuilder();
            bool indentFlag = true;
            Action<int> addIndent = (indentCount) =>
            {
                for (int i = 0; i < indentCount; ++i)
                {
                    stringBuilder.Append("\t");
                }
            };
            foreach (var item in items)
            {
                if (item is LayerDefinition)
                {
                    var ld = (LayerDefinition)item;
                    addIndent(indent);
                    stringBuilder.Append(@"namespace ");
                    stringBuilder.AppendLine(ld.Name);
                    addIndent(indent);
                    stringBuilder.AppendLine(@"{");
                    stringBuilder.AppendLine(ItemsToStringForLayers(ld.Contents, layerStructure, layerId, baseClassNameList, constructorLists, indent, ns, true));
                    addIndent(indent);
                    stringBuilder.AppendLine(@"}");
                    indentFlag = true;
                }
                else if (item is NamespaceDefinition)
                {
                    var nd = (NamespaceDefinition)item;
                    addIndent(indent);
                    if (nd.IsInline) stringBuilder.Append(@"inline ");
                    stringBuilder.Append(@"namespace ");
                    stringBuilder.AppendLine(nd.Name);
                    addIndent(indent);
                    stringBuilder.AppendLine(@"{");
                    string newns = "";
                    if (ns == "")
                    {
                        newns = nd.Name;
                    }
                    else
                    {
                        newns = ns + "::" + nd.Name;
                    }
                    stringBuilder.AppendLine(ItemsToStringForLayers(nd.Contents, layerStructure, layerId, baseClassNameList, constructorLists, indent, newns, inLayer));
                    addIndent(indent);
                    stringBuilder.AppendLine(@"}");
                    indentFlag = true;
                }
                else if (item is LayerdClassDefinition)
                {
                    var lcd = (LayerdClassDefinition)item;
                    string newns = "";
                    if (ns == "")
                    {
                        newns = lcd.Name;
                    }
                    else
                    {
                        newns = ns + "::" + lcd.Name;
                    }
                    addIndent(indent);
                    stringBuilder.Append(lcd.ClassKey);
                    stringBuilder.Append(@" ");
                    stringBuilder.Append(lcd.Name);
                    if (lcd.IsBase != null && !lcd.IsBase.Value)
                    {
                        stringBuilder.Append(@" : public RTCOP::Core::LayerdObject<");
                        string superName = newns;
                        if (!baseClassNameList.Contains(superName))
                        {
                            superName = "baselayer::" + superName;
                            stringBuilder.Append(superName);
                        }
                        else
                        {
                            stringBuilder.Append(@"::");
                            stringBuilder.Append(superName);
                        }
                        stringBuilder.AppendLine(@">");
                        addIndent(indent);
                        stringBuilder.AppendLine(@"{");
                        addIndent(indent);
                        stringBuilder.AppendLine(@"public:");
                        addIndent(indent + 1);
                        stringBuilder.Append(@"friend RTCOP::Generated::");
                        stringBuilder.Append(layerStructure.LayerName);
                        stringBuilder.AppendLine(@";");
                        addIndent(indent + 1);
                        stringBuilder.AppendLine(@"class PartialClassMembers : public RTCOP::Core::PartialClassMembers");
                        addIndent(indent + 1);
                        stringBuilder.AppendLine(@"{");
                        addIndent(indent + 1);
                        stringBuilder.AppendLine(@"public:");
                        var members = lcd.LayerMemberDeclaration.ToArray();
                        foreach (var member in members)
                        {
                            addIndent(indent + 2);
                            stringBuilder.Append(member.ToString());
                            stringBuilder.AppendLine(@";");
                            lcd.Contents.Remove(member);
                        }
                        addIndent(indent + 1);
                        stringBuilder.AppendLine(@"};");
                        int id = baseClassNameList.IndexOf(newns);
                        if (id == -1)
                        {
                            // 再検索
                            id = baseClassNameList.IndexOf("baselayer::" + newns);
                        }
                        if (id != -1)
                        {
                            var constructorList = constructorLists[id];
                            ConstructorDefinition constructor = null;
                            foreach (var c in constructorList)
                            {
                                if (constructor == null)
                                {
                                    constructor = c;
                                }
                                else if (c.Parameters.Count < constructor.Parameters.Count)
                                {
                                    constructor = c;
                                }
                            }
                            ConstructorDefinition constructor2 = new ConstructorDefinition(constructor.Name, constructor.Parameters, ";", constructor.Modifiers, constructor.IsNoexcept);
                            constructorList.Add(constructor2);
                            stringBuilder.Append("\t");
                            stringBuilder.AppendLine(constructor2.ToString());
                        }
                        else
                        {
                            throw new Exception(layerStructure.LayerName + "::" + newns + "のベースクラスが見つかりません。");
                        }
                        addIndent(indent);
                        stringBuilder.AppendLine(@"private:");
                        addIndent(indent + 1);
                        stringBuilder.AppendLine(@"void _RTCOP_InitializePartialClass();");
                        addIndent(indent + 1);
                        stringBuilder.AppendLine(@"void _RTCOP_FinalizePartialClass();");
                        if (lcd.ClassKey == "struct")
                        {
                            addIndent(indent);
                            stringBuilder.AppendLine(@"public:");
                        }
                        stringBuilder.AppendLine(ItemsToStringForLayers(lcd.Contents, layerStructure, layerId, baseClassNameList, constructorLists, indent + 1, newns, inLayer));
                        addIndent(indent);
                        stringBuilder.AppendLine(@"};");
                    }
                    else
                    {
                        if (lcd.SuperClasses.Count > 0)
                        {
                            stringBuilder.Append(" : ");
                            stringBuilder.Append(string.Join(", ", lcd.SuperClasses));
                        }
                        stringBuilder.AppendLine();
                        addIndent(indent);
                        stringBuilder.AppendLine(@"{");
                        stringBuilder.AppendLine(ItemsToStringForLayers(lcd.Contents, layerStructure, layerId, baseClassNameList, constructorLists, indent + 1, newns, inLayer));
                        addIndent(indent);
                        stringBuilder.AppendLine(@"};");
                    }
                    indentFlag = true;
                }
                else if (item is LayerdMethodDefinition)
                {
                    if (item is EventHandlerDefinition)
                    {
                    }
                    var lmd = (LayerdMethodDefinition)item;
                    addIndent(indent);
                    if (lmd.IsBase != null && !lmd.IsVirtual)
                    {
                        stringBuilder.Append(@"virtual ");
                    }
                    foreach (string modifier in lmd.Modifiers)
                    {
                        stringBuilder.Append(modifier);
                        stringBuilder.Append(@" ");
                    }
                    stringBuilder.Append(lmd.ReturnType);
                    stringBuilder.Append(@" ");
                    stringBuilder.Append(lmd.Name);
                    stringBuilder.Append(@" (");
                    stringBuilder.Append(string.Join(", ", lmd.Parameters));
                    stringBuilder.Append(@")");
                    foreach (string modifier in lmd.ThisModifiers)
                    {
                        stringBuilder.Append(@" ");
                        stringBuilder.Append(modifier);
                    }
                    if (lmd.IsNoexcept) stringBuilder.Append(@" noexcept");
                    if (lmd.Contents is IgnoreObjectBlock)
                    {
                        stringBuilder.AppendLine();
                        stringBuilder.AppendLine(ItemsToStringForLayers(((IgnoreObjectBlock)lmd.Contents).Contents, layerStructure, layerId, baseClassNameList, constructorLists, indent, ns + "::" + lmd.Name, inLayer));
                    }
                    else
                    {
                        stringBuilder.AppendLine(lmd.Contents.ToString());
                    }
                    indentFlag = true;
                }
                else if (item is MethodImplementation)
                {
                    var mi = (MethodImplementation)item;
                    addIndent(indent);
                    stringBuilder.Append(mi.ReturnType);
                    stringBuilder.Append(@" ");
                    stringBuilder.Append(mi.FullName);
                    stringBuilder.Append(@" (");
                    stringBuilder.Append(string.Join(@", ", mi.Parameters));
                    stringBuilder.Append(@")");
                    foreach (string modifier in mi.ThisModifiers)
                    {
                        stringBuilder.Append(@" ");
                        stringBuilder.Append(modifier);
                    }
                    if (mi.IsNoexcept) stringBuilder.Append(@" noexcept");
                    stringBuilder.AppendLine();
                    if (inLayer)
                    {
                        addIndent(indent);
                        stringBuilder.AppendLine(@"{");
                        addIndent(indent + 1);
                        stringBuilder.Append(mi.ClassName);
                        stringBuilder.Append(@"::PartialClassMembers* layer_members = (");
                        stringBuilder.Append(mi.ClassName);
                        stringBuilder.Append(@"::PartialClassMembers*)_Private->_PartialClassMembers[");
                        stringBuilder.Append(layerId);
                        stringBuilder.AppendLine(@"];");
                        addIndent(indent + 1);
                        stringBuilder.Append(@"auto proceed = [this, layer_members](");
                        int n = mi.Parameters.Count;
                        if (n > 0)
                        {
                            stringBuilder.Append(mi.Parameters[0].Type);
                            stringBuilder.Append(" ");
                            stringBuilder.Append(mi.Parameters[0].Name);
                            for (int i = 1; i < n; ++i)
                            {
                                stringBuilder.Append(", ");
                                stringBuilder.Append(mi.Parameters[0].Type);
                                stringBuilder.Append(" ");
                                stringBuilder.Append(mi.Parameters[0].Name);
                            }
                        }
                        stringBuilder.Append(@") { RTCOP::Generated::DependentCode::");
                        string newns = "";
                        if (ns == "")
                        {
                            newns = mi.ClassName;
                        }
                        else
                        {
                            newns = ns + "::" + mi.ClassName;
                        }
                        int classId = -1;
                        classId = baseClassNameList.IndexOf(newns);
                        if (classId == -1)
                        {
                            classId = baseClassNameList.IndexOf("baselayer::" + newns);
                            stringBuilder.Append(@"baselayer::");
                        }
                        stringBuilder.Append(mi.ClassName);
                        stringBuilder.Append(@"::ExecuteProceed_");
                        stringBuilder.Append(mi.MethodName);
                        stringBuilder.Append(@"(this, layer_members->_VirtualFunctionTableForProceeding[");
                        stringBuilder.Append(classId);
                        stringBuilder.Append(@"]");
                        foreach (var param in mi.Parameters)
                        {
                            stringBuilder.Append(@", ");
                            stringBuilder.Append(param.Name);
                        }
                        stringBuilder.AppendLine(@"); };");
                        List<object> contents = mi.Contents.Contents.ToList();
                        contents.Remove(contents.Last());
                        contents.Remove(contents.First());
                        stringBuilder.AppendLine(ItemsToStringForLayers(contents, layerStructure, layerId, baseClassNameList, constructorLists, indent + 1, ns + "::" + mi.FullName, inLayer));
                        addIndent(indent);
                        stringBuilder.AppendLine(@"}");
                    }
                    else
                    {
                        stringBuilder.AppendLine(ItemsToStringForLayers(mi.Contents.Contents, layerStructure, layerId, baseClassNameList, constructorLists, indent, ns + "::" + mi.FullName, inLayer));
                    }
                    indentFlag = true;
                }
                else if (item is VariableDeclaration)
                {
                    addIndent(indent);
                    stringBuilder.AppendLine(item.ToString());
                    indentFlag = true;
                }
                else if (item is IgnoreObjectBlock)
                {
                    var iob = (IgnoreObjectBlock)item;
                    stringBuilder.AppendLine(ItemsToStringForLayers(iob.Contents, layerStructure, layerId, baseClassNameList, constructorLists, indent, ns, inLayer));
                    indentFlag = true;
                }
                else if (item is IgnoreObject)
                {
                    var io = (IgnoreObject)item;
                    if (indentFlag) addIndent(indent);
                    if (io.Content is PreprocessDirective)
                    {
                        var directive = io.Content as PreprocessDirective;
                        if (directive.Kind == DirectiveKind.ImportBaseClassHeader || directive.Kind == DirectiveKind.ImportLayerHeader)
                        {
                            // 読み飛ばす
                        }
                        else
                        {
                            stringBuilder.AppendLine(io.Content.ToString());
                            indentFlag = true;
                        }
                    }
                    else
                    {
                        string text = io.Content.ToString();
                        if (text == "{")
                        {
                            stringBuilder.AppendLine(text);
                            indent += 1;
                            indentFlag = true;
                        }
                        else if (text == "}")
                        {
                            stringBuilder.AppendLine(text);
                            indent -= 1;
                            indentFlag = true;
                        }
                        else if ((text == ";") || (text == ":"))
                        {
                            stringBuilder.AppendLine(text);
                            indentFlag = true;
                        }
                        else
                        {
                            stringBuilder.Append(text);
                            stringBuilder.Append(" ");
                            indentFlag = false;
                        }
                    }
                }
                else if (item is Keyword)
                {
                    if (indentFlag) addIndent(indent);
                    stringBuilder.Append(item.ToString());
                    stringBuilder.Append(" ");
                    indentFlag = false;
                }
                else
                {
                    addIndent(indent);
                    stringBuilder.AppendLine(item.ToString());
                    indentFlag = true;
                }
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 環境依存コード
        /// </summary>
        private void GenerateDependentCode(GeneratedCodes result, LayerStructure baseLayerStructure, List<LayerStructure> layerStructures, List<string> baseClassNameList, List<List<LayerdMethodDefinition>> baseMethodLists, string includeFilePath)
        {
            // ヘッダファイル
            StringBuilder stringBuilderForHeader = new StringBuilder();
            stringBuilderForHeader.AppendLine(@"#ifndef __RTCOP_GENERATED_DEPENDENTCODE__");
            stringBuilderForHeader.AppendLine(@"#define __RTCOP_GENERATED_DEPENDENTCODE__");
            stringBuilderForHeader.AppendLine();
            foreach (var baseClassName in baseClassNameList)
            {
                var nss = baseClassName.Split(new string[] { "::" }, StringSplitOptions.None);
                string nscheck = "";
                for (int i = 0; i < (nss.Length - 1); ++i)
                {
                    if (nscheck == "") nscheck = nss[i];
                    else nscheck += ("::" + nss[i]);
                    if (baseClassNameList.Contains(nscheck))
                    {
                        stringBuilderForHeader.Append(@"class ");
                    }
                    else
                    {
                        stringBuilderForHeader.Append(@"namespace ");
                    }
                    stringBuilderForHeader.Append(nss[i]);
                    stringBuilderForHeader.Append(@" { ");
                }
                stringBuilderForHeader.Append(@"class ");
                stringBuilderForHeader.Append(nss[nss.Length - 1]);
                stringBuilderForHeader.Append(@";");
                for (int i = 0; i < (nss.Length - 1); ++i)
                {
                    stringBuilderForHeader.Append(@" } ");
                }
                stringBuilderForHeader.AppendLine();
                foreach (var layerStructure in layerStructures)
                {
                    stringBuilderForHeader.Append(@"namespace ");
                    stringBuilderForHeader.Append(layerStructure.LayerName);
                    stringBuilderForHeader.Append(@" { ");
                    string cname = baseClassName;
                    if (baseClassName.StartsWith("baselayer::"))
                        cname = baseClassName.Substring("baselayer::".Length);
                    var nss2 = cname.Split(new string[] { "::" }, StringSplitOptions.None);
                    nscheck = "";
                    string nscheck2 = "baselayer";
                    for (int i = 0; i < (nss2.Length - 1); ++i)
                    {
                        if (nscheck == "") nscheck = nss2[i];
                        else nscheck += ("::" + nss2[i]);
                        nscheck2 += ("::" + nss2[i]);
                        if (baseClassNameList.Contains(nscheck) || baseClassNameList.Contains(nscheck2))
                        {
                            stringBuilderForHeader.Append(@"class ");
                        }
                        else
                        {
                            stringBuilderForHeader.Append(@"namespace ");
                        }
                        stringBuilderForHeader.Append(nss2[i]);
                        stringBuilderForHeader.Append(@" { ");
                    }
                    stringBuilderForHeader.Append(@"class ");
                    stringBuilderForHeader.Append(nss2[nss2.Length - 1]);
                    stringBuilderForHeader.Append(@";");
                    for (int i = 0; i < nss2.Length; ++i)
                    {
                        stringBuilderForHeader.Append(@" } ");
                    }
                    stringBuilderForHeader.AppendLine();
                }
            }
            stringBuilderForHeader.AppendLine();
            stringBuilderForHeader.AppendLine(@"namespace RTCOP {");
            stringBuilderForHeader.AppendLine(@"namespace Generated {");
            stringBuilderForHeader.AppendLine();
            stringBuilderForHeader.AppendLine(@"class BaseLayer;");
            foreach (var layerStructure in layerStructures)
            {
                stringBuilderForHeader.Append(@"class ");
                stringBuilderForHeader.Append(layerStructure.LayerName);
                stringBuilderForHeader.AppendLine(@";");
            }
            stringBuilderForHeader.AppendLine();
            stringBuilderForHeader.AppendLine(@"namespace DependentCode {");
            stringBuilderForHeader.AppendLine();
            foreach (var layerStructure in layerStructures)
            {
                foreach (var baseClassName in baseClassNameList)
                {
                    string cname = baseClassName;
                    if (baseClassName.StartsWith("baselayer::"))
                        cname = baseClassName.Substring("baselayer::".Length);
                    stringBuilderForHeader.Append(@"volatile void* GetLayerdObjectFinalizer(::");
                    stringBuilderForHeader.Append(layerStructure.LayerName);
                    stringBuilderForHeader.Append(@"::");
                    stringBuilderForHeader.Append(cname);
                    stringBuilderForHeader.AppendLine(@"* obj);");
                }
            }
            stringBuilderForHeader.AppendLine();
            int classCount = 0;
            foreach (var baseClassName in baseClassNameList)
            {
                var nss = baseClassName.Split(new string[] { "::" }, StringSplitOptions.None);
                for (int i = 0; i < nss.Length; ++i)
                {
                    stringBuilderForHeader.Append(@"namespace ");
                    stringBuilderForHeader.Append(nss[i]);
                    stringBuilderForHeader.Append(@" { ");
                }
                stringBuilderForHeader.AppendLine();
                stringBuilderForHeader.AppendLine(@"volatile void** GetVirtualFunctionTable(BaseLayer* layer);");
                foreach (var layer in layerStructures)
                {
                    stringBuilderForHeader.Append(@"volatile void** GetVirtualFunctionTable(");
                    stringBuilderForHeader.Append(layer.LayerName);
                    stringBuilderForHeader.AppendLine(@"* layer);");
                }
                stringBuilderForHeader.AppendLine();
                var baseMethodList = baseMethodLists[classCount];
                foreach (var baseMethod in baseMethodList)
                {
                    if (baseMethod != null)
                    {
                        stringBuilderForHeader.Append(@"void ExecuteProceed_");
                        stringBuilderForHeader.Append(baseMethod.Name);
                        stringBuilderForHeader.Append(@"(void* layerdObject, volatile void* proceed");
                        foreach (var param in baseMethod.Parameters)
                        {
                            stringBuilderForHeader.Append(@", ");
                            stringBuilderForHeader.Append(param);
                        }
                        stringBuilderForHeader.AppendLine(@");");
                    }
                }
                stringBuilderForHeader.AppendLine();
                for (int i = 0; i < nss.Length; ++i)
                {
                    stringBuilderForHeader.Append(@" } ");
                }
                stringBuilderForHeader.AppendLine();
                ++classCount;
            }
            stringBuilderForHeader.AppendLine();
            stringBuilderForHeader.AppendLine(@"} // namespace DependentCode {}");
            stringBuilderForHeader.AppendLine(@"} // namespace Generated {}");
            stringBuilderForHeader.AppendLine(@"} // namespace RTCOP {}");
            stringBuilderForHeader.AppendLine(@"#endif // !__RTCOP_GENERATED_DEPENDENTCODE__");
            result.CodeDictionary[@"DependentCode.h"] = stringBuilderForHeader.ToString();

            // ソースコード
            if (Target == DevelopmentTarget.WindowsX64)
            {
                if (Environment == DevelopmentEnvironment.VisualStudio)
                {
                    GenerateDependentCodeForWindowsX64VisualStudio(result, baseLayerStructure, layerStructures, baseClassNameList, baseMethodLists, includeFilePath);
                }
                else
                {
                    GenerateDependentCodeForWindowsX64MinGW(result, baseLayerStructure, layerStructures, baseClassNameList, baseMethodLists, includeFilePath);
                }
            }
            else if (Target == DevelopmentTarget.WindowsX86)
            {
                if (Environment == DevelopmentEnvironment.VisualStudio)
                {
                    GenerateDependentCodeForWindowsX86VisualStudio(result, baseLayerStructure, layerStructures, baseClassNameList, baseMethodLists, includeFilePath);
                }
                else
                {
                    GenerateDependentCodeForWindowsX86MinGW(result, baseLayerStructure, layerStructures, baseClassNameList, baseMethodLists, includeFilePath);
                }
            }
            else if (Target == DevelopmentTarget.LinuxX64)
            {
                GenerateDependentCodeForLinuxX64(result, baseLayerStructure, layerStructures, baseClassNameList, baseMethodLists, includeFilePath);
            }
            else if (Target == DevelopmentTarget.LinuxX86)
            {
                GenerateDependentCodeForLinuxX86(result, baseLayerStructure, layerStructures, baseClassNameList, baseMethodLists, includeFilePath);
            }
            else if (Target == DevelopmentTarget.LinuxARM)
            {
                GenerateDependentCodeForLinuxARM(result, baseLayerStructure, layerStructures, baseClassNameList, baseMethodLists, includeFilePath);
            }
            else if (Target == DevelopmentTarget.LinuxARM64)
            {
                GenerateDependentCodeForLinuxARM64(result, baseLayerStructure, layerStructures, baseClassNameList, baseMethodLists, includeFilePath);
            }
            else if (Target == DevelopmentTarget.macOSX64)
            {
                GenerateDependentCodeForMacOSX64(result, baseLayerStructure, layerStructures, baseClassNameList, baseMethodLists, includeFilePath);
            }
        }

        /// <summary>
        /// 環境依存コード(Windows 64bit Visual Studio)
        /// </summary>
        private void GenerateDependentCodeForWindowsX64VisualStudio(GeneratedCodes result, LayerStructure baseLayerStructure, List<LayerStructure> layerStructures, List<string> baseClassNameList, List<List<LayerdMethodDefinition>> baseMethodLists, string includeFilePath)
        {
            // cppファイル
            StringBuilder stringBuilderForSource = new StringBuilder();
            stringBuilderForSource.Append(@"#include """);
            stringBuilderForSource.Append(includeFilePath);
            stringBuilderForSource.AppendLine(@"DependentCode.h""");
            stringBuilderForSource.AppendLine();
            stringBuilderForSource.AppendLine(@"namespace RTCOP {");
            stringBuilderForSource.AppendLine(@"namespace Generated {");
            stringBuilderForSource.AppendLine(@"namespace DependentCode {");
            stringBuilderForSource.AppendLine();
            int classCount = 0;
            foreach (var baseClassName in baseClassNameList)
            {
                var nss = baseClassName.Split(new string[] { "::" }, StringSplitOptions.None);
                for (int i = 0; i < nss.Length; ++i)
                {
                    stringBuilderForSource.Append(@"namespace ");
                    stringBuilderForSource.Append(nss[i]);
                    stringBuilderForSource.Append(@" { ");
                }
                stringBuilderForSource.AppendLine();
                var baseMethodList = baseMethodLists[classCount];
                foreach (var baseMethod in baseMethodList)
                {
                    if (baseMethod != null)
                    {
                        stringBuilderForSource.Append(@"void ExecuteProceed_");
                        stringBuilderForSource.Append(baseMethod.Name);
                        stringBuilderForSource.Append(@"(void* layerdObject, volatile void* proceed");
                        foreach (var param in baseMethod.Parameters)
                        {
                            stringBuilderForSource.Append(@", ");
                            stringBuilderForSource.Append(param);
                        }
                        stringBuilderForSource.AppendLine(@")");
                        stringBuilderForSource.AppendLine(@"{");
                        stringBuilderForSource.Append("\t");
                        stringBuilderForSource.Append(@"void(*pProceed)(void*");
                        foreach (var param in baseMethod.Parameters)
                        {
                            stringBuilderForSource.Append(@", ");
                            stringBuilderForSource.Append(param.Type);
                        }
                        stringBuilderForSource.Append(@") = (void(*)(void*");
                        foreach (var param in baseMethod.Parameters)
                        {
                            stringBuilderForSource.Append(@", ");
                            stringBuilderForSource.Append(param.Type);
                        }
                        stringBuilderForSource.AppendLine(@"))proceed;");
                        stringBuilderForSource.Append("\t");
                        stringBuilderForSource.Append("pProceed(layerdObject");
                        foreach (var param in baseMethod.Parameters)
                        {
                            stringBuilderForSource.Append(@", ");
                            stringBuilderForSource.Append(param.Name);
                        }
                        stringBuilderForSource.AppendLine(");");
                        stringBuilderForSource.AppendLine(@"}");
                    }
                }
                stringBuilderForSource.AppendLine();
                for (int i = 0; i < nss.Length; ++i)
                {
                    stringBuilderForSource.Append(@" } ");
                }
                stringBuilderForSource.AppendLine();
                ++classCount;
            }
            stringBuilderForSource.AppendLine(@"} // namespace DependentCode {}");
            stringBuilderForSource.AppendLine(@"} // namespace Generated {}");
            stringBuilderForSource.AppendLine(@"} // namespace RTCOP {}");
            result.CodeDictionary[@"DependentCode.cpp"] = stringBuilderForSource.ToString();

            // アセンブリファイル
            StringBuilder stringBuilderForAsm = new StringBuilder();
            foreach (var baseClassName in baseClassNameList)
            {
                stringBuilderForAsm.Append(@"public ?GetVirtualFunctionTable");
                var nss = baseClassName.Split(new string[] { "::" }, StringSplitOptions.None);
                for (int i = (nss.Length - 1); i >= 0; --i)
                {
                    stringBuilderForAsm.Append(@"@");
                    stringBuilderForAsm.Append(nss[i]);
                }
                stringBuilderForAsm.Append(@"@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVBaseLayer@");
                stringBuilderForAsm.Append(nss.Length + 2);
                stringBuilderForAsm.Append(nss.Length + 3);
                stringBuilderForAsm.AppendLine(@"@@Z");
            }
            foreach (var layerStructure in layerStructures)
            {
                foreach (var baseClassName in baseClassNameList)
                {
                    stringBuilderForAsm.Append(@"public ?GetVirtualFunctionTable");
                    var nss = baseClassName.Split(new string[] { "::" }, StringSplitOptions.None);
                    for (int i = (nss.Length - 1); i >= 0; --i)
                    {
                        stringBuilderForAsm.Append(@"@");
                        stringBuilderForAsm.Append(nss[i]);
                    }
                    stringBuilderForAsm.Append(@"@DependentCode@Generated@RTCOP@@YAPEAPECXPEAV");
                    stringBuilderForAsm.Append(layerStructure.LayerName);
                    stringBuilderForAsm.Append(@"@");
                    stringBuilderForAsm.Append(nss.Length + 2);
                    stringBuilderForAsm.Append(nss.Length + 3);
                    stringBuilderForAsm.AppendLine(@"@@Z");
                }
            }
            foreach (var layerStructure in layerStructures)
            {
                foreach (var baseClassName in baseClassNameList)
                {
                    stringBuilderForAsm.Append(@"public ?GetLayerdObjectFinalizer@DependentCode@Generated@RTCOP@@YAPECXPEAV");
                    string cname = baseClassName;
                    if (baseClassName.StartsWith("baselayer::"))
                        cname = baseClassName.Substring("baselayer::".Length);
                    var nss = cname.Split(new string[] { "::" }, StringSplitOptions.None);
                    stringBuilderForAsm.Append(nss[nss.Length - 1]);
                    for (int i = (nss.Length - 2); i >= 0; --i)
                    {
                        stringBuilderForAsm.Append(@"@");
                        stringBuilderForAsm.Append(nss[i]);
                    }
                    stringBuilderForAsm.Append(@"@");
                    stringBuilderForAsm.Append(layerStructure.LayerName);
                    stringBuilderForAsm.AppendLine(@"@@@Z");
                }
            }
            // extern
            foreach (var baseClassName in baseClassNameList)
            {
                stringBuilderForAsm.Append(@"extrn ??_7?$LayerdObject@V");
                var nss = baseClassName.Split(new string[] { "::" }, StringSplitOptions.None);
                stringBuilderForAsm.Append(nss[nss.Length - 1]);
                for (int i = (nss.Length - 2); i >= 0; --i)
                {
                    stringBuilderForAsm.Append(@"@");
                    stringBuilderForAsm.Append(nss[i]);
                }
                stringBuilderForAsm.AppendLine(@"@@@Core@RTCOP@@6B@:PTR");
                foreach (var layerStructure in layerStructures)
                {
                    stringBuilderForAsm.Append(@"extrn ??_7");
                    string cname = baseClassName;
                    if (baseClassName.StartsWith("baselayer::"))
                        cname = baseClassName.Substring("baselayer::".Length);
                    var nss2 = cname.Split(new string[] { "::" }, StringSplitOptions.None);
                    stringBuilderForAsm.Append(nss2[nss2.Length - 1]);
                    for (int i = (nss2.Length - 2); i >= 0; --i)
                    {
                        stringBuilderForAsm.Append(@"@");
                        stringBuilderForAsm.Append(nss2[i]);
                    }
                    stringBuilderForAsm.Append(@"@");
                    stringBuilderForAsm.Append(layerStructure.LayerName);
                    stringBuilderForAsm.AppendLine(@"@@6B@:PTR");
                }
            }
            foreach (var layerStructure in layerStructures)
            {
                foreach (var baseClassName in baseClassNameList)
                {
                    stringBuilderForAsm.Append(@"extrn ?_RTCOP_FinalizePartialClass@");
                    string cname = baseClassName;
                    if (baseClassName.StartsWith("baselayer::"))
                        cname = baseClassName.Substring("baselayer::".Length);
                    var nss = cname.Split(new string[] { "::" }, StringSplitOptions.None);
                    stringBuilderForAsm.Append(nss[nss.Length - 1]);
                    for (int i = (nss.Length - 2); i >= 0; --i)
                    {
                        stringBuilderForAsm.Append(@"@");
                        stringBuilderForAsm.Append(nss[i]);
                    }
                    stringBuilderForAsm.Append(@"@");
                    stringBuilderForAsm.Append(layerStructure.LayerName);
                    stringBuilderForAsm.AppendLine(@"@@AEAAXXZ:PROC");
                }
            }
            // 中身
            stringBuilderForAsm.AppendLine();
            stringBuilderForAsm.AppendLine(@"_TEXT segment");
            stringBuilderForAsm.Append("\t");
            stringBuilderForAsm.AppendLine(@"align 16");
            foreach (var baseClassName in baseClassNameList)
            {
                stringBuilderForAsm.Append(@"?GetVirtualFunctionTable");
                var nss = baseClassName.Split(new string[] { "::" }, StringSplitOptions.None);
                for (int i = (nss.Length - 1); i >= 0; --i)
                {
                    stringBuilderForAsm.Append(@"@");
                    stringBuilderForAsm.Append(nss[i]);
                }
                stringBuilderForAsm.Append(@"@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVBaseLayer@");
                stringBuilderForAsm.Append(nss.Length + 2);
                stringBuilderForAsm.Append(nss.Length + 3);
                stringBuilderForAsm.AppendLine(@"@@Z proc");
                stringBuilderForAsm.Append("\t");
                stringBuilderForAsm.Append(@"mov rax, offset ??_7?$LayerdObject@V");
                stringBuilderForAsm.Append(nss[nss.Length - 1]);
                for (int i = (nss.Length - 2); i >= 0; --i)
                {
                    stringBuilderForAsm.Append(@"@");
                    stringBuilderForAsm.Append(nss[i]);
                }
                stringBuilderForAsm.AppendLine(@"@@@Core@RTCOP@@6B@");
                stringBuilderForAsm.Append("\t");
                stringBuilderForAsm.AppendLine(@"ret");
                stringBuilderForAsm.Append(@"?GetVirtualFunctionTable");
                for (int i = (nss.Length - 1); i >= 0; --i)
                {
                    stringBuilderForAsm.Append(@"@");
                    stringBuilderForAsm.Append(nss[i]);
                }
                stringBuilderForAsm.Append(@"@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVBaseLayer@");
                stringBuilderForAsm.Append(nss.Length + 2);
                stringBuilderForAsm.Append(nss.Length + 3);
                stringBuilderForAsm.AppendLine(@"@@Z endp");
            }
            stringBuilderForAsm.AppendLine();
            foreach (var layerStructure in layerStructures)
            {
                foreach (var baseClassName in baseClassNameList)
                {
                    stringBuilderForAsm.Append("\t");
                    stringBuilderForAsm.AppendLine(@"align 16");
                    stringBuilderForAsm.Append(@"?GetVirtualFunctionTable");
                    var nss = baseClassName.Split(new string[] { "::" }, StringSplitOptions.None);
                    for (int i = (nss.Length - 1); i >= 0; --i)
                    {
                        stringBuilderForAsm.Append(@"@");
                        stringBuilderForAsm.Append(nss[i]);
                    }
                    stringBuilderForAsm.Append(@"@DependentCode@Generated@RTCOP@@YAPEAPECXPEAV");
                    stringBuilderForAsm.Append(layerStructure.LayerName);
                    stringBuilderForAsm.Append(@"@");
                    stringBuilderForAsm.Append(nss.Length + 2);
                    stringBuilderForAsm.Append(nss.Length + 3);
                    stringBuilderForAsm.AppendLine(@"@@Z proc");
                    stringBuilderForAsm.Append("\t");
                    stringBuilderForAsm.Append(@"mov rax, offset ??_7");
                    string cname = baseClassName;
                    if (baseClassName.StartsWith("baselayer::"))
                        cname = baseClassName.Substring("baselayer::".Length);
                    var nss2 = cname.Split(new string[] { "::" }, StringSplitOptions.None);
                    stringBuilderForAsm.Append(nss2[nss2.Length - 1]);
                    for (int i = (nss2.Length - 2); i >= 0; --i)
                    {
                        stringBuilderForAsm.Append(@"@");
                        stringBuilderForAsm.Append(nss2[i]);
                    }
                    stringBuilderForAsm.Append(@"@");
                    stringBuilderForAsm.Append(layerStructure.LayerName);
                    stringBuilderForAsm.AppendLine(@"@@6B@");
                    stringBuilderForAsm.Append("\t");
                    stringBuilderForAsm.AppendLine(@"ret");
                    stringBuilderForAsm.Append(@"?GetVirtualFunctionTable");
                    for (int i = (nss.Length - 1); i >= 0; --i)
                    {
                        stringBuilderForAsm.Append(@"@");
                        stringBuilderForAsm.Append(nss[i]);
                    }
                    stringBuilderForAsm.Append(@"@DependentCode@Generated@RTCOP@@YAPEAPECXPEAV");
                    stringBuilderForAsm.Append(layerStructure.LayerName);
                    stringBuilderForAsm.Append(@"@");
                    stringBuilderForAsm.Append(nss.Length + 2);
                    stringBuilderForAsm.Append(nss.Length + 3);
                    stringBuilderForAsm.AppendLine(@"@@Z endp");
                    stringBuilderForAsm.AppendLine();
                }
            }
            foreach (var layerStructure in layerStructures)
            {
                foreach (var baseClassName in baseClassNameList)
                {
                    stringBuilderForAsm.Append("\t");
                    stringBuilderForAsm.AppendLine(@"align 16");
                    stringBuilderForAsm.Append(@"?GetLayerdObjectFinalizer@DependentCode@Generated@RTCOP@@YAPECXPEAV");
                    string cname = baseClassName;
                    if (baseClassName.StartsWith("baselayer::"))
                        cname = baseClassName.Substring("baselayer::".Length);
                    var nss = cname.Split(new string[] { "::" }, StringSplitOptions.None);
                    stringBuilderForAsm.Append(nss[nss.Length - 1]);
                    for (int i = (nss.Length - 2); i >= 0; --i)
                    {
                        stringBuilderForAsm.Append(@"@");
                        stringBuilderForAsm.Append(nss[i]);
                    }
                    stringBuilderForAsm.Append(@"@");
                    stringBuilderForAsm.Append(layerStructure.LayerName);
                    stringBuilderForAsm.AppendLine(@"@@@Z proc");
                    stringBuilderForAsm.Append("\t");
                    stringBuilderForAsm.Append(@"mov rax, offset ?_RTCOP_FinalizePartialClass@");
                    stringBuilderForAsm.Append(nss[nss.Length - 1]);
                    for (int i = (nss.Length - 2); i >= 0; --i)
                    {
                        stringBuilderForAsm.Append(@"@");
                        stringBuilderForAsm.Append(nss[i]);
                    }
                    stringBuilderForAsm.Append(@"@");
                    stringBuilderForAsm.Append(layerStructure.LayerName);
                    stringBuilderForAsm.AppendLine(@"@@AEAAXXZ");
                    stringBuilderForAsm.Append("\t");
                    stringBuilderForAsm.AppendLine(@"ret");
                    stringBuilderForAsm.Append(@"?GetLayerdObjectFinalizer@DependentCode@Generated@RTCOP@@YAPECXPEAV");
                    stringBuilderForAsm.Append(nss[nss.Length - 1]);
                    for (int i = (nss.Length - 2); i >= 0; --i)
                    {
                        stringBuilderForAsm.Append(@"@");
                        stringBuilderForAsm.Append(nss[i]);
                    }
                    stringBuilderForAsm.Append(@"@");
                    stringBuilderForAsm.Append(layerStructure.LayerName);
                    stringBuilderForAsm.AppendLine(@"@@@Z endp");
                    stringBuilderForAsm.AppendLine();
                }
            }
            stringBuilderForAsm.AppendLine(@"_TEXT ends");
            stringBuilderForAsm.Append("\t");
            stringBuilderForAsm.AppendLine(@"end");
            result.CodeDictionary[@"DependentCodeAsm.asm"] = stringBuilderForAsm.ToString();
        }

        /// <summary>
        /// 環境依存コード(Windows 32bit Visual Studio)
        /// </summary>
        private void GenerateDependentCodeForWindowsX86VisualStudio(GeneratedCodes result, LayerStructure baseLayerStructure, List<LayerStructure> layerStructures, List<string> baseClassNameList, List<List<LayerdMethodDefinition>> baseMethodLists, string includeFilePath)
        {
            // cppファイル
            StringBuilder stringBuilderForSource = new StringBuilder();
            stringBuilderForSource.Append(@"#include """);
            stringBuilderForSource.Append(includeFilePath);
            stringBuilderForSource.AppendLine(@"DependentCode.h""");
            foreach (var layerStructure in layerStructures)
            {
                stringBuilderForSource.Append(@"#include """);
                stringBuilderForSource.Append(includeFilePath);
                stringBuilderForSource.Append(layerStructure.LayerName);
                stringBuilderForSource.AppendLine(@".h""");
            }
            stringBuilderForSource.AppendLine();
            stringBuilderForSource.AppendLine(@"namespace RTCOP {");
            stringBuilderForSource.AppendLine(@"namespace Generated {");
            stringBuilderForSource.AppendLine(@"namespace DependentCode {");
            stringBuilderForSource.AppendLine();
            int classCount = 0;
            foreach (var baseClassName in baseClassNameList)
            {
                var nss = baseClassName.Split(new string[] { "::" }, StringSplitOptions.None);
                for (int i = 0; i < nss.Length; ++i)
                {
                    stringBuilderForSource.Append(@"namespace ");
                    stringBuilderForSource.Append(nss[i]);
                    stringBuilderForSource.Append(@" { ");
                }
                stringBuilderForSource.AppendLine();
                var baseMethodList = baseMethodLists[classCount];
                foreach (var baseMethod in baseMethodList)
                {
                    if (baseMethod != null)
                    {
                        stringBuilderForSource.Append(@"void ExecuteProceed_");
                        stringBuilderForSource.Append(baseMethod.Name);
                        stringBuilderForSource.Append(@"(void* layerdObject, volatile void* proceed");
                        foreach (var param in baseMethod.Parameters)
                        {
                            stringBuilderForSource.Append(@", ");
                            stringBuilderForSource.Append(param);
                        }
                        stringBuilderForSource.AppendLine(@")");
                        stringBuilderForSource.AppendLine(@"{");
                        stringBuilderForSource.Append("\t");
                        stringBuilderForSource.AppendLine(@"__asm");
                        stringBuilderForSource.Append("\t");
                        stringBuilderForSource.AppendLine(@"{");
                        foreach (var param in baseMethod.Parameters)
                        {
                            stringBuilderForSource.Append("\t\t");
                            stringBuilderForSource.Append(@"push dword ptr[");
                            stringBuilderForSource.Append(param.Name);
                            stringBuilderForSource.AppendLine(@"]");
                        }
                        stringBuilderForSource.Append("\t\t");
                        stringBuilderForSource.AppendLine(@"mov ecx, dword ptr[layerdObject]");
                        stringBuilderForSource.Append("\t\t");
                        stringBuilderForSource.AppendLine(@"mov eax, dword ptr[proceed]");
                        stringBuilderForSource.Append("\t\t");
                        stringBuilderForSource.AppendLine(@"call eax");
                        stringBuilderForSource.Append("\t");
                        stringBuilderForSource.AppendLine(@"}");
                        stringBuilderForSource.AppendLine(@"}");
                    }
                }
                stringBuilderForSource.AppendLine();
                for (int i = 0; i < nss.Length; ++i)
                {
                    stringBuilderForSource.Append(@" } ");
                }
                stringBuilderForSource.AppendLine();
                stringBuilderForSource.AppendLine();
                ++classCount;
            }
            foreach (var layerStructure in layerStructures)
            {
                foreach (var baseClassName in baseClassNameList)
                {
                    stringBuilderForSource.Append(@"volatile void* GetLayerdObjectFinalizer(::");
                    stringBuilderForSource.Append(layerStructure.LayerName);
                    stringBuilderForSource.Append(@"::");
                    string cname = baseClassName;
                    if (baseClassName.StartsWith("baselayer::"))
                        cname = baseClassName.Substring("baselayer::".Length);
                    stringBuilderForSource.Append(cname);
                    stringBuilderForSource.AppendLine(@"* obj)");
                    stringBuilderForSource.AppendLine(@"{");
                    stringBuilderForSource.Append("\t");
                    stringBuilderForSource.AppendLine(@"void* vfp = 0;");
                    stringBuilderForSource.Append("\t");
                    stringBuilderForSource.Append(@"typedef ::");
                    stringBuilderForSource.Append(layerStructure.LayerName);
                    stringBuilderForSource.Append(@"::");
                    stringBuilderForSource.Append(cname);
                    stringBuilderForSource.Append(@" C");
                    stringBuilderForSource.Append(layerStructure.LayerName);
                    stringBuilderForSource.Append(cname);
                    stringBuilderForSource.AppendLine(@";");
                    stringBuilderForSource.Append("\t");
                    stringBuilderForSource.AppendLine(@"__asm");
                    stringBuilderForSource.Append("\t");
                    stringBuilderForSource.AppendLine(@"{");
                    stringBuilderForSource.Append("\t\t");
                    stringBuilderForSource.Append(@"mov eax, C");
                    stringBuilderForSource.Append(layerStructure.LayerName);
                    stringBuilderForSource.Append(cname);
                    stringBuilderForSource.AppendLine(@"::_RTCOP_FinalizePartialClass");
                    stringBuilderForSource.Append("\t\t");
                    stringBuilderForSource.AppendLine(@"mov vfp, eax");
                    stringBuilderForSource.Append("\t");
                    stringBuilderForSource.AppendLine(@"}");
                    stringBuilderForSource.Append("\t");
                    stringBuilderForSource.AppendLine(@"return vfp;");
                    stringBuilderForSource.AppendLine(@"}");
                    stringBuilderForSource.AppendLine();
                }
            }
            stringBuilderForSource.AppendLine(@"} // namespace DependentCode {}");
            stringBuilderForSource.AppendLine(@"} // namespace Generated {}");
            stringBuilderForSource.AppendLine(@"} // namespace RTCOP {}");
            result.CodeDictionary[@"DependentCode.cpp"] = stringBuilderForSource.ToString();

            // アセンブリファイル
            StringBuilder stringBuilderForAsm = new StringBuilder();
            foreach (var baseClassName in baseClassNameList)
            {
                stringBuilderForAsm.Append(@"public	?GetVirtualFunctionTable");
                var nss = baseClassName.Split(new string[] { "::" }, StringSplitOptions.None);
                for (int i = (nss.Length - 1); i >= 0; --i)
                {
                    stringBuilderForAsm.Append(@"@");
                    stringBuilderForAsm.Append(nss[i]);
                }
                stringBuilderForAsm.Append(@"@DependentCode@Generated@RTCOP@@YAPAPCXPAVBaseLayer@");
                stringBuilderForAsm.Append(nss.Length + 2);
                stringBuilderForAsm.Append(nss.Length + 3);
                stringBuilderForAsm.AppendLine(@"@@Z");
            }
            foreach (var layerStructure in layerStructures)
            {
                foreach (var baseClassName in baseClassNameList)
                {
                    stringBuilderForAsm.Append(@"public	?GetVirtualFunctionTable");
                    var nss = baseClassName.Split(new string[] { "::" }, StringSplitOptions.None);
                    for (int i = (nss.Length - 1); i >= 0; --i)
                    {
                        stringBuilderForAsm.Append(@"@");
                        stringBuilderForAsm.Append(nss[i]);
                    }
                    stringBuilderForAsm.Append(@"@DependentCode@Generated@RTCOP@@YAPAPCXPAV");
                    stringBuilderForAsm.Append(layerStructure.LayerName);
                    stringBuilderForAsm.Append(@"@");
                    stringBuilderForAsm.Append(nss.Length + 2);
                    stringBuilderForAsm.Append(nss.Length + 3);
                    stringBuilderForAsm.AppendLine(@"@@Z");
                }
            }
            // extern
            foreach (var baseClassName in baseClassNameList)
            {
                var nss = baseClassName.Split(new string[] { "::" }, StringSplitOptions.None);
                stringBuilderForAsm.Append(@"extrn ??_7");
                stringBuilderForAsm.Append(nss[nss.Length - 1]);
                for (int i = (nss.Length - 2); i >= 0; --i)
                {
                    stringBuilderForAsm.Append(@"@");
                    stringBuilderForAsm.Append(nss[i]);
                }
                stringBuilderForAsm.AppendLine(@"@@6B@:PTR");
                stringBuilderForAsm.Append(@"extrn ??_7?$LayerdObject@V");
                stringBuilderForAsm.Append(nss[nss.Length - 1]);
                for (int i = (nss.Length - 2); i >= 0; --i)
                {
                    stringBuilderForAsm.Append(@"@");
                    stringBuilderForAsm.Append(nss[i]);
                }
                stringBuilderForAsm.AppendLine(@"@@@Core@RTCOP@@6B@:PTR");
                foreach (var layerStructure in layerStructures)
                {
                    stringBuilderForAsm.Append(@"extrn	??_7");
                    string cname = baseClassName;
                    if (baseClassName.StartsWith("baselayer::"))
                        cname = baseClassName.Substring("baselayer::".Length);
                    var nss2 = cname.Split(new string[] { "::" }, StringSplitOptions.None);
                    stringBuilderForAsm.Append(nss2[nss2.Length - 1]);
                    for (int i = (nss2.Length - 2); i >= 0; --i)
                    {
                        stringBuilderForAsm.Append(@"@");
                        stringBuilderForAsm.Append(nss2[i]);
                    }
                    stringBuilderForAsm.Append(@"@");
                    stringBuilderForAsm.Append(layerStructure.LayerName);
                    stringBuilderForAsm.AppendLine(@"@@6B@:PTR");
                }
            }
            // 中身
            stringBuilderForAsm.AppendLine();
            stringBuilderForAsm.AppendLine(@"_TEXT segment USE32");
            stringBuilderForAsm.Append("\t");
            stringBuilderForAsm.AppendLine(@"align 16");
            foreach (var baseClassName in baseClassNameList)
            {
                stringBuilderForAsm.Append(@"?GetVirtualFunctionTable");
                var nss = baseClassName.Split(new string[] { "::" }, StringSplitOptions.None);
                for (int i = (nss.Length - 1); i >= 0; --i)
                {
                    stringBuilderForAsm.Append(@"@");
                    stringBuilderForAsm.Append(nss[i]);
                }
                stringBuilderForAsm.Append(@"@DependentCode@Generated@RTCOP@@YAPAPCXPAVBaseLayer@");
                stringBuilderForAsm.Append(nss.Length + 2);
                stringBuilderForAsm.Append(nss.Length + 3);
                stringBuilderForAsm.AppendLine(@"@@Z proc");
                stringBuilderForAsm.Append("\t");
                stringBuilderForAsm.Append(@"mov eax, offset ??_7?$LayerdObject@V");
                stringBuilderForAsm.Append(nss[nss.Length - 1]);
                for (int i = (nss.Length - 2); i >= 0; --i)
                {
                    stringBuilderForAsm.Append(@"@");
                    stringBuilderForAsm.Append(nss[i]);
                }
                stringBuilderForAsm.AppendLine(@"@@@Core@RTCOP@@6B@");
                stringBuilderForAsm.Append("\t");
                stringBuilderForAsm.AppendLine(@"ret");
                stringBuilderForAsm.Append(@"?GetVirtualFunctionTable");
                for (int i = (nss.Length - 1); i >= 0; --i)
                {
                    stringBuilderForAsm.Append(@"@");
                    stringBuilderForAsm.Append(nss[i]);
                }
                stringBuilderForAsm.Append(@"@DependentCode@Generated@RTCOP@@YAPAPCXPAVBaseLayer@");
                stringBuilderForAsm.Append(nss.Length + 2);
                stringBuilderForAsm.Append(nss.Length + 3);
                stringBuilderForAsm.AppendLine(@"@@Z endp");
            }
            stringBuilderForAsm.AppendLine();
            foreach (var layerStructure in layerStructures)
            {
                foreach (var baseClassName in baseClassNameList)
                {
                    stringBuilderForAsm.Append("\t");
                    stringBuilderForAsm.AppendLine(@"align 16");
                    stringBuilderForAsm.Append(@"?GetVirtualFunctionTable");
                    var nss = baseClassName.Split(new string[] { "::" }, StringSplitOptions.None);
                    for (int i = (nss.Length - 1); i >= 0; --i)
                    {
                        stringBuilderForAsm.Append(@"@");
                        stringBuilderForAsm.Append(nss[i]);
                    }
                    stringBuilderForAsm.Append(@"@DependentCode@Generated@RTCOP@@YAPAPCXPAV");
                    stringBuilderForAsm.Append(layerStructure.LayerName);
                    stringBuilderForAsm.Append(@"@");
                    stringBuilderForAsm.Append(nss.Length + 2);
                    stringBuilderForAsm.Append(nss.Length + 3);
                    stringBuilderForAsm.AppendLine(@"@@Z proc");
                    stringBuilderForAsm.Append("\t");
                    stringBuilderForAsm.Append(@"mov eax, offset ??_7");
                    string cname = baseClassName;
                    if (baseClassName.StartsWith("baselayer::"))
                        cname = baseClassName.Substring("baselayer::".Length);
                    var nss2 = cname.Split(new string[] { "::" }, StringSplitOptions.None);
                    stringBuilderForAsm.Append(nss2[nss2.Length - 1]);
                    for (int i = (nss2.Length - 2); i >= 0; --i)
                    {
                        stringBuilderForAsm.Append(@"@");
                        stringBuilderForAsm.Append(nss2[i]);
                    }
                    stringBuilderForAsm.Append(@"@");
                    stringBuilderForAsm.Append(layerStructure.LayerName);
                    stringBuilderForAsm.AppendLine(@"@@6B@");
                    stringBuilderForAsm.Append("\t");
                    stringBuilderForAsm.AppendLine(@"ret");
                    stringBuilderForAsm.Append(@"?GetVirtualFunctionTable");
                    for (int i = (nss.Length - 1); i >= 0; --i)
                    {
                        stringBuilderForAsm.Append(@"@");
                        stringBuilderForAsm.Append(nss[i]);
                    }
                    stringBuilderForAsm.Append(@"@DependentCode@Generated@RTCOP@@YAPAPCXPAV");
                    stringBuilderForAsm.Append(layerStructure.LayerName);
                    stringBuilderForAsm.Append(@"@");
                    stringBuilderForAsm.Append(nss.Length + 2);
                    stringBuilderForAsm.Append(nss.Length + 3);
                    stringBuilderForAsm.AppendLine(@"@@Z endp");
                    stringBuilderForAsm.AppendLine();
                }
            }
            stringBuilderForAsm.AppendLine(@"_TEXT ends");
            stringBuilderForAsm.Append("\t");
            stringBuilderForAsm.AppendLine(@"end");
            result.CodeDictionary[@"DependentCodeAsm.asm"] = stringBuilderForAsm.ToString();
        }

        /// <summary>
        /// 環境依存コード(Windows 64bit minGW)
        /// </summary>
        private void GenerateDependentCodeForWindowsX64MinGW(GeneratedCodes result, LayerStructure baseLayerStructure, List<LayerStructure> layerStructures, List<string> baseClassNameList, List<List<LayerdMethodDefinition>> baseMethodLists, string includeFilePath)
        {
        }

        /// <summary>
        /// 環境依存コード(Windows 32bit minGW)
        /// </summary>
        private void GenerateDependentCodeForWindowsX86MinGW(GeneratedCodes result, LayerStructure baseLayerStructure, List<LayerStructure> layerStructures, List<string> baseClassNameList, List<List<LayerdMethodDefinition>> baseMethodLists, string includeFilePath)
        {
        }

        /// <summary>
        /// 環境依存コード(Linux 64bit)
        /// </summary>
        private void GenerateDependentCodeForLinuxX64(GeneratedCodes result, LayerStructure baseLayerStructure, List<LayerStructure> layerStructures, List<string> baseClassNameList, List<List<LayerdMethodDefinition>> baseMethodLists, string includeFilePath)
        {
        }

        /// <summary>
        /// 環境依存コード(Linux 32bit)
        /// </summary>
        private void GenerateDependentCodeForLinuxX86(GeneratedCodes result, LayerStructure baseLayerStructure, List<LayerStructure> layerStructures, List<string> baseClassNameList, List<List<LayerdMethodDefinition>> baseMethodLists, string includeFilePath)
        {
        }

        /// <summary>
        /// 環境依存コード(Linux ARM)
        /// </summary>
        private void GenerateDependentCodeForLinuxARM(GeneratedCodes result, LayerStructure baseLayerStructure, List<LayerStructure> layerStructures, List<string> baseClassNameList, List<List<LayerdMethodDefinition>> baseMethodLists, string includeFilePath)
        {
        }

        /// <summary>
        /// 環境依存コード(Linux ARM64)
        /// </summary>
        private void GenerateDependentCodeForLinuxARM64(GeneratedCodes result, LayerStructure baseLayerStructure, List<LayerStructure> layerStructures, List<string> baseClassNameList, List<List<LayerdMethodDefinition>> baseMethodLists, string includeFilePath)
        {
        }

        /// <summary>
        /// 環境依存コード(macOS)
        /// </summary>
        private void GenerateDependentCodeForMacOSX64(GeneratedCodes result, LayerStructure baseLayerStructure, List<LayerStructure> layerStructures, List<string> baseClassNameList, List<List<LayerdMethodDefinition>> baseMethodLists, string includeFilePath)
        {
        }

        #endregion

    }
}
