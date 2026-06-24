using System.Collections;
using UnityEngine;

/// <summary>
/// PlayerState
/// </summary>
public enum PlayerState
{
    Idle,   // 待機
    Attack, // 攻撃中
    Guard,  // ガード中
    Dodge,  // 回避中
    Dead    // 死亡
}

/// <summary>
/// 回避方向
/// </summary>
public enum DodgeDirection
{
    None,
    Left,
    Right,
}

public class Player : MonoBehaviour
{
    public PlayerState CurrentState { get; private set; }
    public DodgeDirection LastDodgeDirection { get; private set; }

    [SerializeField] private Animator animator;
    [SerializeField] private Enemy enemy;

    // 攻撃中フラグ
    private bool isAttacking;

    private void Start()
    {
        CurrentState = PlayerState.Idle;
    }

    public void Attack()
    {
        if(isAttacking)
        {
            return;
        }

        // スタン中でなければ攻撃できない
        if(enemy.CurrentState != EnemyState.Stunned)
        {
            Debug.Log("Enemy is not stunned");
            return;
        }

        isAttacking = true;

        animator.SetTrigger("Attack");

        Debug.Log("Attack Success");

        CurrentState = PlayerState.Attack;

        StartCoroutine(EndAttack());
    }

    private IEnumerator EndAttack()
    {
        yield return new WaitForSeconds(1.0f);

        CurrentState = PlayerState.Idle;

        isAttacking = false;
    }

    /// <summary>
    /// 左回避
    /// </summary>
    public void DodgeLeft()
    {
        LastDodgeDirection = DodgeDirection.Left;

        CurrentState = PlayerState.Dodge;

        Debug.Log("Dodge Left");
    }

    /// <summary>
    /// 右回避
    /// </summary>
    public void DodgeRight()
    {
        LastDodgeDirection = DodgeDirection.Right;

        CurrentState = PlayerState.Dodge;

        Debug.Log("Doge Right");
    }

    /// <summary>
    /// 回避状態をリセット
    /// </summary>
    public void ClearDodge()
    {
        LastDodgeDirection = DodgeDirection.None;

        if(CurrentState == PlayerState.Dodge)
        {
            CurrentState = PlayerState.Idle;
        }
    }

    public void StartGuard()
    {
        CurrentState = PlayerState.Guard;

        Debug.Log("Guard");
    }

    public void EndGuard()
    {
        if(CurrentState == PlayerState.Guard)
        {
            CurrentState = PlayerState.Idle;
        }

        Debug.Log("Guard End");
    }
}
