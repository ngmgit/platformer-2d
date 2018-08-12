using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour {

	// Key Bindings
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
		m_horizontal = InputManager.Forward();
		m_jumpPressed = InputManager.Jump();
		m_crouchPressed  = InputManager.Crouch();
		m_attack1 = InputManager.AttackPrimary();
		m_attack2 = InputManager.AttackSecondary();

		if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
	}
}
