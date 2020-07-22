using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sprache;
using LayerCompiler.Parsers;
using System.CodeDom;
using LayerCompiler.Model;

namespace LayerCompiler
{
    class Jikken
    {
        public static void Test()
        {
            string src =
@"
#include <stdio.h>
namespace aaa 
{
   int a;
   class A { int m1() { } };
}
namespace{}
";
            var text = RTCOPParser.ToIgnoringTokenAndDirective.TokenWithSkipComment().Many().Parse(src);
            //var text = TokenParser.Token.TokenWithSkipComment().Many().Parse(src);
            //var text = CppTokenParser.Keyword.Token().Many().Parse(src);

            foreach (var t in text) Console.WriteLine(t);
            //Console.WriteLine(text);
        }
    }
}
