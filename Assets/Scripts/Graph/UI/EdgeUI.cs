using System.Collections;
using System.Collections.Generic;
using MGraph;
using UnityEngine;
using UnityEngine.EventSystems;
public class EdgeUI : MonoBehaviour, IPointerClickHandler, ITargetable, IEdgeReference {
    // Start is called before the first frame update

    [SerializeField] private LineRenderer line;
    [SerializeField] private LineRenderer focus;
    [SerializeField] private BoxCollider2D boxCollider2D;

    private AbGraph graph;
    GraphCreator graphcreator;

    List<Edge> edges = new List<Edge> ();

    Node v1, v2;

    private void Awake () {

    }
    void Start () {
        UpdateCollider ();
        graphcreator = GraphCreator.instance;

        Unfocus ();
    }

    public void Initialize (AbGraph graph, Node n1, Node n2, bool directed = false) {
        if (graph == null || n1 == null || n2 == null) {
            throw new System.ArgumentNullException ();
        }

        if (v1 != null) {
            v1.OnSetPosCallBack -= OnMoved;
            v1.OnObjectDestroyedCallback -= DestroySelf;
        }

        if (v2 != null) {
            v2.OnSetPosCallBack -= OnMoved;
            v2.OnObjectDestroyedCallback -= DestroySelf;
        }

        n1.OnSetPosCallBack += OnMoved;
        n1.OnObjectDestroyedCallback += DestroySelf;
        n2.OnSetPosCallBack += OnMoved;
        n2.OnObjectDestroyedCallback += DestroySelf;

        this.v1 = n1;
        this.v2 = n2;

        UnSubscribe ();
        edges = new List<Edge> ();
        this.graph = graph;

        OnMoved ();
        if (directed) {
            var edge = graph.AddDirectedEdge (n1.id, n2.id);
            edges.Add (edge);

            Subscribe (edge);

            line.endColor = Color.black;
            return;
        }
        line.endColor = Color.white;
        var eds = graph.AddEdge (n1.id, n2.id);
        edges.Add (eds.Item1);
        Subscribe (eds.Item1);
        edges.Add (eds.Item2);
        Subscribe (eds.Item2);
    }

    public bool AddEdge (int n1, int n2) {
        Edge edge = null;
        if (v1.id == n1 && v2.id == n2) {
            line.startColor = Color.white;
            edge = graph.AddDirectedEdge (n1, n2);
        } else if (v2.id == n1 && v1.id == n2) {
            line.endColor = Color.white;
            edge = graph.AddDirectedEdge (n1, n2);
        }

        if (edge != null) {
            edges.Add (edge);
            Subscribe (edge);
        }
        return false;
    }

    public void UpdateCollider () {
        var p1 = line.GetPosition (0);
        var p2 = line.GetPosition (1);
        var boxt = boxCollider2D.transform;
        boxt.position = (p1 + p2) / 2;

        var dir = p2 - p1;
        boxCollider2D.size = new Vector2 (line.endWidth * 1.5f, dir.magnitude);

        boxt.up = dir.normalized;
    }

    public void OnMoved () {
        line.SetPosition (0, v1.position);
        line.SetPosition (1, v2.position);

        focus.SetPosition (0, v1.position);
        focus.SetPosition (1, v2.position);

        UpdateCollider ();
    }
    //Detect if a click occurs
    public void OnPointerClick (PointerEventData pointerEventData) {
        //Output to console the clicked GameObject's name and the following message. You can replace this with your own actions for when clicking the GameObject.
        graphcreator.FocusEdge (this);
        Debug.Log (name + " Game Object Clicked!");
    }

    private void UnSubscribe () {
        foreach (var edge in edges) {
            edge.reference = null;
        }
    }

    bool des = false;
    public void DestroySelf () {
        if (des) {
            return;
        }
        if (edges.Count > 0) {
            var e = edges[0];
            graph.RemoveEdge (e.src, e.dest);
        }
        des = true;
        Destroy (this.gameObject);
    }

    public void Focus () {
        focus.gameObject.SetActive (true);
        //throw new System.NotImplementedException ();
    }

    public void Unfocus () {
        focus.gameObject.SetActive (false);
        //throw new System.NotImplementedException ();
    }

    public void Subscribe (Edge edge) {
        edge.reference = this;
    }

    private void OnDisable () {
        // print ("Edge destroyed");
        UnSubscribe ();
    }
}
