using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parallax_effect : MonoBehaviour {

    public float depth_factor = 0.5f;

	// Use this for initialization
	void Start () {
        previous_cam_position = Camera.main.transform.position;
	}

    // Update is called once per frame
    Vector3 previous_cam_position = Vector3.zero;
    void Update () {
        Vector3 delta = Camera.main.transform.position - previous_cam_position;
        transform.position += delta * depth_factor;
        previous_cam_position = Camera.main.transform.position;
    }
}
