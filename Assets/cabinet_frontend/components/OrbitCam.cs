using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitCam : MonoBehaviour {

    public Transform target;

    public float radius = 5.0f;
    public float vertical_offset = 3;
    public float orbit_velocity_factor = 1.0f;

    public float starting_offset = 1.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {



        float t = (Time.time + starting_offset) * orbit_velocity_factor;

        Vector3 desired_pos = target.position + Vector3.right * Mathf.Cos(t) * radius + Vector3.forward * Mathf.Sin(t) * radius + Vector3.up * vertical_offset;
        transform.position = desired_pos;

        transform.forward = target.position - transform.position;
    }
}
