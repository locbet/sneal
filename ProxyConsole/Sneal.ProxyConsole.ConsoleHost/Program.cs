using System;

namespace Sneal.ProxyConsole.ConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var host = new Host())
            {
                host.Start();
                Console.WriteLine("ProxyConsole hosting service");
                Console.WriteLine();
                Console.WriteLine("Waiting for connection...");
                Console.ReadKey();
            }
        }
    }
}
