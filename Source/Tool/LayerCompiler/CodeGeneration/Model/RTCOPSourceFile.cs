using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayerCompiler.Parsers.Model;

namespace LayerCompiler.CodeGeneration.Model
{
    /// <summary>
    /// RTCOPのソースファイル
    /// </summary>
    [Serializable]
    class RTCOPSourceFile
    {
        #region プロパティ
        /// <summary>
        /// ファイルパス
        /// </summary>
        public string FilePath { get; protected set; }

        /// <summary>
        /// ファイルの中身
        /// </summary>
        public string Text { get; protected set; }

        /// <summary>
        /// 読み込まれたファイル
        /// </summary>
        public List<RTCOPSourceFile> ImportedFiles { get; private set; }

        #endregion

        #region コンストラクタ
        /// <summary>
        /// RTCOPのソースファイル
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <param name="text">ファイルのテキスト</param>
        /// <param name="importedFiles">読み込まれたファイル</param>
        public RTCOPSourceFile(string filePath, string text, IEnumerable<RTCOPSourceFile> importedFiles = null)
        {
            FilePath = filePath;
            Text = text;
            if (importedFiles != null)
            {
                ImportedFiles = new List<RTCOPSourceFile>(importedFiles);
            }
            else
            {
                ImportedFiles = new List<RTCOPSourceFile>();
            }
        }

        #endregion

    }
}
