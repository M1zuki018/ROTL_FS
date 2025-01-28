using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PlayerSystem.Fight;
using UnityEngine;
using UniRx;

/// <summary>
/// 攻撃時の衝突判定を管理
/// </summary>
public class AttackHitDetector : MonoBehaviour
{
    //TODO: システム基盤を変更していく場合、Updateのタイミングを指定できるようにする

    [Header("設定")] 
    [SerializeField, Comment("衝突検出の所有者")] private CharacterBase _owner; 
    [SerializeField, Comment("衝突検出を行うレイヤー")] private LayerMask _hitLayer;
    [SerializeField, Comment("衝突するタグ")] private string[] _hitTags;
    
    [Header("検出フレームの範囲")] 
    [Range(0, 1)] [ReadOnlyOnRuntime] [SerializeField] private float _frame;

    [Header("検出位置と有効範囲の設定")]
    [SerializeField] private Data[] _hitPositions =
    {
        new() //初期設定
        {
            Range = new Vector2(0, 1), //範囲
            Collisions = new[]
            {
                new CollisionData
                {
                    Range = new Vector2(0, 1), //範囲
                    Offset = Vector3.zero, //オフセット
                    Rotation = Vector3.zero, //回転
                    Scale = Vector3.one //スケール
                }
            }
        }
    };
    
    private Transform _transform;
    
    private readonly List<GameObject> hitObjects = new();
    private readonly List<Collider> hitCollidersInThisFrame = new();
    private readonly List<GameObject> hitObjectsInThisFrame = new();
    
    public event Action<List<GameObject>> OnHitObjects;

    [SerializeField] private int _currentStage;

    /// <summary>多段攻撃の場合、現在何段目なのか取得するためのプロパティ</summary>
    public int CurrentStage
    {
        get => _currentStage; 
        set => _currentStage = value;
    }

    /// <summary>検出のフレーム範囲を更新</summary>
    public float Frame => _frame;

    private void Awake()
    {
        TryGetComponent(out _transform);
        
        /*
        // ステージ変更時にフレームをリセット
        _currentStage
            .DistinctUntilChanged()
            .Subscribe(stage =>
            {
                _frame = 0f; // フレームをリセット
                Debug.Log($"Stage changed to {stage}. Frame reset.");
            })
            .AddTo(this);
        */
    }

    private void OnEnable()
    {
        hitObjects.Clear(); //リストの中身をクリアする
    }

    /// <summary>
    /// 攻撃した時に呼び出す
    /// </summary>
    public List<IDamageable> PerformAttack()
    {
        return DetectCollisions();
    }

    /// <summary>
    /// 判定検出を実行する
    /// </summary>
    private List<IDamageable> DetectCollisions()
    {
        //処理開始時にリストをクリア
        hitCollidersInThisFrame.Clear();
        hitObjectsInThisFrame.Clear();

        // transformの位置と回転をキャッシュ
        Vector3 position = _transform.position;
        Quaternion rotation = _transform.rotation;
        Collider[] hitResults = new Collider[50];
        List<IDamageable> damageables = new();
        
        var data = _hitPositions[CurrentStage]; 
        if (!data.IsInRange(_frame)) //効果フレームの範囲外の場合は処理をスキップ
            return null;

        var activeCollisions = data.GetActiveCollisions(_frame); //有効なHitボックスを取得
        foreach (var col in activeCollisions)
        { 
            // 衝突検出を実行
            var count = CalculateSphereCast(hitResults, col, position, rotation);
            //Debug.Log(this.gameObject.transform.parent.name + "衝突した数" + count);
                
            // 範囲内で衝突検出を実行
            for (var i = 0; i < count; i++) 
            { 
                var hit = hitResults[i];
                var hitObject = hit.gameObject;
                    
                if (!IsValidTarget(hitObject)) 
                    continue;
                
                //if (hitObjects.Contains(hitObject) || !IsValidTarget(hitObject)) continue;
                    
                //コライダーを登録
                hitObjects.Add(hitObject);
                hitCollidersInThisFrame.Add(hit);
                hitObjectsInThisFrame.Add(hitObject);
            }
        }

        //Debug.Log(this.gameObject.transform.parent.name + "リスト作成前" + hitObjectsInThisFrame.Count);
        if (hitObjectsInThisFrame.Count > 0)
        {
            foreach (var hit in hitObjectsInThisFrame)
            {
                damageables.Add(hit.GetComponent<IDamageable>());
            }
        }
            
        //Debug.Log(this.gameObject.transform.parent.name + "作成されたリストに残っている数" + damageables.Count);
        return damageables;
    }

    private int CalculateSphereCast(Collider[] hitColliders, CollisionData col, Vector3 position, Quaternion rotation)
    {
        var worldPosition = position + _transform.TransformVector(col.Offset);
        var colRotation = rotation * Quaternion.Euler(col.Rotation);
        return Physics.OverlapBoxNonAlloc(worldPosition, col.Scale * 0.5f,
            hitColliders, colRotation, _hitLayer, QueryTriggerInteraction.Ignore);
    }

    /// <summary>
    /// ターゲットがHitTagに設定したタグを持っているか調べる
    /// </summary>
    private bool IsValidTarget(GameObject target)
    {
        foreach (var tag in _hitTags)
        {
            if (target.CompareTag(tag)) return true;
        }
        return _hitTags.Length == 0;
    }
    
    /// <summary>
    /// ギズモを表示する
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;

        if (_hitPositions == null || _hitPositions.Length <= CurrentStage) 
            return;
        
        var data = _hitPositions[CurrentStage];
        
        //現在のフレームがデータの範囲外なら描画しない
        if(!data.IsInRange(_frame))
            return;
        
        //var isInRange = data.IsInRange(_frame);

        // 有効な CollisionData を取得
        var activeCollisions = data.GetActiveCollisions(_frame);
        
        // 検出範囲をグラフィカルに表示
        foreach (var col in activeCollisions)
        {
            Gizmos.color = enabled ? Color.white : new Color(Color.white.r, Color.white.g, Color.white.b, 0.4f);
            var position = _transform != null 
                ? _transform.position + _transform.TransformVector(col.Offset)
                : transform.position + transform.TransformVector(col.Offset);
            var rotation = _transform != null
                ? _transform.rotation * Quaternion.Euler(col.Rotation)
                : transform.rotation * Quaternion.Euler(col.Rotation);

            Gizmos.matrix = Matrix4x4.TRS(position, rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, col.Scale);
            
            /*
            var worldPosition = pos + transform.TransformVector(col.Offset);
            var rotation = rot * Quaternion.Euler(col.Rotation);

            // コンポーネントが有効な場合は白、無効な場合は半透明の白でGizmosの色を設定
            Gizmos.color = enabled ? Color.white : new Color(Color.white.r, Color.white.g, Color.white.b, 0.4f);

            // ワイヤーフレームとして範囲を表示
            Gizmos.matrix = Matrix4x4.TRS(worldPosition, rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, col.Scale);

            // 範囲内かつ有効な場合は半透明の黄色で表示
            if (isInRange && enabled)
            {
                Gizmos.color = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, 0.2f);
                Gizmos.DrawCube(Vector3.zero, col.Scale);
            } ;
            */
        }
        
    }
}