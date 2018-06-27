using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour {

	private GameManager gameManager;

	void Awake () {
		GameObject gameManagerObject = GameObject.FindGameObjectWithTag ("GameController");
		if (gameManagerObject != null)
		{
			gameManager = gameManagerObject.GetComponent <GameManager>();
		}
	}

	void OnTriggerEnter2D (Collider2D col) {
		if (col.gameObject.tag != null) {
			if (col.gameObject.tag == "Player") {
				GetComponent<Collider2D>().enabled = false;
				gameManager.SetScore(10);

				Destroy(gameObject);
				Debug.Log("Hope its disabld!");
			}
		}
	}
}
