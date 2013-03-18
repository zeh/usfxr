using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour {

	private bool isADown;
	private bool isBDown;

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
		bool newIsBDown = Input.GetKey("b");

		if (newIsADown && !isADown) {
			Debug.Log("Key: A");

			SfxrSynth synth = new SfxrSynth();
			synth.paramss.setSettingsString("0,,0.032,0.4138,0.4365,0.834,,,,,,0.3117,0.6925,,,,,,1,,,,,0.5");
			synth.cacheSound();
			synth.play();
		}
		if (newIsBDown && !isBDown) {
			Debug.Log("Key: B");

			SfxrSynth synth = new SfxrSynth();
			synth.paramss.generatePickupCoin();
			//synth.cacheSound();
			synth.paramss.setSettingsString("0,,0.032,0.4138,0.4365,0.834,,,,,,0.3117,0.6925,,,,,,1,,,,,0.5");
			synth.play();
		}

		isADown = newIsADown;
		isBDown = newIsBDown;
	}
}