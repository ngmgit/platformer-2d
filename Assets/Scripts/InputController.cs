using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour {

	// Key Bindings
	[HideInInspector] public float m_vertical;
	[HideInInspector] public float m_horizontal;
	[HideInInspector] public bool m_jumpPressed;
	[HideInInspector] public bool m_crouchPressed = false;
	[HideInInspector] public bool m_slidePressed = false;

	// Status of player
	[HideInInspector] public bool isOnGround = false;
	[HideInInspector] public bool isFalling = false;
	[HideInInspector] public bool isInFlight = false;

	// Update is called once per frame
	void Update () {
		m_horizontal = Input.GetAxis ("Horizontal");
		m_vertical = Input.GetAxis ("Vertical");
		m_jumpPressed = Input.GetKeyDown (KeyCode.W);
		m_crouchPressed  = Input.GetKey (KeyCode.S);
		m_slidePressed = Input.GetKeyDown (KeyCode.LeftShift);

		if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
	}
}
