using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace LayerCompiler.Parsers.Model
{
    /// <summary>
    /// トークン
    /// </summary>
    class Token
    {
        #region プロパティ
        /// <summary>
        /// テキスト
        /// </summary>
        public string Text { get; protected set; }

        #endregion

        #region コンストラクタ
        /// <summary>
        /// トークン
        /// </summary>
        /// <param name="text">テキスト</param>
        public Token(string text)
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

    /// <summary>
    /// 識別子
    /// </summary>
    class Identifier : Token
    {
        #region コンストラクタ
        /// <summary>
        /// 識別子
        /// </summary>
        /// <param name="text">テキスト</param>
        public Identifier(string text)
            : base(text)
        {
        }

        #endregion
    }

    /// <summary>
    /// キーワード
    /// </summary>
    class Keyword : Token
    {
        #region コンストラクタ
        /// <summary>
        /// キーワード
        /// </summary>
        /// <param name="text">テキスト</param>
        public Keyword(string text)
            : base(text)
        {
        }

        #endregion

    }

    /// <summary>
    /// 演算子と区切り文字
    /// </summary>
    class OperatorOrPunctuator : Token
    {
        #region コンストラクタ
        /// <summary>
        /// 演算子と区切り文字
        /// </summary>
        /// <param name="text">テキスト</param>
        public OperatorOrPunctuator(string text)
            : base(text)
        {
        }

        #endregion
    }

    /// <summary>
    /// リテラル
    /// </summary>
    class Literal : Token
    {
        #region コンストラクタ
        /// <summary>
        /// リテラル
        /// </summary>
        /// <param name="text">テキスト</param>
        public Literal(string text)
            : base(text)
        {
        }

        #endregion

    }

    /// <summary>
    /// 整数リテラル
    /// </summary>
    class IntegerLiteral : Literal
    {
        #region プロパティ
        /// <summary>
        /// サフィックス
        /// </summary>
        public string Suffix { get; protected set; }

        /// <summary>
        /// ユーザサフィックス
        /// </summary>
        public string UserSuffix { get; protected set; }

        /// <summary>
        /// 値
        /// </summary>
        public long Value { get; protected set; }

        #endregion

        #region コンストラクタ
        /// <summary>
        /// 整数リテラル
        /// </summary>
        /// <param name="text">テキスト</param>
        /// <param name="value">数値</param>
        /// <param name="suffix">サフィックス</param>
        /// <param name="usersuffix">ユーザサフィックス</param>
        public IntegerLiteral(string text, string value, string suffix, string usersuffix)
            : base(text)
        {
            // 0
            if (value == "0")
            {
                Value = 0;
            }
            // 16進数
            else if (value[0] == '0' && ((value[1] == 'x') || (value[1] == 'X')))
            {
                string s = value.Substring(2);
                Value = Convert.ToInt64(s, 16);
            }
            // 8進数
            else if (value[0] == '0')
            {
                string s = value.Substring(1);
                Value = Convert.ToInt64(s, 8);
            }
            // 10進数
            else
            {
                Value = long.Parse(value);
            }
            // サフィックス
            Suffix = suffix;
            UserSuffix = usersuffix;
        }

        #endregion

    }

    /// <summary>
    /// 文字リテラル
    /// </summary>
    class CharacterLiteral : Literal
    {
        #region プロパティ
        /// <summary>
        /// プレフィックス
        /// </summary>
        public string Prefix { get; protected set; }

        /// <summary>
        /// ユーザサフィックス
        /// </summary>
        public string UserSuffix { get; protected set; }

        /// <summary>
        /// 文字コード
        /// </summary>
        public uint Value { get; protected set; }

        /// <summary>
        /// 文字
        /// </summary>
        public char Character { get { return (char)Value; } }

        #endregion

        #region コンストラクタ
        /// <summary>
        /// 文字リテラル
        /// </summary>
        /// <param name="text">テキスト</param>
        /// <param name="prefix">プレフィックス</param>
        /// <param name="character">文字</param>
        /// <param name="usersuffix">ユーザサフィックス</param>
        public CharacterLiteral(string text, string prefix, string character, string usersuffix)
            : base(text)
        {
            Prefix = prefix;
            UserSuffix = usersuffix;
            if ((character[0] == '\\') && (character[1] == 'U'))
            {
                string c2 = character.Substring(2);
                Value = Convert.ToUInt32(c2, 16);
            }
            else
            {
                Value = Regex.Unescape(character)[0];
            }
        }

        #endregion

    }

    /// <summary>
    /// 浮動小数リテラル
    /// </summary>
    class FloatingLiteral : Literal
    {
        #region プロパティ
        /// <summary>
        /// サフィックス
        /// </summary>
        public string Suffix { get; protected set; }

        /// <summary>
        /// ユーザサフィックス
        /// </summary>
        public string UserSuffix { get; protected set; }

        /// <summary>
        /// 値
        /// </summary>
        public double Value { get; protected set; }

        #endregion

        #region コンストラクタ
        /// <summary>
        /// 浮動小数リテラル
        /// </summary>
        /// <param name="text">テキスト</param>
        /// <param name="value">数値</param>
        /// <param name="suffix">サフィックス</param>
        /// <param name="usersuffix">ユーザサフィックス</param>
        public FloatingLiteral(string text, string value, string suffix, string usersuffix)
            : base(text)
        {
            // 変換
            double outValue = 0;
            if (double.TryParse(value, out outValue))
            {
                Value = outValue;
            }
            // サフィックス
            Suffix = suffix;
            UserSuffix = usersuffix;
        }

        #endregion

    }

    /// <summary>
    /// 文字列リテラル
    /// </summary>
    class StringLiteral : Literal
    {
        #region プロパティ
        /// <summary>
        /// プレフィックス
        /// </summary>
        public string Prefix { get; protected set; }

        /// <summary>
        /// ユーザサフィックス
        /// </summary>
        public string UserSuffix { get; protected set; }

        /// <summary>
        /// 文字列
        /// </summary>
        public string String { get; protected set; }

        #endregion

        #region コンストラクタ
        /// <summary>
        /// 文字列リテラル
        /// </summary>
        /// <param name="text">テキスト</param>
        /// <param name="value">数値</param>
        /// <param name="prefix">プレフィックス</param>
        /// <param name="usersuffix">ユーザサフィックス</param>
        public StringLiteral(string text, string value, string prefix, string usersuffix)
            : base(text)
        {
            // プレフィックス
            Prefix = prefix;
            // サフィックス
            UserSuffix = usersuffix;
            // プレフィックス分の文字数だけ進める
            string s = text.Substring(Prefix.Length);
            // 生の文字列なら
            if (s[0] == 'R')
            {
                String = value;
            }
            // 通常の文字列の場合、エスケープシーケンスを置き換える
            else
            {
                // \U が入っている場合は面倒だからあきらめる
                try { String = Regex.Unescape(value); } catch { }
            }
        }

        #endregion

    }

    /// <summary>
    /// 真偽値リテラル
    /// </summary>
    class BooleanLiteral : Literal
    {
        #region プロパティ
        /// <summary>
        /// 値
        /// </summary>
        public bool Value { get; protected set; }

        #endregion

        #region コンストラクタ
        /// <summary>
        /// 真偽リテラル
        /// </summary>
        /// <param name="text">テキスト</param>
        public BooleanLiteral(string text)
            : base(text)
        {
            if (text == "true")
            {
                Value = true;
            }
            else
            {
                Value = false;
            }
        }

        #endregion

    }

    /// <summary>
    /// ポインタリテラル
    /// </summary>
    class PointerLiteral : Literal
    {
        #region プロパティ
        /// <summary>
        /// 値
        /// </summary>
        public IntPtr Value { get; protected set; }

        #endregion

        #region コンストラクタ
        /// <summary>
        /// ポインタリテラル
        /// </summary>
        /// <param name="text">テキスト</param>
        public PointerLiteral(string text)
            : base(text)
        {
            // nullptrしかない
            Value = IntPtr.Zero;
        }

        #endregion

    }

}
