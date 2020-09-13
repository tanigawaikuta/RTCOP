using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace LayerCompiler
{
    class Program
    {
        /// <summary>
        /// エントリポイント。
        /// </summary>
        /// <param name="args">コマンドライン引数。</param>
        static int Main(string[] args)
        {
            // 実験用
            //Jikken.Test();

            // コンパイル
            try
            {
                CompileManager compileManager = new CompileManager(args);
                compileManager.Compile();
            }
            catch (Exception ex)
            {
                // メッセージ
                Console.WriteLine("コンパイルエラーが発生しました。詳細は、errorlog.txtをご覧ください。");
                Console.WriteLine(ex.Message);
                // ログファイルのパスを設定
                Assembly myAssembly = Assembly.GetEntryAssembly();
                string dirName = Path.GetDirectoryName(myAssembly.Location);
                string filePath = dirName + "/" + "errorlog.txt";
                // ログファイルの作成
                using (FileStream fs = File.OpenWrite(filePath))
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    Exception ex2 = ex;
                    while (ex2 != null)
                    {
                        sw.WriteLine("====例外クラスの型====");
                        sw.WriteLine(ex2.GetType());
                        sw.WriteLine("====例外メッセージ====");
                        sw.WriteLine(ex2.Message);
                        sw.WriteLine("====スタックトレース====");
                        sw.WriteLine(ex2.StackTrace);
                        sw.WriteLine("");
                        ex2 = ex2.InnerException;
                    }
                }
                return 1;
            }
            return 0;
        }
    }
}
