using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGraph {
    public abstract class AbGraph {
        private static int NextGraphID = 1;
        private int NextNodeId = 1;
        protected Dictionary<int, Node> nodes = new Dictionary<int, Node> ();
        protected Dictionary<int, Dictionary<int, Edge>> edges = new Dictionary<int, Dictionary<int, Edge>> ();
        protected Dictionary<int, List<int>> incomingEdges = new Dictionary<int, List<int>> ();
        public bool IsDirected {
            get;
            private set;
        } = false;
        // private Dictionary < (int, int), Edge > edges = new Dictionary < (int, int), Edge > ();

        public int GraphID {
            get;
            private set;
        } = NextGraphID++;
        /// <summary>
        /// A way to make sure only decendant's of graphs can Get, important data.
        /// /// </summary>
        public class ShareLock {
            protected ShareLock () { }
        }

        private class SInstance : ShareLock {
            public static ShareLock GetLock () {
                return new SInstance ();
            }
        }

        protected ShareLock innerShare = SInstance.GetLock ();

        public Dictionary<int, Node> GetDicNodes (ShareLock keyLock) {
            return nodes;
        }

        public Dictionary<int, Dictionary<int, Edge>> GetDicEdges (ShareLock keyLock) {
            return edges;
        }

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

        public Dictionary<int, Node>.ValueCollection GetNodes () {
            return nodes.Values;
        }

        public Node GetNode (int id) {
            if (NodeExist (id)) {
                return nodes[id];
            }
            return null;
        }

        public List<Edge> GetAllEdges () {
            List<Edge> all = new List<Edge> ();
            foreach (var item in edges.Values) {
                all.AddRange (item.Values);
            }
            return all;
        }

        /// <summary>
        /// Add node with specific ID, next automatic node will have to be of higher id!
        /// </summary>
        /// <param name="id"> </param>
        /// <returns> new Node with node.id = id </returns>
        public Node AddNode (int id) {
            if (NodeExist (id)) {
                throw new System.ArgumentException ("Node with id " + id + " already exist!");
            } else if (id == 0) {
                throw new System.ArgumentException ("Id 0, is invalid, and saved for other purposes.");
            }

            if (NextNodeId < id) {
                NextNodeId = id + 1;
            }
            // ADD
            var node = new Node (id);
            nodes.Add (id, node);
            edges.Add (id, new Dictionary<int, Edge> ());
            incomingEdges.Add (id, new List<int> ());
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
            node = new Node (node);

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

        public Dictionary<int, Edge> GetEdges (int nID) {
            if (NodeExist (nID)) {
                return edges[nID];
            }
            return null;
        }

        public Edge GetEdge (int n1, int n2) {
            if (NodeExist (n1)) {
                return InnerGetEdge (n1, n2);
            }
            return null;
        }

        public abstract (Edge, Edge) AddEdge (int n1, int n2);
        protected abstract Edge InnerAddDirectedEdge (int n1, int n2);
        public Edge AddDirectedEdge (int n1, int n2) {

            var edge = GetEdge (n1, n2);
            if (edge == null) {
                edge = InnerAddDirectedEdge (n1, n2);
                if (edge != null) {
                    IsDirected = true;
                }
            }
            return edge;
        }
    }
}
