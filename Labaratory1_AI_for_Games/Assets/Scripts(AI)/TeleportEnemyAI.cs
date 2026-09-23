using UnityEngine;
using System.Collections;

public class TeleportEnemyMove : EnemyMove
{
    [SerializeField] private float ScanRadius = 1f;
    [SerializeField] private float ScanInterval = 0.5f;

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
        }
    }

    private void Start()
    {
        base.Start();
        StartCoroutine(ScanRoutine());
    }

    private IEnumerator ScanRoutine()
    {
        while (true)
        {
            ScanForPlayer();

            yield return new WaitForSeconds(ScanInterval);
        }
    }
}
