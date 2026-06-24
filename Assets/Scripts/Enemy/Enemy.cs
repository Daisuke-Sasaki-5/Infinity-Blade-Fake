using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// EnemyState
/// </summary>
public enum EnemyState
{
    Idle,            // ‘Ò‹@
    PrepareAttack,   // UŒ‚—\’›
    Attack,          // UŒ‚Às
    Stunned,         // d’¼
    Dead             // €–S
}

/// <summary>
/// UŒ‚•ûŒü
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

    [Header("ƒXƒ^ƒ“ŠÔ")]
    [SerializeField] private float stunTimer = 2.0f;

    void Start()
    {
        CurrentState = EnemyState.Idle;

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        while (true)
        {
            // ƒXƒ^ƒ“’†‚ÍUŒ‚‚µ‚È‚¢
            if(CurrentState == EnemyState.Stunned)
            {
                yield return null;
                continue;
            }

            yield return new WaitForSeconds(3f);

            CurrentState = EnemyState.PrepareAttack;

            // UŒ‚•ûŒü‚ğƒ‰ƒ“ƒ_ƒ€‚É‘I‘ğ
           // CurrentAttackDirection = (EnemyAttackDirection)UnityEngine.Random.Range(1, 4);
            CurrentAttackDirection = EnemyAttackDirection.Center;

            Debug.Log("Enemy Prepare : Left");

            yield return new WaitForSeconds(1f);

            CurrentState = EnemyState.Attack;

            if(ISSuccessDodge())
            {
                Debug.Log("Success");

                StartCoroutine(StunRoutine());

                player.ClearDodge();
            }
            else
            {
                Debug.Log("Hit");

                CurrentState = EnemyState.Idle;
            }
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
    /// UŒ‚‚²‚Æ‚ÌŠe”»’è
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

    // ˆê“I‚É’Ç‰Á
    private void OnGUI()
    {
        GUI.Label(new Rect(20,150,500,50),$"Enemy : {CurrentState}{CurrentAttackDirection}");

        GUI.Label(
            new Rect(20, 200, 500, 50),
            $"Player : {player.CurrentState}"
        );
    }
}
