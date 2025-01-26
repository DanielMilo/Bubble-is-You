using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour
{
    public Vector3 pointA; // First patrol point
    public Vector3 pointB; // Second patrol point
    public float speed = 2.5f; // Patrol speed
    public float RotationSpeed = 180;

    public float delay = 1f; // Delay at each end
    private bool isWaiting = false; // Flag to track waiting state

    public SpriteRenderer SpriteRenderer;

    private Vector3 currentTarget; // Current target point

    void Start()
    {
        // Start moving towards pointA initially
        currentTarget = pointA;
    }

    void Update()
    {
        if (!isWaiting)
        {
            // Move the object towards the current target
            Vector3 direction = (currentTarget - transform.position).normalized;
            transform.Translate(direction * speed * Time.deltaTime, Space.World);

            if(SpriteRenderer != null)
            {
                SpriteRenderer.transform.Rotate(new Vector3(0, 0, RotationSpeed * Time.deltaTime * -direction.x));
            }

            if (Vector3.Distance(transform.position, currentTarget) < 0.01f)
            {
                // Start waiting before switching the target
                StartCoroutine(WaitAndSwitch());
            }
        }
    }

    private IEnumerator WaitAndSwitch()
    {
        isWaiting = true;
        yield return new WaitForSeconds(delay);
        // Switch the target to the other point
        currentTarget = currentTarget == pointA ? pointB : pointA;
        isWaiting = false;
    }
}

