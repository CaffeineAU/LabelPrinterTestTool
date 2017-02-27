using System;
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

        private List<string> _Labels;

        public List<string> Labels
        {
            get { return _Labels; }
            set { _Labels = value; }
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
            string output = string.Format(@"

                        ""{0}"" [label=<<TABLE BORDER=""1"" CELLBORDER=""0"" CELLSPACING=""0"" BGCOLOR=""{7}"">
                        <TR><TD ColSpan=""2"" PORT=""Title"" HREF=""http://jirapd.corp.resmed.org/browse/{0}""><font color=""blue"" point-size=""24"">{1}</font></TD></TR>

                        <TR><TD Align=""left"">Document Title</TD><TD Align=""left"">{2}</TD></TR>
                        <TR><TD Align=""left"">JIRA Ref</TD><TD Align=""left"">{0}</TD></TR>
                        <TR><TD Align=""left"">Component</TD><TD Align=""left"">{6}</TD></TR>
                        <TR><TD Align=""left"">Status</TD><TD Align=""left"">{3}</TD></TR>
                        <TR><TD Align=""left"">Assignee</TD><TD Align=""left"">{4}</TD></TR>
                        <TR><TD Align=""left"">Due Date</TD><TD Align=""left"">{5}</TD></TR>
                        </TABLE>>][tooltip = ""{0}""]
            ", JIRA_KEY, DocumentNumber, DocumentName, Status, Assignee, DueDate, Component, Status == "Open" ? "White:Red" : Status == "In Progress" ? "White:Orange" : "White:Green");

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
                                rankdir=""LR"";
                                node[shape = plaintext]"
                                );

            var results = from node in Nodes
                          group node by node.Component into componentgroup
                          select new { component = componentgroup.Key, nodes = componentgroup.ToList() };

            //var resultslabel = from node in Nodes
            //                   group node by node.Labels[0] into labelgroup
            //                   select new { label = labelgroup.Key, nodes = labelgroup.ToList() };


            int i = 0;

            foreach (var component in results)
            {
                int step = 4;
                int nodesperrow = 10;
                int nodewidth = 6;

                int x = 0;
                int y = 0;
                output.AppendFormat(@" 

                    subgraph cluster_{0} {{
                    style = filled;
                    color = lightgrey;
                    fontsize=40;
                    tooltip=""{1}"";
                    label =""{1}"";
                    ", i += step, component.component);

                foreach (var node in component.nodes)
                {
                    y = component.nodes.Last() == node ? (i - 1) + ((x / nodesperrow) * step)+1 : (i - 1) + ((x / nodesperrow) * step); // kick the last node up 1 notch, so that the cluster shows the title better
                    output.AppendFormat(@"{0}[pos = ""{1},{2}!""]", node.ToString(), (x % nodesperrow) * ( component.nodes.Count == 2 ? nodewidth *3: nodewidth), y); // if there's only 2 nodes in the cluster, stretch them out a bit to fit the cluster title in the middle
                    Console.WriteLine("Comp : {0}, x {1}, y{2}", node.Component + node.DocumentName, (x % nodesperrow) * nodewidth, y);
                    x++;
                }

                i += (x - 1) / nodesperrow > 0 ? step : 0; // did we have a second line of nodes? if so then move the 'y' value another step

                output.AppendFormat(@"}}");

            }

            /*foreach (var item in resultslabel)
            {
                if (item.label != "None")
                {
                    output.AppendFormat(@"
                        {{rank=same; ");
                    foreach (var issue in item.nodes)
                    {
                        output.AppendFormat(@" ""{0}"";", issue.JIRA_KEY);
                    }
                    output.AppendFormat(@"}}");

                }
            }*/

            foreach (MapNode node in Nodes)
            {
                foreach (string parent_id in node.Dependencies)
                {
                    output.AppendFormat(@"""{0}""-> ""{1}"";", node.JIRA_KEY, parent_id); //[label=""{0} to {1}""]
                }
            }

            output.AppendFormat("}}");

            System.Windows.Clipboard.SetText(output.ToString());

            File.WriteAllText("output.gv", output.ToString());

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = @"C:\bin\neato.exe";
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

            int timeout = 500;

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
