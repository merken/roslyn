using moksy.core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace moksy.roslyn
{
    public class RoslynMoksy : IMoksy
    {
        public T GetMokFor<T>()
        {
            var typeName = GetTypeName<T>();
            var typeNamespace = GetTypeNamespace<T>();

            var tree = GetNewMokAsSyntaxTree<T>();
            var root = tree.GetRoot();
            var @class = GetClassFromRoot(root);

            if (@class == null)
                throw new NotSupportedException($"Type {typeName} is not supported");

            @class = AddOverridesToClass<T>(@class);
            @class = AddBaseTypeToClass<T>(@class);

            var usings = GetUsingsFromRoot(root);
            var @namespace = GetNamespaceFromRoot(root);
            var newCompilation = SyntaxFactory.CompilationUnit()
                .AddUsings(usings.Select(u => SyntaxFactory.UsingDirective(u.Name)).ToArray())
                .AddMembers(@namespace
                    .AddMembers(@class)).NormalizeWhitespace();

            // var formattedTree = newCompilation.FormatTree();
            var compilation = CreateCompilation<T>(newCompilation.SyntaxTree);

            if (File.Exists(@"moksy.dll"))
            {
                File.Delete(@"moksy.dll");
            }

            var stream = new FileStream(@"moksy.dll", FileMode.CreateNew);
            var emitResult = compilation.Emit(stream);
            stream.Close();

            var mokAssembly = compilation.BuildAssembly();
            var mokkedType = mokAssembly.GetType($"{typeNamespace}.{typeName}Mok");

            return (T)Activator.CreateInstance(mokkedType);
        }

        private static NamespaceDeclarationSyntax GetNamespaceFromRoot(SyntaxNode root)
        {
            var namespaceName = root.DescendantNodes().OfType<NamespaceDeclarationSyntax>().FirstOrDefault().Name;

            return SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName(namespaceName.ToFullString().Trim()));
        }

        private static List<UsingDirectiveSyntax> GetUsingsFromRoot(SyntaxNode root)
        {
            return root.DescendantNodes().OfType<UsingDirectiveSyntax>().ToList();
        }

        private ClassDeclarationSyntax AddOverridesToClass<T>(ClassDeclarationSyntax @class)
        {
            var type = typeof(T);
            var overrideCandidates = GetOverrideCandidates(type);
            foreach (var candidateMethod in overrideCandidates)
            {
                var methodReturnType = candidateMethod.ReturnType;
                var methodModifier = candidateMethod.IsPublic ? SyntaxKind.PublicKeyword : SyntaxKind.ProtectedKeyword;
                var methodName = candidateMethod.Name;
                var parameters = candidateMethod.GetParameters();
                var parametersList = GetParameterListFromParameterInfos(parameters);

                var methodNameForMock = MokExtensions.GetFullMethodName(candidateMethod);
                var executeMokked = GetExecuteMokkedStatement(methodNameForMock, parameters);
                var baseCall = GetBaseCallStatement(methodName, parameters);
                var methodDeclaration = SyntaxFactory.MethodDeclaration(methodReturnType.GetTypeSyntax(), methodName)
                    .AddModifiers(SyntaxFactory.Token(methodModifier))
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.OverrideKeyword))
                    .WithBody(SyntaxFactory.Block(executeMokked, baseCall))
                    .WithParameterList(parametersList);

                @class = @class.AddMembers(methodDeclaration);
            }

            return @class;
        }

        private static ParameterListSyntax GetParameterListFromParameterInfos(ParameterInfo[] parameters)
        {
            var parametersList = SyntaxFactory.ParameterList();
            parametersList = parametersList.AddParameters(parameters.Select(p => SyntaxFactory.Parameter(SyntaxFactory.Identifier(p.Name)).WithType(SyntaxFactory.ParseTypeName(p.ParameterType.Name))).ToArray());
            return parametersList;
        }

        private static StatementSyntax GetExecuteMokkedStatement(string methodNameForMock, params ParameterInfo[] methodParameters)
        {
            var builder = new StringBuilder();
            builder.Append("this.ExecuteMokked(");
            builder.Append($"\"{methodNameForMock}\"");

            if (methodParameters.Any())
                methodParameters.ToList().ForEach(p => builder.Append($",{p.Name}"));

            builder.Append(");");

            var executeMokked = SyntaxFactory.ParseStatement(builder.ToString());
            return executeMokked;
        }

        private static StatementSyntax GetBaseCallStatement(string methodName, params ParameterInfo[] methodParameters)
        {
            var builder = new StringBuilder();
            builder.Append($"if(callBase)base.{methodName}(");

            if (methodParameters.Any())
            {
                methodParameters.ToList().ForEach(p => builder.Append($"{p.Name},"));
                builder.Remove(builder.Length - 1, 1);
            }

            builder.Append(");");

            var baseCall = SyntaxFactory.ParseStatement(builder.ToString());
            return baseCall;
        }

        private static CSharpCompilation CreateCompilation<T>(SyntaxTree tree)
        {
            var compilationOptions = new CSharpCompilationOptions(Microsoft.CodeAnalysis.OutputKind.DynamicallyLinkedLibrary);
            var compilation = CSharpCompilation.Create(Guid.NewGuid().ToString() + ".dll")
                .WithOptions(new CSharpCompilationOptions(Microsoft.CodeAnalysis.OutputKind.DynamicallyLinkedLibrary).WithOptimizationLevel(OptimizationLevel.Debug))
                .AddCoreReference()
                .AddReferenceFromType(typeof(IMok))
                .AddReferenceFromType(typeof(T))
                .AddSyntaxTrees(tree);
            return compilation;
        }

        private ClassDeclarationSyntax AddBaseTypeToClass<T>(ClassDeclarationSyntax @class)
        {
            var typeName = GetTypeName<T>();
            return @class.WithBaseList(@class.BaseList.WithTypes(@class.BaseList.Types.Insert(0, (
                        SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseName(typeName))))));
        }

        private string GetTypeName<T>()
        {
            var type = typeof(T);
            return type.Name;
        }

        private string GetTypeNamespace<T>()
        {
            var type = typeof(T);
            return type.Namespace;
        }

        private SyntaxTree GetNewMokAsSyntaxTree<T>()
        {
            var type = typeof(T);
            var typeName = type.Name;
            var typeNamespace = type.Namespace;

            return TemplateMokGenerator.GenerateTemplateMok(typeNamespace, typeName);
        }

        private MethodInfo[] GetOverrideCandidates(Type type)
        {
            var blackList = new String[] { "ToString", "Equals", "GetHashCode" };

            return type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.IsVirtual && !blackList.Contains(m.Name)).ToArray();
        }

        private ClassDeclarationSyntax GetClassFromRoot(SyntaxNode root)
        {
            return root.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault(p =>
                                                                         p.Modifiers.Any(m => m.RawKind == (int)SyntaxKind.PublicKeyword) &&
                                                                         !p.Modifiers.Any(m => m.RawKind == (int)SyntaxKind.PartialKeyword) &&
                                                                         !p.Modifiers.Any(m => m.RawKind == (int)SyntaxKind.StaticKeyword) &&
                                                                         //Exclude the generated code attributes
                                                                         !p.AttributeLists.SelectMany(al => al.Attributes).Any(a => a.Name.ToFullString().Contains("GeneratedCode")));
        }
    }
}
