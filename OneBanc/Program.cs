using OneBanc.Models;
using System.IO;

namespace OneBanc
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] fileEntries = Directory.GetFiles(@"../../../Input/");
            foreach (string inputFile in fileEntries)
            {
                CreditCardStandard creditCardStandard = new CreditCardStandard();
                string outputFile = inputFile.Replace("Input", "Output");
                creditCardStandard.StandardizeStatement(inputFile, outputFile);
            }            
        }        
    }
}
