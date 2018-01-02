using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeSprite3D : MonoBehaviour {

    public Sprite s;

    public float scale = 1.0f;

	// Use this for initialization
	void Start () {
        YMeshUtilities.GetAndApplyMeshFromSprite(s, 0.25f, GetComponent<MeshRenderer>(), GetComponent<MeshFilter>());
        transform.localScale = Vector3.one * scale;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
