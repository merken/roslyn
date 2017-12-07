using System;
using System.Linq;
using System.IO;
using Microsoft.Extensions.Configuration;
using welfare.core;
using welfare.scripting.context;
using welfare.scripting.model;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace welfare.scripting
{
    public class WelfareRuleHost
    {
        public int monthsOfUnemployment { get; set; }
        public int maritalStatus { get; set; }
        public int handicap { get; set; }
        public int numberOfChildren { get; set; }
        public decimal brutowage { get; set; }
        public int age { get; set; }
    }

    class ScriptingWelfareService : IWelfareService
    {
        private readonly UnemploymentDbContext database;
        private readonly ScriptOptions scriptOptions;

        public ScriptingWelfareService(UnemploymentDbContext database)
        {
            this.database = database;
            scriptOptions = ScriptOptions.Default.AddReferences(
                typeof(object).GetTypeInfo().Assembly)
                .AddImports("System");
        }

        public decimal GetWelfare(int monthsOfUnemployment, int maritalStatus, int handicap, int numberOfChildren, decimal brutowage, int age)
        {
            var host = GenerateHost(monthsOfUnemployment, maritalStatus, handicap, numberOfChildren, brutowage, age);

            var scripts = GetWelfareScripts();
            decimal? result = null;

            foreach (var script in scripts)
            {
                Console.WriteLine($"Running welfare middleware : {script.Name}");
                result = RunWelfareScriptAsync(host, script.Name, script.Middleware).Result;
                if (result != null)
                    break;
            }

            if (result == null || !result.HasValue)
                throw new NotSupportedException("This business logic script has not yet been covered");

            return result.Value;
        }

        private IEnumerable<WelfareScript> GetWelfareScripts()
        {
            var scripts = database.WelfareScript.OrderBy(w => w.Id);
            return scripts;
        }

        private async Task<decimal?> RunWelfareScriptAsync(WelfareRuleHost host, string name, string script)
        {
            var compiledScript = CSharpScript.Create(script, scriptOptions, typeof(WelfareRuleHost));
            var diagnostics = compiledScript.GetCompilation().GetDiagnostics();

            if (diagnostics.Any())
            {
                var allDiagnostics = diagnostics.Select(d => d.GetMessage());
                var allMessages = String.Join(", ", allDiagnostics);
                throw new ApplicationException($"Runtime exception while compiling script : {allMessages}");
            }

            var result = await compiledScript.RunAsync(host);

            if (result.Exception != null)
                throw new ApplicationException($"Runtime exception while compiling script : {result.Exception.Message}");

            if (result.ReturnValue == null)
                return null;

            return (decimal)result.ReturnValue;
        }

        private WelfareRuleHost GenerateHost(int monthsOfUnemployment, int maritalStatus, int handicap, int numberOfChildren, decimal brutowage, int age)
        {
            return new WelfareRuleHost
            {
                monthsOfUnemployment = monthsOfUnemployment,
                maritalStatus = maritalStatus,
                handicap = handicap,
                numberOfChildren = numberOfChildren,
                brutowage = brutowage,
                age = age
            };
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json");

            var configuration = builder.Build();

            using (var unemployment = new UnemploymentDbContext(configuration["unemployment"]))
            {
                var welfareService = new ScriptingWelfareService(unemployment);
                Console.WriteLine($"Welfare for 0 months, unmarried, no handicap, no kids, wage:500, age:19");
                Console.WriteLine($"{welfareService.GetWelfare(0, 1, 1, 0, 500, 19)}");
                Console.WriteLine($"");

                Console.WriteLine($"Welfare for 5 months, unmarried, no handicap, no kids, wage:500, age:19");
                Console.WriteLine($"{welfareService.GetWelfare(5, 1, 1, 0, 500, 19)}");
                Console.WriteLine($"");

                Console.WriteLine($"Welfare for 16 months, unmarried, no handicap, no kids, wage:500, age:19");
                Console.WriteLine($"{welfareService.GetWelfare(16, 1, 1, 0, 500, 19)}");
                Console.WriteLine($"");

                Console.WriteLine($"Welfare for 0 months, unmarried, no handicap, no kids, wage:1600, age:19");
                Console.WriteLine($"{welfareService.GetWelfare(0, 1, 1, 0, 1600, 19)}");
                Console.WriteLine($"");

                Console.WriteLine($"Welfare for 5 months, unmarried, no handicap, no kids, wage:1600, age:19");
                Console.WriteLine($"{welfareService.GetWelfare(5, 1, 1, 0, 1600, 19)}");
                Console.WriteLine($"");

                Console.WriteLine($"Welfare for 16 months, unmarried, no handicap, no kids, wage:1600, age:19");
                Console.WriteLine($"{welfareService.GetWelfare(16, 1, 1, 0, 1600, 19)}");
                Console.WriteLine($"");

                Console.WriteLine($"Terminated");
                Console.Read();
            }
        }
    }
}
