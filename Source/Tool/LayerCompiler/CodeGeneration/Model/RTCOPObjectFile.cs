using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayerCompiler.CodeGeneration.Model
{
    /// <summary>
    /// RTCOPのオブジェクトファイル
    /// </summary>
    [Serializable]
    class RTCOPObjectFile
    {
        #region プロパティ
        /// <summary>
        /// ファイルパス
        /// </summary>
        public string FilePath { get; protected set; }

        #endregion

        #region コンストラクタ
        public RTCOPObjectFile()
        {
        }

        #endregion

        #region メソッド
        #endregion
    }
}
