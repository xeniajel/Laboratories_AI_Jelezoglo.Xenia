using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Animator AnimatorComponent;
    SpriteRenderer SpriteRendererComponent;

    public List<GameObject> Paths;

    int CurrentIndex = 0;
    GameObject CurrentPath;

    [SerializeField] private float MoveSpeed = 1;
    [SerializeField] private float WaitTime = 2;
    private GameObject player;

    [SerializeField] private float DetectionRange = 5f;
    private bool hasLineOfSight = false;
    private Vector2 facingDirection = Vector2.right;

    private void Awake()
    {
        AnimatorComponent = GetComponent<Animator>();
        SpriteRendererComponent = GetComponent<SpriteRenderer>();

        CurrentPath = Paths[CurrentIndex];
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(MoveTo());
    }

    IEnumerator MoveTo()
    {
        // Останавливаемся
        AnimatorComponent.Play("Idle");

        yield return new WaitForSeconds(WaitTime);

        // Начинаем движение
        AnimatorComponent.Play("Patrol");

        while (Vector2.Distance(transform.position, CurrentPath.transform.position) > 0.05f)
        {
            Vector2 direction = Direction2Points2D(
                transform.position,
                CurrentPath.transform.position
            );

            transform.position += (Vector3)(direction * MoveSpeed * Time.deltaTime);

            RotateToPath(direction);

            yield return null;
        }

        // Пришли к точке
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
        
    }

    void FixedUpdate()
    {
        float distanceToPlayer = Vector2.Distance (
            transform.position,
            player.transform.position
        );

        if (distanceToPlayer <= DetectionRange)
        {
            Vector2 directionToPlayer = (player.transform.position - transform.position).normalized;

            float angle = Vector2.Angle(facingDirection, directionToPlayer);

            if(angle <= 90f)
            {
                RaycastHit2D ray = Physics2D.Raycast(
                    transform.position,
                    directionToPlayer
                );

                if (ray.collider != null && ray.collider.CompareTag("Player"))
                {
                    hasLineOfSight = true;
                    Debug.DrawLine(transform.position, player.transform.position, Color.green);
                }
                else
                {
                    hasLineOfSight = false;
                    Debug.DrawLine(transform.position, player.transform.position, Color.red);
                }
            }
            else
            {
                hasLineOfSight = false;
                Debug.DrawLine(transform.position, player.transform.position, Color.red);
            }
        }
        else
        {
            hasLineOfSight = false;
            Debug.DrawLine(transform.position, player.transform.position, Color.red);
        }
    }
}