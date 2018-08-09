using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum EnemyType {
    ZOMBIES,
    SORCERER,
    BANDITS
};

public class Enemy : MonoBehaviour {

    public float m_speed;
    public EnemyType m_type;
    public int m_damage;
    public bool isCollidingWithObstacle;

    [SerializeField]
    int m_health;

    float currentSpeed;
    Vector2 m_direction;
    float groundRaySize;

    Rigidbody2D m_enemyRb;
    CapsuleCollider2D m_collider;
    Animator m_animator;

    void Start() {
        currentSpeed = m_speed;
        m_direction = new Vector2(transform.right.x, transform.right.y);
    }

    void Awake () {
        m_animator = GetComponent <Animator> ();
        m_enemyRb = GetComponent <Rigidbody2D> ();
        m_collider = GetComponent <CapsuleCollider2D> ();
        m_animator.SetBool (EnemyAnimation.TransitionCoditions.Walk, true);
        groundRaySize = m_collider.bounds.size.y * 0.75f;
        Debug.Log("");
    }

    public void Move () {
        m_enemyRb.velocity = m_direction * currentSpeed;
    }

    // To turn user scale is changed and making use of scale value to check the direction
    // NOTE: Make sure the NPC sprite is aligned with the scale direction.
    public void FlipEnemy () {
		Vector2 localScale = m_enemyRb.transform.localScale;
		localScale.x *= -1;
		transform.localScale = localScale;
        m_direction = new Vector2(localScale.x , transform.right.y);
    }

    // Two raycasts pointing downwards on either sides of the NPC to make checks on floor ends
    public void CheckGroundEnds () {
        LayerMask targetLayer = 1 << LayerMask.NameToLayer("Platform");
        Vector2 bounds = m_collider.bounds.size;
        Vector2 origin1 = new Vector2 ( transform.position.x + bounds.x,  transform.position.y);
        Vector2 origin2 = new Vector2 (transform.position.x - bounds.x, transform.position.y);

        RaycastHit2D groundCheckRayRight = Physics2D.Raycast (origin1, Vector2.down, groundRaySize, targetLayer);
        RaycastHit2D groundCheckRayLeft = Physics2D.Raycast (origin2, Vector2.down, groundRaySize, targetLayer);

        // If any of the raycasts is not on ground
        // Ground check bool is used to avoid the below 'if' statement to execute repeatedly until both of rays are back ground.
        if ((groundCheckRayRight.collider == null || groundCheckRayLeft.collider == null)) {
            CheckAndFlip (groundCheckRayLeft, groundCheckRayRight);
        }
    }

    void CheckAndFlip (RaycastHit2D LeftRay, RaycastHit2D RightRay) {
        if (LeftRay.collider == null) {
            // check if the player position is set to right side i.e scale = 1
            Vector2 localScale = m_enemyRb.transform.localScale;
            if (localScale.x != 1) {
                localScale.x = 1;
                transform.localScale = localScale;
            }

            if (m_direction.x != 1) {
                m_direction = new Vector2(localScale.x , transform.right.y);
            }
        }

        if (RightRay.collider == null) {
            // check if player position is set to left sidee i.e scale = -1
            Vector2 localScale = m_enemyRb.transform.localScale;
		    if (localScale.x != -1) {
                localScale.x = -1;
                transform.localScale = localScale;
            }
            if (m_direction.x != -1) {
                m_direction = new Vector2(localScale.x , transform.right.y);
            }
        }
    }

    public void CheckIfEnemyHasToTurn (Vector2 playerPosition) {
        if (playerPosition.x > transform.position.x && transform.localScale.x == -1) {
            FlipEnemy ();
        }
        if (playerPosition.x < transform.position.x && transform.localScale.x == 1) {
            FlipEnemy ();
        }
    }

    // Play hurt animation and decrease health value
    void TakeDamage () {
        m_health -= 1;
        if (m_health < 0) {
            Die ();
        }
    }

    void Die () {
        SetIdle ();
    }

    public void SetIdle () {
        m_enemyRb.velocity = Vector2.zero;
    }

    public void IdleDelay () {
        StartCoroutine("IdleDelayCR");
    }

    IEnumerator IdleDelayCR () {
        yield return new WaitForSeconds(2);
        m_animator.SetBool (EnemyAnimation.TransitionCoditions.Walk, true);
    }

    void OnTriggerEnter2D (Collider2D other) {
        // If player is nearby disable attack related coroutine before
        if (other.gameObject.tag == "Player") {
            // Trigger Attack Transition
            StopCoroutine("IdleDelayCR");
            m_animator.SetBool(EnemyAnimation.TransitionCoditions.AtkIdle, true);
        }

        // Turn and wait for a few seconds before moving
        if (other.gameObject.tag == "Ground&Obstacles") {
            isCollidingWithObstacle = true;
            m_animator.SetBool (EnemyAnimation.TransitionCoditions.Idle, true);
        }

        // If player attacks with sword
        if (other.gameObject.tag == "PlayerSword") {
            StopCoroutine("IdleDelayCR");
            TakeDamage();
        }
    }

    void OnTriggerExit2D (Collider2D other) {
       if (other.gameObject.tag == "Ground&Obstacles") {
            isCollidingWithObstacle = false;
        }
    }
}