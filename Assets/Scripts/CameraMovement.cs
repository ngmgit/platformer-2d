using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CameraFollowThresholds {
	public Vector2 min;
	public Vector2 max;
}

public class CameraMovement : MonoBehaviour {

	public Transform m_playerTrans;
	public Transform minClamp;
	public Transform maxClamp;
	public CameraFollowThresholds cameraThresholds;
	public float m_smoothTime = 0.15f;

	private Vector3 m_velocity = Vector2.zero;
	private Vector2 minClampCalcPos;
	private Vector3 maxClampCalcPos;
	private InputController m_input;
	private bool boundsTouched;
	public float currentSmoothDamp;

	void Awake () {
		GameObject PlayerObj = GameObject.FindGameObjectWithTag("Player");
		if (PlayerObj) {
			m_input = PlayerObj.GetComponent<InputController> ();
		}
	}

	// Use this for initialization
	void Start () {
		transform.position = new Vector3 (m_playerTrans.position.x, m_playerTrans.position.y, transform.position.z);;

		currentSmoothDamp = m_smoothTime;
		float screenHeightInUnits = Camera.main.orthographicSize * 2;
		float screenWidthInUnits = screenHeightInUnits * Screen.width/ Screen.height;

		//based on camera aspect ratio and boundary limits get the camera position
		minClampCalcPos.x = minClamp.position.x + screenWidthInUnits / 2;
		maxClampCalcPos.x = maxClamp.position.x - screenWidthInUnits / 2;
		minClampCalcPos.y = minClamp.position.y + screenHeightInUnits / 2;
		maxClampCalcPos.y = maxClamp.position.y - screenHeightInUnits / 2;
	}

	void FixedUpdate () {
		float x = Mathf.Clamp (m_playerTrans.position.x, minClampCalcPos.x, maxClampCalcPos.x);
		float y = this.transform.position.y;

		PlayerTouchesBounds ();

		// above threshold
		if (boundsTouched) {
			y = Mathf.Clamp (m_playerTrans.position.y, minClampCalcPos.y, maxClampCalcPos.y);

		// below threshold
		} else if (m_playerTrans.position.y < transform.position.y - cameraThresholds.min.y) {
			y = Mathf.Clamp (m_playerTrans.position.y, minClampCalcPos.y, maxClampCalcPos.y);
		}

		Vector3 targetPos = new Vector3 (x, y, this.transform.position.z);
		this.transform.position = Vector3.SmoothDamp (this.transform.position, targetPos, ref m_velocity, currentSmoothDamp, Mathf.Infinity, Time.fixedDeltaTime);

	}

	// Set flag to track player position
	// Also change smooth time when camera tracks player and still in flight.
	void PlayerTouchesBounds () {
		if (m_playerTrans.position.y >= transform.position.y + cameraThresholds.max.y) {
			if (!boundsTouched) {
				currentSmoothDamp = m_smoothTime * 4f;
			}
			boundsTouched = true;
		}

		if (boundsTouched && (m_input.isFalling || !m_input.isInFlight)) {
			currentSmoothDamp = m_smoothTime;
			boundsTouched = false;
		}
	}
}
