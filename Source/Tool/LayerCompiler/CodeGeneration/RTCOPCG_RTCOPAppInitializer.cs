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
        /// RTCOPAppInitializerの生成
        /// </summary>
        private void GenerateRTCOPAppInitializer(GeneratedCodes result, int numOfLayers, int numOfClasses, int[] numOfMethods, List<LayerStructure> layerStructures)
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
            stringBuilderForSource.AppendLine(@"#include ""RTCOPAppInitializer.h""");
            stringBuilderForSource.AppendLine(@"#include ""RTCOP/Core/RTCOPManager.h""");
            stringBuilderForSource.AppendLine(@"#include ""RTCOP/Core/LayerdObjectInitializer.h""");
            stringBuilderForSource.AppendLine(@"#include ""RTCOP/Core/LayerActivater.h""");
            stringBuilderForSource.AppendLine(@"#include ""BaseLayer.h""");
            foreach (LayerStructure ls in layerStructures)
            {
                stringBuilderForSource.Append(@"#include """);
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
            stringBuilderForSource.AppendLine(@";");
            stringBuilderForSource.Append("\t");
            stringBuilderForSource.Append(@"int numOfClasses = ");
            stringBuilderForSource.Append(numOfClasses);
            stringBuilderForSource.AppendLine(@";");
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

    }
}
