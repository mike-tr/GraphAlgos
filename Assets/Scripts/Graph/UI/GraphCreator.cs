using System.Collections;
using System.Collections.Generic;
using MGraph;
using UnityEngine;
using UnityEngine.EventSystems;
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

    public VertexUI[] vertexTarget = new VertexUI[2];
    private VertexUI lastVertex;
    public EdgeUI edgeTarget;

    public Color[] BColors = new Color[2];

    [SerializeField] private GraphCState state;

    [SerializeField] private VertexUI vertexPrefab;
    [SerializeField] private EdgeUI edgePrefab;

    float lastClick = 0;
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
        if (Input.GetKeyDown (KeyCode.Mouse1)) {
            UnfocusVertecies ();
            UnforcusEdge ();
        } else if (Input.GetKeyDown (KeyCode.Mouse0)) {
            UnforcusEdge ();
            if (!EventSystem.current.IsPointerOverGameObject ()) {
                UnfocusVertecies ();
                if (lastClick > Time.timeSinceLevelLoad) {
                    //Debug.Log ("???");
                    var nodeOb = Instantiate (vertexPrefab);
                    var node = nodeOb.Initialize (graph);
                    if (bipartite) {
                        nodeOb.SetColor (BColors[node.id % 2]);
                    }
                    nodeOb.SetPosition (cam.ScreenToWorldPoint (Input.mousePosition));
                    lastClick = 0;
                } else {
                    lastClick = Time.timeSinceLevelLoad + 0.5f;
                }
            }
        } else if (Input.GetKeyDown (KeyCode.V)) {
            SetVertexState ();
        } else if (Input.GetKeyDown (KeyCode.E)) {
            SetEdgeState ();
        } else if (Input.GetKeyDown (KeyCode.D)) {
            DestroyTarget ();
        }
    }

    public void SetVertexState () {
        this.state = GraphCState.Node;
        UnfocusVertecies ();
        UnforcusEdge ();
    }

    public void SetEdgeState () {
        this.state = GraphCState.Edge;
        if (vertexTarget[0] && vertexTarget[1]) {
            int id1 = vertexTarget[0].id;
            int id2 = vertexTarget[1].id;
            if (graph.CheckPossibleConnection (id1, id2)) {
                var edge = graph.GetEdge (id1, id2);
                if (edge?.reference != null) {
                    UnfocusVertecies ();
                    UnforcusEdge ();
                    return;
                }

                edge = graph.GetEdge (id2, id1);
                if (edge?.reference != null) {
                    edge.reference.AddEdge (id1, id2);
                    UnfocusVertecies ();
                    UnforcusEdge ();
                    return;
                }
                var edgeUI = Instantiate (edgePrefab);
                edgeUI.Initialize (graph, vertexTarget[0], vertexTarget[1], true);
            }
        }
        UnfocusVertecies ();
        UnforcusEdge ();
    }

    public void UnfocusVertecies () {
        vertexTarget[0]?.Unfocus ();
        vertexTarget[1]?.Unfocus ();
        vertexTarget[0] = null;
        vertexTarget[1] = null;
        secondaryTarget = false;
        lastVertex = null;
    }

    public void UnforcusEdge () {
        edgeTarget?.Unfocus ();
        edgeTarget = null;
    }

    bool secondaryTarget = false;
    public void FocusVertex (VertexUI vertex) {
        UnforcusEdge ();
        lastVertex = vertex;
        if (secondaryTarget) {
            UnfocusVertecies ();
            vertexTarget[0] = vertex;
            secondaryTarget = false;

        } else if (vertexTarget[0]) {
            vertexTarget[1] = vertex;
            secondaryTarget = true;
        } else {
            vertexTarget[0] = vertex;
        }
        vertex.Focus ();
    }

    public void FocusEdge (EdgeUI edge) {
        UnfocusVertecies ();
        edgeTarget?.Unfocus ();
        this.edgeTarget = edge;
        edge.Focus ();
    }

    public void DestroyTarget () {
        if (edgeTarget) {
            edgeTarget.DestroySelf ();
            UnforcusEdge ();
            //edgeTarget = null;
        }

        if (lastVertex) {
            lastVertex.DestroySelf ();
            UnfocusVertecies ();
        }
    }
}
