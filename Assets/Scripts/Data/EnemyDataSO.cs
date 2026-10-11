using UnityEngine;

namespace CrystalForge.Data
{
    [CreateAssetMenu(fileName = "NewEnemyData", menuName = "CrystalForge/Data/Enemy Data")]
    public class EnemyDataSO : ScriptableObject
    {
        [Header("Identity")]
        public string enemyId = "crystal_slime";
        public string enemyName = "Crystal Slime";
        [TextArea(2, 3)]
        public string loreDescription = "Thạch pha lê mềm mại thường lảng vảng quanh khu mỏ.";
        public Sprite enemyAvatar;

        [Header("Combat Stats")]
        public int maxHp = 300;
        public int attackPower = 25;
        public int defense = 5;
        [Range(0f, 100f)]
        public float critRatePercent = 5f;
        public bool isBoss = false;

        [Header("Rewards Upon Defeat")]
        public double crystalReward = 50.0;
        [Tooltip("Hệ số nhân sản lượng tạm thời (VD: 1.5x)")]
        public float productionBuffMultiplier = 1.0f;
        [Tooltip("Thời gian buff sản lượng (giây)")]
        public float buffDurationSeconds = 0f;
    }
}
