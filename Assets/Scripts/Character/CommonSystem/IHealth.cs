using System;
using PlayerSystem.Fight;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// すべてのエンティティのHPを管理するクラス
/// </summary>
public class Health : MonoBehaviour, IHealth
{
    [SerializeField] private int _maxHPData = 100;
    public int MaxHP { get; private set; } = 100; //最大HP
    public int CurrentHP { get; private set; } //現在のHP
    public bool IsDead => CurrentHP <= 0; //HPが0以下になったら死亡する
    public event Action<int, GameObject> OnDamaged; //ダメージを受けた時のイベント
    public event Action<int, GameObject> OnHealed; //回復イベント
    public event Action<GameObject> OnDeath; //死亡イベント

    
    private void Awake()
    {
        MaxHP = _maxHPData;
        CurrentHP = _maxHPData; //HPを初期化する
    }
    
    /// <summary>
    /// ダメージを受ける処理
    /// </summary>
    public void TakeDamage(int amount, GameObject attacker)
    {
        if(IsDead) return; //死亡状態ならこれ以降の処理は行わない
        
        CurrentHP -= amount;
        OnDamaged?.Invoke(amount, attacker); //ダメージイベント発火

        if (IsDead)
        {
            OnDeath?.Invoke(attacker); //死亡イベント発火
        }
    }

    /// <summary>
    /// 回復処理
    /// </summary>
    public void Heal(int amount, GameObject healer)
    {
        if(IsDead) return; //死亡状態ならこれ以降の処理は行わない
        
        CurrentHP += amount;
        OnHealed?.Invoke(amount, healer);
    }

    /// <summary>
    /// 死亡処理
    /// </summary>
    public void Die()
    {
        Debug.Log("死んでしまった！！！！");
    }
}
