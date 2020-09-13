using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sprache;
using LayerCompiler.Parsers;
using System.CodeDom;
using LayerCompiler.CodeGeneration.Model;
using LayerCompiler.CodeGeneration;

namespace LayerCompiler
{
    class Jikken
    {
        public static void Test()
        {
            string src =
@"
#define aaaa 2
#define bbbb -2
#define aho(aa, bb) aa##bb

#include <stdio.h>
int a = 0;

aho(base, layer)
{
    base class Aho : paka
    {
#if aaaa - bbbb
        void m1(){}
        void m2(){};
        void m3();
#endif
    };
}
";
            var text = RTCOPParser.BaseLayerDefinition.TokenWithSkipComment().Many().Parse(src);
            //var text = PreprocessParser.DirectiveOrLine.TokenWithSkipCommentForPreprocessParser().Many().Parse(src2);
            //var text = RTCOPParser.BaseLayerDefinition.TokenWithSkipComment().Many().Parse(src);
            //var text = TokenParser.Token.TokenWithSkipComment().Many().Parse(src);
            //var text = PreprocessParser.Directive.TokenWithSkipCommentForPreprocessParser().Many().Parse(src);

            //foreach (var t in text) Console.WriteLine(t);
            //Console.WriteLine(text);

            RTCOPSourceFile f = new RTCOPSourceFile("a.lcpp", src);
            RTCOPPreprocessor p = new RTCOPPreprocessor(new string[0], new List<string>(), Encoding.UTF8);
            var f2 = p.Run(f);
            Console.WriteLine(f2.Text);

            RTCOPCompiler c = new RTCOPCompiler();
            var of = c.Compile(f2);
            LayerStructureFile.SaveFile(@"C:\Users\Ikuta\Desktop\aaa.lobj", of);
            var of2 = LayerStructureFile.LoadFile(@"C:\Users\Ikuta\Desktop\aaa.lobj");
        }
    }
}
