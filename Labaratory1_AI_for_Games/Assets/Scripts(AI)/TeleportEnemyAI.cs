using UnityEngine;
using System.Collections;

public class TeleportEnemyMove : EnemyAI
{
    [SerializeField] private float ScanRadius = 1f;
    [SerializeField] private float ScanInterval = 0.5f;

    [SerializeField] private float TeleportCooldown = 3f;
    [SerializeField] private float MaxTeleportDistance = 3f;

    private bool canTeleport = true;
    private bool detectedBy360Scan = false;

    private void ScanForPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        float distance = Vector2.Distance(
            transform.position,
            player.transform.position
        );

        Debug.Log("Distance to player: " + distance);

        if (distance <= ScanRadius)
        {
            Debug.Log("Player detected by 360 scan!");

            detectedBy360Scan = true;
            isChasing = true;
        }
    }

    private void Teleport()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized;

        Vector2 newPosition =
            (Vector2)transform.position +
            randomDirection * MaxTeleportDistance;

        transform.position = newPosition;

        Debug.Log("Enemy teleported!");
    }

    private void Start()
    {
        base.Start();
        StartCoroutine(ScanRoutine());
        StartCoroutine(TeleportRoutine());
    }

    private IEnumerator ScanRoutine()
    {
        while (true)
        {
            ScanForPlayer();

            yield return new WaitForSeconds(ScanInterval);
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (detectedBy360Scan)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            float distance = Vector2.Distance(
                transform.position,
                player.transform.position
            );

            if (distance <= 5f)
            {
                isChasing = true;
            }
            else
            {
                detectedBy360Scan = false;
                isChasing = false;
            }
        }
    }

    private IEnumerator TeleportCooldownRoutine()
    {
        yield return new WaitForSeconds(TeleportCooldown);

        canTeleport = true;
    }

    private IEnumerator TeleportRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (canTeleport && !isChasing)
            {
                Teleport();

                canTeleport = false;
                StartCoroutine(TeleportCooldownRoutine());
            }
        }
    }
}
