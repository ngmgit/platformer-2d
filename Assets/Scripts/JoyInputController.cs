using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JoyInputController : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{

	[SerializeField] Image OJoyImg;
	[SerializeField] Image IJoyImg;

	Vector2 InputDirection;
	InputController m_input;

	public static float m_forward;
	public static bool m_crouch;
	public static bool m_jump;
	public static bool m_attackPrimary;
	public static bool m_attackSecondary;

	void Awake () {
		// if (Application.platform != RuntimePlatform.Android ||
		// 	Application.platform != RuntimePlatform.IPhonePlayer)
		// {
		// 	gameObject.SetActive(false);
		// }

		Input.simulateMouseWithTouches = false;
		InputDirection = Vector2.zero;
		m_input = GameObject.FindGameObjectWithTag("Player").GetComponent<InputController> ();
	}

    public void OnDrag(PointerEventData eventData)
    {
		Vector2 pos = Vector2.zero;

		if (RectTransformUtility.ScreenPointToLocalPointInRectangle (
				OJoyImg.rectTransform,
				eventData.position,
				eventData.pressEventCamera,
				out pos)
			&&
			RectTransformUtility.RectangleContainsScreenPoint (
            	OJoyImg.rectTransform.parent.GetComponent<RectTransform> (),
				eventData.position,
				eventData.pressEventCamera)
			) {

			pos.x = ( pos.x / OJoyImg.rectTransform.sizeDelta.x);
			pos.y = ( pos.y / OJoyImg.rectTransform.sizeDelta.y);

			float x = (OJoyImg.rectTransform.pivot.x == 1) ? pos.x * 2 + 1: pos.x * 2 - 1;
			float y = (OJoyImg.rectTransform.pivot.y == 1) ? pos.y * 2 + 1: pos.y * 2 - 1;

			InputDirection = new Vector2 (x, y);
			InputDirection = (InputDirection.magnitude > 1) ? InputDirection.normalized : InputDirection;

			SetJoyKeyValues ();

			IJoyImg.rectTransform.anchoredPosition = new Vector3 (
				InputDirection.x * (OJoyImg.rectTransform.sizeDelta.x / 3),
				InputDirection.y * (OJoyImg.rectTransform.sizeDelta.y / 3)
				);

		}
    }

	void SetJoyKeyValues () {

		if (InputDirection.x > 0.05f) {
			m_forward = 1.0f;
		} else if (InputDirection.x < -0.05f) {
			m_forward = -1.0f;
		} else {
			m_forward = 0;
		}


		if (InputDirection.y < -0.5) {
			m_crouch = true;
		} else {
			m_crouch = false;
		}

		bool leftSideJump = ( InputDirection.x < 0 && InputDirection.x > -0.75f ) &&
			(InputDirection.y > 0.5f && InputDirection.y < 1.0f);

		bool rightSideJump = ( InputDirection.x > 0 && InputDirection.x < 0.75f ) &&
			(InputDirection.y > 0.5f && InputDirection.y < 1.0f);

		if ( (leftSideJump || rightSideJump) && m_input.isOnGround) {
			m_jump = true;
		} else {
			m_jump = false;
		}
	}

    public void OnPointerDown(PointerEventData eventData)
    {
		OnDrag (eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
		InputDirection = Vector3.zero;
		SetJoyKeyValues ();
		IJoyImg.rectTransform.anchoredPosition = Vector3.zero;
    }

	public void SetAttackPrimary (bool status) {
		Debug.Log (status);
		m_attackPrimary = status;
		//m_attackPrimary = false;
	}

	public void SetAttackSecondary (bool status) {
		m_attackSecondary = status;
		m_attackSecondary = false;
	}

	public void SetUtility (bool status) {
	}
}
