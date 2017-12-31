using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ObjectIcon : MonoBehaviour {

	Vector3 desired_position = Vector3.zero;

	Vector3 previous_desired_position;

	public static ObjectIcon selected_icon;

	float time_duration = 2.0f;
	float previous_time = 0.0f;

	public static int top_most_sorting_level = 10000;

	public Renderer icon_renderer;

	bool was_selected_last_frame = false;

	public SelectableCell _cell_info;
	public void SetSelectableCell(SelectableCell c) {
		if (_cell_info == null)
			_cell_info = c;
		else
			Debug.LogError ("Attempted to re-assign selectable cell.");

        SetTexture(c.game_info.icon);
	}

	public void RemixDesiredPosition() {
		time_duration = UnityEngine.Random.Range (0.5f, 3.0f);
		desired_position = _cell_info.initial_desired_position + UnityEngine.Random.onUnitSphere * 0.1f;
	}

    void SetTexture(Sprite s)
    {
        //icon_renderer.material.mainTexture = s.texture;
        YMeshUtilities.GetAndApplyMeshFromSprite(s, 0.25f, GetComponent<MeshRenderer>(), GetComponent<MeshFilter>());
        //transform.Find("sprite_icon").GetComponent<SpriteRenderer>().sprite = s;
    }

    private void Awake()
    {
        icon_renderer = GetComponent<Renderer>();
        icon_renderer.material.mainTextureScale = new Vector2(-1, -1);
    }

    // Use this for initialization
    void Start () {
        transform.position = _cell_info.initial_desired_position - Vector3.up * (10.0f + UnityEngine.Random.Range(0.0f, 50.0f));
		previous_time = Time.time;
		SetTexture(_cell_info.game_info.icon);
		RemixDesiredPosition ();
	}

    float scale_velocity = 0.0f;

	// Update is called once per frame
	void Update () {
		ProcessMovement();

        Vector3 desired_forward = Vector3.forward;
        Vector3 desired_scale = Vector3.one;

		if (_cell_info.is_selected) {
			//if (was_selected_last_frame == false)
			//	my_renderer.sortingOrder = ++top_most_sorting_level;

			was_selected_last_frame = true;
            desired_forward = Vector3.forward + Vector3.right * Mathf.Sin(Time.time * 2.0f) * 0.5f;
            desired_scale = Vector3.one * (1.75f + Mathf.Abs(Mathf.Sin(Time.time * 5.0f) * 0.1f));
            
			transform.SetAsFirstSibling ();
			selected_icon = this;
		} else {
            transform.forward = Vector3.forward;

            was_selected_last_frame = false;
			transform.localScale += (Vector3.one - transform.localScale) * 0.2f;
		}

        // Local Scale Bounce
        float x = desired_scale.x - transform.localScale.x;
        float k = 0.5f;
        float f = k * x;

        scale_velocity += f;
        scale_velocity *= 0.7f;
        transform.localScale += Vector3.one * scale_velocity;

         //transform.localScale += (desired_scale - transform.localScale) * 0.2f;
        transform.forward = Vector3.Slerp(transform.forward, desired_forward, 0.1f);
    }

    void UpdateScale()
    {

        /*float x = normal_scale.x - rt.localScale.x;
        float k = 0.5f;
        float f = k * x;

        velocity += f;
        velocity *= 0.7f;
        rt.localScale += Vector3.one * velocity;*/

        //rt.localScale = Vector3.Lerp(rt.localScale, normal_scale, 0.25f * Application.targetFrameRate * Time.deltaTime);
    }

    void ProcessMovement() {
		if (GameManager.GetSelectorState () != SelectorState.GRID) {
			if(!_cell_info.is_selected) {
                NormalMovement();
			} else {
                SelectedMovement();
			}
		} else {
			NormalMovement();
		}
	}

	void NormalMovement() {

        Vector3 desired_position = _cell_info.initial_desired_position;
        transform.position += (desired_position - transform.position) * 0.1f;
	}

    void SelectedMovement()
    {
        Vector3 desired_position = Camera.main.transform.position + Vector3.forward * 5 - Vector3.right * 1;
        transform.position += (desired_position - transform.position) * 0.1f;
    }

	void ConfirmMovement() {
		SelectableCell selected_cell = GameManager.GetFocusedCell ();
		Vector3 delta = _cell_info.initial_desired_position - selected_cell.initial_desired_position;
		Vector3 new_desired_position = selected_cell.initial_desired_position + delta * 2.0f;
		transform.position = new_desired_position + Vector3.up * Mathf.Sin (Time.time * 2 + _cell_info.initial_desired_position.x) * 0.1f;
    }
}