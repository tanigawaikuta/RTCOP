using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayerCompiler.Parsers.Model
{
    /// <summary>
    /// スーパークラスの定義
    /// </summary>
    [Serializable]
    class SuperClassDefinition
    {
        #region プロパティ
        /// <summary>
        /// クラス名
        /// </summary>
        public string ClassName { get; protected set; }

        /// <summary>
        /// アクセス修飾子
        /// </summary>
        public string Access { get; protected set; }

        #endregion

        #region コンストラクタ
        /// <summary>
        /// スーパークラスの定義
        /// </summary>
        /// <param name="name">クラス名</param>
        /// <param name="access">アクセス修飾子</param>
        public SuperClassDefinition(string name, string access)
        {
            ClassName = name;
            Access = access;
        }

        #endregion

        #region メソッド
        /// <summary>
        /// 文字列を返す
        /// </summary>
        /// <returns>文字列</returns>
        public override string ToString()
        {
            string result = Access + " " + ClassName;
            return result;
        }

        #endregion

    }

    /// <summary>
    /// 変数宣言
    /// </summary>
    [Serializable]
    class VariableDeclaration
    {
        #region プロパティ
        /// <summary>
        /// 型
        /// </summary>
        public VariableType Type { get; protected set; }

        /// <summary>
        /// 名前
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// 配列サイズ
        /// </summary>
        public List<long> ArraySizes { get; private set; }

        /// <summary>
        /// 式
        /// </summary>
        public List<Model.IgnoreObject> DefaultExpression { get; private set; }

        /// <summary>
        /// ストレージクラス
        /// </summary>
        public string StorageClass { get; protected set; }

        #endregion

        #region コンストラクタ
        /// <summary>
        /// 変数宣言
        /// </summary>
        /// <param name="name">名前</param>
        /// <param name="type">型</param>
        /// <param name="arraySizes">配列サイズ</param>
        /// <param name="dexpression">式</param>
        /// <param name="storageClass">ストレージクラス</param>
        public VariableDeclaration(string name, VariableType type, IEnumerable<long> arraySizes, IEnumerable<Model.IgnoreObject> dexpression, string storageClass)
        {
            Name = name;
            Type = type;
            ArraySizes = new List<long>(arraySizes);
            DefaultExpression = new List<IgnoreObject>(dexpression);
            StorageClass = storageClass;
        }

        #endregion

        #region メソッド
        /// <summary>
        /// 文字列を返す
        /// </summary>
        /// <returns>文字列</returns>
        public override string ToString()
        {
            string result = StorageClass;
            if (result != "")
            {
                result += " ";
            }
            result += Type.ToString();
            if (Name != "")
            {
                result += (" " + Name);
                if (ArraySizes.Count > 0)
                {
                    foreach (long size in ArraySizes)
                    {
                        string sizetext = (size != -1 ? size.ToString() : "");
                        result += ("[" + sizetext + "]");
                    }
                }
                if (DefaultExpression.Count > 0)
                {
                    result += " =";
                    foreach (IgnoreObject obj in DefaultExpression)
                    {
                        result += (" " + obj);
                    }
                }
            }
            return result;
        }

        #endregion

    }

    /// <summary>
    /// 変数型
    /// </summary>
    [Serializable]
    class VariableType
    {
        #region プロパティ
        /// <summary>
        /// 型情報
        /// </summary>
        public object Type { get; protected set; }

        /// <summary>
        /// ユーザ定義型であるか
        /// </summary>
        public bool IsUserDefinedType { get; protected set; }

        /// <summary>
        /// 修飾子
        /// </summary>
        public List<string> Modifiers { get; private set; }

        /// <summary>
        /// ポインタ
        /// </summary>
        public List<Pointer> Pointers { get; private set; }

        /// <summary>
        /// 参照
        /// </summary>
        public string Reference { get; private set; }

        /// <summary>
        /// constであるかどうか
        /// </summary>
        public bool IsConst
        {
            get
            {
                return Modifiers.Contains("const");
            }
        }

        /// <summary>
        /// unsignedであるかどうか
        /// </summary>
        public bool IsUnsigned
        {
            get
            {
                return Modifiers.Contains("unsigned");
            }
        }

        /// <summary>
        /// volatileであるかどうか
        /// </summary>
        public bool IsVolatile
        {
            get
            {
                return Modifiers.Contains("volatile");
            }
        }

        /// <summary>
        /// ポインタであるかどうか
        /// </summary>
        public bool IsPointer
        {
            get
            {
                return (Pointers.Count > 0);
            }
        }

        /// <summary>
        /// 参照かどうか
        /// </summary>
        public bool IsReference
        {
            get
            {
                return Reference != "";
            }
        }

        /// <summary>
        /// 型のバイトサイズ(非ポインタのユーザ定義型を0、ポインタを-1、long doubleを-2、wchar_tを-3とする)
        /// </summary>
        public int ByteSize
        {
            get
            {
                int result = 0;
                // ポインタかどうか
                if (IsPointer || IsReference)
                {
                    result = -1;
                }
                // 非ユーザ定義型
                else if (!IsUserDefinedType)
                {
                    string typeText = (string)Type;
                    int longCount = Modifiers.FindAll((mod) => mod == "long").Count;
                    if ((typeText == "") || (typeText == "int"))
                    {
                        if (Modifiers.Contains("short"))
                        {
                            result = 2;
                        }
                        else if (longCount >= 2)
                        {
                            result = 8;
                        }
                        else
                        {
                            result = 4;
                        }
                    }
                    else if (typeText == "double")
                    {
                        if (longCount >= 1)
                        {
                            result = -2;
                        }
                        else
                        {
                            result = 8;
                        }
                    }
                    else if (typeText == "wchar_t")
                    {
                        result = -3;
                    }
                    else
                    {
                        if ((typeText == "char") || (typeText == "bool"))
                        {
                            result = 1;
                        }
                        else if (typeText == "char16_t")
                        {
                            result = 2;
                        }
                        else if ((typeText == "char32_t") || (typeText == "float"))
                        {
                            result = 4;
                        }
                    }
                }
                return result;
            }
        }

        #endregion

        #region コンストラクタ
        /// <summary>
        /// 変数型
        /// </summary>
        /// <param name="type">型情報</param>
        /// <param name="preModifiers">型の前で記述された修飾子</param>
        /// <param name="postModifiers">型の後で記述された修飾子</param>
        /// <param name="pointers">ポインタ</param>
        /// <param name="reference">参照テキスト</param>
        /// <param name="referenceConst">参照の後ろのconst</param>
        public VariableType(object type, IEnumerable<string> preModifiers, IEnumerable<string> postModifiers, IEnumerable<Pointer> pointers, string reference, string referenceConst)
        {
            Type = type;
            if (Type is UserDefinedType)
                IsUserDefinedType = true;
            Modifiers = new List<string>(preModifiers);
            Modifiers.AddRange(postModifiers);
            Pointers = new List<Pointer>(pointers);
            Reference = reference;
            if ((IsReference) && (referenceConst != ""))
            {
                Modifiers.Add("const");
            }
        }

        #endregion

        #region メソッド
        /// <summary>
        /// 型が一致しているかどうか
        /// </summary>
        /// <param name="obj">比較対象</param>
        /// <returns>一致しているかどうか</returns>
        public bool CompareType(VariableType obj)
        {
            // 修飾子で一致しないものが無いかチェック
            if ((IsConst != obj.IsConst) || (IsVolatile != obj.IsVolatile) || (IsReference != obj.IsReference))
            {
                return false;
            }
            // 型情報の一致チェック
            if (IsUserDefinedType && obj.IsUserDefinedType)
            {
                var udt1 = (UserDefinedType)Type;
                var udt2 = (UserDefinedType)obj.Type;
                if (udt1.Name != udt2.Name)
                {
                    return false;
                }
            }
            else if (!IsUserDefinedType && !obj.IsUserDefinedType)
            {
                if (ByteSize != obj.ByteSize)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            // ポインタチェック
            if (Pointers.Count == obj.Pointers.Count)
            {
                int n = Pointers.Count;
                for (int i = 0; i < n; ++i)
                {
                    if (Pointers[i].IsConst != obj.Pointers[i].IsConst)
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 文字列を返す
        /// </summary>
        /// <returns>文字列</returns>
        public override string ToString()
        {
            string result = string.Join(" ", Modifiers);
            if (result != "") result += " ";
            result += Type.ToString();

            int pcount = Pointers.Count;
            if (pcount > 0) result += " ";
            for (int i = 0; i < pcount; ++i)
            {
                Pointer pointer = Pointers[i];
                result += Pointers[i];
                if ((pointer.IsConst) && (i < (pcount - 1)))
                    result += " ";
            }
            if (IsReference)
            {
                result += (" " + Reference);
            }
            return result;
        }

        #endregion
    }

    /// <summary>
    /// ポインタ
    /// </summary>
    [Serializable]
    class Pointer
    {
        #region プロパティ
        /// <summary>
        /// constであるかどうか
        /// </summary>
        public bool IsConst { get; protected set; }

        #endregion

        #region コンストラクタ
        /// <summary>
        /// ポインタ
        /// </summary>
        /// <param name="isConst">constであるかどうか</param>
        public Pointer(bool isConst)
        {
            IsConst = isConst;
        }

        #endregion

        #region メソッド
        /// <summary>
        /// 文字列を返す
        /// </summary>
        /// <returns>文字列</returns>
        public override string ToString()
        {
            string result = "*";
            if (IsConst) result += " const";
            return result;
        }

        #endregion

    }

    /// <summary>
    /// ユーザ定義型
    /// </summary>
    [Serializable]
    class UserDefinedType
    {
        #region プロパティ
        /// <summary>
        /// 名前
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// enumkey
        /// </summary>
        public string EnumKey { get; protected set; }

        /// <summary>
        /// classkey
        /// </summary>
        public string ClassKey { get; protected set; }

        #endregion

        #region コンストラクタ
        /// <summary>
        /// ユーザ定義型
        /// </summary>
        /// <param name="name">型名</param>
        /// <param name="enumkey">enumkey</param>
        /// <param name="classkey">classkey</param>
        public UserDefinedType(string name, string enumkey, string classkey)
        {
            Name = name;
            EnumKey = enumkey;
            ClassKey = classkey;
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
            if (EnumKey != "") result += (EnumKey + " ");
            if (ClassKey != "") result += (ClassKey + " ");
            result += Name;
            return result;
        }

        #endregion
    }

}
