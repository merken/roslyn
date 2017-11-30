using System;
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
using System.Net.Http;
using System.Text;

namespace Asmx2WebApiFixer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(Asmx2WebApiFixerCodeFixProvider)), Shared]
    public class Asmx2WebApiFixerCodeFixProvider : CodeFixProvider
    {
        private const string title = "Generate WebApi Controller";
        private string[] webApiUsings = new String[] { "System.Net.Http", "System.Web.Http" };

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
            var semanticModel = await document.GetSemanticModelAsync();
            var webserviceName = classDeclaration.Identifier.Text;
            webserviceName = webserviceName.Replace("WebService", String.Empty);
            webserviceName = webserviceName.Replace("webservice", String.Empty);
            webserviceName = webserviceName.Replace("Service", String.Empty);
            var controllerName = webserviceName.Replace("service", String.Empty);

            var controllerClass = GetWebApiControllerClassDeclaration(controllerName);
            var classUsings = GetUsingsFromClass(classDeclaration);
            var additionalUsings = webApiUsings.Select(u => SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(u)));
            var classNamespace = GetNamespaceFromClass(classDeclaration);
            var webmethods = GetWebMethodsFrom(classDeclaration);
            var constructors = GetConstructorsFrom(classDeclaration);
            var fields = GetFieldsFrom(classDeclaration);
            var othermembers = GetMembersFrom(classDeclaration).Except(webmethods).Except(constructors).Except(fields);
            var controllerMethods = GetApiControllerMethods(semanticModel, webmethods);

            var newFileTree = SyntaxFactory.CompilationUnit()
                .WithUsings(SyntaxFactory.List<UsingDirectiveSyntax>(classUsings.Concat(additionalUsings)))
                .WithMembers(
                            SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                                SyntaxFactory.NamespaceDeclaration(
                                    SyntaxFactory.IdentifierName(classNamespace.Name.ToString()))
                                    .AddMembers(controllerClass
                                        .AddMembers(fields.ToArray())
                                        .AddMembers(constructors.Select(c =>
                                            c.WithIdentifier(SyntaxFactory.Identifier($"{controllerName}Controller"))).ToArray())
                                        .AddMembers(controllerMethods.ToArray())
                                        .AddMembers(othermembers.ToArray()))))
                .WithoutLeadingTrivia()
                .NormalizeWhitespace();

            var newDocument = document.Project.AddDocument($"{webserviceName}Controller.cs", SourceText.From(newFileTree.ToFullString()), document.Folders);
            return newDocument.Project.Solution;
        }

        private ClassDeclarationSyntax GetWebApiControllerClassDeclaration(string controllerName)
        {
            var classDeclaration = SyntaxFactory.ClassDeclaration($"{controllerName}Controller")
                                        .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                                        .WithBaseList(
                                            SyntaxFactory.BaseList().WithTypes(SyntaxFactory.SingletonSeparatedList<BaseTypeSyntax>(
                                                    SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseName("ApiController")))));

            var routePrefixAttribute = SyntaxFactory.Attribute(SyntaxFactory.ParseName("RoutePrefix"))
                .WithArgumentList(
                    SyntaxFactory.AttributeArgumentList(
                        SyntaxFactory.SingletonSeparatedList<AttributeArgumentSyntax>(
                            SyntaxFactory.AttributeArgument(
                                SyntaxFactory.LiteralExpression(
                                    SyntaxKind.StringLiteralExpression,
                                    SyntaxFactory.Literal($"api/{controllerName}"))))));

            return classDeclaration.WithAttributeLists(
                SyntaxFactory.SingletonList<AttributeListSyntax>(
                    SyntaxFactory.AttributeList(
                        SyntaxFactory.SingletonSeparatedList<AttributeSyntax>(routePrefixAttribute))));
        }

        private IList<MethodDeclarationSyntax> GetApiControllerMethods(SemanticModel semanticModel, IList<MethodDeclarationSyntax> webmethods)
        {
            var apiMethods = new List<MethodDeclarationSyntax>();

            foreach (var webmethod in webmethods)
            {
                bool isHttpPost = false;
                var parameters = GetMethodParameters(webmethod);
                var routeName = webmethod.Identifier.Text;
                var isHttpGetMethod = routeName.ToLower().StartsWith("get");
                var complexParameters = parameters.Where(p => IsComplexType(semanticModel.GetTypeInfo(p.Type).Type));
                var simpleParameters = parameters.Except(complexParameters);
                var routeAttribute = GetRouteAttributeForWebmethod(routeName, semanticModel, simpleParameters);
                AttributeSyntax methodAttribute;

                if (complexParameters.Any())
                    isHttpPost = true;

                if (!isHttpPost && !isHttpGetMethod)
                    isHttpPost = false;

                methodAttribute = GenerateAttribute(isHttpPost ? "HttpPost" : "HttpGet");

                var apiMethod = webmethod.WithAttributeLists(
                                SyntaxFactory.List<AttributeListSyntax>(
                                    new AttributeListSyntax[]{
                                        SyntaxFactory.AttributeList(
                                            SyntaxFactory.SingletonSeparatedList<AttributeSyntax>(
                                                routeAttribute
                                            )),
                                        SyntaxFactory.AttributeList(
                                            SyntaxFactory.SingletonSeparatedList<AttributeSyntax>(
                                                methodAttribute
                                            ))
                                    }));

                var apiParameters = new SeparatedSyntaxList<ParameterSyntax>().AddRange(parameters.Select(
                    p =>
                    {
                        var isComplex = IsComplexType(semanticModel.GetTypeInfo(p.Type).Type);

                        if (isComplex)
                        {
                            return p.WithAttributeLists(
                                                SyntaxFactory.SingletonList<AttributeListSyntax>(
                                                    SyntaxFactory.AttributeList(
                                                        SyntaxFactory.SingletonSeparatedList<AttributeSyntax>(
                                                            SyntaxFactory.Attribute(
                                                                SyntaxFactory.IdentifierName("FromBody"))))));
                        }

                        return p;
                    }));

                apiMethod = apiMethod.WithParameterList(
                    apiMethod.ParameterList.WithParameters(
                        apiParameters));

                apiMethods.Add(apiMethod);
            }
            return apiMethods;
        }

        private AttributeSyntax GetRouteAttributeForWebmethod(string routeName, SemanticModel semanticModel, IEnumerable<ParameterSyntax> routeParams)
        {
            var parameters = new List<String>();

            foreach (var routeParam in routeParams)
            {
                var paramName = routeParam.Identifier.Text;
                parameters.Add($"{{{paramName}}}");
            }

            var route = $"{routeName}";
            if (routeParams.Any())
                route = $"{route}/{String.Join("/", parameters)}";

            return SyntaxFactory.Attribute(
                        SyntaxFactory.IdentifierName("Route"))
                    .WithArgumentList(
                        SyntaxFactory.AttributeArgumentList(
                            SyntaxFactory.SingletonSeparatedList<AttributeArgumentSyntax>(
                                SyntaxFactory.AttributeArgument(
                                    SyntaxFactory.LiteralExpression(
                                        SyntaxKind.StringLiteralExpression,
                                        SyntaxFactory.Literal($"{route}"))))));
        }

        private AttributeSyntax GenerateAttribute(string attribute)
        {
            return SyntaxFactory.Attribute(
                        SyntaxFactory.IdentifierName($"{attribute}"));
        }

        private bool IsComplexType(ITypeSymbol type)
        {
            var isValueType = type.IsValueType;
            var isString = type.Name.ToLower() == "string";
            return !isValueType && !isString;
        }

        private IList<ParameterSyntax> GetMethodParameters(MethodDeclarationSyntax method)
        {
            return method.ParameterList.Parameters.ToList();
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
            return webservice.Members.Where(m => m is ConstructorDeclarationSyntax).Cast<ConstructorDeclarationSyntax>().ToList();
        }

        private IList<FieldDeclarationSyntax> GetFieldsFrom(ClassDeclarationSyntax webservice)
        {
            return webservice.Members.Where(m => m is FieldDeclarationSyntax).Cast<FieldDeclarationSyntax>().ToList();
        }

        private IList<MemberDeclarationSyntax> GetMembersFrom(ClassDeclarationSyntax webservice)
        {
            return webservice.Members.ToList();
        }

        private bool IsScriptService(ClassDeclarationSyntax classDeclaration)
        {
            return classDeclaration.AttributeLists.ContainsAttributeInList("ScriptService");
        }

        private bool IsHttpPostForMethodDeclaration(MethodDeclarationSyntax method)
        {
            return method.AttributeLists.ContainsAttributeInList("ScriptMethod");
        }
    }
}