using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	private const float TIME_TO_FULL_SCALE = 0.8f;
	private const float TIME_TO_FULL_SPEED = 5;
	private const int MIN_SPEED = 60; // units per second
	private const float TIME_TO_LIVE = 10; // In seconds
	private const float MAX_HEALTH = 1000;

	private bool modeMutated;
	private float scale;
	private float health;
	private float speed;

	private float spawnTime;

	void Start () {
		modeMutated = true;

		scale = 0.5f + (Random.value * 0.5f);
		scale *= scale;
		scale *= transform.localScale.x;

		health = MAX_HEALTH * scale;

		spawnTime = Time.realtimeSinceStartup;

		speed = MIN_SPEED / scale;

		transform.localScale = new Vector3(0, 0, 0);

		SfxrSynth synthSpawn = new SfxrSynth();
		synthSpawn.parameters.SetSettingsString("0,0.03,0.23,0.05,0.42,0.15,0.07,-0.04,-0.02,,,0.02,0.05,0.11,0.1799,0.03,-0.4599,0.3999,0.38,,0.11,0.02,-0.02,0.5");
		if (modeMutated) {
			synthSpawn.PlayMutated(0.05f);
		} else {
			synthSpawn.Play();
		}
	}
	
	void Update () {
		float speedPassed = Time.deltaTime;

		float speedMult = (Time.realtimeSinceStartup - spawnTime) / TIME_TO_FULL_SPEED;
		if (speedMult > 1) speedMult = 1;

		float scaleMult = (Time.realtimeSinceStartup - spawnTime) / TIME_TO_FULL_SCALE;
		if (scaleMult > 1) scaleMult = 1;
		transform.localScale = new Vector3(scale * scaleMult, scale * scaleMult, scale * scaleMult);

		transform.Translate(speed * -speedPassed * speedMult, 0, 0);

		if (Time.realtimeSinceStartup > spawnTime + TIME_TO_LIVE) Die(false);
	}

	private void Die(bool __withExplosion) {
		UnityEngine.Object.Destroy(gameObject);

		if (__withExplosion) {
		}
	}
}
