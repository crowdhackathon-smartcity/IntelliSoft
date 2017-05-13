using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

public class TextToSpeach : MonoBehaviour, IVirtualButtonEventHandler {

	public AudioSource Audio;

	public string AudioText = "test one";

	private GameObject _btn;

	private GameObject _robot;

	private bool show = true;

	// Use this for initialization
	void Start () {
		Audio = gameObject.GetComponent<AudioSource>();
		_btn = GameObject.Find("VirtualButton");
		_robot = GameObject.Find("RobotKyle");
		_btn.GetComponent<VirtualButtonBehaviour>().RegisterEventHandler(this);
		StartCoroutine(FindLocation());

		distanceText = GameObject.Find("distanceText");
	}

	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator DownloadAudio(string text) {
		Debug.Log("1");
		Regex regex = new Regex("\\s+");
		var url = "https://api.voicerss.org/?key=745d143e300d440a8b0afaeb41409c35&hl=en-gb&src="+text;
		//var url = "https://translate.google.com/translate_tts?ie=UTF-8&total=1&idx=0&textlen=32&client=tw-ob&q="+WWW.EscapeURL(text)+"&tl=En-gb";
		WWW www = new WWW(url);
		yield return www;
		Debug.Log("2");
		//while (www.isDone == false) {
		//	yield return 0;
		//}

		Debug.Log("3");
		try {
			Audio.clip = www.GetAudioClip(false, true, AudioType.MPEG);
		}
		catch (Exception ex) {
			Debug.Log(ex.Message);
			if (ex.InnerException != null) {
				Debug.Log(ex.InnerException.Message);
			}
		}
		Debug.Log("4");
		Audio.Play();
	}

	public void OnButtonPressed(VirtualButtonAbstractBehaviour vb) {
		StartCoroutine(DownloadAudio("test"));
		_robot.SetActive(!show);
		show = !show;
	}

	private GameObject distanceText;
	IEnumerator FindLocation() {
		while (true) {

			// First, check if user has location service enabled
			if (!Input.location.isEnabledByUser)
				yield break;

			// Start service before querying location
			Input.location.Start();

			// Wait until service initializes
			int maxWait = 20;
			while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0) {
				yield return new WaitForSeconds(1);
				maxWait--;
			}

			// Service didn't initialize in 20 seconds
			if (maxWait < 1) {
				print("Timed out");
				yield break;
			}

			// Connection has failed
			if (Input.location.status == LocationServiceStatus.Failed) {
				print("Unable to determine device location");
				yield break;
			} else {
				// Access granted and location value could be retrieved
				print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " +
					  Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " +
					  Input.location.lastData.timestamp);
			}
			distanceText = GameObject.Find("distanceText");
			distanceText.GetComponent<Text>().text = Input.location.lastData.latitude + " , " +
													 Input.location.lastData.longitude;
			// Stop service if there is no need to query location updates continuously
			Input.location.Stop();
		}
	}

	public void OnButtonReleased(VirtualButtonAbstractBehaviour vb) {

	}
}
