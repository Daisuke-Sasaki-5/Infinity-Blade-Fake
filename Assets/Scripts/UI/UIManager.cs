using System;
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

    private void Start()
    {
        SetAplha(leftWarning, 0);
        SetAplha(centerWarning, 0);
        SetAplha(rightWarning, 0);
    }

    /// <summary>
    /// 透明度変更
    /// </summary>
    /// <param name="image"></param>
    /// <param name="alpha"></param>
    private void SetAplha(Image image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }

    private void Update()
    {
        SetAplha(leftWarning, 0);
        SetAplha(centerWarning, 0);
        SetAplha(rightWarning, 0);

        // HPバー表示処理
        playerHPBar.fillAmount = (float)player.CurrentHP / player.MaxHP;
        enemyHPBar.fillAmount = (float)enemy.CurrentHP / enemy.MaxHP;

        // 予兆状態のときのみ表示
        if (enemy.CurrentState == EnemyState.PrepareAttack || enemy.CurrentState == EnemyState.Attack)
        {
            switch (enemy.CurrentAttackDirection)
            {
                case EnemyAttackDirection.Left:
                    SetAplha(leftWarning, enemy.PrepareProgress);
                    leftWarning.enabled = true;
                    break;
                case EnemyAttackDirection.Center:
                    SetAplha(centerWarning, enemy.PrepareProgress);
                    centerWarning.enabled = true;
                    break;
                case EnemyAttackDirection.Right:
                    SetAplha(rightWarning, enemy.PrepareProgress);
                    rightWarning.enabled = true;
                    break;
            }
        }
    }
}
