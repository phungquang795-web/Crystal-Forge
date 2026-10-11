using System;
using System.Collections.Generic;
using UnityEngine;
using CrystalForge.Core;
using CrystalForge.Data;

namespace CrystalForge.Economy
{
    [Serializable]
    public class GeneratorRuntimeState
    {
        public GeneratorDataSO data;
        public int currentLevel = 0;
        public bool isUnlocked = false;

        public double CurrentCost => data != null ? data.CalculateCost(currentLevel) : 0.0;
        public double CurrentProduction => data != null ? data.CalculateProduction(currentLevel) : 0.0;
    }

    [DefaultExecutionOrder(-50)]
    public class GeneratorSystem : MonoBehaviour
    {
        public static GeneratorSystem Instance { get; private set; }

        [Header("Generator Configurations")]
        [SerializeField] private List<GeneratorDataSO> generatorConfigs = new List<GeneratorDataSO>();

        private readonly Dictionary<GeneratorType, GeneratorRuntimeState> generatorStates = new Dictionary<GeneratorType, GeneratorRuntimeState>();

        // Events
        public event Action<GeneratorType, int> OnGeneratorUpgraded; // Type, newLevel
        public event Action<GeneratorType> OnGeneratorUnlocked;
        public event Action<double> OnTotalProductionChanged;

        public IReadOnlyDictionary<GeneratorType, GeneratorRuntimeState> GeneratorStates => generatorStates;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            InitializeGenerators();
        }

        private void Start()
        {
            // Lắng nghe sự kiện Lifetime Crystal để tự động mở khóa theo mốc nếu đủ điều kiện
            if (ResourceManager.Instance != null)
            {
                ResourceManager.Instance.OnLifetimeCrystalChanged += CheckAutoUnlockGenerators;
            }
            RecalculateTotalProduction();
        }

        private void OnDestroy()
        {
            if (ResourceManager.Instance != null)
            {
                ResourceManager.Instance.OnLifetimeCrystalChanged -= CheckAutoUnlockGenerators;
            }
        }

        private void InitializeGenerators()
        {
            generatorStates.Clear();
            foreach (var config in generatorConfigs)
            {
                if (config == null) continue;

                var state = new GeneratorRuntimeState
                {
                    data = config,
                    currentLevel = 0,
                    isUnlocked = false
                };
                generatorStates[config.generatorType] = state;
            }
        }

        private void CheckAutoUnlockGenerators(double lifetime)
        {
            foreach (var kvp in generatorStates)
            {
                var state = kvp.Value;
                if (!state.isUnlocked && state.data != null)
                {
                    if (lifetime >= state.data.unlockLifetimeRequired)
                    {
                        UnlockGenerator(kvp.Key);
                    }
                }
            }
        }

        public void UnlockGenerator(GeneratorType type)
        {
            if (generatorStates.TryGetValue(type, out var state))
            {
                if (!state.isUnlocked)
                {
                    state.isUnlocked = true;
                    OnGeneratorUnlocked?.Invoke(type);
                }
            }
        }

        /// <summary>
        /// Nâng cấp máy phát theo công thức lũy tiến
        /// </summary>
        public bool TryUpgrade(GeneratorType type)
        {
            if (!generatorStates.TryGetValue(type, out var state)) return false;
            if (!state.isUnlocked) return false;

            double cost = state.CurrentCost;
            if (ResourceManager.Instance != null && ResourceManager.Instance.TrySpend(cost))
            {
                state.currentLevel++;
                RecalculateTotalProduction();
                OnGeneratorUpgraded?.Invoke(type, state.currentLevel);
                return true;
            }
            return false;
        }

        public void RecalculateTotalProduction()
        {
            double total = 0.0;
            foreach (var kvp in generatorStates)
            {
                total += kvp.Value.CurrentProduction;
            }

            if (ResourceManager.Instance != null)
            {
                ResourceManager.Instance.SetBaseProduction(total);
            }
            OnTotalProductionChanged?.Invoke(total);
        }

        public GeneratorRuntimeState GetState(GeneratorType type)
        {
            generatorStates.TryGetValue(type, out var state);
            return state;
        }

        /// <summary>
        /// Nạp dữ liệu cấp độ khi Load Game
        /// </summary>
        public void LoadLevels(Dictionary<GeneratorType, int> loadedLevels)
        {
            foreach (var kvp in loadedLevels)
            {
                if (generatorStates.TryGetValue(kvp.Key, out var state))
                {
                    state.currentLevel = kvp.Value;
                    if (state.currentLevel > 0)
                    {
                        state.isUnlocked = true;
                    }
                }
            }
            RecalculateTotalProduction();
        }
    }
}
