using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Image whiteFlashImage;
    [SerializeField] private Image damageFlashImage;
    [SerializeField] private Transform playerLifePanel;
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;

    [Header("Game Objects")]
    [SerializeField] public Enemy enemy; // 現在の敵
    [SerializeField] private MonsterSpawner monsterSpawner;
    [SerializeField] private OrbSpawner orbSpawner; // オーブを再配置するための参照
    [SerializeField] private SpriteRenderer backgroundImage;
    [SerializeField] private Sprite[] fieldBackgrounds;

    [Header("Game Parameters")]
    private int currentStage = 1;
    private int currentField = 1;
    private const int StagesPerField = 3;
    private const int MaxFields = 5;
    private List<Image> playerHearts = new List<Image>();

    [Header("Player State")]
    public int playerLife = 3;
    private int maxLife = 3;
    private float currentTime;
    private bool isGameOver = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start()
    {
        InitializePlayerLife();
        StartSpawningMonsters();

        if (orbSpawner == null)
        {
            orbSpawner = FindFirstObjectByType<OrbSpawner>();
        }
    }

    private void StartSpawningMonsters()
    {
        monsterSpawner.SpawnNextMonster();
        enemy = FindAnyObjectByType<Enemy>(); // 新しくスポーンした敵を取得
        if (enemy != null)
        {
            SetTimerForEnemy(enemy.TimeLimit);
        }
    }

    private void InitializePlayerLife()
    {
        for (int i = 0; i < maxLife; i++)
        {
            GameObject heart = Instantiate(heartPrefab, playerLifePanel);
            Image heartImage = heart.GetComponent<Image>();
            heartImage.sprite = fullHeart;
            playerHearts.Add(heartImage);
        }
    }

    void Update()
    {
        if (!isGameOver)
        {
            UpdateTimer();
        }
    }

    private void UpdateTimer()
    {
        if (enemy == null) return;

        currentTime -= Time.deltaTime;
        timerText.text = "Time: " + Mathf.Ceil(currentTime);

        if (currentTime <= 0)
        {
            TakeDamage();
            ResetTimer(); // タイマーリセットとオーブの再配置
        }
    }

    public void SetTimerForEnemy(float time)
    {
        currentTime = time;
    }

    public void TakeDamage()
    {
        if (isGameOver) return; // ゲームオーバー時に処理をしない

        playerLife--;
        UpdatePlayerLifeUI();
        FlashRedScreen();

        if (playerLife <= 0)
        {
            GameOver();
        }
    }

    public void DamageEnemy(int damage)
    {
        if (enemy == null)
        {
            enemy = FindAnyObjectByType<Enemy>();
        }

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            FlashWhiteScreen();
        }
    }

    public void OnEnemyDefeated()
    {
        Debug.Log("GameManager: OnEnemyDefeated() が実行された！");
        monsterSpawner.SpawnNextMonster();
        enemy = FindAnyObjectByType<Enemy>(); // 新しくスポーンした敵を取得
        if (enemy != null)
        {
            SetTimerForEnemy(enemy.TimeLimit);
        }
    }

    private void FlashRedScreen()
    {
        StartCoroutine(RedFlashEffect());
    }

    private void FlashWhiteScreen()
    {
        StartCoroutine(WhiteFlashEffect());
    }

    private IEnumerator RedFlashEffect()
    {
        damageFlashImage.color = new Color(1, 0, 0, 0.5f);
        yield return new WaitForSeconds(0.1f);
        damageFlashImage.color = new Color(1, 0, 0, 0f);
    }

    private IEnumerator WhiteFlashEffect()
    {
        whiteFlashImage.color = new Color(1, 1, 1, 0.5f);
        yield return new WaitForSeconds(0.1f);
        whiteFlashImage.color = new Color(1, 1, 1, 0f);
    }

    private void UpdatePlayerLifeUI()
    {
        for (int i = 0; i < playerHearts.Count; i++)
        {
            playerHearts[i].sprite = (i < playerLife) ? fullHeart : emptyHeart;
        }
    }

    public void Heal(int amount)
    {
        playerLife = Mathf.Min(playerLife + amount, maxLife);
        UpdatePlayerLifeUI();
    }

    private void GameOver()
    {
        isGameOver = true;
        Debug.Log("ゲームオーバー！");
        timerText.text = "GAME OVER";
    }

    public void ResetTimer()
    {
        if (enemy != null)
        {
            currentTime = enemy.TimeLimit;
            if (orbSpawner != null)
            {
                orbSpawner.RespawnOrbs(); // タイマーリセット時にオーブ再配置
            }
        }
        else
        {
            Debug.LogWarning("敵が存在しないため、タイマーをリセットできません。");
        }
    }

    public void StageClear()
    {
        currentStage++;

        if (currentStage % StagesPerField == 1)
            NextField();

        if (currentField > MaxFields)
            GameClear();
    }

    private void ChangeBackground()
    {
        if (currentField - 1 < fieldBackgrounds.Length)
        {
            backgroundImage.sprite = fieldBackgrounds[currentField - 1];
        }
    }

    private void NextField()
    {
        currentField++;
        ChangeBackground();
    }

    private void GameClear()
    {
        isGameOver = true;
        Debug.Log("ゲームクリア！");
        timerText.text = "GAME CLEAR!";
    }
}
