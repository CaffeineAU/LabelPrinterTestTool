using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentationMapper
{
    public class MapNode
    {
        public string Jira_ID { get; set; }
        public string Doc_Num { get; set; }
        public string Status { get; set; }
        public string Owner { get; set; }

        private List<string> dependencies = new List<string>();

        public List<string> Dependencies
        {
            get { return dependencies; }
            set { dependencies = value; }
        }



    }


    public class DocMap
    {
        public DocMap()
        {

            Nodes = new List<MapNode>();

            MapNode m = new MapNode { Jira_ID = "1234", Doc_Num = "4321", Dependencies = new List<string> { "hello", "is", "it", "me", "you're", "looking", "for?" } };
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
        }
    }
}
