using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour {

	[HideInInspector] public float m_vertical;
	[HideInInspector] public float m_horizontal;
	[HideInInspector] public bool m_jumpPressed;
	[HideInInspector] public bool isOnGround = false;
	[HideInInspector] public bool isFalling = false;

	// Update is called once per frame
	void Update () {
		m_horizontal = Input.GetAxis ("Horizontal");
		m_vertical = Input.GetAxis ("Vertical");
		m_jumpPressed = Input.GetKeyDown (KeyCode.W);
	}
}
