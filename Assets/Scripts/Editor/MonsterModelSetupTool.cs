#if UNITY_EDITOR
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using CrystalForge.Adventure;
using CrystalForge.Utils;

namespace CrystalForge.Editor
{
    public static class MonsterModelSetupTool
    {
        [MenuItem("CrystalForge/7. Generate 3 Monster Models From Images")]
        public static void GenerateThreeMonsterModels()
        {
            var activeScene = EditorSceneManager.GetActiveScene();
            int undoGroup = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("Generate 3 Monster Models From Images");

            Debug.Log("<color=#00FFCC><b>[CrystalForge]</b> Đang tạo 3 mẫu Quái Vật 3D theo ảnh...</color>");

            // 1. Tạo hoặc tải các Materials chất lượng cao URP Lit
            Material rockStoneMat = GetOrCreateMaterial("Assets/Materials/Mat_RockGolem_Stone.mat", new Color(0.55f, 0.53f, 0.50f, 1f), 0.35f, Color.black);
            Material rockAmberEyeMat = GetOrCreateMaterial("Assets/Materials/Mat_RockGolem_AmberEye.mat", new Color(1.0f, 0.85f, 0.15f, 1f), 0.9f, new Color(1.0f, 0.8f, 0.15f) * 2.8f);
            Material spirePurpleMat = GetOrCreateMaterial("Assets/Materials/Mat_RockGolem_CrystalSpirePurple.mat", new Color(0.72f, 0.25f, 0.95f, 0.9f), 0.95f, new Color(0.68f, 0.20f, 0.92f) * 2.2f);
            Material spireCyanMat = GetOrCreateMaterial("Assets/Materials/Mat_RockGolem_CrystalSpireCyan.mat", new Color(0.18f, 0.88f, 0.96f, 0.9f), 0.95f, new Color(0.15f, 0.85f, 0.95f) * 2.5f);

            Material batBodyMat = GetOrCreateMaterial("Assets/Materials/Mat_Bat_ChibiBody.mat", new Color(0.62f, 0.58f, 0.74f, 1f), 0.45f, Color.black);
            Material batBellyMat = GetOrCreateMaterial("Assets/Materials/Mat_Bat_ChibiBelly.mat", new Color(0.75f, 0.70f, 0.85f, 1f), 0.45f, Color.black);
            Material batWingPrismMat = GetOrCreateMaterial("Assets/Materials/Mat_Bat_CrystalWingPrism.mat", new Color(0.25f, 0.85f, 0.98f, 0.88f), 0.95f, new Color(0.20f, 0.75f, 1.0f) * 2.6f);
            Material batVortexOrbMat = GetOrCreateMaterial("Assets/Materials/Mat_Bat_VortexOrb.mat", new Color(0.35f, 0.90f, 1.0f, 0.95f), 0.98f, new Color(0.25f, 0.85f, 1.0f) * 3.8f);
            Material batInnerEarMat = GetOrCreateMaterial("Assets/Materials/Mat_Bat_InnerEarGlow.mat", new Color(0.55f, 0.85f, 1.0f, 1f), 0.85f, new Color(0.35f, 0.80f, 1.0f) * 2.0f);
            Material runeBaseMat = GetOrCreateMaterial("Assets/Materials/Mat_Bat_RuneBase.mat", new Color(0.25f, 0.23f, 0.28f, 1f), 0.5f, new Color(0.4f, 0.2f, 0.6f) * 0.8f);

            Material guardianStoneMat = GetOrCreateMaterial("Assets/Materials/Mat_Guardian_AncientStone.mat", new Color(0.48f, 0.50f, 0.48f, 1f), 0.3f, Color.black);
            Material emeraldMossMat = GetOrCreateMaterial("Assets/Materials/Mat_Guardian_EmeraldMoss.mat", new Color(0.28f, 0.60f, 0.22f, 1f), 0.25f, Color.black);
            Material cyanGlowMat = GetOrCreateMaterial("Assets/Materials/Mat_Guardian_CyanGlow.mat", new Color(0.15f, 0.92f, 1.0f, 1f), 0.98f, new Color(0.12f, 0.90f, 1.0f) * 3.5f);
            Material maceSpikeMat = GetOrCreateMaterial("Assets/Materials/Mat_Guardian_CrystalSpike.mat", new Color(0.20f, 0.88f, 1.0f, 0.92f), 0.95f, new Color(0.15f, 0.85f, 1.0f) * 2.6f);

            // 2. Chuẩn bị Thư mục Prefabs
            string prefabFolder = "Assets/Prefabs/Enemies";
            if (!Directory.Exists(prefabFolder))
            {
                Directory.CreateDirectory(prefabFolder);
                AssetDatabase.Refresh();
            }

            // 3. Tìm hoặc tạo EnemyContainer trong Scene
            var enemyContainer = GameObject.Find("EnemyContainer");
            if (enemyContainer == null)
            {
                enemyContainer = new GameObject("EnemyContainer");
                enemyContainer.transform.position = Vector3.zero;
            }

            // Xóa triệt để các quái vật cũ, chỉ giữ lại duy nhất 3 mẫu quái theo ảnh
            DestroyOldMonster(enemyContainer.transform, "Monster_Slime");
            DestroyOldMonster(enemyContainer.transform, "Monster_ShadowMinion");
            DestroyOldMonster(enemyContainer.transform, "Monster_GolemGuardian");
            DestroyOldMonster(enemyContainer.transform, "Monster_DragonBoss");
            DestroyOldMonster(enemyContainer.transform, "Monster_CrystalSpireGolem");
            DestroyOldMonster(enemyContainer.transform, "Monster_PrismBat");
            DestroyOldMonster(enemyContainer.transform, "Monster_MossyTitanGuardian");

            // 4. Dựng Model 1: Crystal Spire Golem (Ảnh 1)
            var golemObj = CreateCrystalSpireGolem(enemyContainer.transform, rockStoneMat, rockAmberEyeMat, spirePurpleMat, spireCyanMat);

            // 5. Dựng Model 2: Prism Crystal Bat (Ảnh 2)
            var batObj = CreatePrismBat(enemyContainer.transform, batBodyMat, batBellyMat, batWingPrismMat, batVortexOrbMat, batInnerEarMat, runeBaseMat);

            // 6. Dựng Model 3: Mossy Titan Guardian (Ảnh 3)
            var guardianObj = CreateMossyTitanGuardian(enemyContainer.transform, guardianStoneMat, emeraldMossMat, cyanGlowMat, maceSpikeMat);

            // 7. Lưu Prefab cho cả 3 Model
            SaveAsPrefab(golemObj, $"{prefabFolder}/Prefab_CrystalSpireGolem.prefab");
            SaveAsPrefab(batObj, $"{prefabFolder}/Prefab_PrismCrystalBat.prefab");
            SaveAsPrefab(guardianObj, $"{prefabFolder}/Prefab_MossyTitanGuardian.prefab");

            // 8. Cập nhật CrystalDefenseManager & Adventure3DManager
            var managersObj = GameObject.Find("_Managers");
            if (managersObj != null)
            {
                var defenseMgr = managersObj.GetComponent<CrystalDefenseManager>();
                if (defenseMgr != null)
                {
                    var soDefense = new SerializedObject(defenseMgr);
                    soDefense.FindProperty("crystalSpireGolemPrefab").objectReferenceValue = golemObj.GetComponent<CombatUnit3D>();
                    soDefense.FindProperty("prismBatPrefab").objectReferenceValue = batObj.GetComponent<CombatUnit3D>();
                    soDefense.FindProperty("mossyGuardianPrefab").objectReferenceValue = guardianObj.GetComponent<CombatUnit3D>();
                    soDefense.FindProperty("slimePrefab").objectReferenceValue = null;
                    soDefense.FindProperty("minionPrefab").objectReferenceValue = null;
                    soDefense.FindProperty("golemPrefab").objectReferenceValue = null;
                    soDefense.FindProperty("dragonBossPrefab").objectReferenceValue = null;
                    soDefense.ApplyModifiedProperties();
                }

                var adv3DMgr = managersObj.GetComponent<Adventure3DManager>();
                if (adv3DMgr != null)
                {
                    var soAdv = new SerializedObject(adv3DMgr);
                    soAdv.FindProperty("crystalSpireGolemPrefab").objectReferenceValue = golemObj.GetComponent<CombatUnit3D>();
                    soAdv.FindProperty("prismBatPrefab").objectReferenceValue = batObj.GetComponent<CombatUnit3D>();
                    soAdv.FindProperty("mossyGuardianPrefab").objectReferenceValue = guardianObj.GetComponent<CombatUnit3D>();
                    soAdv.FindProperty("slimePrefab").objectReferenceValue = null;
                    soAdv.FindProperty("minionPrefab").objectReferenceValue = null;
                    soAdv.FindProperty("golemPrefab").objectReferenceValue = null;
                    soAdv.FindProperty("dragonBossPrefab").objectReferenceValue = null;
                    soAdv.ApplyModifiedProperties();
                }
            }

            Undo.CollapseUndoOperations(undoGroup);
            EditorSceneManager.MarkSceneDirty(activeScene);
            EditorSceneManager.SaveOpenScenes();

            Debug.Log("<color=#00FF88><b>[CrystalForge]</b> Tạo 3 Model Quái Vật thành công và đã đồng bộ hoàn hảo vào Game Loop!</color>");
        }

        // =========================================================================
        // MODEL 1: CRYSTAL SPIRE GOLEM (Ảnh 1)
        // =========================================================================
        private static GameObject CreateCrystalSpireGolem(Transform parent, Material stoneMat, Material eyeMat, Material spirePurpleMat, Material spireCyanMat)
        {
            var golem = new GameObject("Monster_CrystalSpireGolem");
            if (parent != null) golem.transform.parent = parent;
            golem.transform.localPosition = new Vector3(1.6f, 0.05f, 0f);
            golem.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
            golem.transform.localScale = Vector3.one * 1.05f;

            // Bệ đá lục giác dưới chân
            var hexBase = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            hexBase.name = "HexPedestal";
            hexBase.transform.parent = golem.transform;
            hexBase.transform.localPosition = new Vector3(0f, 0.04f, 0f);
            hexBase.transform.localScale = new Vector3(1.15f, 0.04f, 1.15f);
            hexBase.GetComponent<Renderer>().sharedMaterial = stoneMat;

            // Bàn chân và cẳng chân đá vững chắc
            CreateStoneLeg(golem.transform, new Vector3(-0.32f, 0.22f, 0f), stoneMat);
            CreateStoneLeg(golem.transform, new Vector3(0.32f, 0.22f, 0f), stoneMat);

            // Thân đá tảng (Torso)
            var torso = GameObject.CreatePrimitive(PrimitiveType.Cube);
            torso.name = "GolemTorso";
            torso.transform.parent = golem.transform;
            torso.transform.localPosition = new Vector3(0f, 0.62f, 0f);
            torso.transform.localScale = new Vector3(0.85f, 0.58f, 0.65f);
            torso.GetComponent<Renderer>().sharedMaterial = stoneMat;

            // Mảng đá cơ ngực và bụng gồ ghề
            var chestPlate = GameObject.CreatePrimitive(PrimitiveType.Cube);
            chestPlate.name = "ChestRockPlate";
            chestPlate.transform.parent = torso.transform;
            chestPlate.transform.localPosition = new Vector3(0f, 0.1f, 0.28f);
            chestPlate.transform.localScale = new Vector3(0.92f, 0.55f, 0.55f);
            chestPlate.GetComponent<Renderer>().sharedMaterial = stoneMat;

            // Đầu đá chiseled
            var head = GameObject.CreatePrimitive(PrimitiveType.Cube);
            head.name = "GolemHead";
            head.transform.parent = golem.transform;
            head.transform.localPosition = new Vector3(0f, 1.05f, 0.12f);
            head.transform.localScale = new Vector3(0.58f, 0.46f, 0.52f);
            head.GetComponent<Renderer>().sharedMaterial = stoneMat;

            // Cằm đá nhô
            var jaw = GameObject.CreatePrimitive(PrimitiveType.Cube);
            jaw.name = "GolemJaw";
            jaw.transform.parent = head.transform;
            jaw.transform.localPosition = new Vector3(0f, -0.32f, 0.2f);
            jaw.transform.localScale = new Vector3(0.85f, 0.38f, 0.65f);
            jaw.GetComponent<Renderer>().sharedMaterial = stoneMat;

            // 2 Sừng đá trên đầu
            CreateHorn(head.transform, new Vector3(-0.35f, 0.48f, 0f), new Vector3(0f, 0f, 25f), stoneMat);
            CreateHorn(head.transform, new Vector3(0.35f, 0.48f, 0f), new Vector3(0f, 0f, -25f), stoneMat);

            // 2 Mắt vàng rực rỡ phát sáng (Amber Eyes)
            CreateGlowingEye(head.transform, new Vector3(-0.19f, 0.05f, 0.52f), new Vector3(0.12f, 0.08f, 0.08f), eyeMat);
            CreateGlowingEye(head.transform, new Vector3(0.19f, 0.05f, 0.52f), new Vector3(0.12f, 0.08f, 0.08f), eyeMat);

            // Cánh tay & Nắm đấm đá to khỏe
            var armL = CreateStoneArm(golem.transform, new Vector3(-0.62f, 0.58f, 0.1f), -1, stoneMat);
            var armR = CreateStoneArm(golem.transform, new Vector3(0.62f, 0.58f, 0.1f), 1, stoneMat);

            // CỤM TINH THỂ LƯNG KHỔNG LỒ (Spire Cluster: 7 Tinh Thể Tím & Lam)
            var spireGroup = new GameObject("BackCrystalSpireCluster");
            spireGroup.transform.parent = torso.transform;
            spireGroup.transform.localPosition = new Vector3(0f, 0.15f, -0.28f);

            // Cột tinh thể trung tâm cao vút (Tím)
            CreateCrystalSpire(spireGroup.transform, new Vector3(0f, 0.2f, 0f), new Vector3(-25f, 0f, 0f), new Vector3(0.24f, 1.05f, 0.24f), spirePurpleMat);
            // 2 Cột trên (Lam & Tím)
            CreateCrystalSpire(spireGroup.transform, new Vector3(-0.22f, 0.15f, 0.02f), new Vector3(-32f, -18f, -12f), new Vector3(0.20f, 0.92f, 0.20f), spireCyanMat);
            CreateCrystalSpire(spireGroup.transform, new Vector3(0.22f, 0.15f, 0.02f), new Vector3(-30f, 20f, 15f), new Vector3(0.21f, 0.95f, 0.21f), spirePurpleMat);
            // 2 Cột giữa xòe ngang (Tím & Lam)
            CreateCrystalSpire(spireGroup.transform, new Vector3(-0.35f, 0.02f, 0.05f), new Vector3(-22f, -38f, -25f), new Vector3(0.17f, 0.78f, 0.17f), spirePurpleMat);
            CreateCrystalSpire(spireGroup.transform, new Vector3(0.35f, 0.02f, 0.05f), new Vector3(-20f, 36f, 22f), new Vector3(0.18f, 0.82f, 0.18f), spireCyanMat);
            // 2 Cột dưới chân cụm (Lam & Tím)
            CreateCrystalSpire(spireGroup.transform, new Vector3(-0.25f, -0.18f, 0.08f), new Vector3(-12f, -25f, -8f), new Vector3(0.15f, 0.62f, 0.15f), spireCyanMat);
            CreateCrystalSpire(spireGroup.transform, new Vector3(0.25f, -0.18f, 0.08f), new Vector3(-10f, 25f, 10f), new Vector3(0.15f, 0.60f, 0.15f), spirePurpleMat);

            // Gắn CombatUnit3D
            var unit = golem.AddComponent<CombatUnit3D>();
            var so = new SerializedObject(unit);
            so.FindProperty("isHero").boolValue = false;
            so.FindProperty("weaponTransform").objectReferenceValue = armR.transform;
            so.ApplyModifiedProperties();
            unit.CacheRenderers();

            // Gắn thanh máu 3D trên đầu
            AttachOverheadHealthBar(golem, "CRYSTAL GOLEM", 1.55f);

            golem.SetActive(false);
            return golem;
        }

        // =========================================================================
        // MODEL 2: PRISM CRYSTAL BAT (Ảnh 2)
        // =========================================================================
        private static GameObject CreatePrismBat(Transform parent, Material bodyMat, Material bellyMat, Material wingMat, Material vortexMat, Material earGlowMat, Material runeBaseMat)
        {
            var bat = new GameObject("Monster_PrismBat");
            if (parent != null) bat.transform.parent = parent;
            bat.transform.localPosition = new Vector3(1.6f, 0.05f, 0f);
            bat.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
            bat.transform.localScale = Vector3.one * 0.92f;

            // Bệ đá ma pháp cổ ngữ dưới đất
            var runeBase = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            runeBase.name = "RunePedestal";
            runeBase.transform.parent = bat.transform;
            runeBase.transform.localPosition = new Vector3(0f, 0.04f, 0f);
            runeBase.transform.localScale = new Vector3(1.1f, 0.04f, 1.1f);
            runeBase.GetComponent<Renderer>().sharedMaterial = runeBaseMat;

            // Thân dơi lơ lửng trong không trung (+0.48m)
            var hoverRoot = new GameObject("HoverBodyRoot");
            hoverRoot.transform.parent = bat.transform;
            hoverRoot.transform.localPosition = new Vector3(0f, 0.68f, 0f);

            // Thân tròn Chibi tím pastel
            var torso = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            torso.name = "BatTorso";
            torso.transform.parent = hoverRoot.transform;
            torso.transform.localPosition = Vector3.zero;
            torso.transform.localScale = new Vector3(0.55f, 0.52f, 0.50f);
            torso.GetComponent<Renderer>().sharedMaterial = bodyMat;

            // Bụng dơi mềm sáng
            var belly = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            belly.name = "BatBelly";
            belly.transform.parent = torso.transform;
            belly.transform.localPosition = new Vector3(0f, -0.05f, 0.35f);
            belly.transform.localScale = new Vector3(0.75f, 0.72f, 0.45f);
            belly.GetComponent<Renderer>().sharedMaterial = bellyMat;

            // Đầu chibi tròn to
            var head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            head.name = "BatHead";
            head.transform.parent = hoverRoot.transform;
            head.transform.localPosition = new Vector3(0f, 0.32f, 0.08f);
            head.transform.localScale = new Vector3(0.52f, 0.48f, 0.48f);
            head.GetComponent<Renderer>().sharedMaterial = bodyMat;

            // Mũi dơi nhỏ
            var snout = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            snout.name = "BatSnout";
            snout.transform.parent = head.transform;
            snout.transform.localPosition = new Vector3(0f, -0.12f, 0.46f);
            snout.transform.localScale = new Vector3(0.22f, 0.14f, 0.18f);
            snout.GetComponent<Renderer>().sharedMaterial = bellyMat;

            // 2 Răng nanh trắng nhỏ xinh xắn
            CreateFang(head.transform, new Vector3(-0.09f, -0.22f, 0.46f), bellyMat);
            CreateFang(head.transform, new Vector3(0.09f, -0.22f, 0.46f), bellyMat);

            // 2 Mắt to tròn đen lay láy
            CreateBatEye(head.transform, new Vector3(-0.16f, 0.05f, 0.44f));
            CreateBatEye(head.transform, new Vector3(0.16f, 0.05f, 0.44f));

            // 2 Tai dơi khổng lồ vểnh cao có lòng tai phát sáng tinh thể
            CreateBatEar(head.transform, new Vector3(-0.25f, 0.38f, 0.05f), new Vector3(15f, -15f, 25f), bodyMat, earGlowMat);
            CreateBatEar(head.transform, new Vector3(0.25f, 0.38f, 0.05f), new Vector3(15f, 15f, -25f), bodyMat, earGlowMat);

            // Đuôi thạch anh nhọn
            var tail = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            tail.name = "BatTail";
            tail.transform.parent = hoverRoot.transform;
            tail.transform.localPosition = new Vector3(0f, -0.38f, -0.15f);
            tail.transform.localRotation = Quaternion.Euler(35f, 0f, 0f);
            tail.transform.localScale = new Vector3(0.06f, 0.18f, 0.06f);
            tail.GetComponent<Renderer>().sharedMaterial = bodyMat;

            // 2 Chân vuốt nhỏ ôm lấy quả cầu ma thuật
            CreateBatClaw(hoverRoot.transform, new Vector3(-0.18f, -0.22f, 0.28f), bodyMat);
            CreateBatClaw(hoverRoot.transform, new Vector3(0.18f, -0.22f, 0.28f), bodyMat);

            // ĐÔI CÁNH PHA LÊ NAN QUẠT ĐA SẮC (Prism Crystal Shard Wings)
            var wingL = CreatePrismWing(hoverRoot.transform, isLeft: true, wingMat);
            var wingR = CreatePrismWing(hoverRoot.transform, isLeft: false, wingMat);

            // QUẢ CẦU MA THUẬT XOÁY TRÒN (Swirling Vortex Orb)
            var orbRoot = new GameObject("SwirlingVortexOrb");
            orbRoot.transform.parent = hoverRoot.transform;
            orbRoot.transform.localPosition = new Vector3(0f, -0.16f, 0.42f);

            var coreSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            coreSphere.name = "VortexCore";
            coreSphere.transform.parent = orbRoot.transform;
            coreSphere.transform.localPosition = Vector3.zero;
            coreSphere.transform.localScale = Vector3.one * 0.46f;
            coreSphere.GetComponent<Renderer>().sharedMaterial = vortexMat;

            // Vòng xoáy năng lượng bao quanh quả cầu
            var swirlRing1 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            swirlRing1.name = "SwirlRing1";
            swirlRing1.transform.parent = orbRoot.transform;
            swirlRing1.transform.localPosition = Vector3.zero;
            swirlRing1.transform.localRotation = Quaternion.Euler(45f, 30f, 0f);
            swirlRing1.transform.localScale = new Vector3(0.58f, 0.02f, 0.58f);
            swirlRing1.GetComponent<Renderer>().sharedMaterial = wingMat;

            var swirlRing2 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            swirlRing2.name = "SwirlRing2";
            swirlRing2.transform.parent = orbRoot.transform;
            swirlRing2.transform.localPosition = Vector3.zero;
            swirlRing2.transform.localRotation = Quaternion.Euler(-35f, 60f, 40f);
            swirlRing2.transform.localScale = new Vector3(0.54f, 0.02f, 0.54f);
            swirlRing2.GetComponent<Renderer>().sharedMaterial = wingMat;

            var rotator = orbRoot.AddComponent<Rotator>();
            rotator.rotationSpeed = new Vector3(45f, 90f, 35f);
            rotator.enableBobbing = true;
            rotator.bobbingSpeed = 2.5f;
            rotator.bobbingHeight = 0.06f;

            // Gắn CombatUnit3D
            var unit = bat.AddComponent<CombatUnit3D>();
            var so = new SerializedObject(unit);
            so.FindProperty("isHero").boolValue = false;
            so.FindProperty("weaponTransform").objectReferenceValue = orbRoot.transform;
            so.ApplyModifiedProperties();
            unit.CacheRenderers();

            // Gắn thanh máu 3D trên đầu
            AttachOverheadHealthBar(bat, "PRISM BAT", 1.35f);

            bat.SetActive(false);
            return bat;
        }

        // =========================================================================
        // MODEL 3: MOSSY TITAN GUARDIAN (Ảnh 3)
        // =========================================================================
        private static GameObject CreateMossyTitanGuardian(Transform parent, Material stoneMat, Material mossMat, Material cyanGlowMat, Material spikeMat)
        {
            var guardian = new GameObject("Monster_MossyTitanGuardian");
            if (parent != null) guardian.transform.parent = parent;
            guardian.transform.localPosition = new Vector3(1.6f, 0.05f, 0f);
            guardian.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
            guardian.transform.localScale = Vector3.one * 1.28f;

            // Bốt đá chân và cẳng chân hộ vệ
            CreateGuardianLeg(guardian.transform, new Vector3(-0.35f, 0.32f, 0f), stoneMat, mossMat);
            CreateGuardianLeg(guardian.transform, new Vector3(0.35f, 0.32f, 0f), stoneMat, mossMat);

            // Thân giáp đá cổ đại khổng lồ (Titan Torso)
            var torso = GameObject.CreatePrimitive(PrimitiveType.Cube);
            torso.name = "GuardianTorso";
            torso.transform.parent = guardian.transform;
            torso.transform.localPosition = new Vector3(0f, 0.85f, 0f);
            torso.transform.localScale = new Vector3(0.95f, 0.72f, 0.75f);
            torso.GetComponent<Renderer>().sharedMaterial = stoneMat;

            // Mảng giáp ngực phủ rêu xanh
            var chestMoss = GameObject.CreatePrimitive(PrimitiveType.Cube);
            chestMoss.name = "ChestMossArmor";
            chestMoss.transform.parent = torso.transform;
            chestMoss.transform.localPosition = new Vector3(0f, 0.12f, 0.35f);
            chestMoss.transform.localScale = new Vector3(0.96f, 0.65f, 0.45f);
            chestMoss.GetComponent<Renderer>().sharedMaterial = mossMat;

            // Lõi ngực năng lượng phát sáng lục giác màu xanh lam (Cyan Power Core)
            var powerCore = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            powerCore.name = "CyanPowerCore";
            powerCore.transform.parent = torso.transform;
            powerCore.transform.localPosition = new Vector3(0f, -0.15f, 0.52f);
            powerCore.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            powerCore.transform.localScale = new Vector3(0.28f, 0.08f, 0.28f);
            powerCore.GetComponent<Renderer>().sharedMaterial = cyanGlowMat;

            // Đầu hiệp sĩ đá cổ đại
            var head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            head.name = "GuardianHead";
            head.transform.parent = guardian.transform;
            head.transform.localPosition = new Vector3(0f, 1.38f, 0.08f);
            head.transform.localScale = new Vector3(0.62f, 0.58f, 0.58f);
            head.GetComponent<Renderer>().sharedMaterial = stoneMat;

            // Lớp rêu phủ trên đỉnh đầu
            var headMoss = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            headMoss.name = "HeadMossTop";
            headMoss.transform.parent = head.transform;
            headMoss.transform.localPosition = new Vector3(0f, 0.22f, 0f);
            headMoss.transform.localScale = new Vector3(0.95f, 0.55f, 0.95f);
            headMoss.GetComponent<Renderer>().sharedMaterial = mossMat;

            // 2 Sừng đá rêu cong lên
            CreateHorn(head.transform, new Vector3(-0.35f, 0.35f, 0f), new Vector3(0f, 0f, 35f), mossMat);
            CreateHorn(head.transform, new Vector3(0.35f, 0.35f, 0f), new Vector3(0f, 0f, -35f), mossMat);

            // 2 Mắt tròn phát sáng xanh huỳnh quang (Circular Cyan Energy Eyes)
            var eyeL = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            eyeL.transform.parent = head.transform;
            eyeL.transform.localPosition = new Vector3(-0.16f, 0.02f, 0.44f);
            eyeL.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            eyeL.transform.localScale = new Vector3(0.14f, 0.04f, 0.14f);
            eyeL.GetComponent<Renderer>().sharedMaterial = cyanGlowMat;

            var eyeR = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            eyeR.transform.parent = head.transform;
            eyeR.transform.localPosition = new Vector3(0.16f, 0.02f, 0.44f);
            eyeR.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            eyeR.transform.localScale = new Vector3(0.14f, 0.04f, 0.14f);
            eyeR.GetComponent<Renderer>().sharedMaterial = cyanGlowMat;

            // 2 Cầu vai phủ rêu có gai tinh thể xanh ngọc
            CreateGuardianPauldron(guardian.transform, new Vector3(-0.68f, 1.08f, 0f), isLeft: true, stoneMat, mossMat, spikeMat);
            CreateGuardianPauldron(guardian.transform, new Vector3(0.68f, 1.08f, 0f), isLeft: false, stoneMat, mossMat, spikeMat);

            // TAY PHẢI: CẦM CHÙY GAI PHA LÊ KHỔNG LỒ (Crystal Spiked Morningstar / Mace)
            var armR = new GameObject("MaceArm");
            armR.transform.parent = guardian.transform;
            armR.transform.localPosition = new Vector3(0.65f, 0.72f, 0.15f);

            var armMesh = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            armMesh.transform.parent = armR.transform;
            armMesh.transform.localPosition = new Vector3(0f, 0f, 0f);
            armMesh.transform.localRotation = Quaternion.Euler(45f, 0f, 0f);
            armMesh.transform.localScale = new Vector3(0.22f, 0.38f, 0.22f);
            armMesh.GetComponent<Renderer>().sharedMaterial = stoneMat;

            var maceShaft = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            maceShaft.name = "MaceShaft";
            maceShaft.transform.parent = armR.transform;
            maceShaft.transform.localPosition = new Vector3(0f, -0.2f, 0.35f);
            maceShaft.transform.localRotation = Quaternion.Euler(65f, 0f, 0f);
            maceShaft.transform.localScale = new Vector3(0.10f, 0.65f, 0.10f);
            maceShaft.GetComponent<Renderer>().sharedMaterial = stoneMat;

            var maceHead = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            maceHead.name = "MaceHead";
            maceHead.transform.parent = maceShaft.transform;
            maceHead.transform.localPosition = new Vector3(0f, 0.85f, 0f);
            maceHead.transform.localScale = new Vector3(3.5f, 0.55f, 3.5f);
            maceHead.GetComponent<Renderer>().sharedMaterial = stoneMat;

            // Đính 8 gai pha lê sắc nhọn quanh đầu chùy
            CreateMaceSpikes(maceHead.transform, spikeMat);

            // TAY TRÁI: MANG KHIÊN ĐÁ CỔ ĐẠI (Ancient Engraved Shield)
            var armL = new GameObject("ShieldArm");
            armL.transform.parent = guardian.transform;
            armL.transform.localPosition = new Vector3(-0.65f, 0.72f, 0.15f);

            var shieldBody = GameObject.CreatePrimitive(PrimitiveType.Cube);
            shieldBody.name = "AncientShieldBody";
            shieldBody.transform.parent = armL.transform;
            shieldBody.transform.localPosition = new Vector3(-0.1f, 0f, 0.25f);
            shieldBody.transform.localRotation = Quaternion.Euler(0f, -15f, 0f);
            shieldBody.transform.localScale = new Vector3(0.12f, 0.95f, 0.65f);
            shieldBody.GetComponent<Renderer>().sharedMaterial = stoneMat;

            // Rêu phủ mép khiên
            var shieldMoss = GameObject.CreatePrimitive(PrimitiveType.Cube);
            shieldMoss.name = "ShieldMossTrim";
            shieldMoss.transform.parent = shieldBody.transform;
            shieldMoss.transform.localPosition = new Vector3(0.1f, 0.35f, 0f);
            shieldMoss.transform.localScale = new Vector3(1.1f, 0.32f, 1.05f);
            shieldMoss.GetComponent<Renderer>().sharedMaterial = mossMat;

            // Vòng tròn cổ ngữ ở tâm khiên
            var shieldBoss = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            shieldBoss.name = "ShieldRuneBoss";
            shieldBoss.transform.parent = shieldBody.transform;
            shieldBoss.transform.localPosition = new Vector3(0.55f, 0f, 0f);
            shieldBoss.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            shieldBoss.transform.localScale = new Vector3(0.52f, 0.15f, 0.52f);
            shieldBoss.GetComponent<Renderer>().sharedMaterial = cyanGlowMat;

            // Gắn CombatUnit3D
            var unit = guardian.AddComponent<CombatUnit3D>();
            var so = new SerializedObject(unit);
            so.FindProperty("isHero").boolValue = false;
            so.FindProperty("weaponTransform").objectReferenceValue = armR.transform;
            so.ApplyModifiedProperties();
            unit.CacheRenderers();

            // Gắn thanh máu 3D trên đầu
            AttachOverheadHealthBar(guardian, "MOSSY TITAN", 1.85f);

            guardian.SetActive(false);
            return guardian;
        }

        // =========================================================================
        // HELPER BUILDERS & GEOMETRY
        // =========================================================================
        private static void CreateStoneLeg(Transform parent, Vector3 localPos, Material mat)
        {
            var leg = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leg.transform.parent = parent;
            leg.transform.localPosition = localPos;
            leg.transform.localScale = new Vector3(0.28f, 0.40f, 0.32f);
            leg.GetComponent<Renderer>().sharedMaterial = mat;

            var foot = GameObject.CreatePrimitive(PrimitiveType.Cube);
            foot.transform.parent = leg.transform;
            foot.transform.localPosition = new Vector3(0f, -0.45f, 0.25f);
            foot.transform.localScale = new Vector3(1.15f, 0.45f, 1.45f);
            foot.GetComponent<Renderer>().sharedMaterial = mat;
        }

        private static GameObject CreateStoneArm(Transform parent, Vector3 localPos, int side, Material mat)
        {
            var arm = new GameObject(side < 0 ? "ArmL" : "ArmR");
            arm.transform.parent = parent;
            arm.transform.localPosition = localPos;

            var shoulder = GameObject.CreatePrimitive(PrimitiveType.Cube);
            shoulder.transform.parent = arm.transform;
            shoulder.transform.localPosition = Vector3.zero;
            shoulder.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);
            shoulder.GetComponent<Renderer>().sharedMaterial = mat;

            var fist = GameObject.CreatePrimitive(PrimitiveType.Cube);
            fist.name = side < 0 ? "FistL" : "FistR";
            fist.transform.parent = arm.transform;
            fist.transform.localPosition = new Vector3(0f, -0.38f, 0.18f);
            fist.transform.localScale = new Vector3(0.38f, 0.42f, 0.42f);
            fist.GetComponent<Renderer>().sharedMaterial = mat;

            return arm;
        }

        private static void CreateHorn(Transform parent, Vector3 localPos, Vector3 localRot, Material mat)
        {
            var horn = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            horn.transform.parent = parent;
            horn.transform.localPosition = localPos;
            horn.transform.localRotation = Quaternion.Euler(localRot);
            horn.transform.localScale = new Vector3(0.12f, 0.26f, 0.12f);
            horn.GetComponent<Renderer>().sharedMaterial = mat;
        }

        private static void CreateGlowingEye(Transform parent, Vector3 localPos, Vector3 scale, Material mat)
        {
            var eye = GameObject.CreatePrimitive(PrimitiveType.Cube);
            eye.name = "GlowingEye";
            eye.transform.parent = parent;
            eye.transform.localPosition = localPos;
            eye.transform.localScale = scale;
            eye.GetComponent<Renderer>().sharedMaterial = mat;
        }

        private static void CreateCrystalSpire(Transform parent, Vector3 localPos, Vector3 localRot, Vector3 scale, Material mat)
        {
            var spire = new GameObject("CrystalSpire");
            spire.transform.parent = parent;
            spire.transform.localPosition = localPos;
            spire.transform.localRotation = Quaternion.Euler(localRot);
            spire.transform.localScale = scale;

            var meshFilter = spire.AddComponent<MeshFilter>();
            var meshRenderer = spire.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = mat;

            // Sử dụng HexagonalQuartzMesh nếu có sẵn, hoặc cylinder đa giác vát đỉnh
            var hexMesh = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Data/HexagonalQuartzMesh.asset");
            if (hexMesh != null)
            {
                meshFilter.sharedMesh = hexMesh;
            }
            else
            {
                var fallback = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                meshFilter.sharedMesh = fallback.GetComponent<MeshFilter>().sharedMesh;
                Object.DestroyImmediate(fallback);
            }
        }

        private static void CreateFang(Transform parent, Vector3 localPos, Material mat)
        {
            var fang = GameObject.CreatePrimitive(PrimitiveType.Cube);
            fang.transform.parent = parent;
            fang.transform.localPosition = localPos;
            fang.transform.localRotation = Quaternion.Euler(45f, 0f, 0f);
            fang.transform.localScale = new Vector3(0.04f, 0.08f, 0.04f);
            fang.GetComponent<Renderer>().sharedMaterial = mat;
        }

        private static void CreateBatEye(Transform parent, Vector3 localPos)
        {
            var eye = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            eye.transform.parent = parent;
            eye.transform.localPosition = localPos;
            eye.transform.localScale = Vector3.one * 0.12f;

            var blackMat = GetOrCreateMaterial("Assets/Materials/Mat_Bat_EyeBlack.mat", new Color(0.08f, 0.05f, 0.12f, 1f), 0.9f, Color.black);
            eye.GetComponent<Renderer>().sharedMaterial = blackMat;

            // Điểm sáng mắt Specular nhỏ
            var spec = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            spec.transform.parent = eye.transform;
            spec.transform.localPosition = new Vector3(0.25f, 0.25f, 0.45f);
            spec.transform.localScale = Vector3.one * 0.35f;
            var whiteMat = GetOrCreateMaterial("Assets/Materials/Mat_Bat_EyeSpecular.mat", Color.white, 0.95f, Color.white * 1.5f);
            spec.GetComponent<Renderer>().sharedMaterial = whiteMat;
        }

        private static void CreateBatEar(Transform parent, Vector3 localPos, Vector3 localRot, Material outerMat, Material innerMat)
        {
            var ear = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ear.name = "BatEar";
            ear.transform.parent = parent;
            ear.transform.localPosition = localPos;
            ear.transform.localRotation = Quaternion.Euler(localRot);
            ear.transform.localScale = new Vector3(0.14f, 0.32f, 0.08f);
            ear.GetComponent<Renderer>().sharedMaterial = outerMat;

            var inner = GameObject.CreatePrimitive(PrimitiveType.Cube);
            inner.name = "InnerEarCrystalGlow";
            inner.transform.parent = ear.transform;
            inner.transform.localPosition = new Vector3(0f, 0f, 0.35f);
            inner.transform.localScale = new Vector3(0.65f, 0.75f, 0.35f);
            inner.GetComponent<Renderer>().sharedMaterial = innerMat;
        }

        private static void CreateBatClaw(Transform parent, Vector3 localPos, Material mat)
        {
            var claw = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            claw.transform.parent = parent;
            claw.transform.localPosition = localPos;
            claw.transform.localScale = new Vector3(0.12f, 0.09f, 0.15f);
            claw.GetComponent<Renderer>().sharedMaterial = mat;
        }

        private static GameObject CreatePrismWing(Transform parent, bool isLeft, Material wingMat)
        {
            var wingRoot = new GameObject(isLeft ? "PrismWingL" : "PrismWingR");
            wingRoot.transform.parent = parent;
            wingRoot.transform.localPosition = new Vector3(isLeft ? -0.28f : 0.28f, 0.12f, -0.05f);

            float sideSign = isLeft ? -1f : 1f;

            // Khung xương cánh chính
            var arm = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            arm.transform.parent = wingRoot.transform;
            arm.transform.localPosition = new Vector3(sideSign * 0.35f, 0.25f, 0f);
            arm.transform.localRotation = Quaternion.Euler(0f, 0f, sideSign * -55f);
            arm.transform.localScale = new Vector3(0.06f, 0.42f, 0.06f);
            arm.GetComponent<Renderer>().sharedMaterial = wingMat;

            // 4 Nan pha lê xòe quạt (Crystal Shards)
            float[] shardAngles = new float[] { 15f, -10f, -35f, -58f };
            float[] shardLengths = new float[] { 0.48f, 0.62f, 0.55f, 0.40f };

            for (int i = 0; i < 4; i++)
            {
                var shard = GameObject.CreatePrimitive(PrimitiveType.Cube);
                shard.name = $"CrystalShard_{i}";
                shard.transform.parent = wingRoot.transform;
                shard.transform.localPosition = new Vector3(sideSign * (0.35f + i * 0.12f), 0.15f - i * 0.08f, 0f);
                shard.transform.localRotation = Quaternion.Euler(0f, sideSign * 15f, sideSign * shardAngles[i]);
                shard.transform.localScale = new Vector3(0.14f, shardLengths[i], 0.035f);
                shard.GetComponent<Renderer>().sharedMaterial = wingMat;
            }

            return wingRoot;
        }

        private static void CreateGuardianLeg(Transform parent, Vector3 localPos, Material stoneMat, Material mossMat)
        {
            var leg = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leg.transform.parent = parent;
            leg.transform.localPosition = localPos;
            leg.transform.localScale = new Vector3(0.36f, 0.55f, 0.38f);
            leg.GetComponent<Renderer>().sharedMaterial = stoneMat;

            var mossTrim = GameObject.CreatePrimitive(PrimitiveType.Cube);
            mossTrim.transform.parent = leg.transform;
            mossTrim.transform.localPosition = new Vector3(0f, 0.32f, 0f);
            mossTrim.transform.localScale = new Vector3(1.1f, 0.35f, 1.1f);
            mossTrim.GetComponent<Renderer>().sharedMaterial = mossMat;

            var boot = GameObject.CreatePrimitive(PrimitiveType.Cube);
            boot.transform.parent = leg.transform;
            boot.transform.localPosition = new Vector3(0f, -0.45f, 0.22f);
            boot.transform.localScale = new Vector3(1.15f, 0.45f, 1.45f);
            boot.GetComponent<Renderer>().sharedMaterial = stoneMat;
        }

        private static void CreateGuardianPauldron(Transform parent, Vector3 localPos, bool isLeft, Material stoneMat, Material mossMat, Material spikeMat)
        {
            var pauldron = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pauldron.name = isLeft ? "PauldronL" : "PauldronR";
            pauldron.transform.parent = parent;
            pauldron.transform.localPosition = localPos;
            pauldron.transform.localRotation = Quaternion.Euler(0f, 0f, isLeft ? 20f : -20f);
            pauldron.transform.localScale = new Vector3(0.48f, 0.38f, 0.48f);
            pauldron.GetComponent<Renderer>().sharedMaterial = mossMat;

            // 2 Gai tinh thể lam nhọn nhô lên từ cầu vai
            var spike1 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            spike1.transform.parent = pauldron.transform;
            spike1.transform.localPosition = new Vector3(0f, 0.65f, -0.15f);
            spike1.transform.localRotation = Quaternion.Euler(-15f, 0f, 0f);
            spike1.transform.localScale = new Vector3(0.25f, 0.65f, 0.25f);
            spike1.GetComponent<Renderer>().sharedMaterial = spikeMat;

            var spike2 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            spike2.transform.parent = pauldron.transform;
            spike2.transform.localPosition = new Vector3(0f, 0.65f, 0.15f);
            spike2.transform.localRotation = Quaternion.Euler(15f, 0f, 0f);
            spike2.transform.localScale = new Vector3(0.22f, 0.55f, 0.22f);
            spike2.GetComponent<Renderer>().sharedMaterial = spikeMat;
        }

        private static void CreateMaceSpikes(Transform maceHead, Material spikeMat)
        {
            Vector3[] dirs = new Vector3[]
            {
                Vector3.up, Vector3.down,
                Vector3.left, Vector3.right,
                Vector3.forward, Vector3.back,
                (Vector3.up + Vector3.left).normalized,
                (Vector3.up + Vector3.right).normalized
            };

            for (int i = 0; i < dirs.Length; i++)
            {
                var spike = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                spike.name = $"MaceSpike_{i}";
                spike.transform.parent = maceHead;
                spike.transform.localPosition = dirs[i] * 0.55f;
                spike.transform.localRotation = Quaternion.FromToRotation(Vector3.up, dirs[i]);
                spike.transform.localScale = new Vector3(0.18f, 0.55f, 0.18f);
                spike.GetComponent<Renderer>().sharedMaterial = spikeMat;
            }
        }

        private static void AttachOverheadHealthBar(GameObject unitObj, string monsterName, float headOffset)
        {
            var oldBar = unitObj.transform.Find("OverheadHealthBar");
            if (oldBar != null) Object.DestroyImmediate(oldBar.gameObject);

            var barObj = new GameObject("OverheadHealthBar", typeof(RectTransform));
            barObj.transform.SetParent(unitObj.transform, false);
            barObj.transform.localPosition = new Vector3(0f, headOffset, 0f);
            barObj.transform.localRotation = Quaternion.identity;
            barObj.transform.localScale = Vector3.one * 0.008f;

            var rt = barObj.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(110f, 20f);
            rt.pivot = new Vector2(0.5f, 0.5f);

            var canvas = barObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = Camera.main;
            canvas.sortingOrder = 50;

            var cg = barObj.AddComponent<CanvasGroup>();
            cg.alpha = 1f;

            var bgObj = CreateUIChild("BgFrame", barObj.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            var bgImg = bgObj.AddComponent<Image>();
            bgImg.color = new Color(0.04f, 0.03f, 0.08f, 0.92f);
            bgImg.raycastTarget = false;

            var delObj = CreateUIChild("DelayedFill", barObj.transform, Vector2.zero, Vector2.one, new Vector2(1.5f, 1.5f), new Vector2(-1.5f, -1.5f));
            var delImg = delObj.AddComponent<Image>();
            delImg.color = new Color(1f, 0.9f, 0.55f, 0.85f);
            delImg.raycastTarget = false;
            var delRt = delObj.GetComponent<RectTransform>();

            var mainObj = CreateUIChild("MainFill", barObj.transform, Vector2.zero, Vector2.one, new Vector2(1.5f, 1.5f), new Vector2(-1.5f, -1.5f));
            var mainImg = mainObj.AddComponent<Image>();
            Color hpColor = new Color(1f, 0.22f, 0.28f, 1f);
            mainImg.color = hpColor;
            mainImg.raycastTarget = false;
            var mainRt = mainObj.GetComponent<RectTransform>();

            var textObj = CreateUIChild("HpText", barObj.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            var tmpText = textObj.AddComponent<TextMeshProUGUI>();
            tmpText.text = monsterName;
            tmpText.fontSize = 11.5f;
            tmpText.fontStyle = FontStyles.Bold;
            tmpText.color = Color.white;
            tmpText.alignment = TextAlignmentOptions.Center;
            tmpText.raycastTarget = false;

            var overhead = barObj.AddComponent<OverheadHealthBar3D>();
            var so = new SerializedObject(overhead);
            so.FindProperty("barOwner").enumValueIndex = (int)HealthBarOwner.Monster;
            so.FindProperty("isHero").boolValue = false;
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

        private static void DestroyOldMonster(Transform parent, string name)
        {
            var old = parent.Find(name);
            if (old != null) Object.DestroyImmediate(old.gameObject);
        }

        private static void SaveAsPrefab(GameObject go, string prefabPath)
        {
            PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
            Debug.Log($"<color=#70FF00>Saved Prefab: {prefabPath}</color>");
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
