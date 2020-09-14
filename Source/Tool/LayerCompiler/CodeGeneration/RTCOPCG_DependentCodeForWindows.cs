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
                classCount = 0;
                foreach (var baseClassName in baseClassNameList)
                {
                    if (layerStructure._FindedPartialClasses[classCount] != null)
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
                    ++classCount;
                }
            }
            foreach (var layerStructure in layerStructures)
            {
                classCount = 0;
                foreach (var baseClassName in baseClassNameList)
                {
                    if (layerStructure._FindedPartialClasses[classCount] != null)
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
                    ++classCount;
                }
            }
            // extern
            classCount = 0;
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
                    if (layerStructure._FindedPartialClasses[classCount] != null)
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
                ++classCount;
            }
            foreach (var layerStructure in layerStructures)
            {
                classCount = 0;
                foreach (var baseClassName in baseClassNameList)
                {
                    if (layerStructure._FindedPartialClasses[classCount] != null)
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
                    ++classCount;
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
                classCount = 0;
                foreach (var baseClassName in baseClassNameList)
                {
                    if (layerStructure._FindedPartialClasses[classCount] != null)
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
                    ++classCount;
                }
            }
            foreach (var layerStructure in layerStructures)
            {
                classCount = 0;
                foreach (var baseClassName in baseClassNameList)
                {
                    if (layerStructure._FindedPartialClasses[classCount] != null)
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
                    ++classCount;
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
                classCount = 0;
                foreach (var baseClassName in baseClassNameList)
                {
                    if (layerStructure._FindedPartialClasses[classCount] != null)
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
                    ++classCount;
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
                classCount = 0;
                foreach (var baseClassName in baseClassNameList)
                {
                    if (layerStructure._FindedPartialClasses[classCount] != null)
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
                    ++classCount;
                }
            }
            // extern
            classCount = 0;
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
                    if (layerStructure._FindedPartialClasses[classCount] != null)
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
                ++classCount;
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
                classCount = 0;
                foreach (var baseClassName in baseClassNameList)
                {
                    if (layerStructure._FindedPartialClasses[classCount] != null)
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
                    ++classCount;
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

    }
}
