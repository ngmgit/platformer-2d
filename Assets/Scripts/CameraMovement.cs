using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CameraClamps {
	public float xMin = Mathf.Infinity; 
	public float xMax = Mathf.Infinity; 
	public float yMin = Mathf.Infinity; 
	public float yMax = Mathf.Infinity;
}

public class CameraMovement : MonoBehaviour {

	public Transform m_playerTrans;
	public CameraClamps m_cameraClamps;
	public float m_smoothTime = 0.15f;

	private Vector3 m_velocity = Vector2.zero;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		float xMinClamp = m_cameraClamps.xMin != Mathf.Infinity ? m_cameraClamps.xMin : m_playerTrans.position.x;
		float xMaxClamp = m_cameraClamps.xMax != Mathf.Infinity ? m_cameraClamps.xMax : m_playerTrans.position.x;
		float yMinClamp = m_cameraClamps.yMin != Mathf.Infinity ? m_cameraClamps.yMin : m_playerTrans.position.y;
		float yMaxClamp = m_cameraClamps.yMax != Mathf.Infinity ? m_cameraClamps.yMin : m_playerTrans.position.y;

		float x = Mathf.Clamp (m_playerTrans.position.x, xMinClamp, xMaxClamp);
		float y = Mathf.Clamp (m_playerTrans.position.y, yMinClamp, yMaxClamp);
		
		Vector3 targetPos = new Vector3 (x, y, this.transform.position.z);
		//this.transform.position = new Vector3 (x, y, this.transform.position.z);
		this.transform.position = Vector3.SmoothDamp (this.transform.position, targetPos, ref m_velocity, m_smoothTime, Mathf.Infinity, Time.fixedDeltaTime);
	}
}
