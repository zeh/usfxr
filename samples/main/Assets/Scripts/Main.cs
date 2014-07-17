using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour {

	private SfxrSynth synthA;
	private SfxrSynth synthB;
	private SfxrSynth synthC;
	private SfxrSynth synthD;
	private SfxrSynth synthE;

	void Start () {
	    Debug.Log("Initialized");
    }
	
	void Update () {
		if (Input.GetKeyDown("a")) {
			Debug.Log("Key: A (Coin with synchronous cache)");

			if (synthA == null) {
				// Coin
				synthA = new SfxrSynth();
				synthA.parameters.SetSettingsString("0,,0.032,0.4138,0.4365,0.834,,,,,,0.3117,0.6925,,,,,,1,,,,,0.5");
				synthA.CacheSound();
			}

			synthA.Play();
		}
		if (Input.GetKeyDown("b")) {
			Debug.Log("Key: B (Coin without caching, generating asynchronously)");

			if (synthB == null) {
				// Coin
				synthB = new SfxrSynth();
				synthB.parameters.SetSettingsString("0,,0.032,0.4138,0.4365,0.834,,,,,,0.3117,0.6925,,,,,,1,,,,,0.5");
			}

			synthB.Play();
		}
		if (Input.GetKeyDown("c")) {
			Debug.Log("Key: C (Laser with mutations cached synchronously)");

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
		}
		if (Input.GetKeyDown("d")) {
			Debug.Log("Key: D (Long death with asynchronous caching and callback)");

			if (synthD == null) {
				synthD = new SfxrSynth();
				synthD.parameters.SetSettingsString("2,,0.0782,0.6203,0.9024,0.5044,,-0.1298,0.0094,-0.0008,-0.5123,0.2868,-0.3859,-0.8811,0.9692,0.3616,0.001,0.0001,0.9528,0.0437,-0.4492,0.1089,,0.5");
				synthD.CacheSound(() => synthD.Play());
			} else {
				synthD.Play();
			}
		}
		if (Input.GetKeyDown("e")) {
			Debug.Log("Key: E (Long death with parallel caching of mutations and callback)");

			if (synthE == null) {
				// Coin
				synthE = new SfxrSynth();
				synthE.parameters.SetSettingsString("2,,0.0782,0.6203,0.9024,0.5044,,-0.1298,0.0094,-0.0008,-0.5123,0.2868,-0.3859,-0.8811,0.9692,0.3616,0.001,0.0001,0.9528,0.0437,-0.4492,0.1089,,0.5");
				synthE.CacheMutations(15, 0.05f, () => synthE.PlayMutated());
			} else {
				synthE.PlayMutated();
			}
		}
		if (Input.GetKeyDown("f")) {
			Debug.Log("Key: F (Random coin/pickup sound, automatically generated)");
			SfxrSynth synthF = new SfxrSynth();
			synthF.parameters.GeneratePickupCoin();
			synthF.Play();
		}
	}
}