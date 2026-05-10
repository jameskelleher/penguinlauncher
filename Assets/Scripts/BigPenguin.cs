using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using SmallHedge.SoundManager;

public class BigPenguin : MonoBehaviour
{
    public GameObject explosionEffect;
    public GameManager gameManager;

    [HideInInspector] public UnityEvent onDestroy;

    void Update()
    {
        if (transform.position.y < -30)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject other = collision.gameObject;
        if (BlowThisUp(other))
        {
            Destroy(other);
            Destroy(other.transform.root.gameObject);
            GameObject explosionInstance = Instantiate(explosionEffect, other.transform.position, Quaternion.identity);
            SoundManager.PlaySound(SoundType.EXPLOSION);
            Destroy(explosionInstance, 3.0f);
        }
    }


    bool BlowThisUp(GameObject obj) => new[] { "Launcher", "Target", "Ring" }.Any(t => obj.CompareTag(t));


    void OnDestroy() { gameManager.CountResult(ResultType.UNDERWATER); onDestroy.Invoke(); }
}
