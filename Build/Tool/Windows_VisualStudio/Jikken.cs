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
//#define Aho(aa, bb) aa + bb
baselayer
{
    base class Aho
    {
        int a[10] = {0};
        int b = 0;
        base void m1() const;
        void* m2(int a, int b = (1 + 1)) { return 0; }
    };
    namespace ABC
    {
        base class Baka : public Aho
        {
            volatile const int*** const c;
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
            //var text = PreprocessParser.DirectiveOrLine.TokenWithSkipCommentForPreprocessParser().Many().Parse(src);
            //var text = RTCOPParser.BaseLayerDefinition.TokenWithSkipComment().Many().Parse(src);
            //var text = TokenParser.Token.TokenWithSkipComment().Many().Parse(src);
            //var text = PreprocessParser.Directive.TokenWithSkipCommentForPreprocessParser().Many().Parse(src);

            var ex = PreprocessParser.IfDirectiveExpression.TokenWithSkipComment().Parse("10 > 3 && false");
            var result = ex.Evaluate();
            
            //foreach (var t in text) Console.WriteLine(t);
            //Console.WriteLine(text);
        }
    }
}
