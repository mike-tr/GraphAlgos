using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EdgeUI : MonoBehaviour, IPointerClickHandler {
    // Start is called before the first frame update

    [SerializeField] private LineRenderer line;
    [SerializeField] private BoxCollider2D boxCollider2D;
    void Start () {
        UpdateCollider ();
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

    //Detect if a click occurs
    public void OnPointerClick (PointerEventData pointerEventData) {
        //Output to console the clicked GameObject's name and the following message. You can replace this with your own actions for when clicking the GameObject.
        Debug.Log (name + " Game Object Clicked!");
    }

    // Update is called once per frame
    void Update () {

    }
}
