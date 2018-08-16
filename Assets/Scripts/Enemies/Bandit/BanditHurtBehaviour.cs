using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BanditHurtBehaviour : BanditBaseFSM {

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		base.OnStateEnter (animator, stateInfo, layerIndex);
		if (NPCScriptRef.m_health <= 0) {
			NPCScriptRef.SetIdle ();
			NPC.GetComponent <CapsuleCollider2D> ().enabled = false;
        	// disable triggers for attack and obstacle
			Transform[] childLayers = new Transform[NPC.transform.childCount];

			for (int i = 0; i < childLayers.Length; i++) {
				NPC.transform.GetChild(i).GetComponent<BoxCollider2D>().enabled = false;
			}

			animator.SetBool(EnemyAnimation.TransitionCoditions.Die, true);
		}
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		animator.SetBool(EnemyAnimation.TransitionCoditions.Hurt, false);
	}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
