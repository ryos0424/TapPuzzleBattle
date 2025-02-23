using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class OrbSpawner : MonoBehaviour
{
    [SerializeField] private GameObject orbPrefab;
    [SerializeField] private float spawnAreaSize = 3.0f;
    [SerializeField] private float minDistance = 1.0f;
    private List<Vector2> spawnedPositions = new List<Vector2>();
    private int remainingPairs;

    private Color[] possibleColors = { Color.red, Color.blue, Color.green, Color.yellow, Color.magenta, Color.cyan };
    private List<Color> assignedColors = new List<Color>();

    private List<GameObject> spawnedOrbs = new List<GameObject>(); // 修正: 生成されたオーブを管理

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
        remainingPairs = 3;
        spawnedOrbs.Clear(); // 修正: 既存のオーブリストをクリア

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
                spawnedOrbs.Add(newOrb); // 修正: リストに追加
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

            if (GameManager.Instance != null)
            {
                GameManager.Instance.DamageEnemy(1);
                GameManager.Instance.ResetTimer();
                RespawnOrbs(); // ★タイムリミット切れでもオーブを再配置できるようにする
            }
            else
            {
                Debug.LogError("GameManager の Instance が null です。");
            }
        }
    }

    /// </summary>
    public void RespawnOrbs()
    {
        ClearOrbs(); // 修正: すでに生成されたオーブを削除
        StartCoroutine(RespawnAfterDelay());
    }

    private IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(0.1f);
        SpawnOrbs();
    }

    // 既存のオーブを削除するメソッドを追加
    private void ClearOrbs()
    {
        // 修正: 生成されたオーブをリストから削除する
        foreach (GameObject orb in spawnedOrbs)
        {
            if (orb != null)
            {
                Destroy(orb);
            }
        }
        spawnedOrbs.Clear(); // 修正: リストをクリア
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
