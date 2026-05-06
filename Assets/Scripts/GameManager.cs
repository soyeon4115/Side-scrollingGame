using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Xml.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    int score = 0;
    public int Score => score;

    public void AddScore(int amount = 1)
    {
        if (isGameOver) return;

        score += amount;
        Debug.Log($"Score: {score}");
        scoreText.text = $"Score: {score}";
    }
    void Awake()
    {
        Instance = this;
        hp = maxHp;
    }

    [SerializeField]
    GameObject itemPrefab;

    [SerializeField]
    GameObject obstaclePrefab;

    [SerializeField]
    float itemSpawnInterval = 2f;

    [SerializeField]
    Vector2 itemSpawnYRange = new Vector2(-2f, 2f);

    [SerializeField]
    float scrollSpeed = 10f;

    [SerializeField]
    Camera targetCamera;

    [SerializeField]
    Transform itemParent;

    [SerializeField]
    TextMeshProUGUI scoreText;

    [SerializeField]
    Image[] hpImages;

    [SerializeField]
    Sprite fullHpSprite;

    [SerializeField]
    Sprite emptyHpSprite;

    [SerializeField]
    GameObject gameOverPanel;

    [SerializeField]
    int itemsPerObstacle = 10;

    float spawnTimer;
    int itemSpawnCount;

    [SerializeField]
    int maxHp = 3;
    int hp;
    bool isGameOver = false;

    void Start()
    {
        UpdateHpUI();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    void Update()
    {
        if (isGameOver) return;

        spawnTimer += Time.deltaTime;
        if (spawnTimer < itemSpawnInterval)
            return;

        spawnTimer = 0f;

        itemSpawnCount++;
        if (itemSpawnCount >= itemsPerObstacle)
        {
            itemSpawnCount = 0;
            SpawnObstacle();
        }
        else
        {
            SpawnItem();
        }
    }

    public void TakeDamage()
    {
        if (isGameOver) return;

        hp--;
        Debug.Log($"HP: {hp}");

        UpdateHpUI();

        if (hp <= 0)
            GameOver();
    }

    void GameOver()
    {
        isGameOver = true;
        Debug.Log("Game Over");

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            TextMeshProUGUI finalScoreText = gameOverPanel.GetComponentInChildren<TextMeshProUGUI>();
            if (finalScoreText != null)
                finalScoreText.text = $"Final Score: {score}";
        }
    }

    void SpawnItem()
    {
        float spawnX = GetCameraRightX();
        float spawnY = Random.Range(itemSpawnYRange.x, itemSpawnYRange.y);
        Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0f);

        GameObject item = Instantiate(itemPrefab, spawnPosition, Quaternion.identity, itemParent);

        ItemMover mover = item.GetComponent<ItemMover>();
        if (mover == null)
            mover = item.AddComponent<ItemMover>();

        mover.scrollSpeed = scrollSpeed;
        mover.targetCamera = targetCamera != null ? targetCamera : Camera.main;
    }

    void SpawnObstacle()
    {
        float spawnX = GetCameraRightX();
        float spawnY = Random.Range(itemSpawnYRange.x, itemSpawnYRange.y);
        Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0f);

        GameObject obstacle = Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity, itemParent);

        ItemMover mover = obstacle.GetComponent<ItemMover>();
        if (mover == null)
            mover = obstacle.AddComponent<ItemMover>();

        mover.scrollSpeed = scrollSpeed;
        mover.targetCamera = targetCamera != null ? targetCamera : Camera.main;
        mover.isObstacle = true;
    }

    float GetCameraRightX()
    {
        Camera cam = targetCamera != null ? targetCamera : Camera.main;
        if (cam == null)
            return 10f;

        float distance = Mathf.Abs(cam.transform.position.z);
        return cam.ViewportToWorldPoint(new Vector3(1f, 0.5f, distance)).x;
    }
    

    void UpdateHpUI()
    {
        for (int i = 0; i < hpImages.Length; i++)
        {
            if (hpImages[i] != null)
            {
                hpImages[i].sprite = i < hp ? fullHpSprite : emptyHpSprite;
            }
        }
    }
}
