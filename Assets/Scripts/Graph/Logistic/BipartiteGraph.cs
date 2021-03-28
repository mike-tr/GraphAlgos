﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGraph {
    public class BipartiteGraph : AbGraph {
        public override Edge AddDEdge (int n1, int n2) {
            if (n1 % 2 == n2 % 2) {
                Debug.Log ("Cannot have connections between nodes in the same group");
                return null;
            }
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
            return (AddDEdge (n1, n2), AddDEdge (n2, n1));
        }
    }
}
