using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using PlayerSystem.Fight;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// ボスがいる部屋を開くためのキーアイテム
/// </summary>
public class KeyItem : InteractableItemBase
{
    [SerializeField] private string _keyName;
    [SerializeField] private float _moveY = 1.5f;
    [SerializeField] private BariierSystem _bariierSystem; //行動範囲を制限しているバリアを管理するクラス
    private Transform _keyObject; //親オブジェクトのトランスフォーム
    
    [SerializeField] private List<Health> _targets = new List<Health>(); //倒さなければいけないエネミーのHealthクラスを管理する
    
    private void Start()
    {
        _keyObject = transform.parent;
        _keyObject.DOMoveY(_keyObject.position.y + _moveY, 1.5f).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
    }
    
    public override async void Interact()
    {
        //まだ範囲内に敵が残っていた場合
        foreach (IHealth target in _targets)
        {
            if (!target.IsDead)
            {
                UIManager.Instance.ShowQuestMessage(); //警告メッセージを表示
                
                await UniTask.DelayFrame(100);
                
                UIManager.Instance.HideQuestMessage(); //警告メッセージを非表示
                return;
            }
        }
        
        //CameraManager.Instance.UseTargetGroup(_keyObject, 1, 0.5f);
        //CameraManager.Instance.UseCamera(1);
        
        //敵が残っておらず、キーアイテムが取得可能な状態の時
        
        AudioManager.Instance.PlaySE(6);
        UIManager.Instance.QuestUpdate();
        
        Inventory inventory = _player.GetComponent<Inventory>(); //プレイヤーからイベントリクラスを取得
        if (inventory != null)
        {
            inventory.AddKey(_keyName);
            Destroy(gameObject.transform.parent.gameObject); //キーを追加したらオブジェクトを削除する
            _bariierSystem.HideBariier();
        }
        
        //CameraManager.Instance.UseCamera(0);
    }
}
