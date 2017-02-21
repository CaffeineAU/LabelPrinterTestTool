using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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

            ""{0}"" [label=<<TABLE BORDER=""1"" CELLBORDER=""0"" CELLSPACING=""0"">
            <TR><TD ColSpan=""2"" PORT=""Title"" HREF=""http://jirapd.corp.resmed.org/browse/{0}""><font color=""blue"" point-size=""24"">{0}</font></TD></TR>
            <TR><TD Align=""left"">Document Number:</TD><TD Align=""left"">{1}</TD></TR>
            <TR><TD Align=""left"">Document Title</TD><TD Align=""left"">{2}</TD></TR>
            <TR><TD Align=""left"">Component</TD><TD Align=""left"">{6}</TD></TR>
            <TR><TD Align=""left"">Status</TD><TD Align=""left"">{3}</TD></TR>
            <TR><TD Align=""left"">Assignee</TD><TD Align=""left"">{4}</TD></TR>
            <TR><TD Align=""left"">Due Date</TD><TD Align=""left"">{5}</TD></TR>
            </TABLE>>];

", JIRA_KEY, DocumentNumber, DocumentName, Status, Assignee, DueDate, Component);

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

            string output = @"digraph G {
                                graph[fontname = ""Segoe UI""];
                                node[fontname = ""Segoe UI""];
                                edge[fontname = ""Segoe UI""];
                                node[shape = plaintext]";

            Dictionary<String, List<MapNode>> groupedMapNodes = new Dictionary<string, List<MapNode>>();

            var results = from node in Nodes
                          group node.JIRA_KEY by node.Component into componentgroup
                          select new { component = componentgroup.Key, nodes = componentgroup.ToList() };

            // That should work, but all of the components are null....



            foreach(MapNode mn in Nodes)
            {
                output += mn.ToString();
            }

            output += "}";

            File.WriteAllText("output.gv",output);

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
                processTemp.Start();
            }
            catch (Exception e)
            {
                throw;
            }

            System.Diagnostics.Process.Start(@"DocMap.svg");
        }




    }
}
