using UnityEngine;
using TMPro; // HP表示用

public class Enemy : MonoBehaviour
{
    public int maxHealth = 10; // 敵の最大HP
    private int currentHealth;

    [SerializeField] private TextMeshProUGUI enemyHpText; // 敵のHP表示

    void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Defeated();
        }
        UpdateUI();
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
        UpdateUI();
        Debug.Log("次の敵が登場！");
    }

    private void UpdateUI()
    {
        enemyHpText.text = "敵HP: " + currentHealth;
    }
}
