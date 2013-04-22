using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour {

	private const float TIME_ENEMY_SPAWN_INTERVAL_START = 2; // In seconds
	private const float TIME_ENEMY_SPAWN_INTERVAL_MIN = 0.5f; // In seconds

	private float timeEnemySpawned;
	private float timeEnemySpawnInterval;

	public static bool isModeMutated;

	void Start() {
		timeEnemySpawnInterval = TIME_ENEMY_SPAWN_INTERVAL_START;
		isModeMutated = true;
		UpdateGUI();
    }
	
	void Update() {
		if (Time.realtimeSinceStartup > timeEnemySpawned + timeEnemySpawnInterval) {
			timeEnemySpawnInterval *= 0.97f; // Makes it go faster and faster over time
			if (timeEnemySpawnInterval < TIME_ENEMY_SPAWN_INTERVAL_MIN) timeEnemySpawnInterval = TIME_ENEMY_SPAWN_INTERVAL_MIN;
			SpawnEnemy();
		}

		if (Input.GetKeyDown(KeyCode.Z)) {
			Main.isModeMutated = !isModeMutated;
			UpdateGUI();
		}
	}

	private void UpdateGUI() {
		GUIText guiText = (GUIText) GameObject.Find("GUIText").GetComponent("GUIText");
		guiText.text = "Z: Mutated mode [" + (isModeMutated ? "ON" : "OFF") + "]";
	}

	private void SpawnEnemy() {
		// Creates a random enemy
		timeEnemySpawned = Time.realtimeSinceStartup;

		Instantiate(Resources.Load("Prefabs/Enemy"), transform.position + new Vector3(35, (Random.value - 0.5f) * 46f, 0), Quaternion.identity);
	}
}