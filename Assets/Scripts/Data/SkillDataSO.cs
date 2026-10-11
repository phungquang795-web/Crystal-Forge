using UnityEngine;
using CrystalForge.Core;

namespace CrystalForge.Data
{
    [CreateAssetMenu(fileName = "NewSkillData", menuName = "CrystalForge/Data/Skill Data")]
    public class SkillDataSO : ScriptableObject
    {
        [Header("Identity")]
        public string skillId = "fireball";
        public string skillName = "Fireball";
        public SkillType skillType = SkillType.Attack;
        [TextArea(2, 4)]
        public string description = "Gây thêm 50 sát thương phép sau mỗi 3 lượt đánh.";
        public Sprite skillIcon;

        [Header("Combat Mechanics")]
        [Tooltip("Sát thương cộng thêm")]
        public int flatBonusDamage = 50;
        [Tooltip("Số lượt đánh kích hoạt đòn đánh phụ (0 = mỗi lượt)")]
        public int triggerIntervalTurns = 3;
        [Tooltip("Tỉ lệ kích hoạt ngẫu nhiên theo phần trăm (0 - 100)")]
        [Range(0f, 100f)]
        public float procChancePercent = 0f;

        [Header("Support / Defense Effects")]
        [Tooltip("Hồi phục % máu theo sát thương gây ra")]
        [Range(0f, 100f)]
        public float lifestealPercent = 0f;
        [Tooltip("Giảm % sát thương gánh chịu")]
        [Range(0f, 100f)]
        public float damageReductionPercent = 0f;
        [Tooltip("Cộng thẳng % tỉ lệ chí mạng")]
        [Range(0f, 100f)]
        public float critRateBonusPercent = 0f;
    }
}
