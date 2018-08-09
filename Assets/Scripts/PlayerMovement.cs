using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour {

	private InputController m_input;

	public int playerSpeed = 10;
	public int jumpForce = 1250;
	public float downRaySize = 0.8f;
	public Transform swordTransform;

	private Rigidbody2D m_playerRb;
	private SpriteRenderer m_playerSpriteRenderer;
	private Animator m_animator;

	private float m_moveX;
	private Vector2 prevPosition;

	// Use this for initialization
	void Awake () {
		m_input = GetComponent <InputController> ();
		m_playerRb = GetComponent <Rigidbody2D> ();
		m_playerSpriteRenderer = GetComponent <SpriteRenderer> ();
		m_animator = GetComponent <Animator> ();
		prevPosition = transform.position;
	}

	void Update () {
		m_moveX = m_input.m_horizontal;
		CheckIfAirborne ();
		CheckIfFalling ();
		ResetIfDead ();
		prevPosition = transform.position;
	}

	// Update is called once per frame
	void FixedUpdate () {
		MovePlayer ();
		PlayerRaycast ();
		ModifyGravity();
	}

	void ModifyGravity () {
		if (m_input.isFalling) {
			m_playerRb.gravityScale = 2.5f;
		}
		if (m_input.isOnGround) {
			m_playerRb.gravityScale = 4f;
		}
	}

	void MovePlayer () {

		if (m_input.m_crouchPressed) {
			if (m_input.isOnGround) {
				m_playerRb.velocity = Vector2.zero;
			}
		} else {
			if (m_input.m_jumpPressed == true && m_input.isOnGround == true) {
				Jump();
			}

			bool attack1Active = m_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack State.Attack1");
			bool attack2Active = m_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack State.Attack2");

			if (!(attack1Active || attack2Active)) {
				m_playerRb.velocity = new Vector2(m_moveX * playerSpeed, m_playerRb.velocity.y);
			} else {
				m_playerRb.velocity = Vector2.zero;
			}

		}

		// flip sprite based on direction facing
		if (m_moveX < 0.0f) {
			m_playerSpriteRenderer.flipX = true;
			swordTransform.localScale = new Vector2 (-1, 1);
		} else if (m_moveX > 0.0f) {
			swordTransform.localScale = new Vector2 (1, 1);
			m_playerSpriteRenderer.flipX = false;
		}

	}

	void Jump () {
		m_input.m_jumpPressed = false;
		if (m_input.isOnGround) {
			m_playerRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
		}
		SetGroundStatus (false);
	}

	void CheckIfFalling () {
		if (!m_input.isOnGround) {
			if (transform.position.y < prevPosition.y) {
				m_input.isFalling = true;
			}
		} else {
			m_input.isFalling = false;
		}
	}

	void CheckIfAirborne () {
		if (!m_input.isOnGround) {
			if (transform.position.y > prevPosition.y) {
				m_input.isInFlight = true;
			}
		} else {
			m_input.isInFlight = false;
		}
	}

	void ResetIfDead () {
		if (this.transform.position.y < -7) {
			SceneManager.LoadScene("SampleScene");
		}
	}

	void PlayerRaycast() {
		RaycastHit2D downRayLeft = Physics2D.Raycast (this.transform.position + new Vector3(-0.4f, 0), Vector2.down, downRaySize);
		RaycastHit2D downRayRight = Physics2D.Raycast (this.transform.position + new Vector3(0.4f, 0), Vector2.down, downRaySize);
		RaycastHit2D downRay = Physics2D.Raycast (this.transform.position, Vector2.down, downRaySize);

		if (downRayRight.collider != null || downRayLeft.collider != null || downRay.collider != null) {
			bool leftCollider = downRayLeft.collider != null && downRayLeft.collider.tag == "Ground&Obstacles";
			bool rightCollider = downRayRight.collider !=null && downRayRight.collider.tag == "Ground&Obstacles";
			bool centerCollider = downRay.collider !=null && downRay.collider.tag == "Ground&Obstacles";

			if (leftCollider || rightCollider || centerCollider) {
				SetGroundStatus (true);
			}
		}
		else {
			SetGroundStatus (false);
		}
	}

	void SetGroundStatus (bool m_status) {
		m_input.isOnGround = m_status;
	}
}
