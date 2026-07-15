using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
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

    public float PrepareProgress {  get; private set; }

    [SerializeField] private Player player;

    [Header("スタン時間")]
    [SerializeField] private float stunTimer = 2.0f;

    [SerializeField] private Animator animator;
    private bool attackFinished;

    [Header("HP")]
    [SerializeField] private int maxHP = 100;
    [Header("プレイヤーへのダメージ量")]
    [SerializeField] private int damage = 10;

    [Header("ヒットエフェクト")]
    [SerializeField] private GameObject effectPrefab;

    [Header("SE")]
    [SerializeField] private AudioClip hitClip;
    [SerializeField] private AudioSource audioSource;

    public int CurrentHP { get; private set; }

    public int MaxHP => maxHP;

    void Start()
    {
        CurrentState = EnemyState.Idle;

        CurrentHP = maxHP;

        StartCoroutine(AttackRoutine());

        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// 一定間隔で攻撃を繰り返すループ
    /// </summary>
    /// <returns></returns>
    private IEnumerator AttackRoutine()
    {
        while (true)
        {
         if(GameManager.instance.currentState == GameState.Win || GameManager.instance.currentState == GameState.Lose)
            {
                yield break;
            }

            // スタン中は攻撃しない
            if(CurrentState == EnemyState.Stunned)
            {
                yield return null;
                continue;
            }

            yield return new WaitForSeconds(3f);

            yield return StartCoroutine(AttackSequence());
        } 
    }

    /// <summary>
    /// 攻撃予兆から攻撃実行までの流れ
    /// </summary>
    /// <returns></returns>
    private IEnumerator AttackSequence()
    {
        if (GameManager.instance.currentState == GameState.Win || GameManager.instance.currentState == GameState.Lose)
        {
            yield break;
        }

        attackFinished = false;

        CurrentState = EnemyState.PrepareAttack;

        // 攻撃方向をランダムに選択
        CurrentAttackDirection = (EnemyAttackDirection)UnityEngine.Random.Range(1, 4);

        Debug.Log("Enemy Attack");

        animator.speed = 0.3f;
        animator.SetTrigger("Attack");
        yield return StartCoroutine(PrepareAttack(1));

        if (GameManager.instance.currentState != GameState.Playing)
            yield break;

        CurrentState = EnemyState.Attack;

        while (!attackFinished) yield return null;
    }

    /// <summary>
    /// 一定時間スタン状態にする
    /// </summary>
    /// <returns></returns>
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
        bool timing = Time.time - player.LastDodgeTime <= 0.2f;
        bool GuradTiming = Time.time - player.LastGuradTime <= 0.2f;

        switch (CurrentAttackDirection)
        {
            case EnemyAttackDirection.Left:
                return timing && player.LastDodgeDirection == DodgeDirection.Right;

            case EnemyAttackDirection.Right:
                return timing && player.LastDodgeDirection == DodgeDirection.Left;

            case EnemyAttackDirection.Center:
                return GuradTiming && player.CurrentState == PlayerState.Guard;
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

    /// <summary>
    /// 攻撃アニメーションのヒットタイミングで呼ばれる
    /// </summary>
    public void OnAttackHit()
    {
        if (ISSuccessDodge())
        {
            Debug.Log("Success");

            StartCoroutine(StunRoutine());

            player.ClearDodge();
            player.EndGuard();

            return;
        }

        DamagePlayer();
    }

    private void DamagePlayer()
    {
        // ヒットSE
        audioSource.PlayOneShot(hitClip);

        player.TakeDamage(damage);
        Vector3 effectPos = player.transform.position + Vector3.up * 1.1f;
        Instantiate(effectPrefab, effectPos, Quaternion.identity);
    }

    private void FinishPrepare()
    {
        PrepareProgress = 1;
    }

    public void IsDead()
    {
        GameManager.instance.Win();
    }

    public void ResumeAttack()
    {
        animator.speed = 1f;
        PrepareProgress = 0;
        attackFinished = true;
    }

    /// <summary>
    /// 攻撃予兆ゲージを時間経過で更新する
    /// </summary>
    /// <param name="prepareTime"></param>
    /// <returns></returns>
    private IEnumerator PrepareAttack(float prepareTime)
    {
        PrepareProgress = 0;

        float timer = 0f;

        while (timer < prepareTime)
        {
            timer += Time.deltaTime;
            PrepareProgress = Mathf.Lerp(0f, 0.7f, timer / prepareTime);

            yield return null;
        }

        PrepareProgress = 0.7f;
    }

    // 一時的に追加
    //private void OnGUI()
    //{
    //    GUI.Label(new Rect(20,150,500,50),$"Enemy : {CurrentState}{CurrentAttackDirection}");

    //    GUI.Label(
    //        new Rect(20, 200, 500, 50),
    //        $"Player : {player.CurrentState}"
    //    );
    //}
}
