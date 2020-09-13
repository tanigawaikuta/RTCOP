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
    /// 生成されたコード
    /// </summary>
    //[Serializable]
    class GeneratedCodes
    {
        #region プロパティ
        /// <summary>
        /// 生成されたコードの辞書、キーがファイル名、値がファイルの内容となっている
        /// </summary>
        public Dictionary<string, string> CodeDictionary { get; private set; } = new Dictionary<string, string>();

        #endregion

        #region コンストラクタ
        /// <summary>
        /// 生成されたコード
        /// </summary>
        public GeneratedCodes()
        {
        }

        #endregion

    }

}
