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
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

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

        private ObservableCollection<Ticket> result = new ObservableCollection<Ticket>();

        public ObservableCollection<Ticket> Result
        {
            get { return result; }
            set
            {
                result = value;
                OnPropertyChanged("Result");
            }
        }

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
                OnPropertyChanged("IssueStatuses");
            }
        }

        private TimeSpan issueTimePeriod = Properties.Settings.Default.SelectedTimePeriod;

        public TimeSpan IssueTimePeriod
        {
            get { return issueTimePeriod; }
            set
            {
                issueTimePeriod = value;
                OnPropertyChanged("IssueTimePeriod");
                Properties.Settings.Default.SelectedTimePeriod = value;
                Properties.Settings.Default.Save();
                refreshTimer.Interval = IssueTimePeriod;
                refreshTimer.Start();

            }
        }

        private String lastChecked;

        public String LastChecked
        {
            get { return lastChecked; }
            set
            {
                lastChecked = value;
                OnPropertyChanged("LastChecked");
            }
        }

        private TimeSpan countdown;

        public TimeSpan Countdown
        {
            get { return countdown; }
            set
            {
                countdown = value;
                OnPropertyChanged("Countdown");
                OnPropertyChanged("CountdownText");
            }
        }

        public String CountdownText
        {
            get { return String.Format("Next check in {0:hh\\:mm\\:ss}", Countdown); }
        }



        private void RegeneratePropertiesAndSave()
        {
            Properties.Settings.Default.IssueFields.Clear();
            Properties.Settings.Default.IssueFields.AddRange(IssueFields.ToArray());
            Properties.Settings.Default.IssueStatuses.Clear();
            Properties.Settings.Default.IssueStatuses.AddRange(IssueStatuses.ToArray());
            Properties.Settings.Default.Save();
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
                OnPropertyChanged("IssueFields");
            }
        }

        TicketPrinter printer;
            DispatcherTimer refreshTimer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            printer = new TicketPrinter();
            printer.FindPrinter();
            printer.Connect();

            refreshTimer.Interval = IssueTimePeriod;
            refreshTimer.Tick += delegate { ShowIssues(); };
            refreshTimer.Start();

            DispatcherTimer t2 = new DispatcherTimer();
            t2.Interval = TimeSpan.FromSeconds(0.5);
            t2.Tick += delegate { Countdown = Properties.Settings.Default.LAST_QUERY + IssueTimePeriod - DateTime.Now; };
            t2.Start();


            // do this once, to create the .settings file
            if (Properties.Settings.Default.LAST_QUERY == null || Properties.Settings.Default.LAST_QUERY < DateTime.Now.AddYears(-1))
            {
                Properties.Settings.Default.LAST_QUERY = DateTime.Now.AddYears(-1);
                Properties.Settings.Default.Save();
            }

            if (Properties.Settings.Default.LastPrintTime == null || Properties.Settings.Default.LastPrintTime < DateTime.Now.AddYears(-1))
            {
                Properties.Settings.Default.LastPrintTime = DateTime.Now.AddYears(-1);
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

            IssueStatuses.CollectionChanged += delegate { RegeneratePropertiesAndSave(); };
            IssueFields.CollectionChanged += delegate { RegeneratePropertiesAndSave(); };

            ShowIssues();

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

        private void AddFieldCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !String.IsNullOrEmpty(newField.Text);
        }

        private void AddFieldCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            IssueFields.Add(newField.Text);
        }

        private void RemoveFieldCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (IssueFields.Count > 0 && FieldslistBox.SelectedIndex >= 0);
        }

        private void RemoveFieldCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            IssueFields.RemoveAt(FieldslistBox.SelectedIndex);
        }

        private void ShowIssuesCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true; //!String.IsNullOrEmpty(IssueTimePeriod);
        }


        private void ShowIssuesCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ShowIssues();
        }

        private void ShowIssues()
        {
            

            string project = "MTE";
            int num_results = 1000;

            string str_status = @"""" + String.Join(@""", """, IssueStatuses) + @"""";
            string str_fields = String.Join(", ", IssueFields);


            //Updated since the last print
            double issuetimeperiod = Math.Max((DateTime.Now - Properties.Settings.Default.LastPrintTime).TotalMinutes, 1);


            string request = string.Format(@"{0}search?jql=(project={1} AND (status in ({2})) AND updated>=-{5:F0}m)&startAt=0&maxResults={4}&fields={3}",
                Properties.Settings.Default.JIRA_API,
                project,
                str_status,
                str_fields,
                num_results,
                issuetimeperiod);

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

                        String responseString = new StreamReader(responseStream).ReadToEnd();

                        // Cheating :-)
                        responseString = responseString.Replace("48x48", "_48x48");
                        responseString = responseString.Replace("32x32", "_32x32");
                        responseString = responseString.Replace("24x24", "_24x24");
                        responseString = responseString.Replace("16x16", "_16x16");

                        d = Json.Decode(responseString);

                        int newissues = 0;

                        foreach (var issue in d.issues)
                        {
                            if (Result.Any(n => n.Key == issue.key))
                            {
                                Result.Remove(Result.First(n => n.Key == issue.key)); // if it's already there, delete it and add it again, it must have changed
                            }
                            else // a new one
                            {
                                newissues++;
                            }

                            Result.Add(new Ticket
                            {
                                Key = issue.key ?? "None",
                                Component = "Not implemented",
                                Summary = issue.fields.summary ?? "None",
                                Status = issue.fields.status.name,
                                IssueType = issue.fields.issuetype.name,
                                IssueTypeIcon = issue.fields.issuetype.iconUrl,
                                Updated = DateTime.Parse(issue.fields.updated).ToString("dd MMM yyyy HH:mm:ss"),
                                //Source = issue,
                                Assignee = issue.fields.assignee != null ? issue.fields.assignee.displayName ?? "None" : "None",
                                Progress = issue.fields.progress != null && issue.fields.progress.percent != null ? (int)(issue.fields.progress.percent) : 0,
                                DueDate = issue.fields.duedate ?? "None"
                            });
                        }

                        if (newissues > 0)
                        {
                            JiraBalloon balloon = new JiraBalloon();
                            balloon.MouseDown += delegate { WindowState = WindowState.Normal; Activate(); };
                            balloon.BalloonText = "JIRA Issues updated";
                            balloon.BalloonContent = String.Format("{0} issue{1} updated", newissues, newissues == 1? "":"s");

                            //show balloon and close it after 4 seconds
                            myNotifyIcon.ShowCustomBalloon(balloon, PopupAnimation.Slide, 4000);

                            //if (WindowState == WindowState.Normal)
                            //{
                                dataGrid.Focus();
                            //}
                            

                        }


                        DateTime lastchecked = DateTime.Now;
                        Properties.Settings.Default.LAST_QUERY =lastchecked; // Not actually used anymore
                        LastChecked = String.Format("Last checked at {0}, next check is {1}. Last printed at {2}", lastchecked.ToString("dd MMM yyyy HH:mm:ss"),  (lastchecked + IssueTimePeriod).ToString("dd MMM yyyy HH:mm:ss"), Properties.Settings.Default.LastPrintTime.ToString("dd MMM yyyy HH:mm:ss"));
                        Properties.Settings.Default.Save();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (printer.Connected)
            {
                printer.Disconnect();
            }
        }

        private void PrintIssuesCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Result.Count > 0;
        }

        private void PrintIssuesCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            foreach (var issue in Result)
            {
                TicketTemplate tt = new TicketTemplate { TheTicket = issue };

                tt.DownloadComplete += delegate (object sender2, DownloadEventArgs e2)
                {
                    // would be nicer to do this with a memory stream or something, rather than temporary files
                    printer.PrintBitImage(GTP_250.GetBitmapData(String.Format("{0}{1}.png", System.IO.Path.GetTempPath(), e2.FileName)));
                    File.Delete(String.Format("{0}{1}.png", System.IO.Path.GetTempPath(), e2.FileName));
                    printer.Feed(8);
                    printer.Cut();
                    tt.Close();
                };
                //Show
                //System.Diagnostics.Process.Start(String.Format("{0}{1}.png", System.IO.Path.GetTempPath(), issue.Key));
                // Print
                tt.Export();
            }
            Properties.Settings.Default.LastPrintTime = DateTime.Now;
            Properties.Settings.Default.Save();
            Result.Clear();

        }

        private void myNotifyIcon_TrayLeftMouseUp(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.WindowState = WindowState.Normal;
            }
            this.Activate();

        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == System.Windows.WindowState.Minimized)
            {
                this.ShowInTaskbar = false;
            }
            else
            {
                this.ShowInTaskbar = true;
            }

            //base.OnStateChanged(e);
        }
    }


    //public class JIRAResult
    //{
    //    /*
    //     * {"expand":"names,schema","startAt":0,"maxResults":1,"total":114,"issues":[
    //     * {"expand":"operations,versionedRepresentations,editmeta,changelog,renderedFields","id":"73389","self":"http://jirapd.corp.resmed.org/rest/api/2/issue/73389",
    //     * "key":"MTE-1053","fields":{"summary":"Support BC file","progress":{"progress":28800,"total":86400,"percent":33},
    //     * "assignee":{"self":"http://jirapd.corp.resmed.org/rest/api/2/user?username=bishoyb","name":"bishoyb","key":"bishoyb","emailAddress":"Bishoy.Botros@resmed.com.au",
    //     * "avatarUrls":{"48x48":"http://jirapd.corp.resmed.org/secure/useravatar?avatarId=10109","24x24":"http://jirapd.corp.resmed.org/secure/useravatar?size=small&avatarId=10109","16x16":"http://jirapd.corp.resmed.org/secure/useravatar?size=xsmall&avatarId=10109","32x32":"http://jirapd.corp.resmed.org/secure/useravatar?size=medium&avatarId=10109"},
    //     * "displayName":"Bishoy Botros","active":true,"timeZone":"Australia/Sydney"},
    //     * "duedate":null,
    //     * "status":{"self":"http://jirapd.corp.resmed.org/rest/api/2/status/3","description":"This issue is being actively worked on at the moment by the assignee.","iconUrl":"http://jirapd.corp.resmed.org/images/icons/statuses/inprogress.png",
    //     * "name":"In Progress","id":"3","statusCategory":{"self":"http://jirapd.corp.resmed.org/rest/api/2/statuscategory/4","id":4,"key":"indeterminate","colorName":"yellow","name":"In Progress"}}}}]}
    //     */

    //    public String expand { get; set; }
    //    public String startAt { get; set; }
    //    public String maxResults { get; set; }
    //    public String total { get; set; }
    //    public JIRAIssue[] issues { get; set; }

    //}

    //public class JIRAIssue
    //{
    //    public String expand { get; set; }

    //    public int id { get; set; }

    //    public Uri self { get; set; }
    //    public String key { get; set; }
    //    public JIRAField fields { get; set; }
    //}
    //public class JIRAField
    //{
    //    public String summary { get; set; }

    //    public JIRAProgress progress { get; set; }

    //    public JIRAAssignee assignee { get; set; }
    //    public String duedate { get; set; }
    //    public JIRAStatus status { get; set; }

    //}

    //public class JIRAStatus
    //{
    //    public Uri self { get; set; }
    //    public String description { get; set; }
    //    public Uri iconUrl { get; set; }
    //    public String name { get; set; }
    //    public int id { get; set; }
    //    public JIRAStatusCategory statusCategory { get; set; }
    //}

    //public class JIRAStatusCategory
    //{
    //    public Uri self { get; set; }
    //    public int id { get; set; }
    //    public String key { get; set; }
    //    public String colorName { get; set; }
    //    public String name { get; set; }

    //}

    //public class JIRAAssignee
    //{
    //    public Uri self { get; set; }
    //    public String name { get; set; }
    //    public String key { get; set; }
    //    public String emailAddress { get; set; }

    //    public JIRAAvatarURL avatarUrls { get; set; }
    //    public String displayName { get; set; }
    //    public String active { get; set; }
    //    public String timeZone { get; set; }
    //}

    //public class JIRAAvatarURL
    //{

    //    public Uri _48x48 { get; set; }
    //    public Uri _24x24 { get; set; }
    //    public Uri _16x16 { get; set; }
    //    public Uri _32x32 { get; set; }
    //}

    //public class JIRAProgress
    //{
    //    public int progress { get; set; }
    //    public int total { get; set; }
    //    public int percent { get; set; }
    //}
}

