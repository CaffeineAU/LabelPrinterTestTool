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

        private JIRAResult  result;

        public JIRAResult Result
        {
            get { return result; }
            set
            {
                result = value;
                OnPropertyChanged("Result");
            }
        }


        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            var webRequest = WebRequest.Create("http://jirapd.corp.resmed.org/rest/api/2/search?jql=project=MTE%20AND%20(status%20in(%22Open%22,%22In%20Progress%22))&fields=key,status,summary,progress,duedate,assignee&maxResults=20");

            // do this once, to create the .settings file
            Properties.Settings.Default.JIRAUsername = Properties.Settings.Default.JIRAUsername;
            Properties.Settings.Default.Save();

            #region
            string authorization = "Basic " + Base64Encode(String.Format("{0}:{1}", Properties.Settings.Default.JIRAUsername, Properties.Settings.Default.JIRAPassword));
            #endregion

            webRequest.Headers.Add("Authorization", authorization);

            dynamic d;
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
                        // this apparently works
                        sb.AppendFormat("{0} - Summary: {1}\r\n", issue.key , issue.fields.summary);
                    }
                    MessageBox.Show(sb.ToString(), "Dynamic is awesome!");
                }

            }



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

