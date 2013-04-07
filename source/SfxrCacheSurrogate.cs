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

	public void CacheMutations(SfxrSynth __synth, uint __mutationsNum, float __mutationAmount, Action __callback) {
		StartCoroutine(CacheMutationsAsynchronously(__synth, __mutationsNum, __mutationAmount, __callback));
	}

	private IEnumerator CacheMutationsAsynchronously(SfxrSynth __synth, uint __mutationsNum, float __mutationAmount, Action __callback) {
		yield return null;
		__synth.CacheMutations(__mutationsNum, __mutationAmount, null, true);
		__callback();
		UnityEngine.Object.Destroy(gameObject);
	}
}
