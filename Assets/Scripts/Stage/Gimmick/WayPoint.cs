using UnityEngine;

/// <summary>
/// 目標地点を管理する
/// </summary>
public class Waypoint : MonoBehaviour
{
    private WayPointSystem _wayPointSystem;
    private RespawnEvent _respawn;

    /// <summary>
    /// 目標地点システムを参照をセットする
    /// </summary>
    public void Initialize(WayPointSystem wps, RespawnEvent respawn)
    {
        _wayPointSystem = wps;
        _respawn = respawn;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _respawn.SetRespawn(other.gameObject.transform.position, other.gameObject.transform.rotation); //リスポーン地点を更新
            _wayPointSystem.NextWaypoint(); //次の地点のアイコンを表示する
        }
    }

    /// <summary>
    /// アイコンの表示非表示を切り替える
    /// </summary>
    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}