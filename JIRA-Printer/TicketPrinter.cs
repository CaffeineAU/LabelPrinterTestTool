using SENOR_LIB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using Svg;
using System.Drawing.Imaging;

namespace JIRA_Printer
{
    class TicketPrinter : GTP_250
    {

        public TicketPrinter()
        {
            this.PageMode = GTP_250.NumericOptions.One;

            this.PageModePrintDirection = GTP_250.NumericOptions.One;
        }

        public void PrintTicket(Ticket t)
        {

            int line_width = 42;




            DrawLine(line_width);
            //Feed(2);
            FontXMultiplier = 2;
            FontYMultiplier = 2;
            PrintPosition = 144; // 20mm
            Emphasised = NumericOptions.One;
            WriteAsciiString(t.Key);
            Reset();

            DrawLine(line_width);
            //Feed(1);
            WriteAsciiString("Component:\t" + t.Component);
            DownloadAndPrintImage(t.Source.fields.status.iconUrl);
            WriteAsciiString("Status:\t" + t.Status);
            //Feed(1);
            WriteAsciiString("Summary:\t" + t.Summary);
            //Feed(1);
            WriteAsciiString("Assignee:\t" + t.Assignee);
            //Feed(1);
            WriteAsciiString("Due:\t\t" + t.DueDate);
            //Feed(1);
            WriteAsciiString("Progress:\t" + t.Progress.ToString() + "%");
            //Feed(1);
            DrawLine(line_width);

            Feed(10);

            Print();

            //Feed(5);

            Cut();
        }

        private void DownloadAndPrintImage(string URL)
        {
            Guid temp = Guid.NewGuid();
            using (WebClient wc = new WebClient())
            {
                string authorization = "Basic " + System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(String.Format("{0}:{1}", Properties.Settings.Default.JIRAUsername, Properties.Settings.Default.JIRAPassword)));

                wc.Headers.Add("Authorization", authorization);

                wc.DownloadFile(URL, String.Format("{0}{1}.png", Path.GetTempPath(), temp));
                if (File.ReadAllText(String.Format("{0}{1}.png", Path.GetTempPath(), temp)).StartsWith("<?xml")) // it's an SVG file
                {
                    var svgDocument = SvgDocument.Open(String.Format("{0}{1}.png", Path.GetTempPath(), temp));
                    var bitmap = svgDocument.Draw(48, 48);
                    bitmap.Save(String.Format("{0}{1}.png", Path.GetTempPath(), temp), ImageFormat.Png);
                }
            }


            PrintBitImage(GetBitmapData(String.Format("{0}{1}.png", Path.GetTempPath(), temp)));
            File.Delete(String.Format("{0}{1}.png", Path.GetTempPath(), temp));

        }

        private void DrawLine(int line_width)
        {
            string hr = "";
            for (int i = 0; i < line_width; i++)
            {
                hr += " ";
            }

            Underlined = NumericOptions.One;
            WriteAsciiString(hr);
            Underlined = NumericOptions.Zero;
        }
    }
}
