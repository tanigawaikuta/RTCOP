using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayerCompiler.Model
{
    /// <summary>
    /// レイヤ定義
    /// </summary>
    class LayerDefinition
    {
        #region プロパティ]
        /// <summary>
        /// レイヤ名
        /// </summary>
        public string Name { get; protected set; }

        #endregion

        #region コンストラクタ
        /// <summary>
        /// レイヤ定義
        /// </summary>
        public LayerDefinition(string name)
        {
            Name = name;
        }

        #endregion

        #region メソッド
        /// <summary>
        /// 文字列を返す
        /// </summary>
        /// <returns>文字列</returns>
        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}
