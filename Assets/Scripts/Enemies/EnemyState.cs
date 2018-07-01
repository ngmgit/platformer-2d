using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState : MonoBehaviour {

	public bool IS_IDLE;
	public bool IS_WALKING;
	public bool ATTACK_IDLE;
	public bool ATTACK;
	public bool HURT;
	public bool DIE;

	void Start () {
		IS_IDLE = false;
		IS_WALKING = false;
		ATTACK_IDLE = false;
		ATTACK = false;
		HURT = false;
		DIE = false;
	}
}
