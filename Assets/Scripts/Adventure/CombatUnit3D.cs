using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrystalForge.Adventure
{
    public class CombatUnit3D : MonoBehaviour
    {
        [Header("Unit Identity")]
        [SerializeField] private bool isHero = false;
        [SerializeField] private Transform weaponTransform;
        [SerializeField] private Renderer[] bodyRenderers;

        private Vector3 baseWorldPos;
        private Quaternion baseWorldRot;
        private Vector3 baseScale;

        private bool isBusy = false;
        private bool isDead = false;
        private bool isVictory = false;
        private bool isRunningForward = false;
        private bool isPatrolling = false;
        private Coroutine idlePatrolCoroutine;
        private Coroutine currentStrikeCoroutine;
        private int strikeComboStep = 0;

        private readonly List<Color> originalColors = new List<Color>();
        private readonly List<Material> cachedMaterials = new List<Material>();

        public bool IsHero => isHero;
        public Vector3 BaseWorldPos => baseWorldPos;
        public bool IsBusy => isBusy;
        public bool IsPatrolling => isPatrolling;

        private void Awake()
        {
            baseWorldPos = transform.position;
            baseWorldRot = transform.rotation;
            baseScale = transform.localScale;

            CacheRenderers();
        }

        public void SetBaseStance(Vector3 worldPos, Quaternion worldRot, Vector3 scale)
        {
            StopIdlePatrol();

            baseWorldPos = worldPos;
            baseWorldRot = worldRot;
            baseScale = scale;

            transform.position = worldPos;
            transform.rotation = worldRot;
            transform.localScale = scale;

            isBusy = false;
            isDead = false;
            isVictory = false;
            isRunningForward = false;

            if (weaponTransform != null)
            {
                weaponTransform.localRotation = Quaternion.identity;
            }
        }

        public void CacheRenderers()
        {
            cachedMaterials.Clear();
            originalColors.Clear();

            if (bodyRenderers == null || bodyRenderers.Length == 0)
            {
                bodyRenderers = GetComponentsInChildren<Renderer>();
            }

            foreach (var r in bodyRenderers)
            {
                if (r != null)
                {
                    Material mat = Application.isPlaying ? r.material : r.sharedMaterial;
                    if (mat != null)
                    {
                        cachedMaterials.Add(mat);
                        originalColors.Add(mat.HasProperty("_BaseColor") ? mat.GetColor("_BaseColor") : Color.white);
                    }
                }
            }
        }

        private void Update()
        {
            if (isDead) return;

            if (isVictory)
            {
                // Nhảy múa ăn mừng chiến thắng vui nhộn kiểu Capybara Go (nhảy nảy, lắc hông và vung vũ khí)
                float hop = Mathf.Abs(Mathf.Sin(Time.time * 8.5f)) * 0.35f;
                float danceYaw = Mathf.Sin(Time.time * 4.5f) * 22f;
                float danceRoll = Mathf.Sin(Time.time * 9f) * 8f;
                transform.position = baseWorldPos + Vector3.up * hop;
                transform.rotation = baseWorldRot * Quaternion.Euler(0f, danceYaw, danceRoll);
                if (weaponTransform != null)
                {
                    weaponTransform.localRotation = Quaternion.Euler(-55f + Mathf.Sin(Time.time * 12f) * 28f, 0f, danceRoll * 1.5f);
                }
                return;
            }

            if (!isBusy && !isRunningForward && !isPatrolling)
            {
                // Nhịp thở Idle phập phồng đáng yêu (Squash & Stretch)
                float bobFreq = isHero ? 3.5f : 4f;
                float bobAmp = isHero ? 0.04f : 0.06f;
                float bob = Mathf.Sin(Time.time * bobFreq) * bobAmp;
                transform.position = baseWorldPos + Vector3.up * bob;

                float squash = 1f + (Mathf.Sin(Time.time * bobFreq) * 0.04f);
                transform.localScale = new Vector3(baseScale.x * (2f - squash), baseScale.y * squash, baseScale.z * (2f - squash));
            }
        }

        /// <summary>
        /// Gõ búa khi ở Lò Rèn (Idle Forge Mode)
        /// </summary>
        public void PlayForgeHammerSwing()
        {
            if (isBusy || isDead) return;
            StartCoroutine(ForgeHammerSwingRoutine());
        }

        private IEnumerator ForgeHammerSwingRoutine()
        {
            if (weaponTransform == null) yield break;

            float elapsed = 0f;
            float duration = 0.18f;

            // Nâng búa lên rồi đập xuống
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                float angle = (t < 0.4f) ? Mathf.Lerp(0f, -60f, t / 0.4f) : Mathf.Lerp(-60f, 25f, (t - 0.4f) / 0.6f);
                weaponTransform.localRotation = Quaternion.Euler(angle, 0f, 0f);
                yield return null;
            }

            // Hồi phục
            elapsed = 0f;
            while (elapsed < 0.12f)
            {
                elapsed += Time.deltaTime;
                weaponTransform.localRotation = Quaternion.Lerp(Quaternion.Euler(25f, 0f, 0f), Quaternion.identity, elapsed / 0.12f);
                yield return null;
            }

            weaponTransform.localRotation = Quaternion.identity;
        }

        /// <summary>
        /// Bắt đầu chế độ đi tuần quanh trụ tinh thể khi Idle / AFK (tuần tra liên tục, quan sát cảnh giới, không đánh trụ tinh thể)
        /// </summary>
        public void StartIdlePatrol(Vector3 center, float radius, float speed, Action<Vector3> onAttackStrike = null)
        {
            StopIdlePatrol();
            idlePatrolCoroutine = StartCoroutine(IdlePatrolRoutine(center, radius, speed));
        }

        /// <summary>
        /// Dừng chế độ đi tuần lò rèn
        /// </summary>
        public void StopIdlePatrol()
        {
            if (idlePatrolCoroutine != null)
            {
                StopCoroutine(idlePatrolCoroutine);
                idlePatrolCoroutine = null;
            }
            isPatrolling = false;
            isRunningForward = false;
            if (weaponTransform != null) weaponTransform.localRotation = Quaternion.identity;
        }

        private IEnumerator IdlePatrolRoutine(Vector3 center, float radius, float speed)
        {
            isPatrolling = true;
            isBusy = false;
            isRunningForward = true;

            float currentAngle = Mathf.Atan2(transform.position.z - center.z, transform.position.x - center.x);
            float stepFreq = 4.8f;
            float scanTimer = 0f;
            float scanInterval = 8.5f; // Cứ sau khoảng 1 vòng tuần tra (~8.5s), dừng chân quan sát cảnh giới 1.2s

            while (isPatrolling)
            {
                // 1. Đi tuần tra vòng quanh trụ tinh thể theo quỹ đạo tròn mượt mà
                currentAngle += speed * Time.deltaTime;
                if (currentAngle > Mathf.PI * 2f) currentAngle -= Mathf.PI * 2f;

                // Nhịp chân đi tuần nảy nhẹ và lắc lư lạch bạch (waddle) đáng yêu
                float hop = Mathf.Abs(Mathf.Sin(Time.time * stepFreq)) * 0.08f;
                float waddleRoll = Mathf.Sin(Time.time * stepFreq * 0.5f) * 4.5f;

                float x = center.x + Mathf.Cos(currentAngle) * radius;
                float z = center.z + Mathf.Sin(currentAngle) * radius;

                transform.position = new Vector3(x, center.y + hop, z);

                // Hướng mặt chính xác theo tiếp tuyến đường tròn tiến bước tới trước
                Vector3 tangent = new Vector3(-Mathf.Sin(currentAngle), 0f, Mathf.Cos(currentAngle));
                if (tangent != Vector3.zero)
                {
                    Quaternion targetRot = Quaternion.LookRotation(tangent, Vector3.up);
                    transform.rotation = targetRot * Quaternion.Euler(0f, 0f, waddleRoll);
                }

                // Tư thế vác búa hộ vệ: vác chéo vai nảy nhịp nhàng theo sải bước
                if (weaponTransform != null)
                {
                    float weaponSway = Mathf.Sin(Time.time * stepFreq) * 6f;
                    weaponTransform.localRotation = Quaternion.Euler(-26f + weaponSway, 0f, 0f);
                }

                scanTimer += Time.deltaTime;

                // 2. Định kỳ dừng chân quan sát cảnh giới xung quanh đảo bay (không đánh trụ)
                if (scanTimer >= scanInterval)
                {
                    scanTimer = 0f;
                    isRunningForward = false;

                    // Đứng thẳng trang nghiêm trên mặt đất
                    transform.position = new Vector3(x, center.y, z);

                    Vector3 tangentBase = new Vector3(-Mathf.Sin(currentAngle), 0f, Mathf.Cos(currentAngle));
                    Quaternion basePatrolRot = (tangentBase != Vector3.zero) ? Quaternion.LookRotation(tangentBase, Vector3.up) : transform.rotation;

                    if (weaponTransform != null)
                    {
                        weaponTransform.localRotation = Quaternion.Euler(-12f, 0f, 0f);
                    }

                    // Quan sát cảnh giới sang trái (hướng ra ngoài không gian)
                    float lookElapsed = 0f;
                    Quaternion lookLeftRot = basePatrolRot * Quaternion.Euler(0f, -32f, 0f);
                    Quaternion lookRightRot = basePatrolRot * Quaternion.Euler(0f, 32f, 0f);

                    while (lookElapsed < 0.45f && isPatrolling)
                    {
                        lookElapsed += Time.deltaTime;
                        transform.rotation = Quaternion.Slerp(basePatrolRot, lookLeftRot, lookElapsed / 0.45f);
                        yield return null;
                    }

                    // Quét mắt sang phải quan sát hướng đối diện
                    lookElapsed = 0f;
                    while (lookElapsed < 0.5f && isPatrolling)
                    {
                        lookElapsed += Time.deltaTime;
                        transform.rotation = Quaternion.Slerp(lookLeftRot, lookRightRot, lookElapsed / 0.5f);
                        yield return null;
                    }

                    // Trả tầm nhìn về hướng tiến bước tiếp tục tuần tra
                    lookElapsed = 0f;
                    while (lookElapsed < 0.3f && isPatrolling)
                    {
                        lookElapsed += Time.deltaTime;
                        transform.rotation = Quaternion.Slerp(lookRightRot, basePatrolRot, lookElapsed / 0.3f);
                        yield return null;
                    }

                    isRunningForward = true;
                }

                yield return null;
            }

            isRunningForward = false;
        }

        /// <summary>
        /// Khi người chơi click chủ động, Capybara lập tức phóng tới tung đòn tấn công uy lực (hỗ trợ đòn thường & chí mạng)
        /// </summary>
        public void PlayImmediateStrikeTowards(Vector3 targetPos, bool isCrit = false)
        {
            if (isDead) return;
            if (currentStrikeCoroutine != null)
            {
                StopCoroutine(currentStrikeCoroutine);
            }
            currentStrikeCoroutine = StartCoroutine(ImmediateStrikeRoutine(targetPos, isCrit));
        }

        private IEnumerator ImmediateStrikeRoutine(Vector3 targetPos, bool isCrit)
        {
            strikeComboStep++;
            bool strikeLeft = (strikeComboStep % 2 == 1);
            float strikeTilt = strikeLeft ? -15f : 15f;

            Vector3 toTarget = (targetPos - baseWorldPos);
            toTarget.y = 0f;
            Quaternion targetFaceRot = (toTarget != Vector3.zero)
                ? Quaternion.LookRotation(toTarget.normalized, Vector3.up)
                : transform.rotation;

            Vector3 lungeDir = (toTarget != Vector3.zero) ? toTarget.normalized : transform.forward;
            float lungeDist = isCrit ? 0.42f : 0.25f;
            Vector3 strikePos = baseWorldPos + lungeDir * lungeDist;
            float strikeHeight = isCrit ? 0.38f : 0.12f;

            float duration = isCrit ? 0.22f : 0.14f;
            float elapsed = 0f;

            // 1. Phóng vút tới mục tiêu & Vung đòn uy lực (Lunge & Impact)
            float halfDuration = duration * 0.45f;
            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / halfDuration);

                // Lướt tới phía trước và nhấc bổng nhẹ
                float arc = Mathf.Sin(t * Mathf.PI * 0.5f) * strikeHeight;
                transform.position = Vector3.Lerp(baseWorldPos, strikePos, t) + Vector3.up * arc;

                // Độ nghiêng vung đòn luân phiên trái / phải (Combo feel)
                transform.rotation = targetFaceRot * Quaternion.Euler(isCrit ? -14f : 6f, 0f, Mathf.Lerp(0f, strikeTilt, t));

                // Squash & stretch tụ lực rồi phóng tới
                float squashY = Mathf.Lerp(1.15f, 0.88f, t);
                float squashXZ = 2f - squashY;
                transform.localScale = new Vector3(baseScale.x * squashXZ, baseScale.y * squashY, baseScale.z * squashXZ);

                if (weaponTransform != null)
                {
                    float weaponAngle = Mathf.Lerp(0f, -85f, t);
                    weaponTransform.localRotation = Quaternion.Euler(weaponAngle, strikeTilt * 1.5f, 0f);
                }

                yield return null;
            }

            // Đòn giáng chấn động (Impact Frame)
            if (weaponTransform != null)
            {
                weaponTransform.localRotation = Quaternion.Euler(32f, strikeTilt * 0.5f, 0f);
            }

            // 2. Nảy đàn hồi hồi phục về vị trí đứng ban đầu (Snappy Recovery)
            elapsed = 0f;
            float recoverDuration = duration - halfDuration;
            while (elapsed < recoverDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / recoverDuration);

                transform.position = Vector3.Lerp(strikePos, baseWorldPos, t);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetFaceRot, t);
                transform.localScale = Vector3.Lerp(transform.localScale, baseScale, t);

                if (weaponTransform != null)
                {
                    weaponTransform.localRotation = Quaternion.Lerp(Quaternion.Euler(32f, strikeTilt * 0.5f, 0f), Quaternion.identity, t);
                }

                yield return null;
            }

            transform.position = baseWorldPos;
            transform.rotation = targetFaceRot;
            transform.localScale = baseScale;
            if (weaponTransform != null) weaponTransform.localRotation = Quaternion.identity;
            currentStrikeCoroutine = null;
        }

        /// <summary>
        /// Hoạt họa Dũng Sĩ Tụ Lực Gầm Thét & Bùng Nổ Cuồng Nộ (Runic Overdrive Roar)
        /// </summary>
        public void PlayOverdriveRoar(Action onComplete = null)
        {
            if (isDead) return;
            StartCoroutine(OverdriveRoarRoutine(onComplete));
        }

        private IEnumerator OverdriveRoarRoutine(Action onComplete)
        {
            isBusy = true;
            Vector3 startPos = baseWorldPos;
            Vector3 startScale = baseScale;
            Quaternion startRot = transform.rotation;

            // 1. Nhún sâu xuống đất tụ lực (0.12s)
            float elapsed = 0f;
            while (elapsed < 0.12f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / 0.12f;
                transform.position = startPos - Vector3.up * (0.14f * t);
                transform.localScale = new Vector3(startScale.x * 1.25f, startScale.y * 0.72f, startScale.z * 1.25f);
                if (weaponTransform != null)
                {
                    weaponTransform.localRotation = Quaternion.Euler(Mathf.Lerp(0f, 30f, t), 0f, 0f);
                }
                yield return null;
            }

            // 2. Bật nhảy vọt lên không trung, giương búa lên trời gầm thét (0.28s)
            elapsed = 0f;
            while (elapsed < 0.28f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / 0.28f;
                float height = Mathf.Sin(t * Mathf.PI) * 0.65f;
                transform.position = startPos + Vector3.up * height;
                transform.rotation = startRot * Quaternion.Euler(-22f * Mathf.Sin(t * Mathf.PI), 0f, 0f);
                transform.localScale = new Vector3(startScale.x * 0.88f, startScale.y * 1.22f, startScale.z * 0.88f);
                if (weaponTransform != null)
                {
                    weaponTransform.localRotation = Quaternion.Euler(Mathf.Lerp(30f, -115f, t), 0f, 0f);
                }
                yield return null;
            }

            // 3. Đập sầm xuống đất tạo sóng chấn động & phục hồi tư thế (0.18s)
            elapsed = 0f;
            while (elapsed < 0.18f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / 0.18f;
                transform.position = startPos;
                transform.rotation = Quaternion.Slerp(transform.rotation, startRot, t);
                transform.localScale = Vector3.Lerp(new Vector3(startScale.x * 1.28f, startScale.y * 0.75f, startScale.z * 1.28f), startScale, t);
                if (weaponTransform != null)
                {
                    weaponTransform.localRotation = Quaternion.Lerp(Quaternion.Euler(-115f, 0f, 0f), Quaternion.identity, t);
                }
                yield return null;
            }

            transform.position = startPos;
            transform.rotation = startRot;
            transform.localScale = startScale;
            if (weaponTransform != null) weaponTransform.localRotation = Quaternion.identity;
            isBusy = false;
            onComplete?.Invoke();
        }

        /// <summary>
        /// Hoạt họa Xoay Lốc Xoáy 720 độ (Whirlwind Slash)
        /// </summary>
        public void PlayWhirlwindSpin(Action onComplete = null)
        {
            if (isDead) return;
            StartCoroutine(WhirlwindSpinRoutine(onComplete));
        }

        private IEnumerator WhirlwindSpinRoutine(Action onComplete)
        {
            isBusy = true;
            Quaternion startRot = transform.rotation;
            Vector3 startPos = baseWorldPos;
            float duration = 0.32f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                // Xoay 2 vòng 720 độ cực bốc
                float currentYaw = t * 720f;
                float hop = Mathf.Sin(t * Mathf.PI) * 0.28f;
                transform.position = startPos + Vector3.up * hop;
                transform.rotation = startRot * Quaternion.Euler(0f, currentYaw, -10f * Mathf.Sin(t * Mathf.PI));

                if (weaponTransform != null)
                {
                    weaponTransform.localRotation = Quaternion.Euler(-35f, 90f, 0f);
                }

                yield return null;
            }

            transform.position = startPos;
            transform.rotation = startRot;
            if (weaponTransform != null) weaponTransform.localRotation = Quaternion.identity;
            isBusy = false;
            onComplete?.Invoke();
        }

        /// <summary>
        /// Hoạt họa Khai Thác Tinh Thể (Gõ búa/cuốc vào Trụ Pha Lê)
        /// </summary>
        public void PlayMineCrystalStrike(Vector3 crystalCenterPos)
        {
            if (isDead || isBusy) return;
            StartCoroutine(MineCrystalStrikeRoutine(crystalCenterPos));
        }

        private IEnumerator MineCrystalStrikeRoutine(Vector3 crystalCenterPos)
        {
            Vector3 toCrystal = (crystalCenterPos - baseWorldPos);
            toCrystal.y = 0f;
            Quaternion faceCrystalRot = (toCrystal != Vector3.zero)
                ? Quaternion.LookRotation(toCrystal.normalized, Vector3.up)
                : transform.rotation;

            transform.rotation = faceCrystalRot;

            float elapsed = 0f;
            float duration = 0.16f;

            // Nâng cao rồi gõ dứt khoát
            while (elapsed < duration * 0.45f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (duration * 0.45f);
                transform.position = baseWorldPos + Vector3.up * (0.08f * t);
                transform.localScale = new Vector3(baseScale.x * 0.95f, baseScale.y * 1.1f, baseScale.z * 0.95f);
                if (weaponTransform != null)
                {
                    weaponTransform.localRotation = Quaternion.Euler(Mathf.Lerp(0f, -80f, t), 0f, 0f);
                }
                yield return null;
            }

            elapsed = 0f;
            while (elapsed < duration * 0.55f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (duration * 0.55f);
                transform.position = Vector3.Lerp(baseWorldPos + Vector3.up * 0.08f, baseWorldPos, t);
                transform.localScale = Vector3.Lerp(new Vector3(baseScale.x * 1.12f, baseScale.y * 0.88f, baseScale.z * 1.12f), baseScale, t);
                if (weaponTransform != null)
                {
                    weaponTransform.localRotation = Quaternion.Lerp(Quaternion.Euler(28f, 0f, 0f), Quaternion.identity, t);
                }
                yield return null;
            }

            transform.position = baseWorldPos;
            transform.localScale = baseScale;
            if (weaponTransform != null) weaponTransform.localRotation = Quaternion.identity;
        }

        /// <summary>
        /// Di chuyển vòng cung xung quanh lò rèn từ vị trí bắt đầu tới vị trí đích
        /// </summary>
        public void PlayMoveAroundForge(Vector3 center, Vector3 startPos, Vector3 targetPos, Quaternion targetRot, float duration, Action onComplete = null)
        {
            StartCoroutine(MoveAroundForgeRoutine(center, startPos, targetPos, targetRot, duration, onComplete));
        }

        private IEnumerator MoveAroundForgeRoutine(Vector3 center, Vector3 startPos, Vector3 targetPos, Quaternion targetRot, float duration, Action onComplete)
        {
            isBusy = true;
            isRunningForward = true;
            float elapsed = 0f;

            float startAngle = Mathf.Atan2(startPos.z - center.z, startPos.x - center.x);
            float endAngle = Mathf.Atan2(targetPos.z - center.z, targetPos.x - center.x);
            float startRadius = new Vector2(startPos.x - center.x, startPos.z - center.z).magnitude;
            float endRadius = new Vector2(targetPos.x - center.x, targetPos.z - center.z).magnitude;

            // Tính góc quét quanh lò rèn
            float diff = Mathf.DeltaAngle(startAngle * Mathf.Rad2Deg, endAngle * Mathf.Rad2Deg) * Mathf.Deg2Rad;
            if (Mathf.Abs(diff) < 0.4f) diff -= Mathf.PI * 2f; // Chạy 1 vòng cung rõ nét

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                float currentAngle = startAngle + diff * t;
                float currentRadius = Mathf.Lerp(startRadius, endRadius, t);
                float currentY = Mathf.Lerp(startPos.y, targetPos.y, t);

                float hop = Mathf.Abs(Mathf.Sin(t * Mathf.PI * 6f)) * 0.22f;
                float x = center.x + Mathf.Cos(currentAngle) * currentRadius;
                float z = center.z + Mathf.Sin(currentAngle) * currentRadius;

                transform.position = new Vector3(x, currentY + hop, z);

                float dirSign = Mathf.Sign(diff);
                Vector3 tangent = new Vector3(-Mathf.Sin(currentAngle) * dirSign, 0f, Mathf.Cos(currentAngle) * dirSign);
                if (tangent != Vector3.zero)
                {
                    Quaternion moveRot = Quaternion.LookRotation(tangent, Vector3.up);
                    transform.rotation = Quaternion.Slerp(moveRot, targetRot, t * t);
                }

                if (weaponTransform != null)
                {
                    weaponTransform.localRotation = Quaternion.Euler(-25f * Mathf.Sin(t * Mathf.PI * 6f), 0f, 0f);
                }

                yield return null;
            }

            transform.position = targetPos;
            transform.rotation = targetRot;
            baseWorldPos = targetPos;
            baseWorldRot = targetRot;
            isBusy = false;
            isRunningForward = false;
            if (weaponTransform != null) weaponTransform.localRotation = Quaternion.identity;
            onComplete?.Invoke();
        }

        /// <summary>
        /// Chạy 1 vòng quanh lò rèn kiểu Capybara Go khi di chuyển hoặc chuyển tầng
        /// </summary>
        public void PlayCircleAroundForge(Vector3 center, float radius, float duration, Action onComplete = null)
        {
            StartCoroutine(CircleAroundForgeRoutine(center, radius, duration, onComplete));
        }

        private IEnumerator CircleAroundForgeRoutine(Vector3 center, float radius, float duration, Action onComplete)
        {
            isBusy = true;
            isRunningForward = true;
            float elapsed = 0f;
            float startAngle = Mathf.Atan2(transform.position.z - center.z, transform.position.x - center.x);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                // Chạy vòng cung quanh tâm lò rèn
                float angle = startAngle + t * Mathf.PI * 2f;

                float x = center.x + Mathf.Cos(angle) * radius;
                float z = center.z + Mathf.Sin(angle) * radius;
                float hop = Mathf.Abs(Mathf.Sin(t * Mathf.PI * 8f)) * 0.22f;

                transform.position = new Vector3(x, baseWorldPos.y + hop, z);

                Vector3 tangent = new Vector3(-Mathf.Sin(angle), 0f, Mathf.Cos(angle));
                if (tangent != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(tangent, Vector3.up);
                }

                if (weaponTransform != null)
                {
                    weaponTransform.localRotation = Quaternion.Euler(-25f * Mathf.Sin(t * Mathf.PI * 8f), 0f, 0f);
                }

                yield return null;
            }

            transform.position = baseWorldPos;
            transform.rotation = baseWorldRot;
            isBusy = false;
            isRunningForward = false;
            if (weaponTransform != null) weaponTransform.localRotation = Quaternion.identity;
            onComplete?.Invoke();
        }

        /// <summary>
        /// Hoạt họa di chuyển nảy bước tới vị trí mục tiêu
        /// </summary>
        public void PlayRun(Vector3 startWorldPos, Vector3 targetWorldPos, float duration, Action onArrival = null)
        {
            StartCoroutine(RunRoutine(startWorldPos, targetWorldPos, duration, onArrival));
        }

        private IEnumerator RunRoutine(Vector3 startWorldPos, Vector3 targetWorldPos, float duration, Action onArrival)
        {
            isBusy = true;
            float elapsed = 0f;
            transform.position = startWorldPos;

            Vector3 moveDir = (targetWorldPos - startWorldPos).normalized;
            float forwardTilt = isHero ? -10f : 10f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                float stepBounce = Mathf.Abs(Mathf.Sin(t * Mathf.PI * 4f)) * 0.22f;
                Vector3 current = Vector3.Lerp(startWorldPos, targetWorldPos, t);
                current.y += stepBounce;

                transform.position = current;

                if (moveDir != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(moveDir, Vector3.up) * Quaternion.Euler(0f, 0f, forwardTilt * Mathf.Sin(t * Mathf.PI * 4f));
                }

                if (weaponTransform != null)
                {
                    weaponTransform.localRotation = Quaternion.Euler(-25f * Mathf.Sin(t * Mathf.PI * 4f), 0f, 0f);
                }

                yield return null;
            }

            transform.position = targetWorldPos;
            transform.rotation = baseWorldRot;
            transform.localScale = baseScale;
            baseWorldPos = targetWorldPos;
            isBusy = false;

            if (weaponTransform != null) weaponTransform.localRotation = Quaternion.identity;
            onArrival?.Invoke();
        }

        /// <summary>
        /// Tấn công lần lượt kiểu Capybara Go
        /// </summary>
        public void PlayCapybaraAttack(Vector3 targetWorldPos, Action onImpact = null, Action onComplete = null)
        {
            StartCoroutine(CapybaraAttackRoutine(targetWorldPos, onImpact, onComplete));
        }

        private IEnumerator CapybaraAttackRoutine(Vector3 targetWorldPos, Action onImpact, Action onComplete)
        {
            isBusy = true;
            Vector3 startPos = baseWorldPos;
            Quaternion startRot = baseWorldRot;

            Vector3 toTarget = (targetWorldPos - startPos);
            toTarget.y = 0f;
            Vector3 attackMeleeSpot = targetWorldPos - toTarget.normalized * 0.75f;
            attackMeleeSpot.y = startPos.y;

            // 1. Nhún người kéo lùi lấy đà (0.07s)
            Vector3 prepPos = startPos - toTarget.normalized * 0.25f;
            float elapsed = 0f;
            while (elapsed < 0.07f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / 0.07f;
                transform.position = Vector3.Lerp(startPos, prepPos, t);
                transform.localScale = new Vector3(baseScale.x * 1.15f, baseScale.y * 0.85f, baseScale.z * 1.15f);
                yield return null;
            }

            // 2. Phóng vút lao tới trước mặt đối thủ (0.16s)
            elapsed = 0f;
            while (elapsed < 0.16f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / 0.16f;

                float arc = Mathf.Sin(t * Mathf.PI) * 0.4f;
                Vector3 current = Vector3.Lerp(prepPos, attackMeleeSpot, t);
                current.y += arc;
                transform.position = current;

                if (weaponTransform != null)
                {
                    weaponTransform.localRotation = Quaternion.Euler(Mathf.Lerp(0f, -95f, t), 0f, 0f);
                }

                transform.localScale = Vector3.Lerp(new Vector3(baseScale.x * 1.15f, baseScale.y * 0.85f, baseScale.z * 1.15f), baseScale, t);
                yield return null;
            }

            // 3. Nện búa dập xuống (Impact Frame)
            transform.position = attackMeleeSpot;
            if (weaponTransform != null)
            {
                weaponTransform.localRotation = Quaternion.Euler(30f, 0f, 0f);
            }

            onImpact?.Invoke();
            yield return new WaitForSeconds(0.06f);

            // 4. Nhảy giật lùi về thế thủ ban đầu (0.24s)
            elapsed = 0f;
            while (elapsed < 0.24f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / 0.24f;

                float backHop = Mathf.Abs(Mathf.Sin(t * Mathf.PI * 2f)) * 0.2f;
                Vector3 current = Vector3.Lerp(attackMeleeSpot, startPos, t);
                current.y += backHop;
                transform.position = current;

                if (weaponTransform != null)
                {
                    weaponTransform.localRotation = Quaternion.Lerp(Quaternion.Euler(30f, 0f, 0f), Quaternion.identity, t);
                }
                yield return null;
            }

            transform.position = startPos;
            transform.rotation = startRot;
            transform.localScale = baseScale;
            if (weaponTransform != null) weaponTransform.localRotation = Quaternion.identity;

            isBusy = false;
            onComplete?.Invoke();
        }

        public void PlayTakeHit(Vector3 attackerWorldPos, bool isCrit = false)
        {
            StartCoroutine(TakeHitRoutine(attackerWorldPos, isCrit));
        }

        private IEnumerator TakeHitRoutine(Vector3 attackerWorldPos, bool isCrit)
        {
            Vector3 pushDir = (baseWorldPos - attackerWorldPos);
            pushDir.y = 0f;
            pushDir = pushDir.normalized;

            float pushDist = isCrit ? 0.55f : 0.35f;
            Vector3 pushedPos = baseWorldPos + pushDir * pushDist;

            Color flashColor = isCrit ? new Color(1f, 0.9f, 0.2f) : new Color(1f, 0.2f, 0.2f);
            SetRendererColor(flashColor);

            float elapsed = 0f;
            while (elapsed < 0.08f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / 0.08f;
                transform.position = Vector3.Lerp(baseWorldPos, pushedPos, t);
                transform.localScale = new Vector3(baseScale.x * 1.15f, baseScale.y * 0.85f, baseScale.z * 1.15f);
                yield return null;
            }

            RestoreRendererColors();

            elapsed = 0f;
            while (elapsed < 0.2f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / 0.2f;
                float shake = Mathf.Sin(t * Mathf.PI * 4f) * (1f - t) * 0.12f;
                transform.position = Vector3.Lerp(pushedPos, baseWorldPos, t) + Vector3.up * Mathf.Abs(shake);
                transform.localScale = Vector3.Lerp(new Vector3(baseScale.x * 1.15f, baseScale.y * 0.85f, baseScale.z * 1.15f), baseScale, t);
                yield return null;
            }

            transform.position = baseWorldPos;
            transform.localScale = baseScale;
        }

        public void PlayDeath(Action onComplete = null)
        {
            isDead = true;
            StartCoroutine(DeathRoutine(onComplete));
        }

        private IEnumerator DeathRoutine(Action onComplete)
        {
            Vector3 startPos = transform.position;
            Vector3 startScale = transform.localScale;

            float elapsed = 0f;
            float duration = 0.5f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(0f, 90f, t));
                transform.position = startPos - Vector3.up * Mathf.Lerp(0f, 0.3f, t);
                transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t * t);
                yield return null;
            }

            gameObject.SetActive(false);
            onComplete?.Invoke();
        }

        public void PlayVictory()
        {
            isVictory = true;
        }

        public void StopVictory()
        {
            isVictory = false;
            transform.position = baseWorldPos;
            transform.rotation = baseWorldRot;
            transform.localScale = baseScale;
            if (weaponTransform != null) weaponTransform.localRotation = Quaternion.identity;
        }

        public void SetScaleMultiplier(float mult)
        {
            transform.localScale = baseScale * mult;
        }

        public void SetGoldenAppearance(bool isGolden)
        {
            if (isGolden)
            {
                SetRendererColor(new Color(1f, 0.85f, 0.2f, 1f));
            }
            else
            {
                RestoreRendererColors();
            }
        }

        public void SetOverdriveVisual(bool isOverdrive)
        {
            if (isOverdrive)
            {
                SetRendererColor(new Color(1f, 0.7f, 0.15f, 1f));
            }
            else
            {
                RestoreRendererColors();
            }
        }

        public void SetEnrageVisual(bool isEnrage)
        {
            if (isEnrage)
            {
                SetRendererColor(new Color(1f, 0.25f, 0.2f, 1f));
            }
            else
            {
                RestoreRendererColors();
            }
        }

        /// <summary>
        /// Hoạt họa Tụ Lực Cảnh Báo (Telegraph Windup) trước khi tung kỹ năng nguy hiểm
        /// </summary>
        public void PlayTelegraphWindup(float duration, Action onComplete)
        {
            StartCoroutine(TelegraphWindupRoutine(duration, onComplete));
        }

        private IEnumerator TelegraphWindupRoutine(float duration, Action onComplete)
        {
            isBusy = true;
            Vector3 origPos = transform.position;
            Vector3 origScale = transform.localScale;
            float elapsed = 0f;
            Color warningColor = new Color(1f, 0.35f, 0.15f, 1f);
            SetRendererColor(warningColor);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                // Rung lắc dồn dập báo hiệu nguy hiểm
                float shake = Mathf.Sin(elapsed * 45f) * 0.08f * (0.5f + t * 0.5f);
                transform.position = origPos + new Vector3(shake, 0f, 0f);
                // Nén thân người chuẩn bị bung lực
                float squash = 1f + Mathf.Sin(t * Mathf.PI) * 0.15f;
                transform.localScale = new Vector3(origScale.x * squash, origScale.y * (2f - squash), origScale.z * squash);
                yield return null;
            }

            transform.position = origPos;
            transform.localScale = origScale;
            RestoreRendererColors();
            isBusy = false;
            onComplete?.Invoke();
        }

        /// <summary>
        /// Hoạt họa Dậm Đất Chấn Động của Đại Boss Titan (Titan Earth-Quake Stomp)
        /// </summary>
        public void PlayBossEarthQuakeStomp(Action onImpact, Action onComplete)
        {
            StartCoroutine(BossEarthQuakeStompRoutine(onImpact, onComplete));
        }

        private IEnumerator BossEarthQuakeStompRoutine(Action onImpact, Action onComplete)
        {
            isBusy = true;
            Vector3 startPos = transform.position;
            Vector3 startScale = transform.localScale;

            // 1. Nhấc bổng thân hình khổng lồ lên trời tụ lực (0.35s)
            float elapsed = 0f;
            while (elapsed < 0.35f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / 0.35f;
                transform.position = startPos + Vector3.up * (Mathf.Sin(t * Mathf.PI * 0.5f) * 0.9f);
                transform.localScale = new Vector3(startScale.x * 0.9f, startScale.y * 1.25f, startScale.z * 0.9f);
                yield return null;
            }

            // 2. Nện sấm sét dập mạnh xuống mặt đất (0.12s)
            elapsed = 0f;
            while (elapsed < 0.12f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / 0.12f;
                transform.position = Vector3.Lerp(startPos + Vector3.up * 0.9f, startPos, t * t);
                transform.localScale = Vector3.Lerp(new Vector3(startScale.x * 0.9f, startScale.y * 1.25f, startScale.z * 0.9f), new Vector3(startScale.x * 1.3f, startScale.y * 0.7f, startScale.z * 1.3f), t);
                yield return null;
            }

            transform.position = startPos;
            onImpact?.Invoke();
            yield return new WaitForSeconds(0.1f);

            // 3. Phục hồi dáng dấp ban đầu (0.2s)
            elapsed = 0f;
            while (elapsed < 0.2f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / 0.2f;
                transform.localScale = Vector3.Lerp(new Vector3(startScale.x * 1.3f, startScale.y * 0.7f, startScale.z * 1.3f), startScale, t);
                yield return null;
            }

            transform.localScale = startScale;
            isBusy = false;
            onComplete?.Invoke();
        }

        private void SetRendererColor(Color c)
        {
            foreach (var mat in cachedMaterials)
            {
                if (mat != null && mat.HasProperty("_BaseColor"))
                {
                    mat.SetColor("_BaseColor", c);
                }
            }
        }

        private void RestoreRendererColors()
        {
            for (int i = 0; i < cachedMaterials.Count; i++)
            {
                var mat = cachedMaterials[i];
                if (mat != null && mat.HasProperty("_BaseColor") && i < originalColors.Count)
                {
                    mat.SetColor("_BaseColor", originalColors[i]);
                }
            }
        }
    }
}
