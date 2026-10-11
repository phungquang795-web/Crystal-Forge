using System;
using UnityEngine;
using CrystalForge.Adventure;
using CrystalForge.Core;
using CrystalForge.Economy;

namespace CrystalForge.Audio
{
    [DefaultExecutionOrder(-60)]
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource bgmSource;

        [Header("Volume Settings")]
        [Range(0f, 1f)] [SerializeField] private float masterVolume = 1.0f;
        [Range(0f, 1f)] [SerializeField] private float sfxVolume = 0.8f;
        [Range(0f, 1f)] [SerializeField] private float bgmVolume = 0.35f;
        [SerializeField] private bool isMuted = false;

        private AudioClip clickSfxClip;
        private AudioClip upgradeSfxClip;
        private AudioClip milestoneSfxClip;
        private AudioClip combatAttackClip;
        private AudioClip spellSfxClip;
        private AudioClip victorySfxClip;
        private AudioClip ambientBgmClip;
        private AudioClip overdriveSfxClip;
        private AudioClip bossRoarSfxClip;
        private AudioClip treasureChimeSfxClip;

        public bool IsMuted => isMuted;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            EnsureAudioSources();
            GenerateProceduralAudioClips();
        }

        private void Start()
        {
            SubscribeEvents();
            StartAmbientBgm();
        }

        private void OnDestroy()
        {
            UnsubscribeEvents();
        }

        private void EnsureAudioSources()
        {
            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
                sfxSource.loop = false;
            }

            if (bgmSource == null)
            {
                bgmSource = gameObject.AddComponent<AudioSource>();
                bgmSource.playOnAwake = false;
                bgmSource.loop = true;
            }
        }

        private void SubscribeEvents()
        {
            ClickSystem.OnCoreClicked += HandleCoreClicked;

            if (UpgradeManager.Instance != null)
            {
                UpgradeManager.Instance.OnTapPowerUpgraded += HandleTapUpgraded;
            }

            if (GeneratorSystem.Instance != null)
            {
                GeneratorSystem.Instance.OnGeneratorUpgraded += HandleGenUpgraded;
            }

            if (MilestoneManager.Instance != null)
            {
                MilestoneManager.Instance.OnMilestoneAchieved += HandleMilestoneAchieved;
            }

            if (AdventureManager.Instance != null)
            {
                AdventureManager.Instance.OnCombatLogMessage += HandleCombatLog;
                AdventureManager.Instance.OnStateChanged += HandleAdventureStateChanged;
            }
        }

        private void UnsubscribeEvents()
        {
            ClickSystem.OnCoreClicked -= HandleCoreClicked;

            if (UpgradeManager.Instance != null)
            {
                UpgradeManager.Instance.OnTapPowerUpgraded -= HandleTapUpgraded;
            }

            if (GeneratorSystem.Instance != null)
            {
                GeneratorSystem.Instance.OnGeneratorUpgraded -= HandleGenUpgraded;
            }

            if (MilestoneManager.Instance != null)
            {
                MilestoneManager.Instance.OnMilestoneAchieved -= HandleMilestoneAchieved;
            }

            if (AdventureManager.Instance != null)
            {
                AdventureManager.Instance.OnCombatLogMessage -= HandleCombatLog;
                AdventureManager.Instance.OnStateChanged -= HandleAdventureStateChanged;
            }
        }

        private void HandleCoreClicked(Vector3 pos, double amount)
        {
            PlayClickSfx();
        }

        private void HandleTapUpgraded(int level, double bonus, double cost)
        {
            PlayUpgradeSfx();
        }

        private void HandleGenUpgraded(GeneratorType type, int level)
        {
            PlayUpgradeSfx();
        }

        private void HandleMilestoneAchieved(CrystalForge.Data.MilestoneDataSO milestone)
        {
            PlayMilestoneSfx();
        }

        private void HandleCombatLog(string message, Color color)
        {
            if (message.Contains("Fireball") || message.Contains("Sét") || message.Contains("phép"))
            {
                PlaySpellSfx();
            }
            else if (message.Contains("Hero chém") || message.Contains("BẠO KÍCH") || message.Contains("cắn"))
            {
                PlayCombatAttackSfx();
            }
        }

        private void HandleAdventureStateChanged(AdventureState state)
        {
            if (state == AdventureState.AdventureReward)
            {
                PlayVictorySfx();
            }
        }

        public void PlayClickSfx()
        {
            if (isMuted || sfxSource == null || clickSfxClip == null) return;
            sfxSource.pitch = UnityEngine.Random.Range(0.95f, 1.08f);
            sfxSource.PlayOneShot(clickSfxClip, sfxVolume * masterVolume);
        }

        public void PlayUpgradeSfx()
        {
            if (isMuted || sfxSource == null || upgradeSfxClip == null) return;
            sfxSource.pitch = 1.0f;
            sfxSource.PlayOneShot(upgradeSfxClip, sfxVolume * masterVolume * 0.9f);
        }

        public void PlayMilestoneSfx()
        {
            if (isMuted || sfxSource == null || milestoneSfxClip == null) return;
            sfxSource.pitch = 1.0f;
            sfxSource.PlayOneShot(milestoneSfxClip, sfxVolume * masterVolume);
        }

        public void PlayCombatAttackSfx()
        {
            if (isMuted || sfxSource == null || combatAttackClip == null) return;
            sfxSource.pitch = UnityEngine.Random.Range(0.92f, 1.05f);
            sfxSource.PlayOneShot(combatAttackClip, sfxVolume * masterVolume * 0.85f);
        }

        public void PlaySpellSfx()
        {
            if (isMuted || sfxSource == null || spellSfxClip == null) return;
            sfxSource.pitch = UnityEngine.Random.Range(0.96f, 1.04f);
            sfxSource.PlayOneShot(spellSfxClip, sfxVolume * masterVolume * 0.9f);
        }

        public void PlayVictorySfx()
        {
            if (isMuted || sfxSource == null || victorySfxClip == null) return;
            sfxSource.pitch = 1.0f;
            sfxSource.PlayOneShot(victorySfxClip, sfxVolume * masterVolume);
        }

        public void ToggleMute()
        {
            isMuted = !isMuted;
            if (bgmSource != null)
            {
                bgmSource.mute = isMuted;
            }
            if (sfxSource != null)
            {
                sfxSource.mute = isMuted;
            }
        }

        public void SetMute(bool mute)
        {
            isMuted = mute;
            if (bgmSource != null) bgmSource.mute = isMuted;
            if (sfxSource != null) sfxSource.mute = isMuted;
        }

        private void StartAmbientBgm()
        {
            if (bgmSource == null || ambientBgmClip == null) return;
            bgmSource.clip = ambientBgmClip;
            bgmSource.volume = bgmVolume * masterVolume;
            bgmSource.loop = true;
            bgmSource.mute = isMuted;
            bgmSource.Play();
        }

        public void PlayOverdriveActivatedSfx()
        {
            if (isMuted || sfxSource == null || overdriveSfxClip == null) return;
            sfxSource.pitch = 1.0f;
            sfxSource.PlayOneShot(overdriveSfxClip, sfxVolume * masterVolume * 1.1f);
        }

        public void PlayBossRoarSfx()
        {
            if (isMuted || sfxSource == null || bossRoarSfxClip == null) return;
            sfxSource.pitch = UnityEngine.Random.Range(0.88f, 0.98f);
            sfxSource.PlayOneShot(bossRoarSfxClip, sfxVolume * masterVolume * 1.15f);
        }

        public void PlayTreasureChimeSfx()
        {
            if (isMuted || sfxSource == null || treasureChimeSfxClip == null) return;
            sfxSource.pitch = UnityEngine.Random.Range(1.02f, 1.12f);
            sfxSource.PlayOneShot(treasureChimeSfxClip, sfxVolume * masterVolume);
        }

        public void PlayGameOverSfx()
        {
            if (isMuted || sfxSource == null) return;
            sfxSource.pitch = 0.82f;
            if (spellSfxClip != null) sfxSource.PlayOneShot(spellSfxClip, sfxVolume * masterVolume * 1.2f);
            if (bossRoarSfxClip != null) sfxSource.PlayOneShot(bossRoarSfxClip, sfxVolume * masterVolume * 0.95f);
        }

        #region Procedural Audio Synthesis
        private void GenerateProceduralAudioClips()
        {
            clickSfxClip = CreateCrystalChimeClip("SFX_CrystalClick", 0.09f, 1800f, 3200f);
            upgradeSfxClip = CreateAnvilChimeClip("SFX_ForgeUpgrade", 0.35f, 880f, 1320f);
            milestoneSfxClip = CreateFanfareClip("SFX_MilestoneFanfare", 0.65f);
            combatAttackClip = CreateImpactHitClip("SFX_CombatAttack", 0.14f);
            spellSfxClip = CreateSpellBurstClip("SFX_SpellBurst", 0.38f);
            victorySfxClip = CreateVictoryFanfareClip("SFX_VictoryChime", 0.8f);
            ambientBgmClip = CreateAmbientFantasyLoop("BGM_ForgeAmbient", 12.0f);
            overdriveSfxClip = CreateOverdriveBurstClip("SFX_OverdriveBurst", 0.6f);
            bossRoarSfxClip = CreateBossRoarClip("SFX_BossRoar", 0.7f);
            treasureChimeSfxClip = CreateTreasureChimeClip("SFX_TreasureChime", 0.5f);
        }

        private AudioClip CreateCrystalChimeClip(string name, float duration, float freq1, float freq2)
        {
            int sampleRate = 44100;
            int samplesCount = (int)(sampleRate * duration);
            float[] data = new float[samplesCount];

            for (int i = 0; i < samplesCount; i++)
            {
                float t = (float)i / sampleRate;
                float envelope = Mathf.Exp(-t * 35f); // Fast decay
                float wave = 0.7f * Mathf.Sin(2f * Mathf.PI * freq1 * t) +
                             0.3f * Mathf.Sin(2f * Mathf.PI * freq2 * t);
                data[i] = wave * envelope;
            }

            AudioClip clip = AudioClip.Create(name, samplesCount, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private AudioClip CreateAnvilChimeClip(string name, float duration, float freqA, float freqB)
        {
            int sampleRate = 44100;
            int samplesCount = (int)(sampleRate * duration);
            float[] data = new float[samplesCount];

            for (int i = 0; i < samplesCount; i++)
            {
                float t = (float)i / sampleRate;
                float env = Mathf.Exp(-t * 9f);
                float wave = 0.5f * Mathf.Sin(2f * Mathf.PI * freqA * t) +
                             0.35f * Mathf.Sin(2f * Mathf.PI * freqB * t) +
                             0.15f * Mathf.Sin(2f * Mathf.PI * freqB * 1.5f * t);
                data[i] = wave * env;
            }

            AudioClip clip = AudioClip.Create(name, samplesCount, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private AudioClip CreateFanfareClip(string name, float duration)
        {
            int sampleRate = 44100;
            int samplesCount = (int)(sampleRate * duration);
            float[] data = new float[samplesCount];

            float[] notes = { 523.25f, 659.25f, 783.99f, 1046.50f }; // C5, E5, G5, C6
            float noteDuration = duration / notes.Length;

            for (int i = 0; i < samplesCount; i++)
            {
                float t = (float)i / sampleRate;
                int noteIndex = Mathf.Clamp((int)(t / noteDuration), 0, notes.Length - 1);
                float noteT = t - (noteIndex * noteDuration);
                float env = Mathf.Clamp01(1f - (noteT / noteDuration));

                float freq = notes[noteIndex];
                float wave = Mathf.Sin(2f * Mathf.PI * freq * t) * 0.7f +
                             Mathf.Sin(2f * Mathf.PI * freq * 2f * t) * 0.3f;
                data[i] = wave * env;
            }

            AudioClip clip = AudioClip.Create(name, samplesCount, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private AudioClip CreateImpactHitClip(string name, float duration)
        {
            int sampleRate = 44100;
            int samplesCount = (int)(sampleRate * duration);
            float[] data = new float[samplesCount];

            for (int i = 0; i < samplesCount; i++)
            {
                float t = (float)i / sampleRate;
                float env = Mathf.Exp(-t * 22f);
                float noise = (UnityEngine.Random.value * 2f - 1f) * 0.5f;
                float kickFreq = Mathf.Lerp(140f, 40f, t / duration);
                float punch = Mathf.Sin(2f * Mathf.PI * kickFreq * t) * 0.5f;
                data[i] = (noise + punch) * env;
            }

            AudioClip clip = AudioClip.Create(name, samplesCount, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private AudioClip CreateSpellBurstClip(string name, float duration)
        {
            int sampleRate = 44100;
            int samplesCount = (int)(sampleRate * duration);
            float[] data = new float[samplesCount];

            for (int i = 0; i < samplesCount; i++)
            {
                float t = (float)i / sampleRate;
                float progress = t / duration;
                float env = Mathf.Sin(progress * Mathf.PI);
                float freq = Mathf.Lerp(300f, 950f, Mathf.Sin(progress * Mathf.PI));
                float wave = Mathf.Sin(2f * Mathf.PI * freq * t) * 0.6f +
                             (UnityEngine.Random.value * 2f - 1f) * 0.25f;
                data[i] = wave * env;
            }

            AudioClip clip = AudioClip.Create(name, samplesCount, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private AudioClip CreateVictoryFanfareClip(string name, float duration)
        {
            int sampleRate = 44100;
            int samplesCount = (int)(sampleRate * duration);
            float[] data = new float[samplesCount];

            float[] chord = { 523.25f, 659.25f, 783.99f }; // C major chord

            for (int i = 0; i < samplesCount; i++)
            {
                float t = (float)i / sampleRate;
                float env = Mathf.Clamp01(1f - (t / duration));
                float wave = 0f;
                foreach (var f in chord)
                {
                    wave += Mathf.Sin(2f * Mathf.PI * f * t) * 0.3f;
                }
                data[i] = wave * env;
            }

            AudioClip clip = AudioClip.Create(name, samplesCount, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private AudioClip CreateAmbientFantasyLoop(string name, float duration)
        {
            int sampleRate = 22050; // Tiết kiệm bộ nhớ cho ambient
            int samplesCount = (int)(sampleRate * duration);
            float[] data = new float[samplesCount];

            // Vòng hợp âm Am -> F -> C -> G
            float[][] progression = new float[][] {
                new float[] { 220f, 261.63f, 329.63f }, // Am
                new float[] { 174.61f, 220f, 261.63f }, // F
                new float[] { 130.81f, 164.81f, 196f },  // C
                new float[] { 196f, 246.94f, 293.66f }   // G
            };

            float chordDuration = duration / progression.Length;

            for (int i = 0; i < samplesCount; i++)
            {
                float t = (float)i / sampleRate;
                int chordIdx = Mathf.Clamp((int)(t / chordDuration), 0, progression.Length - 1);
                float chordT = t - (chordIdx * chordDuration);

                // Smooth bell envelope for each chord transition
                float env = Mathf.Sin(Mathf.Clamp01(chordT / chordDuration) * Mathf.PI);

                float[] notes = progression[chordIdx];
                float sample = 0f;
                for (int n = 0; n < notes.Length; n++)
                {
                    sample += Mathf.Sin(2f * Mathf.PI * notes[n] * t) * 0.12f;
                }

                // Nhẹ nhàng thêm chút ấm áp của lò rèn (crackling whisper)
                float subtleWarmth = (UnityEngine.Random.value * 2f - 1f) * 0.015f;
                data[i] = (sample * env) + subtleWarmth;
            }

            AudioClip clip = AudioClip.Create(name, samplesCount, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private AudioClip CreateOverdriveBurstClip(string name, float duration)
        {
            int sampleRate = 44100;
            int samplesCount = (int)(sampleRate * duration);
            float[] data = new float[samplesCount];

            for (int i = 0; i < samplesCount; i++)
            {
                float t = (float)i / sampleRate;
                float env = Mathf.Pow(1f - (t / duration), 1.2f);
                // Frequency sweep upwards + thunder roar
                float freq = Mathf.Lerp(160f, 950f, t / duration);
                float sub = Mathf.Sin(2f * Mathf.PI * 75f * t) * 0.4f;
                float sweep = Mathf.Sin(2f * Mathf.PI * freq * t) * 0.5f;
                float noise = (UnityEngine.Random.value * 2f - 1f) * 0.25f;
                data[i] = (sweep + sub + noise) * env;
            }

            AudioClip clip = AudioClip.Create(name, samplesCount, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private AudioClip CreateBossRoarClip(string name, float duration)
        {
            int sampleRate = 44100;
            int samplesCount = (int)(sampleRate * duration);
            float[] data = new float[samplesCount];

            for (int i = 0; i < samplesCount; i++)
            {
                float t = (float)i / sampleRate;
                float env = Mathf.Sin(Mathf.Clamp01(t / duration) * Mathf.PI);
                // Heavy sub-bass earthquake rumble
                float rumble = Mathf.Sin(2f * Mathf.PI * (55f + Mathf.Sin(t * 18f) * 15f) * t) * 0.65f;
                float grit = (UnityEngine.Random.value * 2f - 1f) * 0.35f;
                data[i] = (rumble + grit) * env;
            }

            AudioClip clip = AudioClip.Create(name, samplesCount, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private AudioClip CreateTreasureChimeClip(string name, float duration)
        {
            int sampleRate = 44100;
            int samplesCount = (int)(sampleRate * duration);
            float[] data = new float[samplesCount];

            for (int i = 0; i < samplesCount; i++)
            {
                float t = (float)i / sampleRate;
                float env = Mathf.Exp(-t * 8f);
                // Pentatonic bell arpeggio sparkles
                float c6 = Mathf.Sin(2f * Mathf.PI * 1046.50f * t);
                float e6 = Mathf.Sin(2f * Mathf.PI * 1318.51f * t);
                float g6 = Mathf.Sin(2f * Mathf.PI * 1567.98f * t);
                float c7 = Mathf.Sin(2f * Mathf.PI * 2093.00f * t);
                data[i] = (c6 * 0.25f + e6 * 0.25f + g6 * 0.25f + c7 * 0.25f) * env;
            }

            AudioClip clip = AudioClip.Create(name, samplesCount, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }
        #endregion
    }
}
