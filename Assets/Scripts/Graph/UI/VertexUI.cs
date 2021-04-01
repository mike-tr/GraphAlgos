using System.Collections;
using System.Collections.Generic;
using MGraph;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VertexUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, ITargetable {
    private static Camera cam;
    [SerializeField] Text text;
    private bool drag = false;
    private float time = 0;

    public Node node { get; private set; }

    public int id {
        get {
            return node.id;
        }
    }

    [SerializeField] private SpriteRenderer sprite;

    [SerializeField] private GameObject focus;

    private GraphCreator graphCreator;

    private AbGraph graph;
    // Start is called before the first frame update
    void Start () {
        if (cam == null) {
            cam = Camera.main;
        }
        graphCreator = GraphCreator.instance;
        Unfocus ();
    }

    public Node Initialize (AbGraph graph) {
        this.node = graph.AddNode ();
        this.graph = graph;
        SetText ("Node : " + node.id);
        return node;
    }

    public Node Initialize (AbGraph graph, Node node) {
        if (graph.GetNode (node.id) != node) {
            throw new System.ArgumentException ("the given node is not in the graph!");
        }
        this.node = node;
        this.graph = graph;

        SetPosition (node.position);

        SetText ("Node : " + node.id);
        return node;
    }

    public void OnPointerDown (PointerEventData eventData) {
        // Debug.Log (name + "Game Object Click in Progress");
        graphCreator.FocusVertex (this);
        drag = true;
        time += 0.1f;
    }

    //Detect if clicks are no longer registering
    public void OnPointerUp (PointerEventData pointerEventData) {
        // Debug.Log (name + "No longer being clicked");
        drag = false;
    }

    public void SetText (string text) {
        this.text.text = text;
    }

    public void SetColor (Color color) {
        sprite.color = color;
    }

    public void SetPosition (Vector3 pos) {
        pos.z = 0;
        transform.position = pos;

        node.position = pos;
    }

    // Update is called once per frame
    void Update () {
        if (drag) {
            if (time > 0) {
                time -= Time.deltaTime;
                return;
            }

            SetPosition (cam.ScreenToWorldPoint (Input.mousePosition));
        }
    }

    public void DestroySelf () {
        graph.RemoveNode (node.id);
        Destroy (this.gameObject);
        //throw new System.NotImplementedException ();
    }

    public void Focus () {
        focus.SetActive (true);
        //throw new System.NotImplementedException ();
    }

    public void Unfocus () {
        focus.SetActive (false);
        //throw new System.NotImplementedException ();
    }

}
