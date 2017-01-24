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
using System.Net;
using System.IO;

namespace JIRA_Printer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }


        public MainWindow()
        {
            InitializeComponent();


            var webRequest = WebRequest.Create("http://jirapd.corp.resmed.org/rest/api/2/search?jql=project=MTE%20AND%20(status%20in(%22Open%22,%22In%20Progress%22))&fields=key,status,summary,progress,duedate,assignee&maxResults=100");

            // do this once, to create the .settings file
            Properties.Settings.Default.JIRAUsername = Properties.Settings.Default.JIRAUsername;
            Properties.Settings.Default.Save();

            #region
            string authorization = "Basic " + Base64Encode(String.Format("{0}:{1}", Properties.Settings.Default.JIRAUsername, Properties.Settings.Default.JIRAPassword));
            #endregion

            webRequest.Headers.Add("Authorization", authorization);


            var response = (HttpWebResponse)webRequest.GetResponse();

            using (Stream responseStream = response.GetResponseStream())
            {
                if (responseStream != null)
                {

                    //responseStream.CopyTo(memoryStream);
                    String responseString = new StreamReader(responseStream).ReadToEnd();

                    Clipboard.SetText(responseString);
                    //return new JavaScriptSerializer().Deserialize<SupportedVoices>(responseString);
                }

            }


        }
    }
}

