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

    [SerializeField]
    int m_health;

    float m_attackAnimTime;
    float currentSpeed;
    Vector2 m_direction;
    float groundRaySize;
    bool m_GroundCollisionCheck = true;

    Rigidbody2D m_enemyRb;
    CapsuleCollider2D m_collider;
    Animator m_animator;
    EnemyState m_enemyState;


    void Start() {
        currentSpeed = m_speed;
        m_direction = new Vector2(transform.right.x, transform.right.y);
    }

    void Awake () {
        m_animator = GetComponent <Animator> ();
        m_enemyRb = GetComponent <Rigidbody2D> ();
        m_collider = GetComponent <CapsuleCollider2D> ();
        m_enemyState = GetComponent <EnemyState> ();
        m_enemyState.IS_WALKING = true;
        groundRaySize = m_collider.bounds.size.y * 0.75f;
        SetAttackAnimationTime ();
    }

    void SetAttackAnimationTime () {
        RuntimeAnimatorController ac = m_animator.runtimeAnimatorController;    //Get Animator controller
        for(int i = 0; i<ac.animationClips.Length; i++)                 //For all animations
        {
            if(ac.animationClips[i].name == "Attack")        //If it has the same name as your clip
            {
                m_attackAnimTime = ac.animationClips[i].length;
            }
        }
    }


    void Update () {
        Move ();
    }

    void FixedUpdate () {
        CheckBoundariesAndTurn ();
    }

    void Move () {
        if (!m_enemyState.IS_IDLE) {
            m_enemyRb.velocity = m_direction * currentSpeed;
        }
    }

    // To turn user scale is changed and making use of scale value to check the direction
    // NOTE: Make sure the NPC sprite is aligned with the scale direction.
    void FlipEnemy () {
		Vector2 localScale = m_enemyRb.transform.localScale;
		localScale.x *= -1;
		transform.localScale = localScale;
        m_direction = new Vector2(localScale.x , transform.right.y);
    }

    // Two raycasts pointing downwards on either sides of the NPC to make checks on floor ends
    void CheckBoundariesAndTurn () {
        Vector2 bounds = m_collider.bounds.size;
        Vector2 origin1 = new Vector2 ( transform.position.x + bounds.x,  transform.position.y);
        Vector2 origin2 = new Vector2 (transform.position.x - bounds.x, transform.position.y);

        RaycastHit2D groundCheckRayRight = Physics2D.Raycast (origin1, Vector2.down, groundRaySize);
        RaycastHit2D groundCheckRayLeft = Physics2D.Raycast (origin2, Vector2.down, groundRaySize);

        // If any of the raycasts is not on ground
        // Ground check bool is used to avoid the below 'if' statement to execute repeatedly until both of rays are back ground.
        if ((groundCheckRayRight.collider == null || groundCheckRayLeft.collider == null) && m_GroundCollisionCheck) {
            m_GroundCollisionCheck = false;
            FlipEnemy ();
            SetIdle ();
            StartCoroutine ("EnemyWaitDelay");
        }

        if (groundCheckRayLeft.collider != null && groundCheckRayRight.collider != null) {
            m_GroundCollisionCheck = true;
        }
    }

    void CheckAndAttackPlayer () {
        SetIdle ();
        m_enemyState.ATTACK_IDLE = true;
        m_enemyState.ATTACK = true;
        StartCoroutine ("EnemyAtkDelay");
    }

    // Play hurt animation and decrease health value
    void TakeDamage () {
        m_health -= 10;
        if (m_health < 0) {
            Die ();
        }
        m_enemyState.HURT = true;
    }

    void Die () {
        SetIdle ();
        m_enemyState.DIE = true;
    }

    // Make idle state true and velocity set to zero
    void SetIdle () {
        m_enemyRb.velocity = Vector2.zero;
        m_enemyState.IS_IDLE = true;
        m_enemyState.IS_WALKING = false;
    }

    // Set walk and idle state
    void SetWalk () {
        m_enemyState.IS_IDLE = false;
        m_enemyState.IS_WALKING = true;
    }

    void OnTriggerEnter2D (Collider2D other) {

        // Turn and wait for a few seconds before moving
        if (other.gameObject.tag == "Ground&Obstacles") {
            if (!m_enemyState.ATTACK) {
                FlipEnemy ();
                SetIdle ();
                StartCoroutine ("EnemyWaitDelay");
            }
        }

        // If player is nearby disable attack related coroutine before
        if (other.gameObject.tag == "Player") {
            CheckIfEnemyHasToTurn (other.gameObject.transform.position);
            StopCoroutine ("EnemyAtkDelay");
            CheckAndAttackPlayer ();
        }

        // If player attacks with sword
        if (other.gameObject.name == "PlayerSword") {
            TakeDamage ();
        }
    }

    void CheckIfEnemyHasToTurn (Vector2 playerPosition) {
        if (playerPosition.x > transform.position.x && transform.localScale.x == -1) {
            FlipEnemy ();
        }

        if (playerPosition.x < transform.position.x && transform.localScale.x == 1) {
            FlipEnemy ();
        }
    }

    IEnumerator EnemyWaitDelay () {
        yield return new WaitForSeconds(2f);
        SetWalk ();
    }

    // Let the attack animation execute and after that disable attack and attackdelay state
    IEnumerator EnemyAtkDelay () {
        yield return new WaitForSeconds(m_attackAnimTime);
        m_enemyState.ATTACK = false;
        m_enemyState.ATTACK_IDLE = false;
        SetWalk ();
    }
}