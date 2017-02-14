using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JIRA_Printer
{
    public class Ticket: INotifyPropertyChanged
    {
        public override string ToString()
        {
            return string.Format("{0} - Summary: {1}\r\n", Key, Summary);
        }

        //private dynamic source;

        //public dynamic Source
        //{
        //    get { return source; }
        //    set { source = value;}
        //}

        private string _Updated;

        public string Updated
        {
            get { return _Updated; }
            set
            {
                _Updated = value;
                OnPropertyChanged("Updated");
            }
        }

        private string _Created;

        public string Created
        {
            get { return _Created; }
            set
            {
                _Created = value;
                OnPropertyChanged("Created");
            }
        }

        private string _issueTypeIcon;

        public string IssueTypeIcon
        {
            get { return _issueTypeIcon; }
            set
            {
                _issueTypeIcon = value;
                OnPropertyChanged("IssueTypeIcon");
            }
        }


        private string _Key;

        public string Key
        {
            get { return _Key; }
            set { _Key = value;
                OnPropertyChanged("Key");
            }
        }

        private string _IssueType;

        public string IssueType
        {
            get { return _IssueType; }
            set
            {
                _IssueType = value;
                OnPropertyChanged("IssueType");
            }
        }

        private string _Status;

        public string Status
        {
            get { return _Status; }
            set
            {
                _Status = value;
                OnPropertyChanged("Status");
            }
        }

        private string _Summary;

        public string Summary
        {
            get { return _Summary; }
            set { _Summary = value;
                OnPropertyChanged("Summary");
            }
        }

        private string _Assignee;

        public string Assignee
        {
            get { return _Assignee; }
            set { _Assignee = value;
                OnPropertyChanged("Assignee");
            }
        }

        private string _DueDate;

        public string DueDate
        {
            get { return _DueDate; }
            set { _DueDate = value;
                OnPropertyChanged("DueDate");
            }
        }

        private int _Progress;

        public int Progress
        {
            get { return _Progress; }
            set { _Progress = value;
                OnPropertyChanged("Progress");
            }
        }

        private string _Component;

        public string Component
        {
            get { return _Component; }
            set { _Component = value;
                OnPropertyChanged("Component");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(String name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


    }
}
