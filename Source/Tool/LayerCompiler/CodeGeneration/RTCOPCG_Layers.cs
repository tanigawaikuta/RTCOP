﻿using System;
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
        /// 各種レイヤ
        /// </summary>
        private void GenerateLayers(GeneratedCodes result, LayerStructure baseLayerStructure, List<LayerStructure> layerStructures, List<string> baseClassNameList, List<List<LayerdMethodDefinition>> baseMethodLists, List<List<ConstructorDefinition>> constructorLists, List<int> superClassIDs, string includeFilePath)
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
                        if (ls.LayerName == "baselayer" || ls.LayerName == layerStructure.LayerName || layerNamesH.Contains(ls.LayerName))
                        {
                            continue;
                        }
                        if (ls.ImportedLhInfomation.Exists((obj) => obj.FilePath == filePath))
                        {
                            layerNamesH.Add(ls.LayerName);
                        }
                    }
                    layerStructure.HeaderFileItems.Remove(impFile);
                }
                stringBuilderForHeader.AppendLine(@"#ifdef __RTCOP_GENERAED_CODE_FLAG__");
                stringBuilderForHeader.AppendLine(@"#include ""BaseLayer.h""");
                foreach (string lname in layerNamesH)
                {
                    stringBuilderForHeader.Append(@"#include """);
                    stringBuilderForHeader.Append(lname);
                    stringBuilderForHeader.AppendLine(@".h""");
                }
                stringBuilderForHeader.AppendLine(@"#else");
                stringBuilderForHeader.Append(@"#include """);
                stringBuilderForHeader.Append(includeFilePath);
                stringBuilderForHeader.AppendLine(@"BaseLayer.h""");
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
                stringBuilderForHeader.AppendLine(ItemsToStringForLayers(layerStructure.HeaderFileItems, layerStructure, layerID, baseClassNameList,  constructorLists, baseMethodLists, superClassIDs, 0, "", false));
                stringBuilderForHeader.AppendLine();
                stringBuilderForHeader.AppendLine(@"#endif");
                result.CodeDictionary[layerStructure.LayerName + @".h"] = stringBuilderForHeader.ToString();

                // ソースファイル
                StringBuilder stringBuilderForSource = new StringBuilder();
                stringBuilderForSource.AppendLine(@"#include ""COPNewForApp.h""");
                stringBuilderForSource.AppendLine(@"#include ""ActivationForApp.h""");
                stringBuilderForSource.AppendLine(@"#include ""Layer_Private.h""");
                stringBuilderForSource.AppendLine(@"#include ""LayerdObject_Private.h""");
                stringBuilderForSource.AppendLine(@"#include ""DependentCode.h""");
                stringBuilderForSource.AppendLine(@"#include ""RTCOP/Framework.h""");
                stringBuilderForSource.AppendLine(@"#include ""RTCOP/Core/RTCOPManager.h""");
                stringBuilderForSource.AppendLine();
                var srcIncludeFilesArray = srcIncludeFiles.ToArray();
                foreach (var incFile in srcIncludeFilesArray)
                {
                    stringBuilderForSource.AppendLine(incFile.ToString());
                    layerStructure.SourceFileItems.Remove(incFile);
                }
                stringBuilderForSource.AppendLine();
                stringBuilderForSource.AppendLine(@"#define __RTCOP_GENERAED_CODE_FLAG__");
                var srcImportFilesArray = srcImportFiles.ToArray();
                List<string> layerNamesS = new List<string>() { layerStructure.LayerName };
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
                    if (!layerNamesS.Contains("baselayer") && baseLayerStructure.ImportedLhInfomation.Exists((obj) => obj.FilePath == filePath))
                    {
                        stringBuilderForSource.AppendLine(@"#include ""BaseLayer.h""");
                        layerNamesS.Add("baselayer");
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
                    layerStructure.SourceFileItems.Remove(impFile);
                }
                stringBuilderForSource.Append(@"#include """);
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
                        stringBuilderForSource.Append("\t");
                        stringBuilderForSource.Append(@"volatile void** virtualFunctionTable");
                        stringBuilderForSource.Append(classCount);
                        stringBuilderForSource.Append("\t");
                        stringBuilderForSource.Append(@" = DependentCode::");
                        stringBuilderForSource.Append(baseClassName);
                        stringBuilderForSource.AppendLine(@"::GetVirtualFunctionTable(this);");
                        stringBuilderForSource.Append("\t");
                        stringBuilderForSource.Append(@"for (int i = 0; i < ");
                        stringBuilderForSource.Append(@"numOfBaseMethods[");
                        stringBuilderForSource.Append(classCount);
                        stringBuilderForSource.Append(@"]; ++i) _Private->_VirtualFunctionTables[");
                        stringBuilderForSource.Append(classCount);
                        stringBuilderForSource.Append(@"][i] = virtualFunctionTable");
                        stringBuilderForSource.Append(classCount);
                        stringBuilderForSource.AppendLine(@"[i];");
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
                            var partialMethod = findedPartialClass.MethodDefinitions.SingleOrDefault((obj) => obj.CompareMethod(baseMethod));
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
                bool firstIfFlag = true;
                for (int i = 0; i < baseClassNameList.Count; ++i)
                {
                    string baseClassName = baseClassNameList[i];
                    var findedPartialClass = findedPartialClasses[i];
                    if (findedPartialClass != null)
                    {
                        if (baseClassName.StartsWith("baselayer::"))
                        {
                            baseClassName = baseClassName.Substring("baselayer::".Length);
                        }
                        stringBuilderForSource.Append("\t");
                        if (firstIfFlag)
                        {
                            stringBuilderForSource.Append(@"if (classID == ");
                            firstIfFlag = false;
                        }
                        else
                        {
                            stringBuilderForSource.Append(@"else if (classID == ");
                        }
                        stringBuilderForSource.Append(i);
                        stringBuilderForSource.AppendLine(@")");
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
                        stringBuilderForSource.AppendLine(@"volatile void* vfp = DependentCode::GetLayerdObjectFinalizer(layerdObject);");
                        stringBuilderForSource.Append("\t\t");
                        stringBuilderForSource.AppendLine(@"layerdObject->_Private->_PartialClassMembers[layerID]->_Finalizer = vfp;");
                        stringBuilderForSource.Append("\t\t");
                        stringBuilderForSource.AppendLine(@"layerdObject->_RTCOP_InitializePartialClass();");
                        stringBuilderForSource.Append("\t");
                        stringBuilderForSource.AppendLine(@"}");
                    }
                }
                stringBuilderForSource.Append("\t");
                stringBuilderForSource.AppendLine(@"return obj;");
                stringBuilderForSource.AppendLine(@"}");
                stringBuilderForSource.AppendLine();
                // 未実装のイベントハンドラ
                var layerDefinitionsH = from layerDefinition in layerStructure.HeaderFileItems
                                        where layerDefinition is LayerDefinition
                                        select layerDefinition as LayerDefinition;
                var layerDefinitionsS = from layerDefinition in layerStructure.SourceFileItems
                                        where layerDefinition is LayerDefinition
                                        select layerDefinition as LayerDefinition;
                List<LayerDefinition> layerDefinitions = new List<LayerDefinition>(layerDefinitionsH);
                layerDefinitions.AddRange(layerDefinitionsS);
                var eventhandlers = from layerDefinition in layerDefinitions
                                    from eventhandler in layerDefinition.EventHandlerDefinitions
                                    select eventhandler;
                var activatings = from handler in eventhandlers
                                  where handler.EventName == "activating"
                                  select handler;
                stringBuilderForSource.Append(@"void ");
                stringBuilderForSource.Append(layerStructure.LayerName);
                stringBuilderForSource.AppendLine(@"::_RTCOP_OnActivating()");
                stringBuilderForSource.AppendLine(@"{");
                foreach (var activating in activatings)
                {
                    stringBuilderForSource.Append("\t");
                    stringBuilderForSource.Append(@"::");
                    stringBuilderForSource.Append(layerStructure.LayerName);
                    stringBuilderForSource.Append(@"::");
                    stringBuilderForSource.Append(activating.Name);
                    stringBuilderForSource.AppendLine("();");
                }
                stringBuilderForSource.AppendLine(@"}");
                stringBuilderForSource.AppendLine();
                var activateds = from handler in eventhandlers
                                 where handler.EventName == "activated"
                                 select handler;
                stringBuilderForSource.Append(@"void ");
                stringBuilderForSource.Append(layerStructure.LayerName);
                stringBuilderForSource.AppendLine(@"::_RTCOP_OnActivated()");
                stringBuilderForSource.AppendLine(@"{");
                foreach (var activated in activateds)
                {
                    stringBuilderForSource.Append("\t");
                    stringBuilderForSource.Append(@"::");
                    stringBuilderForSource.Append(layerStructure.LayerName);
                    stringBuilderForSource.Append(@"::");
                    stringBuilderForSource.Append(activated.Name);
                    stringBuilderForSource.AppendLine("();");
                }
                stringBuilderForSource.AppendLine(@"}");
                stringBuilderForSource.AppendLine();
                var deactivatings = from handler in eventhandlers
                                    where handler.EventName == "deactivating"
                                    select handler;
                stringBuilderForSource.Append(@"void ");
                stringBuilderForSource.Append(layerStructure.LayerName);
                stringBuilderForSource.AppendLine(@"::_RTCOP_OnDeactivating()");
                stringBuilderForSource.AppendLine(@"{");
                foreach (var deactivating in deactivatings)
                {
                    stringBuilderForSource.Append("\t");
                    stringBuilderForSource.Append(@"::");
                    stringBuilderForSource.Append(layerStructure.LayerName);
                    stringBuilderForSource.Append(@"::");
                    stringBuilderForSource.Append(deactivating.Name);
                    stringBuilderForSource.AppendLine("();");
                }
                stringBuilderForSource.AppendLine(@"}");
                stringBuilderForSource.AppendLine();
                var deactivateds = from handler in eventhandlers
                                    where handler.EventName == "deactivated"
                                    select handler;
                stringBuilderForSource.Append(@"void ");
                stringBuilderForSource.Append(layerStructure.LayerName);
                stringBuilderForSource.AppendLine(@"::_RTCOP_OnDeactivated()");
                stringBuilderForSource.AppendLine(@"{");
                foreach (var deactivated in deactivateds)
                {
                    stringBuilderForSource.Append("\t");
                    stringBuilderForSource.Append(@"::");
                    stringBuilderForSource.Append(layerStructure.LayerName);
                    stringBuilderForSource.Append(@"::");
                    stringBuilderForSource.Append(deactivated.Name);
                    stringBuilderForSource.AppendLine("();");
                }
                stringBuilderForSource.AppendLine(@"}");
                stringBuilderForSource.AppendLine();
                stringBuilderForSource.AppendLine(@"} // namespace Generated {}");
                stringBuilderForSource.AppendLine(@"} // namespace RTCOP {}");
                stringBuilderForSource.AppendLine();
                stringBuilderForSource.AppendLine(ItemsToStringForLayers(layerStructure.SourceFileItems, layerStructure, layerID, baseClassNameList, constructorLists, baseMethodLists, superClassIDs, 0, "", false));
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
                                           where eventHandler.EventName == "initialize"
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
                                         where eventHandler.EventName == "finalize"
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
        private string ItemsToStringForLayers(IEnumerable<object> items, LayerStructure layerStructure, int layerId, List<string> baseClassNameList, List<List<ConstructorDefinition>> constructorLists, List<List<LayerdMethodDefinition>> baseMethodLists, List<int> superClassIDs, int indent, string ns, bool inLayer)
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
                    stringBuilder.AppendLine(ItemsToStringForLayers(ld.Contents, layerStructure, layerId, baseClassNameList, constructorLists, baseMethodLists, superClassIDs, indent, ns, true));
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
                    stringBuilder.AppendLine(ItemsToStringForLayers(nd.Contents, layerStructure, layerId, baseClassNameList, constructorLists, baseMethodLists, superClassIDs, indent, newns, inLayer));
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
                        string baseName = newns;
                        if (!baseClassNameList.Contains(baseName))
                        {
                            baseName = "baselayer::" + baseName;
                            stringBuilder.Append(baseName);
                        }
                        else
                        {
                            stringBuilder.Append(@"::");
                            stringBuilder.Append(baseName);
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
                        int classid = baseClassNameList.IndexOf(newns);
                        if (classid == -1)
                        {
                            // 再検索
                            classid = baseClassNameList.IndexOf("baselayer::" + newns);
                        }
                        int superClassId = superClassIDs[classid];
                        stringBuilder.Append(@"class PartialClassMembers : public ");
                        if (superClassId != -1)
                        {
                            stringBuilder.Append(@"::");
                            stringBuilder.Append(layerStructure.LayerName);
                            stringBuilder.Append(@"::");
                            string superClassName = baseClassNameList[superClassId];
                            if (superClassName.StartsWith(@"baselayer::"))
                                superClassName = superClassName.Substring(@"baselayer::".Length);
                            stringBuilder.Append(superClassName);
                            stringBuilder.AppendLine(@"::PartialClassMembers");
                        }
                        else
                        {
                            stringBuilder.AppendLine(@"RTCOP::Core::PartialClassMembers");
                        }
                        addIndent(indent + 1);
                        stringBuilder.AppendLine(@"{");
                        addIndent(indent + 1);
                        stringBuilder.AppendLine(@"public:");
                        var members = lcd.LayerMemberDeclaration.ToArray();
                        foreach (var member in members)
                        {
                            if (member.StorageClass != "static")
                            {
                                addIndent(indent + 2);
                                stringBuilder.Append(member.ToString());
                                stringBuilder.AppendLine(@";");
                                lcd.Contents.Remove(member);
                            }
                        }
                        addIndent(indent + 1);
                        stringBuilder.AppendLine(@"};");
                        if (classid != -1)
                        {
                            var constructorList = constructorLists[classid];
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
                            ConstructorDefinition constructor2 = null;
                            if (constructor != null)
                            {
                                constructor2 = new ConstructorDefinition(constructor.Name, constructor.Parameters, ";", constructor.Modifiers, constructor.IsNoexcept);
                            }
                            else
                            {
                                constructor2 = new ConstructorDefinition(lcd.Name, new VariableDeclaration[] { }, ";", new string[] { }, false);
                            }
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
                        stringBuilder.AppendLine(ItemsToStringForLayers(lcd.Contents, layerStructure, layerId, baseClassNameList, constructorLists, baseMethodLists, superClassIDs, indent + 1, newns, inLayer));
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
                        stringBuilder.AppendLine(ItemsToStringForLayers(lcd.Contents, layerStructure, layerId, baseClassNameList, constructorLists, baseMethodLists, superClassIDs, indent + 1, newns, inLayer));
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
                        stringBuilder.AppendLine(ItemsToStringForLayers(((IgnoreObjectBlock)lmd.Contents).Contents, layerStructure, layerId, baseClassNameList, constructorLists, baseMethodLists, superClassIDs, indent, ns + "::" + lmd.Name, inLayer));
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
                    if (inLayer)
                    {
                        addIndent(indent + 1);
                        stringBuilder.Append(mi.ClassName);
                        stringBuilder.Append(@"::PartialClassMembers* layer_members = (");
                        stringBuilder.Append(mi.ClassName);
                        stringBuilder.Append(@"::PartialClassMembers*)_Private->_PartialClassMembers[");
                        stringBuilder.Append(layerId);
                        stringBuilder.AppendLine(@"];");
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
                        // proceed
                        int methodId = baseMethodLists[classId].FindIndex((obj) => obj != null && obj.CompareMethod(mi.ToLayerdMethodDefinition()));
                        int methodOffset = 0;
                        if (methodId != -1)
                        {
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
                            stringBuilder.Append(@"volatile void* _RTCOP_proceedaddr = RTCOP::Framework::Instance->GetRTCOPManager()->GetLayer(");
                            stringBuilder.Append(layerId);
                            stringBuilder.Append(@")->GetVirtualFunctionTableForProceeding(");
                            stringBuilder.Append(classId);
                            stringBuilder.Append(@")[");
                            stringBuilder.Append(methodId + methodOffset);
                            stringBuilder.AppendLine(@"];");
                            addIndent(indent + 1);
                            stringBuilder.Append(@"auto proceed = [this, _RTCOP_proceedaddr](");
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
                            stringBuilder.Append(@") { ");
                            if (!((mi.ReturnType.Type.ToString() == "void") && (mi.ReturnType.Pointers.Count == 0)))
                            {
                                stringBuilder.Append(@"return ");
                            }
                            stringBuilder.Append(@"RTCOP::Generated::DependentCode::");
                            stringBuilder.Append(baseClassNameList[classId]);
                            stringBuilder.Append(@"::ExecuteProceed_");
                            stringBuilder.Append(mi.MethodName);
                            stringBuilder.Append(@"(this, _RTCOP_proceedaddr");
                            foreach (var param in mi.Parameters)
                            {
                                stringBuilder.Append(@", ");
                                stringBuilder.Append(param.Name);
                            }
                            stringBuilder.AppendLine(@"); };");
                            // スーパーコール
                            int superId = superClassIDs[classId];
                            if (superId != -1)
                            {
                                addIndent(indent + 1);
                                stringBuilder.Append(@"volatile void** _RTCOP_vft = RTCOP::Framework::Instance->GetRTCOPManager()->GetVirtualFunctionTable(");
                                stringBuilder.Append(superId);
                                stringBuilder.AppendLine(@");");
                                addIndent(indent + 1);
                                stringBuilder.Append(@"auto supercall = [this, _RTCOP_vft](");
                                n = mi.Parameters.Count;
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
                        stringBuilder.AppendLine(ItemsToStringForLayers(contents, layerStructure, layerId, baseClassNameList, constructorLists, baseMethodLists, superClassIDs, indent + 1, ns + "::" + mi.FullName, inLayer));
                        addIndent(indent);
                        stringBuilder.AppendLine(@"}");
                        stringBuilder.AppendLine();
                    }
                    else
                    {
                        List<object> contents = mi.Contents.Contents.ToList();
                        contents.Remove(contents.Last());
                        contents.Remove(contents.First());
                        stringBuilder.AppendLine(ItemsToStringForLayers(contents, layerStructure, layerId, baseClassNameList, constructorLists, baseMethodLists, superClassIDs, indent + 1, ns + "::" + mi.FullName, inLayer));
                        addIndent(indent);
                        stringBuilder.AppendLine(@"}");
                        stringBuilder.AppendLine();
                        indentFlag = true;
                    }
                    indentFlag = true;
                }
                else if (item is VariableDeclaration)
                {
                    addIndent(indent);
                    stringBuilder.Append(item.ToString());
                    stringBuilder.AppendLine(@";");
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
                        stringBuilder.AppendLine(ItemsToStringForLayers(contents, layerStructure, layerId, baseClassNameList, constructorLists, baseMethodLists, superClassIDs, indent + 1, ns, inLayer));
                        addIndent(indent);
                        stringBuilder.AppendLine(@"}");
                    }
                    else
                    {
                        stringBuilder.AppendLine(ItemsToStringForLayers(iob.Contents, layerStructure, layerId, baseClassNameList, constructorLists, baseMethodLists, superClassIDs, indent, ns, inLayer));
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
                else if (item is Keyword)
                {
                    if (indentFlag) addIndent(indent);
                    stringBuilder.Append(item.ToString());
                    stringBuilder.Append(" ");
                    indentFlag = false;
                }
                else if (item is PreprocessDirective)
                {
                    if (indentFlag) addIndent(indent);
                    stringBuilder.AppendLine(item.ToString());
                    indentFlag = true;
                }
                else
                {
                    if (indentFlag) addIndent(indent);
                    stringBuilder.AppendLine(item.ToString());
                    indentFlag = true;
                }
            }
            return stringBuilder.ToString();
        }

    }
}
