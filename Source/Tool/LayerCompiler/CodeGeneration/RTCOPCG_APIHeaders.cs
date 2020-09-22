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
            stringBuilderForNew.Append("\t");
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
                stringBuilderForActivation.AppendLine(@",");
                ++layercount;
            }
            stringBuilderForActivation.AppendLine(@"};");
            stringBuilderForActivation.AppendLine(@"} // namespace Generated {}");
            stringBuilderForActivation.AppendLine();
            stringBuilderForActivation.AppendLine(@"inline void activate(Generated::LayerID layerid)");
            stringBuilderForActivation.AppendLine(@"{");
            stringBuilderForActivation.Append("\t");
            stringBuilderForActivation.AppendLine(@"Activate((int)layerid);");
            stringBuilderForActivation.AppendLine(@"}");
            stringBuilderForActivation.AppendLine();
            stringBuilderForActivation.AppendLine(@"inline void deactivate(Generated::LayerID layerid)");
            stringBuilderForActivation.AppendLine(@"{");
            stringBuilderForActivation.Append("\t");
            stringBuilderForActivation.AppendLine(@"Deactivate((int)layerid);");
            stringBuilderForActivation.AppendLine(@"}");
            stringBuilderForActivation.AppendLine();
            stringBuilderForActivation.AppendLine(@"} // namespace RTCOP {}");
            stringBuilderForActivation.AppendLine(@"#endif // !__RTCOP_GENERATED_ACTIVATION__");
            result.CodeDictionary[@"ActivationForApp.h"] = stringBuilderForActivation.ToString();
        }

    }
}
