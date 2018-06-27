using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour {

	private InputController m_input; 

	public int playerSpeed = 10;
	public int jumpForce = 1250;
	public float downRaySize = 0.8f;
	public bool isOnGround = false;
	public bool isFalling = false;

	private Rigidbody2D m_playerRb;
	private SpriteRenderer m_playerSpriteRenderer;

	private float m_moveX;
	private bool m_facingRight = true;
	private bool m_jumpPressed = false;
	private Vector2 prevPosition;
	
	// Use this for initialization
	void Start () {
		m_input = GetComponent <InputController> ();
		m_playerRb = GetComponent <Rigidbody2D> ();
		m_playerSpriteRenderer = GetComponent <SpriteRenderer> ();
		prevPosition = transform.position;
	}
	
	void Update () {
		m_moveX = m_input.m_horizontal;
		m_jumpPressed = m_input.m_jumpPressed;
		CheckIfFalling ();
		ResetIfDead ();
	}

	// Update is called once per frame
	void FixedUpdate () {
		MovePlayer ();
		PlayerRaycast ();
	}

	void MovePlayer () {
		
		if (m_jumpPressed == true && isOnGround == true) {
			Jump();
		}

		if (m_moveX < 0.0f) {
			m_playerSpriteRenderer.flipX = true; 
		} else if (m_moveX > 0.0f) {
			m_playerSpriteRenderer.flipX = false;
		}

		m_playerRb.velocity = new Vector2(m_moveX * playerSpeed, m_playerRb.velocity.y);
	}

	void Jump () {
		m_jumpPressed = false;
		if (isOnGround) {
			m_playerRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
		}
		SetGroundStatus (false);
	}

	void CheckIfFalling () {
		if (!isOnGround) {
			if (transform.position.y < prevPosition.y) {
				isFalling = true;
				m_input.isFalling = true;
			} 
		} else {
			isFalling = false;
			m_input.isFalling = false;
		}
		prevPosition = transform.position;
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
		Debug.DrawRay(this.transform.position + new Vector3(-0.5f, 0), Vector2.down);
		Debug.DrawRay(this.transform.position + new Vector3(0.5f, 0), Vector2.down);
		Debug.DrawRay(this.transform.position, Vector2.down);

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
		isOnGround = m_status;
		m_input.isOnGround = m_status;
	}
}
