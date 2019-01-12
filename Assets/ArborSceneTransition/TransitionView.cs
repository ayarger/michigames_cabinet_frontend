using UnityEngine;
using System.Collections;

public abstract class TransitionView : MonoBehaviour {
	void Start() {
		TransitionViewStart ();
		ArborSceneTransitionManager.RegisterActiveTransitionView (this);
	}

	public abstract void TransitionViewStart ();
}
