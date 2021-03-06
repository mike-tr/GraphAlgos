using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGraph {
    public class Graph : AbGraph {
        protected override Edge InnerAddDirectedEdge (int n1, int n2) {
            if (NodeExist (n1) && NodeExist (n2)) {
                Edge E = InnerGetEdge (n1, n2);
                if (E == null) {
                    E = new Edge (n1, n2);
                    edges[n1].Add (n2, E);
                    incomingEdges[n2].Add (n1);
                }
                return E;
            }
            return null;
        }

        public override (Edge, Edge) AddEdge (int n1, int n2) {
            return (InnerAddDirectedEdge (n1, n2), InnerAddDirectedEdge (n2, n1));
        }
    }
}
