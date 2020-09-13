using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace LayerCompiler.CodeGeneration.Model
{
    /// <summary>
    /// レイヤ構造ファイル(.lstr)
    /// </summary>
    [Serializable]
    class LayerStructureFile
    {
        #region プロパティ
        /// <summary>
        /// ベースレイヤ構造の情報
        /// </summary>
        public LayerStructure BaseLayerStructure { get; internal set; }

        /// <summary>
        /// レイヤ構造の情報
        /// </summary>
        public List<LayerStructure> LayerStructures { get; private set; } = new List<LayerStructure>();

        /// <summary>
        /// ベースクラスヘッダの要素
        /// </summary>
        public List<object> BaseClassHeaderItems { get; private set; } = new List<object>();

        /// <summary>
        /// 読み込まれたベースクラスヘッダの情報
        /// </summary>
        public List<ImportedFileInfomation> ImportedBaseClassHeaderInfomation { get; private set; } = new List<ImportedFileInfomation>();

        #endregion

        #region コンストラクタ
        /// <summary>
        /// レイヤ構造ファイル
        /// </summary>
        public LayerStructureFile()
        {
        }

        #endregion

        #region メソッド
        /// <summary>
        /// レイヤ構造ファイルの保存
        /// </summary>
        /// <param name="path">ファイルパス</param>
        /// <param name="file">レイヤ構造ファイル</param>
        public static void SaveFile(string path, LayerStructureFile file)
        {
            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, file);
            }
        }

        /// <summary>
        /// レイヤ構造ファイルの読み込み
        /// </summary>
        /// <param name="path">ファイルパス</param>
        /// <returns>レイヤ構造ファイル</returns>
        public static LayerStructureFile LoadFile(string path)
        {
            LayerStructureFile result = null;
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                BinaryFormatter bf = new BinaryFormatter();
                result = (LayerStructureFile)bf.Deserialize(fs);
            }
            return result;
        }

        #endregion

    }

    /// <summary>
    /// レイヤ構造
    /// </summary>
    [Serializable]
    class LayerStructure
    {
        #region フィールド
        /// <summary>
        /// 作業用、いつかソースコード整理したい…
        /// </summary>
        [NonSerialized]
        internal List<Parsers.Model.LayerdClassDefinition> _FindedPartialClasses;

        #endregion

        #region プロパティ
        /// <summary>
        /// レイヤ名
        /// </summary>
        public string LayerName { get; protected set; }

        /// <summary>
        /// ヘッダファイルの要素
        /// </summary>
        public List<object> HeaderFileItems { get; private set; }

        /// <summary>
        /// ソースファイルの要素
        /// </summary>
        public List<object> SourceFileItems { get; private set; }

        /// <summary>
        /// 読み込まれた.lhファイルの情報
        /// </summary>
        public List<ImportedFileInfomation> ImportedLhInfomation { get; private set; } = new List<ImportedFileInfomation>();

        #endregion

        #region コンストラクタ
        /// <summary>
        /// レイヤ構造
        /// </summary>
        /// <param name="layerName">レイヤ名</param>
        /// <param name="headerItems">ヘッダファイルの要素</param>
        /// <param name="sourceItems">ソースファイルの要素</param>
        public LayerStructure(string layerName, IEnumerable<object> headerItems = null, IEnumerable<object> sourceItems = null)
        {
            LayerName = layerName;
            if (headerItems != null)
            {
                HeaderFileItems = new List<object>(headerItems);
            }
            else
            {
                HeaderFileItems = new List<object>();
            }
            if (sourceItems != null)
            {
                SourceFileItems = new List<object>(sourceItems);
            }
            else
            {
                SourceFileItems = new List<object>();
            }
        }

        #endregion

        #region メソッド
        /// <summary>
        /// 文字列を返す
        /// </summary>
        /// <returns>文字列</returns>
        public override string ToString()
        {
            return LayerName;
        }

        #endregion

    }

    /// <summary>
    /// 読み込みファイルの情報
    /// </summary>
    [Serializable]
    class ImportedFileInfomation
    {
        #region プロパティ
        /// <summary>
        /// ファイルパス
        /// </summary>
        public string FilePath { get; protected set; }

        /// <summary>
        /// 読み込み初めのインデックス
        /// </summary>
        public int StartIndex { get; internal set; }

        /// <summary>
        /// 読み込んだ要素数
        /// </summary>
        public int NumOfItems { get; internal set; }

        #endregion

        #region コンストラクタ
        public ImportedFileInfomation(string filePath)
        {
            FilePath = filePath;
        }

        #endregion

        #region メソッド
        /// <summary>
        /// 文字列を返す
        /// </summary>
        /// <returns>文字列</returns>
        public override string ToString()
        {
            return FilePath;
        }

        #endregion

    }

}
