using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour {

	private bool isADown;

	void Start () {
		isADown = false;
	    Debug.Log("Initialized");

        /*
        var synth:SfxrSynth = new SfxrSynth();
        synth.params.setSettingsString("0,,0.271,,0.18,0.395,,0.201,,,,,,0.284,,,,,0.511,,,,,0.5");

        ...

        synth.play();
        */
    }
	
	void Update () {
		bool newIsADown = Input.GetKey("a");

		if (newIsADown && !isADown) {
			Debug.Log("A");

			SfxrSynth synth = new SfxrSynth();
			synth.paramss.setSettingsString("0,,0.271,,0.18,0.395,,0.201,,,,,,0.284,,,,,0.511,,,,,0.5");
			synth.play();
		}

		isADown = newIsADown;
	}
}