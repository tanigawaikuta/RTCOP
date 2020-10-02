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
        /// 環境依存コード(Windows 64bit Visual Studio)
        /// </summary>
        private void GenerateDependentCodeForWindowsX64VisualStudio(GeneratedCodes result, LayerStructure baseLayerStructure, List<LayerStructure> layerStructures, List<string> baseClassNameList, List<List<LayerdMethodDefinition>> baseMethodLists)
        {
            // cppファイル
            StringBuilder stringBuilderForSource = new StringBuilder();
            stringBuilderForSource.AppendLine(@"#include ""DependentCode.h""");
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
                        stringBuilderForSource.Append(baseMethod.ReturnType);
                        stringBuilderForSource.Append(@" ExecuteProceed_");
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
                        stringBuilderForSource.Append(baseMethod.ReturnType);
                        stringBuilderForSource.Append(@"(*pProceed)(void*");
                        foreach (var param in baseMethod.Parameters)
                        {
                            stringBuilderForSource.Append(@", ");
                            stringBuilderForSource.Append(param.Type);
                        }
                        stringBuilderForSource.Append(@") = (");
                        stringBuilderForSource.Append(baseMethod.ReturnType);
                        stringBuilderForSource.Append(@"(*)(void*");
                        foreach (var param in baseMethod.Parameters)
                        {
                            stringBuilderForSource.Append(@", ");
                            stringBuilderForSource.Append(param.Type);
                        }
                        stringBuilderForSource.AppendLine(@"))proceed;");
                        stringBuilderForSource.Append("\t");
                        if (!((baseMethod.ReturnType.Type.ToString() == "void") && (baseMethod.ReturnType.Pointers.Count == 0)))
                        {
                            stringBuilderForSource.Append(@"return ");
                        }
                        stringBuilderForSource.Append(@"pProceed(layerdObject");
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
                    stringBuilderForSource.Append(@"}");
                }
                stringBuilderForSource.AppendLine();
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
        private void GenerateDependentCodeForWindowsX86VisualStudio(GeneratedCodes result, LayerStructure baseLayerStructure, List<LayerStructure> layerStructures, List<string> baseClassNameList, List<List<LayerdMethodDefinition>> baseMethodLists)
        {
            // cppファイル
            StringBuilder stringBuilderForSource = new StringBuilder();
            stringBuilderForSource.AppendLine(@"#include ""DependentCode.h""");
            foreach (var layerStructure in layerStructures)
            {
                stringBuilderForSource.Append(@"#include """);
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
                        stringBuilderForSource.Append(baseMethod.ReturnType);
                        stringBuilderForSource.Append(@" ExecuteProceed_");
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
                            if ((param.ArraySizes.Count == 0) && ((param.Type.ByteSize == 8) || (param.Type.ByteSize == -2)))
                            {
                                stringBuilderForSource.Append("\t\t");
                                stringBuilderForSource.Append(@"push dword ptr[");
                                stringBuilderForSource.Append(param.Name);
                                stringBuilderForSource.AppendLine(@"+4]");
                            }
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
                    stringBuilderForSource.Append(@"}");
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
        private void GenerateDependentCodeForWindowsX64MinGW(GeneratedCodes result, LayerStructure baseLayerStructure, List<LayerStructure> layerStructures, List<string> baseClassNameList, List<List<LayerdMethodDefinition>> baseMethodLists)
        {
            // cppファイル
            StringBuilder stringBuilderForSource = new StringBuilder();
            stringBuilderForSource.AppendLine(@"#include ""DependentCode.h""");
            stringBuilderForSource.AppendLine(@"#include ""RTCOP/Core/LayerdObject.h""");
            stringBuilderForSource.AppendLine();
            stringBuilderForSource.AppendLine(@"#include ""BaseLayer.h""");
            foreach (var layerStructure in layerStructures)
            {
                stringBuilderForSource.Append(@"#include """);
                stringBuilderForSource.Append(layerStructure.LayerName);
                stringBuilderForSource.AppendLine(@".h""");
            }
            stringBuilderForSource.AppendLine();
            foreach (LayerStructure layerStructure in layerStructures)
            {
                var headerIncludeFiles = from item in layerStructure.HeaderFileItems
                                         where (item is PreprocessDirective) && (((PreprocessDirective)item).Kind == DirectiveKind.Include)
                                         select (PreprocessDirective)item;
                var srcIncludeFiles = from item in layerStructure.SourceFileItems
                                      where (item is PreprocessDirective) && (((PreprocessDirective)item).Kind == DirectiveKind.Include)
                                      select (PreprocessDirective)item;
                foreach (var inc in headerIncludeFiles)
                {
                    stringBuilderForSource.Append(@"#include """);
                    stringBuilderForSource.Append(inc.Param1);
                    stringBuilderForSource.AppendLine(@"""");
                }
                foreach (var inc in srcIncludeFiles)
                {
                    stringBuilderForSource.Append(@"#include """);
                    stringBuilderForSource.Append(inc.Param1);
                    stringBuilderForSource.AppendLine(@"""");
                }
            }
            stringBuilderForSource.AppendLine();
            int classCount = 0;
            foreach (var baseClassName in baseClassNameList)
            {
                stringBuilderForSource.Append(@"asm("".rtcop_vft");
                stringBuilderForSource.Append(classCount);
                stringBuilderForSource.Append(@"0: .quad _ZTVN5RTCOP4Core12LayerdObjectI");
                string end = "EE";
                string[] nss = baseClassName.Split(new string[] { "::" }, StringSplitOptions.None);
                if (nss.Length >= 2)
                {
                    stringBuilderForSource.Append(@"N");
                    end = "EEE";
                }
                foreach (string ns in nss)
                {
                    stringBuilderForSource.Append(ns.Length);
                    stringBuilderForSource.Append(ns);
                }
                stringBuilderForSource.Append(end);
                stringBuilderForSource.AppendLine(@"+16"");");
                int layerCount = 1;
                foreach (var layerStructure in layerStructures)
                {
                    if (layerStructure._FindedPartialClasses[classCount] != null)
                    {
                        stringBuilderForSource.Append(@"asm("".rtcop_vft");
                        stringBuilderForSource.Append(classCount);
                        stringBuilderForSource.Append(layerCount);
                        stringBuilderForSource.Append(@": .quad _ZTVN");
                        stringBuilderForSource.Append(layerStructure.LayerName.Length);
                        stringBuilderForSource.Append(layerStructure.LayerName);
                        foreach (string ns in nss)
                        {
                            if (ns != "baselayer")
                            {
                                stringBuilderForSource.Append(ns.Length);
                                stringBuilderForSource.Append(ns);
                            }
                        }
                        stringBuilderForSource.AppendLine(@"E+16"");");
                        stringBuilderForSource.Append(@"asm("".rtcop_vfaddr");
                        stringBuilderForSource.Append(classCount);
                        stringBuilderForSource.Append(layerCount);
                        stringBuilderForSource.Append(@": .quad _ZN");
                        stringBuilderForSource.Append(layerStructure.LayerName.Length);
                        stringBuilderForSource.Append(layerStructure.LayerName);
                        foreach (string ns in nss)
                        {
                            if (ns != "baselayer")
                            {
                                stringBuilderForSource.Append(ns.Length);
                                stringBuilderForSource.Append(ns);
                            }
                        }
                        stringBuilderForSource.AppendLine(@"27_RTCOP_FinalizePartialClassEv"");");
                    }
                    ++layerCount;
                }
                stringBuilderForSource.AppendLine();
                ++classCount;
            }
            stringBuilderForSource.AppendLine();
            stringBuilderForSource.AppendLine(@"namespace RTCOP {");
            stringBuilderForSource.AppendLine(@"namespace Generated {");
            stringBuilderForSource.AppendLine(@"namespace DependentCode {");
            stringBuilderForSource.AppendLine();
            classCount = 0;
            foreach (var baseClassName in baseClassNameList)
            {
                var nss = baseClassName.Split(new string[] { "::" }, StringSplitOptions.None);
                int layerCount = 1;
                foreach (var layerStructure in layerStructures)
                {
                    if (layerStructure._FindedPartialClasses[classCount] != null)
                    {
                        stringBuilderForSource.Append(@"volatile void* GetLayerdObjectFinalizer(::");
                        stringBuilderForSource.Append(layerStructure.LayerName);
                        stringBuilderForSource.Append(@"::");
                        for (int i = 0; i < (nss.Length - 1); ++i)
                        {
                            string ns = nss[i];
                            if (ns != "baselayer")
                            {
                                stringBuilderForSource.Append(ns);
                                stringBuilderForSource.Append(@"::");
                            }
                        }
                        stringBuilderForSource.Append(nss[nss.Length - 1]);
                        stringBuilderForSource.AppendLine(@"* obj)");
                        stringBuilderForSource.AppendLine(@"{");
                        stringBuilderForSource.Append("\t");
                        stringBuilderForSource.AppendLine(@"volatile void* vfp = 0;");
                        stringBuilderForSource.Append("\t");
                        stringBuilderForSource.Append(@"asm volatile(""movq .rtcop_vfaddr");
                        stringBuilderForSource.Append(classCount);
                        stringBuilderForSource.Append(layerCount);
                        stringBuilderForSource.AppendLine(@"(%%rip), %0"" : ""=r""(vfp) : : ""memory""); ");
                        stringBuilderForSource.Append("\t");
                        stringBuilderForSource.AppendLine(@"return vfp;");
                        stringBuilderForSource.AppendLine(@"}");
                        stringBuilderForSource.AppendLine();
                    }
                    ++layerCount;
                }
                stringBuilderForSource.AppendLine();
                for (int i = 0; i < nss.Length; ++i)
                {
                    stringBuilderForSource.Append(@"namespace ");
                    stringBuilderForSource.Append(nss[i]);
                    stringBuilderForSource.Append(@" { ");
                }
                stringBuilderForSource.AppendLine();
                stringBuilderForSource.AppendLine(@"volatile void** GetVirtualFunctionTable(BaseLayer* layer)");
                stringBuilderForSource.AppendLine(@"{");
                stringBuilderForSource.Append("\t");
                stringBuilderForSource.AppendLine(@"volatile void** vft = 0;");
                stringBuilderForSource.Append("\t");
                stringBuilderForSource.Append(@"asm volatile(""movq .rtcop_vft");
                stringBuilderForSource.Append(classCount);
                stringBuilderForSource.AppendLine(@"0(%%rip), %0"" : ""=r""(vft) : : ""memory""); ");
                stringBuilderForSource.Append("\t");
                stringBuilderForSource.AppendLine(@"return vft;");
                stringBuilderForSource.AppendLine(@"}");
                stringBuilderForSource.AppendLine();
                layerCount = 1;
                foreach (var layerStructure in layerStructures)
                {
                    if (layerStructure._FindedPartialClasses[classCount] != null)
                    {
                        stringBuilderForSource.Append(@"volatile void** GetVirtualFunctionTable(");
                        stringBuilderForSource.Append(layerStructure.LayerName);
                        stringBuilderForSource.AppendLine(@"* layer)");
                        stringBuilderForSource.AppendLine(@"{");
                        stringBuilderForSource.Append("\t");
                        stringBuilderForSource.AppendLine(@"volatile void** vft = 0;");
                        stringBuilderForSource.Append("\t");
                        stringBuilderForSource.Append(@"asm volatile(""movq .rtcop_vft");
                        stringBuilderForSource.Append(classCount);
                        stringBuilderForSource.Append(layerCount);
                        stringBuilderForSource.AppendLine(@"(%%rip), %0"" : ""=r""(vft) : : ""memory""); ");
                        stringBuilderForSource.Append("\t");
                        stringBuilderForSource.AppendLine(@"return vft;");
                        stringBuilderForSource.AppendLine(@"}");
                        stringBuilderForSource.AppendLine();
                    }
                    ++layerCount;
                }
                var baseMethodList = baseMethodLists[classCount];
                foreach (var baseMethod in baseMethodList)
                {
                    if (baseMethod != null)
                    {
                        stringBuilderForSource.Append(baseMethod.ReturnType);
                        stringBuilderForSource.Append(@" ExecuteProceed_");
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
                        stringBuilderForSource.Append(baseMethod.ReturnType);
                        stringBuilderForSource.Append(@"(*pProceed)(void*");
                        foreach (var param in baseMethod.Parameters)
                        {
                            stringBuilderForSource.Append(@", ");
                            stringBuilderForSource.Append(param.Type);
                        }
                        stringBuilderForSource.Append(@") = (");
                        stringBuilderForSource.Append(baseMethod.ReturnType);
                        stringBuilderForSource.Append(@"(*)(void*");
                        foreach (var param in baseMethod.Parameters)
                        {
                            stringBuilderForSource.Append(@", ");
                            stringBuilderForSource.Append(param.Type);
                        }
                        stringBuilderForSource.AppendLine(@"))proceed;");
                        stringBuilderForSource.Append("\t");
                        if (!((baseMethod.ReturnType.Type.ToString() == "void") && (baseMethod.ReturnType.Pointers.Count == 0)))
                        {
                            stringBuilderForSource.Append(@"return ");
                        }
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
                    stringBuilderForSource.Append(@"}");
                }
                stringBuilderForSource.AppendLine();
                stringBuilderForSource.AppendLine();
                ++classCount;
            }
            stringBuilderForSource.AppendLine(@"} // namespace DependentCode {}");
            stringBuilderForSource.AppendLine(@"} // namespace Generated {}");
            stringBuilderForSource.AppendLine(@"} // namespace RTCOP {}");
            result.CodeDictionary[@"DependentCode.cpp"] = stringBuilderForSource.ToString();
            result.CodeDictionary[@"DependentCodeAsm.asm"] = "";
        }


        /// <summary>
        /// 環境依存コード(Windows 32bit minGW)
        /// </summary>
        private void GenerateDependentCodeForWindowsX86MinGW(GeneratedCodes result, LayerStructure baseLayerStructure, List<LayerStructure> layerStructures, List<string> baseClassNameList, List<List<LayerdMethodDefinition>> baseMethodLists)
        {
            throw new Exception(@"Windows 32bit MinGWは現在未対応");

            // cppファイル
            //StringBuilder stringBuilderForSource = new StringBuilder();
        }

    }
}
