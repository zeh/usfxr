using UnityEngine;
using System.Collections;

public class PlayerCube : MonoBehaviour {
	
	private const int PLAYER_SPEED = 50; // units per second
	private const int SHOTS_PER_SECOND = 20;
	private const int SHOTS_WITH_AUDIO_PER_SECOND = 10; // Doesn't play audio on EVERY shot otherwise it's just a cacophony of audio effects

	private SfxrSynth synthFire;
	private float lastTimeFired;
	private float lastTimeFiredAudio;

	private bool modeMutated;

	public Transform bullet;

	void Start () {
		synthFire = new SfxrSynth();
		synthFire.parameters.SetSettingsString("2,,0.2563,0.3007,0.0251,0.7527,,-0.3176,,,,,,,,,-0.0929,-0.0095,1,,,,,0.5");

		modeMutated = true;
	}
	
	void Update () {
		float speedPassed = Time.deltaTime;
		float now = Time.realtimeSinceStartup;
		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))    transform.Translate(0, PLAYER_SPEED * speedPassed, 0);
		if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))  transform.Translate(0, PLAYER_SPEED * -speedPassed, 0);
		if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))  transform.Translate(PLAYER_SPEED * -speedPassed, 0, 0);
		if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) transform.Translate(PLAYER_SPEED * speedPassed, 0, 0);

		if (Input.GetKey(KeyCode.Space) && now > lastTimeFired + 1f/SHOTS_PER_SECOND) {
			// Fire
			Instantiate(bullet, transform.position, Quaternion.identity);
			lastTimeFired = now;

			if (now > lastTimeFiredAudio + 1f/SHOTS_WITH_AUDIO_PER_SECOND) {
				if (modeMutated) {
					synthFire.PlayMutated(0.08f, 15);
				} else {
					synthFire.Play();
				}
				lastTimeFiredAudio = now;
			}
		}
	}
}
