using RuleEngineChallenge;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleEngineApp
{
    class Program
    {
        static string startupPath = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName, "raw_data.json");
        private static readonly string inputData = File.ReadAllText(startupPath);
        static void Main(string[] args)
        {
            var rules = Rule.GetRules();

            var parser = new Parser();
            if (parser.TryParse(inputData, out List<SignalData> actualValidSignals, out List<SignalData> actualInvalidSignals, false, rules))
            {
                Console.WriteLine("The below mentioned data violate the rules");
                foreach (SignalData invalidsignals in actualInvalidSignals)
                {
                    Console.WriteLine(invalidsignals.Signal);
                }
                // Incase the successful signals are also needed
                //Console.WriteLine("The below mentioned data obey the rules");
                //foreach (var validsignals in actualValidSignals)
                //{
                //    Console.WriteLine(String.Concat(validsignals.Signal, ',', validsignals.Value, ',', validsignals.ValueType));
                //}
                if(actualInvalidSignals.Count()==0)
                {
                    Console.WriteLine("The incoming data are all valid.");
                }
                
            }
            else
            {
                Console.WriteLine("Unable to parse incoming data.");
                
            }
            Console.Read();

        }
    }
}
