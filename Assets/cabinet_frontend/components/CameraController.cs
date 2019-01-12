using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public Vector3 offset = Vector3.zero;

	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void LateUpdate () {
		

		SelectorState current_state = GameManager.GetSelectorState ();

		if (current_state == SelectorState.GRID)
			ProcessGrid ();
		else if (current_state == SelectorState.CONFIRM)
			ProcessConfirm ();



		//float vectical_distance = ObjectIcon.selected_icon.initial_desired_position.y - transform.position.y;
		//transform.position += Vector3.up * vectical_distance * 0.1f;


	}

	void ProcessGrid() {
		if (ObjectIcon.selected_icon == null)
			return;

		Vector3 temp_desired_position = new Vector3 (0, GameManager.GetFocusedCell().initial_desired_position.y, 0);
		transform.position += (temp_desired_position + offset - transform.position) * 0.1f;
	}

	void ProcessConfirm() {
        return;
        SelectableCell cell = GameManager.GetFocusedCell ();
		transform.position += (GameManager.GetFocusedCell().initial_desired_position + -Vector3.forward * 5 - transform.position) * 0.1f;
	}
}
