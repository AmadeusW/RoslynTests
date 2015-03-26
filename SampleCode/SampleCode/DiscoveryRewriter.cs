namespace SampleCode
{
    using Microsoft.CodeAnalysis.CSharp;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal class DiscoveryRewriter : CSharpSyntaxRewriter
    {
        Stack<string> _parentElements;

        public DiscoveryRewriter()
        {
            _parentElements = new Stack<string>();
        }

        public override SyntaxNode VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            string namespaceName = node.Name.ToString();
            _parentElements.Push(namespaceName);
            var visitedNode = base.VisitNamespaceDeclaration(node);
            _parentElements.Pop();
            return visitedNode;
        }

        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            string className = node.Identifier.ToString();
            _parentElements.Push(className);
            var visitedNode = base.VisitClassDeclaration(node);
            _parentElements.Pop();
            return visitedNode;
        }

        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            var method = (MethodDeclarationSyntax)base.VisitMethodDeclaration(node);
            StringBuilder sb = new StringBuilder(8);

            // Store method's location
            string location = String.Join(".", _parentElements.Reverse());
            sb.Append("My name is ");
            sb.Append(method.Identifier.ToString());
            sb.Append(" and I live in ");
            sb.Append(location);
            sb.Append('.');

            var trackingData = sb.ToString();

            // Add tracking code
            var bodyBlock = method.Body as BlockSyntax;
            if (bodyBlock == null)
            {
                bodyBlock = SyntaxFactory.Block();
            }
            SyntaxList<StatementSyntax> newStatements = new SyntaxList<StatementSyntax>();
            newStatements = newStatements.Add(buildTrackingInvocation(trackingData));
            newStatements = newStatements.AddRange(bodyBlock.Statements);
            bodyBlock = bodyBlock.WithStatements(newStatements);
            return method.WithBody(bodyBlock);
        }

        private StatementSyntax buildTrackingInvocation(string data)
        {
            var newNode = SyntaxFactory.ExpressionStatement(
                            SyntaxFactory.InvocationExpression(
                                SyntaxFactory.MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                        SyntaxFactory.IdentifierName(
                                            "System.Console"),
                                        SyntaxFactory.IdentifierName(
                                            "WriteLine")))
                                .WithArgumentList(
                                    SyntaxFactory.ArgumentList(
                                        SyntaxFactory.SingletonSeparatedList<ArgumentSyntax>(
                                            SyntaxFactory.Argument(
                                                SyntaxFactory.LiteralExpression(
                                                    SyntaxKind.StringLiteralExpression,
                                                    SyntaxFactory.Literal(
                                                        data
                                                        )))))))
            ;//.WithTrailingTrivia(SyntaxFactory.TriviaList().Add(SyntaxFactory.SyntaxTrivia(SyntaxKind.EndOfLineTrivia, "\n")));
            return newNode;
        }
    }
}