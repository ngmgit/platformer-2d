using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour {

	Animator m_animator;
	InputController m_input;

	static class TransitionCoditions {
		public static string moving 	 	= "moving";
		public static string isJumpPressed  = "isJumpPressed";
		public static string isFalling      = "isFalling";
		public static string isGrounded  	= "isGrounded";
		public static string isCrouching 	= "isCrouching";
		public static string isSliding 	 	= "isSliding";
		public static string isInFlight  	= "isInFlight";
		public static string Attack         = "Attack";
		public static string AttackType     = "AttackType";
	};

	bool prevJumpState;
	int currentAttackT;

	// Use this for initialization
	void Awake () {
		m_animator = GetComponent <Animator> ();
		m_input = GetComponent <InputController>();

		currentAttackT = 0;
	}

	// Update is called once per frame
	void Update () {
		m_animator.SetBool (TransitionCoditions.isGrounded, m_input.isOnGround);

		SetWalk ();

		SetJump ();

		SetCrouch ();
		// SetSlide ();

		SetAttack ();
	}


	void SetWalk () {
		if (m_input.isOnGround && !m_input.m_crouchPressed) {
			m_animator.SetFloat (TransitionCoditions.moving, Mathf.Abs (m_input.m_horizontal));
		}
	}

	void SetJump () {

		// as soon as jump is pressed activate jump state
		if (m_input.isOnGround && m_input.m_jumpPressed) {
			m_animator.SetBool (TransitionCoditions.isJumpPressed, true);
		// When in air and falling state
		} else if (!m_input.isOnGround && m_input.isFalling) {
			m_animator.SetBool (TransitionCoditions.isFalling, true);
		// when neither state, on ground
		} else {
			m_animator.SetBool (TransitionCoditions.isJumpPressed, false);
			m_animator.SetBool (TransitionCoditions.isFalling, false);
		}

		m_animator.SetBool (TransitionCoditions.isInFlight, m_input.isInFlight);
	}

	void SetCrouch () {
		m_animator.SetBool (TransitionCoditions.isCrouching, m_input.m_crouchPressed);
	}

	void SetAttack () {
		if (m_input.m_attack1) {
			currentAttackT = currentAttackT == 0? 1: 0;
		}

		if (m_input.m_attack2) {
			currentAttackT = 2;
		}

		m_animator.SetBool (TransitionCoditions.Attack, m_input.m_attack1 || m_input.m_attack2);
		m_animator.SetInteger (TransitionCoditions.AttackType, currentAttackT);
	}
}
