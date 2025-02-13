using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ゲーム開始演出
/// </summary>
public class GameStartPerformance : MonoBehaviour
{
    [SerializeField] private Button _button;
    
    private void Start()
    {
        GameManager.Instance.SetGameState(GameState.Title);
        _button.onClick.AddListener(() => GameStartButton());
    }

    /// <summary>
    /// ゲーム開始ボタンを押したときの処理
    /// </summary>
    private async void GameStartButton()
    {
        //フェードアウトしつつスタートパネルを非表示
        UIManager.Instance?.FadeOut();

        await UniTask.Delay(800);
        
        //暗転中
        UIManager.Instance?.HideStartPanel();
        SkinManager.Instance?.ChangeSkin(0);
        UIManager.Instance?.HideRightUI();
        UIManager.Instance?.HideFirstText();
        UIManager.Instance?.HideStartText();
        
        await UniTask.Delay(700);
        
        GameManager.Instance.SetGameState(GameState.Playing); //ステート変更
        UIManager.Instance?.FadeIn();
    }
}
