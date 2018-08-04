using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour {
	public float parallaxSpeed = 0.125f;

	private Transform cameraTransform;
	private Transform[] bglayers;
	private float viewZone = 10;
	private int leftIndex;
	private int rightIndex;
	private float positionOffset;
	private float LastCameraX;

	// Use this for initialization
	void Start () {
		cameraTransform = Camera.main.transform;
		bglayers = new Transform[transform.childCount];

		for (int i = 0; i < transform.childCount; i++) {
			bglayers[i] = transform.GetChild(i);
		}
		LastCameraX = Camera.main.transform.position.x;
		positionOffset =  bglayers[0].GetComponent<SpriteRenderer>().bounds.size.x;
		leftIndex = 0;
		rightIndex = bglayers.Length - 1;
	}

	// Update is called once per frame
	void Update () {
		float cameraDeltaX = LastCameraX - Camera.main.transform.position.x;

		transform.position += new Vector3 (cameraDeltaX * parallaxSpeed, 0, 0) ;
		LastCameraX = Camera.main.transform.position.x;

		if (Camera.main.transform.position.x > bglayers[rightIndex].position.x) {
			scrollRight ();
		}

		if (Camera.main.transform.position.x < bglayers[leftIndex].position.x) {
			ScrollLeft ();
		}
	}

	private void ScrollLeft () {
		transform.localScale = new Vector2 (transform.localScale.x * -1, transform.localScale.y);
		bglayers[rightIndex].transform.position = new Vector2 (bglayers [leftIndex].position.x - positionOffset, bglayers[leftIndex].position.y);
		bglayers[rightIndex].localScale = new Vector2 (bglayers[rightIndex].localScale.x * -1, bglayers[rightIndex].localScale.y);
		leftIndex = rightIndex;
		rightIndex--;
		if (rightIndex < 0) {
			rightIndex = bglayers.Length - 1;
		}
	}

	private void scrollRight () {

		bglayers[leftIndex].transform.position = new Vector2 (bglayers [rightIndex].position.x + positionOffset, bglayers[rightIndex].position.y);
		bglayers[leftIndex].localScale = new Vector2 (bglayers[leftIndex].localScale.x * -1, bglayers[leftIndex].localScale.y);
		rightIndex = leftIndex;
		leftIndex++;
		if (leftIndex == bglayers.Length) {
			leftIndex = 0;
		}
	}
}
