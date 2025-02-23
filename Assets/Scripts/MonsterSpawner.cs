using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private GameObject goblinPrefab;
    [SerializeField] private GameObject orcPrefab;
    [SerializeField] private GameObject darkElfPrefab;

    private GameObject currentMonster;
    private int spawnIndex;
    private GameObject[] monsterSequence;

    private void Start()
    {
        InitializeMonsterSequence();
    }

    private void InitializeMonsterSequence()
    {
        monsterSequence = new GameObject[] { goblinPrefab, orcPrefab, darkElfPrefab };
        spawnIndex = 0;
    }

    public void SpawnNextMonster()
    {
        if (spawnIndex >= monsterSequence.Length)
        {
            Debug.Log("すべてのモンスターを出現させました");
            return;
        }

        DestroyCurrentMonster();
        SpawnNewMonster();

        spawnIndex++;
    }

    private void DestroyCurrentMonster()
    {
        if (currentMonster != null)
        {
            Destroy(currentMonster);
        }
    }

    private void SpawnNewMonster()
    {
        Vector3 spawnPosition = new Vector3(0, -1, 0);
        currentMonster = Instantiate(monsterSequence[spawnIndex], spawnPosition, Quaternion.identity);

        Enemy enemy = currentMonster.GetComponent<Enemy>();

        if (enemy != null)
        {
            AssignEnemyToGameManager(enemy);
            SetupEnemyLifePanel(enemy);
            enemy.InitializeEnemyLife();

            // **モンスターごとのタイマーをセット**
            GameManager.Instance.SetTimerForEnemy(enemy.TimeLimit);
        }
    }

    private void AssignEnemyToGameManager(Enemy enemy)
    {
        GameManager.Instance.enemy = enemy;
    }

    private void SetupEnemyLifePanel(Enemy enemy)
    {
        Transform lifePanel = GameObject.Find("EnemyLifePanel")?.transform;

        if (lifePanel != null)
        {
            enemy.enemyLifePanel = lifePanel;
        }
        else
        {
            Debug.LogError("EnemyLifePanel が見つかりません！");
        }
    }
}
