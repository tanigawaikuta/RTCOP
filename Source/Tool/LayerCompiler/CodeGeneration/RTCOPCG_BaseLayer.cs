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
        /// <summary>
        /// ベースレイヤ
        /// </summary>
        private void GenerateBaseLayer(GeneratedCodes result, LayerStructure baseLayerStructure, List<LayerStructure> layerStructures, List<string> baseClassNameList, List<List<LayerdMethodDefinition>> baseMethodLists, List<int> superClassIDs, string includeFilePath)
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
                string filePath = "";
                foreach (var incpath in IncludePaths)
                {
                    string path = incpath + impFile.Param1;
                    string fullPath = Path.GetFullPath(path);
                    if (File.Exists(fullPath))
                    {
                        filePath = fullPath;
                        break;
                    }
                }
                foreach (var ls in layerStructures)
                {
                    if (ls.LayerName == "baselayer" || layerNamesH.Contains(ls.LayerName))
                    {
                        continue;
                    }
                    if (ls.ImportedLhInfomation.Exists((obj) => obj.FilePath == filePath))
                    {
                        layerNamesH.Add(ls.LayerName);
                    }
                }
                baseLayerStructure.HeaderFileItems.Remove(impFile);
            }
            stringBuilderForHeader.AppendLine(@"#ifdef __RTCOP_GENERAED_CODE_FLAG__");
            foreach (string lname in layerNamesH)
            {
                stringBuilderForHeader.Append(@"#include """);
                stringBuilderForHeader.Append(lname);
                stringBuilderForHeader.AppendLine(@".h""");
            }
            stringBuilderForHeader.AppendLine(@"#else");
            foreach (string lname in layerNamesH)
            {
                stringBuilderForHeader.Append(@"#include """);
                stringBuilderForHeader.Append(includeFilePath);
                stringBuilderForHeader.Append(lname);
                stringBuilderForHeader.AppendLine(@".h""");
            }
            stringBuilderForHeader.AppendLine(@"#endif");
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
            stringBuilderForHeader.AppendLine(ItemsToStringForBaseLayer(baseLayerStructure.HeaderFileItems, baseClassNameList, baseMethodLists, superClassIDs, 0, ""));
            stringBuilderForHeader.AppendLine();
            stringBuilderForHeader.AppendLine(@"#endif //!__RTCOP_GENERATED_JAPANESELAYER__");
            result.CodeDictionary[@"BaseLayer.h"] = stringBuilderForHeader.ToString();

            // ソースファイル
            StringBuilder stringBuilderForSource = new StringBuilder();
            stringBuilderForSource.AppendLine(@"#include ""COPNewForApp.h""");
            stringBuilderForSource.AppendLine(@"#include ""ActivationForApp.h""");
            stringBuilderForSource.AppendLine(@"#include ""Layer_Private.h""");
            stringBuilderForSource.AppendLine(@"#include ""LayerdObject_Private.h""");
            stringBuilderForSource.AppendLine(@"#include ""DependentCode.h""");
            stringBuilderForSource.AppendLine(@"#include ""RTCOP/Framework.h""");
            stringBuilderForSource.AppendLine(@"#include ""RTCOP/Core/RTCOPManager.h""");
            stringBuilderForSource.AppendLine(@"#include ""RTCOP/Core/PartialClassMembers.h""");
            stringBuilderForSource.AppendLine();
            var srcIncludeFilesArray = srcIncludeFiles.ToArray();
            foreach (var incFile in srcIncludeFilesArray)
            {
                stringBuilderForSource.AppendLine(incFile.ToString());
                baseLayerStructure.SourceFileItems.Remove(incFile);
            }
            stringBuilderForSource.AppendLine();
            stringBuilderForSource.AppendLine(@"#define __RTCOP_GENERAED_CODE_FLAG__");
            stringBuilderForSource.AppendLine(@"#include ""BaseLayer.h""");
            var srcImportFilesArray = srcImportFiles.ToArray();
            List<string> layerNamesS = new List<string>();
            foreach (var impFile in srcImportFilesArray)
            {
                string filePath = "";
                foreach (var incpath in IncludePaths)
                {
                    string path = incpath + impFile.Param1;
                    string fullPath = Path.GetFullPath(path);
                    if (File.Exists(fullPath))
                    {
                        filePath = fullPath;
                        break;
                    }
                }
                foreach (var ls in layerStructures)
                {
                    if (layerNamesS.Contains(ls.LayerName))
                    {
                        continue;
                    }
                    if (ls.ImportedLhInfomation.Exists((obj) => obj.FilePath == filePath))
                    {
                        stringBuilderForSource.Append(@"#include """);
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
                stringBuilderForSource.Append(@"volatile void** virtualFunctionTable");
                stringBuilderForSource.Append(classcount);
                stringBuilderForSource.Append(@" = DependentCode::");
                stringBuilderForSource.Append(classname);
                stringBuilderForSource.AppendLine(@"::GetVirtualFunctionTable(this);");
                stringBuilderForSource.Append("\t");
                stringBuilderForSource.Append(@"for (int i = 0; i < ");
                stringBuilderForSource.Append(@"numOfBaseMethods[");
                stringBuilderForSource.Append(classcount);
                stringBuilderForSource.Append(@"]; ++i) _Private->_VirtualFunctionTables[");
                stringBuilderForSource.Append(classcount);
                stringBuilderForSource.Append(@"][i] = virtualFunctionTable");
                stringBuilderForSource.Append(classcount);
                stringBuilderForSource.AppendLine(@"[i];");
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
            stringBuilderForSource.AppendLine(ItemsToStringForBaseLayer(baseLayerStructure.SourceFileItems, baseClassNameList, baseMethodLists, superClassIDs, 0, ""));
            stringBuilderForSource.AppendLine();
            result.CodeDictionary[@"BaseLayer.cpp"] = stringBuilderForSource.ToString();
        }


        /// <summary>
        /// 要素の集合を文字列に変換
        /// </summary>
        private string ItemsToStringForBaseLayer(IEnumerable<object> items, List<string> baseClassNameList, List<List<LayerdMethodDefinition>> baseMethodLists, List<int> superClassIDs, int indent, string ns)
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
                    stringBuilder.AppendLine(ItemsToStringForBaseLayer(ld.Contents, baseClassNameList, baseMethodLists, superClassIDs, indent, "baselayer"));
                    addIndent(indent);
                    stringBuilder.AppendLine(@"}");
                    indentFlag = true;
                }
                else if (item is NamespaceDefinition)
                {
                    var nd = (NamespaceDefinition)item;
                    string newns = "";
                    if (ns == "")
                    {
                        newns = nd.Name;
                    }
                    else
                    {
                        newns = ns + "::" + nd.Name;
                    }
                    addIndent(indent);
                    if (nd.IsInline) stringBuilder.Append(@"inline ");
                    stringBuilder.Append(@"namespace ");
                    stringBuilder.AppendLine(nd.Name);
                    addIndent(indent);
                    stringBuilder.AppendLine(@"{");
                    stringBuilder.AppendLine(ItemsToStringForBaseLayer(nd.Contents, baseClassNameList, baseMethodLists, superClassIDs, indent, newns));
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
                    stringBuilder.AppendLine(ItemsToStringForBaseLayer(lcd.Contents, baseClassNameList, baseMethodLists, superClassIDs, indent + 1, newns));
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
                        stringBuilder.AppendLine(ItemsToStringForBaseLayer(((IgnoreObjectBlock)lmd.Contents).Contents, baseClassNameList, baseMethodLists, superClassIDs, indent, ns));
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
                    addIndent(indent);
                    stringBuilder.AppendLine(@"{");
                    // supercall
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
                    }
                    int superId = superClassIDs[classId];
                    if (superId != -1)
                    {
                        int methodId = baseMethodLists[classId].FindIndex((obj) => obj != null && obj.CompareMethod(mi.ToLayerdMethodDefinition()));
                        if (methodId != -1)
                        {
                            int methodOffset = 0;
                            if (Environment != DevelopmentEnvironment.VisualStudio)
                            {
                                for (int i = methodId - 1; i >= 0; --i)
                                {
                                    if (baseMethodLists[classId][i] == null)
                                    {
                                        methodOffset = 1;
                                        break;
                                    }
                                }
                            }
                            addIndent(indent + 1);
                            stringBuilder.Append(@"volatile void** _RTCOP_vft = RTCOP::Framework::Instance->GetRTCOPManager()->GetVirtualFunctionTable(");
                            stringBuilder.Append(superId);
                            stringBuilder.AppendLine(@");");
                            addIndent(indent + 1);
                            stringBuilder.Append(@"auto supercall = [this, _RTCOP_vft](");
                            int n = mi.Parameters.Count;
                            if (n > 0)
                            {
                                stringBuilder.Append(mi.Parameters[0].Type);
                                stringBuilder.Append(" ");
                                stringBuilder.Append(mi.Parameters[0].Name);
                                for (int i = 1; i < n; ++i)
                                {
                                    stringBuilder.Append(", ");
                                    stringBuilder.Append(mi.Parameters[i].Type);
                                    stringBuilder.Append(" ");
                                    stringBuilder.Append(mi.Parameters[i].Name);
                                }
                            }
                            stringBuilder.Append(@") { RTCOP::Generated::DependentCode::");
                            stringBuilder.Append(baseClassNameList[classId]);
                            stringBuilder.Append(@"::ExecuteProceed_");
                            stringBuilder.Append(mi.MethodName);
                            stringBuilder.Append(@"(this, _RTCOP_vft[");
                            stringBuilder.Append(methodId + methodOffset);
                            stringBuilder.Append(@"]");
                            foreach (var param in mi.Parameters)
                            {
                                stringBuilder.Append(@", ");
                                stringBuilder.Append(param.Name);
                            }
                            stringBuilder.AppendLine(@"); };");
                        }
                    }
                    // 中身
                    List<object> contents = mi.Contents.Contents.ToList();
                    contents.Remove(contents.Last());
                    contents.Remove(contents.First());
                    stringBuilder.AppendLine(ItemsToStringForBaseLayer(contents, baseClassNameList, baseMethodLists, superClassIDs, indent + 1, ns));
                    addIndent(indent);
                    stringBuilder.AppendLine(@"}");
                    stringBuilder.AppendLine();
                    indentFlag = true;
                }
                else if (item is VariableDeclaration)
                {
                    addIndent(indent);
                    stringBuilder.Append(item.ToString());
                    stringBuilder.AppendLine(";");
                    indentFlag = true;
                }
                else if (item is IgnoreObjectBlock)
                {
                    var iob = (IgnoreObjectBlock)item;
                    List<object> contents = iob.Contents.ToList();
                    if (contents.First().ToString() == "{")
                    {
                        stringBuilder.AppendLine();
                        contents.Remove(contents.Last());
                        contents.Remove(contents.First());
                        addIndent(indent);
                        stringBuilder.AppendLine(@"{");
                        stringBuilder.AppendLine(ItemsToStringForBaseLayer(contents, baseClassNameList, baseMethodLists, superClassIDs, indent, ns));
                        addIndent(indent);
                        stringBuilder.AppendLine(@"}");
                    }
                    else
                    {
                        stringBuilder.AppendLine(ItemsToStringForBaseLayer(iob.Contents, baseClassNameList, baseMethodLists, superClassIDs, indent, ns));
                    }
                    stringBuilder.AppendLine();
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

    }
}
