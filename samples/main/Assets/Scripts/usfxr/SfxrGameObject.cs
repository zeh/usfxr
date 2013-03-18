using UnityEngine;
using System.Collections;

// A game object responsible for streaming audio to the engine

public class SfxrGameObject : MonoBehaviour {
	
	private double samplingFrequency = AudioSettings.outputSampleRate;

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
	
	}
	
	void OnAudioFilterRead(float[] data, int channels) {
		// Generates audio
		// Data is an array of floats ranging from -1.0f to 1.0f
		
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

	// Internal methods
	
	private void Destroy() {
		// Destroys self - not necessary anymore
		UnityEngine.Object.Destroy(gameObject);
	}
		

}
