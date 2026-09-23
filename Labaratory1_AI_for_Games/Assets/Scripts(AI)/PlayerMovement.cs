using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    SpriteRenderer sprite;
    Animator animator;

    [SerializeField] private float movSpeed;
    float speedX, speedY;
    Rigidbody2D rb;

    [SerializeField] private float attackRange = 1.5f;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        speedX = Input.GetAxisRaw("Horizontal") * movSpeed;
        speedY = Input.GetAxisRaw("Vertical") * movSpeed;

        if (speedX > 0)
            sprite.flipX = false;

        if (speedX < 0)
            sprite.flipX = true;

        rb.linearVelocity = new Vector2(speedX, speedY);

        animator.SetBool("isWalking", speedX != 0 || speedY != 0);

        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        animator.SetBool("isAttacking", true);
        Collider2D[] enemies = Physics2D.OverlapCircleAll(
        transform.position,
        attackRange
        );

        foreach (Collider2D enemy in enemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                Debug.Log("Enemy hit!");

                EnemyAI enemyScript = enemy.GetComponent<EnemyAI>();

                if (enemyScript != null)
                {
                    enemyScript.Die();
                }
            }
        }

        yield return new WaitForSeconds(0.1f);

        animator.SetBool("isAttacking", false);
    }
    }