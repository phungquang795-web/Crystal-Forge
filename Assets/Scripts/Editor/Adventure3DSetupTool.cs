#if UNITY_EDITOR
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using CrystalForge.Adventure;
using CrystalForge.UI;

namespace CrystalForge.Editor
{
    public static class Adventure3DSetupTool
    {
        [MenuItem("CrystalForge/6. Setup 3D Combat Arena & Models")]
        public static void Setup3DCombatArena()
        {
            var activeScene = EditorSceneManager.GetActiveScene();
            int undoGroup = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("Setup 3D Combat Arena & Models");

            Debug.Log("<color=#2EA3FF><b>[CrystalForge]</b> Bắt đầu thiết lập Đấu Trường 3D và Model Hero duy nhất...</color>");

            // 1. Chuẩn bị Materials
            Material stoneMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_ForgeStone.mat");
            Material trimMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_GoldTrim.mat");
            Material crystalMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_AmethystCrystal.mat");
            Material bronzeMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_MachineBronze.mat");
            Material mascotMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_MascotWarm.mat");

            Material slimeMat = GetOrCreateMaterial("Assets/Materials/Mat_EnemySlime.mat", new Color(0.2f, 0.95f, 0.65f, 0.88f), 0.9f, new Color(0.1f, 0.6f, 0.4f) * 1.5f);
            Material shadowMat = GetOrCreateMaterial("Assets/Materials/Mat_EnemyShadow.mat", new Color(0.12f, 0.10f, 0.18f, 1f), 0.6f, new Color(0.4f, 0.05f, 0.15f));
            Material fireMat = GetOrCreateMaterial("Assets/Materials/Mat_EnemyFire.mat", new Color(1f, 0.35f, 0.1f, 1f), 0.85f, new Color(1f, 0.4f, 0.1f) * 2.0f);

            // 2. Dọn dẹp triệt để các model cũ và đấu trường rời rạc để tránh trùng lặp 2 player
            var allObjs = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include);
            foreach (var go in allObjs)
            {
                if (go == null) continue;
                string n = go.name;
                if (n.Contains("Prop_ForgeMascot") || n.Contains("Hero_Player3D") || n.Contains("Adventure_CombatArena") || n.Contains("EnemyContainer") || n.Contains("Player_CapybaraHero"))
                {
                    Object.DestroyImmediate(go);
                }
            }

            // 3. Dựng Model 3D Hero DUY NHẤT cho toàn bộ game (Capybara Thợ Rèn phong cách Capybara Go)
            // Đặt trực tiếp tại bệ đe rèn trên đảo nổi
            var heroObj = CreateHeroModel(null, bronzeMat, trimMat, crystalMat, mascotMat);

            // 4. Dựng Container Quái Vật trực tiếp trên Đảo Nổi
            var enemyContainer = new GameObject("EnemyContainer");
            enemyContainer.transform.position = Vector3.zero;

            var slimeObj = CreateSlimeModel(enemyContainer.transform, slimeMat, crystalMat);
            var minionObj = CreateShadowMinionModel(enemyContainer.transform, shadowMat, fireMat);
            var golemObj = CreateGolemModel(enemyContainer.transform, stoneMat, trimMat, crystalMat);
            var bossObj = CreateDragonBossModel(enemyContainer.transform, shadowMat, fireMat, crystalMat);

            // 5. Cài đặt Adventure3DManager
            var managersObj = GameObject.Find("_Managers");
            if (managersObj == null) managersObj = new GameObject("_Managers");

            var adv3D = managersObj.GetComponent<Adventure3DManager>();
            if (adv3D == null) adv3D = managersObj.AddComponent<Adventure3DManager>();

            var soAdv3D = new SerializedObject(adv3D);
            soAdv3D.FindProperty("heroUnit").objectReferenceValue = heroObj.GetComponent<CombatUnit3D>();
            soAdv3D.FindProperty("enemyContainer").objectReferenceValue = enemyContainer.transform;
            soAdv3D.FindProperty("slimePrefab").objectReferenceValue = slimeObj.GetComponent<CombatUnit3D>();
            soAdv3D.FindProperty("minionPrefab").objectReferenceValue = minionObj.GetComponent<CombatUnit3D>();
            soAdv3D.FindProperty("golemPrefab").objectReferenceValue = golemObj.GetComponent<CombatUnit3D>();
            soAdv3D.FindProperty("dragonBossPrefab").objectReferenceValue = bossObj.GetComponent<CombatUnit3D>();

            soAdv3D.FindProperty("idleForgePos").vector3Value = new Vector3(-1.6f, 0.0f, -1.4f);
            soAdv3D.FindProperty("idleForgeRot").quaternionValue = Quaternion.Euler(0f, 155f, 0f);
            soAdv3D.FindProperty("heroCombatPos").vector3Value = new Vector3(-1.5f, 0.0f, -0.8f);
            soAdv3D.FindProperty("heroCombatRot").quaternionValue = Quaternion.Euler(0f, 90f, 0f);
            soAdv3D.FindProperty("enemyCombatPos").vector3Value = new Vector3(1.5f, 0.0f, -0.8f);
            soAdv3D.FindProperty("enemyCombatRot").quaternionValue = Quaternion.Euler(0f, -90f, 0f);
            soAdv3D.FindProperty("forgeCenter").vector3Value = new Vector3(0f, 0.0f, 0f);

            soAdv3D.FindProperty("normalCamPos").vector3Value = new Vector3(0f, 4.8f, -6.6f);
            soAdv3D.FindProperty("combatCamPos").vector3Value = new Vector3(0f, 4.2f, -5.6f);
            soAdv3D.FindProperty("normalCamRot").quaternionValue = Quaternion.Euler(28f, 0f, 0f);
            soAdv3D.FindProperty("combatCamRot").quaternionValue = Quaternion.Euler(25f, 0f, 0f);
            soAdv3D.ApplyModifiedProperties();

            // 6. Cập nhật Giao diện CombatPanel trong Canvas để mở rộng tầm nhìn 3D
            OptimizeCombatPanelLayout();

            // 7. Khử dấu tiếng Việt trong toàn bộ TextMeshPro trên Scene để chữ hiển thị trọn vẹn 100%
            SanitizeSceneTexts();

            Undo.CollapseUndoOperations(undoGroup);
            EditorSceneManager.MarkSceneDirty(activeScene);
            EditorSceneManager.SaveOpenScenes();

            Debug.Log("<color=#00FF88><b>[CrystalForge]</b> Thiết lập Đấu Trường 3D Capybara Go và Model Duy Nhất hoàn tất 100%!</color>");
        }

        private static GameObject CreateHeroModel(Transform parent, Material armorMat, Material trimMat, Material crystalMat, Material skinMat)
        {
            var heroPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Player_Hero.prefab");
            GameObject heroRoot;
            if (heroPrefab != null)
            {
                heroRoot = (GameObject)PrefabUtility.InstantiatePrefab(heroPrefab);
                if (parent != null) heroRoot.transform.parent = parent;
            }
            else
            {
                heroRoot = new GameObject("Player_CapybaraHero");
                if (parent != null) heroRoot.transform.parent = parent;
                heroRoot.tag = "Player";

                var fbxPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Spirte/Player.fbx");
                if (fbxPrefab != null)
                {
                    var model = (GameObject)PrefabUtility.InstantiatePrefab(fbxPrefab, heroRoot.transform);
                    model.name = "Model_Player";
                    model.transform.localPosition = Vector3.zero;
                    model.transform.localRotation = Quaternion.identity;

                    var pbrMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_PlayerPBR.mat");
                    var mr = model.GetComponentInChildren<MeshRenderer>();
                    if (mr != null && pbrMat != null)
                    {
                        mr.sharedMaterial = pbrMat;
                    }
                }

                var lampLight = new GameObject("Headlamp_MagicLight");
                lampLight.transform.SetParent(heroRoot.transform, false);
                lampLight.transform.localPosition = new Vector3(0f, 1.05f, 0.22f);
                var lComp = lampLight.AddComponent<Light>();
                lComp.type = LightType.Point;
                lComp.color = new Color(0.2f, 0.92f, 1.0f);
                lComp.range = 3.5f;
                lComp.intensity = 1.8f;

                var unit = heroRoot.AddComponent<CombatUnit3D>();
                var so = new SerializedObject(unit);
                so.FindProperty("isHero").boolValue = true;
                so.FindProperty("weaponTransform").objectReferenceValue = null;
                so.ApplyModifiedProperties();
                unit.CacheRenderers();

                AttachOverheadHealthBar(heroRoot, isHero: true, headOffset: 1.55f);
            }

            heroRoot.transform.position = new Vector3(-1.6f, 0.0f, -1.4f);
            heroRoot.transform.rotation = Quaternion.Euler(0f, 155f, 0f);
            heroRoot.transform.localScale = Vector3.one * 1.15f;
            heroRoot.tag = "Player";

            var cUnit = heroRoot.GetComponent<CombatUnit3D>();
            if (cUnit != null) cUnit.CacheRenderers();

            return heroRoot;
        }

        private static GameObject CreateSlimeModel(Transform parent, Material slimeMat, Material crystalMat)
        {
            var slime = new GameObject("Monster_Slime");
            slime.transform.parent = parent;
            slime.transform.localPosition = new Vector3(1.6f, 0.05f, 0f);
            slime.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
            slime.transform.localScale = new Vector3(0.85f, 0.7f, 0.85f);

            var body = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            body.name = "SlimeBody";
            body.transform.parent = slime.transform;
            body.transform.localPosition = new Vector3(0f, 0.45f, 0f);
            body.GetComponent<Renderer>().sharedMaterial = slimeMat;

            // Lõi pha lê bên trong
            var core = GameObject.CreatePrimitive(PrimitiveType.Cube);
            core.name = "SlimeCore";
            core.transform.parent = body.transform;
            core.transform.localPosition = Vector3.zero;
            core.transform.localRotation = Quaternion.Euler(45f, 45f, 45f);
            core.transform.localScale = Vector3.one * 0.45f;
            core.GetComponent<Renderer>().sharedMaterial = crystalMat;

            // 2 mắt dễ thương
            CreateEye(body.transform, new Vector3(-0.25f, 0.2f, 0.85f), crystalMat);
            CreateEye(body.transform, new Vector3(0.25f, 0.2f, 0.85f), crystalMat);

            var unit = slime.AddComponent<CombatUnit3D>();
            var so = new SerializedObject(unit);
            so.FindProperty("isHero").boolValue = false;
            so.ApplyModifiedProperties();
            unit.CacheRenderers();

            // Gắn thanh máu trên đầu Quái (Màu đỏ)
            AttachOverheadHealthBar(slime, isHero: false, headOffset: 0.95f);

            slime.SetActive(false);
            return slime;
        }

        private static GameObject CreateShadowMinionModel(Transform parent, Material shadowMat, Material fireMat)
        {
            var minion = new GameObject("Monster_ShadowMinion");
            minion.transform.parent = parent;
            minion.transform.localPosition = new Vector3(1.6f, 0.05f, 0f);
            minion.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
            minion.transform.localScale = new Vector3(0.8f, 0.85f, 0.8f);

            var body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.name = "MinionBody";
            body.transform.parent = minion.transform;
            body.transform.localPosition = new Vector3(0f, 0.5f, 0f);
            body.transform.localScale = new Vector3(0.55f, 0.5f, 0.5f);
            body.GetComponent<Renderer>().sharedMaterial = shadowMat;

            // 2 Sừng quỷ sắc nhọn
            var horn1 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            horn1.transform.parent = body.transform;
            horn1.transform.localPosition = new Vector3(-0.3f, 0.75f, 0f);
            horn1.transform.localRotation = Quaternion.Euler(0f, 0f, 35f);
            horn1.transform.localScale = new Vector3(0.12f, 0.4f, 0.12f);
            horn1.GetComponent<Renderer>().sharedMaterial = fireMat;

            var horn2 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            horn2.transform.parent = body.transform;
            horn2.transform.localPosition = new Vector3(0.3f, 0.75f, 0f);
            horn2.transform.localRotation = Quaternion.Euler(0f, 0f, -35f);
            horn2.transform.localScale = new Vector3(0.12f, 0.4f, 0.12f);
            horn2.GetComponent<Renderer>().sharedMaterial = fireMat;

            // 2 Mắt đỏ rực
            CreateEye(body.transform, new Vector3(-0.2f, 0.3f, 0.85f), fireMat);
            CreateEye(body.transform, new Vector3(0.2f, 0.3f, 0.85f), fireMat);

            var unit = minion.AddComponent<CombatUnit3D>();
            var so = new SerializedObject(unit);
            so.FindProperty("isHero").boolValue = false;
            so.ApplyModifiedProperties();
            unit.CacheRenderers();

            // Gắn thanh máu trên đầu Quái (Màu đỏ)
            AttachOverheadHealthBar(minion, isHero: false, headOffset: 1.25f);

            minion.SetActive(false);
            return minion;
        }

        private static GameObject CreateGolemModel(Transform parent, Material stoneMat, Material trimMat, Material crystalMat)
        {
            var golem = new GameObject("Monster_GolemGuardian");
            golem.transform.parent = parent;
            golem.transform.localPosition = new Vector3(1.6f, 0.05f, 0f);
            golem.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
            golem.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);

            var torso = GameObject.CreatePrimitive(PrimitiveType.Cube);
            torso.name = "GolemTorso";
            torso.transform.parent = golem.transform;
            torso.transform.localPosition = new Vector3(0f, 0.65f, 0f);
            torso.transform.localScale = new Vector3(0.9f, 0.85f, 0.7f);
            torso.GetComponent<Renderer>().sharedMaterial = stoneMat;

            // Lõi ma thuật ngực
            var core = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            core.transform.parent = torso.transform;
            core.transform.localPosition = new Vector3(0f, 0.1f, 0.5f);
            core.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            core.GetComponent<Renderer>().sharedMaterial = crystalMat;

            // 2 Nắm đấm đá hộ vệ
            var fistL = GameObject.CreatePrimitive(PrimitiveType.Cube);
            fistL.transform.parent = golem.transform;
            fistL.transform.localPosition = new Vector3(-0.75f, 0.45f, 0.2f);
            fistL.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            fistL.GetComponent<Renderer>().sharedMaterial = trimMat;

            var fistR = GameObject.CreatePrimitive(PrimitiveType.Cube);
            fistR.transform.parent = golem.transform;
            fistR.transform.localPosition = new Vector3(0.75f, 0.45f, 0.2f);
            fistR.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            fistR.GetComponent<Renderer>().sharedMaterial = trimMat;

            var unit = golem.AddComponent<CombatUnit3D>();
            var so = new SerializedObject(unit);
            so.FindProperty("isHero").boolValue = false;
            so.ApplyModifiedProperties();
            unit.CacheRenderers();

            // Gắn thanh máu trên đầu Quái (Màu đỏ)
            AttachOverheadHealthBar(golem, isHero: false, headOffset: 1.55f);

            golem.SetActive(false);
            return golem;
        }

        private static GameObject CreateDragonBossModel(Transform parent, Material shadowMat, Material fireMat, Material crystalMat)
        {
            var boss = new GameObject("Monster_DragonBoss");
            boss.transform.parent = parent;
            boss.transform.localPosition = new Vector3(1.6f, 0.05f, 0f);
            boss.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
            boss.transform.localScale = new Vector3(1.35f, 1.35f, 1.35f);

            var body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.name = "BossBody";
            body.transform.parent = boss.transform;
            body.transform.localPosition = new Vector3(0f, 0.65f, 0f);
            body.transform.localRotation = Quaternion.Euler(20f, 0f, 0f);
            body.transform.localScale = new Vector3(0.75f, 0.7f, 0.75f);
            body.GetComponent<Renderer>().sharedMaterial = shadowMat;

            // Đôi cánh pha lê bóng tối
            var wingL = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wingL.transform.parent = body.transform;
            wingL.transform.localPosition = new Vector3(-0.75f, 0.4f, -0.2f);
            wingL.transform.localRotation = Quaternion.Euler(0f, -30f, 40f);
            wingL.transform.localScale = new Vector3(1.2f, 0.08f, 0.6f);
            wingL.GetComponent<Renderer>().sharedMaterial = crystalMat;

            var wingR = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wingR.transform.parent = body.transform;
            wingR.transform.localPosition = new Vector3(0.75f, 0.4f, -0.2f);
            wingR.transform.localRotation = Quaternion.Euler(0f, 30f, -40f);
            wingR.transform.localScale = new Vector3(1.2f, 0.08f, 0.6f);
            wingR.GetComponent<Renderer>().sharedMaterial = crystalMat;

            // Sừng rồng dung nham
            var crest = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            crest.transform.parent = body.transform;
            crest.transform.localPosition = new Vector3(0f, 0.95f, 0.2f);
            crest.transform.localRotation = Quaternion.Euler(-30f, 0f, 0f);
            crest.transform.localScale = new Vector3(0.15f, 0.5f, 0.15f);
            crest.GetComponent<Renderer>().sharedMaterial = fireMat;

            // 2 Mắt rồng đỏ rực
            CreateEye(body.transform, new Vector3(-0.25f, 0.35f, 0.8f), fireMat);
            CreateEye(body.transform, new Vector3(0.25f, 0.35f, 0.8f), fireMat);

            var unit = boss.AddComponent<CombatUnit3D>();
            var so = new SerializedObject(unit);
            so.FindProperty("isHero").boolValue = false;
            so.ApplyModifiedProperties();
            unit.CacheRenderers();

            // Gắn thanh máu trên đầu Quái (Màu đỏ)
            AttachOverheadHealthBar(boss, isHero: false, headOffset: 1.85f);

            boss.SetActive(false);
            return boss;
        }

        private static void CreateEye(Transform parent, Vector3 localPos, Material mat)
        {
            var eye = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            eye.name = "Eye";
            eye.transform.parent = parent;
            eye.transform.localPosition = localPos;
            eye.transform.localScale = Vector3.one * 0.15f;
            eye.GetComponent<Renderer>().sharedMaterial = mat;
        }

        private static void CreateTorchPylon(Transform parent, Vector3 localPos, Material stoneMat, Material trimMat, Material crystalMat)
        {
            var pylon = new GameObject("RunicTorch");
            pylon.transform.parent = parent;
            pylon.transform.localPosition = localPos;

            var pillar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pillar.transform.parent = pylon.transform;
            pillar.transform.localPosition = new Vector3(0f, 0.5f, 0f);
            pillar.transform.localScale = new Vector3(0.25f, 0.5f, 0.25f);
            pillar.GetComponent<Renderer>().sharedMaterial = stoneMat;

            var ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ring.transform.parent = pylon.transform;
            ring.transform.localPosition = new Vector3(0f, 0.9f, 0f);
            ring.transform.localScale = new Vector3(0.38f, 0.08f, 0.38f);
            ring.GetComponent<Renderer>().sharedMaterial = trimMat;

            var orb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            orb.transform.parent = pylon.transform;
            orb.transform.localPosition = new Vector3(0f, 1.15f, 0f);
            orb.transform.localScale = Vector3.one * 0.3f;
            orb.GetComponent<Renderer>().sharedMaterial = crystalMat;
        }

        private static void OptimizeCombatPanelLayout()
        {
            var modal = Object.FindAnyObjectByType<AdventureModalUI>(FindObjectsInactive.Include);
            if (modal == null) return;

            var combatPanel = modal.transform.Find("CombatPanel");
            if (combatPanel == null) return;

            // 1. HeroBox: Ẩn hoàn toàn vì đã có thanh máu 3D World-Space trên đầu Capybara Hero
            var heroBox = combatPanel.Find("HeroBox");
            if (heroBox != null)
            {
                heroBox.gameObject.SetActive(false);
            }

            // 2. EnemyBox: Ẩn hoàn toàn vì đã có thanh máu 3D World-Space trên đầu Quái Vật
            var enemyBox = combatPanel.Find("EnemyBox");
            if (enemyBox != null)
            {
                enemyBox.gameObject.SetActive(false);
            }

            // 3. LogBox: Ẩn hoàn toàn bảng nhật ký để giữ tầm nhìn chiến đấu 3D thoáng đãng
            var logBox = combatPanel.Find("LogBox");
            if (logBox != null)
            {
                logBox.gameObject.SetActive(false);
            }
        }

        private static void SanitizeSceneTexts()
        {
            var texts = Object.FindObjectsByType<TMP_Text>(FindObjectsInactive.Include);
            foreach (var t in texts)
            {
                if (t == null) continue;
                string original = t.text;
                if (string.IsNullOrEmpty(original)) continue;

                string clean = CrystalForge.Utils.FontSanitizer.Sanitize(original);
                if (clean != original)
                {
                    t.text = clean;
                    EditorUtility.SetDirty(t);
                }
            }
        }

        private static void AttachOverheadHealthBar(GameObject unitObj, bool isHero, float headOffset)
        {
            // Xoa bar cu neu da ton tai de tranh duplicate hoac loi scale
            var oldBar = unitObj.transform.Find("OverheadHealthBar");
            if (oldBar != null)
            {
                Object.DestroyImmediate(oldBar.gameObject);
            }

            var barObj = new GameObject("OverheadHealthBar", typeof(RectTransform));
            barObj.transform.SetParent(unitObj.transform, false);
            barObj.transform.localPosition = new Vector3(0f, headOffset, 0f);
            barObj.transform.localRotation = Quaternion.identity;
            barObj.transform.localScale = Vector3.one * 0.008f; // 110px * 0.008 = 0.88m chuẩn vừa vặn trên đầu

            var rt = barObj.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(110f, 20f);
            rt.pivot = new Vector2(0.5f, 0.5f);

            var canvas = barObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = Camera.main;
            canvas.sortingOrder = 50;

            var cg = barObj.AddComponent<CanvasGroup>();
            cg.alpha = isHero ? 0f : 1f;

            // 1. Khung nen toi bo goc
            var bgObj = CreateUIChild("BgFrame", barObj.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            var bgImg = bgObj.AddComponent<Image>();
            bgImg.color = new Color(0.04f, 0.03f, 0.08f, 0.92f);
            bgImg.raycastTarget = false;

            // 2. Delayed Fill (Vang nhat catch-up)
            var delObj = CreateUIChild("DelayedFill", barObj.transform, Vector2.zero, Vector2.one, new Vector2(1.5f, 1.5f), new Vector2(-1.5f, -1.5f));
            var delImg = delObj.AddComponent<Image>();
            delImg.color = new Color(1f, 0.9f, 0.55f, 0.85f);
            delImg.raycastTarget = false;
            var delRt = delObj.GetComponent<RectTransform>();

            // 3. Main Fill (Xanh cho Hero, Do cho Quai)
            var mainObj = CreateUIChild("MainFill", barObj.transform, Vector2.zero, Vector2.one, new Vector2(1.5f, 1.5f), new Vector2(-1.5f, -1.5f));
            var mainImg = mainObj.AddComponent<Image>();
            Color hpColor = isHero ? new Color(0.0f, 0.95f, 0.45f, 1f) : new Color(1f, 0.18f, 0.25f, 1f);
            mainImg.color = hpColor;
            mainImg.raycastTarget = false;
            var mainRt = mainObj.GetComponent<RectTransform>();

            // 4. So HP Text
            var textObj = CreateUIChild("HpText", barObj.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            var tmpText = textObj.AddComponent<TextMeshProUGUI>();
            tmpText.text = isHero ? "1000/1000" : "300/300";
            tmpText.fontSize = 11.5f;
            tmpText.fontStyle = FontStyles.Bold;
            tmpText.color = Color.white;
            tmpText.alignment = TextAlignmentOptions.Center;
            tmpText.raycastTarget = false;

            // 5. OverheadHealthBar3D Component
            var overhead = barObj.AddComponent<OverheadHealthBar3D>();
            var so = new SerializedObject(overhead);
            so.FindProperty("isHero").boolValue = isHero;
            so.FindProperty("mainFillRect").objectReferenceValue = mainRt;
            so.FindProperty("delayedFillRect").objectReferenceValue = delRt;
            so.FindProperty("mainFillImage").objectReferenceValue = mainImg;
            so.FindProperty("hpText").objectReferenceValue = tmpText;
            so.FindProperty("canvasGroup").objectReferenceValue = cg;
            so.FindProperty("healthColor").colorValue = hpColor;
            so.ApplyModifiedProperties();
        }

        private static GameObject CreateUIChild(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;

            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.offsetMin = offsetMin;
            rt.offsetMax = offsetMax;
            return go;
        }

        private static Material GetOrCreateMaterial(string path, Color baseColor, float smoothness, Color emissiveColor)
        {
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat != null) return mat;

            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");

            mat = new Material(shader);
            mat.SetColor("_BaseColor", baseColor);
            mat.SetFloat("_Smoothness", smoothness);

            if (emissiveColor != Color.black)
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", emissiveColor);
            }

            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            AssetDatabase.CreateAsset(mat, path);
            AssetDatabase.SaveAssets();
            return mat;
        }
    }
}
#endif
