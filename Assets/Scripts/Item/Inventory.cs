using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーのインベントリを管理するクラス
/// </summary>
public class Inventory : MonoBehaviour
{
    private HashSet<string> _collectedKeys = new HashSet<string>(); //集めたキーのハッシュセット
    private void Start()
    {
        UIManager.Instance.UpdateQuestText(
            $"Collect the keys\nGoal {_collectedKeys.Count} / 3");
    }
    
    /// <summary>
    /// ハッシュセットに入手したキーを追加する
    /// </summary>
    public void AddKey(string keyName)
    {
        _collectedKeys.Add(keyName);
        UIManager.Instance.UpdateQuestText(
            $"Collect the keys\nGoal {_collectedKeys.Count} / 3");

        if (HasAllKeys())
        {
            QuestFinished();
        }
    }

    /// <summary>
    /// 3つキーが揃ったら呼び出す
    /// </summary>
    private void QuestFinished()
    {
        UIManager.Instance.UpdateQuestText($"Head to the boss door.");
    }

    /// <summary>
    /// 3つキーが揃っていたらtrueを返す
    /// </summary>
    public bool HasAllKeys()
    {
        return _collectedKeys.Count >= 3;
    }

    /// <summary>
    /// 現在所持しているキーの個数を返す
    /// </summary>
    public int CurrentHasKeys()
    {
        return _collectedKeys.Count;
    }

    public void UseKey()
    {
        _collectedKeys.Clear();
        UIManager.Instance.UpdateQuestText($"Fight the boss.");
        UIManager.Instance.ShowBossUI();
    }
}