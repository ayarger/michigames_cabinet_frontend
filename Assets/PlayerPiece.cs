using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerPiece : MonoBehaviour {

	public Sprite normal_sprite;
	public Sprite blink_sprite;

	float timer = 0;
	float blink_time = 120.0f;

	float desired_x_scale = 1.0f;

	// Use this for initialization
	void Start () {
		GetComponent<Image> ().sprite = normal_sprite;
		blink_time = UnityEngine.Random.Range (60.0f, 540.0f);
	}
	
	// Update is called once per frame
	void Update () {
		timer ++;

		if (transform.localScale.x > desired_x_scale && desired_x_scale < 0.0f) {
			transform.localScale = new Vector3 (transform.localScale.x - 0.1f, transform.localScale.y, transform.localScale.z);
		} else if (transform.localScale.x < desired_x_scale && desired_x_scale > 0.0f) {
			transform.localScale = new Vector3 (transform.localScale.x + 0.1f, transform.localScale.y, transform.localScale.z);
		}

		if (timer > blink_time) {
			GetComponent<Image> ().sprite = blink_sprite;
		} else {
			GetComponent<Image> ().sprite = normal_sprite;
		}

		if (timer > blink_time + 10) {
			timer = 0;
			blink_time = UnityEngine.Random.Range (60.0f, 540.0f);

			if (UnityEngine.Random.Range (0.0f, 1.0f) > 0.6f)
				desired_x_scale *= -1.0f;
		}
	}
}
