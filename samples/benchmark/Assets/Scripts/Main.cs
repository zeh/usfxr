using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour {
	
	private GUIText cameraGuiText;
	
	private float timeShort = 0;
	private float timeLong = 0;
	private int timesRan = 0;

	void Start () {
		cameraGuiText = (GUIText) GameObject.Find("CameraText").GetComponent("GUIText");
		
		cameraGuiText.text = "Press A to start benchmarking sound generation";
    }

	void Update () {
		if (Input.GetKeyDown("a")) {
			// Start benchmarking
			cameraGuiText.text = "Benchmarking...";
			
			string completeText = "";
			completeText += "Output sample rate: " + AudioSettings.outputSampleRate + "\n";
			completeText += "\n";
			
			int i;
			float ti;
			SfxrSynth synth;

			int count = 100;
			
			// Complete short sound caching
			ti = Time.realtimeSinceStartup;
			for (i = 0; i < count; i++) {
				synth = new SfxrSynth();
				synth.parameters.SetSettingsString("0,,0.032,0.4138,0.4365,0.834,,,,,,0.3117,0.6925,,,,,,1,,,,,0.5");
				synth.CacheSound();
			}
			float newTimeShort = Time.realtimeSinceStartup - ti;
			completeText += "Time to generate short sound " + count + " times: " + newTimeShort + " seconds (" + (newTimeShort*1000/count) + " ms/sound)\n";

			// Complete long sound caching
			ti = Time.realtimeSinceStartup;
			for (i = 0; i < count; i++) {
				synth = new SfxrSynth();
				synth.parameters.SetSettingsString("2,,0.0782,0.6203,0.9024,0.5044,,-0.1298,0.0094,-0.0008,-0.5123,0.2868,-0.3859,-0.8811,0.9692,0.3616,0.001,0.0001,0.9528,0.0437,-0.4492,0.1089,,0.5");
				synth.CacheSound();
			}
			float newTimeLong = Time.realtimeSinceStartup - ti;
			completeText += "Time to generate long sound " + count + " times: " + newTimeLong + " seconds (" + (newTimeLong*1000/count) + " ms/sound)\n";
			
			timeShort += newTimeShort;
			timeLong += newTimeLong;
			timesRan++;
			
			if (timesRan > 1) {
				completeText += "\n";
				completeText += "Averages after " + timesRan + " tests ran:\n";
				completeText += "Time to generate short sound " + count + " times: " + (timeShort / timesRan) + " seconds (" + ((timeShort / timesRan)*1000/count) + " ms/sound)\n";
				completeText += "Time to generate long sound " + count + " times: " + (timeLong / timesRan) + " seconds (" + ((timeLong / timesRan)*1000/count) + " ms/sound)\n";
			}
				
			completeText += "\n";
			completeText += "Press A to benchmark again";
			cameraGuiText.text = completeText;
		}
		/*
		if (newIsCDown && !isCDown) {
			Debug.Log("Key: C");

			SfxrSynth synthC = null;
			if (synthC == null) {
				synthC = new SfxrSynth();
				// Laser
				synthC.parameters.SetSettingsString("0,,0.1783,,0.3898,0.7523,0.2,-0.2617,,,,,,0.261,0.0356,,,,1,,,0.2466,,0.5");
				
				// Hit
				//synthC.paramss.setSettingsString("2,,0.1702,,0.1689,0.7793,0.0224,-0.4882,,,,,,0.271,0.1608,,,,1,,,,,0.5");
			}

			synthC.PlayMutated(0.05f);
			//synthC.play();
		}
		*/

		//isADown = newIsADown;
		//isBDown = newIsBDown;
		//isCDown = newIsCDown;
	}
}