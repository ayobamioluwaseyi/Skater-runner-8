using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    public int maxCoin = 5;
    public float chanceToSpawn = 0.5f;

    public bool forceSpawnAll = false;

    private GameObject[] coins;

    private void Awake()
    {
        coins = new GameObject[transform.childCount]; // the amount of children a coin has
        for (int i = 0; i < transform.childCount; i++)
        {
            // get single coin
            coins[i] = transform.GetChild(i).gameObject;
        }

        OnDisable();
    }

    private void OnEnable()
    {
        // check to see if we are spawning the game obj
        if(Random.Range(0.0f, 1.0f) > chanceToSpawn)
        {
            return;
        }
        if (forceSpawnAll)
        {
            for (int i = 0; i < maxCoin; i++)
            {
                coins[i].SetActive(true);
            }
        }
        else
        {
            int r = Random.Range(0, maxCoin);
            for (int i = 0; i < r; i++)
            {
                coins[i].SetActive(true);
            }
        }
        
    }

    private void OnDisable()
    {
        foreach (GameObject go in coins)
        {
            go.SetActive(false);
        }
    }
}
