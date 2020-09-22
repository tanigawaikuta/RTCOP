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
using Sprache;

namespace LayerCompiler.CodeGeneration
{
    /// <summary>
    /// RTCOPファイルのコンパイラ
    /// </summary>
    class RTCOPCompiler
    {
        #region コンストラクタ
        /// <summary>
        /// RTCOPファイルのコンパイラ
        /// </summary>
        public RTCOPCompiler()
        {
        }

        #endregion

        #region メソッド
        /// <summary>
        /// コンパイル
        /// </summary>
        /// <param name="sourceFile">ソースファイル</param>
        /// <returns>レイヤ構造ファイル</returns>
        public LayerStructureFile Compile(RTCOPSourceFile sourceFile)
        {
            // 結果を格納するためのレイヤ構造ファイル
            LayerStructureFile result = new LayerStructureFile();
            // パーサに通す
            string sourceCode = sourceFile.Text;
            var items = RTCOPParser.RTCOPSourceFile.Parse(sourceCode);
            // 各レイヤ共通のアイテム
            List<object> commonItemsS = new List<object>();
            List<object> commonItemsH = new List<object>();
            // 要素の解釈
            foreach (object item in items)
            {
                // レイヤ定義の場合
                if (item is LayerDefinition)
                {
                    var layerDefinition = (LayerDefinition)item;
                    if (layerDefinition.Name == "baselayer")
                    {
                        if (result.BaseLayerStructure == null)
                        {
                            result.BaseLayerStructure = new LayerStructure("baselayer");
                        }
                        result.BaseLayerStructure.SourceFileItems.Add(layerDefinition);
                    }
                    else
                    {
                        var layerStructure = result.LayerStructures.Find((obj) => obj.LayerName == layerDefinition.Name);
                        if (layerStructure == null)
                        {
                            layerStructure = new LayerStructure(layerDefinition.Name);
                            result.LayerStructures.Add(layerStructure);
                        }
                        layerStructure.SourceFileItems.Add(layerDefinition);
                    }
                }
                // それ以外
                else
                {
                    commonItemsS.Add(item);
                }
            }
            // 各レイヤ共通のアイテムの反映
            if (result.BaseLayerStructure != null)
            {
                result.BaseLayerStructure.SourceFileItems.InsertRange(0, commonItemsS);
            }
            foreach (var layerStructure in result.LayerStructures)
            {
                layerStructure.SourceFileItems.InsertRange(0, commonItemsS);
            }

            // ヘッダファイルの解釈
            Action<RTCOPSourceFile> InterpretImportedFiles = null;
            InterpretImportedFiles = (src) =>
            {
                foreach (RTCOPSourceFile impFile in src.ImportedFiles)
                {
                    // .lh内でさらに読み込まれているファイルをチェック
                    InterpretImportedFiles(impFile);

                    // パーサに通す
                    string impFileCode = impFile.Text;
                    var impFileItems = RTCOPParser.RTCOPSourceFile.Parse(impFileCode);
                    // ファイルの種類によって処理方法を変える
                    if (Path.GetExtension(impFile.FilePath) == ".lh")
                    {
                        List<string> layerNames = new List<string>();
                        // 要素の解釈
                        foreach (object item in impFileItems)
                        {
                            // レイヤ定義の場合
                            if (item is LayerDefinition)
                            {
                                var layerDefinition = (LayerDefinition)item;
                                layerNames.Add(layerDefinition.Name);
                                // ヘッダ要素の追加
                                if (layerDefinition.Name == "baselayer")
                                {
                                    // ベースレイヤ
                                    if (result.BaseLayerStructure == null)
                                    {
                                        result.BaseLayerStructure = new LayerStructure("baselayer");
                                    }
                                    ImportedFileInfomation ifi = result.BaseLayerStructure.ImportedLhInfomation.Find((obj) => obj.FilePath == impFile.FilePath);
                                    if (ifi == null)
                                    {
                                        ifi = new ImportedFileInfomation(impFile.FilePath);
                                        ifi.StartIndex = result.BaseLayerStructure.HeaderFileItems.Count;
                                        ifi.NumOfItems = 0;
                                        result.BaseLayerStructure.ImportedLhInfomation.Add(ifi);
                                    }
                                    ++ifi.NumOfItems;
                                    result.BaseLayerStructure.HeaderFileItems.Add(layerDefinition);
                                }
                                else
                                {
                                    // ベースレイヤ以外
                                    var layerStructure = result.LayerStructures.Find((obj) => obj.LayerName == layerDefinition.Name);
                                    if (layerStructure == null)
                                    {
                                        layerStructure = new LayerStructure(layerDefinition.Name);
                                        result.LayerStructures.Add(layerStructure);
                                    }
                                    ImportedFileInfomation ifi = layerStructure.ImportedLhInfomation.Find((obj) => obj.FilePath == impFile.FilePath);
                                    if (ifi == null)
                                    {
                                        ifi = new ImportedFileInfomation(impFile.FilePath);
                                        ifi.StartIndex = layerStructure.HeaderFileItems.Count;
                                        ifi.NumOfItems = 0;
                                        layerStructure.ImportedLhInfomation.Add(ifi);
                                    }
                                    ++ifi.NumOfItems;
                                    layerStructure.HeaderFileItems.Add(layerDefinition);
                                }
                            }
                            // それ以外
                            else
                            {
                                commonItemsH.Add(item);
                            }
                        }
                        // 各レイヤ共通のアイテムの反映
                        if (layerNames.Contains("baselayer"))
                        {
                            ImportedFileInfomation ifi = result.BaseLayerStructure.ImportedLhInfomation.Find((obj) => obj.FilePath == impFile.FilePath);
                            ifi.NumOfItems += commonItemsH.Count;
                            result.BaseLayerStructure.HeaderFileItems.InsertRange(ifi.StartIndex, commonItemsH);
                        }
                        foreach (var layerStructure in result.LayerStructures)
                        {
                            if (layerNames.Contains(layerStructure.LayerName))
                            {
                                ImportedFileInfomation ifi = layerStructure.ImportedLhInfomation.Find((obj) => obj.FilePath == impFile.FilePath);
                                ifi.NumOfItems += commonItemsH.Count;
                                layerStructure.HeaderFileItems.InsertRange(ifi.StartIndex, commonItemsH);
                            }
                        }
                    }
                    // .lhファイル以外
                    else
                    {
                        ImportedFileInfomation ifi = new ImportedFileInfomation(impFile.FilePath);
                        ifi.StartIndex = result.BaseClassHeaderItems.Count;
                        ifi.NumOfItems = impFileItems.Count();
                        result.ImportedBaseClassHeaderInfomation.Add(ifi);
                        result.BaseClassHeaderItems.AddRange(impFileItems);
                    }
                }
            };
            InterpretImportedFiles(sourceFile);
            // 結果を返す
            return result;
        }

        /// <summary>
        /// レイヤ構造ファイルのマージ
        /// </summary>
        /// <param name="structureFiles">マージしたいレイヤ構造ファイル</param>
        /// <returns>マージされたレイヤ構造ファイル</returns>
        public LayerStructureFile MergeObjectFiles(IEnumerable<LayerStructureFile> structureFiles)
        {
            // 結果を格納するためのオブジェクトファイル
            LayerStructureFile result = new LayerStructureFile();
            // マージ
            foreach (var structureFile in structureFiles)
            {
                // ベースクラスヘッダを重複を防いでマージ
                foreach (var ifi in structureFile.ImportedBaseClassHeaderInfomation)
                {
                    var ifi2 = result.ImportedBaseClassHeaderInfomation.Find((obj) => obj.FilePath == ifi.FilePath);
                    if (ifi2 == null)
                    {
                        ifi2 = new ImportedFileInfomation(ifi.FilePath);
                        ifi2.StartIndex = result.BaseClassHeaderItems.Count;
                        ifi2.NumOfItems = ifi.NumOfItems;
                        result.ImportedBaseClassHeaderInfomation.Add(ifi2);
                        var range = structureFile.BaseClassHeaderItems.GetRange(ifi.StartIndex, ifi.NumOfItems);
                        result.BaseClassHeaderItems.AddRange(range);
                    }
                }
                // ベースレイヤ
                if (structureFile.BaseLayerStructure != null)
                {
                    if (result.BaseLayerStructure == null)
                    {
                        result.BaseLayerStructure = new LayerStructure("baselayer");
                    }
                    result.BaseLayerStructure.SourceFileItems.AddRange(structureFile.BaseLayerStructure.SourceFileItems);
                    // ヘッダ重複を防いでマージ
                    foreach (var ifi in structureFile.BaseLayerStructure.ImportedLhInfomation)
                    {
                        var ifi2 = result.BaseLayerStructure.ImportedLhInfomation.Find((obj) => obj.FilePath == ifi.FilePath);
                        if (ifi2 == null)
                        {
                            ifi2 = new ImportedFileInfomation(ifi.FilePath);
                            ifi2.StartIndex = result.BaseLayerStructure.HeaderFileItems.Count;
                            ifi2.NumOfItems = ifi.NumOfItems;
                            result.BaseLayerStructure.ImportedLhInfomation.Add(ifi2);
                            var range = structureFile.BaseLayerStructure.HeaderFileItems.GetRange(ifi.StartIndex, ifi.NumOfItems);
                            result.BaseLayerStructure.HeaderFileItems.AddRange(range);
                        }
                    }
                }
                // ベースレイヤ以外
                foreach (var layerStructure in structureFile.LayerStructures)
                {
                    var rld = result.LayerStructures.Find((obj) => obj.LayerName == layerStructure.LayerName);
                    if (rld == null)
                    {
                        rld = new LayerStructure(layerStructure.LayerName);
                        result.LayerStructures.Add(rld);
                    }
                    rld.SourceFileItems.AddRange(layerStructure.SourceFileItems);
                    // ヘッダ重複を防いでマージ
                    foreach (var ifi in layerStructure.ImportedLhInfomation)
                    {
                        var ifi2 = rld.ImportedLhInfomation.Find((obj) => obj.FilePath == ifi.FilePath);
                        if (ifi2 == null)
                        {
                            ifi2 = new ImportedFileInfomation(ifi.FilePath);
                            ifi2.StartIndex = rld.HeaderFileItems.Count;
                            ifi2.NumOfItems = ifi.NumOfItems;
                            rld.ImportedLhInfomation.Add(ifi2);
                            var range = layerStructure.HeaderFileItems.GetRange(ifi.StartIndex, ifi.NumOfItems);
                            rld.HeaderFileItems.AddRange(range);
                        }
                    }
                }
            }
            // 結果を返す
            return result;
        }

        #endregion

    }
}
