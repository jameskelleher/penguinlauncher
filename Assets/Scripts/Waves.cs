using UnityEngine;

public class Waves : MonoBehaviour
{
    public float offset = 0f;
    public float frequency = 1f;
    public float horizAmplitude = 1.0f;
    public float vertAmplitude = 0.5f;

    private Vector3 initialPosition;


    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        float theta = offset + Time.time;
        float xSin = Mathf.Sin(theta * frequency) * horizAmplitude;
        float ySin = Mathf.Cos(theta * frequency * 2) * vertAmplitude;
        transform.position = initialPosition + transform.right * xSin - transform.up * ySin;
    }
}
