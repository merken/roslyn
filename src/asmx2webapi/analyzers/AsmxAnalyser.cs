using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace FirstAnalyzerCS
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AsmxAnalyzerCSAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "Asmx2WebApi";
        private const string Title = "ASMX is an outdated technology, please migrate to WEBAPI";
        private const string MessageFormat = "Can be migrated to WEBAPI";
        private const string Description = "Migrate to WEBAPI";
        private const string Category = "Usage";

        private static string[] MigrationCandidates = new String[] { "System.Web.Script.Services.ScriptService", "WebService" };

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.ClassDeclaration);
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var classDeclaration = (context.Node as ClassDeclarationSyntax);

            if (classDeclaration != null && classDeclaration.AttributeLists.Any() &&
                (classDeclaration.AttributeLists.First().Contains(SyntaxFactory.Attribute(SyntaxFactory.ParseName("System.Web.Script.Services.ScriptService"))) ||
                classDeclaration.AttributeLists.First().Contains(SyntaxFactory.Attribute(SyntaxFactory.ParseName("WebService")))))

            {
                //This is a script service
                context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation()));
            }
            return;
        }
    }
}