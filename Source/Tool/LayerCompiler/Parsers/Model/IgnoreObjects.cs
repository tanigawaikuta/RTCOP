using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayerCompiler.Parsers.Model
{
    /// <summary>
    /// 無視するオブジェクト
    /// </summary>
    [Serializable]
    class IgnoreObject
    {
        #region プロパティ
        /// <summary>
        /// 無視する内容
        /// </summary>
        public object Content { get; protected set; }

        #endregion

        #region コンストラクタ
        /// <summary>
        /// 無視するオブジェクト
        /// </summary>
        /// <param name="content">無視する内容</param>
        public IgnoreObject(object content)
        {
            Content = content;
        }

        #endregion

        #region メソッド
        /// <summary>
        /// 文字列を返す
        /// </summary>
        /// <returns>文字列</returns>
        public override string ToString()
        {
            return Content.ToString();
        }

        #endregion

    }

    /// <summary>
    /// 無視するオブジェクトのブロック
    /// </summary>
    [Serializable]
    class IgnoreObjectBlock : IgnoreObject
    {
        #region プロパティ
        /// <summary>
        /// 無視する内容
        /// </summary>
        public List<object> Contents { get; protected set; }

        #endregion

        #region コンストラクタ
        /// <summary>
        /// 無視するオブジェクトのブロック
        /// </summary>
        /// <param name="begin">ブロックの初め</param>
        /// <param name="end">ブロックの終わり</param>
        /// <param name="contents">無視する内容</param>
        public IgnoreObjectBlock(object begin, object end, IEnumerable<object> contents)
            : base(null)
        {
            Contents = new List<object>();
            Contents.Add(begin);
            Contents.AddRange(contents);
            Contents.Add(end);
            Content = Contents;
        }

        #endregion

        #region メソッド
        /// <summary>
        /// 文字列を返す
        /// </summary>
        /// <returns>文字列</returns>
        public override string ToString()
        {
            string result = "";
            // コンテンツの書き出し
            foreach (IgnoreObject content in Contents)
            {
                // ディレクティブ
                if (content.Content is PreprocessDirective)
                {
                    string text = content.ToString();
                    result += (text + "\r\n");
                }
                // それ以外
                else
                {
                    if (content is IgnoreObjectBlock)
                    {
                        var block = (IgnoreObjectBlock)content;
                        string text = block.ToString();
                        result += (text + "\r\n");
                    }
                    else
                    {
                        string text = content.ToString();
                        // コンテンツに応じて後に続く文字を変更
                        if ((text == "{") || (text == "}") || (text == ";") || (text == ":"))
                        {
                            result += (text + "\r\n");
                        }
                        else
                        {
                            result += (text + " ");
                        }
                    }
                }
            }
            return result;
        }

        #endregion

    }

}
