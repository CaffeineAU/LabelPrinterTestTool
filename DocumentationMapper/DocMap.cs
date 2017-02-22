﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DocumentationMapper
{
    public class MapNode
    {

        public MapNode()
        {
            Dependencies = new List<string>();
        }

        private string _key;

        public string JIRA_KEY
        {
            get { return _key; }
            set { _key = value; }
        }

        private string _doc_num;

        public string DocumentNumber
        {
            get { return _doc_num; }
            set { _doc_num = value; }
        }

        private string _doc_name;

        public string DocumentName
        {
            get { return _doc_name; }
            set { _doc_name = value; }
        }

        private string _status;

        public string Status
        {
            get { return _status; }
            set { _status = value; }
        }

        private string _assignee;

        public string Assignee
        {
            get { return _assignee; }
            set { _assignee = value; }
        }

        private List<string> _dependencies;

        public List<string> Dependencies
        {
            get { return _dependencies; }
            set { _dependencies = value; }
        }

        private string _duedate;

        public string DueDate
        {
            get { return _duedate; }
            set { _duedate = value; }
        }

        private string _component;

        public string Component
        {
            get { return _component; }
            set { _component = value; }
        }







        public override string ToString()
        {
            //string output = string.Format(@"""{0}""[label = ""<f0> {0}| <f1>{1}| <f3>{2} | <f4>{3}| <f5>{4}| <f6>{5}| <f7>{6}"" shape = ""record"" ];", JIRA_KEY, DocumentNumber, DocumentName, Status, Assignee, DueDate, Component);

            string output = string.Format(@"

            ""{0}"" [label=<<TABLE BORDER=""1"" CELLBORDER=""0"" CELLSPACING=""0"" BGCOLOR=""{7}"">
            <TR><TD ColSpan=""2"" PORT=""Title"" HREF=""http://jirapd.corp.resmed.org/browse/{0}""><font color=""blue"" point-size=""24"">{0}</font></TD></TR>
            <TR><TD Align=""left"">Document Number:</TD><TD Align=""left"">{1}</TD></TR>
            <TR><TD Align=""left"">Document Title</TD><TD Align=""left"">{2}</TD></TR>
            <TR><TD Align=""left"">Component</TD><TD Align=""left"">{6}</TD></TR>
            <TR><TD Align=""left"">Status</TD><TD Align=""left"">{3}</TD></TR>
            <TR><TD Align=""left"">Assignee</TD><TD Align=""left"">{4}</TD></TR>
            <TR><TD Align=""left"">Due Date</TD><TD Align=""left"">{5}</TD></TR>
            </TABLE>>][tooltip = ""{0}""]
", JIRA_KEY, DocumentNumber, DocumentName, Status, Assignee, DueDate, Component, Status == "Open" ? "White:Red" : Status == "In Progress" ? "White:Orange" : "White:Green");

            foreach (string parent_id in Dependencies)
            {
                output += string.Format(@"""{0}"":Title-> ""{1}"":Title;", this.JIRA_KEY, parent_id);
            }
            

            return output;
        }
    }


    public class DocMap
    {
        public DocMap()
        {

            Nodes = new List<MapNode>();
        }

        private List<MapNode> _Nodes;

        public List<MapNode> Nodes
        {
            get { return _Nodes; }
            set { _Nodes = value; }
        }


        public void Export()
        {
            //this is where we export the structure to a text file and then run the graph viz tool

            StringBuilder output = new StringBuilder(@"digraph G {
                                graph[fontname = ""Segoe UI"", pad=""0.5"", nodesep=""1"", ranksep=""2""];
                                node[fontname = ""Segoe UI""];
                                edge[fontname = ""Segoe UI""];
                                rankdir=""LR""
                                node[shape = plaintext]");

            Dictionary<String, List<MapNode>> groupedMapNodes = new Dictionary<string, List<MapNode>>();

            var results = from node in Nodes
                          group node by node.Component into componentgroup
                          select new { component = componentgroup.Key, nodes = componentgroup.ToList() };

            int i = 0;
            foreach (var component in results)
            {
                
                output.AppendFormat(@" subgraph cluster_{0} {{
                    style = filled;
                    color = lightgrey;
                    fontsize=40;
                    tooltip=""{1}"";
                    label =""{1}"";", i++, component.component);

                foreach (var node in component.nodes)
                {
                    //output += string.Format(@" ""{0}""-> ", node.JIRA_KEY);
                    output.AppendFormat("{0}", node.ToString());
                }

                output.AppendFormat(@"}}");

            }

            output.AppendFormat("}}");

            System.Windows.Clipboard.SetText(output.ToString());

            File.WriteAllText("output.gv", output.ToString());

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = @"C:\bin\dot.exe";
            startInfo.Arguments = "-Tsvg output.gv -o DocMap.svg";
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;

            Process processTemp = new Process();
            processTemp.StartInfo = startInfo;
            processTemp.EnableRaisingEvents = true;
            try
            {
                File.Delete(@"DocMap.svg");
                processTemp.Start();
            }
            catch (Exception e)
            {
                throw;
            }

            int timeout = 50;

            while (!File.Exists(@"DocMap.svg") && timeout-- > 0)
            {
                Thread.Sleep(100);
            }

            if (File.Exists(@"DocMap.svg"))
            {
                System.Diagnostics.Process.Start(@"DocMap.svg");
            }
        }




    }
}
