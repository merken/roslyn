using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace moksy.roslyn
{
    static class RoslynExtensions
    {
        //public static MethodDeclarationSyntax AddTypedParametersIfAny(this MethodDeclarationSyntax target, MethodInfo source)
        //{
        //    if (source?.TypeParameterList?.Parameters != null)
        //    {
        //        target = target.AddTypeParameterListParameters(source.TypeParameterList.Parameters.ToArray());
        //    }
        //    return target;
        //}

        public static MethodDeclarationSyntax AddParametersIfAny(this MethodDeclarationSyntax target, MethodInfo source)
        {
            var parameters = source.GetParameters();
            if (parameters.Any())
            {
                target = target.AddParameterListParameters(ToParameterSyntaxArray(parameters));
            }
            return target;
        }

        private static ParameterSyntax[] ToParameterSyntaxArray(ParameterInfo[] parameters)
        {
            return parameters.Select(p => SyntaxFactory.Parameter(SyntaxFactory.Identifier(p.Name))
                    .WithType(SyntaxFactory.ParseTypeName(p.ParameterType.Name)))
                .ToArray();
        }

        public static TypeSyntax GetTypeSyntax(this Type type)
        {
            string typeName = type.Name;
            if (typeName.ToLower() == "void")
                typeName = "void";

            return SyntaxFactory.ParseTypeName(typeName);
        }

        public static CSharpCompilation AddCoreReference(this CSharpCompilation compilation)
        {
            var mscorlib = typeof(object).GetTypeInfo().Assembly.Location;
            var coreDir = Directory.GetParent(mscorlib);

            return compilation.AddReferences(
                    MetadataReference.CreateFromFile(coreDir.FullName + Path.DirectorySeparatorChar + "mscorlib.dll"),
                    MetadataReference.CreateFromFile(coreDir.FullName + Path.DirectorySeparatorChar + "System.dll"),
                    MetadataReference.CreateFromFile(coreDir.FullName + Path.DirectorySeparatorChar + "System.Runtime.dll"));
        }

        public static CSharpCompilation AddReferenceFromType(this CSharpCompilation compilation, Type type)
        {
            return compilation.AddReferences(
                       MetadataReference.CreateFromFile(type.GetTypeInfo().Assembly.Location));

        }

        // public static SyntaxTree FormatTree(this CompilationUnitSyntax compilation)
        // {
        //     var workspace = new AdhocWorkspace();
        //     var options = workspace.Options.WithChangedOption(CSharpFormattingOptions.NewLinesForBracesInMethods, true)
        //                         .WithChangedOption(CSharpFormattingOptions.NewLinesForBracesInTypes, true);

        //     SyntaxNode formattedNode = Formatter.Format(compilation, workspace, options);
        //     var formattedTree = SyntaxFactory.SyntaxTree(formattedNode, CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp7));

        //     return formattedTree;
        // }

        public static Assembly BuildAssembly(this CSharpCompilation compilation)
        {
            //Emit to stream
            var ms = new MemoryStream();
            var emitResult = compilation.Emit(ms);

            if (!emitResult.Success)
                throw new NotSupportedException($"Compilation did not succeed");

            return Assembly.Load(ms.ToArray());
        }
    }
}
