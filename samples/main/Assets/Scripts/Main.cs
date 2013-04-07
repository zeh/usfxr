using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour {

	private SfxrSynth synthA;
	private SfxrSynth synthB;
	private SfxrSynth synthC;
	private SfxrSynth synthD;

	void Start () {
	    Debug.Log("Initialized");
    }
	
	void Update () {
		if (Input.GetKeyDown("a")) {
			Debug.Log("Key: A (Coin)");

			if (synthA == null) {
				// Coin
				synthA = new SfxrSynth();
				synthA.parameters.SetSettingsString("0,,0.032,0.4138,0.4365,0.834,,,,,,0.3117,0.6925,,,,,,1,,,,,0.5");
				synthA.CacheSound();
			}

			synthA.Play();
		}
		if (Input.GetKeyDown("b")) {
			Debug.Log("Key: B (Coin without caching)");

			if (synthB == null) {
				// Coin
				synthB = new SfxrSynth();
				synthB.parameters.SetSettingsString("0,,0.032,0.4138,0.4365,0.834,,,,,,0.3117,0.6925,,,,,,1,,,,,0.5");
			}

			synthB.Play();
		}
		if (Input.GetKeyDown("c")) {
			Debug.Log("Key: C (Laser)");

			if (synthC == null) {
				// Laser
				synthC = new SfxrSynth();
				synthC.parameters.SetSettingsString("0,,0.1783,,0.3898,0.7523,0.2,-0.2617,,,,,,0.261,0.0356,,,,1,,,0.2466,,0.5");
				synthC.SetParentTransform(Camera.main.transform);

				float ti = Time.realtimeSinceStartup;
				synthC.CacheMutations(15, 0.05f);
				Debug.Log("Took " + (Time.realtimeSinceStartup - ti) + "s to cache mutations.");

				// Hit
				//synthC.paramss.setSettingsString("2,,0.1702,,0.1689,0.7793,0.0224,-0.4882,,,,,,0.271,0.1608,,,,1,,,,,0.5");
			}

			synthC.PlayMutated();
			//synthC.play();
		}
		if (Input.GetKeyDown("d")) {
			Debug.Log("Key: D (Coin with parallel caching)");

			if (synthD == null) {
				// Coin
				synthD = new SfxrSynth();
				synthD.parameters.SetSettingsString("0,,0.032,0.4138,0.4365,0.834,,,,,,0.3117,0.6925,,,,,,1,,,,,0.5");
				synthD.CacheSound();
			}

			synthD.Play();
		}
	}
}