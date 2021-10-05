using OneBanc.Models;
using System.IO;

namespace OneBanc
{
    class Program
    {
        static void Main(string[] args)
        {
            //fetch file list from input directory
            string[] fileList = Directory.GetFiles(@"../../../Input/");
            foreach (string inputFile in fileList)
            {
                CreditCardStandard creditCardStandard = new CreditCardStandard();
                string outputFile = inputFile.Replace("Input", "Output");
                creditCardStandard.StandardizeStatement(inputFile, outputFile);
            }            
        }        
    }
}
