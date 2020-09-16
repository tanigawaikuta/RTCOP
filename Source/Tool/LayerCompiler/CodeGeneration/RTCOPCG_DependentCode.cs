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
        /// 環境依存コード
        /// </summary>
        private void GenerateDependentCode(GeneratedCodes result, LayerStructure baseLayerStructure, List<LayerStructure> layerStructures, List<string> baseClassNameList, List<List<LayerdMethodDefinition>> baseMethodLists, string includeFilePath)
        {
            // ヘッダファイル
            StringBuilder stringBuilderForHeader = new StringBuilder();
            stringBuilderForHeader.AppendLine(@"#ifndef __RTCOP_GENERATED_DEPENDENTCODE__");
            stringBuilderForHeader.AppendLine(@"#define __RTCOP_GENERATED_DEPENDENTCODE__");
            stringBuilderForHeader.AppendLine();
            int classCount = 0;
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
                    if (layerStructure._FindedPartialClasses[classCount] != null)
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
                ++classCount;
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
                classCount = 0;
                foreach (var baseClassName in baseClassNameList)
                {
                    if (layerStructure._FindedPartialClasses[classCount] != null)
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
                    ++classCount;
                }
            }
            stringBuilderForHeader.AppendLine();
            classCount = 0;
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
                    if (layer._FindedPartialClasses[classCount] != null)
                    {
                        stringBuilderForHeader.Append(@"volatile void** GetVirtualFunctionTable(");
                        stringBuilderForHeader.Append(layer.LayerName);
                        stringBuilderForHeader.AppendLine(@"* layer);");
                    }
                }
                stringBuilderForHeader.AppendLine();
                var baseMethodList = baseMethodLists[classCount];
                foreach (var baseMethod in baseMethodList)
                {
                    if (baseMethod != null)
                    {
                        stringBuilderForHeader.Append(baseMethod.ReturnType);
                        stringBuilderForHeader.Append(@" ExecuteProceed_");
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
                if (Environment == DevelopmentEnvironment.GCC)
                {
                    GenerateDependentCodeForLinuxX64(result, baseLayerStructure, layerStructures, baseClassNameList, baseMethodLists, includeFilePath);
                }
                else
                {
                    throw new Exception("ターゲットと開発環境の組み合わせが不正です");
                }
            }
            else if (Target == DevelopmentTarget.LinuxX86)
            {
                if (Environment == DevelopmentEnvironment.GCC)
                {
                    GenerateDependentCodeForLinuxX86(result, baseLayerStructure, layerStructures, baseClassNameList, baseMethodLists, includeFilePath);
                }
                else
                {
                    throw new Exception("ターゲットと開発環境の組み合わせが不正です");
                }
            }
            else if (Target == DevelopmentTarget.LinuxARM)
            {
                if (Environment == DevelopmentEnvironment.GCC)
                {
                    GenerateDependentCodeForLinuxARM(result, baseLayerStructure, layerStructures, baseClassNameList, baseMethodLists, includeFilePath);
                }
                else
                {
                    throw new Exception("ターゲットと開発環境の組み合わせが不正です");
                }
            }
            else if (Target == DevelopmentTarget.LinuxARM64)
            {
                if (Environment == DevelopmentEnvironment.GCC)
                {
                    GenerateDependentCodeForLinuxARM64(result, baseLayerStructure, layerStructures, baseClassNameList, baseMethodLists, includeFilePath);
                }
                else
                {
                    throw new Exception("ターゲットと開発環境の組み合わせが不正です");
                }
            }
            else if (Target == DevelopmentTarget.macOSX64)
            {
                if (Environment == DevelopmentEnvironment.GCC || Environment == DevelopmentEnvironment.Clang)
                {
                    GenerateDependentCodeForMacOSX64(result, baseLayerStructure, layerStructures, baseClassNameList, baseMethodLists, includeFilePath);
                }
                else
                {
                    throw new Exception("ターゲットと開発環境の組み合わせが不正です");
                }
            }
        }

    }
}
