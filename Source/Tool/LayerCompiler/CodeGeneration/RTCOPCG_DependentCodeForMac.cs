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
        /// 環境依存コード(macOS)
        /// </summary>
        private void GenerateDependentCodeForMacOSX64(GeneratedCodes result, LayerStructure baseLayerStructure, List<LayerStructure> layerStructures, List<string> baseClassNameList, List<List<LayerdMethodDefinition>> baseMethodLists)
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
                stringBuilderForSource.Append(@"0: .quad __ZTVN5RTCOP4Core12LayerdObjectI");
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
                        stringBuilderForSource.Append(@": .quad __ZTVN");
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
                        stringBuilderForSource.Append(@": .quad __ZN");
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

    }
}
