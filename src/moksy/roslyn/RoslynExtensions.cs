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
                return SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword));

            return SyntaxFactory.ParseTypeName(typeName);
        }

        public static CSharpCompilation AddCoreReference(this CSharpCompilation compilation)
        {
            var trustedPlatformAssemblies = AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES");

            var libraries = ((String)trustedPlatformAssemblies).Split(Path.PathSeparator);
            foreach (var library in libraries)
            {
                Console.WriteLine("Adding Reference:" + library);
                compilation = compilation.AddReferences(MetadataReference.CreateFromFile(library));
            }

            return compilation;
        }

        public static CSharpCompilation AddReferenceFromType(this CSharpCompilation compilation, Type type)
        {
            return compilation.AddReferences(
                       MetadataReference.CreateFromFile(type.GetTypeInfo().Assembly.Location));

        }

        public static Assembly BuildAssembly(this CSharpCompilation compilation)
        {
            //Emit to stream
            var ms = new MemoryStream();
            var emitResult = compilation.Emit(ms);

            if (!emitResult.Success)
            {
                IEnumerable<Diagnostic> failures = emitResult.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);

                foreach (Diagnostic diagnostic in failures)
                {
                    Console.Error.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                }

                throw new NotSupportedException($"Compilation did not succeed : {failures.Count()} errors.");
            }

            return Assembly.Load(ms.ToArray());
        }
    }
}
