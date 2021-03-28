using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VertexUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    private static Camera cam;
    [SerializeField] Text text;

    private int id = 0;

    private bool drag = false;
    private float time = 0;
    // Start is called before the first frame update
    void Start () {
        if (cam == null) {
            cam = Camera.main;
        }
    }

    public void OnPointerDown (PointerEventData eventData) {
        Debug.Log (name + "Game Object Click in Progress");
        drag = true;
        time += 0.1f;
    }

    //Detect if clicks are no longer registering
    public void OnPointerUp (PointerEventData pointerEventData) {
        Debug.Log (name + "No longer being clicked");
        drag = false;
    }

    public void SetTest (string text) {
        this.text.text = text;
    }

    // Update is called once per frame
    void Update () {
        if (drag) {
            if (time > 0) {
                time -= Time.deltaTime;
                return;
            }
            var np = cam.ScreenToWorldPoint (Input.mousePosition);
            np.z = 0;
            transform.position = np;
        }
    }
}
