using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Enemy enemy;

    [Header("攻撃予兆Imaage")]
    [SerializeField] private Image leftWarning;
    [SerializeField] private Image centerWarning;
    [SerializeField] private Image rightWarning;

    [Header("HPバー")]
    [SerializeField] private Image playerHPBar;
    [SerializeField] private Image enemyHPBar;


    private void Update()
    {
        leftWarning.enabled = false;
        centerWarning.enabled = false;
        rightWarning.enabled = false;

        // HPバー表示処理
        playerHPBar.fillAmount = (float)player.CurrentHP / player.MaxHP;
        enemyHPBar.fillAmount = (float)enemy.CurrentHP / enemy.MaxHP;

        // 予兆状態のときのみ表示
        if (enemy.CurrentState == EnemyState.PrepareAttack)
        {
            switch (enemy.CurrentAttackDirection)
            {
                case EnemyAttackDirection.Left:
                    leftWarning.enabled = true;
                    break;
                case EnemyAttackDirection.Center:
                    centerWarning.enabled = true;
                    break;
                case EnemyAttackDirection.Right:
                    rightWarning.enabled = true;
                    break;
            }
        }
    }
}
