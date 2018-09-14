using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformCreator : MonoBehaviour {

	[System.Serializable]
	public class MovePositions
	{
		public Transform[] Points;
	}

	[Range(2,5)]
	public int size = 2;
	public GameObject leftEnd;
	public GameObject rightEnd;
	public GameObject mid;
	public MovePositions mPositions;
	public float speed;
	public bool canMove;

	private Vector3 mBounds;
	private Vector3 midClonePos;
	private int currentMovePointIdx;
	private BoxCollider2D boxCollider;

	// Use this for initialization
	void Start () {
		canMove = true;
		boxCollider = GetComponent<BoxCollider2D>();
		boxCollider.offset = new Vector2 (0.5f, 0f);
		boxCollider.size = new Vector2 (2, 1);

		currentMovePointIdx = 0;

		if (SpritesNotNull())
		{
			mBounds = leftEnd.GetComponent<SpriteRenderer>().bounds.size;

			if (size > 2) {
				SetMidTiles();
			} else {
				midClonePos = new Vector3 (leftEnd.transform.position.x + mBounds.x, leftEnd.transform.position.y, 0);
				mid.SetActive(false);
			}

			rightEnd.transform.position = midClonePos;
		}
	}

	private void Update()
	{
		if (SpritesNotNull() && canMove) {
			Transform currentPoint  = mPositions.Points[currentMovePointIdx];

			if (Vector3.Distance(transform.position, currentPoint.transform.position) <= 0) {
				if (currentMovePointIdx == 0)
					currentMovePointIdx = 1;
				else
					currentMovePointIdx = 0;

				currentPoint = mPositions.Points[currentMovePointIdx];
			}

			transform.position = Vector2.MoveTowards (transform.position, currentPoint.transform.position, speed * Time.deltaTime);
		}
	}

	private bool SpritesNotNull()
	{
		if (leftEnd != null  && rightEnd != null && mid != null)
			return true;

		return false;
	}

	private void SetMidTiles()
	{
		mid.transform.position = new Vector3 (leftEnd.transform.position.x + mBounds.x, leftEnd.transform.position.y , 0);
		SetColliderSize();

		midClonePos = new Vector3 (mid.transform.position.x + mBounds.x, mid.transform.position.y, 0);

		for (int i = 2; i < size; i++)
		{
			// create copy of Mid
			GameObject go = Instantiate(mid, midClonePos, Quaternion.identity, this.gameObject.transform);
			midClonePos = new Vector3 (go.transform.position.x + mBounds.x, go.transform.position.y, 0);
			SetColliderSize();
		}

	}

	private void SetColliderSize()
	{
		Vector2 currentColOffset = boxCollider.offset;
		Vector2 currentColSize = boxCollider.size;

		boxCollider.offset = new Vector2(currentColOffset.x + mBounds.x * 0.5f, currentColOffset.y);
		boxCollider.size = new Vector2(currentColSize.x + mBounds.x, currentColSize.y);
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.tag == "Player")
			other.transform.parent = transform;
	}

	private void OnCollisionExit2D(Collision2D other)
	{
		if (other.gameObject.tag == "Player")
			other.transform.parent = null;
	}
}
