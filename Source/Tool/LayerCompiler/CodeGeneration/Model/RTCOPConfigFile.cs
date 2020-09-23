using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayerCompiler.CodeGeneration.Model
{
    /// <summary>
    /// RTCOPコンフィグファイル
    /// </summary>
    [Serializable]
    class RTCOPConfigFile
    {
        #region プロパティ
        /// <summary>
        /// ベースレイヤコンフィグ
        /// </summary>
        public LayerConfig BaseLayerConfig { get; protected set; }

        /// <summary>
        /// レイヤコンフィグ
        /// </summary>
        public List<LayerConfig> LayerConfigs { get; private set; } = new List<LayerConfig>();

        #endregion

        #region コンストラクタ
        /// <summary>
        /// RTCOPコンフィグファイル
        /// </summary>
        protected RTCOPConfigFile()
        {
        }

        #endregion

        #region メソッド
        /// <summary>
        /// RTCOPコンフィグファイルの保存
        /// </summary>
        /// <param name="path">ファイルパス</param>
        /// <param name="file">RTCOPコンフィグファイル</param>
        public static void SaveFile(string path, RTCOPConfigFile file)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(@"<?xml version = ""1.0"" encoding = ""UTF-8""?>");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(@"<RTCOPConfig>");
            stringBuilder.AppendLine(@"  <BaseLayerConfig>");
            stringBuilder.AppendLine(@"  </BaseLayerConfig>");
            stringBuilder.AppendLine(@"  <LayerConfigs>");
            foreach (var lconfig in file.LayerConfigs)
            {
                stringBuilder.Append(@"    <LayerConfig layername=""");
                stringBuilder.Append(lconfig.LayerName);
                stringBuilder.AppendLine(@""">");
                stringBuilder.Append(@"      <ExecutionPriority>");
                stringBuilder.Append(lconfig.ExecutionPriority);
                stringBuilder.AppendLine(@"</ExecutionPriority>");
                stringBuilder.AppendLine(@"    </LayerConfig>");
            }
            stringBuilder.AppendLine(@"  </LayerConfigs>");
            stringBuilder.AppendLine(@"</RTCOPConfig>");
            // ファイル書き込み
            string text = stringBuilder.ToString();
            string dirpath = Path.GetDirectoryName(path);
            if (!Directory.Exists(dirpath))
            {
                Directory.CreateDirectory(dirpath);
            }
            using (StreamWriter sw = new StreamWriter(path, false, new UTF8Encoding(false)))
            {
                sw.Write(text);
            }
        }

        /// <summary>
        /// RTCOPコンフィグファイルの読み込み
        /// </summary>
        /// <param name="path">ファイルパス</param>
        /// <returns>RTCOPコンフィグファイル</returns>
        public static RTCOPConfigFile LoadFile(string path)
        {
            RTCOPConfigFile result = new RTCOPConfigFile();
            XElement xml = XElement.Load(path);
            var baseLayerElement = xml.Element(@"BaseLayerConfig");
            if ((baseLayerElement != null) && (result.BaseLayerConfig == null))
            {
                string layerName = "baselayer";
                int executionPriority = 0;
                var executionPriorityElement = baseLayerElement.Element(@"ExecutionPriority");
                if (executionPriorityElement != null)
                {
                    executionPriority = int.Parse(executionPriorityElement.Value);
                }
                result.BaseLayerConfig = new LayerConfig(layerName, executionPriority);
            }
            var layerElements = from elements in xml.Elements(@"LayerConfigs")
                                from element in elements.Elements(@"LayerConfig")
                                where element.Attribute(@"layername") != null
                                select element;
            foreach (var layerElement in layerElements)
            {
                string layerName = layerElement.Attribute(@"layername").Value;
                if (!(result.LayerConfigs.Exists((obj) => obj.LayerName == layerName)))
                {
                    int executionPriority = 0;
                    var executionPriorityElement = layerElement.Element(@"ExecutionPriority");
                    if (executionPriorityElement != null)
                    {
                        executionPriority = int.Parse(executionPriorityElement.Value);
                    }
                    var layerConfig = new LayerConfig(layerName, executionPriority);
                    result.LayerConfigs.Add(layerConfig);
                }
            }
            // 実行優先度で昇順ソート
            result.LayerConfigs.Sort((a, b) => a.ExecutionPriority - b.ExecutionPriority);
            return result;
        }

        #endregion

    }

    /// <summary>
    /// レイヤコンフィグ
    /// </summary>
    [Serializable]
    class LayerConfig
    {
        #region プロパティ
        /// <summary>
        /// レイヤ名
        /// </summary>
        public string LayerName { get; protected set; }

        /// <summary>
        /// 実行優先度
        /// </summary>
        public int ExecutionPriority { get; protected set; }

        #endregion

        #region コンストラクタ
        /// <summary>
        /// レイヤコンフィグ
        /// </summary>
        public LayerConfig(string layerName, int executionPriority)
        {
            LayerName = layerName;
            ExecutionPriority = executionPriority;
        }

        #endregion

    }

}
