using OneBanc.Models;
using System;
using System.IO;

namespace OneBanc
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileName = string.Empty;
            try
            {
                //fetch file list from input directory
                string[] fileList = Directory.GetFiles(@"../../../Input/");
                foreach (string inputFile in fileList)
                {
                    fileName = inputFile;
                    CreditCardStandard creditCardStandard = new CreditCardStandard();
                    string outputFile = inputFile.Replace("Input", "Output");
                    creditCardStandard.StandardizeStatement(inputFile, outputFile);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error while standardising file {fileName}");
            }
        }        
    }
}
