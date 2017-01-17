using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Paper : MonoBehaviour {

    static Paper instance;

    static Vector2 desired_position;
    static Vector2 initial_position;
    RectTransform rt;

    // Use this for initialization
    void Start () {
        instance = this;
        desired_position = new Vector2(200, 11.84f);
        initial_position = new Vector2(190.0f, 0.0f);
        rt = GetComponent<RectTransform>();
    }
    
    // Update is called once per frame
    void Update () {
        if(rt == null)
            rt = GetComponent<RectTransform>();
        if (instance == null || instance != this)
            instance = this;

        desired_position = new Vector2(200, 11.84f);
        //initial_position = new Vector2(181.1f, -12.7f);
        initial_position = new Vector2(190.0f, 0.0f);

        rt.anchoredPosition += (desired_position - rt.anchoredPosition) * 0.05f;
    }

    public static void Refresh()
    {
        instance.rt.anchoredPosition = initial_position;
    }
}
