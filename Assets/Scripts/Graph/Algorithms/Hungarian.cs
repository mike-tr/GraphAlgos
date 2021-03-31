using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGraph {
    /// <summary>
    /// Find max matching
    /// 
    /// Set M = empty set
    /// While there is M-Augmenting path in G
    ///     Let P be the M-Augmenting path
    ///     Augment M along P
    /// return M
    /// 
    /// This class will Caclculate the max matching group,
    /// as well as "save" all the steps in between untill the loop finished,
    /// this is in order to "show" the steps of the algorithm
    /// </summary>
    public class Hungarian {

        private List<BipartiteGraph> steps = new List<BipartiteGraph> ();
        private List<Edge> matching = new List<Edge> ();
        private BipartiteGraph graph;
        private BipartiteGraph currentIter;
        public Hungarian (BipartiteGraph graph) {
            this.graph = graph;
        }

        /// <summary>
        /// Denote Group A would be 0 mod 2 nodes,
        /// while Group B would be 1 mod 2 nodes.
        /// </summary>
        private void FindMaxMatching () {

        }
    }
}
