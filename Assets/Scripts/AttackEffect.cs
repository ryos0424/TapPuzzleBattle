using UnityEngine;

public class AttackEffect : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("【エラー】Animator が見つかりません！");
        }
    }

    public void PlayEffect()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack"); // **アニメーションを再生**
        }
    }
}
