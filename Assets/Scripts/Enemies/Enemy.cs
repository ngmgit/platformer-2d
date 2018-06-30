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
    int health;

    float groundRaySize;
    bool m_GroundCollisionCheck = true;
    Rigidbody2D m_enemyRb;
    SpriteRenderer enemySprite;
    CapsuleCollider2D m_collider;

    void Start() {
        m_enemyRb = GetComponent <Rigidbody2D> ();
        enemySprite = GetComponent <SpriteRenderer> ();
        m_collider = GetComponent <CapsuleCollider2D> ();
        groundRaySize = m_collider.bounds.size.y * 0.75f;
    }


    void Update () {
        // move left to right
        Move ();
        // raycast to check boundaries
        // raycast to attack
        // check if dead
    }

    void FixedUpdate () {
        CheckBoundariesAndTurn ();
    }
    void Move () {
        Vector2 direction = new Vector2 ( transform.right.x, transform.right.y);
        m_enemyRb.velocity = direction * m_speed;
    }

    void FlipEnemy () {
        enemySprite.flipX = !enemySprite.flipX;
         m_speed *= -1;
    }

    void CheckBoundariesAndTurn () {
        Vector2 bounds = m_collider.bounds.size;
        Vector2 origin1 = new Vector2 ( transform.position.x + bounds.x,  transform.position.y);
        Vector2 origin2 = new Vector2 (transform.position.x - bounds.x, transform.position.y);

        RaycastHit2D groundCheckRayRight = Physics2D.Raycast (origin1, Vector2.down, groundRaySize);
        RaycastHit2D groundCheckRayLeft = Physics2D.Raycast (origin2, Vector2.down, groundRaySize);

        if ((groundCheckRayRight.collider == null || groundCheckRayLeft.collider == null) && m_GroundCollisionCheck) {
            m_GroundCollisionCheck = false;
            FlipEnemy ();
        }

        if (groundCheckRayLeft.collider != null && groundCheckRayRight.collider != null) {
            m_GroundCollisionCheck = true;
        }
    }

    void CheckAndAttackPlayer () {

    }

    void TakeDamage () {

    }

    void Die () {

    }

    void OnTriggerEnter2D (Collider2D other) {
        if (other.gameObject.tag == "Ground&Obstacles") {
            FlipEnemy ();
        }
    }
}