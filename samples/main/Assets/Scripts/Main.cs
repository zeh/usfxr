using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour {

	private bool isADown;
	private bool isBDown;
	private bool isCDown;

	private SfxrSynth synthA;
	private SfxrSynth synthB;
	private SfxrSynth synthC;

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
		bool newIsCDown = Input.GetKey("c");

		if (newIsADown && !isADown) {
			Debug.Log("Key: A");

			if (synthA == null) {
				synthA = new SfxrSynth();
				synthA.paramss.setSettingsString("0,,0.032,0.4138,0.4365,0.834,,,,,,0.3117,0.6925,,,,,,1,,,,,0.5");
				synthA.cacheSound();
			}

			synthA.play();
		}
		if (newIsBDown && !isBDown) {
			Debug.Log("Key: B");

			if (synthB == null) {
				synthB = new SfxrSynth();
				synthB.paramss.setSettingsString("0,,0.032,0.4138,0.4365,0.834,,,,,,0.3117,0.6925,,,,,,1,,,,,0.5");
			}

			synthB.play();
		}
		if (newIsCDown && !isCDown) {
			Debug.Log("Key: C");

			if (synthC == null) {
				synthC = new SfxrSynth();
				synthC.paramss.setSettingsString("2,,0.1702,,0.1689,0.7793,0.0224,-0.4882,,,,,,0.271,0.1608,,,,1,,,,,0.5");
			}

			synthC.playMutated(0.2f);
		}

		isADown = newIsADown;
		isBDown = newIsBDown;
		isCDown = newIsCDown;
	}
}