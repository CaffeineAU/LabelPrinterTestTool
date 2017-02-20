using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DocumentationMapper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            GetDocs();
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }


        private void GetDocs()
        {

            Properties.Settings.Default.Save();

            string project = "MTE";
            int num_results = 1000;

            //string str_status = @"""" + String.Join(@""", """, IssueStatuses) + @"""";

            string[] IssueFields = new string[] { };

            string str_fields = String.Join(", ", IssueFields);

            //http://jirapd.corp.resmed.org/rest/api/2/search?jql=(project=MTE%20AND%20issuetype=%22Document%20Request%22)&startAt=0&maxResults=100&fields=key,status

            // key,status,duedate,component,asignee

            string request = string.Format(@"{0}search?jql=(project={1} AND issuetype='Document Request')&startAt=0&maxResults={2}&fields={3} ",
                Properties.Settings.Default.JIRA_API,
                project,
                num_results, 
                str_fields
                );

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

                        foreach (var issue in d.issues)
                        {
                            Console.WriteLine(issue.key);

                            //for each issue, write a GraphViz node to the text file
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
