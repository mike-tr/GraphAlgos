using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGraph {
    public class Node {
        public delegate void OnObjectDestroyed ();
        public OnObjectDestroyed OnObjectDestroyedCallback;
        // Start is called before the first frame update
        public int id { get; protected set; }
        public Vector3 pos;
        public Node (int id) {
            this.id = id;
        }

        public void DestroySelf () {
            if (OnObjectDestroyedCallback != null) {
                OnObjectDestroyedCallback.Invoke ();
            }
        }
    }
}
