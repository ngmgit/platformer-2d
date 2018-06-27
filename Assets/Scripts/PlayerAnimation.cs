using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour {
	
	Animator m_animator;
	InputController m_input;

	// Use this for initialization
	void Awake () {
		m_animator = GetComponent <Animator> ();
		m_input = GetComponent <InputController>();
	}
	
	// Update is called once per frame
	void Update () {
		
		if (m_input.isOnGround) {
			m_animator.SetFloat("moving", Mathf.Abs (m_input.m_horizontal));
		}

		if (m_input.m_jumpPressed == true) {
			m_animator.SetBool ("isJumping", true);
		} else if (m_input.isOnGround == false && m_input.isFalling == true) {
			m_animator.SetBool ("isFalling", true);
		} else {
			m_animator.SetBool ("isJumping", false);
			m_animator.SetBool ("isFalling", false);
		}
			
		
	}
}
