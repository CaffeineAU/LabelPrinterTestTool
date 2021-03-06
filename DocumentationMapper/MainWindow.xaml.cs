﻿using System;
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

            if (Properties.Settings.Default.JIRAUsername == null)
            {
                Properties.Settings.Default.JIRAUsername = "Hello";
                Properties.Settings.Default.Save();
            }



            GetDocs();

            this.Close();
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
            int num_results = 100;

            //string str_status = @"""" + String.Join(@""", """, IssueStatuses) + @"""";

            string[] IssueFields = new string[] { "key", "summary", "status", "duedate", "components", "assignee", "issuelinks", "labels" };

            string str_fields = String.Join(", ", IssueFields);

            //http://jirapd.corp.resmed.org/rest/api/2/search?jql=(project=MTE%20AND%20issuetype=%22Document%20Request%22)&startAt=0&maxResults=100&fields=key,status

            // 


            string request = string.Format(@"{0}search?jql=(project={1} AND issuetype='Document Request')&startAt=0&maxResults={2}&fields={3} ",
                Properties.Settings.Default.JIRA_API,
                project,
                num_results,
                str_fields
                );

            Clipboard.SetText(request);

            var webRequest = WebRequest.Create(request);



            #region Authorisation
            string authorization = "Basic " + Base64Encode(String.Format("{0}:{1}", Properties.Settings.Default.JIRAUsername, Properties.Settings.Default.JIRAPassword));
            #endregion

            webRequest.Headers.Add("Authorization", authorization);



            DocMap dm = new DocMap();

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
                        int index = 0;

                        foreach (var issue in d.issues)
                        {
                            Console.WriteLine(issue.key);



                            string summary = issue.fields.summary ?? "DocNum DocName";

                            List<string> split = summary.Split(':').ToList();

                            //hack 
                            while (split.Count <2 )
                            {
                                split.Insert(0, "No Doc #");
                            }

                            dynamic labels = issue.fields.labels;
                            List<String> issuelabels = new List<string>();

                            foreach (var item in labels)
                            {
                                issuelabels.Add(item);
                            }
                            if (issuelabels.Count == 0)
                            {
                                issuelabels.Add("None");
                            }
                            MapNode mn = new MapNode
                            {
                                JIRA_KEY = issue.key ?? "None",
                                DocumentNumber = split[0],
                                DocumentName = split[1],
                                Component = issue.fields.components[0].name ?? "None",
                                Status = issue.fields.status.name,
                                Assignee = issue.fields.assignee != null ? issue.fields.assignee.displayName ?? "None" : "None",
                                DueDate = DateTime.Parse(issue.fields.duedate ?? DateTime.Now.ToString()).ToString("dd MMM yyyy") ?? "None",
                                Labels = issuelabels,
                            };

                            dynamic test = issue.fields.issuelinks;

                            foreach (var link in test)
                            {
                                if (link.outwardIssue != null)
                                {
                                    mn.Dependencies.Add(link.outwardIssue.key);
                                    Console.WriteLine("Linked {0} to {1}", issue.key, link.outwardIssue.key);
                                }
                            }





                            dm.Nodes.Add(mn);


                            //for each issue, write a GraphViz node to the text file
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }



            dm.Export();
        }

    }
}
