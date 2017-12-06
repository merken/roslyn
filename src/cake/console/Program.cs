using System;
using lib;
namespace console
{
    class Program
    {
        static void Main(string[] args)
        {
            var swag = new BusinessLogic().GetSwag("BILL");
            Console.WriteLine(swag);
        }
    }
}
