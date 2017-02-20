using System;
using System.Collections.Generic;
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
            string output = string.Format(@"""{0}""[label = ""<f0> {0}| <f1>{1}| <f3>{2} | <f4>{3}| <f5>{4}| <f6>{5}| <f7>{6}"" shape = ""record"" ];", JIRA_KEY, DocumentNumber, DocumentName, Status, Assignee, DueDate, Component);

            foreach(string parent_id in Dependencies)
            {
                output += string.Format(@"""{0}"":f0-> ""{1}"":f0[id = 0];", this.JIRA_KEY, parent_id);
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

            string output = "";

            output += @"digraph g {graph[rankdir = ""LR""];";

            output += @"node [fontsize = ""16"" shape = ""ellipse""]; ";

            foreach(MapNode mn in Nodes)
            {
                

                if(mn.JIRA_KEY == "MTE-465")
                {
                    mn.Dependencies.Add("MTE-609");
                }

                output += mn.ToString();
            }

            output += "}";

            File.WriteAllText("output.gv",output);

            System.Diagnostics.Process.Start(@"C:\bin\dot.exe", "-Tsvg output.gv -o DocMap.svg");

            System.Diagnostics.Process.Start(@"DocMap.svg");

            int yyy = 0;
        }




    }
}
