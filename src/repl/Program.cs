using System;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace repl
{
    class Program
    {
        static void Main(string[] args)
        {
            ScriptState<object> scriptState = null;
            while (true)
            {
                Console.Write("* ");
                var input = Console.ReadLine();
                scriptState = scriptState == null ?
                    CSharpScript.RunAsync(input, ScriptOptions.Default.AddImports("System")).Result :
                    scriptState.ContinueWithAsync(input).Result;
            }
        }
    }
}
