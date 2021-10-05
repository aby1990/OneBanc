using CsvHelper;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace OneBanc.Models
{
    class CreditCardStandard
    {
        public DateTime Date { get; set; }
        public string TransDescription { get; set; }
        public double Debit { get; set; }
        public double Credit { get; set; }
        public string Currency { get; set; }
        public string CardName { get; set; }
        public string Transaction { get; set; }
        public string Location { get; set; }

        public void StandardizeStatement(string inputFile, string outputFile)
        {
            try
            {
                ICreditCard creditCard = null;

                creditCard = GetCreditCardObject(inputFile);
                creditCard.CardValues = new List<string[]>();

                using (TextFieldParser textFieldParser = new TextFieldParser(inputFile))
                {
                    textFieldParser.TextFieldType = FieldType.Delimited;
                    textFieldParser.SetDelimiters(",");
                    while (!textFieldParser.EndOfData)
                    {
                        string[] rows = textFieldParser.ReadFields();
                        creditCard.CardValues.Add(rows);
                    }
                }

                var outputData = MapCsvDatatoStandardFormat(creditCard);

                CreateOutputFile(outputData, outputFile);
            }
            catch(Exception ex)
            {

            }

        }

        public void CreateOutputFile(List<CreditCardStandard> outputData, string outputFile)
        {
            try
            {

                using var mem = new MemoryStream();
                using var writer = new StreamWriter(mem);
                using var csvWriter = new CsvWriter(writer, CultureInfo.CurrentCulture);

                csvWriter.WriteField("Date");
                csvWriter.WriteField("Transaction Description");
                csvWriter.WriteField("Debit");
                csvWriter.WriteField("Credit");
                csvWriter.WriteField("Currency");
                csvWriter.WriteField("CardName");
                csvWriter.WriteField("Transaction");
                csvWriter.WriteField("Location");
                csvWriter.NextRecord();
                outputData.Sort((x, y) => x.Date.CompareTo(y.Date));
                foreach (var output in outputData)
                {
                    csvWriter.WriteField(output.Date.ToString("dd-MM-yyyy"));
                    csvWriter.WriteField(output.TransDescription);
                    csvWriter.WriteField(output.Debit);
                    csvWriter.WriteField(output.Credit);
                    csvWriter.WriteField(output.Currency);
                    csvWriter.WriteField(output.CardName);
                    csvWriter.WriteField(output.Transaction);
                    csvWriter.WriteField(output.Location);
                    csvWriter.NextRecord();
                }

                writer.Flush();
                var result = Encoding.UTF8.GetString(mem.ToArray());

                File.WriteAllText(outputFile, result);
            }
            catch(Exception ex)
            {

            }
        }

        public ICreditCard GetCreditCardObject(string inputFile)
        {
            ICreditCard creditCard = null;
            if (inputFile.Contains("axis", StringComparison.OrdinalIgnoreCase))
            {
                creditCard = new AxisCard();
            }
            else if (inputFile.Contains("hdfc", StringComparison.OrdinalIgnoreCase))
            {
                creditCard = new HdfcCard();
            }
            else if (inputFile.Contains("idfc", StringComparison.OrdinalIgnoreCase))
            {
                creditCard = new IdfcCard();
            }
            else if (inputFile.Contains("icici", StringComparison.OrdinalIgnoreCase))
            {
                creditCard = new IciciCard();
            }
            return creditCard;
        }

        public List<CreditCardStandard> MapCsvDatatoStandardFormat(ICreditCard creditCard)
        {
            string tranIdentifier = "Transaction";
            string headIdentifier = "Date";

            string tranType = string.Empty;
            string cardName = string.Empty;
            //defined a mapping in respective card statement class as per the input file type
            int[] mapping = creditCard.Mapping;
            List<CreditCardStandard> creditCardStandards = new List<CreditCardStandard>();
            try
            {
                foreach (var arr in creditCard.CardValues)
                {
                    string[] tmpArr = Array.FindAll(arr, f => !string.IsNullOrEmpty(f));
                    if (tmpArr.Length == 0)
                        continue;
                    else if (tmpArr.Length == 1)
                    {
                        if (tmpArr[0].Contains(tranIdentifier, StringComparison.OrdinalIgnoreCase))
                            tranType = tmpArr[0].Contains("domestic", StringComparison.OrdinalIgnoreCase) ? "Domestic" : "International";
                        else
                            cardName = tmpArr[0];
                        continue;
                    }
                    else if (arr[mapping[0]].Contains(headIdentifier, StringComparison.OrdinalIgnoreCase))
                        continue;

                    CreditCardStandard ccs = new CreditCardStandard();
                    ccs.CardName = cardName;
                    ccs.Transaction = tranType;
                    if (!string.IsNullOrEmpty(arr[mapping[0]]))
                        ccs.Date = GetTranDate(arr[mapping[0]]);
                    ccs.TransDescription = arr[mapping[1]];
                    string desc = ccs.TransDescription;
                    if (tranType.Equals("international", StringComparison.OrdinalIgnoreCase))
                    {
                        ccs.Currency = desc.Substring(desc.Length - 4).Trim();
                        desc = desc.Substring(0, desc.Length - 4).Trim();
                    }
                    else
                        ccs.Currency = "INR";
                    if (mapping[2] == mapping[3])
                    {
                        if (arr[mapping[2]].Contains("cr", StringComparison.OrdinalIgnoreCase))
                            ccs.Credit = Convert.ToDouble(arr[mapping[2]].Replace("cr", "", StringComparison.OrdinalIgnoreCase).Trim());
                        else
                            ccs.Debit = Convert.ToDouble(arr[mapping[2]]);
                    }
                    else
                    {
                        ccs.Debit = string.IsNullOrEmpty(arr[mapping[2]]) ? 0 : Convert.ToDouble(arr[mapping[2]]);
                        ccs.Credit = string.IsNullOrEmpty(arr[mapping[3]]) ? 0 : Convert.ToDouble(arr[mapping[3]]);
                    }
                    ccs.Location = GetLocation(desc);
                    creditCardStandards.Add(ccs);
                }
            }
            catch(Exception ex)
            {

            }
            return creditCardStandards;
        }

        //fetching location from description
        string GetLocation(string desc)
        {
            return desc.Substring(desc.LastIndexOf(' ') + 1).ToLower(); ;
        }

        //handling multiple date formats
        DateTime GetTranDate(string date)
        {
            string[] formats = { "MM-dd-yyyy", "dd-MM-yyyy", "dd-MM-yy" };
            var culture = System.Globalization.CultureInfo.CurrentCulture;
            return DateTime.ParseExact(date, formats, culture);
        }
    }
}
