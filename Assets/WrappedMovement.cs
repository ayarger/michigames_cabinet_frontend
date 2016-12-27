using UnityEngine;
using System.Collections;

public class WrappedMovement : MonoBehaviour {

	public Vector3 movement;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.position += movement * Time.deltaTime;
	}
}
