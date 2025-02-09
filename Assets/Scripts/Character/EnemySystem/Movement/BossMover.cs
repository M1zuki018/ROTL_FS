using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PlayerSystem.Fight;
using UniRx;
using UnityEngine;

/// <summary>
/// ボスの移動を制御するクラス
/// </summary>
public class BossMover : MonoBehaviour
{
    [SerializeField] private BossAttackPattern _attackPattern;

    private BossHealth _health;
    private CharacterController _cc;
    private Vector3 _initializePos;
    private int _currentPattern = 0; //現在の攻撃パターン
    private int _pattern2Count; //パターン2で使用する
    private bool _isDamageImmunity; //ダメージ無効
    private bool _isDPSCheak; //DPSチェック中かどうか
    
    public bool IsDamageImmunity => _isDamageImmunity;
    public bool IsDPSCheak => _isDPSCheak;

    private readonly List<Func<UniTask>> _attackPatterns = new();

    private void Start()
    {
        //コンポーネントの取得と初期化
        _health = GetComponent<BossHealth>();
        _cc = GetComponent<CharacterController>();
        _initializePos = transform.position;
        transform.position = new Vector3(_initializePos.x, _initializePos.y + 4f, _initializePos.z); //初期位置に移動
       
        /*
        //HPが50%以下になったら攻撃パターンを変更する
        Observable
            .EveryUpdate()
            .Where(_ => _health.CurrentHP <= _health.MaxHP * 0.5f)
            .Take(1)
            .Subscribe(_ => ChangeAttackPattern())
            .AddTo(this);
        */
        
        //HPが10%以下になったらDPSチェックを始める
        Observable
            .EveryUpdate()
            .Where(_ => _health != null && _health.CurrentHP <= _health.MaxHP * 0.1f) //HP10%以下になったら
            .Take(1) //一度だけに制限
            .Subscribe(_ => _isDamageImmunity = true) //ダメージ無効状態に変更する
            .AddTo(this);
        
        //攻撃パターンを登録
        _attackPatterns.Add(Pattern1);
        _attackPatterns.Add(Pattern2);
        _attackPatterns.Add(Pattern3);
    }

    /// <summary>
    /// ボス戦開始
    /// </summary>
    public async UniTask BattleStart()
    {
        await _attackPatterns[0]();
    }

    /// <summary>
    /// 地上で休憩する
    /// </summary>
    public async void Break()
    {
        if (IsDamageImmunity)
        {
            //ダメージ無効状態を解除してDPSチェックを始める
            _isDamageImmunity = false;
            DPSCheak().Forget();
            return;
        }
        
        _cc.Move(new Vector3(0, _initializePos.y - transform.position.y, 0)); //初期位置と現在の位置の差分だけ移動する
        Debug.Log("休み");
        await UniTask.Delay(6000);
        
        if (IsDamageImmunity)
        {
            //ダメージ無効状態を解除してDPSチェックを始める
            _isDamageImmunity = false;
            DPSCheak().Forget();
            return;
        }
        
        //次の攻撃に向けて移動する
        if (_currentPattern == 1) await Pattern2();
        else if (_currentPattern == 2) await Pattern3();
        else if (_currentPattern == 3)
        {
            _currentPattern = 0; //初期化
            await Pattern1();
        }
    }

    /// <summary>
    /// ゆっくり飛び上がる
    /// </summary>
    private async void Emerge()
    {
        _cc.Move(new Vector3(0, 4, 0));
        await UniTask.Delay(1000);
        
        //次の攻撃を行う
        if(_currentPattern == 1) Pattern2(); //パターン2に繋げる
        else if (_currentPattern == 0) Pattern1();
    }

    /// <summary>
    /// 特定の場所へワープする
    /// </summary>
    private void Warp(Vector3 position)
    {
        _cc.Move(position - transform.position);
    }
    
    
    /// <summary>
    /// 攻撃パターン1 レーザーメインの回避パート
    /// </summary>
    [ContextMenu("Pattern1")]
    public async UniTask Pattern1()
    {
       await _attackPattern.StartAttackPattern1();
       _currentPattern = 1;
    }

    /// <summary>
    /// 攻撃パターン2 影移動と近接戦闘
    /// </summary>
    [ContextMenu("Pattern2")]
    public async UniTask Pattern2()
    {
        await _attackPattern.StartShadowAttack();
        _currentPattern = 2;
    }

    /// <summary>
    /// 攻撃パターン3　時間操作
    /// </summary>
    [ContextMenu("Pattern3")]
    public async UniTask Pattern3()
    {
        Warp(new Vector3(100f, 15f, 250f));
        _attackPattern.DefaultTransform = transform.position;
        
        await UniTask.Delay(500); //発動予兆0.5秒
        
        _attackPattern.TimeControl(); //攻撃
        
        await UniTask.Delay(5000);

        _currentPattern = 3;
        Break();
    }

    /// <summary>
    /// DPSチェックの操作
    /// </summary>
    [ContextMenu("DPSCheak")]
    public async UniTask DPSCheak()
    {
        _isDPSCheak = true;
        
        //UIの操作
        UIManager.Instance.InitializeBossDpsSlider(_health.CurrentBreakAmount, _health.CurrentBreakAmount);
        UIManager.Instance.ShowBossDpsSlider();
        UIManager.Instance.HideBossUI();
        UIManager.Instance.HidePlayerHP();
    }

    [ContextMenu("LastAttack")]
    public async void LastAttack()
    {
        _attackPattern.FinalTimeControl();
    }

    public void After()
    {
        _attackPattern.After();
    }
    
    /// <summary>
    /// 攻撃パターンを変更する
    /// </summary>
    private void ChangeAttackPattern()
    {
        Debug.Log("攻撃パターン変更");
        _attackPatterns.Clear();
        _attackPatterns.Add(Pattern2);
        _attackPatterns.Add(Pattern3);
    }
}
