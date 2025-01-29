using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

/// <summary>
/// ゲームクリア後の処理を書いたクラス
/// ゲームクリアのステートはEnemyBrainで変更される
/// </summary>
public class GameClearPerformance : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.OnClear += HandleClearPerformance; //登録
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnClear -= HandleClearPerformance; //解除
    }

    /// <summary>
    /// 処理
    /// </summary>
    private async void HandleClearPerformance()
    {
        //UIを隠す（ボス・プレイヤーのUI）
        UIManager.Instance.HideBossUI();
        UIManager.Instance.HidePlayerBattleUI();
        UIManager.Instance.HideRightUI();

        //await UniTask.Delay(2000); //少し待つ
        
        UIManager.Instance.FadeOut();
        
        await UniTask.Delay(1000);
        
        EditorApplication.isPlaying = false;//ゲームプレイ終了
    }
}
