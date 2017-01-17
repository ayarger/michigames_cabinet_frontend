using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeController : MonoBehaviour {

	Image fade_panel;

	// Use this for initialization
	void Start () {
		fade_panel = GetComponent<Image> ();
	}
	
	// Update is called once per frame
	void Update () {
		float progress = GameManager.GetTransitionProgress ();
		fade_panel.color = Color.Lerp (new Color (1, 1, 1, 0), new Color (1, 1, 1, 1), progress); 
	}
}
