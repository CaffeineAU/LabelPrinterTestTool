using SENOR_LIB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            string hr = "";
            for(int i = 0; i < line_width; i++)
            {
                hr += "=";
            }

            WriteAsciiString(hr);
            //Feed(2);
            WriteAsciiString("Key:\t\t" + t.Key);
            //Feed(1);
            WriteAsciiString("Component:\t" + t.Component);
            //Feed(1);
            WriteAsciiString("Summary:\t" + t.Summary);
            //Feed(1);
            WriteAsciiString("Assignee:\t" + t.Assignee);
            //Feed(1);
            WriteAsciiString("Due:\t\t" + t.DueDate);
            //Feed(1);
            WriteAsciiString("Progress:\t" + t.Progress.ToString() + "%");
            //Feed(1);
            WriteAsciiString(hr);

            Feed(10);

            Print();

            //Feed(5);

            Cut();
        }

    }
}
