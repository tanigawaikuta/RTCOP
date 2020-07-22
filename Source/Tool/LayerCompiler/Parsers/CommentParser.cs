using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Sprache;

using LayerCompiler.Model;

namespace LayerCompiler.Parsers
{
    /// <summary>
    /// コメントのパーサ
    /// </summary>
    static class CommentParser
    {
        #region パーサ
        /// <summary>
        /// 一行コメント
        /// </summary>
        public static readonly Parser<Comment> SingleLineComment =
                                                    from slash in Parse.String("//").Text()
                                                    from comment in Parse.Regex(@".*")
                                                    select new Comment(slash + comment);

        /// <summary>
        /// 複数行コメント
        /// </summary>
        public static readonly Parser<Comment> DelimitedComment =
                                                    from begin in Parse.String("/*").Text()
                                                    from comment in Parse.Regex(@"((/*\**[^\*/]+)|([^\*/]*))*")
                                                    from end in Parse.Regex(@"\*+/")
                                                    select new Comment(begin + comment + end);

        /// <summary>
        /// 一行コメントか複数行コメント
        /// </summary>
        public static readonly Parser<Comment> Comment =
                                                    SingleLineComment
                                                    .Or(DelimitedComment);

        #endregion

        #region メソッド
        /// <summary>
        /// 標準のTokenメソッドにコメントスキップ機能を追加したメソッド
        /// </summary>
        /// <typeparam name="T">パース後の結果の型</typeparam>
        /// <param name="parser">パーサ</param>
        /// <returns>パース後の結果</returns>
        public static Parser<T> TokenWithSkipComment<T>(this Parser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException("parser");

            return from leading in Comment.Or(Parse.WhiteSpace.Return((Comment)null)).Many()
                   from item in parser
                   from trailing in Comment.Or(Parse.WhiteSpace.Return((Comment)null)).Many()
                   select item;
        }

        /// <summary>
        /// 標準のTokenメソッドにコメントスキップ機能を追加したメソッド (プリプロセスパーサ用)
        /// </summary>
        /// <typeparam name="T">パース後の結果の型</typeparam>
        /// <param name="parser">パーサ</param>
        /// <returns>パース後の結果</returns>
        public static Parser<T> TokenWithSkipCommentForPreprocessParser<T>(this Parser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException("parser");

            return from leading in ((Parser<object>)Comment).Or(Parse.Regex(@"[ \t]")).Or(Parse.Regex(@"\\(\n|\r\n)")).Many()
                   from item in parser
                   from trailing in ((Parser<object>)Comment).Or(Parse.Regex(@"[ \t]")).Or(Parse.Regex(@"\\(\n|\r\n)")).Many()
                   select item;
        }

        #endregion

    }
}
