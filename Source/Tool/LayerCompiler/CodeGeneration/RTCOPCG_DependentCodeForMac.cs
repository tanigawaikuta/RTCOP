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
        private void GenerateDependentCodeForMacOSX64(GeneratedCodes result, LayerStructure baseLayerStructure, List<LayerStructure> layerStructures, List<string> baseClassNameList, List<List<LayerdMethodDefinition>> baseMethodLists, string includeFilePath)
        {
            // まったく同じ
            GenerateDependentCodeForWindowsX64MinGW(result, baseLayerStructure, layerStructures, baseClassNameList, baseMethodLists, includeFilePath);
        }

    }
}
