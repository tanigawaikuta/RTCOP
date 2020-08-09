using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace LayerCompiler.Parsers.Model
{
    /// <summary>
    /// コメント
    /// </summary>
    class Comment
    {
        #region プロパティ
        /// <summary>
        /// テキスト
        /// </summary>
        public string Text { get; protected set; }

        #endregion

        #region コンストラクタ
        /// <summary>
        /// コメント
        /// </summary>
        /// <param name="text">テキスト</param>
        public Comment(string text)
        {
            Text = text;
        }

        #endregion

        #region メソッド
        /// <summary>
        /// 文字列を返す
        /// </summary>
        /// <returns>文字列</returns>
        public override string ToString()
        {
            return Text;
        }

        #endregion
    }
}
