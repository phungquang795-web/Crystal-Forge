using System;

namespace CrystalForge.Core
{
    public enum GameState
    {
        IdleWorkshop,
        InAdventure,
        Paused
    }

    public enum AdventureState
    {
        None,
        Exploring,
        EventChoice,
        AutoCombat,
        SkillSelection,
        AdventureReward,
        RunDefeat
    }

    public enum SkillType
    {
        Attack,
        Defense,
        Support,
        Passive
    }

    public enum MilestoneTier
    {
        Bronze = 0,
        Silver = 1,
        Gold = 2,
        Platinum = 3,
        Guardian = 4
    }

    public enum GeneratorType
    {
        CrystalDrill = 0,
        ManaFurnace = 1,
        AutoExtractor = 2
    }

    public enum PlayerStatType
    {
        Hp = 0,
        Attack = 1,
        Speed = 2,
        Skill = 3
    }

    public enum RelicType
    {
        ThunderHammer = 0,
        GlacialAegis = 1,
        IgnisCore = 2
    }
}

