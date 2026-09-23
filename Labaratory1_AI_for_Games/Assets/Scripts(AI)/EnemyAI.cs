using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    Animator AnimatorComponent;
    SpriteRenderer SpriteRendererComponent;
    Collider2D EnemyCollider;
    private SpriteRenderer playerSpriteRenderer;

    public List<GameObject> Paths;

    int CurrentIndex = 0;
    GameObject CurrentPath;

    [SerializeField] private float MoveSpeed = 1;
    [SerializeField] private float WaitTime = 2;
    private GameObject player;

    [SerializeField] private float AttackCooldown = 0.6f;
    [SerializeField] private float DetectionRange = 5f;
    [SerializeField] private float AttackRange = 1.5f;
    private bool isAttacking = false;


    private bool hasLineOfSight = false;
    protected bool isChasing = false;
    private Vector2 facingDirection = Vector2.right;
    [SerializeField] private float ChaseSpeed = 2f;

    private Vector2 lastPlayerPosition;
    private bool isSearching = false;
    [SerializeField] private float SearchTime = 3f;
    private bool searchRoutineStarted = false;

    private bool isDead = false;

    private void Awake()
    {
        AnimatorComponent = GetComponent<Animator>();
        SpriteRendererComponent = GetComponent<SpriteRenderer>();
        EnemyCollider = GetComponent<Collider2D>();

        CurrentPath = Paths[CurrentIndex];
    }

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerSpriteRenderer = player.GetComponent<SpriteRenderer>();
        StartCoroutine(MoveTo());
    }

    IEnumerator MoveTo()
    {
        AnimatorComponent.Play("Idle");

        yield return new WaitForSeconds(WaitTime);

        AnimatorComponent.Play("Patrol");

        while (Vector2.Distance(transform.position, CurrentPath.transform.position) > 0.05f)
        {
            if (isDead)
                yield break;

            if (isChasing || isSearching)
            {
                yield return null;
                continue;
            }

            Vector2 direction = Direction2Points2D(
                transform.position,
                CurrentPath.transform.position
            );

            transform.position += (Vector3)(direction * MoveSpeed * Time.deltaTime);

            RotateToPath(direction);

            yield return null;
        }

        ChangePlatformDirectionMovement();
    }

    void ChangePlatformDirectionMovement()
    {
        CurrentIndex++;

        if (CurrentIndex >= Paths.Count)
        {
            CurrentIndex = 0;
        }

        CurrentPath = Paths[CurrentIndex];

        StartCoroutine(MoveTo());
    }

    void RotateToPath(Vector2 DirectionToPath)
    {
        if (DirectionToPath.x < 0)
        {
            SpriteRendererComponent.flipX = true;
            facingDirection = Vector2.left;
        }
        else if (DirectionToPath.x > 0)
        {
            SpriteRendererComponent.flipX = false;
            facingDirection = Vector2.right;
        }
    }

    Vector2 Direction2Points2D(Vector2 Point1, Vector2 Point2)
    {
        Vector2 Direction2D = Point2 - Point1;
        return Direction2D.normalized;
    }
    void Update()
    {
        if (isDead)
            return;

        if (isChasing)
        {
            float distanceToPlayer = Vector2.Distance(
                transform.position,
                player.transform.position
            );

            if (distanceToPlayer <= AttackRange && !isAttacking)
            {
                isAttacking = true;
                AnimatorComponent.SetTrigger("Attack");
                StartCoroutine(AttackRoutine());
                StartCoroutine(PlayerHitEffect());
            }
            if (!isAttacking)
            {
                Vector2 directionToPlayer =
                    player.transform.position - transform.position;

                if (directionToPlayer.x < 0)
                {
                    SpriteRendererComponent.flipX = true;
                    facingDirection = Vector2.left;
                }
                else if (directionToPlayer.x > 0)
                {
                    SpriteRendererComponent.flipX = false;
                    facingDirection = Vector2.right;
                }

                transform.position = Vector2.MoveTowards(
                    transform.position,
                    player.transform.position,
                    ChaseSpeed * Time.deltaTime);
            }
        }

        if (isSearching)
        {
            float distanceToLastPosition = Vector2.Distance(
            transform.position,
            lastPlayerPosition
            );

            if (distanceToLastPosition > 0.05f)
            {
                transform.position = Vector2.MoveTowards(
                    transform.position,
                    lastPlayerPosition,
                    MoveSpeed * Time.deltaTime
                );

                AnimatorComponent.Play("Patrol");
            }
            else
            {
                AnimatorComponent.Play("Idle");

                if (!searchRoutineStarted)
                {
                    searchRoutineStarted = true;
                    StartCoroutine(SearchRoutine());
                }
            }
        }
    }

    IEnumerator AttackRoutine()
    {
        yield return new WaitForSeconds(AttackCooldown);

        isAttacking = false;
        AnimatorComponent.Play("Patrol");
    }

    IEnumerator PlayerHitEffect()
    {
        Debug.Log("Player hit!");

        playerSpriteRenderer.color = Color.red;

        yield return new WaitForSeconds(1f);

        playerSpriteRenderer.color = Color.white;
    }

    IEnumerator SearchRoutine()
    {
        AnimatorComponent.Play("Idle");

        yield return new WaitForSeconds(SearchTime);

        isSearching = false;
        searchRoutineStarted = false;
        AnimatorComponent.Play("Patrol");
    }

    protected virtual void FixedUpdate()
    {
        bool wasChasing = isChasing;

        float distanceToPlayer = Vector2.Distance(
            transform.position,
            player.transform.position
        );

        if (distanceToPlayer <= DetectionRange)
        {
            Vector2 directionToPlayer = (player.transform.position - transform.position).normalized;

            float angle = Vector2.Angle(facingDirection, directionToPlayer);

            if (angle <= 90f)
            {
                RaycastHit2D[] rays = Physics2D.RaycastAll(
                    transform.position,
                    directionToPlayer,
                    DetectionRange
                );

                bool playerDetected = false;

                foreach (RaycastHit2D hit in rays)
                {
                    if (hit.collider == EnemyCollider)
                    {
                        continue;
                    }

                    if (hit.collider.CompareTag("Player"))
                    {
                        playerDetected = true;
                        break;
                    }
                }
                ;

                if (playerDetected)
                {
                    hasLineOfSight = true;
                    isSearching = false;
                    isChasing = true;
                    lastPlayerPosition = player.transform.position;
                    Debug.DrawLine(transform.position, player.transform.position, Color.green);
                }
                else
                {
                    hasLineOfSight = false;
                    if (wasChasing && !isSearching)
                    {
                        isSearching = true;
                    }
                    isChasing = false;
                    Debug.DrawLine(transform.position, player.transform.position, Color.red);
                }
            }
            else
            {
                hasLineOfSight = false;
                if (wasChasing && !isSearching)
                {
                    isSearching = true;
                }
                isChasing = false;
                Debug.DrawLine(transform.position, player.transform.position, Color.red);
            }
        }
        else
        {
            hasLineOfSight = false;
            if (wasChasing && !isSearching)
            {
                isSearching = true;
            }
            isChasing = false;
            Debug.DrawLine(transform.position, player.transform.position, Color.red);
        }
    }
    public void Die()
    {
        if (isDead)
            return;

        isDead = true;

        AnimatorComponent.Play("Die");
        Destroy(gameObject, 1f);
    }
}
