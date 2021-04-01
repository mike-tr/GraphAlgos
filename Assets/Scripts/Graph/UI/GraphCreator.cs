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
    // private List<EdgeUI> edgesUIs = new List<EdgeUI> ();
    // private List<VertexUI> vertexUIs = new List<VertexUI> ();
    public delegate void OnRestart ();
    public OnRestart OnRestartCallback;
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

    private bool IsDirected = false;

    private Transform edgeHolder, nodeHolder;

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
            graph = new Graph ();
        }

        cam = Camera.main;

        edgeHolder = new GameObject ("Edges").transform;
        nodeHolder = new GameObject ("Nodes").transform;
        edgeHolder.parent = this.transform;
        nodeHolder.parent = this.transform;

        edgeHolder.position = Vector3.zero;
        nodeHolder.position = Vector3.zero;
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
                    var nodeOb = Instantiate (vertexPrefab);
                    var node = nodeOb.Initialize (graph);
                    if (bipartite) {
                        nodeOb.SetColor (BColors[node.id % 2]);
                    }
                    nodeOb.SetPosition (cam.ScreenToWorldPoint (Input.mousePosition));
                    nodeOb.transform.parent = nodeHolder;

                    lastClick = 0;
                } else {
                    lastClick = Time.timeSinceLevelLoad + 0.5f;
                }
            }
        } else if (Input.GetKeyDown (KeyCode.V)) {
            SetVertexState ();
        } else if (Input.GetKeyDown (KeyCode.E)) {
            InputCreateEdge ();
        } else if (Input.GetKeyDown (KeyCode.D)) {
            DestroyTarget ();
        } else if (Input.GetKeyDown (KeyCode.H)) {
            Debug.Log ("Run hungarian algorithm");
            //RunHungarian ();
            LoadHungarianNext ();
        } else if (Input.GetKeyDown (KeyCode.J)) {
            Debug.Log ("Run hungarian algorithm");
            //RunHungarian ();
            StartCoroutine (ShowHungarian ());
        }
    }

    public void SetVertexState () {
        this.state = GraphCState.Node;
        UnfocusVertecies ();
        UnforcusEdge ();
    }

    public void InputCreateEdge () {
        this.state = GraphCState.Edge;
        if (vertexTarget[0] && vertexTarget[1]) {
            int id1 = vertexTarget[0].id;
            int id2 = vertexTarget[1].id;
            CreateEdge (graph, id1, id2, IsDirected);
        }
        UnfocusVertecies ();
        UnforcusEdge ();
    }

    private void CreateEdge (AbGraph targetGraph, int n1, int n2, bool directed) {
        if (targetGraph.CheckPossibleConnection (n1, n2)) {
            var edge = targetGraph.GetEdge (n1, n2);
            if (edge?.reference != null) {
                UnfocusVertecies ();
                UnforcusEdge ();
                return;
            }

            edge = targetGraph.GetEdge (n2, n1);
            if (edge?.reference != null) {
                edge.reference.AddEdge (n1, n2);
                UnfocusVertecies ();
                UnforcusEdge ();
                return;
            }
            var edgeUI = Instantiate (edgePrefab);
            edgeUI.Initialize (targetGraph, targetGraph.GetNode (n1), targetGraph.GetNode (n2), directed);
            edgeUI.transform.parent = edgeHolder;
        }
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

    public void LoadGraph (AbGraph loadGraph, bool overrideGraph) {
        UnfocusVertecies ();
        UnforcusEdge ();

        // this is not optimized but should work for now.
        bipartite = loadGraph is BipartiteGraph;

        Destroy (edgeHolder.gameObject);
        Destroy (nodeHolder.gameObject);

        edgeHolder = new GameObject ("Edges").transform;
        nodeHolder = new GameObject ("Nodes").transform;
        edgeHolder.parent = this.transform;
        nodeHolder.parent = this.transform;

        edgeHolder.position = Vector3.zero;
        nodeHolder.position = Vector3.zero;

        if (overrideGraph) {
            this.graph = loadGraph;
        }
        foreach (var node in loadGraph.GetNodes ()) {
            node.OnObjectDestroyedCallback = null;
            node.OnSetPosCallBack = null;

            var nodeOb = Instantiate (vertexPrefab);
            nodeOb.Initialize (loadGraph, node);
            if (bipartite) {
                var index = Mathf.Abs (node.id % 2);
                nodeOb.SetColor (BColors[index]);
            }

            nodeOb.transform.parent = nodeHolder;
        }

        foreach (var edge in loadGraph.GetAllEdges ()) {
            // Debug.Log (edge.src + " : " + edge.dest);
            edge.OnObjectDestroyedCallback = null;
            edge.reference = null;

            CreateEdge (loadGraph, edge.src, edge.dest, true);

            // Debug.Log (edge.reference);
        }

        // Debug.Log ("Graph loaded!");
    }

    public void Reload () {
        hungarian = null;
        hid = 0;
        LoadGraph (graph, false);
    }

    public void NewGeneralGraph () {
        IsDirected = false;
        bipartite = false;
        graph = new Graph ();
        Reload ();
        if (OnRestartCallback != null) {
            OnRestartCallback.Invoke ();
        }
    }

    public void NewBiptiteGraph () {
        IsDirected = false;
        bipartite = true;
        graph = new BipartiteGraph ();
        Reload ();
        if (OnRestartCallback != null) {
            OnRestartCallback.Invoke ();
        }
    }
    public void ChangeToDirected () {
        IsDirected = true;
    }

    Hungarian hungarian;
    public bool RunHungarian () {
        if (graph is BipartiteGraph && !graph.IsDirected) {
            hungarian = new Hungarian ((BipartiteGraph) graph);

            foreach (var edge in hungarian.GetMaxMatching ()) {
                // print ("PM : " + edge.src + " , " + edge.dest);
                ((EdgeUI) edge.reference).Focus ();
            }
            return true;
        } else {
            Debug.Log ("Cannot run hunarian D : " + graph.IsDirected);
            return false;
        }
    }

    public IEnumerator ShowHungarian () {
        Reload ();
        if (hungarian?.targetID == graph.GraphID || RunHungarian ()) {
            yield return null;
            foreach (var edge in hungarian.GetMaxMatching ()) {
                print ("PM : " + edge.src + " , " + edge.dest);
                ((EdgeUI) edge.reference).Focus ();
            }
        }
    }

    int hid = 0;
    public void LoadHungarianNext () {
        if (hungarian?.targetID == graph.GraphID || RunHungarian ()) {
            if (hid < hungarian.GetSteps ().Count) {
                Debug.Log ("Loading step : " + hid + " out of " + hungarian.GetSteps ().Count);
                LoadGraph (hungarian.GetSteps () [hid], false);
                hid++;
            } else {
                Debug.Log ("No more steps to show");
            }
        } else {
            Debug.Log ("Cannot run hunarian D : " + graph.IsDirected);
        }
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
