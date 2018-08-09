using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BanditAtkIdleBehaviour : BanditBaseFSM {

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		base.OnStateEnter (animator, stateInfo, layerIndex);
		NPCScriptRef.SetIdle();
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if (!NPCScriptRef.isCollidingWithObstacle) {
			NPCScriptRef.CheckIfEnemyHasToTurn (PlayerObject.transform.position);
		}

		if (Vector2.Distance(animator.transform.position,PlayerObject.transform.position) < 2) {
			animator.SetBool (EnemyAnimation.TransitionCoditions.Attack, true);
		} else if (Vector2.Distance(animator.transform.position,PlayerObject.transform.position) > 5) {
			animator.SetBool (EnemyAnimation.TransitionCoditions.Walk, true);
		}
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		animator.SetBool (EnemyAnimation.TransitionCoditions.AtkIdle, false);
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
