using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGraph {
    public class Node {
        public delegate void OnObjectDestroyed ();
        public OnObjectDestroyed OnObjectDestroyedCallback;

        public delegate void OnSetPos ();
        public OnSetPos OnSetPosCallBack;

        // Start is called before the first frame update
        public int id { get; protected set; }

        private Vector3 _pos;
        public Vector3 position {
            get { return _pos; }
            set {
                _pos = value;
                if (OnSetPosCallBack != null) {
                    OnSetPosCallBack.Invoke ();
                }
            }
        }
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
