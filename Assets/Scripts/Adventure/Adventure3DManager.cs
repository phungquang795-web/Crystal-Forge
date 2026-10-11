using System;
using System.Collections;
using UnityEngine;
using CrystalForge.Audio;
using CrystalForge.Core;
using CrystalForge.Data;
using CrystalForge.Economy;
using CrystalForge.UI;
using CrystalForge.VFX;

namespace CrystalForge.Adventure
{
    public class Adventure3DManager : MonoBehaviour
    {
        public static Adventure3DManager Instance { get; private set; }

        [Header("Single Player Model Reference")]
        [SerializeField] private CombatUnit3D heroUnit;
        [SerializeField] private Transform enemyContainer;

        [Header("Monster 3D Models")]
        [SerializeField] private CombatUnit3D slimePrefab;
        [SerializeField] private CombatUnit3D minionPrefab;
        [SerializeField] private CombatUnit3D golemPrefab;
        [SerializeField] private CombatUnit3D dragonBossPrefab;

        [Header("New Image-Based Monster Models")]
        [SerializeField] private CombatUnit3D crystalSpireGolemPrefab;
        [SerializeField] private CombatUnit3D prismBatPrefab;
        [SerializeField] private CombatUnit3D mossyGuardianPrefab;

        [Header("Key Forge & Combat Positions")]
        [SerializeField] private Vector3 idleForgePos = new Vector3(-1.3f, 0.4f, -0.6f);
        [SerializeField] private Quaternion idleForgeRot = Quaternion.Euler(0f, 55f, 0f);
        [SerializeField] private Vector3 heroCombatPos = new Vector3(-1.5f, 0.05f, -0.9f);
        [SerializeField] private Quaternion heroCombatRot = Quaternion.Euler(0f, 90f, 0f);
        [SerializeField] private Vector3 enemyCombatPos = new Vector3(1.5f, 0.05f, -0.9f);
        [SerializeField] private Quaternion enemyCombatRot = Quaternion.Euler(0f, -90f, 0f);
        [SerializeField] private Vector3 forgeCenter = new Vector3(0f, 0.05f, 0f);

        [Header("Camera Framing")]
        [SerializeField] private Vector3 normalCamPos = new Vector3(0f, 5.8f, -7.2f);
        [SerializeField] private Vector3 combatCamPos = new Vector3(0f, 4.4f, -5.8f);
        [SerializeField] private Quaternion normalCamRot = Quaternion.Euler(32f, 0f, 0f);
        [SerializeField] private Quaternion combatCamRot = Quaternion.Euler(26f, 0f, 0f);

        private Camera mainCamera;
        private Coroutine camTransitionCoroutine;
        private CombatUnit3D currentActiveEnemyUnit;
        private bool isInAdventure = false;
        private int lastSeenFloor = 1;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            mainCamera = Camera.main;
        }

        private void Start()
        {
            if (CrystalDefenseManager.Instance != null)
            {
                // Nhường toàn bộ quyền điều khiển phòng thủ và tuần tra cho CrystalDefenseManager
                enabled = false;
                return;
            }

            // Ban đầu: Đặt Hero tại đảo nổi và bắt đầu đi tuần tra quanh lò rèn
            if (heroUnit != null)
            {
                heroUnit.gameObject.SetActive(true);
                StartIdlePatrol();
            }

            ClickSystem.OnCoreClicked += HandleCoreClicked;
            SubscribeEvents();
        }

        private void OnDestroy()
        {
            if (heroUnit != null)
            {
                heroUnit.StopIdlePatrol();
            }
            ClickSystem.OnCoreClicked -= HandleCoreClicked;
            UnsubscribeEvents();
        }

        public void StartIdlePatrol()
        {
            if (heroUnit == null || isInAdventure) return;

            // Capybara di tuan quanh tru tinh the theo dung nghia bao ve / tuan tra (khong danh vao tru)
            heroUnit.StartIdlePatrol(forgeCenter, 2.15f, 0.75f);
        }

        private void HandleCoreClicked(Vector3 pos, double gain)
        {
            // Capybara chuyen tam di tuan canh gioi quanh tru tinh the, khong ngat quang de danh vao tru
        }

        private void SubscribeEvents()
        {
            if (AdventureManager.Instance == null) return;

            AdventureManager.Instance.OnStateChanged += HandleStateChanged;
            AdventureManager.Instance.OnEnemyEncountered += HandleEnemyEncountered;
            AdventureManager.Instance.OnHeroAttackTriggered += HandleHeroAttack;
            AdventureManager.Instance.OnHeroDamageDealt += HandleHeroDamageDealt;
            AdventureManager.Instance.OnEnemyAttackTriggered += HandleEnemyAttack;
            AdventureManager.Instance.OnEnemyDefeatedTriggered += HandleEnemyDefeated;
            AdventureManager.Instance.OnHeroDefeatedTriggered += HandleHeroDefeated;
            AdventureManager.Instance.OnFloorChanged += HandleFloorChanged;
        }

        private void UnsubscribeEvents()
        {
            if (AdventureManager.Instance == null) return;

            AdventureManager.Instance.OnStateChanged -= HandleStateChanged;
            AdventureManager.Instance.OnEnemyEncountered -= HandleEnemyEncountered;
            AdventureManager.Instance.OnHeroAttackTriggered -= HandleHeroAttack;
            AdventureManager.Instance.OnHeroDamageDealt -= HandleHeroDamageDealt;
            AdventureManager.Instance.OnEnemyAttackTriggered -= HandleEnemyAttack;
            AdventureManager.Instance.OnEnemyDefeatedTriggered -= HandleEnemyDefeated;
            AdventureManager.Instance.OnHeroDefeatedTriggered -= HandleHeroDefeated;
            AdventureManager.Instance.OnFloorChanged -= HandleFloorChanged;
        }

        private void HandleStateChanged(AdventureState newState)
        {
            if (newState == AdventureState.None)
            {
                // Rút lui hoặc kết thúc thám hiểm -> Chú Capybara quay trở lại đi tuần quanh lò rèn và tấn công
                isInAdventure = false;
                if (currentActiveEnemyUnit != null) currentActiveEnemyUnit.gameObject.SetActive(false);
                if (heroUnit != null)
                {
                    heroUnit.StopVictory();
                    heroUnit.PlayRun(heroUnit.transform.position, heroUnit.transform.position + (forgeCenter - heroUnit.transform.position).normalized * 0.3f, 0.35f, () =>
                    {
                        StartIdlePatrol();
                    });
                }
                TransitionCamera(false);
                return;
            }

            if (newState == AdventureState.AutoCombat)
            {
                if (!isInAdventure)
                {
                    isInAdventure = true;
                    TransitionCamera(true);

                    // Khi bắt đầu thám hiểm: Dừng đi tuần, Capybara chạy ra vị trí giao chiến trực tiếp với quái vật
                    if (heroUnit != null)
                    {
                        heroUnit.StopIdlePatrol();
                        heroUnit.StopVictory();
                        heroUnit.PlayRun(heroUnit.transform.position, heroCombatPos, 0.45f, () =>
                        {
                            heroUnit.SetBaseStance(heroCombatPos, heroCombatRot, heroUnit.transform.localScale);
                        });
                    }
                }
            }
        }

        private void HandleFloorChanged(int newFloor)
        {
            if (newFloor > lastSeenFloor && isInAdventure && heroUnit != null)
            {
                lastSeenFloor = newFloor;
                heroUnit.StopVictory();
                heroUnit.PlayVictory();
            }
            else
            {
                lastSeenFloor = newFloor;
            }
        }

        private void HandleEnemyEncountered(EnemyDataSO enemyData)
        {
            // Ẩn quái cũ nếu có
            if (currentActiveEnemyUnit != null)
            {
                currentActiveEnemyUnit.gameObject.SetActive(false);
            }

            // Chọn Model 3D quái vật theo tầng
            CombatUnit3D selectedPrefab = slimePrefab;
            string enemyId = enemyData != null ? enemyData.enemyId.ToLower() : "slime";

            if (enemyId.Contains("bat") || enemyId.Contains("prism"))
            {
                selectedPrefab = prismBatPrefab != null ? prismBatPrefab : slimePrefab;
            }
            else if (enemyId.Contains("spire") || enemyId.Contains("crystal_spire"))
            {
                selectedPrefab = crystalSpireGolemPrefab != null ? crystalSpireGolemPrefab : golemPrefab;
            }
            else if (enemyId.Contains("mossy") || enemyId.Contains("titan"))
            {
                selectedPrefab = mossyGuardianPrefab != null ? mossyGuardianPrefab : (dragonBossPrefab != null ? dragonBossPrefab : golemPrefab);
            }
            else if (enemyId.Contains("boss") || (enemyData != null && enemyData.isBoss))
            {
                selectedPrefab = mossyGuardianPrefab != null ? mossyGuardianPrefab : (dragonBossPrefab != null ? dragonBossPrefab : golemPrefab);
            }
            else if (enemyId.Contains("golem"))
            {
                selectedPrefab = crystalSpireGolemPrefab != null ? crystalSpireGolemPrefab : (golemPrefab != null ? golemPrefab : minionPrefab);
            }
            else if (enemyId.Contains("minion") || enemyId.Contains("shadow"))
            {
                selectedPrefab = prismBatPrefab != null ? prismBatPrefab : (minionPrefab != null ? minionPrefab : slimePrefab);
            }
            else
            {
                selectedPrefab = prismBatPrefab != null ? prismBatPrefab : slimePrefab;
            }

            if (selectedPrefab != null)
            {
                currentActiveEnemyUnit = selectedPrefab;
                currentActiveEnemyUnit.gameObject.SetActive(true);

                // Quái vật nhảy vào đảo từ bên phải lò rèn
                Vector3 spawnFromRight = enemyCombatPos + new Vector3(2.5f, 0f, 0f);
                currentActiveEnemyUnit.SetBaseStance(enemyCombatPos, enemyCombatRot, selectedPrefab.transform.localScale);
                currentActiveEnemyUnit.PlayRun(spawnFromRight, enemyCombatPos, 0.45f);
            }

            if (heroUnit != null)
            {
                heroUnit.StopVictory();
            }
        }

        private void HandleHeroAttack()
        {
            if (heroUnit == null || currentActiveEnemyUnit == null) return;

            Vector3 enemyTargetPos = currentActiveEnemyUnit.transform.position;

            // Hero Capybara phi thân tới trước mặt quái nện búa và nhảy lùi về
            heroUnit.PlayCapybaraAttack(enemyTargetPos, onImpact: () =>
            {
                if (currentActiveEnemyUnit != null && currentActiveEnemyUnit.gameObject.activeInHierarchy)
                {
                    currentActiveEnemyUnit.PlayTakeHit(heroUnit.transform.position, false);

                    if (VFXManager.Instance != null)
                    {
                        VFXManager.Instance.PlayClickSpark(currentActiveEnemyUnit.transform.position);
                    }

                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.PlayCombatAttackSfx();
                    }
                }
            });
        }

        private void HandleHeroDamageDealt(int damage, bool isCrit)
        {
            if (currentActiveEnemyUnit == null || !currentActiveEnemyUnit.gameObject.activeInHierarchy)
            {
                return;
            }

            // Hiện đúng tại thời điểm sát thương được áp dụng, phía trên đầu quái vật.
            FloatingTextSpawner.Instance?.SpawnDamageText(
                currentActiveEnemyUnit.transform.position + Vector3.up * 1.15f,
                damage,
                isCrit);
        }

        private void HandleEnemyAttack()
        {
            if (heroUnit == null || currentActiveEnemyUnit == null) return;

            Vector3 heroTargetPos = heroUnit.transform.position;

            // Quái vật lao tới cắn/tát Hero và nhảy lùi về
            currentActiveEnemyUnit.PlayCapybaraAttack(heroTargetPos, onImpact: () =>
            {
                if (heroUnit != null && heroUnit.gameObject.activeInHierarchy)
                {
                    heroUnit.PlayTakeHit(currentActiveEnemyUnit.transform.position, false);

                    if (VFXManager.Instance != null)
                    {
                        VFXManager.Instance.PlayClickSpark(heroUnit.transform.position);
                    }

                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.PlayCombatAttackSfx();
                    }
                }
            });
        }

        private void HandleEnemyDefeated()
        {
            if (currentActiveEnemyUnit != null)
            {
                currentActiveEnemyUnit.PlayDeath();
            }

            if (heroUnit != null)
            {
                heroUnit.PlayVictory();
            }

            if (VFXManager.Instance != null)
            {
                VFXManager.Instance.PlayMilestoneBurst();
            }
        }

        private void HandleHeroDefeated()
        {
            if (heroUnit != null)
            {
                heroUnit.PlayDeath();
            }
        }

        private void TransitionCamera(bool toCombat)
        {
            if (mainCamera == null) mainCamera = Camera.main;
            if (mainCamera == null) return;

            if (camTransitionCoroutine != null)
            {
                StopCoroutine(camTransitionCoroutine);
            }
            camTransitionCoroutine = StartCoroutine(CameraTransitionRoutine(toCombat));
        }

        private IEnumerator CameraTransitionRoutine(bool toCombat)
        {
            Vector3 startPos = mainCamera.transform.position;
            Quaternion startRot = mainCamera.transform.rotation;

            Vector3 endPos = toCombat ? combatCamPos : normalCamPos;
            Quaternion endRot = toCombat ? combatCamRot : normalCamRot;

            float elapsed = 0f;
            float duration = 0.45f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float smoothT = t * t * (3f - 2f * t);

                mainCamera.transform.position = Vector3.Lerp(startPos, endPos, smoothT);
                mainCamera.transform.rotation = Quaternion.Slerp(startRot, endRot, smoothT);
                yield return null;
            }

            mainCamera.transform.position = endPos;
            mainCamera.transform.rotation = endRot;
        }
    }
}
