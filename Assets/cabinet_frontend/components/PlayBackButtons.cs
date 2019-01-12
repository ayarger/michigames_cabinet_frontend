using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayBackButtons : MonoBehaviour {

    public Camera main_camera;

    Transform a_button;
    public Transform b_button;

	// Use this for initialization
	void Start () {
        a_button = transform;
	}
	
	// Update is called once per frame
	void Update () {

        Vector3 desired_position = main_camera.transform.position + Vector3.forward * 1 + Vector3.down * 3;

		if(GameManager.GetSelectorState() == SelectorState.CONFIRM)
        {
            desired_position = main_camera.transform.position + Vector3.forward * 8 - Vector3.right * 2.5f - Vector3.up;

        }

        Vector3 desired_forward_a = Vector3.forward + Vector3.right * Mathf.Sin(Time.time * 2.0f - 0.5f) * 0.5f;
        a_button.forward = Vector3.Slerp(a_button.forward, desired_forward_a, 0.1f);
        a_button.position += (desired_position - a_button.position) * 0.1f;

        Vector3 desired_forward_b = Vector3.forward + Vector3.right * Mathf.Sin(Time.time * 2.0f - 1.0f) * 0.5f;
        b_button.forward = Vector3.Slerp(b_button.forward, desired_forward_b, 0.1f);
        b_button.position += (desired_position + Vector3.down * 1.25f + Vector3.right - b_button.position) * 0.1f;
    }
}
