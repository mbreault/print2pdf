using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;

namespace PrintToPDF
{
    public class PDFPrinter
    {
        // Credit to https://stackoverflow.com/questions/31896952/how-to-programmatically-print-to-pdf-file-without-prompting-for-filename-in-c-sh

        public string Text { get; set; }


        // Credit to https://stackoverflow.com/questions/1344221/how-can-i-generate-random-alphanumeric-strings
        private Random _random = new Random();
        Color _rectangleColor = Color.FromArgb(255, 0, 0, 0);//black
        Color _textColor = Color.FromArgb(255, 0, 0, 0);//black
        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        public string GetTimeStampAsString()
        {
            return Convert.ToInt32((DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds).ToString();
        }
        public void Print(string text, string textColor=null, string rectangleColor=null) // print using optional colors
        {
            SetColors(textColor, rectangleColor); // set the optional colors
            this.Text = text;

            // generate a file name as the current date/time in unix timestamp format
            //string file = (string)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds.ToString();
            string file = "pdfout.pdf";
            string documentName = RandomString(10);
            string printerName = "Microsoft Print to PDF";

            // the directory to store the output.
            string myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string directory = Path.Combine(myDocuments,printerName,documentName);
            System.IO.Directory.CreateDirectory(directory);

            // initialize PrintDocument object
            PrintDocument printDocument = new PrintDocument()
            {
                DocumentName = documentName,
                PrinterSettings = new PrinterSettings()
                {
                    // set the printer to 'Microsoft Print to PDF'
                    PrinterName = printerName,

                    // tell the object this document will print to file
                    PrintToFile = true,

                    // set the filename to whatever you like (full path)
                    PrintFileName = Path.Combine(directory, file)
                }
            };

            Console.WriteLine(printDocument.PrinterSettings.PrintFileName);

            printDocument.PrintPage += new PrintPageEventHandler(PrintDocument_PrintPage);

            printDocument.Print();
        }

        private void SetColors(string textColor, string rectangleColor)// sets the compile time colors
        {
            try //try getting the known colors from the colors strings
            {
                if (textColor != null && rectangleColor != null)
                {
                    KnownColor kcRect = (KnownColor)Enum.Parse(typeof(KnownColor), rectangleColor);
                    KnownColor kcText = (KnownColor)Enum.Parse(typeof(KnownColor), textColor);
                    _rectangleColor = Color.FromKnownColor(kcRect);
                    _textColor = Color.FromKnownColor(kcText);
                }
            }
            catch(Exception ex)
            {
                //on error reverts to black
                Debug.WriteLine("Color input error");
            }
           
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs ev)
        {
            ev.Graphics.DrawString(this.Text, new Font("Arial", 10), new SolidBrush(_textColor),
                                 ev.MarginBounds.Left, ev.MarginBounds.Top, new StringFormat());

            // Create pen.
            Pen pen = new Pen(_rectangleColor, 1);

            // Create rectangle.
            Rectangle rect = new Rectangle(ev.MarginBounds.Left, ev.MarginBounds.Top, ev.MarginBounds.Right - ev.MarginBounds.Left, ev.MarginBounds.Bottom - ev.MarginBounds.Top);

            // Draw rectangle to screen.
            ev.Graphics.DrawRectangle(pen, rect);
        }
    }


}
