﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.CSharp.Extensions;


namespace Asmx2WebApiFixer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(Asmx2WebApiFixerCodeFixProvider)), Shared]
    public class Asmx2WebApiFixerCodeFixProvider : CodeFixProvider
    {
        private const string title = "Generate WebApi Controller";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(Asmx2WebApiFixerAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            // TODO: Replace the following code with your own analysis, generating a CodeAction for each fix to suggest
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().First();

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: title,
                    createChangedSolution: c => CreateWebApi(context.Document, declaration, c),
                    equivalenceKey: title),
                diagnostic);
        }

        private async Task<Solution> CreateWebApi(Document document, ClassDeclarationSyntax classDeclaration, CancellationToken cancellationToken)
        {
            var webserviceName = classDeclaration.Identifier.Text;
            webserviceName = webserviceName.Replace("Service", String.Empty);
            webserviceName = webserviceName.Replace("service", String.Empty);
            webserviceName = $"{webserviceName}Controller";

            var classUsings = GetUsingsFromClass(classDeclaration);
            var classNamespace = GetNamespaceFromClass(classDeclaration);
            var webmethods = GetWebMethodsFrom(classDeclaration);
            var constructors = GetConstructorsFrom(classDeclaration);
            var othermembers = GetMembersFrom(classDeclaration).Except(webmethods).Except(constructors);

            //var webApiController = GenerateWebApiFrom(webmethods, othermethods);

            //TODO get name of webservice
            //TODO create api controller
            //TODO add import statements
            //TODO remove attributes
            //TODO add routes based on webmethod names

            var newFileTree = SyntaxFactory.CompilationUnit()
                .WithUsings(SyntaxFactory.List<UsingDirectiveSyntax>(classUsings.Select(i => (UsingDirectiveSyntax)i)))
                .WithMembers(
                            SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                                SyntaxFactory.NamespaceDeclaration(
                                    SyntaxFactory.IdentifierName(classNamespace.Name.ToString()))
                                    .AddMembers(SyntaxFactory.ClassDeclaration($"{webserviceName}")
                                        .AddMembers(constructors.Select(c =>
                                            c.WithIdentifier(SyntaxFactory.Identifier($"{webserviceName}"))).ToArray())
                                        .AddMembers(othermembers.ToArray()))))
                .WithoutLeadingTrivia()
                .NormalizeWhitespace();

            var newDocument = document.Project.AddDocument($"{webserviceName}.cs", SourceText.From(newFileTree.ToFullString()), document.Folders);
            return newDocument.Project.Solution;
        }

        private IList<UsingDirectiveSyntax> GetUsingsFromClass(ClassDeclarationSyntax @class)
        {
            return ((Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax)((Microsoft.CodeAnalysis.SyntaxNode)(@class.Parent)).Parent).Usings.ToList();
        }

        private NamespaceDeclarationSyntax GetNamespaceFromClass(ClassDeclarationSyntax @class)
        {
            return (@class.Parent as NamespaceDeclarationSyntax);
        }

        private IList<MethodDeclarationSyntax> GetWebMethodsFrom(ClassDeclarationSyntax webservice)
        {
            var methodDeclarations = webservice.Members.Where(m => m is MethodDeclarationSyntax).Cast<MethodDeclarationSyntax>();
            return methodDeclarations.Where(m => m.AttributeLists.ContainsAttributeInList("WebMethod")).ToList();
        }

        private IList<ConstructorDeclarationSyntax> GetConstructorsFrom(ClassDeclarationSyntax webservice)
        {
            var constructors = webservice.Members.Where(m => m is ConstructorDeclarationSyntax).Cast<ConstructorDeclarationSyntax>();
            return constructors.ToList();
        }

        private IList<MemberDeclarationSyntax> GetMembersFrom(ClassDeclarationSyntax webservice)
        {
            return webservice.Members.ToList();
        }

        private bool IsHttpPost(ClassDeclarationSyntax classDeclaration, MethodDeclarationSyntax method)
        {
            var isPostConfigured = classDeclaration.AttributeLists.ContainsAttributeInList("ScriptService");

            if (!isPostConfigured)
                isPostConfigured = method.AttributeLists.ContainsAttributeInList("ScriptMethod");

            return isPostConfigured;
        }
    }
}