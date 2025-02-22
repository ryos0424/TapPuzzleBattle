using UnityEngine;

public class Orb : MonoBehaviour
{
    private OrbSpawner spawner;
    private SpriteRenderer spriteRenderer;
    private Color orbColor;

    private static Color? lastTappedColor = null;
    private static Orb firstSelectedOrb = null;

    private Material defaultMaterial; // 元のマテリアル
    private static Material highlightMaterial; // ハイライト用のマテリアル

    public void SetSpawner(OrbSpawner spawner)
    {
        this.spawner = spawner;
    }

    public void SetColor(Color color)
    {
        orbColor = color;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = orbColor;
            defaultMaterial = spriteRenderer.material;
        }
    }

    void Start()
    {
        if (highlightMaterial == null)
        {
            Shader shader = Shader.Find("GUI/Text Shader");
            highlightMaterial = new Material(shader)
            {
                color = new Color(1, 1, 1, 0.5f) // 半透明の白
            };
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                Debug.Log("オーブがタップされた！");

                if (lastTappedColor == null)
                {
                    lastTappedColor = orbColor;
                    firstSelectedOrb = this;
                    spriteRenderer.material = highlightMaterial; // 光をつける
                    Debug.Log("最初のオーブ選択: " + orbColor);
                }
                else if (lastTappedColor == orbColor && firstSelectedOrb != this)
                {
                    Debug.Log("ペアのオーブを発見！" + orbColor);

                    if (spawner != null)
                    {
                        spawner.OrbPairDestroyed();
                    }

                    firstSelectedOrb.spriteRenderer.material = defaultMaterial;
                    Destroy(firstSelectedOrb.gameObject);
                    Destroy(gameObject);

                    lastTappedColor = null;
                    firstSelectedOrb = null;
                }
                else
                {
                    Debug.Log("異なる色のオーブがタップされた。リセット");
                    if (firstSelectedOrb != null)
                    {
                        firstSelectedOrb.spriteRenderer.material = defaultMaterial; // 光を戻す
                    }
                    lastTappedColor = null;
                    firstSelectedOrb = null;
                }
            }
        }
    }
}
