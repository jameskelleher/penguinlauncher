using System.Collections;
using SmallHedge.SoundManager;
using UnityEngine;

public class StarPenguin : MonoBehaviour
{
    public float speed = 1.0f;
    public float distance = 1.0f;
    public float distanceOffset = 0.25f;
    public float degreesPerSecond = 180f;
    public float spawnOffset = 2.0f;
    public GameObject star;
    public float starLifetime = 1.5f;
    public AudioClip starSound;
    public float audioDecay = 0.1f;

    [HideInInspector]
    public GameManager gameManager;

    Vector3 destination;
    AudioSource audioSource;

    void Start()
    {
        float newX = Random.Range(-spawnOffset, spawnOffset);
        transform.position = transform.parent.position + new Vector3(newX, 0.0f, 0.0f);
        distance += Random.Range(-distanceOffset, distanceOffset);
        destination = transform.position + new Vector3(0.5f, -distance, 0.0f);

        audioSource = GetComponent<AudioSource>();

        SoundManager.PlaySound(SoundType.FALLING, audioSource);
    }

    void Update()
    {
        audioSource.volume -= audioDecay * Time.deltaTime;
        transform.Rotate(Vector3.forward * Time.deltaTime * degreesPerSecond);

        float step = speed * Time.deltaTime;
        Vector3 newPos = Vector3.MoveTowards(transform.position, destination, step);
        transform.position = newPos;

        if (Vector3.Distance(destination, Vector3.MoveTowards(transform.position, destination, step)) < 0.01f)
        {
            Destroy(gameObject);
            GameObject starInstance = Instantiate(star, transform.position, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));
            Destroy(starInstance, starLifetime);
            SoundManager.PlaySound(SoundType.STAR);
        }
    }

    void OnDestroy()
    {
        gameManager.CheckIfEnded(extraDelay: starLifetime);
    }
}
