using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JIRA_Printer
{
    class Ticket
    {
        public Ticket(dynamic d)
        {
            Key = d.key;

            Summary = d.fields.summary;

        }

        public override string ToString()
        {
            return string.Format("{0} - Summary: {1}\r\n", Key, Summary);
        }


        private string _Key;

        public string Key
        {
            get { return _Key; }
            set { _Key = value; }
        }

        private string _Status;

        public string Status
        {
            get { return _Status; }
            set { _Status = value; }
        }

        private string _Summary;

        public string Summary
        {
            get { return _Summary; }
            set { _Summary = value; }
        }



    }
}
