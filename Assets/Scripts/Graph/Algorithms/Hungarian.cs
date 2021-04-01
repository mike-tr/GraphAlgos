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

        private List<HBipartiteGraph> steps = new List<HBipartiteGraph> ();
        private List<Edge> MaxMatching = new List<Edge> ();
        private BipartiteGraph graph;
        private HBipartiteGraph currentIter;

        public int targetID {
            get {
                return graph.GraphID;
            }
        }

        /// <summary>
        /// Get a graph and run the Hungarian method on the given graph,
        /// Expects Bipartite undirected graph.
        /// </summary>
        /// <param name="graph"></param>
        public Hungarian (BipartiteGraph graph) {
            if (graph.IsDirected) {
                Debug.Log ("the given graph is Directed cannot run the algorithm!");
                return;
            }
            this.graph = graph;
            FindMaxMatching ();
        }
        private void FindMaxMatching () {
            currentIter = new HBipartiteGraph (graph, MaxMatching);
            int i = 0;
            while (currentIter.NoAugmentation () == false && i < 10) {
                MaxMatching = currentIter.GetNewM ();
                steps.Add (currentIter);
                currentIter = new HBipartiteGraph (graph, MaxMatching);
                i++;
            }

            if (i < 10) {
                steps.Add (currentIter);
            }
        }

        public List<Edge> GetMaxMatching () {
            return MaxMatching;
        }

        public List<HBipartiteGraph> GetSteps () {
            return steps;
        }
    }
}
