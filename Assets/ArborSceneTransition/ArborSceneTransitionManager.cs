using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;
using System.Threading;
using UnityEngine.SceneManagement;

public enum SceneTransitionState { TO_GAME, TO_CHANGE, WAITING };

public class ArborSceneTransitionManager : MonoBehaviour {

    static TransitionView active_transition_view;
    public static void RegisterActiveTransitionView(TransitionView tv) {
        if (active_transition_view != null && active_transition_view != tv)
            Destroy (active_transition_view);
        active_transition_view = tv;
    }

    ArborSceneTransitionManager instance;
    static List<Action> gameplay_start_Callbacks = new List<Action>();
    public static void RegisterGameplayStartCallback(Action callback) {
        if (!gameplay_start_Callbacks.Contains (callback))
            gameplay_start_Callbacks.Add (callback);
    }
    public static void RemoveGameplayStartCallback(Action callback) {
        if (gameplay_start_Callbacks.Contains (callback))
            gameplay_start_Callbacks.Remove (callback);
    }
    public static void ClearGameplayStartCallbacks() {
        gameplay_start_Callbacks.Clear ();
    }

    public float transition_duration = 1.0f;
    public float dead_time_pause = 1.0f;

    static string destination;
    static string landing_area_name;
    static bool additive_load;

    public Texture DefaultTex;

    static SceneTransitionState current_state = SceneTransitionState.TO_GAME;

    static float progress = 0.0f;
    static float initial_time = Time.time;

    public static SceneTransitionState GetState() {
        return current_state;
    }

    public static float GetProgress() {
        return progress;
    }

    public static void ChangeScene(string scene_name, string landing_area = "", float ease_factor = 0.25f, bool low_key = false, bool additive_load_mode = false)
    {
        if (current_state != SceneTransitionState.WAITING)
            return;
        print ("CHANGESCENE: " + scene_name);
        //low_key_change = low_key;
        destination = scene_name;
        landing_area_name = landing_area;
        additive_load = additive_load_mode;
        current_state = SceneTransitionState.TO_CHANGE;
        initial_time = Time.time;
    }

    // Needed for UnityEvents
    public void ChangeScene_nonstatic(string scene_name, string landing_area, bool low_key) {
        ChangeScene (scene_name, landing_area, 0.25f, low_key);
    }

   

    void HandleMissingTextures() {
        Renderer[] rends = GameObject.FindObjectsOfType<Renderer> ();
        foreach (Renderer r in rends) {
            if (r.material.mainTexture == null) {
                r.material.mainTexture = DefaultTex;
                r.transform.Rotate (Vector3.up * UnityEngine.Random.Range(-5.0f, 5.0f));
            }
        }
    }

    void Start () {
        initial_time = Time.time;
        current_state = SceneTransitionState.TO_GAME;
    }

    bool coroutine_running = false;
    void ProcessToChange() {
        progress = Mathf.Clamp01((Time.time - initial_time) / transition_duration);
        if (progress >= 1.0f) {
            
            if (coroutine_running == false)
            {
                coroutine_running = true;
                StartCoroutine(ChangeSceneWithDelay(1.0f));
            }
        }
    }

    void ProcessToGame() {
        progress = Mathf.Clamp01((Time.time - initial_time) / transition_duration);

        if (progress >= 1.0f) {
            current_state = SceneTransitionState.WAITING;
        }
    }

    IEnumerator ChangeSceneWithDelay(float delay) {
        yield return new WaitForSeconds (delay);

        initial_time = Time.time;
        coroutine_running = false;
        current_state = SceneTransitionState.TO_GAME;
    }

    // Update is called once per frame
    void Update () {
        if (current_state == SceneTransitionState.TO_CHANGE)
            ProcessToChange ();
        else if (current_state == SceneTransitionState.TO_GAME)
            ProcessToGame ();
    }

    

    public delegate void SceneCallback(string scene_name);
    public delegate void SceneTransitionCallback(string current_scene, string destination_scene);
    private static List<SceneCallback> _OnSceneEnterCallbacks = new List<SceneCallback>();
    private static List<SceneCallback> _OnSceneExitCallbacks = new List<SceneCallback>();
    private static List<SceneTransitionCallback> _OnSceneChangeInitiatedCallbacks = new List<SceneTransitionCallback>();
    
    public static void RegisterSceneEnterCallback(SceneCallback callback)
    {
		if (_OnSceneEnterCallbacks.Contains (callback)) {
			Debug.LogWarning ("Double-Registered callback.");
			return;
		}
        _OnSceneEnterCallbacks.Add(callback);
    }

    public static void UnregisterSceneEnterCallback(SceneCallback callback)
    {
        if (!_OnSceneEnterCallbacks.Contains(callback))
            Debug.LogWarning("Attempted to unregister non-registered callback.");
        _OnSceneEnterCallbacks.Remove(callback);
    }

    public static void RegisterSceneExitCallback(SceneCallback callback)
    {
        if (_OnSceneExitCallbacks.Contains(callback))
            Debug.LogWarning("Double-Registered callback.");
        _OnSceneExitCallbacks.Add(callback);
    }

    public static void UnregisterSceneExitCallback(SceneCallback callback)
    {
        if (!_OnSceneExitCallbacks.Contains(callback))
            Debug.LogWarning("Attempted to unregister non-registered callback.");
        _OnSceneExitCallbacks.Remove(callback);
    }

    public static void RegisterSceneChangeInitiatedCallback(SceneTransitionCallback callback)
    {
        if (_OnSceneChangeInitiatedCallbacks.Contains(callback))
            Debug.LogWarning("Double-Registered callback.");
        _OnSceneChangeInitiatedCallbacks.Add(callback);
    }

    public static void UnregisterSceneChangeInitiatedCallback(SceneTransitionCallback callback)
    {
        if (!_OnSceneChangeInitiatedCallbacks.Contains(callback))
            Debug.LogWarning("Attempted to unregister non-registered callback.");
        _OnSceneChangeInitiatedCallbacks.Remove(callback);
    }
}
