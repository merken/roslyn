using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace welfare.generation
{
    public static class RoslynExtensions
    {
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
            var assembly = type.GetTypeInfo().Assembly.Location;
            
            Console.WriteLine("Adding Reference:" + assembly);
            return compilation.AddReferences(MetadataReference.CreateFromFile(assembly));
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

                Console.Error.WriteLine(compilation.SyntaxTrees[0].ToString());

                throw new NotSupportedException($"Compilation did not succeed : {failures.Count()} errors.");
            }
            Console.WriteLine("Compilation succeeded");
            return Assembly.Load(ms.ToArray());
        }
    }
}