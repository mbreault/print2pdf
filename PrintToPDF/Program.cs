using System;
using System.Drawing.Printing;
using System.IO;

namespace PrintToPDF
{
    class Program
    {
        static void Main(string[] args)
        {
            PDFPrinter pdfPrinter = new PDFPrinter();
            string timestamp = pdfPrinter.GetTimeStampAsString();
            string sentence = "The quick brown fox jumped over the lazy dog.";
            string text = String.Format("{0}: {1}",timestamp,sentence);
            pdfPrinter.Print(text);
        }
    }
}
