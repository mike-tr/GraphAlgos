using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGraph {

    public enum HungarianState {
        OnGoing,
        Done,
    }
    /// <summary>
    /// A Hungarian version of Biptite Graph, used to find max mathing,
    /// This is a graph version for Algorithm use only!
    /// </summary>
    public class HBipartiteGraph : BipartiteGraph {
        private const int SINK = -100;
        private const int SOURCE = -101;
        List<Edge> M = new List<Edge> ();
        List<Edge> NotM = new List<Edge> ();

        List<Edge> MOriginal = new List<Edge> ();

        private Node source, sink;
        BipartiteGraph originalGraph;

        public bool HUpdated {
            get;
            private set;
        } = false;
        public bool hasChanged = false;

        public HBipartiteGraph (BipartiteGraph graph, List<Edge> selected) {
            MOriginal = new List<Edge> (selected);
            originalGraph = graph;
            if (graph.IsDirected) {
                throw new System.ArgumentException ("Given graph cannot be Directed graph");
            }

            HashSet<int> AorB = new HashSet<int> ();
            foreach (var edge in selected) {
                AorB.Add (edge.src);
                AorB.Add (edge.dest);
            }

            source = this.AddNode (SOURCE);
            source.position = new Vector3 (-6, 0, 0);
            sink = this.AddNode (SINK);
            sink.position = new Vector3 (9, 0, 0);
            foreach (var node in graph.GetDicNodes (innerShare).Values) {
                this.AddNode (node);

                if (AorB.Contains (node.id)) {
                    continue;
                }

                if (node.id % 2 == 0) {
                    InnerAddDirectedEdge (SOURCE, node.id);
                } else {
                    InnerAddDirectedEdge (node.id, SINK);
                }
            }

            foreach (var edge in graph.GetAllEdges ()) {
                // we only care about edges from even nodes!
                // note this algorithm should work on UNDIRECTED graphs, so both edges should exist!
                if (edge.src % 2 == 1) {
                    continue;
                }

                if (selected.Contains (edge)) {
                    M.Add (InnerAddDirectedEdge (edge.dest, edge.src));
                } else {
                    NotM.Add (InnerAddDirectedEdge (edge.src, edge.dest));
                }
            }
            UpdateMAugmentedPath ();
        }

        /// <summary>
        /// After we run BFS check whether there was an M-Augmenting path or not.
        /// </summary>
        /// <returns></returns>
        public bool NoAugmentation () {
            return HUpdated && !hasChanged;
        }

        /// <summary>
        /// Run BFS and find M-Augmenting path.
        /// </summary>
        private void UpdateMAugmentedPath () {
            if (HUpdated) {
                return;
            }
            HUpdated = true;

            List<Node> open = new List<Node> ();
            open.Add (source);

            // foreach (var node in nodes.Values) {
            //     node.tag = 0;
            // }

            while (open.Count > 0) {
                Node current = open[0];
                open.RemoveAt (0);

                foreach (var ni in GetEdges (current.id).Keys) {
                    // Debug.Log (current.id + " : " + ni);
                    Node n = GetNode (ni);
                    if (n.tag == Node.EmptyTag) {
                        if (n.id == SINK) {
                            UpdateM (current);
                            return;
                        }
                        n.tag = current.id;
                        open.Add (n);
                    }
                }
            }
        }

        private void UpdateM (Node end) {
            Node current = end;
            while (current.tag != SOURCE) {

                // Debug.Log (current.id + " ," + current.tag);
                Node prev = GetNode (current.tag);
                Edge e = GetEdge (prev.id, current.id);

                if (e.src % 2 == 0) {
                    MOriginal.Add (originalGraph.GetEdge (e.src, e.dest));
                } else {
                    MOriginal.Remove (originalGraph.GetEdge (e.dest, e.src));
                }

                current = prev;
            }

            hasChanged = true;
        }

        public List<Edge> GetNewM () {
            return MOriginal;
        }
    }
}
