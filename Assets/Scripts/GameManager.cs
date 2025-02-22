using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private TextMeshProUGUI lifeText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Enemy enemy;
    [SerializeField] private Image whiteFlashImage; // ⚡ 敵にダメージを与えたときに白く光る
    [SerializeField] private Image damageFlashImage; // 💥 画面全体を光らせるUI
    [SerializeField] private SpriteRenderer backgroundImage; // 背景の Image
    [SerializeField] private Sprite[] fieldBackgrounds; // 背景画像リスト（Inspector で設定）

    private int currentStage = 1; // 現在のステージ（1からスタート）
    private int currentField = 1; // 現在のフィールド（1から5）
    private const int StagesPerField = 3; // 1フィールドのステージ数
    private const int MaxFields = 5; // 最大フィールド数

    public int playerLife = 3;
    public float timeLimit = 10f;
    private float currentTime;
    private bool isGameOver = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        currentTime = timeLimit;
        UpdateUI();
    }

    void Update()
    {
        if (!isGameOver)
        {
            currentTime -= Time.deltaTime;
            timerText.text = "Time: " + Mathf.Ceil(currentTime);

            if (currentTime <= 0)
            {
                TakeDamage();
                ResetTimer();
            }
        }
    }

    public void TakeDamage()
    {
        playerLife--;
        UpdateUI();

        // 💥 自分がダメージを受けたときだけ画面を赤く光らせる
        Debug.Log("プレイヤーがダメージを受けた！画面を赤くする");
        FlashRedScreen();

        if (playerLife <= 0)
        {
            GameOver();
        }
    }

    public void DamageEnemy(int damage)
    {
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Debug.Log("敵にダメージを与えた！画面を白くする");
            FlashWhiteScreen(); // ⚡ 敵にダメージを与えたときに画面を白く光らせる
        }
    }

    public void FlashRedScreen()
    {
        StartCoroutine(RedFlashEffect());
    }

    public void FlashWhiteScreen()
    {
        StartCoroutine(WhiteFlashEffect());
    }

    private IEnumerator RedFlashEffect()
    {
        Color flashColor = damageFlashImage.color;
        flashColor.a = 0.5f; // 半透明の赤に変更
        damageFlashImage.color = flashColor;

        yield return new WaitForSeconds(0.1f);

        flashColor.a = 0f; // 透明に戻す
        damageFlashImage.color = flashColor;
    }

    private IEnumerator WhiteFlashEffect()
    {
        Color flashColor = whiteFlashImage.color;
        flashColor.a = 0.5f; // 半透明の白に変更
        whiteFlashImage.color = flashColor;

        yield return new WaitForSeconds(0.1f);

        flashColor.a = 0f; // 透明に戻す
        whiteFlashImage.color = flashColor;
    }

    private void GameOver()
    {
        isGameOver = true;
        Debug.Log("ゲームオーバー！");
        timerText.text = "GAME OVER";
    }

    public void ResetTimer()
    {
        currentTime = timeLimit;
    }

    private void UpdateUI()
    {
        lifeText.text = "ライフ: " + playerLife;
    }

    public void StageClear()
    {
        currentStage++;

        if (currentStage % StagesPerField == 1) // 3ステージごとにフィールドを変更
        {
            NextField();
        }

        if (currentField > MaxFields) // 5フィールドクリアでゲームクリア
        {
            GameClear();
        }
    }

    private void ChangeBackground()
    {
        if (currentField - 1 < fieldBackgrounds.Length)
        {
            backgroundImage.sprite = fieldBackgrounds[currentField - 1]; // 背景を変更
            Debug.Log($"背景を変更: フィールド {currentField}");
        }
    }

    private void NextField()
    {
        currentField++;
        Debug.Log($"次のフィールドへ移動！現在のフィールド: {currentField}");

        ChangeBackground(); // 🎨 背景を変更
    }

    private void GameClear()
    {
        isGameOver = true;
        Debug.Log("ゲームクリア！");
        timerText.text = "GAME CLEAR!";
    }
}
