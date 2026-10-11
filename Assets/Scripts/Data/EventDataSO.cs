using System;
using UnityEngine;

namespace CrystalForge.Data
{
    [Serializable]
    public struct EventChoice
    {
        public string choiceButtonText;
        [TextArea(1, 2)]
        public string outcomeDescription;

        [Header("Effects")]
        public double crystalChange;
        public int heroHpPercentChange; // e.g. -5 for -5% HP, +20 for heal 20%
        public float temporaryProductionBuff; // e.g. 1.5x
        public float buffDurationSeconds; // e.g. 60s
        public bool grantsRandomSkill;
        public bool triggersCombatEncounter;
        public EnemyDataSO overrideEnemy;
    }

    [CreateAssetMenu(fileName = "NewEventData", menuName = "CrystalForge/Data/Event Data")]
    public class EventDataSO : ScriptableObject
    {
        [Header("Event Identity")]
        public string eventId = "ancient_crystal";
        public string eventTitle = "Ancient Crystal";
        [TextArea(2, 4)]
        public string storyPrompt = "Bạn bắt gặp một khối tinh thể cổ đại đang phát ra luồng năng lượng dị thường...";
        public Sprite eventIllustration;

        [Header("Choices")]
        public EventChoice[] choices;
    }
}
