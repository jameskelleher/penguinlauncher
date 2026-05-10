using System.Collections;
using SmallHedge.SoundManager;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    public GameObject explosion;
    public TMP_Text startText;

    public GameObject tapToStartUI;

    private bool readyForMobile = false;

    void Start()
    {
        if (Application.isMobilePlatform)
        {
            tapToStartUI.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            tapToStartUI.SetActive(false);
            readyForMobile = true;
        }
    }

    void Update()
    {
        if (!readyForMobile &&
                Touchscreen.current != null &&
                Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            // Screen.fullScreen = true;
            readyForMobile = true;
            Time.timeScale = 1;
            tapToStartUI.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
        startText.color = Color.red;
        GameObject explosionInstance = Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(explosionInstance, 2.0f);
        SoundManager.PlaySound(SoundType.EXPLOSION, volume: 1f);
        StartCoroutine(StartGame());
    }

    // hide the launcher until the penguin collides with it
    void OnCollisionEnter2D(Collision2D collision)
    {
        collision.gameObject.GetComponent<SpriteRenderer>().enabled = true;
    }

    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene("Main");
    }
}
