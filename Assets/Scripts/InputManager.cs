using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class InputManager
{

	public static float Forward () {
		float kb = Input.GetAxis ("Horizontal");

		kb += JoyInputController.m_forward;
		kb = Mathf.Clamp(kb, -1f, 1f);

		return kb;
	}

	public static bool Jump() {
		bool kb = Input.GetButtonDown("K_Jump") || JoyInputController.m_jump;
		//private joy

		return kb;
	}

	public static bool Crouch() {
		bool kb = Input.GetButton("K_Crouch") || JoyInputController.m_crouch;
		//private joy

		return kb;
	}

	public static bool AttackPrimary() {
		bool kb = Input.GetButtonDown ("AttackPrimary") || JoyInputController.m_attackPrimary;
		//private joy

		return kb;
	}

	public static bool AttackSecondary() {
		bool kb = Input.GetButtonDown ("AttackSecondary") || JoyInputController.m_attackSecondary;
		//private joy

		return kb;
	}

}
