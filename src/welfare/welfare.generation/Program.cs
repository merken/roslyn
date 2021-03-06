﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Reflection;

namespace welfare.generation
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = ReadConfiguration();

            var welfareRules = GetWelfareRules(configuration["WelfareDb"]);

            var welfareServiceBuilder = ServiceBuilder.CreateWelfareServiceBuilder()
                .AddNamespace("welfare.businesslogic")
                .AddUsing("System")
                .AddUsing("welfare.core");

            foreach (var rule in welfareRules.OrderBy(r => r.Id))
                welfareServiceBuilder = welfareServiceBuilder.AddWelfareRule(rule);

            var compilation = welfareServiceBuilder.CreateCompilation();

            compilation.SaveToFile("welfarelogic.dll");

            var welfareService = compilation
                .Build()
                .GetNewInstance();

            Console.WriteLine($"Welfare for 0 months, unmarried, no handicap, no kids, wage:500, age:19");
            Console.WriteLine($"{welfareService.GetWelfare(0, 1, 1, 0, 500, 19)}");
            Console.WriteLine($"");

            Console.WriteLine($"Welfare for 4 months, unmarried, no handicap, no kids, wage:500, age:19");
            Console.WriteLine($"{welfareService.GetWelfare(4, 1, 1, 0, 500, 19)}");
            Console.WriteLine($"");

            Console.WriteLine($"Welfare for 12 months, unmarried, no handicap, no kids, wage:500, age:19");
            Console.WriteLine($"{welfareService.GetWelfare(12, 1, 1, 0, 500, 19)}");
            Console.WriteLine($"");

            Console.WriteLine($"Welfare for 0 months, unmarried, no handicap, no kids, wage:1600, age:19");
            Console.WriteLine($"{welfareService.GetWelfare(0, 1, 1, 0, 1600, 19)}");
            Console.WriteLine($"");

            Console.WriteLine($"Welfare for 4 months, unmarried, no handicap, no kids, wage:1600, age:19");
            Console.WriteLine($"{welfareService.GetWelfare(4, 1, 1, 0, 1600, 19)}");
            Console.WriteLine($"");

            Console.WriteLine($"Welfare for 12 months, unmarried, no handicap, no kids, wage:1600, age:19");
            Console.WriteLine($"{welfareService.GetWelfare(12, 1, 1, 0, 1600, 19)}");
            Console.WriteLine($"");

            Console.WriteLine($"Terminated");
            Console.Read();
        }

        private static IConfigurationRoot ReadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            return builder.Build();
        }

        private static IList<WelfareRule> GetWelfareRules(string connectionString)
        {
            if (connectionString == null) throw new Exception("connectionString was empty");

            var table = ExecuteSql(connectionString,
                @"SELECT  Id ,
                        Month ,
                        IdMaritalStatus ,
                        IdHandicap ,
                        NumberOfChildren ,
                        BrutoStart,
                        BrutoEnd,
                        Age,
                        Value
                FROM    WelfareLogic");

            var rules = MapFromDataTable(table);

            return rules;
        }

        private static DataTable ExecuteSql(string connectionString, string sql)
        {
            SqlConnection sqlcon = new SqlConnection(connectionString);

            sqlcon.Open();

            SqlCommand command = new SqlCommand(sql, sqlcon);

            SqlDataAdapter custAdapter =
                new SqlDataAdapter(command) { SelectCommand = { CommandTimeout = 1800 } };
            DataSet dataset = new DataSet();
            custAdapter.Fill(dataset);

            return dataset.Tables[0];
        }

        private static IList<WelfareRule> MapFromDataTable(DataTable table)
        {
            var rules = new List<WelfareRule>();
            foreach (DataRow record in table.Rows)
            {
                rules.Add(new WelfareRule
                {
                    Id = (int)record[0],
                    Month = (int)record[1],
                    IdMaritalStatus = (int)record[2],
                    IdHandicap = (int)record[3],
                    NumberOfChildren = (int)record[4],
                    BrutoStart = (int)record[5],
                    BrutoEnd = (int)record[6],
                    Age = (int)record[7],
                    Value = (decimal)record[8]
                });
            }

            return rules;
        }
    }
}
