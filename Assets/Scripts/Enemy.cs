using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro; // HP表示用

public class Enemy : MonoBehaviour
{
    public int maxHealth = 10; // 敵の最大HP
    private int currentHealth;

    [SerializeField] private Transform enemyLifePanel; // 敵ライフのUI
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;
    private List<Image> enemyHearts = new List<Image>();

    void Start()
    {
        currentHealth = maxHealth;
        InitializeEnemyLife();
    }

    private void InitializeEnemyLife()
    {
        for (int i = 0; i < maxHealth; i++)
        {
            GameObject heart = Instantiate(heartPrefab, enemyLifePanel);
            Image heartImage = heart.GetComponent<Image>();
            heartImage.sprite = fullHeart;
            enemyHearts.Add(heartImage);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Defeated();
        }
        UpdateEnemyLifeUI();
    }

    private void UpdateEnemyLifeUI()
    {
        for (int i = 0; i < enemyHearts.Count; i++)
        {
            if (i < currentHealth)
            {
                enemyHearts[i].sprite = fullHeart;
            }
            else
            {
                enemyHearts[i].sprite = emptyHeart;
            }
        }
    }

    private void Defeated()
    {
        Debug.Log("敵を倒した！ 次の敵へ");
        Invoke("NextEnemy", 1.0f); // 1秒後に次の敵を出す

        GameManager.Instance.StageClear();
    }

    private void NextEnemy()
    {
        currentHealth = maxHealth; // HPをリセット
        UpdateEnemyLifeUI();
        Debug.Log("次の敵が登場！");
    }
}
