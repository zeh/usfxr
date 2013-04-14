using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour {

	private const float TIME_ENEMY_SPAWN_INTERVAL_START = 2; // In seconds

	private float timeEnemySpawned;
	private float timeEnemySpawnInterval;

	void Start () {
		timeEnemySpawnInterval = TIME_ENEMY_SPAWN_INTERVAL_START;
    }
	
	void Update () {
		if (Time.realtimeSinceStartup > timeEnemySpawned + timeEnemySpawnInterval) {
			timeEnemySpawnInterval *= 0.99f; // Makes it go faster and faster over time
			SpawnEnemy();
		}
	}

	private void SpawnEnemy() {
		// Creates a random enemy
		timeEnemySpawned = Time.realtimeSinceStartup;

		Instantiate(Resources.Load("Prefabs/Enemy"), transform.position + new Vector3(35, (Random.value - 0.5f) * 46f, 0), Quaternion.identity);
	}
}