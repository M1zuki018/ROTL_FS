using UnityEngine;

/// <summary>
/// レーザービームPrefabにつけるクラス。詳細な設定を行える
/// </summary>
public class LaserParticle : MonoBehaviour
{
    [SerializeField] private ParticleSystem _laserEffect;
    public GameObject LaserEffect => _laserEffect.gameObject;

    /// <summary>
    /// レーザーを放つ
    /// </summary>
    public void Fire(Transform firePoint)
    {
        _laserEffect.transform.position = firePoint.position;
        _laserEffect.Play();
    }

    /// <summary>
    /// レーザーを止める
    /// </summary>
    public void Stop()
    {
        _laserEffect.Stop();
    }
}