using System.Collections;
using System.Collections.Generic;

namespace MGraph {
    public class HBiptiteGraph : BipartiteGraph {
        private const int SINK = -100;
        private const int SOURCE = -101;
        List<Edge> M = new List<Edge> ();
        List<Edge> NotM = new List<Edge> ();

        // List<Node> Anm = new List<Node> ();
        // List<Node> Bnm = new List<Node> ();

        private Node source, sink;
        BipartiteGraph originalGraph;
        public HBiptiteGraph (BipartiteGraph graph, List<Edge> selected) {
            originalGraph = graph;
            if (graph.IsDirected) {
                throw new System.ArgumentException ("Given graph cannot be Directed graph");
            }
            nodes = new Dictionary<int, Node> (graph.GetDicNodes (innerShare));

            source = graph.AddNode (SOURCE);
            sink = graph.AddNode (SINK);
            foreach (var edge in graph.GetAllEdges ()) {
                // we only care about edges from even nodes!
                // note this algorithm should work on UNDIRECTED graphs, so both edges should exist!
                if (edge.src % 2 == 1) {
                    return;
                }

                if (selected.Contains (edge)) {
                    M.Add (InnerAddDirectedEdge (edge.dest, edge.src));
                } else {
                    NotM.Add (InnerAddDirectedEdge (edge.src, edge.dest));
                    InnerAddDirectedEdge (SOURCE, edge.src);
                    InnerAddDirectedEdge (edge.dest, SINK);
                }
            }
        }

    }
}
