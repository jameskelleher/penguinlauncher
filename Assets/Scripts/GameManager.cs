using System;
using System.Collections;
using System.Collections.Generic;
using SmallHedge.SoundManager;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Penguins")]
    public int numPenguins = 100;
    public List<SpawnData> spawnData;
    public GameObject penguinPrefab;
    public GameObject spawnPoint;
    public float penguinSpeed = 10.0f;
    public GameObject bigPenguinPrefab;
    public GameObject bigPenSpawnPoint;
    public float bigPenSpeed;

    [Header("Star Penguins")]
    public GameObject starPenguin;
    public GameObject starPenguinSpawn;

    [Header("UI")]
    public TMP_Text numPenguinsText;
    public TMP_Text resultsText;


    private Dictionary<ResultType, int> resultCounts = new Dictionary<ResultType, int>();
    private int leftToDestroy;

    void Start()
    {
        StartCoroutine(SpawnPenguin());
        SetNumPenguinsText();
        ResetResultCounts();
        resultsText.enabled = false;
        leftToDestroy = numPenguins - 1;
    }


    [Serializable]
    public struct SpawnData
    {
        public float[] spawnIntervals;
        public int numSpawns;
        public bool pickRandomly;
    }

    IEnumerator SpawnPenguin()
    {
        yield return new WaitForSeconds(1.0f);

        Queue<SpawnData> spawnDataQueue = new Queue<SpawnData>(spawnData);

        SpawnData currentSpawnData;
        try
        {
            currentSpawnData = spawnDataQueue.Dequeue();
        }
        catch (Exception e)
        {
            if (e is InvalidOperationException)
            {
                Debug.LogError("please enter at least one set of spawn parameters");
                yield break;
            }
            else
                throw;
        }

        int nextUpdateAt = numPenguins - currentSpawnData.numSpawns;
        int intervalIx = -1;  // init to -1 bc we increment before first use
        float[] spawnIntervals = currentSpawnData.spawnIntervals;

        while (numPenguins > 1)
        {
            if (numPenguins == nextUpdateAt)
            {
                try
                {
                    currentSpawnData = spawnDataQueue.Dequeue();
                    Debug.Log("updating launch params");
                    nextUpdateAt = numPenguins - currentSpawnData.numSpawns;
                    spawnIntervals = currentSpawnData.spawnIntervals;
                    intervalIx = -1;
                }
                catch (Exception e)
                {
                    if (e is InvalidOperationException)
                    {
                        Debug.Log("ran out of spawn params, defaulting to final");
                        nextUpdateAt = 0;
                    }
                    else
                        throw;
                }
            }

            GameObject penguinInstance = Instantiate(penguinPrefab, spawnPoint.transform.position, Quaternion.identity);
            penguinInstance.GetComponent<Penguin>().gameManager = GetComponent<GameManager>();
            penguinInstance.GetComponent<Rigidbody2D>().linearVelocityX = penguinSpeed;
            Destroy(penguinInstance, 7.0f);
            UpdateNumPenguins();

            if (currentSpawnData.pickRandomly)
                intervalIx = UnityEngine.Random.Range(0, spawnIntervals.Length);
            else
                intervalIx = (intervalIx + 1) % spawnIntervals.Length;

            float spawnInterval = spawnIntervals[intervalIx];
            // Debug.Log($"penguins left: {numPenguins}");


            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void UpdateNumPenguins()
    {
        numPenguins--;
        SetNumPenguinsText();
    }

    void SetNumPenguinsText()
    {
        numPenguinsText.text = $"PENGUINS REMAIN: {numPenguins}";
    }

    public void InstantiateStarPenguin()
    {
        leftToDestroy += 1;
        GameObject instance = Instantiate(starPenguin, starPenguinSpawn.transform);
        instance.GetComponent<StarPenguin>().gameManager = this;
    }

    public void CountResult(ResultType result)
    {
        Debug.Log($"Counting results {result}");
        resultCounts[result] += 1;
    }

    public void CheckIfEnded(float extraDelay = 0f)
    {
        leftToDestroy -= 1;
        Debug.Log(leftToDestroy);
        if (leftToDestroy == 0)
            StartCoroutine(SpawnBigPenguin());
    }

    void ResetResultCounts()
    {
        foreach (ResultType result in Enum.GetValues(typeof(ResultType)))
            resultCounts[result] = 0;
    }

    IEnumerator SpawnBigPenguin()
    {
        Debug.Log("big spawn");
        yield return new WaitForSeconds(3f);
        GameObject bigPenguin = Instantiate(bigPenguinPrefab, bigPenSpawnPoint.transform.position, Quaternion.identity);
        bigPenguin.GetComponent<Rigidbody2D>().linearVelocityX = bigPenSpeed;
        bigPenguin.GetComponent<BigPenguin>().gameManager = GetComponent<GameManager>();
        bigPenguin.GetComponent<BigPenguin>().onDestroy.AddListener(() => StartCoroutine(ShowResults()));
        UpdateNumPenguins();
    }

    IEnumerator ShowResults(float extraDelay = 0f)
    {
        yield return new WaitForSeconds(3.0f + extraDelay);

        if (leftToDestroy > 0)
            yield break;

        // numPenguinsText.enabled = false;
        resultsText.enabled = true;

        resultsText.text = "";

        yield return UpdateResults(1.5f, "NO SCORE\n");
        yield return UpdateResults(2.5f, "ONLY RESULTS\n\n");
        yield return UpdateResults(1.0f, $"RAPTURED: {resultCounts[ResultType.RAPTURE]}\n");
        yield return UpdateResults(1.0f, $"OBLITERATED: {resultCounts[ResultType.EXPLODE]}\n");
        yield return UpdateResults(1.0f, $"STELLA NOVIS: {resultCounts[ResultType.STAR]}\n");
        yield return UpdateResults(15.0f, $"PRIMORDIAL RETURN: {resultCounts[ResultType.UNDERWATER]}");
        SceneManager.LoadScene("Intro");
    }

    IEnumerator UpdateResults(float delay, string text)
    {
        resultsText.text += text;
        SoundManager.PlaySound(SoundType.DAWN);
        yield return new WaitForSeconds(delay);
    }
}

public enum ResultType
{
    EXPLODE,
    STAR,
    RAPTURE,
    UNDERWATER
}