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

	SpriteRenderer my_renderer;

	public SpriteRenderer icon_renderer;

	bool was_selected_last_frame = false;

	public SelectableCell _cell_info;
	public void SetSelectableCell(SelectableCell c) {
		if (_cell_info == null)
			_cell_info = c;
		else
			Debug.LogError ("Attempted to re-assign selectable cell.");

		icon_renderer.sprite = c.game_info.icon;
	}

	public void RemixDesiredPosition() {
		time_duration = UnityEngine.Random.Range (0.5f, 3.0f);
		desired_position = _cell_info.initial_desired_position + UnityEngine.Random.onUnitSphere * 0.1f;
	}

	// Use this for initialization
	void Start () {
		previous_time = Time.time;
		icon_renderer.sprite = _cell_info.game_info.icon;
		my_renderer = GetComponent<SpriteRenderer> ();
		transform.position = _cell_info.initial_desired_position;
		RemixDesiredPosition ();
	}

	// Update is called once per frame
	void Update () {
		ProcessMovement();

		//print ("obj: " + _cell_info.is_selected.ToString ());

		if (_cell_info.is_selected) {
			if (was_selected_last_frame == false)
				my_renderer.sortingOrder = ++top_most_sorting_level;

			was_selected_last_frame = true;
			transform.localScale += (Vector3.one * (1.75f + Mathf.Abs(Mathf.Sin(Time.time * 5.0f) * 0.1f)) - transform.localScale) * 0.2f;
			//my_renderer.color = new Color (my_renderer.color.r, my_renderer.color.g, my_renderer.color.b, 1.0f);
			transform.SetAsFirstSibling ();
			selected_icon = this;
		} else {

			was_selected_last_frame = false;
			transform.localScale += (Vector3.one - transform.localScale) * 0.2f;
			//my_renderer.color = new Color (my_renderer.color.r, my_renderer.color.g, my_renderer.color.b, 0.3f);
		}
	}

	void ProcessMovement() {
		/*float ratio = (Time.time - previous_time) / time_duration;
		transform.position = Vector3.Lerp (previous_desired_position, desired_position, ratio);
		if (ratio >= 1.0f) {
			previous_desired_position = desired_position;
			RemixDesiredPosition ();
			previous_time = Time.time;
		}*/

		if (GameManager.GetSelectorState () != SelectorState.GRID) {
			if(!_cell_info.is_selected) {
				ConfirmMovement ();
			} else {
				NormalMovement();
			}
		} else {
			NormalMovement();
		}
	}

	void NormalMovement() {
        transform.position = _cell_info.initial_desired_position; //+ Vector3.up * Mathf.Sin (Time.time * 2 + _cell_info.initial_desired_position.x) * 0.1f;
	}

	void ConfirmMovement() {
		SelectableCell selected_cell = GameManager.GetFocusedCell ();
		Vector3 delta = _cell_info.initial_desired_position - selected_cell.initial_desired_position;
		Vector3 new_desired_position = selected_cell.initial_desired_position + delta * 2.0f;
		transform.position = new_desired_position + Vector3.up * Mathf.Sin (Time.time * 2 + _cell_info.initial_desired_position.x) * 0.1f;
	}
}