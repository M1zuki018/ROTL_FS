using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 通常攻撃4段目の補正
/// </summary>
public class NormalAttack_Turn : AttackAdjustBase
{
    [SerializeField] private HitDetectionInfo[] _hitDetectionInfo;   

    public override async void StartAttack()
    {
        _hitDetector.DetectHit(_hitDetectionInfo[0]); //当たり判定を発生させる
        
        if (_target != null)
        {
            //TODO:ターゲットがいるときの補正処理を書く
        }
        else
        {
            _animator.applyRootMotion = true;
        }

        AudioManager.Instance?.PlaySE(8);
        
        await UniTask.Delay(300);
        
        _hitDetector.DetectHit(_hitDetectionInfo[1]); //回転後の判定
    }

    public override void CorrectMovement(Vector3 forwardDirection){ }
}
