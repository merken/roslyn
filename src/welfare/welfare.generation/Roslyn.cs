using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using System;
using System.Reflection;
using welfare.core;

namespace welfare.generation
{
    public class CompiledWelfareService : ICompiledWelfareService
    {
        private CSharpCompilation compilation;
        private Assembly assembly;
        private string namespaceType;
        
        public CompiledWelfareService(CompilationUnitSyntax compilationUnit, string namespaceType)
        {
            this.namespaceType = namespaceType;
            compilation = CSharpCompilation.Create(Guid.NewGuid().ToString() + ".dll")
                .WithOptions(new CSharpCompilationOptions(Microsoft.CodeAnalysis.OutputKind.DynamicallyLinkedLibrary))
                .AddCoreReference()
                .AddReferenceFromType(typeof(IWelfareService))
                .AddSyntaxTrees(compilationUnit.SyntaxTree);
            assembly = compilation.BuildAssembly();
        }

        public IWelfareService GetNewInstance()
        {
            var welfareService = assembly.GetType($"{namespaceType}.WelfareService");
            return (IWelfareService)Activator.CreateInstance(welfareService);
        }

        public void SaveToFile(string filename)
        {

        }
    }

    public class WelfareServiceBuilder : IWelfareServiceBuilder
    {
        private CompilationUnitSyntax compilationUnit;
        private IList<UsingDirectiveSyntax> usings;
        private NamespaceDeclarationSyntax namespaceDeclarationSyntax;
        private IList<WelfareRule> welfareRules;
        private readonly string serviceName = "WelfareService";
        private string namespaceType;

        public WelfareServiceBuilder()
        {
            usings = new List<UsingDirectiveSyntax>();
            welfareRules = new List<WelfareRule>();
        }
        public IWelfareServiceBuilder AddNamespace(string name)
        {
            namespaceType = name;
            namespaceDeclarationSyntax = NamespaceDeclaration(IdentifierName(name));
            return this;
        }

        public IWelfareServiceBuilder AddUsing(string name)
        {
            var usingDirective = UsingDirective(IdentifierName(name));
            this.usings.Add(usingDirective);
            return this;
        }

        public IWelfareServiceBuilder AddWelfareRule(WelfareRule rule)
        {
            welfareRules.Add(rule);
            return this;
        }

        public ICompiledWelfareService Build()
        {
            var compilationUnit = CompilationUnit()
                .WithUsings(List<UsingDirectiveSyntax>(usings.ToArray()));

            var classDeclaration = ClassDeclaration($"{serviceName}")
                .WithModifiers(
                    TokenList(
                        Token(SyntaxKind.PublicKeyword)))
                .WithBaseList(
                    BaseList(
                        SingletonSeparatedList<BaseTypeSyntax>(
                            SimpleBaseType(
                                IdentifierName($"{nameof(IWelfareService)}")))));

            classDeclaration = classDeclaration.WithMembers(SingletonList<MemberDeclarationSyntax>(GenerateWelfareRules(welfareRules)));

            namespaceDeclarationSyntax = namespaceDeclarationSyntax.WithMembers(SingletonList<MemberDeclarationSyntax>(classDeclaration));

            compilationUnit = compilationUnit.WithMembers(SingletonList<MemberDeclarationSyntax>(namespaceDeclarationSyntax));

            return new CompiledWelfareService(compilationUnit.NormalizeWhitespace(), namespaceType);
        }

        private MemberDeclarationSyntax GenerateWelfareRules(IList<WelfareRule> rules)
        {
            return MethodDeclaration(
                    PredefinedType(
                        Token(SyntaxKind.DecimalKeyword)),
                    Identifier("GetWelfare"))
                .WithModifiers(
                    TokenList(
                        Token(SyntaxKind.PublicKeyword)))
                .WithParameterList(GetParameterList())
                .WithBody(GenerateMethodBody(rules).AddStatements(GenerateNotSupportedException()));
        }

        private BlockSyntax GenerateMethodBody(IList<WelfareRule> rules)
        {
            var block = Block(rules.Select(r => GenerateIfStatement(r)).ToArray());
            return block;
        }

        private ThrowStatementSyntax GenerateNotSupportedException()
        {
            return ThrowStatement(
                    ObjectCreationExpression(
                        IdentifierName("NotSupportedException"))
                    .WithArgumentList(
                        ArgumentList(
                            SingletonSeparatedList<ArgumentSyntax>(
                                Argument(
                                    LiteralExpression(
                                        SyntaxKind.StringLiteralExpression,
                                        Literal("Business Rule Not Implemented")))))));
        }

        private ParameterListSyntax GetParameterList()
        {
            return ParameterList(
                        SeparatedList<ParameterSyntax>(
                            new SyntaxNodeOrToken[]{
                                Parameter(
                                    Identifier("monthsOfUnemployment"))
                                .WithType(
                                    PredefinedType(
                                        Token(SyntaxKind.IntKeyword))),
                                Token(SyntaxKind.CommaToken),
                                Parameter(
                                    Identifier("maritalStatus"))
                                .WithType(
                                    PredefinedType(
                                        Token(SyntaxKind.IntKeyword))),
                                Token(SyntaxKind.CommaToken),
                                Parameter(
                                    Identifier("handicap"))
                                .WithType(
                                    PredefinedType(
                                        Token(SyntaxKind.IntKeyword))),
                                Token(SyntaxKind.CommaToken),
                                Parameter(
                                    Identifier("numberOfChildren"))
                                .WithType(
                                    PredefinedType(
                                        Token(SyntaxKind.IntKeyword))),
                                Token(SyntaxKind.CommaToken),
                                Parameter(
                                    Identifier("brutowage"))
                                .WithType(
                                    PredefinedType(
                                        Token(SyntaxKind.DecimalKeyword))),
                                Token(SyntaxKind.CommaToken),
                                Parameter(
                                    Identifier("age"))
                                .WithType(
                                    PredefinedType(
                                        Token(SyntaxKind.IntKeyword)))}));
        }

        private IfStatementSyntax GenerateIfStatement(WelfareRule rule)
        {
            return IfStatement(
                BinaryExpression(
                    SyntaxKind.LogicalAndExpression,
                    BinaryExpression(
                        SyntaxKind.LogicalAndExpression,
                        BinaryExpression(
                            SyntaxKind.LogicalAndExpression,
                            BinaryExpression(
                                SyntaxKind.LogicalAndExpression,
                                BinaryExpression(
                                    SyntaxKind.LogicalAndExpression,
                                    BinaryExpression(
                                        SyntaxKind.LogicalAndExpression,
                                        BinaryExpression(
                                            SyntaxKind.EqualsExpression,
                                            IdentifierName("monthsOfUnemployment"),
                                            LiteralExpression(
                                                SyntaxKind.NumericLiteralExpression,
                                                Literal(rule.Month))),
                                        BinaryExpression(
                                            SyntaxKind.EqualsExpression,
                                            IdentifierName("maritalStatus"),
                                            LiteralExpression(
                                                SyntaxKind.NumericLiteralExpression,
                                                Literal(rule.IdMaritalStatus)))),
                                    BinaryExpression(
                                        SyntaxKind.EqualsExpression,
                                        IdentifierName("handicap"),
                                        LiteralExpression(
                                            SyntaxKind.NumericLiteralExpression,
                                            Literal(rule.IdHandicap)))),
                                BinaryExpression(
                                    SyntaxKind.EqualsExpression,
                                    IdentifierName("numberOfChildren"),
                                    LiteralExpression(
                                        SyntaxKind.NumericLiteralExpression,
                                        Literal(rule.NumberOfChildren)))),
                            BinaryExpression(
                                SyntaxKind.LessThanExpression,
                                LiteralExpression(
                                    SyntaxKind.NumericLiteralExpression,
                                    Literal(rule.BrutoStart)),
                                IdentifierName("brutowage"))),
                        BinaryExpression(
                            SyntaxKind.LessThanOrEqualExpression,
                            IdentifierName("brutowage"),
                            LiteralExpression(
                                SyntaxKind.NumericLiteralExpression,
                                Literal(rule.BrutoEnd)))),
                    BinaryExpression(
                        SyntaxKind.LessThanOrEqualExpression,
                        LiteralExpression(
                            SyntaxKind.NumericLiteralExpression,
                            Literal(rule.Age)),
                        IdentifierName("age"))),
                ReturnStatement(
                    LiteralExpression(
                        SyntaxKind.NumericLiteralExpression,
                        Literal(rule.Value))));
        }
    }
}