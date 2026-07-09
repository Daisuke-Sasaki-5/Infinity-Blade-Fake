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

    [Header("HP")]
    [SerializeField] private int maxHP = 100;
    [Header("敵へのダメージ量")]
    [SerializeField] private int damage = 20;

    [Header("ヒットエフェクト")]
    [SerializeField] private GameObject effectPrefab;

    [Header("SE")]
    [SerializeField] private AudioClip hitClip;
    [SerializeField] private AudioSource audioSource;

    public int CurrentHP {  get; private set; }

    public int MaxHP => maxHP;

    // 攻撃中フラグ
    private bool isAttacking;

    private void Start()
    {
        CurrentState = PlayerState.Idle;

        CurrentHP = maxHP;

        audioSource = GetComponent<AudioSource>();
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
    }

    public void EndAttack()
    {
        CurrentState = PlayerState.Idle;

        isAttacking = false;

        Debug.Log("END");
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
        animator.SetBool("Guard", true);

        Debug.Log("Guard");
    }

    public void EndGuard()
    {
        if(CurrentState == PlayerState.Guard)
        {
            CurrentState = PlayerState.Idle;
            animator.SetBool("Guard", false);
        }

        Debug.Log("Guard End");
    }

    /// <summary>
    /// ダメージ関数
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(int damage)
    {
        CurrentHP -= damage;
        CameraShake.instance.Shake();

        if (CurrentHP <= 0)
        {
            CurrentHP = 0;

            CurrentState = PlayerState.Dead;

            animator.SetTrigger("Dead");

            Debug.Log("Player Dead");
        }
    }

    /// <summary>
    /// 敵にダメージを与える
    /// </summary>
    public void OnAttackHit()
    {
        enemy.TakeDamage(damage);

        // ヒット音
        audioSource.PlayOneShot(hitClip);

        // エフェクトを出す
        Vector3 effectPos = enemy.transform.position + Vector3.up * 1.2f;
        Instantiate(effectPrefab, effectPos, Quaternion.identity);
    }

    public void IsDead()
    {
        GameManager.instance.Lose();
    }
}
