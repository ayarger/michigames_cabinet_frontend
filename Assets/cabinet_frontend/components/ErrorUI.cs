using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErrorUI : MonoBehaviour {
	static ErrorUI instance;

	public Text error_text;
	public Image error_panel;
	RectTransform rt;

	float duration = 5.0f;
	float previous_time;

	public static void ShowMessage(string message) {
		instance.error_text.text = message;
		instance.previous_time = Time.time;
	}

	// Use this for initialization
	void Start () {
		instance = this;
		rt = error_panel.GetComponent<RectTransform> ();
		previous_time = -5.1f;
	}
	
	// Update is called once per frame
	void Update () {
		Vector2 desired_position = new Vector2 (0, -214);
		if (Time.time - previous_time >= duration)
			desired_position = new Vector2 (-5, -300);

		rt.anchoredPosition += (desired_position - rt.anchoredPosition) * 0.1f;
	}
}
