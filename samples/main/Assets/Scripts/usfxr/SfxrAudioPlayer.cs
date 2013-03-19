using UnityEngine;
using System.Collections;

public class SfxrAudioPlayer : MonoBehaviour {

	/**
	 * usfxr
	 *
	 * Copyright 2013 Zeh Fernando
	 *
	 * Licensed under the Apache License, Version 2.0 (the "License");
	 * you may not use this file except in compliance with the License.
	 * You may obtain a copy of the License at
	 *
	 * 	http://www.apache.org/licenses/LICENSE-2.0
	 *
	 * Unless required by applicable law or agreed to in writing, software
	 * distributed under the License is distributed on an "AS IS" BASIS,
	 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	 * See the License for the specific language governing permissions and
	 * limitations under the License.
	 *
	 */

	/**
	 * SfxrAudioPlayer
	 * This is the (internal) behavior script responsible for streaming audio to the engine
	 * 
	 * @author Zeh Fernando
	 */


	// Properties
	private bool		isDestroyed = false;		// If true, this instance has been destroyed and shouldn't do anything yes
	private bool		needsToDestroy = false;		// If true, it has been scheduled for destruction (from outside the main thread)

	// Instances
	private SfxrSynth	sfxrSynth;					// SfxrSynth instance that will generate the audio samples used by this
	

	// ================================================================================================================
	// INTERNAL INTERFACE ---------------------------------------------------------------------------------------------
	
	void Start() {
		// Creates an empty audio source so this GameObject can receive audio events
		AudioSource soundSource = (AudioSource) gameObject.AddComponent("AudioSource");
		soundSource.clip = new AudioClip();
		soundSource.volume = 1f;
		soundSource.pitch = 1f;
		soundSource.priority = 128;
	}

	void Update () {
		// Destroys self in case it has been queued for deletion
		if (needsToDestroy) {
			needsToDestroy = false;
			Destroy();
		}
	}

	void OnAudioFilterRead(float[] __data, int __channels) {
		// Requets that sfxrSynth generates the needed audio data

		if (!isDestroyed && !needsToDestroy && sfxrSynth != null) {
			bool hasMoreSamples = sfxrSynth.GenerateAudioFilterData(__data, __channels);
			
			// If no more samples are needed, there's no more need for this GameObject so schedule a destruction (cannot do this in this thread)
			if (!hasMoreSamples) needsToDestroy = true;
		}
  	}
	

	// ================================================================================================================
	// PUBLIC INTERFACE -----------------------------------------------------------------------------------------------

	public void SetSfxrSynth(SfxrSynth __sfxrSynth) {
		// Sets the SfxrSynth instance that will generate the audio samples used by this
		sfxrSynth = __sfxrSynth;
	}

	public void Destroy() {
		// Stops audio immediately and destroys self
		if (!isDestroyed) {
			isDestroyed = true;
			sfxrSynth = null;
			UnityEngine.Object.Destroy(gameObject);
		}
	}


}
