using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.DependencyModel;

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
            return compilation.AddReferences(MetadataReference.CreateFromFile(assembly, MetadataReferenceProperties.Assembly));
        }

        public static Assembly BuildAssembly(this CSharpCompilation compilation, string filename = null)
        {
            Console.WriteLine("Compilation starting for tree");
            Console.Error.WriteLine(compilation.SyntaxTrees[0].ToString());

            //Emit to stream
            Stream stream = new MemoryStream();
            if (filename != null)
                stream = File.Create(filename);
            var emitResult = compilation.Emit(stream);

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
            Console.WriteLine("Compilation succeeded");
            
            if (filename != null)
            {
                filename = (stream as FileStream).Name;
                stream.Close();
                return Assembly.LoadFile(filename);
            }

            return Assembly.Load((stream as MemoryStream).ToArray());
        }
    }
}