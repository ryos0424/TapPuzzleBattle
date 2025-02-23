using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class Enemy : MonoBehaviour
{
    [Header("基本設定")]
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private float timeLimit = 10f;
    private int currentHealth;

    public float TimeLimit
    {
        get => timeLimit;
        set => timeLimit = value;
    }

    [Header("UI要素")]
    [SerializeField] public Transform enemyLifePanel; // 外部からアクセス可能に
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;

    private List<Image> enemyHearts = new List<Image>();

    void Start()
    {
        if (enemyLifePanel == null)
        {
            enemyLifePanel = GameObject.Find("EnemyLifePanel")?.GetComponent<RectTransform>();
            if (enemyLifePanel == null)
            {
                Debug.LogError($"{gameObject.name} の EnemyLifePanel が見つかりません！ Inspector で設定してください。");
                return;
            }
        }

        currentHealth = maxHealth;
        Debug.Log($"{gameObject.name} の HP 初期化: {currentHealth}");
        Debug.Log($"{gameObject.name} のタイマー設定: {timeLimit}秒");

        InitializeEnemyLife();
    }

    public void InitializeEnemyLife() // アクセスを public に変更
    {
        ClearExistingHearts();
        CreateHearts();
    }

    private void ClearExistingHearts()
    {
        if (enemyLifePanel != null)
        {
            foreach (Transform child in enemyLifePanel)
            {
                Destroy(child.gameObject);
            }
        }
        enemyHearts.Clear();
    }

    private void CreateHearts()
    {
        for (int i = 0; i < maxHealth; i++)
        {
            GameObject heart = Instantiate(heartPrefab, enemyLifePanel);
            Image heartImage = heart.GetComponent<Image>();

            if (heartImage != null)
            {
                heartImage.sprite = fullHeart;
                enemyHearts.Add(heartImage);
            }
            else
            {
                Debug.LogError("heartPrefab に Image コンポーネントがありません！");
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
        Debug.Log($"{gameObject.name} がダメージを受けた！ 現在のHP: {currentHealth}");
        UpdateEnemyLifeUI();

        if (currentHealth <= 0)
        {
            Defeated();
        }
    }

    private void UpdateEnemyLifeUI()
    {
        for (int i = 0; i < enemyHearts.Count; i++)
        {
            if (enemyHearts[i] == null)
            {
                Debug.LogWarning($"EnemyHearts[{i}] が null になっています。リストをリセットしてください。");
                continue;
            }

            enemyHearts[i].sprite = (i < currentHealth) ? fullHeart : emptyHeart;
        }
    }

    private void Defeated()
    {
        Debug.Log($"{gameObject.name} が倒された！ 次の敵へ");
        Invoke(nameof(NotifyGameManagerOfDefeat), 0.5f);
    }

    private void NotifyGameManagerOfDefeat()
    {
        GameManager.Instance.OnEnemyDefeated();
        GameManager.Instance.StageClear();
    }
}
