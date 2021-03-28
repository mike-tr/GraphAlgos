using System.Collections;
using System.Collections.Generic;
using MGraph;
using UnityEngine;

public enum GraphCState {
    Node,
    Edge,
}
public class GraphCreator : MonoBehaviour {
    public static GraphCreator instance;
    // Start is called before the first frame update
    public bool bipartite = true;

    private Camera cam;

    public AbGraph graph;

    public VertexUI vertex1, vertex2;
    public EdgeUI edge;

    [SerializeField] private GraphCState state;
    void Awake () {
        if (instance == null) {
            instance = this;
        } else {
            Destroy (this);
        }

        if (bipartite) {
            graph = new BipartiteGraph ();
        } else {
            graph = new DGraph ();
        }

        cam = Camera.main;
    }

    // Update is called once per frame
    void Update () {

    }

    public void SetVertexState () {
        this.state = GraphCState.Node;
    }

    public void SetEdgeState () {
        this.state = GraphCState.Edge;
    }

    public void DestroyTarget () {
        if (this.state == GraphCState.Edge) {
            if (edge) {
                edge.DestroySelf ();
                edge = null;
            }
        } else if (vertex1) {
            vertex1.DestroySelf ();
            vertex1 = null;
        }
    }
}
