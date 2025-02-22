using UnityEngine;
using System.Collections.Generic;

public class OrbSpawner : MonoBehaviour
{
    [SerializeField] private GameObject orbPrefab;
    [SerializeField] private float spawnAreaSize = 3.0f;
    [SerializeField] private float minDistance = 1.0f;
    private List<Vector2> spawnedPositions = new List<Vector2>();
    private int remainingPairs;

    private Color[] possibleColors = { Color.red, Color.blue, Color.green, Color.yellow, Color.magenta, Color.cyan }; // 使用する色
    private List<Color> assignedColors = new List<Color>(); // ペア用の色リスト

    void Start()
    {
        SpawnOrbs();
    }

    void SpawnOrbs()
    {
        if (orbPrefab == null)
        {
            Debug.LogError("【エラー】orbPrefab が設定されていません！");
            return;
        }

        spawnedPositions.Clear();
        assignedColors.Clear();
        remainingPairs = 3; // 3ペア（6個）

        List<Color> selectedColors = new List<Color>();
        while (selectedColors.Count < 3)
        {
            Color newColor = possibleColors[Random.Range(0, possibleColors.Length)];
            if (!selectedColors.Contains(newColor))
            {
                selectedColors.Add(newColor);
                assignedColors.Add(newColor);
                assignedColors.Add(newColor);
            }
        }

        ShuffleColors(assignedColors);

        for (int i = 0; i < 6; i++)
        {
            Vector2 spawnPosition;
            int attempt = 0;

            do
            {
                spawnPosition = new Vector2(
                    Random.Range(-spawnAreaSize, spawnAreaSize),
                    Random.Range(-spawnAreaSize, spawnAreaSize)
                );

                attempt++;
                if (attempt > 10) break;

            } while (IsOverlapping(spawnPosition));

            spawnedPositions.Add(spawnPosition);

            if (orbPrefab != null)
            {
                GameObject newOrb = Instantiate(orbPrefab, spawnPosition, Quaternion.identity);
                Orb orbScript = newOrb.GetComponent<Orb>();
                orbScript.SetSpawner(this);
                orbScript.SetColor(assignedColors[i]);
            }
        }
    }

    private bool IsOverlapping(Vector2 newPos)
    {
        foreach (Vector2 pos in spawnedPositions)
        {
            if (Vector2.Distance(newPos, pos) < minDistance)
            {
                return true;
            }
        }
        return false;
    }

    public void OrbPairDestroyed()
    {
        remainingPairs--;
        Debug.Log("残りのペア数: " + remainingPairs);

        if (remainingPairs <= 0)
        {
            Debug.Log("すべてのオーブが消えた！ 敵にダメージを与える");
            GameManager.Instance.DamageEnemy(1); // **修正**
            GameManager.Instance.ResetTimer();
            Invoke("SpawnOrbs", 0.5f);
        }
    }



    private void ShuffleColors(List<Color> colors)
    {
        for (int i = colors.Count - 1; i > 0; i--)
        {
            int rand = Random.Range(0, i + 1);
            Color temp = colors[i];
            colors[i] = colors[rand];
            colors[rand] = temp;
        }
    }
}
