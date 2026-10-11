using System;
using UnityEngine;
using CrystalForge.Core;
using CrystalForge.Economy;

namespace CrystalForge.VFX
{
    [DefaultExecutionOrder(-55)]
    public class VFXManager : MonoBehaviour
    {
        public static VFXManager Instance { get; private set; }

        [Header("Particle Systems")]
        [SerializeField] private ParticleSystem clickSparkParticles;
        [SerializeField] private ParticleSystem milestoneBurstParticles;
        [SerializeField] private ParticleSystem shockwaveParticles;

        private Material particleSharedMaterial;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            EnsureParticleMaterial();
            EnsureParticles();
        }

        private void Start()
        {
            SubscribeEvents();
        }

        private void OnDestroy()
        {
            UnsubscribeEvents();
        }

        private void SubscribeEvents()
        {
            ClickSystem.OnCoreClicked += HandleCoreClicked;

            if (MilestoneManager.Instance != null)
            {
                MilestoneManager.Instance.OnMilestoneAchieved += HandleMilestoneAchieved;
            }
        }

        private void UnsubscribeEvents()
        {
            ClickSystem.OnCoreClicked -= HandleCoreClicked;

            if (MilestoneManager.Instance != null)
            {
                MilestoneManager.Instance.OnMilestoneAchieved -= HandleMilestoneAchieved;
            }
        }

        private void HandleCoreClicked(Vector3 worldPos, double gain)
        {
            PlayClickSpark(worldPos);
        }

        private void HandleMilestoneAchieved(CrystalForge.Data.MilestoneDataSO milestone)
        {
            PlayMilestoneBurst();
            PlayShockwave(new Vector3(0f, 0.02f, 0f));
        }

        public void PlayClickSpark(Vector3 worldPos)
        {
            if (clickSparkParticles == null) return;

            Vector3 spawnPos = (worldPos != Vector3.zero) ? worldPos : new Vector3(0f, 1.8f, 0f);
            clickSparkParticles.transform.position = spawnPos;
            // Số tia lửa nhỏ nhẹ tinh tế (4-6 tia li ti, biến mất cực nhanh)
            clickSparkParticles.Emit(UnityEngine.Random.Range(4, 7));
        }

        public void PlayShockwave(Vector3 worldPos)
        {
            if (shockwaveParticles == null) return;

            shockwaveParticles.transform.position = worldPos;
            shockwaveParticles.Emit(1);
        }

        public void PlayMilestoneBurst()
        {
            if (milestoneBurstParticles == null) return;

            milestoneBurstParticles.transform.position = new Vector3(0f, 2.0f, 0f);
            milestoneBurstParticles.Emit(UnityEngine.Random.Range(35, 50));
        }

        private void EnsureParticleMaterial()
        {
            if (particleSharedMaterial == null)
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
                if (shader == null) shader = Shader.Find("Particles/Standard Unlit");
                if (shader == null) shader = Shader.Find("Sprites/Default");

                if (shader != null)
                {
                    particleSharedMaterial = new Material(shader);
                    particleSharedMaterial.name = "Mat_CleanParticle";
                }
            }
        }

        private void EnsureParticles()
        {
            // Dọn sạch các particle tím cũ nếu có
            var oldMotes = transform.Find("VFX_AmbientMotes");
            if (oldMotes != null) Destroy(oldMotes.gameObject);

            if (clickSparkParticles == null)
            {
                var sparkObj = new GameObject("VFX_ClickSparks");
                sparkObj.transform.parent = transform;
                // Tia lửa vàng kim rực rỡ nhỏ li ti (không tím, không ô vuông)
                clickSparkParticles = CreateSparkParticleSystem(sparkObj, new Color(1.0f, 0.95f, 0.35f, 1f), new Color(1.0f, 0.65f, 0.15f, 1f), 0.18f);
            }

            if (milestoneBurstParticles == null)
            {
                var burstObj = new GameObject("VFX_MilestoneBurst");
                burstObj.transform.parent = transform;
                milestoneBurstParticles = CreateCelebrationParticleSystem(burstObj, new Color(1.0f, 0.85f, 0.2f), new Color(0.2f, 0.95f, 1.0f), 0.85f);
            }

            if (shockwaveParticles == null)
            {
                var shockwaveObj = new GameObject("VFX_ShockwaveRing");
                shockwaveObj.transform.parent = transform;
                shockwaveParticles = CreateShockwaveParticleSystem(shockwaveObj);
            }
        }

        private ParticleSystem CreateSparkParticleSystem(GameObject go, Color colorA, Color colorB, float lifetime)
        {
            var ps = go.AddComponent<ParticleSystem>();
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            var main = ps.main;
            main.playOnAwake = false;
            main.loop = false;
            main.duration = 1.0f;
            main.startLifetime = lifetime;
            main.startSpeed = new ParticleSystem.MinMaxCurve(1.5f, 3.5f);
            main.startSize = new ParticleSystem.MinMaxCurve(0.03f, 0.06f); // Hạt nhỏ li ti tinh tế
            main.startColor = new ParticleSystem.MinMaxGradient(colorA, colorB);
            main.gravityModifier = 1.2f;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.enabled = false;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.15f;

            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient grad = new Gradient();
            grad.SetKeys(
                new GradientColorKey[] { new GradientColorKey(colorA, 0.0f), new GradientColorKey(colorB, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
            );
            colorOverLifetime.color = grad;

            var renderer = ps.GetComponent<ParticleSystemRenderer>();
            renderer.renderMode = ParticleSystemRenderMode.Billboard;
            if (particleSharedMaterial != null)
            {
                renderer.sharedMaterial = particleSharedMaterial;
            }

            return ps;
        }

        private ParticleSystem CreateShockwaveParticleSystem(GameObject go)
        {
            var ps = go.AddComponent<ParticleSystem>();
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            var main = ps.main;
            main.playOnAwake = false;
            main.loop = false;
            main.duration = 0.4f;
            main.startLifetime = 0.28f;
            main.startSpeed = 0f;
            main.startSize = 0.35f;
            main.startColor = new Color(0.2f, 0.9f, 1f, 0.75f); // Màu Cyan ngọc ma thuật
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.enabled = false;

            var sizeOverLifetime = ps.sizeOverLifetime;
            sizeOverLifetime.enabled = true;
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(0f, 0.5f);
            curve.AddKey(1f, 4.0f);
            sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, curve);

            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient grad = new Gradient();
            grad.SetKeys(
                new GradientColorKey[] { new GradientColorKey(new Color(0.2f, 0.9f, 1f), 0f), new GradientColorKey(new Color(1f, 0.85f, 0.3f), 1f) },
                new GradientAlphaKey[] { new GradientAlphaKey(0.7f, 0f), new GradientAlphaKey(0f, 1f) }
            );
            colorOverLifetime.color = grad;

            var renderer = ps.GetComponent<ParticleSystemRenderer>();
            renderer.renderMode = ParticleSystemRenderMode.HorizontalBillboard;
            if (particleSharedMaterial != null)
            {
                renderer.sharedMaterial = particleSharedMaterial;
            }

            return ps;
        }

        private ParticleSystem CreateCelebrationParticleSystem(GameObject go, Color colorA, Color colorB, float lifetime)
        {
            var ps = go.AddComponent<ParticleSystem>();
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            var main = ps.main;
            main.playOnAwake = false;
            main.loop = false;
            main.duration = 1.0f;
            main.startLifetime = lifetime;
            main.startSpeed = new ParticleSystem.MinMaxCurve(2.5f, 5.0f);
            main.startSize = new ParticleSystem.MinMaxCurve(0.04f, 0.12f);
            main.startColor = new ParticleSystem.MinMaxGradient(colorA, colorB);
            main.gravityModifier = 0.8f;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.enabled = false;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 45f;
            shape.radius = 0.35f;

            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient grad = new Gradient();
            grad.SetKeys(
                new GradientColorKey[] { new GradientColorKey(colorA, 0.0f), new GradientColorKey(colorB, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
            );
            colorOverLifetime.color = grad;

            var renderer = ps.GetComponent<ParticleSystemRenderer>();
            renderer.renderMode = ParticleSystemRenderMode.Billboard;
            if (particleSharedMaterial != null)
            {
                renderer.sharedMaterial = particleSharedMaterial;
            }

            return ps;
        }
    }
}
