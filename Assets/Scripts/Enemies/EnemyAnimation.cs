using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour {

	static class TransitionCoditions {
		public static string Idle    = "Idle";
		public static string Walk 	 = "Walk";
		public static string AtkIdle = "AtkIdle";
		public static string Attack  = "Attack";
		public static string Die  	 = "Die";
		public static string Hurt 	 = "Hurt";
	};

	EnemyState m_enemyState;
	Animator m_animator;

	void Awake () {
		m_enemyState = GetComponent <EnemyState> ();
		m_animator = GetComponent <Animator> ();
	}

	// Update is called once per frame
	void Update () {
		if (m_enemyState.IS_IDLE) {
			m_animator.SetBool (TransitionCoditions.Idle, m_enemyState.IS_IDLE);
			m_animator.SetBool (TransitionCoditions.Walk, m_enemyState.IS_WALKING);
		}

		if (m_enemyState.IS_WALKING) {
			m_animator.SetBool (TransitionCoditions.Walk, m_enemyState.IS_WALKING);
		}

		if (m_enemyState.ATTACK_IDLE && m_enemyState.IS_IDLE) {
			m_animator.SetBool (TransitionCoditions.AtkIdle, m_enemyState.ATTACK_IDLE);
		} else {
			m_animator.SetBool (TransitionCoditions.AtkIdle, m_enemyState.ATTACK_IDLE);
		}

		if (m_enemyState.ATTACK) {
			m_animator.SetBool (TransitionCoditions.Attack, m_enemyState.ATTACK);
		} else {
			m_animator.SetBool (TransitionCoditions.Attack, m_enemyState.ATTACK);
		}

		if (m_enemyState.HURT) {
			m_animator.SetBool (TransitionCoditions.Hurt, m_enemyState.HURT);
		}
	}
}
