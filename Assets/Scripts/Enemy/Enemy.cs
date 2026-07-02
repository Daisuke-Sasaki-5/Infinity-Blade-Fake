using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

/// <summary>
/// EnemyState
/// </summary>
public enum EnemyState
{
    Idle,            // 待機
    PrepareAttack,   // 攻撃予兆
    Attack,          // 攻撃実行
    Stunned,         // 硬直
    Dead             // 死亡
}

/// <summary>
/// 攻撃方向
/// </summary>
public enum EnemyAttackDirection
{
    None,
    Left,
    Right,
    Center
}

public class Enemy : MonoBehaviour
{
    public EnemyState CurrentState {  get; private set; }
    public EnemyAttackDirection CurrentAttackDirection {  get; private set; }

    [SerializeField] private Player player;

    [Header("スタン時間")]
    [SerializeField] private float stunTimer = 2.0f;

    [SerializeField] private Animator animator;

    [Header("HP")]
    [SerializeField] private int maxHP = 100;

    [Header("ヒットエフェクト")]
    [SerializeField] private GameObject effectPrefab;

    public int CurrentHP { get; private set; }

    public int MaxHP => maxHP;

    void Start()
    {
        CurrentState = EnemyState.Idle;

        CurrentHP = maxHP;

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        while (true)
        {
            // スタン中は攻撃しない
            if(CurrentState == EnemyState.Stunned)
            {
                yield return null;
                continue;
            }

            yield return new WaitForSeconds(3f);

            CurrentState = EnemyState.PrepareAttack;

            // 攻撃方向をランダムに選択
            CurrentAttackDirection = (EnemyAttackDirection)UnityEngine.Random.Range(1, 4);

            Debug.Log("Enemy Attack");

            yield return new WaitForSeconds(1f);

            CurrentState = EnemyState.Attack;
            animator.SetTrigger("Attack");
        } 
    }

    private IEnumerator StunRoutine()
    {
        CurrentState  = EnemyState.Stunned;

        Debug.Log("Enemy Stunned");

        yield return new WaitForSeconds(stunTimer);

        CurrentAttackDirection = EnemyAttackDirection.None;
        CurrentState = EnemyState.Idle;

        Debug.Log("Enemy Recoverd");
    }

    /// <summary>
    /// 攻撃ごとの各判定
    /// </summary>
    /// <returns></returns>
    private bool ISSuccessDodge()
    {
        switch (CurrentAttackDirection)
        {
            case EnemyAttackDirection.Left:
                return player.LastDodgeDirection == DodgeDirection.Right;

            case EnemyAttackDirection.Right:
                return player.LastDodgeDirection == DodgeDirection.Left;

            case EnemyAttackDirection.Center:
                return player.CurrentState == PlayerState.Guard;
        }

        return false;
    }

    /// <summary>
    /// ダメージ関数
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(int damage)
    {
        CurrentHP -= damage;

        // HPが０になったとき
        if (CurrentHP <= 0)
        {
            CurrentHP = 0;

            CurrentState = EnemyState.Dead;

            animator.SetTrigger("Dead");

            Debug.Log("Enemy Dead");
        }
    }

    public void OnAttackHit()
    {
        if (ISSuccessDodge())
        {
            Debug.Log("Success");

            StartCoroutine(StunRoutine());

            player.ClearDodge();

            return;
        }

        player.TakeDamage(10);
        Vector3 effectPos = player.transform.position + Vector3.up * 1.1f;
        Instantiate(effectPrefab, effectPos, Quaternion.identity);

        CurrentState = EnemyState.Idle;
    }

    public void IsDead()
    {
        GameManager.instance.Win();
    }

    // 一時的に追加
    private void OnGUI()
    {
        GUI.Label(new Rect(20,150,500,50),$"Enemy : {CurrentState}{CurrentAttackDirection}");

        GUI.Label(
            new Rect(20, 200, 500, 50),
            $"Player : {player.CurrentState}"
        );
    }
}
