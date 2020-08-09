using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sprache;
using LayerCompiler.Parsers;
using System.CodeDom;

namespace LayerCompiler
{
    class Jikken
    {
        public static void Test()
        {
            string src =
@"
baselayer
{
    base class Aho
    {
        int a[10] = {0};
        int b = 0;
        base void m1() const;
        void* m2(int a, int b = (1 + 1));
    };
    namespace ABC
    {
        base class Baka : public Aho
        {
        };
    }
    void Aho::m1() const
    {
        int a = 0;
        ++a;
        for (int i = 0; i < 10; ++i)
            a += i;
        return;
    }
}
";
            //var text = IgnoreParser.TokenOrDirective.TokenWithSkipComment().Many().Parse(src);
            var text = RTCOPParser.BaseLayerDefinition.TokenWithSkipComment().Many().Parse(src);
            //var text = TokenParser.Token.TokenWithSkipComment().Many().Parse(src);
            //var text = CppTokenParser.Keyword.Token().Many().Parse(src);

            foreach (var t in text) Console.WriteLine(t);
            //Console.WriteLine(text);
        }
    }
}
