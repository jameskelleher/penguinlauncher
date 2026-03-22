using UnityEngine;
using UnityEngine.InputSystem;

public class ScoreDetector : MonoBehaviour
{
    private bool hasEntered = false;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (transform.position.x < collision.bounds.min.x)
        {
            hasEntered = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (hasEntered && transform.position.x > collision.bounds.max.x)
        {
            transform.parent.GetComponent<Penguin>().DidScore();
            hasEntered = false;
        }
    }
}
