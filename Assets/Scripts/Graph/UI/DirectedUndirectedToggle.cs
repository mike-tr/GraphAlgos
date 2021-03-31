using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DirectedUndirectedToggle : MonoBehaviour {
    // Start is called before the first frame update
    public Text text;
    public Button button;

    private void Start () {
        GraphCreator.instance.OnRestartCallback += ResetSettings;
        ResetSettings ();
    }

    private void ResetSettings () {
        text.text = "Undirected graph \n ( Convert to Directed )";
        button.interactable = true;
    }

    public void ChangeToDirected () {
        GraphCreator.instance.ChangeToDirected ();
        text.text = "Directed graph";

        button.interactable = false;
    }
}
