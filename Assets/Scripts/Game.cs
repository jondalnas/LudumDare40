using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour {
	public static bool gameIsRunning;
	public static bool paused;
	public static bool clearedLevel;
	public static int enemies;
	public static int bullets;

	private int currentLevel = 1;
	private float sinceLevelClear;

	void Awake() {
		if (GameObject.Find("GAME HANDLER")!=gameObject) {
			Destroy(gameObject);
			return;
		}

		DontDestroyOnLoad(gameObject);
	}

	float timeSinceLoad;
	void Update() {
		timeSinceLoad += Time.deltaTime;

		if (timeSinceLoad < 2f) return;

		if (Input.GetButtonDown("Pause") && gameIsRunning) {
			paused = !paused;
			GameObject.Find("Canvas").transform.Find("Paused").gameObject.SetActive(paused);
		}

		if (!gameIsRunning) return;

		if (clearedLevel) {
			sinceLevelClear += Time.deltaTime;
			if (sinceLevelClear > 1f && Input.anyKeyDown) startLevel(currentLevel+1);
		} else if (enemies <= 0) {
			clearedLevel = true;
			GameObject.Find("Canvas").transform.Find("Next Level").gameObject.SetActive(true);
		}

		GameObject.Find("Canvas").transform.Find("Bullets").Find("Text").GetComponent<Text>().text = "Bullets: " + Game.bullets;
	}

	public void setGameState(bool paused) {
		Game.paused = paused;
		GameObject.Find("Canvas").transform.Find("Paused").gameObject.SetActive(paused);
	}

	public void close() {
		Application.Quit();
	}

	public void startLevel(int level) {
		currentLevel = level;
		SceneManager.LoadScene("Level"+level);
		gameIsRunning = true;
		paused = false;
		clearedLevel = false;
		bullets = 0;
		timeSinceLoad = 0;
	}

	public void startScene() {
		SceneManager.LoadScene("Start");
		gameIsRunning = false;
		timeSinceLoad = 0;
	}
}
