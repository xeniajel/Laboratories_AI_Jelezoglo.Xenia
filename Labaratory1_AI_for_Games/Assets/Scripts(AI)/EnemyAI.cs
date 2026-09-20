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

    public float MoveSpeed = 1;
    public float WaitTime = 2;

    private void Awake()
    {
        AnimatorComponent = GetComponent<Animator>();
        SpriteRendererComponent = GetComponent<SpriteRenderer>();

        CurrentPath = Paths[CurrentIndex];
    }

    private void Start()
    {
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
        }
        else if (DirectionToPath.x > 0)
        {
            SpriteRendererComponent.flipX = false;
        }
    }

    Vector2 Direction2Points2D(Vector2 Point1, Vector2 Point2)
    {
        Vector2 Direction2D = Point2 - Point1;
        return Direction2D.normalized;
    }
}