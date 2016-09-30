using UnityEngine;
using System.Collections;

public class autoStart : MonoBehaviour {

	public GameObject[] hideOnStart;
	public dicomManager dicom;

	public bool autoplay = true;

	// Use this for initialization
	void Start () {
		dicom.load ();

		if (autoplay)
			dicom.play ();

		foreach (GameObject g in hideOnStart)
			g.SetActive (false);

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
