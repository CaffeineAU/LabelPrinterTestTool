using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentationMapper
{
    public class MapNode
    {
        public MapNode(string jira_id, string doc_num, string doc_name, string status, string owner, List<string> dependencies)
        {

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
        }
    }
}
