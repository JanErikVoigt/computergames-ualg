using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;


public class CameraController : MonoBehaviour
{
	public Camera[] cameras;
	private int currentCameraIndex;


	void Start () {
		SetActiveCamera(0);
	}

	void SetActiveCamera(int t)
	{
		currentCameraIndex = t;
		for (int i=0; i<cameras.Length; i++) {
			cameras[i].gameObject.SetActive(t == i);
		}
	}

	void Update () {

		if (Keyboard.current.cKey.wasPressedThisFrame)
		{
			SetActiveCamera((currentCameraIndex + 1) % cameras.Length);
		}
	}
}
