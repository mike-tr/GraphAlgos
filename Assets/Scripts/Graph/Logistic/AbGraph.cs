using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGraph {
    public abstract class AbGraph {
        private int NextNodeId = 0;
        private Dictionary<int, Node> nodes = new Dictionary<int, Node> ();
        private Dictionary < (int, int), Edge > edges = new Dictionary < (int, int), Edge > ();

        public void AddNode () {
            nodes.Add (NextNodeId, new Node (NextNodeId));
            NextNodeId++;
        }

        public void RemoveNode (int nID) {
            if (nodes.ContainsKey (nID)) {

            }
        }

        public abstract void AddEdge (int n1, int n2);
        public abstract void AddDEdge (int n1, int n2);
    }
}
