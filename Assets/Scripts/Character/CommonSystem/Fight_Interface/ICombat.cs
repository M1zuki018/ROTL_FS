namespace PlayerSystem.Fight
{
    /// <summary>
    /// 攻撃に関する処理を管理します
    /// </summary>
    public interface ICombat
    {
        /// <summary>攻撃力</summary>
        int BaseAttackPower { get; }

        /// <summary>通常攻撃</summary>
        void Attack();
        
        /// <summary>特殊スキル</summary>
        /// <param name="target">攻撃対象</param>
        void UseSkill(int index, IDamageable target);
        
        /// <summary>攻撃判定の検知</summary>
        AttackHitDetector Detector { get; }
    }
}