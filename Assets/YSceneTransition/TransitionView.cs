using UnityEngine;
using System.Collections;

public abstract class TransitionView : MonoBehaviour {
	void Start() {
		TransitionViewStart ();
		YSceneTransitionManager.RegisterActiveTransitionView (this);
	}

	public abstract void TransitionViewStart ();
}
