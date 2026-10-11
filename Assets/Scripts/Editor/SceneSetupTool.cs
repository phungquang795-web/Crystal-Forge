#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using CrystalForge.Core;
using CrystalForge.Data;
using CrystalForge.Economy;
using CrystalForge.Utils;
using CrystalForge.Visuals;

namespace CrystalForge.Editor
{
    public static class SceneSetupTool
    {
        [MenuItem("CrystalForge/1. Setup 3D Diorama & Core Scene")]
        public static void BuildDioramaAndCoreScene()
        {
            Undo.SetCurrentGroupName("Setup Crystal Forge 3D Scene");
            int undoGroup = Undo.GetCurrentGroup();

            // 1. Tạo thư mục chứa Asset nếu chưa có
            EnsureFolders();

            // 2. Tạo hoặc nạp các ScriptableObject dữ liệu kinh tế
            GetOrCreateGeneratorData(GeneratorType.CrystalDrill, "Crystal Drill", 100.0, 1.15f, 1.0, 100.0);
            GetOrCreateGeneratorData(GeneratorType.ManaFurnace, "Mana Furnace", 2500.0, 1.15f, 8.0, 25000.0);
            GetOrCreateGeneratorData(GeneratorType.AutoExtractor, "Auto Extractor", 25000.0, 1.15f, 50.0, 250000.0);
            GetOrCreateTapPowerData();
            CreateOrLoadMilestones();

            // 3. Xây dựng Thánh Điện Hầm Ngục Cổ Đại, Trụ Tinh Thể Kim Cương & Player Thợ Mỏ chuẩn Concept Art
            AncientSanctuarySetupTool.BuildSanctuaryAndMinerHero();

            Undo.CollapseUndoOperations(undoGroup);
        }

        public static void BuildDioramaLegacy()
        {
            Undo.SetCurrentGroupName("Setup Legacy Diorama Scene");
            int undoGroup = Undo.GetCurrentGroup();

            var drillData = GetOrCreateGeneratorData(GeneratorType.CrystalDrill, "Crystal Drill", 100.0, 1.15f, 1.0, 100.0);
            var furnaceData = GetOrCreateGeneratorData(GeneratorType.ManaFurnace, "Mana Furnace", 2500.0, 1.15f, 8.0, 25000.0);
            var extractorData = GetOrCreateGeneratorData(GeneratorType.AutoExtractor, "Auto Extractor", 25000.0, 1.15f, 50.0, 250000.0);
            var tapPowerData = GetOrCreateTapPowerData();
            var milestones = CreateOrLoadMilestones();

            // 3. Tạo Materials chuẩn PBR URP sắc nét & rực rỡ
            var stoneMat = GetOrCreateMaterial("Mat_ForgeStone", new Color(0.08f, 0.09f, 0.14f), 0.58f, false, null, 0.22f);
            var trimMat = GetOrCreateMaterial("Mat_GoldTrim", new Color(1.0f, 0.78f, 0.22f), 0.94f, true, new Color(0.6f, 0.38f, 0.08f) * 1.5f, 0.95f);
            var crystalMat = GetOrCreateMaterial("Mat_AmethystCrystal", new Color(0.88f, 0.28f, 1.0f), 0.96f, true, new Color(0.95f, 0.38f, 1.0f) * 3.6f, 0.1f);
            var innerCrystalMat = GetOrCreateMaterial("Mat_CrystalCore_Inner", new Color(1.0f, 0.92f, 1.0f), 0.98f, true, new Color(1.0f, 0.85f, 1.0f) * 5.2f, 0.05f);
            var machineMat = GetOrCreateMaterial("Mat_MachineBronze", new Color(0.48f, 0.34f, 0.22f), 0.55f, false, null, 0.65f);
            var cyanMat = GetOrCreateMaterial("Mat_RunicCyan", new Color(0.05f, 0.88f, 1.0f), 0.95f, true, new Color(0.1f, 0.92f, 1.0f) * 3.5f, 0.15f);
            var anvilMat = GetOrCreateMaterial("Mat_AnvilDark", new Color(0.12f, 0.13f, 0.18f), 0.72f, false, null, 0.85f);
            var ingotMat = GetOrCreateMaterial("Mat_GlowingIngot", new Color(1.0f, 0.42f, 0.05f), 0.88f, true, new Color(1.0f, 0.52f, 0.05f) * 3.8f, 0.3f);
            var woodMat = GetOrCreateMaterial("Mat_WoodOak", new Color(0.32f, 0.20f, 0.12f), 0.35f, false, null, 0.05f);
            var steelMat = GetOrCreateMaterial("Mat_WeaponSteel", new Color(0.55f, 0.60f, 0.68f), 0.85f, true, new Color(0.2f, 0.35f, 0.55f) * 0.9f, 0.9f);

            // Vật liệu đặc tả cho Trụ Thạch Anh Lục Giác Cute & Rễ Cây Đơm Hoa (Mẫu 1 - Chuẩn Ảnh Thực Tế)
            var woodCedarMat = GetOrCreateMaterial("Mat_WoodCedar", new Color(0.38f, 0.20f, 0.12f), 0.68f, false, null, 0.40f);
            var woodVineMat = GetOrCreateMaterial("Mat_WoodVine", new Color(0.46f, 0.28f, 0.16f), 0.52f, false, null, 0.20f);
            var runeGoldMat = GetOrCreateMaterial("Mat_RuneGold", new Color(1.0f, 0.88f, 0.35f), 0.95f, true, new Color(1.0f, 0.85f, 0.28f) * 4.8f, 0.85f);
            var quartzCrystalMat = GetOrCreateMaterial("Mat_QuartzCrystal", new Color(0.96f, 0.98f, 1.0f), 0.96f, true, new Color(0.30f, 0.65f, 0.90f) * 1.8f, 0.05f);
            var quartzSunGlowMat = GetOrCreateMaterial("Mat_QuartzSunGlow", new Color(1.0f, 0.85f, 0.35f), 0.98f, true, new Color(1.0f, 0.80f, 0.25f) * 5.5f, 0.02f);
            var quartzAquaGlowMat = GetOrCreateMaterial("Mat_QuartzAquaGlow", new Color(0.20f, 0.95f, 1.0f), 0.98f, true, new Color(0.15f, 0.92f, 1.0f) * 5.2f, 0.02f);
            var peachBlossomMat = GetOrCreateMaterial("Mat_PeachBlossom", new Color(1.0f, 0.72f, 0.82f), 0.85f, true, new Color(1.0f, 0.68f, 0.78f) * 1.8f, 0.10f);
            var goldBlossomMat = GetOrCreateMaterial("Mat_GoldBlossom", new Color(1.0f, 0.86f, 0.42f), 0.85f, true, new Color(1.0f, 0.82f, 0.35f) * 1.8f, 0.10f);
            var flowerCenterMat = GetOrCreateMaterial("Mat_FlowerCenter", new Color(1.0f, 0.95f, 0.65f), 0.95f, true, new Color(1.0f, 0.92f, 0.55f) * 4.2f, 0.20f);
            var sproutGreenMat = GetOrCreateMaterial("Mat_SproutGreen", new Color(0.40f, 0.92f, 0.45f), 0.85f, true, new Color(0.40f, 0.92f, 0.45f) * 2.2f, 0.15f);
            var emeraldMossMat = GetOrCreateMaterial("Mat_EmeraldMoss", new Color(0.2f, 1.0f, 0.55f), 0.92f, true, new Color(0.2f, 1.0f, 0.55f) * 4.0f, 0.1f);
            var auroraMat = GetOrCreateMaterial("Mat_AuroraHalo", new Color(0.40f, 0.92f, 1.0f), 0.96f, true, new Color(0.30f, 0.95f, 1.0f) * 4.5f, 0.05f);

            // 4. Thiết lập Camera góc 3/4 Isometric
            SetupCamera();

            // 5. Thiết lập Ánh sáng 3-Point Cinematic Lighting
            SetupLighting(crystalMat);

            // 6. Cập nhật Post-Processing Profile (Bloom, ColorAdjustments, Vignette)
            SetupPostProcessing();

            // 7. Xóa các đối tượng cũ nếu đã tồn tại để tránh trùng lặp
            CleanOldSceneObjects();

            // 8. Tạo Map Diorama Workshop Base (Floating Sky Island)
            var environmentRoot = new GameObject("Environment_Diorama");
            Undo.RegisterCreatedObjectUndo(environmentRoot, "Create Environment Root");

            // A. Hệ thống Vũ trụ & Bụi sao không gian sâu (Cosmic Starfield VFX)
            CreateCosmicStarfield(environmentRoot.transform);

            // B. Các tiểu hành tinh / phiến đá cổ lơ lửng ngoài không gian (Floating Asteroids)
            CreateFloatingAsteroids(environmentRoot.transform, stoneMat, crystalMat);

            // C. Sàn đảo nổi chính (Phiến đá hoa cương cổ ngữ)
            var floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "FloatingIsland_Floor";
            floor.transform.parent = environmentRoot.transform;
            floor.transform.position = new Vector3(0f, -0.2f, 0f);
            floor.transform.localScale = new Vector3(8.8f, 0.4f, 7.8f);
            floor.GetComponent<Renderer>().sharedMaterial = stoneMat;

            // Đường viền kim loại vàng hoàng kim dưới mép sàn đá
            var trim = GameObject.CreatePrimitive(PrimitiveType.Cube);
            trim.name = "Floor_GoldTrim";
            trim.transform.parent = environmentRoot.transform;
            trim.transform.position = new Vector3(0f, -0.28f, 0f);
            trim.transform.localScale = new Vector3(9.2f, 0.35f, 8.2f);
            trim.GetComponent<Renderer>().sharedMaterial = trimMat;

            // D. Trận đồ Cổ ngữ Kép Xoay Động (Animated Dual-Rune Magic Circles)
            var runicCircle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            runicCircle.name = "Floor_RunicCircle";
            runicCircle.transform.parent = environmentRoot.transform;
            runicCircle.transform.position = new Vector3(0f, 0.005f, 0f);
            runicCircle.transform.localScale = new Vector3(5.2f, 0.005f, 5.2f);
            runicCircle.GetComponent<Renderer>().sharedMaterial = trimMat;
            Object.DestroyImmediate(runicCircle.GetComponent<Collider>());
            var outerRotator = runicCircle.AddComponent<RunicCircleRotator>();
            var outerSo = new SerializedObject(outerRotator);
            outerSo.FindProperty("rotationSpeed").floatValue = 5.5f;
            outerSo.FindProperty("rotateClockwise").boolValue = true;
            outerSo.ApplyModifiedProperties();

            // 4 Nút ấn cổ ngữ trên vòng tròn ngoài
            for (int i = 0; i < 4; i++)
            {
                float angle = i * 90f * Mathf.Deg2Rad;
                var runeNode = GameObject.CreatePrimitive(PrimitiveType.Cube);
                runeNode.name = $"Floor_RuneNode_{i}";
                runeNode.transform.parent = runicCircle.transform;
                runeNode.transform.localPosition = new Vector3(Mathf.Cos(angle) * 0.46f, 0.6f, Mathf.Sin(angle) * 0.46f);
                runeNode.transform.localRotation = Quaternion.Euler(0f, 45f, 0f);
                runeNode.transform.localScale = new Vector3(0.08f, 0.5f, 0.08f);
                runeNode.GetComponent<Renderer>().sharedMaterial = cyanMat;
                Object.DestroyImmediate(runeNode.GetComponent<Collider>());
            }

            var innerGlowCircle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            innerGlowCircle.name = "Floor_InnerGlowCircle";
            innerGlowCircle.transform.parent = environmentRoot.transform;
            innerGlowCircle.transform.position = new Vector3(0f, 0.008f, 0f);
            innerGlowCircle.transform.localScale = new Vector3(3.4f, 0.005f, 3.4f);
            innerGlowCircle.GetComponent<Renderer>().sharedMaterial = cyanMat;
            Object.DestroyImmediate(innerGlowCircle.GetComponent<Collider>());
            var innerRotator = innerGlowCircle.AddComponent<RunicCircleRotator>();
            var innerSo = new SerializedObject(innerRotator);
            innerSo.FindProperty("rotationSpeed").floatValue = 8.5f;
            innerSo.FindProperty("rotateClockwise").boolValue = false;
            innerSo.FindProperty("subtleBobbing").boolValue = true;
            innerSo.ApplyModifiedProperties();

            // Đáy đá nhũ treo lơ lửng dưới đáy đảo
            CreateIslandRoots(environmentRoot.transform, stoneMat);

            // --- BỆ ĐÁ BÁT GIÁC GỖ GỤ 2 TẦNG (Octagonal Stepped Wooden Stand) ---
            var pedestal = new GameObject("Central_Pedestal");
            pedestal.transform.parent = environmentRoot.transform;
            pedestal.transform.position = new Vector3(0f, 0.08f, 0f);

            // Tầng 1: Đế gỗ bát giác chính (2 khối hộp vuông giao nhau 45 độ)
            var basePart1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            basePart1.name = "Pedestal_OctagonBase_A";
            basePart1.transform.parent = pedestal.transform;
            basePart1.transform.localPosition = new Vector3(0f, 0.14f, 0f);
            basePart1.transform.localScale = new Vector3(2.15f, 0.28f, 2.15f);
            basePart1.GetComponent<Renderer>().sharedMaterial = woodCedarMat;
            Object.DestroyImmediate(basePart1.GetComponent<Collider>());

            var basePart2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            basePart2.name = "Pedestal_OctagonBase_B";
            basePart2.transform.parent = pedestal.transform;
            basePart2.transform.localPosition = new Vector3(0f, 0.14f, 0f);
            basePart2.transform.localRotation = Quaternion.Euler(0f, 45f, 0f);
            basePart2.transform.localScale = new Vector3(2.15f, 0.28f, 2.15f);
            basePart2.GetComponent<Renderer>().sharedMaterial = woodCedarMat;
            Object.DestroyImmediate(basePart2.GetComponent<Collider>());

            // Viền vàng chân đế hoàng kim
            var baseTrim = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            baseTrim.name = "Pedestal_GoldTrimBase";
            baseTrim.transform.parent = pedestal.transform;
            baseTrim.transform.localPosition = new Vector3(0f, 0.01f, 0f);
            baseTrim.transform.localScale = new Vector3(2.25f, 0.03f, 2.25f);
            baseTrim.GetComponent<Renderer>().sharedMaterial = trimMat;
            Object.DestroyImmediate(baseTrim.GetComponent<Collider>());

            // 8 Cổ ngữ vàng phát sáng khảm trên 8 mặt đứng của đế gỗ bát giác (Glowing Golden Runes)
            for (int r = 0; r < 8; r++)
            {
                float angle = r * 45f;
                float rad = angle * Mathf.Deg2Rad;
                Vector3 runePos = new Vector3(Mathf.Sin(rad) * 1.085f, 0.14f, Mathf.Cos(rad) * 1.085f);
                Quaternion runeRot = Quaternion.Euler(0f, angle, 0f);
                CreatePedestalRune(pedestal.transform, runePos, runeRot, r, runeGoldMat);
            }

            // Tầng 2: Đế gỗ giật cấp bên trên (Stepped Upper Tier)
            var stepPart1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stepPart1.name = "Pedestal_OctagonStep_A";
            stepPart1.transform.parent = pedestal.transform;
            stepPart1.transform.localPosition = new Vector3(0f, 0.35f, 0f);
            stepPart1.transform.localScale = new Vector3(1.72f, 0.16f, 1.72f);
            stepPart1.GetComponent<Renderer>().sharedMaterial = woodCedarMat;
            Object.DestroyImmediate(stepPart1.GetComponent<Collider>());

            var stepPart2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stepPart2.name = "Pedestal_OctagonStep_B";
            stepPart2.transform.parent = pedestal.transform;
            stepPart2.transform.localPosition = new Vector3(0f, 0.35f, 0f);
            stepPart2.transform.localRotation = Quaternion.Euler(0f, 45f, 0f);
            stepPart2.transform.localScale = new Vector3(1.72f, 0.16f, 1.72f);
            stepPart2.GetComponent<Renderer>().sharedMaterial = woodCedarMat;
            Object.DestroyImmediate(stepPart2.GetComponent<Collider>());

            // Viền ren vàng giữa 2 tầng
            var stepTrim = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            stepTrim.name = "Pedestal_GoldTrimStep";
            stepTrim.transform.parent = pedestal.transform;
            stepTrim.transform.localPosition = new Vector3(0f, 0.28f, 0f);
            stepTrim.transform.localScale = new Vector3(1.80f, 0.025f, 1.80f);
            stepTrim.GetComponent<Renderer>().sharedMaterial = trimMat;
            Object.DestroyImmediate(stepTrim.GetComponent<Collider>());

            // Vành chỉ vàng gờ trên cùng của đế gỗ
            var topTrim = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            topTrim.name = "Pedestal_GoldTrimTop";
            topTrim.transform.parent = pedestal.transform;
            topTrim.transform.localPosition = new Vector3(0f, 0.43f, 0f);
            topTrim.transform.localScale = new Vector3(1.55f, 0.02f, 1.55f);
            topTrim.GetComponent<Renderer>().sharedMaterial = trimMat;
            Object.DestroyImmediate(topTrim.GetComponent<Collider>());

            // Đe rèn Thần Thoại & Phôi Ngọc Nóng Đỏ Rực Lửa (Đặt trang trọng bên cạnh bệ đá)
            CreateAnvilProp(environmentRoot.transform, new Vector3(-1.45f, 0.12f, 0.45f), Quaternion.Euler(0f, 32f, 0f), anvilMat, trimMat, ingotMat);

            // Props Xưởng Lò Rèn Thần Thoại: Giá Vũ Khí & Rương Quặng Pha Lê
            CreateBlacksmithWeaponRackProp(environmentRoot.transform, woodMat, trimMat, steelMat, crystalMat);
            CreateCrystalOreCrateProp(environmentRoot.transform, woodMat, trimMat, crystalMat, cyanMat);

            // 9. Tạo Crystal Core 3D - Mẫu 1: Sprout Vine Hexagonal Quartz Obelisk (Click Target)
            var crystalCoreObj = new GameObject("CrystalCore");
            Undo.RegisterCreatedObjectUndo(crystalCoreObj, "Create Crystal Core");
            crystalCoreObj.transform.position = new Vector3(0f, 0.38f, 0f);

            var visualCluster = new GameObject("CrystalVisualCluster");
            visualCluster.transform.parent = crystalCoreObj.transform;
            visualCluster.transform.localPosition = Vector3.zero;
            visualCluster.transform.localRotation = Quaternion.identity;

            // --- THÁP THẠCH ANH LỤC GIÁC TRỤ ĐỨNG ĐA DIỆN (Hexagonal Quartz Obelisk Mesh) ---
            var obeliskRoot = new GameObject("HexagonalQuartzObelisk");
            obeliskRoot.transform.parent = visualCluster.transform;
            obeliskRoot.transform.localPosition = Vector3.zero;

            var meshFilter = obeliskRoot.AddComponent<MeshFilter>();
            var meshRenderer = obeliskRoot.AddComponent<MeshRenderer>();
            meshFilter.sharedMesh = GetOrCreateHexagonalQuartzMesh(0.62f, 1.45f, 1.15f, new Vector3(0.04f, 0f, 0.10f));
            meshRenderer.sharedMaterial = quartzCrystalMat;

            // Lõi trên: Nắng vàng ấm áp (Warm Golden Sunlight Core)
            var sunCore = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sunCore.name = "InnerSunCore";
            sunCore.transform.parent = obeliskRoot.transform;
            sunCore.transform.localPosition = new Vector3(0.03f, 1.65f, 0.04f);
            sunCore.transform.localScale = new Vector3(0.48f, 0.65f, 0.48f);
            sunCore.GetComponent<Renderer>().sharedMaterial = quartzSunGlowMat;
            Object.DestroyImmediate(sunCore.GetComponent<Collider>());

            // Lõi dưới: Đại dương ngọc lam (Aqua Ocean Core)
            var aquaCore = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            aquaCore.name = "InnerAquaCore";
            aquaCore.transform.parent = obeliskRoot.transform;
            aquaCore.transform.localPosition = new Vector3(0f, 0.90f, 0f);
            aquaCore.transform.localScale = new Vector3(0.55f, 0.75f, 0.55f);
            aquaCore.GetComponent<Renderer>().sharedMaterial = quartzAquaGlowMat;
            Object.DestroyImmediate(aquaCore.GetComponent<Collider>());

            // Ánh sáng lõi ma thuật (Pulsing Warm Point Light)
            var coreLightObj = new GameObject("CrystalCore_PointLight");
            coreLightObj.transform.parent = obeliskRoot.transform;
            coreLightObj.transform.localPosition = new Vector3(0f, 1.35f, 0f);
            var coreLight = coreLightObj.AddComponent<Light>();
            coreLight.type = LightType.Point;
            coreLight.color = new Color(1.0f, 0.88f, 0.55f);
            coreLight.range = 5.5f;
            coreLight.intensity = 3.2f;

            // --- RỄ CÂY TỰ NHIÊN BÒ ÔM CHÂN THẠCH ANH (Sprout Vine Roots & Blossom Flowers) ---
            var rootsRoot = new GameObject("SproutVineRoots");
            rootsRoot.transform.parent = visualCluster.transform;
            rootsRoot.transform.localPosition = Vector3.zero;

            // 1. Nhánh rễ chính leo chéo mặt trước (Front-Rising Spiral Vine)
            CreateRootSegment(rootsRoot.transform, new Vector3(-0.62f, 0.38f, 0.42f), new Vector3(0.24f, 0.35f, 0.24f), new Vector3(15f, 35f, -25f), woodVineMat);
            CreateRootSegment(rootsRoot.transform, new Vector3(-0.38f, 0.52f, 0.52f), new Vector3(0.22f, 0.38f, 0.22f), new Vector3(25f, 65f, -35f), woodVineMat);
            CreateRootSegment(rootsRoot.transform, new Vector3(-0.05f, 0.68f, 0.56f), new Vector3(0.20f, 0.42f, 0.20f), new Vector3(10f, 90f, -15f), woodVineMat);
            CreateRootSegment(rootsRoot.transform, new Vector3(0.32f, 0.82f, 0.48f), new Vector3(0.18f, 0.38f, 0.18f), new Vector3(-15f, 120f, 20f), woodVineMat);
            CreateRootSegment(rootsRoot.transform, new Vector3(0.55f, 0.96f, 0.30f), new Vector3(0.15f, 0.34f, 0.15f), new Vector3(-30f, 140f, 25f), woodVineMat);

            // 2. Nhánh rễ uốn ngang eo chân trước (Lower Front Arch Vine)
            CreateRootSegment(rootsRoot.transform, new Vector3(0.55f, 0.38f, 0.42f), new Vector3(0.22f, 0.35f, 0.22f), new Vector3(-15f, -35f, 25f), woodVineMat);
            CreateRootSegment(rootsRoot.transform, new Vector3(0.25f, 0.48f, 0.54f), new Vector3(0.20f, 0.38f, 0.20f), new Vector3(-10f, -80f, 15f), woodVineMat);
            CreateRootSegment(rootsRoot.transform, new Vector3(-0.15f, 0.44f, 0.56f), new Vector3(0.18f, 0.36f, 0.18f), new Vector3(5f, -95f, -10f), woodVineMat);
            CreateRootSegment(rootsRoot.transform, new Vector3(-0.48f, 0.48f, 0.46f), new Vector3(0.16f, 0.35f, 0.16f), new Vector3(20f, -130f, -25f), woodVineMat);

            // 3. Nhánh rễ sườn sau và mạn phải (Back-Right Wraparound)
            CreateRootSegment(rootsRoot.transform, new Vector3(0.58f, 0.42f, -0.25f), new Vector3(0.22f, 0.38f, 0.22f), new Vector3(-20f, -40f, 15f), woodVineMat);
            CreateRootSegment(rootsRoot.transform, new Vector3(0.56f, 0.72f, 0.08f), new Vector3(0.19f, 0.40f, 0.19f), new Vector3(-10f, 0f, 15f), woodVineMat);
            CreateRootSegment(rootsRoot.transform, new Vector3(0.48f, 1.05f, 0.22f), new Vector3(0.16f, 0.36f, 0.16f), new Vector3(15f, 35f, -15f), woodVineMat);

            // 4. Nhánh rễ sườn sau và mạn trái (Back-Left Wraparound)
            CreateRootSegment(rootsRoot.transform, new Vector3(-0.55f, 0.45f, -0.28f), new Vector3(0.22f, 0.38f, 0.22f), new Vector3(20f, 40f, -15f), woodVineMat);
            CreateRootSegment(rootsRoot.transform, new Vector3(-0.58f, 0.75f, 0.05f), new Vector3(0.19f, 0.40f, 0.19f), new Vector3(10f, 0f, -15f), woodVineMat);
            CreateRootSegment(rootsRoot.transform, new Vector3(-0.48f, 1.05f, 0.24f), new Vector3(0.16f, 0.36f, 0.16f), new Vector3(-15f, -35f, 15f), woodVineMat);

            // 5. Chân rễ bám đầm xuống đế gỗ (Pedestal Gripping Roots)
            CreateRootSegment(rootsRoot.transform, new Vector3(-0.75f, 0.22f, 0.45f), new Vector3(0.20f, 0.32f, 0.20f), new Vector3(40f, 30f, -20f), woodVineMat);
            CreateRootSegment(rootsRoot.transform, new Vector3(0.72f, 0.22f, 0.42f), new Vector3(0.20f, 0.32f, 0.20f), new Vector3(40f, -30f, 20f), woodVineMat);
            CreateRootSegment(rootsRoot.transform, new Vector3(-0.68f, 0.22f, -0.48f), new Vector3(0.20f, 0.32f, 0.20f), new Vector3(-40f, 140f, 0f), woodVineMat);
            CreateRootSegment(rootsRoot.transform, new Vector3(0.68f, 0.22f, -0.48f), new Vector3(0.20f, 0.32f, 0.20f), new Vector3(-40f, -140f, 0f), woodVineMat);

            // --- 18 HOA ĐÀO & HOA MAI VÀNG 5 CÁNH CỰC CUTE (Cute 5-Petal Blossom Flowers) ---
            // 10 Hoa đào hồng phấn (Peach Blossom Flowers)
            CreateFivePetalFlower(rootsRoot.transform, new Vector3(-0.46f, 0.58f, 0.55f), Quaternion.Euler(-20f, 40f, 30f), 0.95f, peachBlossomMat, flowerCenterMat);
            CreateFivePetalFlower(rootsRoot.transform, new Vector3(-0.18f, 0.72f, 0.58f), Quaternion.Euler(-15f, 75f, 10f), 1.05f, peachBlossomMat, flowerCenterMat);
            CreateFivePetalFlower(rootsRoot.transform, new Vector3(0.12f, 0.78f, 0.56f), Quaternion.Euler(-10f, 105f, -15f), 1.0f, peachBlossomMat, flowerCenterMat);
            CreateFivePetalFlower(rootsRoot.transform, new Vector3(0.42f, 0.92f, 0.45f), Quaternion.Euler(-25f, 130f, -25f), 1.1f, peachBlossomMat, flowerCenterMat);
            CreateFivePetalFlower(rootsRoot.transform, new Vector3(0.60f, 1.02f, 0.22f), Quaternion.Euler(-30f, 150f, 15f), 0.90f, peachBlossomMat, flowerCenterMat);
            CreateFivePetalFlower(rootsRoot.transform, new Vector3(-0.68f, 0.32f, 0.42f), Quaternion.Euler(15f, 30f, 40f), 0.85f, peachBlossomMat, flowerCenterMat);
            CreateFivePetalFlower(rootsRoot.transform, new Vector3(0.62f, 0.36f, 0.38f), Quaternion.Euler(15f, -30f, -40f), 0.90f, peachBlossomMat, flowerCenterMat);
            CreateFivePetalFlower(rootsRoot.transform, new Vector3(0.55f, 0.78f, 0.15f), Quaternion.Euler(0f, -20f, 35f), 0.95f, peachBlossomMat, flowerCenterMat);
            CreateFivePetalFlower(rootsRoot.transform, new Vector3(-0.52f, 0.85f, 0.18f), Quaternion.Euler(0f, 25f, -35f), 0.95f, peachBlossomMat, flowerCenterMat);
            CreateFivePetalFlower(rootsRoot.transform, new Vector3(-0.35f, 1.10f, 0.28f), Quaternion.Euler(-35f, 60f, 20f), 0.85f, peachBlossomMat, flowerCenterMat);

            // 8 Hoa mai vàng mơ mộng (Golden Apricot Blossom Flowers)
            CreateFivePetalFlower(rootsRoot.transform, new Vector3(-0.32f, 0.48f, 0.58f), Quaternion.Euler(-10f, 50f, 20f), 1.0f, goldBlossomMat, flowerCenterMat);
            CreateFivePetalFlower(rootsRoot.transform, new Vector3(-0.02f, 0.56f, 0.60f), Quaternion.Euler(0f, 90f, 0f), 1.15f, goldBlossomMat, flowerCenterMat);
            CreateFivePetalFlower(rootsRoot.transform, new Vector3(0.28f, 0.62f, 0.55f), Quaternion.Euler(-15f, 115f, -10f), 1.05f, goldBlossomMat, flowerCenterMat);
            CreateFivePetalFlower(rootsRoot.transform, new Vector3(0.52f, 0.72f, 0.38f), Quaternion.Euler(-20f, 135f, 20f), 0.95f, goldBlossomMat, flowerCenterMat);
            CreateFivePetalFlower(rootsRoot.transform, new Vector3(-0.58f, 0.48f, 0.40f), Quaternion.Euler(20f, 45f, 30f), 0.85f, goldBlossomMat, flowerCenterMat);
            CreateFivePetalFlower(rootsRoot.transform, new Vector3(0.68f, 0.55f, 0.10f), Quaternion.Euler(10f, -45f, 25f), 0.85f, goldBlossomMat, flowerCenterMat);
            CreateFivePetalFlower(rootsRoot.transform, new Vector3(0.38f, 1.12f, 0.18f), Quaternion.Euler(-25f, 120f, 30f), 0.80f, goldBlossomMat, flowerCenterMat);
            CreateFivePetalFlower(rootsRoot.transform, new Vector3(-0.15f, 0.90f, 0.50f), Quaternion.Euler(-20f, 80f, -20f), 0.90f, goldBlossomMat, flowerCenterMat);

            // Chồi lá non mầm cây (Sprout Leaves)
            CreateSproutLeaf(rootsRoot.transform, new Vector3(-0.50f, 0.62f, 0.52f), Quaternion.Euler(-25f, 35f, 45f), sproutGreenMat);
            CreateSproutLeaf(rootsRoot.transform, new Vector3(0.18f, 0.84f, 0.50f), Quaternion.Euler(15f, 85f, -30f), sproutGreenMat);
            CreateSproutLeaf(rootsRoot.transform, new Vector3(0.48f, 0.98f, 0.32f), Quaternion.Euler(-45f, 120f, 25f), sproutGreenMat);
            CreateSproutLeaf(rootsRoot.transform, new Vector3(0.60f, 0.65f, 0.18f), Quaternion.Euler(0f, -25f, 55f), sproutGreenMat);
            CreateSproutLeaf(rootsRoot.transform, new Vector3(-0.62f, 0.38f, 0.38f), Quaternion.Euler(30f, 45f, -20f), sproutGreenMat);

            // Đốm rêu ngọc bích phát sáng (Glowing Emerald Moss Dots)
            CreateEmeraldMossDot(rootsRoot.transform, new Vector3(-0.35f, 0.44f, 0.56f), emeraldMossMat);
            CreateEmeraldMossDot(rootsRoot.transform, new Vector3(0.02f, 0.52f, 0.58f), emeraldMossMat);
            CreateEmeraldMossDot(rootsRoot.transform, new Vector3(0.35f, 0.70f, 0.52f), emeraldMossMat);
            CreateEmeraldMossDot(rootsRoot.transform, new Vector3(0.55f, 0.85f, 0.28f), emeraldMossMat);
            CreateEmeraldMossDot(rootsRoot.transform, new Vector3(-0.55f, 0.30f, 0.46f), emeraldMossMat);

            // --- VÀNH CỰC QUANG PASTEL BỒNG BỀNH (Aurora Halo Ring) ---
            var haloContainer = new GameObject("AuroraHaloContainer");
            haloContainer.transform.parent = visualCluster.transform;
            haloContainer.transform.localPosition = new Vector3(0f, 1.62f, 0f);
            haloContainer.transform.localRotation = Quaternion.Euler(16f, 25f, -8f);

            var auroraRing = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            auroraRing.name = "AuroraHaloRing";
            auroraRing.transform.parent = haloContainer.transform;
            auroraRing.transform.localPosition = Vector3.zero;
            auroraRing.transform.localRotation = Quaternion.identity;
            auroraRing.transform.localScale = new Vector3(2.15f, 0.022f, 2.15f);
            auroraRing.GetComponent<Renderer>().sharedMaterial = auroraMat;
            Object.DestroyImmediate(auroraRing.GetComponent<Collider>());

            var auroraInnerAura = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            auroraInnerAura.name = "AuroraInnerAura";
            auroraInnerAura.transform.parent = haloContainer.transform;
            auroraInnerAura.transform.localPosition = Vector3.zero;
            auroraInnerAura.transform.localRotation = Quaternion.identity;
            auroraInnerAura.transform.localScale = new Vector3(2.28f, 0.012f, 2.28f);
            auroraInnerAura.GetComponent<Renderer>().sharedMaterial = quartzAquaGlowMat;
            Object.DestroyImmediate(auroraInnerAura.GetComponent<Collider>());

            // Bụi sao lấp lánh (Sparkle Particles)
            CreateCrystalSparkleAura(crystalCoreObj);

            // Gán Visual Controller: Trụ đứng tĩnh vững chãi, Vành cực quang xoay quanh tuần hoàn
            var coreVisual = visualCluster.AddComponent<CrystalCoreVisual>();
            var cvSo = new SerializedObject(coreVisual);
            cvSo.FindProperty("satellitesContainer").objectReferenceValue = haloContainer.transform;
            cvSo.FindProperty("rotationSpeed").floatValue = 0f;
            cvSo.FindProperty("satelliteOrbitSpeed").floatValue = -26f;
            cvSo.FindProperty("hoverAmplitude").floatValue = 0f;
            cvSo.FindProperty("hoverFrequency").floatValue = 1.6f;
            cvSo.ApplyModifiedProperties();

            // Collider chính cho tương tác Click
            var sphereCol = crystalCoreObj.AddComponent<SphereCollider>();
            sphereCol.center = new Vector3(0f, 1.25f, 0f);
            sphereCol.radius = 1.45f;

            var clickSystem = crystalCoreObj.AddComponent<ClickSystem>();
            var csSo = new SerializedObject(clickSystem);
            csSo.FindProperty("visualTransform").objectReferenceValue = visualCluster.transform;
            csSo.ApplyModifiedProperties();

            // 10. Dựng _Managers Root
            SetupManagers(drillData, furnaceData, extractorData, tapPowerData, milestones);

            Undo.CollapseUndoOperations(undoGroup);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log("<color=#00FF88><b>[CrystalForge]</b> Đã hoàn thành nâng cấp Đồ Họa Toàn Diện (Vũ trụ huyền bí, Bụi sao, Đảo thiên thạch, Trận đồ xoay, Ngọn đuốc Cyan, Giá vũ khí & Rương quặng)!</color>");
        }

        private static void SetupCamera()
        {
            var cam = Camera.main;
            if (cam == null)
            {
                var camObj = new GameObject("Main Camera");
                cam = camObj.AddComponent<Camera>();
                camObj.tag = "MainCamera";
            }

            cam.transform.position = new Vector3(0f, 4.8f, -6.6f);
            cam.transform.rotation = Quaternion.Euler(28f, 0f, 0f);
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.035f, 0.02f, 0.075f, 1f); // Nền tím sẫm vũ trụ huyền ảo
            cam.fieldOfView = 50f;
        }

        private static void SetupLighting(Material crystalMat)
        {
            var lights = Object.FindObjectsByType<Light>(FindObjectsInactive.Exclude);
            Light mainLight = null;
            foreach (var l in lights)
            {
                if (l.type == LightType.Directional && l.name == "Directional Light")
                {
                    mainLight = l;
                    break;
                }
            }

            if (mainLight == null)
            {
                var lightObj = new GameObject("Directional Light");
                mainLight = lightObj.AddComponent<Light>();
                mainLight.type = LightType.Directional;
            }

            mainLight.transform.rotation = Quaternion.Euler(48f, -32f, 0f);
            mainLight.color = new Color(1.0f, 0.95f, 0.90f);
            mainLight.intensity = 1.25f;
            mainLight.shadows = LightShadows.Soft;

            // Rim Light viền cyan ma thuật
            var existingRim = GameObject.Find("RimLight_Cyan");
            if (existingRim != null) Object.DestroyImmediate(existingRim);

            var rimObj = new GameObject("RimLight_Cyan");
            rimObj.transform.rotation = Quaternion.Euler(-25f, 150f, 0f);
            var rimLight = rimObj.AddComponent<Light>();
            rimLight.type = LightType.Directional;
            rimLight.color = new Color(0.25f, 0.65f, 1.0f);
            rimLight.intensity = 0.65f;
            rimLight.shadows = LightShadows.None;

            // Point Light phát sáng từ khối Crystal Core
            var existingPoint = GameObject.Find("CrystalCore_PointLight");
            if (existingPoint != null) Object.DestroyImmediate(existingPoint);

            var pointObj = new GameObject("CrystalCore_PointLight");
            pointObj.transform.position = new Vector3(0f, 1.85f, 0f);
            var pointLight = pointObj.AddComponent<Light>();
            pointLight.type = LightType.Point;
            pointLight.color = new Color(0.95f, 0.35f, 1.0f);
            pointLight.range = 9.8f;
            pointLight.intensity = 3.0f;
        }

        private static void SetupPostProcessing()
        {
            var profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>("Assets/Settings/SampleSceneProfile.asset");
            if (profile == null) return;

            // Loại bỏ triệt để bất kỳ component null nào để tránh NullReferenceException trong BuildPipeline
            for (int i = profile.components.Count - 1; i >= 0; i--)
            {
                if (profile.components[i] == null)
                {
                    profile.components.RemoveAt(i);
                }
            }

            if (profile.TryGet<Bloom>(out var bloom))
            {
                bloom.threshold.value = 0.65f;
                bloom.intensity.value = 1.35f;
                bloom.scatter.value = 0.72f;
                bloom.highQualityFiltering.value = true;
            }

            if (!profile.TryGet<ColorAdjustments>(out var colorAdj))
            {
                colorAdj = profile.Add<ColorAdjustments>(true);
                colorAdj.hideFlags = HideFlags.HideInHierarchy;
                AssetDatabase.AddObjectToAsset(colorAdj, profile);
            }
            colorAdj.contrast.overrideState = true;
            colorAdj.contrast.value = 18f;
            colorAdj.saturation.overrideState = true;
            colorAdj.saturation.value = 16f;
            colorAdj.postExposure.overrideState = true;
            colorAdj.postExposure.value = 0.2f;

            if (profile.TryGet<Vignette>(out var vig))
            {
                vig.color.value = new Color(0.04f, 0.02f, 0.08f, 1f);
                vig.intensity.value = 0.28f;
                vig.smoothness.value = 0.42f;
            }

            EditorUtility.SetDirty(profile);
            AssetDatabase.SaveAssets();
        }

        private static void CreateCosmicStarfield(Transform parent)
        {
            var starObj = new GameObject("CosmicStarfield_VFX");
            starObj.transform.parent = parent;
            starObj.transform.position = new Vector3(0f, 2.5f, 2.0f);

            var ps = starObj.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.playOnAwake = true;
            main.loop = true;
            main.duration = 8.0f;
            main.startLifetime = new ParticleSystem.MinMaxCurve(4.0f, 8.0f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(0.04f, 0.15f);
            main.startSize = new ParticleSystem.MinMaxCurve(0.06f, 0.22f);
            main.startColor = new ParticleSystem.MinMaxGradient(new Color(1f, 1f, 1f, 0.9f), new Color(1f, 0.88f, 0.4f, 0.85f));
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.rateOverTime = 18f;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.scale = new Vector3(26f, 15f, 22f);

            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient grad = new Gradient();
            grad.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(new Color(0.9f, 0.6f, 1f), 0.5f), new GradientColorKey(new Color(0.4f, 0.9f, 1f), 1f) },
                new GradientAlphaKey[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(0.85f, 0.3f), new GradientAlphaKey(0.85f, 0.7f), new GradientAlphaKey(0f, 1f) }
            );
            colorOverLifetime.color = grad;

            var renderer = ps.GetComponent<ParticleSystemRenderer>();
            renderer.renderMode = ParticleSystemRenderMode.Billboard;
        }

        private static void CreateFloatingAsteroids(Transform parent, Material stoneMat, Material crystalMat)
        {
            var asteroidsRoot = new GameObject("FloatingAsteroids_Container");
            asteroidsRoot.transform.parent = parent;
            asteroidsRoot.transform.position = Vector3.zero;

            Vector3[] positions = new Vector3[]
            {
                new Vector3(-6.8f, 1.4f, 4.2f),
                new Vector3(7.2f, 2.6f, 3.5f),
                new Vector3(-7.5f, -1.8f, -3.8f),
                new Vector3(7.8f, -0.9f, -4.5f)
            };

            Vector3[] scales = new Vector3[]
            {
                new Vector3(1.3f, 1.7f, 1.3f),
                new Vector3(1.5f, 1.9f, 1.4f),
                new Vector3(1.7f, 1.3f, 1.6f),
                new Vector3(1.4f, 2.1f, 1.4f)
            };

            for (int i = 0; i < positions.Length; i++)
            {
                var ast = new GameObject($"Asteroid_{i + 1}");
                ast.transform.parent = asteroidsRoot.transform;
                ast.transform.position = positions[i];

                var rock = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                rock.name = "RockBody";
                rock.transform.parent = ast.transform;
                rock.transform.localPosition = Vector3.zero;
                rock.transform.localRotation = Quaternion.Euler(Random.Range(-25f, 25f), Random.Range(0f, 360f), Random.Range(-25f, 25f));
                rock.transform.localScale = scales[i];
                rock.GetComponent<Renderer>().sharedMaterial = stoneMat;
                Object.DestroyImmediate(rock.GetComponent<Collider>());

                // Tinh thể nhô ra từ thiên thạch
                var gem = GameObject.CreatePrimitive(PrimitiveType.Cube);
                gem.name = "EmbeddedGem";
                gem.transform.parent = ast.transform;
                gem.transform.localPosition = new Vector3(0.2f, 0.45f, 0f);
                gem.transform.localRotation = Quaternion.Euler(45f, 45f, 20f);
                gem.transform.localScale = Vector3.one * 0.45f;
                gem.GetComponent<Renderer>().sharedMaterial = crystalMat;
                Object.DestroyImmediate(gem.GetComponent<Collider>());

                var rot = ast.AddComponent<Rotator>();
                var rotSo = new SerializedObject(rot);
                rotSo.FindProperty("rotationSpeed").vector3Value = new Vector3(0f, (i % 2 == 0 ? 1f : -1f) * Random.Range(6f, 12f), 0f);
                rotSo.ApplyModifiedProperties();
            }
        }

        private static void CreateSatelliteShard(Transform parent, Vector3 localPos, Vector3 scale, float tiltAngle, Material mat)
        {
            var shard = GameObject.CreatePrimitive(PrimitiveType.Cube);
            shard.name = "SatelliteShard";
            shard.transform.parent = parent;
            shard.transform.localPosition = localPos;
            shard.transform.localRotation = Quaternion.Euler(tiltAngle, 45f, 0f);
            shard.transform.localScale = scale;
            shard.GetComponent<Renderer>().sharedMaterial = mat;
            Object.DestroyImmediate(shard.GetComponent<Collider>());
        }

        private static void CreateCrystalSparkleAura(GameObject parent)
        {
            var vfxObj = new GameObject("CrystalSparkleAura");
            vfxObj.transform.parent = parent.transform;
            vfxObj.transform.localPosition = Vector3.zero;

            var ps = vfxObj.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.playOnAwake = true;
            main.loop = true;
            main.duration = 4f;
            main.startLifetime = new ParticleSystem.MinMaxCurve(1.5f, 2.5f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(0.2f, 0.6f);
            main.startSize = new ParticleSystem.MinMaxCurve(0.06f, 0.16f);
            main.startColor = new ParticleSystem.MinMaxGradient(new Color(0.95f, 0.4f, 1.0f, 0.85f), new Color(1.0f, 0.85f, 0.3f, 0.85f));
            main.gravityModifier = -0.1f;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.rateOverTime = 14f;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.85f;

            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient grad = new Gradient();
            grad.SetKeys(
                new GradientColorKey[] { new GradientColorKey(new Color(0.9f, 0.4f, 1f), 0f), new GradientColorKey(new Color(1f, 0.9f, 0.4f), 1f) },
                new GradientAlphaKey[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(0.9f, 0.25f), new GradientAlphaKey(0f, 1f) }
            );
            colorOverLifetime.color = grad;

            var renderer = ps.GetComponent<ParticleSystemRenderer>();
            renderer.renderMode = ParticleSystemRenderMode.Billboard;
        }

        private static void CreateIslandRoots(Transform islandTransform, Material stoneMat)
        {
            var rootsRoot = new GameObject("IslandUnderRoots");
            rootsRoot.transform.parent = islandTransform;
            rootsRoot.transform.localPosition = Vector3.zero;

            // Tầng đá treo ngược bên dưới sàn
            CreateRockSpire(rootsRoot.transform, new Vector3(0f, -0.6f, 0f), new Vector3(7.2f, 0.2f, 6.2f), stoneMat);
            CreateRockSpire(rootsRoot.transform, new Vector3(0.15f, -1.0f, -0.1f), new Vector3(5.0f, 0.25f, 4.2f), stoneMat);
            CreateRockSpire(rootsRoot.transform, new Vector3(0.1f, -1.55f, 0.1f), new Vector3(3.0f, 0.35f, 2.6f), stoneMat);
            CreateRockSpire(rootsRoot.transform, new Vector3(0.1f, -2.15f, 0.1f), new Vector3(1.2f, 0.35f, 1.1f), stoneMat);

            CreateRockSpire(rootsRoot.transform, new Vector3(-2.8f, -0.9f, -1.8f), new Vector3(1.4f, 0.25f, 1.4f), stoneMat);
            CreateRockSpire(rootsRoot.transform, new Vector3(2.8f, -1.0f, 1.8f), new Vector3(1.3f, 0.3f, 1.3f), stoneMat);
            CreateRockSpire(rootsRoot.transform, new Vector3(2.5f, -0.85f, -2.2f), new Vector3(1.1f, 0.25f, 1.1f), stoneMat);
        }

        private static void CreateRockSpire(Transform parent, Vector3 localPos, Vector3 scale, Material mat)
        {
            var rock = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            rock.transform.parent = parent;
            rock.transform.localPosition = localPos;
            rock.transform.localRotation = Quaternion.Euler(180f, Random.Range(0f, 45f), Random.Range(-10f, 10f));
            rock.transform.localScale = scale;
            rock.GetComponent<Renderer>().sharedMaterial = mat;
            Object.DestroyImmediate(rock.GetComponent<Collider>());
        }

        private static void CreateCornerPylons(Transform parent, Material stoneMat, Material trimMat, Material cyanMat)
        {
            Vector3[] pylonPositions = new Vector3[]
            {
                new Vector3(-4.0f, 0f, -3.4f),
                new Vector3(4.0f, 0f, -3.4f),
                new Vector3(-4.0f, 0f, 3.4f),
                new Vector3(4.0f, 0f, 3.4f)
            };

            foreach (var pos in pylonPositions)
            {
                var pylon = new GameObject("RunicPylon");
                pylon.transform.parent = parent;
                pylon.transform.position = pos;

                // Thân trụ đá
                var pillar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                pillar.transform.parent = pylon.transform;
                pillar.transform.localPosition = new Vector3(0f, 0.42f, 0f);
                pillar.transform.localScale = new Vector3(0.26f, 0.35f, 0.26f);
                pillar.GetComponent<Renderer>().sharedMaterial = stoneMat;

                // Vành đai vàng kim
                var ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                ring.transform.parent = pylon.transform;
                ring.transform.localPosition = new Vector3(0f, 0.78f, 0f);
                ring.transform.localScale = new Vector3(0.38f, 0.05f, 0.38f);
                ring.GetComponent<Renderer>().sharedMaterial = trimMat;

                // Chén đựng ngọn đuốc (Torch Brazier Cup)
                var cup = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                cup.name = "TorchCup";
                cup.transform.parent = pylon.transform;
                cup.transform.localPosition = new Vector3(0f, 0.86f, 0f);
                cup.transform.localScale = new Vector3(0.42f, 0.06f, 0.42f);
                cup.GetComponent<Renderer>().sharedMaterial = trimMat;

                // Quả cầu ngọc Cyan phát sáng
                var orb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                orb.transform.parent = pylon.transform;
                orb.transform.localPosition = new Vector3(0f, 0.98f, 0f);
                orb.transform.localScale = Vector3.one * 0.32f;
                orb.GetComponent<Renderer>().sharedMaterial = cyanMat;

                // Ánh sáng đuốc bập bùng (Flickering Cyan Light)
                var pLightObj = new GameObject("Pylon_FlickerLight");
                pLightObj.transform.parent = pylon.transform;
                pLightObj.transform.localPosition = new Vector3(0f, 1.15f, 0f);
                var pLight = pLightObj.AddComponent<Light>();
                pLight.type = LightType.Point;
                pLight.color = new Color(0.1f, 0.9f, 1.0f);
                pLight.range = 4.8f;
                pLight.intensity = 1.6f;
                var flicker = pLightObj.AddComponent<MysticFlickerLight>();
                var flickSo = new SerializedObject(flicker);
                flickSo.FindProperty("baseIntensity").floatValue = 1.6f;
                flickSo.FindProperty("flickerAmplitude").floatValue = 0.45f;
                flickSo.FindProperty("flickerSpeed").floatValue = 4.0f;
                flickSo.ApplyModifiedProperties();

                // Ngọn lửa ma thuật Cyan (Flame VFX)
                CreateTorchFlameVFX(pylon.transform);
            }
        }

        private static void CreateTorchFlameVFX(Transform parent)
        {
            var flameObj = new GameObject("CyanFlame_VFX");
            flameObj.transform.parent = parent;
            flameObj.transform.localPosition = new Vector3(0f, 1.02f, 0f);

            var ps = flameObj.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.playOnAwake = true;
            main.loop = true;
            main.duration = 1.5f;
            main.startLifetime = new ParticleSystem.MinMaxCurve(0.4f, 0.75f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(0.4f, 0.9f);
            main.startSize = new ParticleSystem.MinMaxCurve(0.08f, 0.18f);
            main.startColor = new ParticleSystem.MinMaxGradient(new Color(0.2f, 0.95f, 1.0f, 0.9f), new Color(1.0f, 1.0f, 1.0f, 0.95f));
            main.gravityModifier = -0.5f;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.rateOverTime = 8f;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 15f;
            shape.radius = 0.08f;

            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient grad = new Gradient();
            grad.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(new Color(0.1f, 0.85f, 1.0f), 0.6f), new GradientColorKey(new Color(0.05f, 0.35f, 0.85f), 1f) },
                new GradientAlphaKey[] { new GradientAlphaKey(0.9f, 0f), new GradientAlphaKey(0.7f, 0.5f), new GradientAlphaKey(0f, 1f) }
            );
            colorOverLifetime.color = grad;

            var renderer = ps.GetComponent<ParticleSystemRenderer>();
            renderer.renderMode = ParticleSystemRenderMode.Billboard;
        }

        private static void CreatePedestalSideStar(Transform parent, Vector3 localPos, Quaternion localRot, Material starMat)
        {
            var star = GameObject.CreatePrimitive(PrimitiveType.Cube);
            star.name = "PedestalSideStar";
            star.transform.parent = parent;
            star.transform.localPosition = localPos;
            star.transform.localRotation = localRot;
            star.transform.localScale = new Vector3(0.18f, 0.18f, 0.04f);
            star.GetComponent<Renderer>().sharedMaterial = starMat;
            Object.DestroyImmediate(star.GetComponent<Collider>());
        }

        private static GameObject CreateCuteStar3D(Transform parent, Vector3 localPos, float scale, Material starMat)
        {
            var starRoot = new GameObject("CuteStar");
            starRoot.transform.parent = parent;
            starRoot.transform.localPosition = localPos;
            starRoot.transform.localRotation = Quaternion.identity;

            // Khối thoi đứng
            var part1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            part1.name = "StarDiamond_A";
            part1.transform.parent = starRoot.transform;
            part1.transform.localPosition = Vector3.zero;
            part1.transform.localRotation = Quaternion.Euler(0f, 0f, 45f);
            part1.transform.localScale = new Vector3(scale, scale, scale * 0.42f);
            part1.GetComponent<Renderer>().sharedMaterial = starMat;
            Object.DestroyImmediate(part1.GetComponent<Collider>());

            // Khối thoi chéo
            var part2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            part2.name = "StarDiamond_B";
            part2.transform.parent = starRoot.transform;
            part2.transform.localPosition = Vector3.zero;
            part2.transform.localRotation = Quaternion.identity;
            part2.transform.localScale = new Vector3(scale * 0.72f, scale * 0.72f, scale * 0.45f);
            part2.GetComponent<Renderer>().sharedMaterial = starMat;
            Object.DestroyImmediate(part2.GetComponent<Collider>());

            return starRoot;
        }

        private static void CreateCuteShard3D(Transform parent, Vector3 localPos, Vector3 scale, float tiltAngle, Material mat)
        {
            var shard = GameObject.CreatePrimitive(PrimitiveType.Cube);
            shard.name = "CuteMiniShard";
            shard.transform.parent = parent;
            shard.transform.localPosition = localPos;
            shard.transform.localRotation = Quaternion.Euler(tiltAngle, 45f, 0f);
            shard.transform.localScale = scale;
            shard.GetComponent<Renderer>().sharedMaterial = mat;
            Object.DestroyImmediate(shard.GetComponent<Collider>());
        }

        private static void CreateRootSegment(Transform parent, Vector3 localPos, Vector3 scale, Vector3 rot, Material mat)
        {
            var root = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            root.name = "RootSegment";
            root.transform.parent = parent;
            root.transform.localPosition = localPos;
            root.transform.localRotation = Quaternion.Euler(rot);
            root.transform.localScale = scale;
            root.GetComponent<Renderer>().sharedMaterial = mat;
            Object.DestroyImmediate(root.GetComponent<Collider>());
        }

        private static void CreateSproutLeaf(Transform parent, Vector3 localPos, Quaternion localRot, Material mat)
        {
            var leaf = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leaf.name = "SproutLeaf";
            leaf.transform.parent = parent;
            leaf.transform.localPosition = localPos;
            leaf.transform.localRotation = localRot;
            leaf.transform.localScale = new Vector3(0.08f, 0.16f, 0.035f);
            leaf.GetComponent<Renderer>().sharedMaterial = mat;
            Object.DestroyImmediate(leaf.GetComponent<Collider>());
        }

        private static void CreateEmeraldMossDot(Transform parent, Vector3 localPos, Material mat)
        {
            var moss = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            moss.name = "EmeraldMossDot";
            moss.transform.parent = parent;
            moss.transform.localPosition = localPos;
            moss.transform.localScale = Vector3.one * 0.065f;
            moss.GetComponent<Renderer>().sharedMaterial = mat;
            Object.DestroyImmediate(moss.GetComponent<Collider>());
        }

        private static GameObject CreateFivePetalFlower(Transform parent, Vector3 localPos, Quaternion localRot, float scale, Material petalMat, Material centerMat)
        {
            var flower = new GameObject("BlossomFlower");
            flower.transform.parent = parent;
            flower.transform.localPosition = localPos;
            flower.transform.localRotation = localRot;
            flower.transform.localScale = Vector3.one * scale;

            // Nhụy hoa vàng phát sáng
            var center = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            center.name = "StamenCenter";
            center.transform.parent = flower.transform;
            center.transform.localPosition = Vector3.zero;
            center.transform.localScale = new Vector3(0.085f, 0.085f, 0.085f);
            center.GetComponent<Renderer>().sharedMaterial = centerMat;
            Object.DestroyImmediate(center.GetComponent<Collider>());

            // 5 Cánh hoa xếp vòng tròn
            for (int i = 0; i < 5; i++)
            {
                float angle = i * 72f * Mathf.Deg2Rad;
                var petal = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                petal.name = $"Petal_{i}";
                petal.transform.parent = flower.transform;
                petal.transform.localPosition = new Vector3(Mathf.Cos(angle) * 0.09f, Mathf.Sin(angle) * 0.09f, -0.015f);
                petal.transform.localRotation = Quaternion.Euler(0f, 0f, i * 72f);
                petal.transform.localScale = new Vector3(0.09f, 0.06f, 0.025f);
                petal.GetComponent<Renderer>().sharedMaterial = petalMat;
                Object.DestroyImmediate(petal.GetComponent<Collider>());
            }

            return flower;
        }

        private static void CreatePedestalRune(Transform parent, Vector3 localPos, Quaternion localRot, int runeIndex, Material runeMat)
        {
            var runeObj = new GameObject($"PedestalRune_{runeIndex}");
            runeObj.transform.parent = parent;
            runeObj.transform.localPosition = localPos;
            runeObj.transform.localRotation = localRot;
            runeObj.transform.localScale = Vector3.one * 0.22f;

            void AddBar(Vector3 pos, Vector3 rot, Vector3 scale)
            {
                var bar = GameObject.CreatePrimitive(PrimitiveType.Cube);
                bar.name = "RuneBar";
                bar.transform.parent = runeObj.transform;
                bar.transform.localPosition = pos;
                bar.transform.localRotation = Quaternion.Euler(rot);
                bar.transform.localScale = scale;
                bar.GetComponent<Renderer>().sharedMaterial = runeMat;
                Object.DestroyImmediate(bar.GetComponent<Collider>());
            }

            float bThick = 0.065f;
            float bLen = 0.65f;

            switch (runeIndex % 8)
            {
                case 0: // Raido (ᚱ)
                    AddBar(new Vector3(-0.12f, 0f, 0f), Vector3.zero, new Vector3(bThick, bLen, bThick));
                    AddBar(new Vector3(0.04f, 0.22f, 0f), Vector3.zero, new Vector3(0.24f, bThick, bThick));
                    AddBar(new Vector3(0.04f, 0.05f, 0f), Vector3.zero, new Vector3(0.24f, bThick, bThick));
                    AddBar(new Vector3(0.16f, 0.135f, 0f), Vector3.zero, new Vector3(bThick, 0.22f, bThick));
                    AddBar(new Vector3(0.08f, -0.18f, 0f), new Vector3(0f, 0f, -42f), new Vector3(bThick, 0.38f, bThick));
                    break;
                case 1: // Kenaz (ᚲ)
                    AddBar(new Vector3(0f, 0.15f, 0f), new Vector3(0f, 0f, 40f), new Vector3(bThick, 0.42f, bThick));
                    AddBar(new Vector3(0f, -0.15f, 0f), new Vector3(0f, 0f, -40f), new Vector3(bThick, 0.42f, bThick));
                    break;
                case 2: // Fehu (ᚠ)
                    AddBar(new Vector3(-0.12f, 0f, 0f), Vector3.zero, new Vector3(bThick, bLen, bThick));
                    AddBar(new Vector3(0.08f, 0.22f, 0f), new Vector3(0f, 0f, 38f), new Vector3(bThick, 0.38f, bThick));
                    AddBar(new Vector3(0.08f, 0.02f, 0f), new Vector3(0f, 0f, 38f), new Vector3(bThick, 0.38f, bThick));
                    break;
                case 3: // Tiwaz (ᛏ)
                    AddBar(Vector3.zero, Vector3.zero, new Vector3(bThick, bLen, bThick));
                    AddBar(new Vector3(-0.14f, 0.20f, 0f), new Vector3(0f, 0f, -45f), new Vector3(bThick, 0.35f, bThick));
                    AddBar(new Vector3(0.14f, 0.20f, 0f), new Vector3(0f, 0f, 45f), new Vector3(bThick, 0.35f, bThick));
                    break;
                case 4: // Othala (ᛟ)
                    AddBar(new Vector3(-0.14f, 0.15f, 0f), new Vector3(0f, 0f, -42f), new Vector3(bThick, 0.40f, bThick));
                    AddBar(new Vector3(0.14f, 0.15f, 0f), new Vector3(0f, 0f, 42f), new Vector3(bThick, 0.40f, bThick));
                    AddBar(new Vector3(0.08f, -0.14f, 0f), new Vector3(0f, 0f, -42f), new Vector3(bThick, 0.45f, bThick));
                    AddBar(new Vector3(-0.08f, -0.14f, 0f), new Vector3(0f, 0f, 42f), new Vector3(bThick, 0.45f, bThick));
                    break;
                case 5: // Sowilo (ᛋ)
                    AddBar(new Vector3(0.08f, 0.20f, 0f), Vector3.zero, new Vector3(0.22f, bThick, bThick));
                    AddBar(new Vector3(-0.02f, 0.09f, 0f), new Vector3(0f, 0f, 45f), new Vector3(bThick, 0.32f, bThick));
                    AddBar(new Vector3(0.02f, -0.09f, 0f), new Vector3(0f, 0f, 45f), new Vector3(bThick, 0.32f, bThick));
                    AddBar(new Vector3(-0.08f, -0.20f, 0f), Vector3.zero, new Vector3(0.22f, bThick, bThick));
                    break;
                case 6: // Algiz (ᛉ)
                    AddBar(Vector3.zero, Vector3.zero, new Vector3(bThick, bLen, bThick));
                    AddBar(new Vector3(-0.14f, 0.16f, 0f), new Vector3(0f, 0f, 45f), new Vector3(bThick, 0.36f, bThick));
                    AddBar(new Vector3(0.14f, 0.16f, 0f), new Vector3(0f, 0f, -45f), new Vector3(bThick, 0.36f, bThick));
                    break;
                case 7: // Wunjo (ᚹ)
                    AddBar(new Vector3(-0.12f, 0f, 0f), Vector3.zero, new Vector3(bThick, bLen, bThick));
                    AddBar(new Vector3(0.04f, 0.20f, 0f), new Vector3(0f, 0f, -35f), new Vector3(bThick, 0.30f, bThick));
                    AddBar(new Vector3(0.04f, 0.04f, 0f), new Vector3(0f, 0f, 35f), new Vector3(bThick, 0.30f, bThick));
                    break;
            }
        }

        private static Mesh GetOrCreateHexagonalQuartzMesh(float radius, float bodyHeight, float apexHeight, Vector3 apexOffset)
        {
            string path = "Assets/Data/HexagonalQuartzMesh.asset";
            var existingMesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if (existingMesh != null)
            {
                AssetDatabase.DeleteAsset(path);
            }
            var mesh = GenerateHexagonalQuartzMesh(radius, bodyHeight, apexHeight, apexOffset);
            AssetDatabase.CreateAsset(mesh, path);
            AssetDatabase.SaveAssets();
            return mesh;
        }

        private static Mesh GenerateHexagonalQuartzMesh(float radius, float bodyHeight, float apexHeight, Vector3 apexOffset)
        {
            var mesh = new Mesh();
            mesh.name = "HexagonalQuartzObelisk_Mesh";

            var vertices = new System.Collections.Generic.List<Vector3>();
            var uvs = new System.Collections.Generic.List<Vector2>();
            var triangles = new System.Collections.Generic.List<int>();

            Vector3[] baseRing = new Vector3[6];
            Vector3[] shoulderRing = new Vector3[6];

            float[] shoulderHeights = new float[] {
                bodyHeight * 1.15f, // i = 0 (NE)
                bodyHeight * 1.18f, // i = 1 (N)
                bodyHeight * 1.10f, // i = 2 (NW)
                bodyHeight * 0.88f, // i = 3 (SW / front-left)
                bodyHeight * 0.82f, // i = 4 (S / front)
                bodyHeight * 0.98f  // i = 5 (SE / front-right)
            };

            for (int i = 0; i < 6; i++)
            {
                float angle = (i * 60f + 30f) * Mathf.Deg2Rad;
                float cos = Mathf.Cos(angle);
                float sin = Mathf.Sin(angle);

                baseRing[i] = new Vector3(radius * cos, 0f, radius * sin);
                shoulderRing[i] = new Vector3(radius * 0.96f * cos, shoulderHeights[i], radius * 0.96f * sin);
            }

            Vector3 apex = new Vector3(apexOffset.x, bodyHeight + apexHeight, apexOffset.z);

            // 1. Dựng 6 mặt trụ chữ nhật đứng
            for (int i = 0; i < 6; i++)
            {
                int next = (i + 1) % 6;
                Vector3 b0 = baseRing[i];
                Vector3 b1 = baseRing[next];
                Vector3 s1 = shoulderRing[next];
                Vector3 s0 = shoulderRing[i];

                int s = vertices.Count;
                vertices.Add(b0);       // s + 0
                vertices.Add(b1);       // s + 1
                vertices.Add(s1);       // s + 2
                vertices.Add(s0);       // s + 3

                uvs.Add(new Vector2((float)i / 6f, 0f));
                uvs.Add(new Vector2((float)(i + 1) / 6f, 0f));
                uvs.Add(new Vector2((float)(i + 1) / 6f, 0.7f));
                uvs.Add(new Vector2((float)i / 6f, 0.7f));

                triangles.Add(s + 0); triangles.Add(s + 3); triangles.Add(s + 2);
                triangles.Add(s + 0); triangles.Add(s + 2); triangles.Add(s + 1);
            }

            // 2. Dựng 6 mặt chóp vát tam giác (Faceted termination)
            for (int i = 0; i < 6; i++)
            {
                int next = (i + 1) % 6;
                Vector3 s0 = shoulderRing[i];
                Vector3 s1 = shoulderRing[next];

                int s = vertices.Count;
                vertices.Add(s0);       // s + 0
                vertices.Add(s1);       // s + 1
                vertices.Add(apex);     // s + 2

                uvs.Add(new Vector2((float)i / 6f, 0.7f));
                uvs.Add(new Vector2((float)(i + 1) / 6f, 0.7f));
                uvs.Add(new Vector2(0.5f, 1f));

                triangles.Add(s + 0); triangles.Add(s + 2); triangles.Add(s + 1);
            }

            // 3. Đáy lục giác
            Vector3 baseCenter = Vector3.zero;
            for (int i = 0; i < 6; i++)
            {
                int next = (i + 1) % 6;
                int s = vertices.Count;
                vertices.Add(baseCenter);     // s + 0
                vertices.Add(baseRing[i]);    // s + 1
                vertices.Add(baseRing[next]); // s + 2

                uvs.Add(new Vector2(0.5f, 0.5f));
                uvs.Add(new Vector2(0.5f, 0f));
                uvs.Add(new Vector2(0f, 0.5f));

                triangles.Add(s + 0); triangles.Add(s + 1); triangles.Add(s + 2);
            }

            mesh.SetVertices(vertices);
            mesh.SetUVs(0, uvs);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        private static void CreateAnvilProp(Transform parent, Vector3 localPos, Quaternion localRot, Material anvilMat, Material trimMat, Material ingotMat)
        {
            var anvilObj = new GameObject("Prop_CrystalAnvil");
            anvilObj.transform.parent = parent;
            anvilObj.transform.localPosition = localPos;
            anvilObj.transform.localRotation = localRot;
            anvilObj.transform.localScale = new Vector3(0.55f, 0.85f, 0.55f);

            var body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = "AnvilBody";
            body.transform.parent = anvilObj.transform;
            body.transform.localPosition = Vector3.zero;
            body.transform.localScale = new Vector3(1.35f, 0.35f, 0.75f);
            body.GetComponent<Renderer>().sharedMaterial = anvilMat;
            Object.DestroyImmediate(body.GetComponent<Collider>());

            var hornL = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            hornL.name = "AnvilHorn_L";
            hornL.transform.parent = anvilObj.transform;
            hornL.transform.localPosition = new Vector3(-0.75f, 0.05f, 0f);
            hornL.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            hornL.transform.localScale = new Vector3(0.25f, 0.35f, 0.25f);
            hornL.GetComponent<Renderer>().sharedMaterial = trimMat;
            Object.DestroyImmediate(hornL.GetComponent<Collider>());

            var hornR = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            hornR.name = "AnvilHorn_R";
            hornR.transform.parent = anvilObj.transform;
            hornR.transform.localPosition = new Vector3(0.75f, 0.05f, 0f);
            hornR.transform.localRotation = Quaternion.Euler(0f, 0f, -90f);
            hornR.transform.localScale = new Vector3(0.25f, 0.35f, 0.25f);
            hornR.GetComponent<Renderer>().sharedMaterial = trimMat;
            Object.DestroyImmediate(hornR.GetComponent<Collider>());

            // Phôi ngọc nóng đỏ rực rỡ nằm trên mặt đe
            var ingot = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ingot.name = "Prop_GlowingIngot";
            ingot.transform.parent = anvilObj.transform;
            ingot.transform.localPosition = new Vector3(0f, 0.22f, 0f);
            ingot.transform.localRotation = Quaternion.Euler(0f, 15f, 0f);
            ingot.transform.localScale = new Vector3(0.48f, 0.12f, 0.25f);
            ingot.GetComponent<Renderer>().sharedMaterial = ingotMat;
            Object.DestroyImmediate(ingot.GetComponent<Collider>());

            // Ánh sáng tỏa nhiệt từ phôi ngọc
            var heatLightObj = new GameObject("Ingot_HeatLight");
            heatLightObj.transform.parent = anvilObj.transform;
            heatLightObj.transform.localPosition = new Vector3(0f, 0.45f, 0f);
            var heatLight = heatLightObj.AddComponent<Light>();
            heatLight.type = LightType.Point;
            heatLight.color = new Color(1.0f, 0.55f, 0.1f);
            heatLight.range = 2.8f;
            heatLight.intensity = 1.8f;
            var flicker = heatLightObj.AddComponent<MysticFlickerLight>();
            var flickSo = new SerializedObject(flicker);
            flickSo.FindProperty("baseIntensity").floatValue = 1.8f;
            flickSo.FindProperty("flickerAmplitude").floatValue = 0.5f;
            flickSo.FindProperty("flickerSpeed").floatValue = 3.2f;
            flickSo.ApplyModifiedProperties();
        }

        private static void CreateBlacksmithWeaponRackProp(Transform parent, Material woodMat, Material trimMat, Material steelMat, Material crystalMat)
        {
            var rackRoot = new GameObject("Prop_BlacksmithWeaponRack");
            rackRoot.transform.parent = parent;
            rackRoot.transform.position = new Vector3(-2.9f, 0f, -1.8f);
            rackRoot.transform.rotation = Quaternion.Euler(0f, 35f, 0f);

            // Khung gỗ đỡ vũ khí
            var postL = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            postL.name = "Post_L";
            postL.transform.parent = rackRoot.transform;
            postL.transform.localPosition = new Vector3(-0.6f, 0.6f, 0f);
            postL.transform.localScale = new Vector3(0.09f, 0.6f, 0.09f);
            postL.GetComponent<Renderer>().sharedMaterial = woodMat;
            Object.DestroyImmediate(postL.GetComponent<Collider>());

            var postR = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            postR.name = "Post_R";
            postR.transform.parent = rackRoot.transform;
            postR.transform.localPosition = new Vector3(0.6f, 0.6f, 0f);
            postR.transform.localScale = new Vector3(0.09f, 0.6f, 0.09f);
            postR.GetComponent<Renderer>().sharedMaterial = woodMat;
            Object.DestroyImmediate(postR.GetComponent<Collider>());

            var barTop = GameObject.CreatePrimitive(PrimitiveType.Cube);
            barTop.name = "Bar_Top";
            barTop.transform.parent = rackRoot.transform;
            barTop.transform.localPosition = new Vector3(0f, 1.0f, 0f);
            barTop.transform.localScale = new Vector3(1.35f, 0.07f, 0.08f);
            barTop.GetComponent<Renderer>().sharedMaterial = woodMat;
            Object.DestroyImmediate(barTop.GetComponent<Collider>());

            var barBot = GameObject.CreatePrimitive(PrimitiveType.Cube);
            barBot.name = "Bar_Bot";
            barBot.transform.parent = rackRoot.transform;
            barBot.transform.localPosition = new Vector3(0f, 0.28f, 0f);
            barBot.transform.localScale = new Vector3(1.35f, 0.07f, 0.08f);
            barBot.GetComponent<Renderer>().sharedMaterial = woodMat;
            Object.DestroyImmediate(barBot.GetComponent<Collider>());

            // Đại Kiếm Pha Lê Thần Thoại cắm trên giá
            var swordObj = new GameObject("CrystalGreatsword");
            swordObj.transform.parent = rackRoot.transform;
            swordObj.transform.localPosition = new Vector3(-0.18f, 0.65f, 0.08f);
            swordObj.transform.localRotation = Quaternion.Euler(0f, 0f, 15f);

            var blade = GameObject.CreatePrimitive(PrimitiveType.Cube);
            blade.name = "SwordBlade";
            blade.transform.parent = swordObj.transform;
            blade.transform.localPosition = new Vector3(0f, 0.28f, 0f);
            blade.transform.localScale = new Vector3(0.12f, 0.95f, 0.04f);
            blade.GetComponent<Renderer>().sharedMaterial = crystalMat;
            Object.DestroyImmediate(blade.GetComponent<Collider>());

            var guard = GameObject.CreatePrimitive(PrimitiveType.Cube);
            guard.name = "SwordGuard";
            guard.transform.parent = swordObj.transform;
            guard.transform.localPosition = new Vector3(0f, -0.2f, 0f);
            guard.transform.localScale = new Vector3(0.35f, 0.06f, 0.08f);
            guard.GetComponent<Renderer>().sharedMaterial = trimMat;
            Object.DestroyImmediate(guard.GetComponent<Collider>());

            var hilt = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            hilt.name = "SwordHilt";
            hilt.transform.parent = swordObj.transform;
            hilt.transform.localPosition = new Vector3(0f, -0.32f, 0f);
            hilt.transform.localScale = new Vector3(0.04f, 0.12f, 0.04f);
            hilt.GetComponent<Renderer>().sharedMaterial = steelMat;
            Object.DestroyImmediate(hilt.GetComponent<Collider>());

            // Rìu Chiến Binh Runic
            var axeObj = new GameObject("RunicBattleaxe");
            axeObj.transform.parent = rackRoot.transform;
            axeObj.transform.localPosition = new Vector3(0.24f, 0.65f, 0.06f);
            axeObj.transform.localRotation = Quaternion.Euler(0f, 0f, -12f);

            var axeHandle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            axeHandle.name = "AxeHandle";
            axeHandle.transform.parent = axeObj.transform;
            axeHandle.transform.localPosition = new Vector3(0f, 0f, 0f);
            axeHandle.transform.localScale = new Vector3(0.045f, 0.55f, 0.045f);
            axeHandle.GetComponent<Renderer>().sharedMaterial = woodMat;
            Object.DestroyImmediate(axeHandle.GetComponent<Collider>());

            var axeBladeL = GameObject.CreatePrimitive(PrimitiveType.Cube);
            axeBladeL.name = "AxeBlade_L";
            axeBladeL.transform.parent = axeObj.transform;
            axeBladeL.transform.localPosition = new Vector3(-0.16f, 0.38f, 0f);
            axeBladeL.transform.localRotation = Quaternion.Euler(0f, 0f, 25f);
            axeBladeL.transform.localScale = new Vector3(0.24f, 0.32f, 0.04f);
            axeBladeL.GetComponent<Renderer>().sharedMaterial = steelMat;
            Object.DestroyImmediate(axeBladeL.GetComponent<Collider>());
        }

        private static void CreateCrystalOreCrateProp(Transform parent, Material woodMat, Material trimMat, Material crystalMat, Material cyanMat)
        {
            var crateRoot = new GameObject("Prop_CrystalOreCrate");
            crateRoot.transform.parent = parent;
            crateRoot.transform.position = new Vector3(-2.7f, 0f, 2.0f);
            crateRoot.transform.rotation = Quaternion.Euler(0f, -25f, 0f);

            // Thùng gỗ sồi
            var box = GameObject.CreatePrimitive(PrimitiveType.Cube);
            box.name = "CrateBox";
            box.transform.parent = crateRoot.transform;
            box.transform.localPosition = new Vector3(0f, 0.22f, 0f);
            box.transform.localScale = new Vector3(0.85f, 0.44f, 0.7f);
            box.GetComponent<Renderer>().sharedMaterial = woodMat;
            Object.DestroyImmediate(box.GetComponent<Collider>());

            // Viền nẹp kim loại
            var band = GameObject.CreatePrimitive(PrimitiveType.Cube);
            band.name = "CrateBand";
            band.transform.parent = box.transform;
            band.transform.localPosition = Vector3.zero;
            band.transform.localScale = new Vector3(1.02f, 0.3f, 1.02f);
            band.GetComponent<Renderer>().sharedMaterial = trimMat;
            Object.DestroyImmediate(band.GetComponent<Collider>());

            // Các khối ngọc pha lê thô nhô lên bên trong thùng
            for (int i = 0; i < 3; i++)
            {
                var gem = GameObject.CreatePrimitive(PrimitiveType.Cube);
                gem.name = $"RawGem_{i}";
                gem.transform.parent = crateRoot.transform;
                float x = -0.2f + i * 0.2f;
                gem.transform.localPosition = new Vector3(x, 0.46f, Random.Range(-0.1f, 0.1f));
                gem.transform.localRotation = Quaternion.Euler(Random.Range(20f, 50f), Random.Range(0f, 90f), Random.Range(-30f, 30f));
                gem.transform.localScale = new Vector3(0.24f, 0.35f, 0.22f);
                gem.GetComponent<Renderer>().sharedMaterial = (i % 2 == 0) ? crystalMat : cyanMat;
                Object.DestroyImmediate(gem.GetComponent<Collider>());
            }
        }

        private static void CreateDrillProp(Transform parent, Material stoneMat, Material machineMat, Material crystalMat)
        {
            var drillRoot = new GameObject("Prop_CrystalDrill");
            drillRoot.transform.parent = parent;
            drillRoot.transform.position = new Vector3(-2.8f, 0.4f, 0.5f);

            var basePod = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            basePod.transform.parent = drillRoot.transform;
            basePod.transform.localPosition = Vector3.zero;
            basePod.transform.localScale = new Vector3(1.2f, 0.2f, 1.2f);
            basePod.GetComponent<Renderer>().sharedMaterial = stoneMat;

            var body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.transform.parent = drillRoot.transform;
            body.transform.localPosition = new Vector3(0f, 0.6f, 0f);
            body.transform.localScale = new Vector3(0.7f, 1.0f, 0.7f);
            body.GetComponent<Renderer>().sharedMaterial = machineMat;

            var drillBit = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            drillBit.transform.parent = drillRoot.transform;
            drillBit.transform.localPosition = new Vector3(0.5f, 0.2f, 0f);
            drillBit.transform.localRotation = Quaternion.Euler(30f, 0f, 45f);
            drillBit.transform.localScale = new Vector3(0.25f, 0.6f, 0.25f);
            drillBit.GetComponent<Renderer>().sharedMaterial = crystalMat;
        }

        private static void CreateFurnaceProp(Transform parent, Material stoneMat, Material trimMat, Material crystalMat, Material ingotMat)
        {
            var furnaceRoot = new GameObject("Prop_ManaFurnace");
            furnaceRoot.transform.parent = parent;
            furnaceRoot.transform.position = new Vector3(2.8f, 0.5f, 1.5f);

            var oven = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            oven.transform.parent = furnaceRoot.transform;
            oven.transform.localPosition = new Vector3(0f, 0.4f, 0f);
            oven.transform.localScale = new Vector3(1.4f, 0.8f, 1.4f);
            oven.GetComponent<Renderer>().sharedMaterial = stoneMat;

            var chimney = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            chimney.transform.parent = furnaceRoot.transform;
            chimney.transform.localPosition = new Vector3(0f, 1.25f, 0f);
            chimney.transform.localScale = new Vector3(0.6f, 0.9f, 0.6f);
            chimney.GetComponent<Renderer>().sharedMaterial = trimMat;

            var coreGlow = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            coreGlow.transform.parent = furnaceRoot.transform;
            coreGlow.transform.localPosition = new Vector3(0f, 0.4f, -0.6f);
            coreGlow.transform.localScale = new Vector3(0.42f, 0.42f, 0.42f);
            coreGlow.GetComponent<Renderer>().sharedMaterial = ingotMat;

            // Ánh lửa lò nung bập bùng
            var fireLightObj = new GameObject("FurnaceFire_Light");
            fireLightObj.transform.parent = furnaceRoot.transform;
            fireLightObj.transform.localPosition = new Vector3(0f, 0.4f, -0.85f);
            var fLight = fireLightObj.AddComponent<Light>();
            fLight.type = LightType.Point;
            fLight.color = new Color(1.0f, 0.5f, 0.1f);
            fLight.range = 4.2f;
            fLight.intensity = 2.2f;
            var flicker = fireLightObj.AddComponent<MysticFlickerLight>();
            var flickSo = new SerializedObject(flicker);
            flickSo.FindProperty("baseIntensity").floatValue = 2.2f;
            flickSo.FindProperty("flickerAmplitude").floatValue = 0.6f;
            flickSo.FindProperty("flickerSpeed").floatValue = 4.5f;
            flickSo.ApplyModifiedProperties();

            // Khói ma thuật bốc lên từ ống khói (Furnace Smoke VFX)
            CreateFurnaceSmokeVFX(furnaceRoot.transform);
        }

        private static void CreateFurnaceSmokeVFX(Transform parent)
        {
            var smokeObj = new GameObject("FurnaceSmoke_VFX");
            smokeObj.transform.parent = parent;
            smokeObj.transform.localPosition = new Vector3(0f, 1.75f, 0f);

            var ps = smokeObj.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.playOnAwake = true;
            main.loop = true;
            main.duration = 3.0f;
            main.startLifetime = new ParticleSystem.MinMaxCurve(2.0f, 3.5f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(0.35f, 0.8f);
            main.startSize = new ParticleSystem.MinMaxCurve(0.18f, 0.45f);
            main.startColor = new ParticleSystem.MinMaxGradient(new Color(0.9f, 0.6f, 1.0f, 0.5f), new Color(1.0f, 0.85f, 0.4f, 0.4f));
            main.gravityModifier = -0.2f;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.rateOverTime = 6f;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 12f;
            shape.radius = 0.15f;

            var sizeOverLifetime = ps.sizeOverLifetime;
            sizeOverLifetime.enabled = true;
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(0f, 0.4f);
            curve.AddKey(1f, 1.8f);
            sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, curve);

            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient grad = new Gradient();
            grad.SetKeys(
                new GradientColorKey[] { new GradientColorKey(new Color(1f, 0.7f, 0.3f), 0f), new GradientColorKey(new Color(0.7f, 0.4f, 0.9f), 0.7f), new GradientColorKey(new Color(0.3f, 0.3f, 0.4f), 1f) },
                new GradientAlphaKey[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(0.45f, 0.25f), new GradientAlphaKey(0f, 1f) }
            );
            colorOverLifetime.color = grad;

            var renderer = ps.GetComponent<ParticleSystemRenderer>();
            renderer.renderMode = ParticleSystemRenderMode.Billboard;
        }

        private static void CreateExtractorProp(Transform parent, Material machineMat, Material trimMat, Material crystalMat)
        {
            var extractorRoot = new GameObject("Prop_AutoExtractor");
            extractorRoot.transform.parent = parent;
            extractorRoot.transform.position = new Vector3(2.8f, 0.5f, -1.5f);

            var pylon = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pylon.transform.parent = extractorRoot.transform;
            pylon.transform.localPosition = new Vector3(0f, 0.8f, 0f);
            pylon.transform.localScale = new Vector3(0.4f, 1.6f, 0.4f);
            pylon.GetComponent<Renderer>().sharedMaterial = machineMat;

            // Vòng xoay ngoài
            var ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ring.name = "FloatingRing";
            ring.transform.parent = extractorRoot.transform;
            ring.transform.localPosition = new Vector3(0f, 1.3f, 0f);
            ring.transform.localScale = new Vector3(1.25f, 0.08f, 1.25f);
            ring.GetComponent<Renderer>().sharedMaterial = trimMat;
            ring.AddComponent<Rotator>();

            // Vòng xoay trong ngược chiều
            var innerRing = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            innerRing.name = "FloatingRingInner";
            innerRing.transform.parent = extractorRoot.transform;
            innerRing.transform.localPosition = new Vector3(0f, 1.3f, 0f);
            innerRing.transform.localScale = new Vector3(0.85f, 0.06f, 0.85f);
            innerRing.GetComponent<Renderer>().sharedMaterial = crystalMat;
            var innerRot = innerRing.AddComponent<Rotator>();
            var rotSo = new SerializedObject(innerRot);
            rotSo.FindProperty("rotationSpeed").vector3Value = new Vector3(0f, -45f, 0f);
            rotSo.ApplyModifiedProperties();

            var tipOrb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            tipOrb.transform.parent = extractorRoot.transform;
            tipOrb.transform.localPosition = new Vector3(0f, 1.85f, 0f);
            tipOrb.transform.localScale = new Vector3(0.52f, 0.52f, 0.52f);
            tipOrb.GetComponent<Renderer>().sharedMaterial = crystalMat;
        }

        private static void SetupManagers(GeneratorDataSO drill, GeneratorDataSO furnace, GeneratorDataSO extractor, UpgradeDataSO tap, MilestoneDataSO[] milestones)
        {
            var managersObj = GameObject.Find("_Managers");
            if (managersObj == null)
            {
                managersObj = new GameObject("_Managers");
                Undo.RegisterCreatedObjectUndo(managersObj, "Create Managers Root");
            }

            var resourceManager = managersObj.GetComponent<ResourceManager>() ?? managersObj.AddComponent<ResourceManager>();
            var genSystem = managersObj.GetComponent<GeneratorSystem>() ?? managersObj.AddComponent<GeneratorSystem>();
            var upgradeManager = managersObj.GetComponent<UpgradeManager>() ?? managersObj.AddComponent<UpgradeManager>();
            var milestoneManager = managersObj.GetComponent<MilestoneManager>() ?? managersObj.AddComponent<MilestoneManager>();

            var genSo = new SerializedObject(genSystem);
            var genListProp = genSo.FindProperty("generatorConfigs");
            genListProp.ClearArray();
            genListProp.InsertArrayElementAtIndex(0);
            genListProp.GetArrayElementAtIndex(0).objectReferenceValue = drill;
            genListProp.InsertArrayElementAtIndex(1);
            genListProp.GetArrayElementAtIndex(1).objectReferenceValue = furnace;
            genListProp.InsertArrayElementAtIndex(2);
            genListProp.GetArrayElementAtIndex(2).objectReferenceValue = extractor;
            genSo.ApplyModifiedProperties();

            var upgSo = new SerializedObject(upgradeManager);
            upgSo.FindProperty("tapPowerData").objectReferenceValue = tap;
            upgSo.ApplyModifiedProperties();

            var msSo = new SerializedObject(milestoneManager);
            var msListProp = msSo.FindProperty("milestoneConfigs");
            msListProp.ClearArray();
            for (int i = 0; i < milestones.Length; i++)
            {
                msListProp.InsertArrayElementAtIndex(i);
                msListProp.GetArrayElementAtIndex(i).objectReferenceValue = milestones[i];
            }
            msSo.ApplyModifiedProperties();
        }

        private static void CleanOldSceneObjects()
        {
            var oldEnv = GameObject.Find("Environment_Diorama");
            if (oldEnv != null) Object.DestroyImmediate(oldEnv);

            var oldCore = GameObject.Find("CrystalCore");
            if (oldCore != null) Object.DestroyImmediate(oldCore);
        }

        private static void EnsureFolders()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Materials"))
                AssetDatabase.CreateFolder("Assets", "Materials");
            if (!AssetDatabase.IsValidFolder("Assets/Data"))
                AssetDatabase.CreateFolder("Assets", "Data");
            if (!AssetDatabase.IsValidFolder("Assets/Data/Generators"))
                AssetDatabase.CreateFolder("Assets/Data", "Generators");
            if (!AssetDatabase.IsValidFolder("Assets/Data/Upgrades"))
                AssetDatabase.CreateFolder("Assets/Data", "Upgrades");
            if (!AssetDatabase.IsValidFolder("Assets/Data/Milestones"))
                AssetDatabase.CreateFolder("Assets/Data", "Milestones");
        }

        private static Material GetOrCreateMaterial(string matName, Color color, float smoothness, bool hasEmission = false, Color? emissionColor = null, float metallic = 0f)
        {
            string path = $"Assets/Materials/{matName}.mat";
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null)
            {
                var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
                mat = new Material(shader);
                AssetDatabase.CreateAsset(mat, path);
            }

            mat.color = color;
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
            if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", smoothness);
            if (mat.HasProperty("_Metallic")) mat.SetFloat("_Metallic", metallic);

            if (hasEmission && emissionColor.HasValue)
            {
                mat.EnableKeyword("_EMISSION");
                if (mat.HasProperty("_EmissionColor")) mat.SetColor("_EmissionColor", emissionColor.Value);
            }
            else
            {
                mat.DisableKeyword("_EMISSION");
            }
            EditorUtility.SetDirty(mat);
            return mat;
        }

        private static GeneratorDataSO GetOrCreateGeneratorData(GeneratorType type, string name, double baseCost, float growth, double baseProd, double unlockLifetime)
        {
            string path = $"Assets/Data/Generators/{type}.asset";
            var data = AssetDatabase.LoadAssetAtPath<GeneratorDataSO>(path);
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<GeneratorDataSO>();
                data.generatorType = type;
                data.generatorName = name;
                data.baseCost = baseCost;
                data.costGrowthRate = growth;
                data.baseProductionPerSec = baseProd;
                data.unlockLifetimeRequired = unlockLifetime;
                AssetDatabase.CreateAsset(data, path);
            }
            return data;
        }

        private static UpgradeDataSO GetOrCreateTapPowerData()
        {
            string path = "Assets/Data/Upgrades/TapPower.asset";
            var data = AssetDatabase.LoadAssetAtPath<UpgradeDataSO>(path);
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<UpgradeDataSO>();
                data.upgradeId = "tap_power";
                data.upgradeName = "Tap Power";
                data.baseCost = 25.0;
                data.costGrowthRate = 1.18f;
                data.basePowerPerClick = 1.0;
                data.powerGainPerLevel = 1.0;
                AssetDatabase.CreateAsset(data, path);
            }
            return data;
        }

        private static MilestoneDataSO[] CreateOrLoadMilestones()
        {
            var tiers = new[]
            {
                (MilestoneTier.Bronze, "Bronze Milestone", "Bua Ren Runic", "Di vat co xua tiep them linh luc khai khoang.", 100.0, 1.15f, true, GeneratorType.CrystalDrill, "Mo khoa Khoan Tinh The & +15% San Luong"),
                (MilestoneTier.Silver, "Silver Milestone", "Kinh Thau Quang", "Thau kinh hoi tu nang luong mana tinh khiet.", 2500.0, 1.30f, false, GeneratorType.CrystalDrill, "+30% San Luong Toan Cuc"),
                (MilestoneTier.Gold, "Gold Milestone", "Khien Co Ngu", "Khien bao ho linh thieng cua Thanh Dien.", 25000.0, 1.50f, true, GeneratorType.ManaFurnace, "Mo khoa Lo Nung Mana & +50% San Luong"),
                (MilestoneTier.Platinum, "Platinum Milestone", "Vuong Mien Tho Mo", "Vuong mien vinh danh nghe nhan ren huyen thoai.", 250000.0, 2.0f, false, GeneratorType.AutoExtractor, "+100% San Luong Toan Cuc"),
                (MilestoneTier.Guardian, "Guardian Milestone", "Trai Tim Thanh Dien", "Loi thach anh vinh cuu ban phuoc lanh toi cao.", 1000000.0, 3.0f, false, GeneratorType.AutoExtractor, "+200% San Luong & Hao Quang Ho Ve")
            };

            var list = new MilestoneDataSO[tiers.Length];
            for (int i = 0; i < tiers.Length; i++)
            {
                var t = tiers[i];
                string path = $"Assets/Data/Milestones/{t.Item1}.asset";
                var data = AssetDatabase.LoadAssetAtPath<MilestoneDataSO>(path);
                if (data == null)
                {
                    data = ScriptableObject.CreateInstance<MilestoneDataSO>();
                    AssetDatabase.CreateAsset(data, path);
                }

                data.tier = t.Item1;
                data.milestoneTitle = t.Item2;
                data.relicName = t.Item3;
                data.relicLore = t.Item4;
                data.requiredLifetimeCrystal = t.Item5;
                data.globalProductionMultiplier = t.Item6;
                data.unlocksGenerator = t.Item7;
                data.unlockedGeneratorType = t.Item8;
                data.rewardSummaryText = t.Item9;
                EditorUtility.SetDirty(data);

                list[i] = data;
            }
            AssetDatabase.SaveAssets();
            return list;
        }
    }
}
#endif
