using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JIRA_Printer
{
    public class Ticket
    {
        public Ticket(dynamic d)
        {
            Key = d.key ?? "None";
            Component = "Not implemented";
            Summary = d.fields.summary ?? "None";
            Status = d.fields.status.name;
            Source = d;
            if (d.fields.assignee != null)
            {
                Assignee = d.fields.assignee.displayName ?? "None";
            }

            

            DueDate = d.fields.duedate ?? "None";

            if (d.fields.progress != null && d.fields.progress.percent != null)
            {
                Progress = (int)(d.fields.progress.percent);
            }
        }

        public override string ToString()
        {
            return string.Format("{0} - Summary: {1}\r\n", Key, Summary);
        }

        private dynamic source;

        public dynamic Source
        {
            get { return source; }
            set { source = value; }
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

        private string _Assignee;

        public string Assignee
        {
            get { return _Assignee; }
            set { _Assignee = value; }
        }

        private string _DueDate;

        public string DueDate
        {
            get { return _DueDate; }
            set { _DueDate = value; }
        }

        private int _Progress;

        public int Progress
        {
            get { return _Progress; }
            set { _Progress = value; }
        }

        private string _Component;

        public string Component
        {
            get { return _Component; }
            set { _Component = value; }
        }




    }
}
