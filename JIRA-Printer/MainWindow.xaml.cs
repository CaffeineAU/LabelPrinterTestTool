using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Http;
using System.Web.Script.Serialization;
using System.Net;
using System.IO;
using System.ComponentModel;
using System.Web.Helpers;
using SENOR_LIB;
using System.Web;
using System.Collections.ObjectModel;

namespace JIRA_Printer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }


        //private JIRAResult  result;

        //public JIRAResult Result
        //{
        //    get { return result; }
        //    set
        //    {
        //        result = value;
        //        OnPropertyChanged("Result");
        //    }
        //}

        private ObservableCollection<String> issueStatuses = new ObservableCollection<string>();

        public ObservableCollection<String> IssueStatuses
        {
            get { return issueStatuses; }
            set
            {
                issueStatuses = value;
                if (Properties.Settings.Default.IssueStatuses == null)
                {
                    Properties.Settings.Default.IssueStatuses = new System.Collections.Specialized.StringCollection();
                }
                Properties.Settings.Default.IssueStatuses.Clear();
                Properties.Settings.Default.IssueStatuses.AddRange(IssueStatuses.ToArray());
                Properties.Settings.Default.Save();

                OnPropertyChanged("IssueStatuses");
            }
        }

        private ObservableCollection<String> issueFields = new ObservableCollection<string>();

        public ObservableCollection<String> IssueFields
        {
            get { return issueFields; }
            set
            {
                issueFields = value;
                if (Properties.Settings.Default.IssueFields == null)
                {
                    Properties.Settings.Default.IssueFields = new System.Collections.Specialized.StringCollection();
                }
                Properties.Settings.Default.IssueFields.Clear();
                Properties.Settings.Default.IssueFields.AddRange(IssueFields.ToArray());
                Properties.Settings.Default.Save();

                OnPropertyChanged("IssueFields");
            }
        }



        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 10);
            dispatcherTimer.Start();



            // do this once, to create the .settings file
            if (Properties.Settings.Default.LAST_QUERY == null || Properties.Settings.Default.LAST_QUERY < DateTime.Now.AddYears(-1))
            {
                Properties.Settings.Default.LAST_QUERY = DateTime.Now.AddYears(-1);
                Properties.Settings.Default.Save();
            }

            if (Properties.Settings.Default.IssueStatuses == null || Properties.Settings.Default.IssueStatuses.Count == 0)
            {
                IssueStatuses = new ObservableCollection<string> { "Open", "In Progress", "Unresolved" };
                Properties.Settings.Default.IssueStatuses = new System.Collections.Specialized.StringCollection();
                Properties.Settings.Default.IssueStatuses.AddRange(IssueStatuses.ToArray());
                Properties.Settings.Default.Save();

            }
            else
            {
                IssueStatuses = new ObservableCollection<String>(Properties.Settings.Default.IssueStatuses.Cast<string>().ToList());
            }

            if (Properties.Settings.Default.IssueFields == null || Properties.Settings.Default.IssueFields.Count == 0)
            {
                IssueFields = new ObservableCollection<string> { "key", "status", "summary", "progress", "duedate", "assignee", "updated" };
                Properties.Settings.Default.IssueFields = new System.Collections.Specialized.StringCollection();
                Properties.Settings.Default.IssueFields.AddRange(IssueFields.ToArray());
                Properties.Settings.Default.Save();

            }
            else
            {
                IssueFields = new ObservableCollection<String>(Properties.Settings.Default.IssueFields.Cast<string>().ToList());
            }


        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            string project = "MTE";
            int num_results = 100;


            DateTime last_run = Properties.Settings.Default.LAST_QUERY;



            string time_diff = string.Format("-{0:F0}m", Math.Max((DateTime.Now - last_run).TotalMinutes, 1)); // @"startOfDay(""-100"")";



            string str_status = @"""" + String.Join(@""", """, IssueStatuses) + @"""";
            string str_fields = String.Join(", ", IssueFields);

            string request = string.Format(@"{0}search?jql=(project={1} AND (status in ({2})) AND updated>={5})&startAt=0&maxResults={4}&fields={3}",
                Properties.Settings.Default.JIRA_API,
                project,
                str_status,
                str_fields,
                num_results,
                time_diff);

            Clipboard.SetText(request);

            var webRequest = WebRequest.Create(request);




            #region
            string authorization = "Basic " + Base64Encode(String.Format("{0}:{1}", Properties.Settings.Default.JIRAUsername, Properties.Settings.Default.JIRAPassword));
            #endregion

            webRequest.Headers.Add("Authorization", authorization);

            dynamic d;
            try
            {
                var response = (HttpWebResponse)webRequest.GetResponse();




                using (Stream responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                    {

                        //responseStream.CopyTo(memoryStream);
                        String responseString = new StreamReader(responseStream).ReadToEnd();

                        //Clipboard.SetText(responseString);
                        //Result =  new JavaScriptSerializer().Deserialize<JIRAResult>(responseString);

                        d = Json.Decode(responseString);
                        StringBuilder sb = new StringBuilder();
                        foreach (var issue in d.issues)
                        {
                            Ticket t = new Ticket(issue);

                            Console.WriteLine(t.ToString());
                        }
                        //MessageBox.Show(sb.ToString(), "Dynamic is awesome!");


                    }

                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }


            Properties.Settings.Default.LAST_QUERY = DateTime.Now;

            Properties.Settings.Default.Save();


            //GTP_250 printer = new GTP_250();

            //printer.FindPrinter();

            //printer.Connect();



            //foreach (var issue in Result.issues)
            //{

            //    printer.PageMode = GTP_250.NumericOptions.One;

            //    //printer.PageModePrintDirection = GTP_250.NumericOptions.One;

            //    //jira key (e.g. MTE-123)
            //    printer.WriteAsciiString(issue.key);

            //    //summary of the issue
            //    printer.WriteAsciiString("Summary: " + issue.fields.summary);


            //    //assignee
            //    printer.WriteAsciiString("Assignee: " + issue.fields.assignee.displayName);


            //    //due date
            //    printer.WriteAsciiString("Due Date: " + issue.fields.duedate);


            //    //progress
            //    printer.WriteAsciiString("Progress: " + issue.fields.progress.percent.ToString() + "%");



            //    printer.Print();

            //    printer.Cut();

            //}






        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(String name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void AddStatusCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !String.IsNullOrEmpty(newstatus.Text);
        }

        private void AddStatusCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            IssueStatuses.Add(newstatus.Text);
        }

        private void RemoveStatusCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (IssueStatuses.Count > 0 && StatuslistBox.SelectedIndex >= 0);
        }

        private void RemoveStatusCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            IssueStatuses.RemoveAt(StatuslistBox.SelectedIndex);
        }



    }

    // public class JIRAResult
    // {
    //     /*
    //      * {"expand":"names,schema","startAt":0,"maxResults":1,"total":114,"issues":[
    //      * {"expand":"operations,versionedRepresentations,editmeta,changelog,renderedFields","id":"73389","self":"http://jirapd.corp.resmed.org/rest/api/2/issue/73389",
    //      * "key":"MTE-1053","fields":{"summary":"Support BC file","progress":{"progress":28800,"total":86400,"percent":33},
    //      * "assignee":{"self":"http://jirapd.corp.resmed.org/rest/api/2/user?username=bishoyb","name":"bishoyb","key":"bishoyb","emailAddress":"Bishoy.Botros@resmed.com.au",
    //      * "avatarUrls":{"48x48":"http://jirapd.corp.resmed.org/secure/useravatar?avatarId=10109","24x24":"http://jirapd.corp.resmed.org/secure/useravatar?size=small&avatarId=10109","16x16":"http://jirapd.corp.resmed.org/secure/useravatar?size=xsmall&avatarId=10109","32x32":"http://jirapd.corp.resmed.org/secure/useravatar?size=medium&avatarId=10109"},
    //      * "displayName":"Bishoy Botros","active":true,"timeZone":"Australia/Sydney"},
    //      * "duedate":null,
    //      * "status":{"self":"http://jirapd.corp.resmed.org/rest/api/2/status/3","description":"This issue is being actively worked on at the moment by the assignee.","iconUrl":"http://jirapd.corp.resmed.org/images/icons/statuses/inprogress.png",
    //      * "name":"In Progress","id":"3","statusCategory":{"self":"http://jirapd.corp.resmed.org/rest/api/2/statuscategory/4","id":4,"key":"indeterminate","colorName":"yellow","name":"In Progress"}}}}]}
    //      */

    //     public String expand { get; set; }
    //     public String startAt { get; set; }
    //     public String maxResults { get; set; }
    //     public String total { get; set; }
    //     public JIRAIssue[] issues { get; set; }

    // }

    // public class JIRAIssue
    // {
    //     public String expand { get; set; }

    //     public int id { get; set; }

    //     public Uri self { get; set; }
    //     public String key { get; set; }
    //     public JIRAField fields { get; set; }
    // }
    // public class JIRAField
    // {
    //     public String summary { get; set; }

    //     public JIRAProgress progress { get; set; }

    //     public JIRAAssignee assignee { get; set; }
    //     public String duedate { get; set; }
    //     public JIRAStatus status { get; set; }

    // }

    // public class JIRAStatus
    // {
    //     public Uri self { get; set; }
    //     public String description { get; set; }
    //      public Uri iconUrl { get; set; }
    //     public String name { get; set; }
    //     public int id { get; set; }
    //     public JIRAStatusCategory statusCategory { get; set; }
    //}

    // public class JIRAStatusCategory
    // {
    //     public Uri self { get; set; }
    //     public int id { get; set; }
    //     public String key { get; set; }
    //     public String colorName { get; set; }
    //     public String name { get; set; }

    // }

    // public class JIRAAssignee
    // {
    //     public Uri self { get; set; }
    //     public String name { get; set; }
    //     public String key { get; set; }
    //     public String emailAddress { get; set; }

    //     public JIRAAvatarURL avatarUrls { get; set; }
    //     public String displayName { get; set; }
    //     public String active { get; set; }
    //     public String timeZone { get; set; }
    // }

    // public class JIRAAvatarURL
    // {
    //     public Uri _48x48 { get; set; }
    //     public Uri _24x24 { get; set; }
    //     public Uri _16x16 { get; set; }
    //     public Uri _32x32 { get; set; }
    // }

    // public class JIRAProgress
    // {
    //     public int progress { get; set; }
    //     public int total { get; set; }
    //     public int percent { get; set; }
    // }
}

