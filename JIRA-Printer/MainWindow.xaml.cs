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


            var webRequest = WebRequest.Create("http://jirapd.corp.resmed.org/rest/api/2/search?jql=assignee%20in%20(dtaprell)");

            #region
            string authorization = "Basic " + Base64Encode("username:password");
            #endregion

            webRequest.Headers.Add("Authorization", authorization);

            /*using (Stream newStream = webRequest.GetRequestStream())
            {
                newStream.Write(ToBytes(requestPayload), 0, requestPayload.Length);
                newStream.Flush();
            }*/

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
