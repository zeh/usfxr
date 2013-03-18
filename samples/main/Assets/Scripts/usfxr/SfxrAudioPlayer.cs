using UnityEngine;
using System.Collections;

// A behavior script responsible for streaming audio to the engine

public class SfxrAudioPlayer : MonoBehaviour {
	
	private double		samplingFrequency = AudioSettings.outputSampleRate;
	private bool		isDestroyed = false;
	private SfxrSynth	sfxrSynth;
	private bool		needsToDestroy = false;

	// Public methods

	void Start () {
		// Create an empty audio source so this GameObject can receive audio events
		AudioSource soundSource = (AudioSource) gameObject.AddComponent("AudioSource");
		soundSource.clip = new AudioClip();
		soundSource.volume = 1f;
		soundSource.pitch = 1f;
		soundSource.priority = 128;
	}
	
	void Update () {
		if (needsToDestroy) {
			needsToDestroy = false;
			destroy();
		}
	}
	
	void OnAudioFilterRead(float[] data, int channels) {
		// Generates audio
		// Data is an array of floats ranging from -1.0f to 1.0f

		if (!isDestroyed && sfxrSynth != null) {
			bool hasMoreSamples = sfxrSynth.getSampleData(data, channels);
			if (!hasMoreSamples) needsToDestroy = true;
		}

		/*
		log("Audio filter read"); // 2048, 2
		// update increment in case frequency has changed
		increment = frequency * 2 * Math.PI / sampling_frequency;
		for (var i = 0; i < data.Length; i = i + channels) {
			phase = phase + increment;
			// this is where we copy audio data to make them “available” to Unity
			data[i] = (float)(gain*Math.Sin(phase));
			// if we have stereo, we copy the mono data to each channel
			if (channels == 2) data[i + 1] = data[i];
			if (phase > 2 * Math.PI) phase = 0;
    	}
		
		times++;
		*/
  	}

	// Public methods
	public void setSfxrSynth(SfxrSynth __sfxrSynth) {
		// Sets the synth that needs to be pooled when new audio data is needed
		sfxrSynth = __sfxrSynth;
	}
	
	public void destroy() {
		// Stops the audio and destroys self
		if (!isDestroyed) {
			isDestroyed = true;
			sfxrSynth = null;
			UnityEngine.Object.Destroy(gameObject);
		}
	}
		

}
