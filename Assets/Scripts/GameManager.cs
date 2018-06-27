using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public Text scoreHolder;
	int score = 0;
	// Use this for initialization
	void Awake () {
		GameObject[] pickups = GameObject.FindGameObjectsWithTag("Pickups");
		foreach(GameObject pickup in pickups) {
			pickup.AddComponent<Pickup> ();
		}
	}

	public void SetScore (int value) {
		score = score + value;
		scoreHolder.text = "Score: " + score.ToString();
	}
}
