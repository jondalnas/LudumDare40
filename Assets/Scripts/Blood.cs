using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blood : MonoBehaviour {
	public float bloodTime = 5f;

	float bloodTimer;
	void Update() {
		if (Game.paused || Game.clearedLevel) return;
		if (bloodTimer < bloodTime) bloodTimer += Time.deltaTime;

		transform.localScale = Vector3.one * Mathf.Lerp(0, 1, bloodTimer / bloodTime);
	}
}
