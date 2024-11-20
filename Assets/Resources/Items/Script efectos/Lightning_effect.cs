using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning_effect : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform pointA; // First point of the lightning
    public Transform pointB; // Second point of the lightning
    public int segments = 10; // Number of segments to create a jagged effect
    public float randomness = 0.2f; // Maximum offset for jaggedness

    private bool deleteAfterDuration = false;

    private float duration = 0f;

    private LineRenderer lineRenderer;

    void Start()
    {
        // Get the Line Renderer component
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            Debug.LogError("LineRenderer component missing on this GameObject.");
        }

        // Set the line renderer initial setup
        lineRenderer.positionCount = segments + 1;
    }

    void Update()
    {
        if (lineRenderer == null || pointA == null || pointB == null) return;

        // Create a lightning path with randomness
        Vector3[] positions = new Vector3[segments + 1];
        Vector3 direction = (pointB.position - pointA.position) / segments;

        for (int i = 0; i <= segments; i++)
        {
            Vector3 basePosition = pointA.position + direction * i;
            Vector3 randomOffset = new Vector3(
                Random.Range(-randomness, randomness),
                Random.Range(-randomness, randomness),
                Random.Range(-randomness, randomness)
            );
            positions[i] = basePosition + randomOffset;
        }

        // Assign the positions to the Line Renderer
        lineRenderer.SetPositions(positions);
    }

    void FixedUpdate(){
        duration -= Time.deltaTime;

        if(deleteAfterDuration && duration <= 0){
            Destroy(this.gameObject);
        }
    }


    public void setDuration(float duration){
        deleteAfterDuration = true;
        this.duration = duration;
    }
}
