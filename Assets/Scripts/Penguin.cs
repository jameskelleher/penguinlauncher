using System;
using System.Collections;
using SmallHedge.SoundManager;
using UnityEngine;

public class Penguin : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject explosionEffect;
    public float ringSoundVelocityThreshold = 1.0f;
    public float postScoreDamping = 100f;
    public float postScoreGravity = -0.5f;
    public float postScoreScaleIncr = 1f;
    public float postScoreRotationIncr = 100f;
    public float postScoreFadeOutDuration = 2f;

    private float starKoAt;
    private float underwaterAt;
    private BoxCollider2D coll;
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private bool destroyOnGround = false;
    private bool explodeOnDestroy = true;
    private bool doRapture = false;

    void Start()
    {
        Camera cam = Camera.main;
        float height = 2f * cam.orthographicSize;
        starKoAt = cam.transform.position.y + height / 2 + 0.5f;
        underwaterAt = cam.transform.position.y - height / 2 - 0.5f;

        coll = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (transform.position.y > starKoAt)
        {
            gameManager.InstantiateStarPenguin();
            explodeOnDestroy = false;
            Destroy(gameObject);
            gameManager.CountResult(ResultType.STAR);
        }

        else if (transform.position.y < underwaterAt)
        {
            explodeOnDestroy = false;
            Destroy(gameObject);
            gameManager.CountResult(ResultType.UNDERWATER);
        }

        if (rb.linearVelocityX < 0.0f)
            destroyOnGround = true;

        if (doRapture)
        {
            if (sprite.color.a <= 0)
                Destroy(gameObject);

            transform.localScale += transform.localScale * postScoreScaleIncr * Time.deltaTime;
            rb.angularVelocity += postScoreRotationIncr * Time.deltaTime * Math.Sign(rb.angularVelocity + 0.01f);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Launcher"))
        {
            if (destroyOnGround)
                Destroy(gameObject, 2.0f);

            float launcherLeft = other.collider.bounds.min.x;
            float penguinRight = coll.bounds.max.x;

            if (penguinRight < launcherLeft)
            {
                Vector2 penguinDown = -rb.transform.up;
                if (penguinDown.y < -0.8f)
                    Destroy(gameObject);
            }
        }
        else if (other.gameObject.CompareTag("Ring"))
        {
            float velocity = other.relativeVelocity.magnitude;
            Debug.Log(velocity);

            if (velocity >= ringSoundVelocityThreshold)
                SoundManager.PlaySound(SoundType.METALBONK);

        }
        else if (other.gameObject.CompareTag("Ground"))
        {
            if (destroyOnGround)
                Destroy(gameObject);
        }

    }

    void OnDestroy()
    {
        gameManager.CheckIfEnded();

        if (explodeOnDestroy)
        {
            GameObject explosionInstance = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(explosionInstance, 3.0f);
            SoundManager.PlaySound(SoundType.EXPLOSION);
            gameManager.CountResult(ResultType.EXPLODE);
        }
    }

    public void DidScore()
    {
        explodeOnDestroy = false;
        rb.linearDamping = postScoreDamping;
        rb.gravityScale = postScoreGravity;
        rb.linearVelocityY = 0f;
        doRapture = true;
        coll.enabled = false;
        StartCoroutine(FadeOut());
        SoundManager.PlaySound(SoundType.ANGELS, volume: 1.5f);
        gameManager.CountResult(ResultType.RAPTURE);
    }

    IEnumerator FadeOut()
    {
        float elapsed = 0.0f;
        Color startColor = sprite.color;
        Color transparent = new Color(1f, 1f, 1f, 0f);

        while (elapsed < postScoreFadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / postScoreFadeOutDuration;

            sprite.color = Color.Lerp(startColor, transparent, t);
            yield return null;
        }
    }
}
