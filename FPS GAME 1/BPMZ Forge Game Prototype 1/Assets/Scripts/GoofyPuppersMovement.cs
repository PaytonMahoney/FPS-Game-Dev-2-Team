using UnityEngine;
using System.Collections;

public class GoofyPuppersMovement : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float wanderRadius = 3f;
    public float pauseTime = 2f;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool isMoving = false;

    public float bounceSpeed = 3f;
    public float bounceHeight = 0.05f;
    private Vector3 originalScale;

    void Start()
    {
        startPosition = transform.position;
        StartCoroutine(WanderRoutine());
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (isMoving)
        {
            float bounce = Mathf.Sin(Time.time * bounceSpeed) * bounceHeight;
            transform.localScale = originalScale + new Vector3(0, bounce, 0);

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                isMoving = false;
                StartCoroutine(WanderRoutine());
            }
        }
    }

    IEnumerator WanderRoutine()
    {
        yield return new WaitForSeconds(pauseTime);

        float randomX = Random.Range(-wanderRadius, wanderRadius);
        float randomZ = Random.Range(-wanderRadius, wanderRadius);
        targetPosition = startPosition + new Vector3(randomX, 0, randomZ);
        isMoving = true;
    }
}
