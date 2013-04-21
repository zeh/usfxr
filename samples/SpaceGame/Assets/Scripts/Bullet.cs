using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	private const int SPEED = 60; // units per second
	private const float TIME_TO_LIVE = 3; // In seconds

	private float spawnTime;

	void Start() {
		// Rotates the bullet a little bit
		transform.Rotate(0, 0, (Random.value - 0.5f) * 10 + 90);

		spawnTime = Time.realtimeSinceStartup;
	}
	
	void Update() {
		float speedPassed = Time.deltaTime;

		transform.Translate(0, SPEED * -speedPassed, 0);

		if (Time.realtimeSinceStartup > spawnTime + TIME_TO_LIVE) {
			Die();
		}
	}

	public void Die() {
		UnityEngine.Object.Destroy(gameObject);
	}

}
