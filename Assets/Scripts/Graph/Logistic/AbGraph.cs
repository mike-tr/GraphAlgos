using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGraph {
    public abstract class AbGraph {
        private int NextNodeId = 1;
        protected Dictionary<int, Node> nodes = new Dictionary<int, Node> ();
        protected Dictionary<int, Dictionary<int, Edge>> edges = new Dictionary<int, Dictionary<int, Edge>> ();
        protected Dictionary<int, List<int>> incomingEdges = new Dictionary<int, List<int>> ();
        // private Dictionary < (int, int), Edge > edges = new Dictionary < (int, int), Edge > ();

        /// <summary>
        /// Add a new node with a unique id, ud us unique only per graph!
        /// </summary>
        /// <returns> new Node </returns>
        public virtual Node AddNode () {
            // ADD
            var node = new Node (NextNodeId);
            nodes.Add (NextNodeId, node);
            edges.Add (NextNodeId, new Dictionary<int, Edge> ());
            incomingEdges.Add (NextNodeId, new List<int> ());
            NextNodeId++;

            return node;
        }

        /// <summary>
        /// Add existing Node to the graph, throws error if not with the same id already exist.
        /// </summary>
        /// <param name="node"></param>
        public virtual void AddNode (Node node) {
            if (nodes.ContainsKey (node.id)) {
                throw new System.ArgumentException ("Node with id " + node.id + " already exist!");
            }
            var id = node.id;
            nodes.Add (id, node);
            edges.Add (id, new Dictionary<int, Edge> ());
            incomingEdges.Add (id, new List<int> ());
            if (NextNodeId <= id) {
                NextNodeId = id + 1;
            }
        }

        /// <summary>
        /// Remove any given node from graph and all its Incoming and outgoing edges, 
        /// if the node doesnt exist do nothing.
        /// </summary>
        /// <param name="nID"></param>
        public void RemoveNode (int nID) {
            if (nodes.ContainsKey (nID)) {
                nodes[nID].DestroySelf ();
                nodes.Remove (nID);

                foreach (int n2 in edges[nID].Keys) {
                    RemoveEdge (nID, n2);
                }

                foreach (int n2 in incomingEdges[nID].ToArray ()) {
                    RemoveDEdge (n2, nID);
                }
            }
        }

        public virtual bool CheckPossibleConnection (int n1, int n2) {
            return NodeExist (n1) && NodeExist (n2);
        }

        public void RemoveEdge (int n1, int n2) {
            RemoveDEdge (n1, n2);
            RemoveDEdge (n2, n1);
        }

        public bool NodeExist (int n) {
            return nodes.ContainsKey (n);
        }

        /// <summary>
        /// Get Edge UNSAFE
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <returns></returns>
        protected Edge InnerGetEdge (int n1, int n2) {
            if (edges[n1].ContainsKey (n2)) {
                return edges[n1][n2];
            }
            return null;
        }

        public void RemoveDEdge (int n1, int n2) {
            if (edges.TryGetValue (n1, out var nedges)) {
                if (nedges.ContainsKey (n2)) {
                    Edge ed = nedges[n2];
                    ed.DestroySelf ();
                    nedges.Remove (n2);

                    incomingEdges[n2].Remove (n1);
                }
            }
        }

        public Edge GetEdge (int n1, int n2) {
            if (NodeExist (n1)) {
                return InnerGetEdge (n1, n2);
            }
            return null;
        }

        public abstract (Edge, Edge) AddEdge (int n1, int n2);
        public abstract Edge AddDEdge (int n1, int n2);
    }
}
