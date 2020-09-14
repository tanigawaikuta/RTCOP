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

    }
}
