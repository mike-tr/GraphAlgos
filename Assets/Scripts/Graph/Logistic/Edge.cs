using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGraph {
    public class Edge {
        // Start is called before the first frame update

        public delegate void OnObjectDestroyed ();
        public OnObjectDestroyed OnObjectDestroyedCallback;
        public IEdgeReference reference;
        public int src { get; private set; }
        public int dest { get; private set; }
        public Edge (int src, int dest) {
            this.src = src;
            this.dest = dest;
        }

        public void DestroySelf () {
            if (OnObjectDestroyedCallback != null) {
                OnObjectDestroyedCallback.Invoke ();
            }
        }

        public override bool Equals (object obj) {
            if (obj == null || GetType () != obj.GetType ()) {
                return false;
            }
            Edge other = (Edge) obj;
            return other.src == src && other.dest == dest;
        }

        // override object.GetHashCode
        public override int GetHashCode () {
            return src * 2 * dest + (dest + 1) * 2 + src;
        }
    }
}
