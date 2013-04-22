using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	private const float TIME_TO_FULL_SCALE = 0.8f;
	private const float TIME_TO_FULL_SPEED = 5;
	private const int MIN_SPEED = 60; // units per second
	private const float TIME_TO_LIVE = 30; // In seconds
	private const float BASE_HEALTH = 1;
	private const float TIME_TO_EXPLODE = 1;

	private float scale;
	private int health;
	private float speed;
	private bool isAlive;

	private float spawnTime;
	private float explodeTime;

	void Start() {
		scale = 0.5f + (Random.value * 0.5f);
		scale *= scale;
		scale *= transform.localScale.x;

		health = (int)Mathf.Round(BASE_HEALTH * scale * 0.5f);
		isAlive = true;

		spawnTime = Time.realtimeSinceStartup;

		speed = MIN_SPEED / scale;

		transform.localScale = new Vector3(0, 0, 0);

		SfxrSynth synthSpawn = new SfxrSynth();
		synthSpawn.parameters.SetSettingsString("0,0.03,0.23,0.05,0.42,0.15,0.07,-0.04,-0.02,,,0.02,0.05,0.11,0.1799,0.03,-0.4599,0.3999,0.38,,0.11,0.02,-0.02,0.5");
		if (Main.isModeMutated) {
			synthSpawn.PlayMutated(0.05f);
		} else {
			synthSpawn.Play();
		}
	}
	
	void Update() {
		float speedPassed = Time.deltaTime;

		float speedMult = (Time.realtimeSinceStartup - spawnTime) / TIME_TO_FULL_SPEED;
		if (speedMult > 1) speedMult = 1;

		float scaleMult = (Time.realtimeSinceStartup - spawnTime) / TIME_TO_FULL_SCALE;
		if (scaleMult > 1) scaleMult = 1;
		transform.localScale = new Vector3(scale * scaleMult, scale * scaleMult, scale * scaleMult);

		transform.Translate(speed * -speedPassed * speedMult, 0, 0);

		if (Time.realtimeSinceStartup > spawnTime + TIME_TO_LIVE) Destroy();

		if (!isAlive) {
			if (Time.realtimeSinceStartup > explodeTime + TIME_TO_EXPLODE) {
				// Already exploded
				Destroy();
			} else {
				// Exploding
				float explodePhase = (Time.realtimeSinceStartup - explodeTime) / TIME_TO_EXPLODE;

				float explodePhaseColor = 1-((1-explodePhase) * (1-explodePhase) * (1-explodePhase));
				transform.renderer.material.color = new Color(1, 0, 0, 0.8f * (1-explodePhaseColor));
				//transform.renderer.material.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0.5f * (1-explodePhaseColor));
				//transform.renderer.material.color = new Color(currentColor.r, currentColor.g, currentColor.b, 1-explodePhaseColor);

				float explodePhaseScale = 1-((1-explodePhase) * (1-explodePhase) * (1-explodePhase));
				explodePhaseScale *= scale * 2;

				transform.localScale += new Vector3(explodePhaseScale, explodePhaseScale, explodePhaseScale);
			}
		}

	}

	private void Explode() {
		if (isAlive) {
			isAlive = false;
			explodeTime = Time.realtimeSinceStartup;

			SfxrSynth synth = new SfxrSynth();
			synth.parameters.SetSettingsString("3,0.0228,0.2553,0.186,0.4243,0.0935,,0.0139,0.0292,0.0346,0.0599,-0.3396,0.7684,,-0.0209,,-0.0302,,1,-0.0272,0.0043,,0.0493,0.5");
			if (Main.isModeMutated) {
				synth.PlayMutated();
			} else {
				synth.Play();
			}

		}
	}

	private void Destroy() {
		UnityEngine.Object.Destroy(gameObject);
	}

	void OnTriggerEnter(Collider __collider) {
		if (isAlive) {
			// Hacky way to determine what is it hitting
			if (__collider.gameObject.tag == "DamageEnemies") {
	        	// Hit by a bullet
				health--;
				__collider.gameObject.GetComponent<Bullet>().Die();

				if (health <= 0) {
					Explode();
				}
			}
		}
    }
}
