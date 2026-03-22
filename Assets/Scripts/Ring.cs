using System;
using UnityEngine;

public class Ring : MonoBehaviour
{
    public float amplitude = 1f;
    public float frequency = 1f;
    public GameManager gameManager;

    private float yInital;
    private float theta = 0f;
    private float startTime;

    void Start()
    {
        yInital = transform.position.y;
        startTime = Time.time;
    }

    void Update()
    {
        if (Time.time - startTime > 10f)
            theta += Time.deltaTime * frequency;
        float offset = Mathf.Cos(theta);
        float y = yInital - offset * amplitude; // start low, instead of high or in the middle
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
        
    }
}
