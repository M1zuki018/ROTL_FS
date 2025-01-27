using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// リスポーンイベントの実装
/// </summary>
public class RespawnEvent : MonoBehaviour
{
    [Header("設定")]
    [SerializeField, HighlightIfNull] private PlayerMovement _player;
    [SerializeField, Comment("フェードの長さ（単位はミリ秒）")] private int _fadeTime = 1500;
    [SerializeField, Comment("暗転の長さ（単位はミリ秒）")] private int _blackOutTime = 500;
    
    private CharacterController _playerController;
    private PlayerInput _playerInput;
    
    private RespawnSystem _system;
    private Vector3 _initialPosition; //初期位置を保存しておく
    private Quaternion _initialRotation; //初期の回転を保存しておく

    private void Start()
    {
        _system = GetComponent<RespawnSystem>();
        _system.OnRespawn += HandleRespawn; //登録
        
        //初期位置を保存しておく
        _initialPosition = _player.gameObject.transform.position; 
        _initialRotation = _player.gameObject.transform.rotation;
        _playerController = _player.gameObject.GetComponent<CharacterController>();
        _playerInput = _player.gameObject.GetComponent<PlayerInput>();
    }

    private void OnDestroy()
    {
        _system.OnRespawn -= HandleRespawn; //解除
    }

    /// <summary>
    /// リスポーンの処理
    /// </summary>
    private async void HandleRespawn()
    {
        UIManager.Instance.FadeOut();
        await UniTask.Delay(_fadeTime); //フェードを待つ
        
        //暗転中の処理
        _playerInput.DeactivateInput(); //全ての入力を無効化
        
        _player._animator.applyRootMotion = false; //一時的にルートモーション・CharacterControllerを無効化する
        _playerController.enabled = false;
        
        _player.gameObject.transform.position = _initialPosition; //初期状態に戻す
        _player.gameObject.transform.rotation = _initialRotation;
        
        _player._animator.applyRootMotion = true; //有効に戻す
        _playerController.enabled = true;
        
        _system.MonitorFall(); //再度落下の監視を行う
        
        await UniTask.Delay(_blackOutTime); //暗転中の処理が十分に終わるまで待つ
        
        UIManager.Instance.FadeIn();
        
        await UniTask.Delay(_fadeTime); //フェードを待つ
        
        _playerInput.ActivateInput(); //全ての入力を有効化
    }
}
