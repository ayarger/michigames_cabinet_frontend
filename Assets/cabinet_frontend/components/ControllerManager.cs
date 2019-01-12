using UnityEngine;
using System.Collections;
using InControl;

public class ControllerManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		var devicesCount = InputManager.Devices.Count;
		for (int i = 0; i < devicesCount; i++)
		{
			var inputDevice = InputManager.Devices[i];
			inputDevice.Vibrate( inputDevice.LeftTrigger, inputDevice.RightTrigger );
			//print (inputDevice.Action1.IsPressed);
		}
	}

	bool GetKeyDownRight () {
		return true;
	}
}
