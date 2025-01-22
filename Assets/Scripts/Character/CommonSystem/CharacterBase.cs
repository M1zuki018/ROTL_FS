using UnityEngine;
using PlayerSystem.Fight;

/// <summary>
/// 全キャラクターの基底クラス
/// </summary>
public abstract class CharacterBase : MonoBehaviour
{
    protected IHealth _health; //体力の管理
    protected UIManager _uiManager; //UIの管理
    
    protected virtual void Awake() 
    { 
        _health = GetComponent<IHealth>(); 
        _uiManager = FindObjectOfType<UIManager>(); 
        _health.OnDamaged += HandleDamage; //イベント登録
        _health.OnDeath += HandleDeath;
    }

    protected virtual void OnDestroy()
    {
        _health.OnDamaged -= HandleDamage; //イベント解除
        _health.OnDeath -= HandleDeath;
    }
    
    /// <summary>現在のHPを取得する</summary>
    public int GetCurrentHP() => _health.CurrentHP;
    /// <summary>最大HPを取得する</summary>
    public int GetMaxHP() => _health.MaxHP;

    /// <summary>ダメージを受けた時の処理</summary>
    protected abstract void HandleDamage(int damage, GameObject attacker);
    /// <summary>死亡した時の処理</summary>
    protected abstract void HandleDeath(GameObject attacker);
}
