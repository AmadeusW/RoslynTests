using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.FindSymbols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SampleCode
{
    public class CustomWalker : CSharpSyntaxWalker
    {
        static int Tabs = 0;

        public CustomWalker() : base(SyntaxWalkerDepth.Token)
        { }

        public override void Visit(SyntaxNode node)
        {
            Tabs++;
            var indents = new String('\t', Tabs);
            Console.WriteLine(indents + node.Kind());
            base.Visit(node);
            Tabs--;
        }
        /*
        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            //base.VisitMethodDeclaration(node);
        }
        */
        /*
        // Uncomment to see what each token represents
        public override void VisitToken(SyntaxToken token)
        {
            var indents = new String('\t', Tabs);
            Console.WriteLine(indents + token);
            base.VisitToken(token);
        }
        */
    }
}
