using UnityEngine;
using System;
using System.Collections;

public class SfxrCacheSurrogate : MonoBehaviour {

	// ================================================================================================================
	// PUBLIC INTERFACE -----------------------------------------------------------------------------------------------

	public void CacheSound(SfxrSynth __synth, Action __callback) {
		StartCoroutine(CacheSoundAsynchronously(__synth, __callback));
	}

	private IEnumerator CacheSoundAsynchronously(SfxrSynth __synth, Action __callback) {
		yield return null;
		__synth.CacheSound(null, true);
		__callback();
		UnityEngine.Object.Destroy(gameObject);
	}

}
