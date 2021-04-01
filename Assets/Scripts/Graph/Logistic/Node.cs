using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGraph {
    public class Node {
        public const int EmptyTag = -1;
        public delegate void OnObjectDestroyed ();
        public OnObjectDestroyed OnObjectDestroyedCallback;

        public delegate void OnSetPos ();
        public OnSetPos OnSetPosCallBack;
        public int tag;
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
            this.tag = EmptyTag;
        }

        public Node (Node node) {
            this.id = node.id;
            this.tag = EmptyTag;
            this._pos = node._pos;
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
            return obj.GetHashCode () == GetHashCode ();
        }

        // override object.GetHashCode
        public override int GetHashCode () {
            return id;
        }
    }
}
