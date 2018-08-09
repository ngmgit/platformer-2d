using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour {

	// Key Bindings
	[HideInInspector] public float m_vertical;
	[HideInInspector] public float m_horizontal;
	[HideInInspector] public bool m_jumpPressed;
	[HideInInspector] public bool m_crouchPressed;
	[HideInInspector] public bool m_slidePressed;
	[HideInInspector] public bool m_attack1;
	[HideInInspector] public bool m_attack2;

	// Status of player
	[HideInInspector] public bool isOnGround;
	[HideInInspector] public bool isFalling;
	[HideInInspector] public bool isInFlight;

	// Update is called once per frame
	void Update () {
		m_horizontal = Input.GetAxis ("Horizontal");
		m_vertical = Input.GetAxis ("Vertical");
		m_jumpPressed = Input.GetKeyDown (KeyCode.W);
		m_crouchPressed  = Input.GetKey (KeyCode.S);
		m_slidePressed = Input.GetKeyDown (KeyCode.LeftShift);
		m_attack1 = Input.GetKeyDown (KeyCode.Mouse0);
		m_attack2 = Input.GetKeyDown (KeyCode.Mouse1);

		if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
	}
}
