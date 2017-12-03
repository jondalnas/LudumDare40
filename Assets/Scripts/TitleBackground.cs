using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleBackground : MonoBehaviour {
	public Color color0;
	public Color color1;
	public float maxTime = 5f;

	float timeSinceStart;
	void Update() {
		timeSinceStart += Time.deltaTime;
		Color color = Color.Lerp(color0, color1, (Mathf.Sin(timeSinceStart/maxTime*Mathf.PI*2)+1)/2);

		GetComponent<Image>().color = color;
	}
}
