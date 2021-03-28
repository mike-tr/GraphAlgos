using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGraph {
    public class Node {
        // Start is called before the first frame update
        private int id;
        public Node (int id) {
            this.id = id;
        }

        public virtual void Destroy () { }
    }
}
