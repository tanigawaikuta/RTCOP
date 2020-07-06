using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sprache;

using LayerCompiler.Parsers;
using LayerCompiler.Parsers.RTCOP;

namespace LayerCompiler
{
    class Jikken
    {
        public static void Test()
        {
            string src =
@"int aa = 111;";
            var text = CppTokenParser.Token.TokenWithSkipComment().Many().Parse(src); //IgnoreItemParser.AttributeSpecifier2.TokenWithSkipComment().Many().Parse(src);
            //var text = CppTokenParser.Keyword.Token().Many().Parse(src);
            foreach (var t in text) Console.WriteLine(t);
            //Console.WriteLine(text);
        }
    }
}
