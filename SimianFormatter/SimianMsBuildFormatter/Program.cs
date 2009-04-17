using System;

namespace SimianMsBuildFormatter
{
    class Program
    {
        static void Main()
        {
            OutputFormatter formatter = new OutputFormatter();
            string s;
            while ((s = Console.ReadLine()) != null)
            {
                formatter.WriteSimianOutputLine(s);
            }
        }
    }
}
