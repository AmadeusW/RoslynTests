using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleCode
{
    class Program
    {
        static void Main(string[] args)
        {
            Walk();
            Rewrite();
        }

        private static void Walk()
        {
            var tree = CSharpSyntaxTree.ParseText(@"
                class MyClass
                {
                    public void Method1()
                    {
                        var test = false;
                        Method2();
                    }
                    public void Method2()
                    {
                        Method1();
                    }
                }
            ");
            var walker = new CustomWalker();
            walker.Visit(tree.GetRoot());
        }

        private static void Rewrite()
        {
            var tree = CSharpSyntaxTree.ParseText(@"
                namespace Vancouver.Meetup
                {
                    class MyClass
                    {
                        public void Method1()
                        {
                            var test = false;
                            Method2();
                        }
                        public void Method2()
                        {
                            Method1();
                        }
                        class PrivateClass
                        {
                            private void PrivateMethod()
                            {
                                MyClass.Method1();
                            }
                        }
                    }
                }
            ");
            var rewriter = new DiscoveryRewriter();
            var rewrittenRoot = rewriter.Visit(tree.GetRoot());
            Console.WriteLine(rewrittenRoot.SyntaxTree);
        }
    }
}
