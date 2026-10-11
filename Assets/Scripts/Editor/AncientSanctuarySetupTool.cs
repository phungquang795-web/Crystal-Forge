#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using CrystalForge.Adventure;
using CrystalForge.Core;
using CrystalForge.Economy;
using CrystalForge.UI;
using CrystalForge.Utils;
using CrystalForge.Visuals;

namespace CrystalForge.Editor
{
    /// <summary>
    /// Công cụ Editor kiến tạo toàn bộ Không gian Thánh Điện Hầm Ngục Cổ Đại (Ancient Cavern Sanctuary),
    /// Trụ Bệ Đá Tinh Thể Khổng Lồ Phát Sáng (Floating Diamond Crystal & Runic Pedestal - Ảnh 1 & 3),
    /// và Nhân Vật Thợ Mỏ Chibi Dũng Sĩ với Cuốc Khai Khoáng Rực Lửa (Miner Apprentice Hero - Ảnh 2).
    /// </summary>
    public static class AncientSanctuarySetupTool
    {
        [MenuItem("CrystalForge/Build Ancient Sanctuary & Miner Hero (Exact Concept Art)")]
        public static void BuildSanctuaryAndMinerHero()
        {
            Undo.SetCurrentGroupName("Build Ancient Sanctuary & Miner Hero");
            int undoGroup = Undo.GetCurrentGroup();

            Debug.Log("<color=#00F0FF><b>[CrystalForge]</b> Đang kiến tạo Thánh Điện Hầm Ngục Cổ Đại, Trụ Tinh Thể & Nhân vật Thợ Mỏ chuẩn Concept Art...</color>");

            // 1. Chuẩn bị Thư mục & Vật liệu PBR URP
            EnsureFolders();
            var mats = SetupSanctuaryMaterials();

            // 2. Tìm hoặc Khởi tạo Gốc Môi Trường
            var envRoot = GameObject.Find("Environment_Diorama");
            if (envRoot == null)
            {
                envRoot = new GameObject("Environment_Diorama");
                Undo.RegisterCreatedObjectUndo(envRoot, "Create Environment Root");
            }

            // Dọn dẹp môi trường cũ để tái thiết kế sạch sẽ chuẩn 100%
            for (int i = envRoot.transform.childCount - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(envRoot.transform.GetChild(i).gameObject);
            }

            // Dọn dẹp các model nhân vật cũ nếu có ngoài envRoot
            CleanOldHeroAndCores();

            // 3. XÂY DỰNG KHUNG CẢNH HẦM NGỤC THÁNH ĐIỆN CỔ ĐẠI (Ảnh 3)
            BuildSanctuaryChamber(envRoot.transform, mats);

            // 4. XÂY DỰNG BỆ ĐÁ BÁT GIÁC CỔ NGỮ & CỤM TINH THỂ VỆ TINH (Ảnh 1 & Ảnh 3)
            var pedestalObj = BuildAncientStonePedestal(envRoot.transform, mats);

            // 5. XÂY DỰNG LÕI TINH THỂ KIM CƯƠNG LƠ LỬNG & HÀO QUANG SƯƠNG LAM (Ảnh 1)
            var crystalCoreObj = BuildFloatingDiamondCrystal(envRoot.transform, mats);

            // 6. XÂY DỰNG NHÂN VẬT THỢ MỎ CHIBI VỚI MODEL PLAYER.FBX
            var heroObj = InstantiatePlayerHeroModel(envRoot.transform);

            // 7. KẾT NỐI HỆ THỐNG QUẢN LÝ (ClickSystem, Adventure3DManager, Camera & Post-Processing)
            WireSystemsAndManagers(crystalCoreObj, heroObj);
            SetupIsometricCameraAndPostProcessing();

            // Khử dấu tiếng Việt các TextMeshPro để tránh lỗi font SDF
            SanitizeSceneTexts();

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Undo.CollapseUndoOperations(undoGroup);

            Debug.Log("<color=#55FF55><b>[CrystalForge]</b> Hoàn thành xây dựng Thánh Điện Hầm Ngục, Trụ Tinh Thể và Player Thợ Mỏ thành công rực rỡ!</color>");
        }

        #region 1. VẬT LIỆU PBR URP CHUẨN CONCEPT ART
        public class SanctuaryMaterials
        {
            public Material StoneDark;
            public Material StoneLight;
            public Material StoneTile;
            public Material RunicCyan;
            public Material DiamondCrystal;
            public Material InnerCrystal;
            public Material CrystalSpire;
            public Material MistRing;
            public Material IvyVine;
            public Material MossPatch;
            public Material TorchFlame;
            public Material TorchMetal;
            public Material TorchWood;
            public Material MinerSkin;
            public Material MinerHair;
            public Material MinerEyes;
            public Material MinerEyeWhite;
            public Material MinerBlush;
            public Material MinerHelmet;
            public Material MinerTunic;
            public Material MinerApron;
            public Material MinerBoots;
            public Material MinerBrass;
            public Material MinerRuneCharm;
            public Material PickaxeWood;
            public Material PickaxeStone;
            public Material PickaxeCyanCore;
            public Material PickaxeMagma;
            public Material GodRay;
        }

        private static SanctuaryMaterials SetupSanctuaryMaterials()
        {
            var m = new SanctuaryMaterials();

            // Đá hầm ngục & Bệ thờ cổ
            m.StoneDark = GetOrCreateLitMaterial("Mat_Sanctuary_StoneDark", new Color(0.24f, 0.26f, 0.32f), 0.20f);
            m.StoneLight = GetOrCreateLitMaterial("Mat_Sanctuary_StoneLight", new Color(0.48f, 0.52f, 0.58f), 0.30f);
            m.StoneTile = GetOrCreateLitMaterial("Mat_Sanctuary_StoneTile", new Color(0.32f, 0.34f, 0.42f), 0.25f);

            // Cổ ngữ & Tinh thể phát quang rực rỡ
            m.RunicCyan = GetOrCreateEmissionMaterial("Mat_Sanctuary_RunicCyan", new Color(0.05f, 0.85f, 1.0f), new Color(0.1f, 0.95f, 1.0f), 4.5f);
            m.DiamondCrystal = GetOrCreateEmissionMaterial("Mat_Sanctuary_DiamondCrystal", new Color(0.25f, 0.75f, 1.0f, 0.92f), new Color(0.15f, 0.85f, 1.0f), 3.8f);
            m.InnerCrystal = GetOrCreateEmissionMaterial("Mat_Sanctuary_InnerCrystal", new Color(0.9f, 0.98f, 1.0f), new Color(0.85f, 0.98f, 1.0f), 6.0f);
            m.CrystalSpire = GetOrCreateEmissionMaterial("Mat_Sanctuary_CrystalSpire", new Color(0.30f, 0.80f, 1.0f), new Color(0.20f, 0.88f, 1.0f), 3.2f);
            m.MistRing = GetOrCreateEmissionMaterial("Mat_Sanctuary_MistRing", new Color(0.2f, 0.9f, 1.0f, 0.55f), new Color(0.2f, 0.95f, 1.0f), 2.8f);

            // Dây leo & Rêu xanh
            m.IvyVine = GetOrCreateLitMaterial("Mat_Sanctuary_IvyVine", new Color(0.20f, 0.45f, 0.25f), 0.15f);
            m.MossPatch = GetOrCreateEmissionMaterial("Mat_Sanctuary_MossPatch", new Color(0.26f, 0.58f, 0.32f), new Color(0.35f, 0.70f, 0.40f), 0.9f);

            // Đuốc & Lửa
            m.TorchFlame = GetOrCreateEmissionMaterial("Mat_Sanctuary_TorchFlame", new Color(1.0f, 0.55f, 0.10f), new Color(1.0f, 0.65f, 0.15f), 5.0f);
            m.TorchMetal = GetOrCreateLitMaterial("Mat_Sanctuary_TorchMetal", new Color(0.16f, 0.17f, 0.22f), 0.75f);
            m.TorchWood = GetOrCreateLitMaterial("Mat_Sanctuary_TorchWood", new Color(0.36f, 0.22f, 0.14f), 0.15f);

            // Nhân vật Thợ mỏ Chibi (Magical Miner Boy)
            m.MinerSkin = GetOrCreateLitMaterial("Mat_Miner_Skin", new Color(1.0f, 0.86f, 0.76f), 0.12f);
            m.MinerHair = GetOrCreateLitMaterial("Mat_Miner_Hair", new Color(0.62f, 0.28f, 0.16f), 0.25f);
            m.MinerEyes = GetOrCreateLitMaterial("Mat_Miner_Eyes", new Color(0.18f, 0.10f, 0.06f), 0.85f);
            m.MinerEyeWhite = GetOrCreateLitMaterial("Mat_Miner_EyeWhite", Color.white, 0.90f);
            m.MinerBlush = GetOrCreateLitMaterial("Mat_Miner_Blush", new Color(1.0f, 0.65f, 0.68f), 0.10f);
            m.MinerHelmet = GetOrCreateLitMaterial("Mat_Miner_Helmet", new Color(0.62f, 0.44f, 0.30f), 0.40f);
            m.MinerTunic = GetOrCreateLitMaterial("Mat_Miner_Tunic", new Color(0.26f, 0.45f, 0.62f), 0.20f);
            m.MinerApron = GetOrCreateLitMaterial("Mat_Miner_Apron", new Color(0.48f, 0.30f, 0.18f), 0.30f);
            m.MinerBoots = GetOrCreateLitMaterial("Mat_Miner_Boots", new Color(0.24f, 0.18f, 0.14f), 0.30f);
            m.MinerBrass = GetOrCreateLitMaterial("Mat_Miner_Brass", new Color(0.85f, 0.68f, 0.28f), 0.85f);
            m.MinerRuneCharm = GetOrCreateEmissionMaterial("Mat_Miner_RuneCharm", new Color(0.15f, 0.95f, 0.55f), new Color(0.1f, 0.95f, 0.5f), 3.5f);

            // Cuốc khai khoáng ma thuật lõi pha lê
            m.PickaxeWood = GetOrCreateLitMaterial("Mat_Pickaxe_Wood", new Color(0.45f, 0.28f, 0.16f), 0.25f);
            m.PickaxeStone = GetOrCreateLitMaterial("Mat_Pickaxe_Stone", new Color(0.42f, 0.44f, 0.50f), 0.45f);
            m.PickaxeCyanCore = GetOrCreateEmissionMaterial("Mat_Pickaxe_CyanCore", new Color(0.15f, 0.90f, 1.0f), new Color(0.12f, 0.90f, 1.0f), 4.8f);
            m.PickaxeMagma = GetOrCreateEmissionMaterial("Mat_Pickaxe_MagmaGlow", new Color(1.0f, 0.45f, 0.10f), new Color(1.0f, 0.60f, 0.20f), 5.5f);

            // Tia sáng thiên giới (God Ray)
            m.GodRay = GetOrCreateEmissionMaterial("Mat_Sanctuary_GodRay", new Color(0.65f, 0.85f, 1.0f, 0.18f), new Color(0.65f, 0.90f, 1.0f), 1.5f);

            return m;
        }
        #endregion

        #region 2. XÂY DỰNG KHUNG CẢNH HẦM NGỤC THÁNH ĐIỆN CỔ ĐẠI (Ảnh 3)
        private static void BuildSanctuaryChamber(Transform parent, SanctuaryMaterials mats)
        {
            var chamber = new GameObject("Sanctuary_Chamber_Architecture");
            chamber.transform.SetParent(parent, false);

            // A. Sàn đá hầm ngục đa tầng (Chamber Floor & Slabs)
            var floorMain = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floorMain.name = "Chamber_FloorMain";
            floorMain.transform.SetParent(chamber.transform, false);
            floorMain.transform.localPosition = new Vector3(0f, -0.2f, 0f);
            floorMain.transform.localScale = new Vector3(10.5f, 0.4f, 9.5f);
            floorMain.GetComponent<Renderer>().sharedMaterial = mats.StoneDark;

            // Bậc đá viền rìa (Stone Borders)
            for (int i = 0; i < 8; i++)
            {
                float x = ((i % 4) - 1.5f) * 2.5f;
                float z = (i < 4 ? -4.3f : 4.3f);
                var edgeSlab = GameObject.CreatePrimitive(PrimitiveType.Cube);
                edgeSlab.name = $"Slab_Edge_{i}";
                edgeSlab.transform.SetParent(chamber.transform, false);
                edgeSlab.transform.localPosition = new Vector3(x, 0.02f, z);
                edgeSlab.transform.localScale = new Vector3(2.2f, 0.1f, 0.9f);
                edgeSlab.transform.localRotation = Quaternion.Euler(0f, (i % 2 == 0 ? 3f : -3f), 0f);
                edgeSlab.GetComponent<Renderer>().sharedMaterial = mats.StoneTile;
                RemoveCollider(edgeSlab);
            }

            // B. Vòng Tròn Cổ Ngữ Khắc Trên Sàn Đá (Floor Runic Magic Circle - Ảnh 3)
            var runeCircle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            runeCircle.name = "Floor_RunicMagicCircle";
            runeCircle.transform.SetParent(chamber.transform, false);
            runeCircle.transform.localPosition = new Vector3(0f, 0.008f, 0f);
            runeCircle.transform.localScale = new Vector3(4.8f, 0.005f, 4.8f);
            runeCircle.GetComponent<Renderer>().sharedMaterial = mats.StoneLight;
            RemoveCollider(runeCircle);

            // Vòng viền trong phát sáng Cyan
            var innerRuneGlow = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            innerRuneGlow.name = "Floor_InnerRuneGlowBand";
            innerRuneGlow.transform.SetParent(chamber.transform, false);
            innerRuneGlow.transform.localPosition = new Vector3(0f, 0.012f, 0f);
            innerRuneGlow.transform.localScale = new Vector3(4.65f, 0.005f, 4.65f);
            innerRuneGlow.GetComponent<Renderer>().sharedMaterial = mats.RunicCyan;
            RemoveCollider(innerRuneGlow);

            // 16 Ký tự cổ ngữ Rune khắc chạy vòng tròn trên sàn
            for (int r = 0; r < 16; r++)
            {
                float angle = r * (360f / 16f);
                float rad = angle * Mathf.Deg2Rad;
                float radius = 2.15f;

                var runeStone = GameObject.CreatePrimitive(PrimitiveType.Cube);
                runeStone.name = $"RuneFloorGlyph_{r}";
                runeStone.transform.SetParent(chamber.transform, false);
                runeStone.transform.localPosition = new Vector3(Mathf.Sin(rad) * radius, 0.025f, Mathf.Cos(rad) * radius);
                runeStone.transform.localRotation = Quaternion.Euler(0f, angle, 0f);
                runeStone.transform.localScale = new Vector3(0.18f, 0.02f, 0.28f);
                runeStone.GetComponent<Renderer>().sharedMaterial = mats.RunicCyan;
                RemoveCollider(runeStone);
            }

            // Gắn script xoay chậm vòng cổ ngữ sàn tạo Game Feel sống động
            var rot = runeCircle.AddComponent<RunicCircleRotator>();
            var rotSo = new SerializedObject(rot);
            rotSo.FindProperty("rotationSpeed").floatValue = 3.5f;
            rotSo.FindProperty("rotateClockwise").boolValue = true;
            rotSo.ApplyModifiedProperties();

            // C. 4 Cột Trụ Đá Cổ Kính Khổng Lồ Quấn Dây Leo (Ancient Pillars with Ivy Vines - Ảnh 3)
            Vector3[] pillarPositions = new Vector3[]
            {
                new Vector3(-4.2f, 0f, -3.2f),
                new Vector3(4.2f, 0f, -3.2f),
                new Vector3(-4.2f, 0f, 3.2f),
                new Vector3(4.2f, 0f, 3.2f)
            };

            for (int p = 0; p < pillarPositions.Length; p++)
            {
                BuildAncientPillar(chamber.transform, pillarPositions[p], p, mats);
            }

            // D. Tường Hầm Ngục Cổ Phía Sau & Các Vòm Đá (Background Dungeon Wall & Arches)
            var backWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            backWall.name = "Dungeon_BackWall";
            backWall.transform.SetParent(chamber.transform, false);
            backWall.transform.localPosition = new Vector3(0f, 2.5f, 4.8f);
            backWall.transform.localScale = new Vector3(10.8f, 5.5f, 0.8f);
            backWall.GetComponent<Renderer>().sharedMaterial = mats.StoneDark;
            RemoveCollider(backWall);

            // Vòm đá cổ phía sau
            var archTop = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            archTop.name = "Dungeon_CenterArch";
            archTop.transform.SetParent(chamber.transform, false);
            archTop.transform.localPosition = new Vector3(0f, 3.8f, 4.35f);
            archTop.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            archTop.transform.localScale = new Vector3(3.6f, 0.35f, 3.6f);
            archTop.GetComponent<Renderer>().sharedMaterial = mats.StoneLight;
            RemoveCollider(archTop);

            // E. Luồng Sáng Thần Quang Rọi Từ Trần Hầm Ngục (God Ray SpotLight - Ảnh 3)
            var godRayLightObj = new GameObject("Volumetric_GodRay_SpotLight");
            godRayLightObj.transform.SetParent(chamber.transform, false);
            godRayLightObj.transform.localPosition = new Vector3(0.4f, 5.0f, -0.6f);
            godRayLightObj.transform.localRotation = Quaternion.Euler(75f, -20f, 0f);
            var spot = godRayLightObj.AddComponent<Light>();
            spot.type = LightType.Spot;
            spot.spotAngle = 45f;
            spot.innerSpotAngle = 22f;
            spot.range = 9.0f;
            spot.color = new Color(0.70f, 0.90f, 1.0f);
            spot.intensity = 4.5f;

            // Cụm tinh thể thạch anh mọc ven tường và góc phòng (Perimeter Crystal Clusters)
            BuildPerimeterCrystals(chamber.transform, mats);
        }

        private static void BuildAncientPillar(Transform parent, Vector3 pos, int index, SanctuaryMaterials mats)
        {
            var pillarObj = new GameObject($"AncientPillar_{index}");
            pillarObj.transform.SetParent(parent, false);
            pillarObj.transform.localPosition = pos;

            // Chân đế cột vuông (Plinth)
            var baseSquare = GameObject.CreatePrimitive(PrimitiveType.Cube);
            baseSquare.transform.SetParent(pillarObj.transform, false);
            baseSquare.transform.localPosition = new Vector3(0f, 0.25f, 0f);
            baseSquare.transform.localScale = new Vector3(0.95f, 0.5f, 0.95f);
            baseSquare.GetComponent<Renderer>().sharedMaterial = mats.StoneLight;
            RemoveCollider(baseSquare);

            // Thân cột trụ tròn (Shaft)
            var shaft = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            shaft.transform.SetParent(pillarObj.transform, false);
            shaft.transform.localPosition = new Vector3(0f, 2.2f, 0f);
            shaft.transform.localScale = new Vector3(0.72f, 1.8f, 0.72f);
            shaft.GetComponent<Renderer>().sharedMaterial = mats.StoneDark;
            RemoveCollider(shaft);

            // Đầu cột (Capital)
            var capital = GameObject.CreatePrimitive(PrimitiveType.Cube);
            capital.transform.SetParent(pillarObj.transform, false);
            capital.transform.localPosition = new Vector3(0f, 4.1f, 0f);
            capital.transform.localScale = new Vector3(1.05f, 0.45f, 1.05f);
            capital.GetComponent<Renderer>().sharedMaterial = mats.StoneLight;
            RemoveCollider(capital);

            // Dây leo xanh uốn quanh cột (Ivy Vines)
            for (int v = 0; v < 6; v++)
            {
                float vineY = 0.6f + v * 0.55f;
                float rotY = v * 55f + index * 40f;
                var vine = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                vine.transform.SetParent(pillarObj.transform, false);
                vine.transform.localPosition = new Vector3(Mathf.Sin(rotY * Mathf.Deg2Rad) * 0.38f, vineY, Mathf.Cos(rotY * Mathf.Deg2Rad) * 0.38f);
                vine.transform.localRotation = Quaternion.Euler(Random.Range(10f, 25f), rotY, Random.Range(-15f, 15f));
                vine.transform.localScale = new Vector3(0.08f, 0.28f, 0.08f);
                vine.GetComponent<Renderer>().sharedMaterial = mats.IvyVine;
                RemoveCollider(vine);
            }

            // Đuốc Hầm Ngục Treo Trên Cột (Wall Torch with Flickering Flame Light - Ảnh 3)
            var torchObj = new GameObject("Dungeon_Torch");
            torchObj.transform.SetParent(pillarObj.transform, false);
            Vector3 inwardDir = -pos.normalized;
            torchObj.transform.localPosition = inwardDir * 0.48f + Vector3.up * 2.2f;

            // Chân đuốc sắt
            var torchArm = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            torchArm.transform.SetParent(torchObj.transform, false);
            torchArm.transform.localPosition = Vector3.zero;
            torchArm.transform.localRotation = Quaternion.Euler(45f, 0f, 0f);
            torchArm.transform.localScale = new Vector3(0.08f, 0.22f, 0.08f);
            torchArm.GetComponent<Renderer>().sharedMaterial = mats.TorchMetal;
            RemoveCollider(torchArm);

            // Bát đựng đuốc (Torch Cup)
            var torchCup = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            torchCup.transform.SetParent(torchObj.transform, false);
            torchCup.transform.localPosition = new Vector3(0f, 0.2f, 0.12f);
            torchCup.transform.localScale = new Vector3(0.25f, 0.12f, 0.25f);
            torchCup.GetComponent<Renderer>().sharedMaterial = mats.TorchMetal;
            RemoveCollider(torchCup);

            // Lửa đuốc cam phát sáng (Flame Core)
            var flame = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            flame.transform.SetParent(torchCup.transform, false);
            flame.transform.localPosition = new Vector3(0f, 1.1f, 0f);
            flame.transform.localScale = new Vector3(0.85f, 1.4f, 0.85f);
            flame.GetComponent<Renderer>().sharedMaterial = mats.TorchFlame;
            RemoveCollider(flame);

            // Nguồn sáng đuốc bập bùng tự nhiên (Warm Flickering Point Light)
            var lightObj = new GameObject("Torch_PointLight");
            lightObj.transform.SetParent(torchCup.transform, false);
            lightObj.transform.localPosition = new Vector3(0f, 1.2f, 0f);
            var pLight = lightObj.AddComponent<Light>();
            pLight.type = LightType.Point;
            pLight.color = new Color(1.0f, 0.65f, 0.22f);
            pLight.range = 5.2f;
            pLight.intensity = 1.9f;

            var flicker = lightObj.AddComponent<MysticFlickerLight>();
            var flickSo = new SerializedObject(flicker);
            flickSo.FindProperty("baseIntensity").floatValue = 1.9f;
            flickSo.FindProperty("flickerAmplitude").floatValue = 0.55f;
            flickSo.FindProperty("flickerSpeed").floatValue = 5.0f;
            flickSo.ApplyModifiedProperties();
        }

        private static void BuildPerimeterCrystals(Transform parent, SanctuaryMaterials mats)
        {
            Vector3[] clusterOrigins = new Vector3[]
            {
                new Vector3(-3.8f, 0f, 2.5f),
                new Vector3(3.8f, 0f, 2.5f),
                new Vector3(-2.8f, 0f, 3.8f),
                new Vector3(2.8f, 0f, 3.8f)
            };

            foreach (var origin in clusterOrigins)
            {
                var group = new GameObject("CrystalCluster_Wall");
                group.transform.SetParent(parent, false);
                group.transform.localPosition = origin;

                for (int c = 0; c < 4; c++)
                {
                    var spire = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    spire.transform.SetParent(group.transform, false);
                    float rot = c * 75f;
                    spire.transform.localPosition = new Vector3(Mathf.Sin(rot) * 0.25f, 0.4f + c * 0.15f, Mathf.Cos(rot) * 0.25f);
                    spire.transform.localRotation = Quaternion.Euler(Random.Range(-15f, 15f), rot, Random.Range(-20f, 20f));
                    spire.transform.localScale = new Vector3(0.14f, 0.45f + c * 0.18f, 0.14f);
                    spire.GetComponent<Renderer>().sharedMaterial = mats.CrystalSpire;
                    RemoveCollider(spire);
                }
            }
        }
        #endregion

        #region 3. XÂY DỰNG BỆ ĐÁ BÁT GIÁC CỔ NGỮ & CỤM TINH THỂ VỆ TINH (Ảnh 1 & Ảnh 3)
        private static GameObject BuildAncientStonePedestal(Transform parent, SanctuaryMaterials mats)
        {
            var pedestalRoot = new GameObject("Central_Pedestal");
            pedestalRoot.transform.SetParent(parent, false);
            pedestalRoot.transform.localPosition = Vector3.zero;

            // Tầng 1: Đế đá bát giác bậc dưới (Lower Tier Octagonal Stone Base)
            var baseTierA = GameObject.CreatePrimitive(PrimitiveType.Cube);
            baseTierA.name = "Pedestal_BaseTier_A";
            baseTierA.transform.SetParent(pedestalRoot.transform, false);
            baseTierA.transform.localPosition = new Vector3(0f, 0.16f, 0f);
            baseTierA.transform.localScale = new Vector3(2.6f, 0.32f, 2.6f);
            baseTierA.GetComponent<Renderer>().sharedMaterial = mats.StoneDark;
            RemoveCollider(baseTierA);

            var baseTierB = GameObject.CreatePrimitive(PrimitiveType.Cube);
            baseTierB.name = "Pedestal_BaseTier_B";
            baseTierB.transform.SetParent(pedestalRoot.transform, false);
            baseTierB.transform.localPosition = new Vector3(0f, 0.16f, 0f);
            baseTierB.transform.localRotation = Quaternion.Euler(0f, 45f, 0f);
            baseTierB.transform.localScale = new Vector3(2.6f, 0.32f, 2.6f);
            baseTierB.GetComponent<Renderer>().sharedMaterial = mats.StoneDark;
            RemoveCollider(baseTierB);

            // Tầng 2: Đế đá chạm khắc cổ ngữ giật cấp bên trên (Upper Tier with Carved Rune Tablets - Ảnh 1)
            var midTierA = GameObject.CreatePrimitive(PrimitiveType.Cube);
            midTierA.name = "Pedestal_MidTier_A";
            midTierA.transform.SetParent(pedestalRoot.transform, false);
            midTierA.transform.localPosition = new Vector3(0f, 0.42f, 0f);
            midTierA.transform.localScale = new Vector3(2.1f, 0.26f, 2.1f);
            midTierA.GetComponent<Renderer>().sharedMaterial = mats.StoneLight;
            RemoveCollider(midTierA);

            var midTierB = GameObject.CreatePrimitive(PrimitiveType.Cube);
            midTierB.name = "Pedestal_MidTier_B";
            midTierB.transform.SetParent(pedestalRoot.transform, false);
            midTierB.transform.localPosition = new Vector3(0f, 0.42f, 0f);
            midTierB.transform.localRotation = Quaternion.Euler(0f, 45f, 0f);
            midTierB.transform.localScale = new Vector3(2.1f, 0.26f, 2.1f);
            midTierB.GetComponent<Renderer>().sharedMaterial = mats.StoneLight;
            RemoveCollider(midTierB);

            // 8 Bảng Phù Điêu Cổ Ngữ Khảm Trên 8 Mặt Bát Giác Của Bệ Đá (Ảnh 1)
            for (int r = 0; r < 8; r++)
            {
                float angle = r * 45f;
                float rad = angle * Mathf.Deg2Rad;
                Vector3 runePos = new Vector3(Mathf.Sin(rad) * 1.07f, 0.42f, Mathf.Cos(rad) * 1.07f);
                Quaternion runeRot = Quaternion.Euler(0f, angle, 0f);

                // Khung viền đá bọc chữ Rune
                var runeFrame = GameObject.CreatePrimitive(PrimitiveType.Cube);
                runeFrame.name = $"RunePlate_Frame_{r}";
                runeFrame.transform.SetParent(pedestalRoot.transform, false);
                runeFrame.transform.localPosition = runePos;
                runeFrame.transform.localRotation = runeRot;
                runeFrame.transform.localScale = new Vector3(0.52f, 0.20f, 0.06f);
                runeFrame.GetComponent<Renderer>().sharedMaterial = mats.StoneDark;
                RemoveCollider(runeFrame);

                // Ký tự cổ ngữ phát sáng Cyan rực rỡ
                var runeGlyph = GameObject.CreatePrimitive(PrimitiveType.Cube);
                runeGlyph.name = $"RuneGlyph_Cyan_{r}";
                runeGlyph.transform.SetParent(runeFrame.transform, false);
                runeGlyph.transform.localPosition = new Vector3(0f, 0f, 0.6f);
                runeGlyph.transform.localScale = new Vector3(0.6f, 0.7f, 0.2f);
                runeGlyph.GetComponent<Renderer>().sharedMaterial = mats.RunicCyan;
                RemoveCollider(runeGlyph);
            }

            // Tầng 3: Mặt Bàn Thờ Đá Trên Cùng Khắc Vòng Cổ Ngữ (Top Altar Surface - Ảnh 1 & 3)
            var topAltar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            topAltar.name = "Pedestal_TopAltarSlab";
            topAltar.transform.SetParent(pedestalRoot.transform, false);
            topAltar.transform.localPosition = new Vector3(0f, 0.56f, 0f);
            topAltar.transform.localScale = new Vector3(1.85f, 0.05f, 1.85f);
            topAltar.GetComponent<Renderer>().sharedMaterial = mats.StoneLight;
            RemoveCollider(topAltar);

            // Vòng cổ ngữ phát sáng trên mặt bàn thờ đá
            var topRuneRing = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            topRuneRing.name = "Pedestal_TopRuneRing";
            topRuneRing.transform.SetParent(pedestalRoot.transform, false);
            topRuneRing.transform.localPosition = new Vector3(0f, 0.588f, 0f);
            topRuneRing.transform.localScale = new Vector3(1.45f, 0.005f, 1.45f);
            topRuneRing.GetComponent<Renderer>().sharedMaterial = mats.RunicCyan;
            RemoveCollider(topRuneRing);

            // Mảng rêu phong phủ trên bệ đá (Moss Clinging to Pedestal - Ảnh 3)
            for (int m = 0; m < 6; m++)
            {
                float rot = m * 60f + 15f;
                var moss = GameObject.CreatePrimitive(PrimitiveType.Cube);
                moss.name = $"Pedestal_Moss_{m}";
                moss.transform.SetParent(pedestalRoot.transform, false);
                moss.transform.localPosition = new Vector3(Mathf.Sin(rot * Mathf.Deg2Rad) * 0.95f, 0.32f, Mathf.Cos(rot * Mathf.Deg2Rad) * 0.95f);
                moss.transform.localRotation = Quaternion.Euler(0f, rot, 0f);
                moss.transform.localScale = new Vector3(0.35f, 0.06f, 0.25f);
                moss.GetComponent<Renderer>().sharedMaterial = mats.MossPatch;
                RemoveCollider(moss);
            }

            // CỤM TINH THỂ NGỌC LAM MỌC QUANH BỆ ĐÁ (Crystal Spires Surrounding Altar - Ảnh 1 & Ảnh 3)
            BuildSurroundingCrystalSpires(pedestalRoot.transform, mats);

            return pedestalRoot;
        }

        private static void BuildSurroundingCrystalSpires(Transform parent, SanctuaryMaterials mats)
        {
            Vector3[] spireClusters = new Vector3[]
            {
                new Vector3(-1.45f, 0.1f, -0.95f),
                new Vector3(1.45f, 0.1f, -0.95f),
                new Vector3(-1.35f, 0.1f, 1.1f),
                new Vector3(1.35f, 0.1f, 1.1f)
            };

            for (int k = 0; k < spireClusters.Length; k++)
            {
                var clusterRoot = new GameObject($"CrystalSpireCluster_{k}");
                clusterRoot.transform.SetParent(parent, false);
                clusterRoot.transform.localPosition = spireClusters[k];

                // 3 Tinh thể chụm lại góc nghiêng tự nhiên
                for (int s = 0; s < 3; s++)
                {
                    var spire = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    spire.name = $"Spire_{s}";
                    spire.transform.SetParent(clusterRoot.transform, false);

                    float angle = s * 120f + k * 45f;
                    float height = 0.65f + s * 0.25f;
                    float thickness = 0.18f - s * 0.02f;

                    spire.transform.localPosition = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad) * 0.15f, height * 0.5f, Mathf.Cos(angle * Mathf.Deg2Rad) * 0.15f);
                    spire.transform.localRotation = Quaternion.Euler(Random.Range(10f, 25f), angle, Random.Range(-15f, 15f));
                    spire.transform.localScale = new Vector3(thickness, height * 0.5f, thickness);
                    spire.GetComponent<Renderer>().sharedMaterial = mats.DiamondCrystal;
                    RemoveCollider(spire);

                    // Chóp nhọn kim tự tháp của tinh thể
                    var tip = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    tip.name = "Tip";
                    tip.transform.SetParent(spire.transform, false);
                    tip.transform.localPosition = new Vector3(0f, 1.0f, 0f);
                    tip.transform.localScale = new Vector3(1.05f, 1.35f, 1.05f);
                    tip.GetComponent<Renderer>().sharedMaterial = mats.DiamondCrystal;
                    RemoveCollider(tip);
                }
            }
        }
        #endregion

        #region 4. XÂY DỰNG LÕI TINH THỂ KIM CƯƠNG LƠ LỬNG & HÀO QUANG SƯƠNG LAM (Ảnh 1)
        private static GameObject BuildFloatingDiamondCrystal(Transform parent, SanctuaryMaterials mats)
        {
            var crystalCoreObj = new GameObject("CrystalCore");
            crystalCoreObj.transform.SetParent(parent, false);
            crystalCoreObj.transform.localPosition = new Vector3(0f, 1.85f, 0f);

            var visualCluster = new GameObject("CrystalVisualCluster");
            visualCluster.transform.SetParent(crystalCoreObj.transform, false);
            visualCluster.transform.localPosition = Vector3.zero;

            // A. Khối Tinh Thể Kim Cương Đa Diện 2 Đầu Nhọn (Double-Terminated Diamond Crystal Mesh - Ảnh 1)
            var diamondObj = new GameObject("GiantDiamondCrystalMesh");
            diamondObj.transform.SetParent(visualCluster.transform, false);
            diamondObj.transform.localPosition = Vector3.zero;

            var meshFilter = diamondObj.AddComponent<MeshFilter>();
            var meshRenderer = diamondObj.AddComponent<MeshRenderer>();
            meshFilter.sharedMesh = GetOrCreateDiamondOctahedronMesh(0.72f, 1.45f);
            meshRenderer.sharedMaterial = mats.DiamondCrystal;

            // B. Lõi Pha Lê Rực Sáng Bên Trong (White-Hot Inner Core Sphere)
            var innerCore = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            innerCore.name = "InnerCrystalCore";
            innerCore.transform.SetParent(diamondObj.transform, false);
            innerCore.transform.localPosition = Vector3.zero;
            innerCore.transform.localScale = new Vector3(0.52f, 0.78f, 0.52f);
            innerCore.GetComponent<Renderer>().sharedMaterial = mats.InnerCrystal;
            RemoveCollider(innerCore);

            // C. Vòng Hào Quang Sương Lam Cuộn Quanh Bụng Tinh Thể (Swirling Mist Energy Ring - Ảnh 1)
            var mistRing = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            mistRing.name = "SwirlingMistEnergyRing";
            mistRing.transform.SetParent(visualCluster.transform, false);
            mistRing.transform.localPosition = Vector3.zero;
            mistRing.transform.localRotation = Quaternion.Euler(18f, 0f, 8f);
            mistRing.transform.localScale = new Vector3(1.85f, 0.02f, 1.85f);
            mistRing.GetComponent<Renderer>().sharedMaterial = mats.MistRing;
            RemoveCollider(mistRing);

            var mistRotator = mistRing.AddComponent<RunicCircleRotator>();
            var mistSo = new SerializedObject(mistRotator);
            mistSo.FindProperty("rotationSpeed").floatValue = -25f;
            mistSo.FindProperty("rotateClockwise").boolValue = false;
            mistSo.ApplyModifiedProperties();

            // D. Bụi Hạt Ma Thuật Lấp Lánh Bay Xung Quanh (Magic Sparkle Particles - Ảnh 1)
            CreateSparkleParticles(visualCluster);

            // E. Ánh Sáng Nhịp Thở Ma Thuật (Breathing Magic Point Light)
            var lightObj = new GameObject("CrystalCore_PointLight");
            lightObj.transform.SetParent(visualCluster.transform, false);
            lightObj.transform.localPosition = Vector3.zero;
            var cLight = lightObj.AddComponent<Light>();
            cLight.type = LightType.Point;
            cLight.color = new Color(0.2f, 0.92f, 1.0f);
            cLight.range = 6.8f;
            cLight.intensity = 3.6f;

            // F. Gắn Script CrystalCoreVisual để Tinh Thể Nhấp Nhô Bồng Bềnh & Xoay Tròn Uyển Chuyển
            var visual = crystalCoreObj.AddComponent<CrystalCoreVisual>();
            var vSo = new SerializedObject(visual);
            vSo.FindProperty("hoverAmplitude").floatValue = 0.12f;
            vSo.FindProperty("hoverFrequency").floatValue = 1.8f;
            vSo.FindProperty("rotationSpeed").floatValue = 28f;
            vSo.FindProperty("corePointLight").objectReferenceValue = cLight;
            vSo.FindProperty("baseLightIntensity").floatValue = 3.6f;
            vSo.FindProperty("lightPulseAmplitude").floatValue = 0.8f;
            vSo.FindProperty("lightPulseFrequency").floatValue = 2.5f;
            vSo.ApplyModifiedProperties();

            // G. Gắn BoxCollider để tương thích 100% với ClickSystem (Khai thác tinh thể)
            var boxCol = crystalCoreObj.AddComponent<BoxCollider>();
            boxCol.center = Vector3.zero;
            boxCol.size = new Vector3(1.8f, 3.2f, 1.8f);

            return crystalCoreObj;
        }

        private static Mesh GetOrCreateDiamondOctahedronMesh(float radius, float halfHeight)
        {
            string path = "Assets/Data/DiamondCrystalOctahedronMesh.asset";
            var existing = AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if (existing != null)
            {
                AssetDatabase.DeleteAsset(path);
            }

            var mesh = new Mesh();
            mesh.name = "DiamondCrystalOctahedron_Mesh";

            var verts = new List<Vector3>();
            var tris = new List<int>();
            var uvs = new List<Vector2>();

            // Vòng xích đạo 8 điểm (Faceted Octagonal Equator Ring)
            Vector3 topApex = new Vector3(0f, halfHeight, 0f);
            Vector3 bottomApex = new Vector3(0f, -halfHeight, 0f);

            int segments = 8;
            Vector3[] equator = new Vector3[segments];
            for (int i = 0; i < segments; i++)
            {
                float angle = i * (360f / segments) * Mathf.Deg2Rad;
                equator[i] = new Vector3(Mathf.Sin(angle) * radius, 0f, Mathf.Cos(angle) * radius);
            }

            // Dựng 8 tam giác chóp trên
            for (int i = 0; i < segments; i++)
            {
                int next = (i + 1) % segments;
                int start = verts.Count;

                verts.Add(topApex);
                verts.Add(equator[i]);
                verts.Add(equator[next]);

                uvs.Add(new Vector2(0.5f, 1f));
                uvs.Add(new Vector2((float)i / segments, 0.5f));
                uvs.Add(new Vector2((float)(i + 1) / segments, 0.5f));

                tris.Add(start);
                tris.Add(start + 1);
                tris.Add(start + 2);
            }

            // Dựng 8 tam giác chóp dưới
            for (int i = 0; i < segments; i++)
            {
                int next = (i + 1) % segments;
                int start = verts.Count;

                verts.Add(bottomApex);
                verts.Add(equator[next]);
                verts.Add(equator[i]);

                uvs.Add(new Vector2(0.5f, 0f));
                uvs.Add(new Vector2((float)(i + 1) / segments, 0.5f));
                uvs.Add(new Vector2((float)i / segments, 0.5f));

                tris.Add(start);
                tris.Add(start + 1);
                tris.Add(start + 2);
            }

            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.SetUVs(0, uvs);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            AssetDatabase.CreateAsset(mesh, path);
            AssetDatabase.SaveAssets();
            return mesh;
        }

        private static void CreateSparkleParticles(GameObject parent)
        {
            var pObj = new GameObject("CrystalSparkleParticles");
            pObj.transform.SetParent(parent.transform, false);
            pObj.transform.localPosition = Vector3.zero;

            var ps = pObj.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.playOnAwake = true;
            main.loop = true;
            main.duration = 3.5f;
            main.startLifetime = new ParticleSystem.MinMaxCurve(1.5f, 2.8f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(0.2f, 0.5f);
            main.startSize = new ParticleSystem.MinMaxCurve(0.06f, 0.15f);
            main.startColor = new ParticleSystem.MinMaxGradient(new Color(0.2f, 0.9f, 1.0f, 0.9f), new Color(0.9f, 0.98f, 1.0f, 0.9f));
            main.gravityModifier = -0.06f;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.rateOverTime = 16f;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 1.1f;

            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient grad = new Gradient();
            grad.SetKeys(
                new GradientColorKey[] { new GradientColorKey(new Color(0.1f, 0.9f, 1f), 0f), new GradientColorKey(new Color(1f, 1f, 1f), 1f) },
                new GradientAlphaKey[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(0.9f, 0.3f), new GradientAlphaKey(0f, 1f) }
            );
            colorOverLifetime.color = grad;

            var rend = ps.GetComponent<ParticleSystemRenderer>();
            rend.renderMode = ParticleSystemRenderMode.Billboard;
        }
        #endregion

        #region 5. XÂY DỰNG NHÂN VẬT THỢ MỎ CHIBI & CUỐC KHAI KHOÁNG PHÉP THUẬT (Concept Art Chuẩn 100%)
        private static GameObject BuildMinerHeroModel(Transform parent, SanctuaryMaterials mats)
        {
            var heroRoot = new GameObject("Player_CapybaraHero");
            if (parent != null) heroRoot.transform.SetParent(parent, false);

            // Vị trí đứng trước lò rèn và bệ thờ hầm ngục (Quay mặt góc 3/4 Isometric hướng Camera)
            heroRoot.transform.localPosition = new Vector3(-1.65f, 0.0f, -0.65f);
            heroRoot.transform.localRotation = Quaternion.Euler(0f, 135f, 0f);
            heroRoot.transform.localScale = Vector3.one * 1.15f;
            heroRoot.tag = "Player";

            // A. THÂN & ÁO TUNIC XANH MỘC MẠC (Blue Tunic Torso)
            var body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.name = "Miner_Torso";
            body.transform.SetParent(heroRoot.transform, false);
            body.transform.localPosition = new Vector3(0f, 0.50f, 0f);
            body.transform.localScale = new Vector3(0.54f, 0.44f, 0.48f);
            body.GetComponent<Renderer>().sharedMaterial = mats.MinerTunic;
            RemoveCollider(body);

            // B. TẠP DỀ DA THỢ RÈN (Rustic Leather Blacksmith Apron)
            var apron = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            apron.name = "Miner_Apron";
            apron.transform.SetParent(body.transform, false);
            apron.transform.localPosition = new Vector3(0f, -0.1f, 0.06f);
            apron.transform.localScale = new Vector3(1.08f, 0.45f, 1.08f);
            apron.GetComponent<Renderer>().sharedMaterial = mats.MinerApron;
            RemoveCollider(apron);

            // Yếm ngực tạp dề (Apron Bib)
            var apronBib = GameObject.CreatePrimitive(PrimitiveType.Cube);
            apronBib.name = "Apron_Bib";
            apronBib.transform.SetParent(body.transform, false);
            apronBib.transform.localPosition = new Vector3(0f, 0.12f, 0.24f);
            apronBib.transform.localScale = new Vector3(0.42f, 0.32f, 0.08f);
            apronBib.GetComponent<Renderer>().sharedMaterial = mats.MinerApron;
            RemoveCollider(apronBib);

            // 2 Dây quai da vắt qua vai có đinh tán đồng
            for (int s = -1; s <= 1; s += 2)
            {
                var strap = GameObject.CreatePrimitive(PrimitiveType.Cube);
                strap.name = $"Apron_Strap_{s}";
                strap.transform.SetParent(body.transform, false);
                strap.transform.localPosition = new Vector3(s * 0.16f, 0.28f, 0.06f);
                strap.transform.localRotation = Quaternion.Euler(35f, 0f, 0f);
                strap.transform.localScale = new Vector3(0.08f, 0.38f, 0.04f);
                strap.GetComponent<Renderer>().sharedMaterial = mats.MinerApron;
                RemoveCollider(strap);

                var rivet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                rivet.name = $"Strap_Rivet_{s}";
                rivet.transform.SetParent(strap.transform, false);
                rivet.transform.localPosition = new Vector3(0f, -0.35f, 0.5f);
                rivet.transform.localScale = Vector3.one * 0.45f;
                rivet.GetComponent<Renderer>().sharedMaterial = mats.MinerBrass;
                RemoveCollider(rivet);
            }

            // Túi da trước bụng
            var pocket = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pocket.name = "Apron_Pocket";
            pocket.transform.SetParent(apron.transform, false);
            pocket.transform.localPosition = new Vector3(0f, -0.15f, 0.46f);
            pocket.transform.localScale = new Vector3(0.38f, 0.25f, 0.10f);
            pocket.GetComponent<Renderer>().sharedMaterial = mats.MinerApron;
            RemoveCollider(pocket);

            // Đai thắt lưng da thợ mỏ & Khóa đồng
            var belt = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            belt.name = "Miner_Belt";
            belt.transform.SetParent(body.transform, false);
            belt.transform.localPosition = new Vector3(0f, -0.02f, 0f);
            belt.transform.localScale = new Vector3(1.12f, 0.08f, 1.12f);
            belt.GetComponent<Renderer>().sharedMaterial = mats.MinerBoots;
            RemoveCollider(belt);

            var beltBuckle = GameObject.CreatePrimitive(PrimitiveType.Cube);
            beltBuckle.name = "Belt_BrassBuckle";
            beltBuckle.transform.SetParent(belt.transform, false);
            beltBuckle.transform.localPosition = new Vector3(0f, 0f, 0.52f);
            beltBuckle.transform.localScale = new Vector3(0.22f, 1.2f, 0.12f);
            beltBuckle.GetComponent<Renderer>().sharedMaterial = mats.MinerBrass;
            RemoveCollider(beltBuckle);

            // Búa nhỏ thợ rèn dắt bên hông thắt lưng
            var miniHammer = new GameObject("Belt_MiniHammer");
            miniHammer.transform.SetParent(belt.transform, false);
            miniHammer.transform.localPosition = new Vector3(0.42f, -0.15f, 0.22f);
            miniHammer.transform.localRotation = Quaternion.Euler(15f, 0f, 25f);

            var hammerHandle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            hammerHandle.transform.SetParent(miniHammer.transform, false);
            hammerHandle.transform.localScale = new Vector3(0.04f, 0.22f, 0.04f);
            hammerHandle.GetComponent<Renderer>().sharedMaterial = mats.PickaxeWood;
            RemoveCollider(hammerHandle);

            var hammerHead = GameObject.CreatePrimitive(PrimitiveType.Cube);
            hammerHead.transform.SetParent(miniHammer.transform, false);
            hammerHead.transform.localPosition = new Vector3(0f, 0.2f, 0f);
            hammerHead.transform.localScale = new Vector3(0.14f, 0.08f, 0.09f);
            hammerHead.GetComponent<Renderer>().sharedMaterial = mats.StoneDark;
            RemoveCollider(hammerHead);

            // Viên ngọc phù chú Rune xanh ngọc treo bên hông
            var runeCharm = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            runeCharm.name = "Belt_RuneCharm";
            runeCharm.transform.SetParent(belt.transform, false);
            runeCharm.transform.localPosition = new Vector3(-0.38f, -0.22f, 0.32f);
            runeCharm.transform.localScale = new Vector3(0.12f, 0.16f, 0.10f);
            runeCharm.GetComponent<Renderer>().sharedMaterial = mats.MinerRuneCharm;
            RemoveCollider(runeCharm);

            // C. ĐẦU & KHUÔN MẶT CHIBI ANIME THÂN THIỆN
            var head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            head.name = "Miner_Head";
            head.transform.SetParent(heroRoot.transform, false);
            head.transform.localPosition = new Vector3(0f, 0.96f, 0.02f);
            head.transform.localScale = new Vector3(0.72f, 0.65f, 0.68f);
            head.GetComponent<Renderer>().sharedMaterial = mats.MinerSkin;
            RemoveCollider(head);

            // 2 Má bầu bĩnh ửng hồng phớt nhẹ (Chubby Chibi Cheeks)
            for (int s = -1; s <= 1; s += 2)
            {
                var cheek = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                cheek.name = $"Cheek_{(s < 0 ? "L" : "R")}";
                cheek.transform.SetParent(head.transform, false);
                cheek.transform.localPosition = new Vector3(s * 0.28f, -0.15f, 0.32f);
                cheek.transform.localScale = new Vector3(0.25f, 0.20f, 0.22f);
                cheek.GetComponent<Renderer>().sharedMaterial = mats.MinerSkin;
                RemoveCollider(cheek);

                var blush = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                blush.name = $"Blush_{(s < 0 ? "L" : "R")}";
                blush.transform.SetParent(cheek.transform, false);
                blush.transform.localPosition = new Vector3(0f, 0.05f, 0.35f);
                blush.transform.localScale = new Vector3(0.65f, 0.45f, 0.25f);
                blush.GetComponent<Renderer>().sharedMaterial = mats.MinerBlush;
                RemoveCollider(blush);
            }

            // Sống mũi nhỏ xinh và nụ cười mỉm thân thiện
            var nose = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            nose.name = "Nose";
            nose.transform.SetParent(head.transform, false);
            nose.transform.localPosition = new Vector3(0f, -0.06f, 0.48f);
            nose.transform.localScale = new Vector3(0.09f, 0.08f, 0.09f);
            nose.GetComponent<Renderer>().sharedMaterial = mats.MinerSkin;
            RemoveCollider(nose);

            var mouth = GameObject.CreatePrimitive(PrimitiveType.Cube);
            mouth.name = "SmileMouth";
            mouth.transform.SetParent(head.transform, false);
            mouth.transform.localPosition = new Vector3(0f, -0.22f, 0.45f);
            mouth.transform.localScale = new Vector3(0.18f, 0.04f, 0.05f);
            mouth.GetComponent<Renderer>().sharedMaterial = mats.MinerHair;
            RemoveCollider(mouth);

            // 2 MẮT ANIME TO TRÒN LONG LANH (Anime Eyes with Highlights)
            for (int s = -1; s <= 1; s += 2)
            {
                var eyeRoot = new GameObject(s < 0 ? "Eye_Left" : "Eye_Right");
                eyeRoot.transform.SetParent(head.transform, false);
                eyeRoot.transform.localPosition = new Vector3(s * 0.19f, 0.02f, 0.42f);
                eyeRoot.transform.localRotation = Quaternion.Euler(0f, s * 8f, 0f);

                // Lòng trắng mắt
                var eyeWhite = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                eyeWhite.name = "EyeWhite";
                eyeWhite.transform.SetParent(eyeRoot.transform, false);
                eyeWhite.transform.localScale = new Vector3(0.17f, 0.20f, 0.08f);
                eyeWhite.GetComponent<Renderer>().sharedMaterial = mats.MinerEyeWhite;
                RemoveCollider(eyeWhite);

                // Tròng đen to tròn hạt dẻ
                var iris = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                iris.name = "Iris";
                iris.transform.SetParent(eyeRoot.transform, false);
                iris.transform.localPosition = new Vector3(0f, -0.01f, 0.035f);
                iris.transform.localScale = new Vector3(0.13f, 0.16f, 0.07f);
                iris.GetComponent<Renderer>().sharedMaterial = mats.MinerEyes;
                RemoveCollider(iris);

                // Điểm sáng phản quang long lanh (Catchlight Specular)
                var spec1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                spec1.name = "Catchlight_1";
                spec1.transform.SetParent(iris.transform, false);
                spec1.transform.localPosition = new Vector3(0.28f, 0.28f, 0.35f);
                spec1.transform.localScale = Vector3.one * 0.35f;
                spec1.GetComponent<Renderer>().sharedMaterial = mats.MinerEyeWhite;
                RemoveCollider(spec1);

                var spec2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                spec2.name = "Catchlight_2";
                spec2.transform.SetParent(iris.transform, false);
                spec2.transform.localPosition = new Vector3(-0.25f, -0.25f, 0.35f);
                spec2.transform.localScale = Vector3.one * 0.22f;
                spec2.GetComponent<Renderer>().sharedMaterial = mats.MinerEyeWhite;
                RemoveCollider(spec2);

                // Hàng mi trên cong nhẹ
                var lash = GameObject.CreatePrimitive(PrimitiveType.Cube);
                lash.name = "UpperLash";
                lash.transform.SetParent(eyeRoot.transform, false);
                lash.transform.localPosition = new Vector3(0f, 0.11f, 0.03f);
                lash.transform.localRotation = Quaternion.Euler(0f, 0f, s * -12f);
                lash.transform.localScale = new Vector3(0.18f, 0.035f, 0.06f);
                lash.GetComponent<Renderer>().sharedMaterial = mats.MinerHair;
                RemoveCollider(lash);
            }

            // MÁI TÓC NÂU ĐỎ ANIME (Reddish-Brown Spiky Hair Bangs)
            var hairBack = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            hairBack.name = "Hair_Back";
            hairBack.transform.SetParent(head.transform, false);
            hairBack.transform.localPosition = new Vector3(0f, 0.08f, -0.10f);
            hairBack.transform.localScale = new Vector3(1.06f, 1.02f, 1.10f);
            hairBack.GetComponent<Renderer>().sharedMaterial = mats.MinerHair;
            RemoveCollider(hairBack);

            // Các lọn tóc mái nhọn tỉa tự nhiên xõa dưới vành mũ
            float[] bangOffsets = new float[] { -0.22f, -0.11f, 0.0f, 0.11f, 0.22f };
            float[] bangAngles = new float[] { -15f, -8f, 0f, 8f, 15f };
            for (int b = 0; b < bangOffsets.Length; b++)
            {
                var bang = GameObject.CreatePrimitive(PrimitiveType.Cube);
                bang.name = $"HairBang_{b}";
                bang.transform.SetParent(head.transform, false);
                bang.transform.localPosition = new Vector3(bangOffsets[b], 0.22f, 0.45f);
                bang.transform.localRotation = Quaternion.Euler(25f, 0f, bangAngles[b]);
                bang.transform.localScale = new Vector3(0.11f, 0.22f, 0.09f);
                bang.GetComponent<Renderer>().sharedMaterial = mats.MinerHair;
                RemoveCollider(bang);
            }

            // 2 Lọn tóc mai hai bên má
            for (int s = -1; s <= 1; s += 2)
            {
                var sideburn = GameObject.CreatePrimitive(PrimitiveType.Cube);
                sideburn.name = $"Sideburn_{(s < 0 ? "L" : "R")}";
                sideburn.transform.SetParent(head.transform, false);
                sideburn.transform.localPosition = new Vector3(s * 0.40f, 0.02f, 0.15f);
                sideburn.transform.localRotation = Quaternion.Euler(15f, 0f, s * -20f);
                sideburn.transform.localScale = new Vector3(0.12f, 0.32f, 0.15f);
                sideburn.GetComponent<Renderer>().sharedMaterial = mats.MinerHair;
                RemoveCollider(sideburn);
            }

            // D. MŨ BẢO HỘ OVERSIZE & ĐÈN TINH THỂ PHÁT SÁNG (Oversized Miner Hat with Glowing Crystal Lamp)
            var helmet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            helmet.name = "Miner_OversizedHardHat";
            helmet.transform.SetParent(head.transform, false);
            helmet.transform.localPosition = new Vector3(0f, 0.32f, -0.02f);
            helmet.transform.localScale = new Vector3(1.25f, 0.95f, 1.28f);
            helmet.GetComponent<Renderer>().sharedMaterial = mats.MinerHelmet;
            RemoveCollider(helmet);

            // Gờ gân mũ đá nổi giữa đỉnh mũ
            var ridge = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ridge.name = "Helmet_CenterRidge";
            ridge.transform.SetParent(helmet.transform, false);
            ridge.transform.localPosition = new Vector3(0f, 0.45f, 0f);
            ridge.transform.localScale = new Vector3(0.22f, 0.25f, 0.95f);
            ridge.GetComponent<Renderer>().sharedMaterial = mats.MinerHelmet;
            RemoveCollider(ridge);

            // Vành mũ rộng che nửa trán (Brim)
            var brim = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            brim.name = "Helmet_Brim";
            brim.transform.SetParent(helmet.transform, false);
            brim.transform.localPosition = new Vector3(0f, -0.16f, 0.06f);
            brim.transform.localRotation = Quaternion.Euler(-14f, 0f, 0f);
            brim.transform.localScale = new Vector3(1.30f, 0.06f, 1.32f);
            brim.GetComponent<Renderer>().sharedMaterial = mats.MinerHelmet;
            RemoveCollider(brim);

            // Đai da chân mũ & Đinh tán đồng
            var helmetBand = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            helmetBand.name = "Helmet_LeatherBand";
            helmetBand.transform.SetParent(helmet.transform, false);
            helmetBand.transform.localPosition = new Vector3(0f, -0.10f, 0.02f);
            helmetBand.transform.localRotation = Quaternion.Euler(-14f, 0f, 0f);
            helmetBand.transform.localScale = new Vector3(1.26f, 0.08f, 1.28f);
            helmetBand.GetComponent<Renderer>().sharedMaterial = mats.MinerApron;
            RemoveCollider(helmetBand);

            // ĐÈN TINH THỂ PHA LÊ LAM PHÁT SÁNG TRÊN TRÁN MŨ (Glowing Blue Crystal Headlamp - Ảnh 1)
            var lampFrame = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            lampFrame.name = "Headlamp_BrassFrame";
            lampFrame.transform.SetParent(helmet.transform, false);
            lampFrame.transform.localPosition = new Vector3(0f, 0.08f, 0.58f);
            lampFrame.transform.localRotation = Quaternion.Euler(75f, 0f, 0f);
            lampFrame.transform.localScale = new Vector3(0.38f, 0.08f, 0.38f);
            lampFrame.GetComponent<Renderer>().sharedMaterial = mats.MinerBrass;
            RemoveCollider(lampFrame);

            // Lõi tinh thể pha lê cắt giác phát quang rực rỡ
            var crystalLamp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            crystalLamp.name = "Headlamp_CyanCrystalCore";
            crystalLamp.transform.SetParent(lampFrame.transform, false);
            crystalLamp.transform.localPosition = new Vector3(0f, 0.35f, 0f);
            crystalLamp.transform.localRotation = Quaternion.Euler(45f, 45f, 0f);
            crystalLamp.transform.localScale = new Vector3(0.55f, 0.55f, 0.55f);
            crystalLamp.GetComponent<Renderer>().sharedMaterial = mats.RunicCyan;
            RemoveCollider(crystalLamp);

            // Đèn rọi ánh sáng lam ma thuật về phía trước
            var lampLight = new GameObject("Headlamp_MagicLight");
            lampLight.transform.SetParent(crystalLamp.transform, false);
            lampLight.transform.localPosition = Vector3.zero;
            var lComp = lampLight.AddComponent<Light>();
            lComp.type = LightType.Point;
            lComp.color = new Color(0.15f, 0.90f, 1.0f);
            lComp.range = 4.2f;
            lComp.intensity = 2.4f;

            // E. ĐÔI CHÂN & ỦNG THỢ MỎ DÀY DẶN (Chunky Mining Boots)
            for (int s = -1; s <= 1; s += 2)
            {
                var leg = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                leg.name = $"Leg_{(s < 0 ? "L" : "R")}";
                leg.transform.SetParent(heroRoot.transform, false);
                leg.transform.localPosition = new Vector3(s * 0.22f, 0.22f, 0f);
                leg.transform.localScale = new Vector3(0.22f, 0.22f, 0.22f);
                leg.GetComponent<Renderer>().sharedMaterial = mats.MinerTunic;
                RemoveCollider(leg);

                var boot = GameObject.CreatePrimitive(PrimitiveType.Cube);
                boot.name = $"Boot_{(s < 0 ? "L" : "R")}";
                boot.transform.SetParent(leg.transform, false);
                boot.transform.localPosition = new Vector3(0f, -0.65f, 0.32f);
                boot.transform.localScale = new Vector3(1.18f, 0.75f, 1.65f);
                boot.GetComponent<Renderer>().sharedMaterial = mats.MinerBoots;
                RemoveCollider(boot);

                var sole = GameObject.CreatePrimitive(PrimitiveType.Cube);
                sole.name = "Sole";
                sole.transform.SetParent(boot.transform, false);
                sole.transform.localPosition = new Vector3(0f, -0.45f, 0f);
                sole.transform.localScale = new Vector3(1.08f, 0.25f, 1.08f);
                sole.GetComponent<Renderer>().sharedMaterial = mats.StoneDark;
                RemoveCollider(sole);
            }

            // F. CÁNH TAY, VÒNG TAY CỔ TỰ & BÀN TAY (Arms & Runic Wristbands)
            for (int s = -1; s <= 1; s += 2)
            {
                var arm = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                arm.name = $"Arm_{(s < 0 ? "L" : "R")}";
                arm.transform.SetParent(heroRoot.transform, false);
                arm.transform.localPosition = new Vector3(s * 0.32f, 0.48f, 0.12f);
                arm.transform.localRotation = Quaternion.Euler(25f, s * -20f, s * 28f);
                arm.transform.localScale = new Vector3(0.14f, 0.22f, 0.14f);
                arm.GetComponent<Renderer>().sharedMaterial = mats.MinerTunic;
                RemoveCollider(arm);

                // Vòng da cổ tay có khắc ký tự Rune phát sáng
                var wrist = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                wrist.name = "RunicWristband";
                wrist.transform.SetParent(arm.transform, false);
                wrist.transform.localPosition = new Vector3(0f, -0.65f, 0f);
                wrist.transform.localScale = new Vector3(1.15f, 0.35f, 1.15f);
                wrist.GetComponent<Renderer>().sharedMaterial = mats.MinerApron;
                RemoveCollider(wrist);

                var runeGlow = GameObject.CreatePrimitive(PrimitiveType.Cube);
                runeGlow.name = "WristRuneGlyph";
                runeGlow.transform.SetParent(wrist.transform, false);
                runeGlow.transform.localPosition = new Vector3(0f, 0f, 0.52f);
                runeGlow.transform.localScale = new Vector3(0.45f, 0.55f, 0.12f);
                runeGlow.GetComponent<Renderer>().sharedMaterial = mats.RunicCyan;
                RemoveCollider(runeGlow);

                // Bàn tay màu da nắm cán cuốc
                var hand = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                hand.name = "Hand";
                hand.transform.SetParent(arm.transform, false);
                hand.transform.localPosition = new Vector3(0f, -0.95f, 0f);
                hand.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
                hand.GetComponent<Renderer>().sharedMaterial = mats.MinerSkin;
                RemoveCollider(hand);
            }

            // G. VŨ KHÍ: CUỐC KHAI KHOÁNG MA THUẬT LÕI PHA LÊ XANH (Magical Pickaxe with Glowing Core)
            var armRoot = new GameObject("WeaponArm");
            armRoot.transform.SetParent(heroRoot.transform, false);
            armRoot.transform.localPosition = new Vector3(0.08f, 0.46f, 0.22f);

            // Cán gỗ cong mộc mạc (Wooden Pickaxe Handle)
            var handle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            handle.name = "Pickaxe_Handle";
            handle.transform.SetParent(armRoot.transform, false);
            handle.transform.localPosition = new Vector3(-0.02f, 0.10f, 0.08f);
            handle.transform.localRotation = Quaternion.Euler(24f, -12f, 32f);
            handle.transform.localScale = new Vector3(0.075f, 0.52f, 0.075f);
            handle.GetComponent<Renderer>().sharedMaterial = mats.PickaxeWood;
            RemoveCollider(handle);

            // Khớp kim loại cổ cuốc
            var collar = GameObject.CreatePrimitive(PrimitiveType.Cube);
            collar.name = "Pickaxe_BrassCollar";
            collar.transform.SetParent(handle.transform, false);
            collar.transform.localPosition = new Vector3(0f, 0.95f, 0f);
            collar.transform.localScale = new Vector3(1.8f, 0.35f, 1.8f);
            collar.GetComponent<Renderer>().sharedMaterial = mats.MinerBrass;
            RemoveCollider(collar);

            // Đầu cuốc đá rèn hai mũi nhọn cong vút
            var pickHead = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pickHead.name = "Pickaxe_HeadStone";
            pickHead.transform.SetParent(handle.transform, false);
            pickHead.transform.localPosition = new Vector3(0f, 1.05f, 0f);
            pickHead.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            pickHead.transform.localScale = new Vector3(3.6f, 1.25f, 1.35f);
            pickHead.GetComponent<Renderer>().sharedMaterial = mats.PickaxeStone;
            RemoveCollider(pickHead);

            // LÕI TINH THỂ PHA LÊ XANH PHÁT SÁNG NẰM TRONG LÒNG ĐẦU CUỐC (Glowing Cyan Core - Ảnh 1)
            var crystalCore = GameObject.CreatePrimitive(PrimitiveType.Cube);
            crystalCore.name = "Pickaxe_GlowingCrystalCore";
            crystalCore.transform.SetParent(pickHead.transform, false);
            crystalCore.transform.localPosition = Vector3.zero;
            crystalCore.transform.localRotation = Quaternion.Euler(45f, 45f, 0f);
            crystalCore.transform.localScale = new Vector3(0.95f, 1.08f, 0.95f);
            crystalCore.GetComponent<Renderer>().sharedMaterial = mats.PickaxeCyanCore;
            RemoveCollider(crystalCore);

            // Ánh sáng lam ma thuật rực rỡ tỏa ra từ đầu cuốc
            var pickLight = new GameObject("Pickaxe_CrystalLight");
            pickLight.transform.SetParent(pickHead.transform, false);
            pickLight.transform.localPosition = Vector3.zero;
            var pL = pickLight.AddComponent<Light>();
            pL.type = LightType.Point;
            pL.color = new Color(0.15f, 0.92f, 1.0f);
            pL.range = 3.2f;
            pL.intensity = 2.8f;

            // H. GẮN COMPONENT COMBATUNIT3D ĐỂ CHIẾN ĐẤU & GÕ TINH THỂ
            var unit = heroRoot.AddComponent<CombatUnit3D>();
            var uSo = new SerializedObject(unit);
            uSo.FindProperty("isHero").boolValue = true;
            uSo.FindProperty("weaponTransform").objectReferenceValue = armRoot.transform;
            uSo.ApplyModifiedProperties();
            unit.CacheRenderers();

            // Gắn thanh máu dũng sĩ trên đầu
            AttachOverheadHealthBar(heroRoot);

            return heroRoot;
        }

        private static void AttachOverheadHealthBar(GameObject target)
        {
            var barObj = new GameObject("OverheadHealthBar");
            barObj.transform.SetParent(target.transform, false);
            barObj.transform.localPosition = new Vector3(0f, 1.75f, 0f);

            var bar = barObj.AddComponent<OverheadHealthBar3D>();
            var bSo = new SerializedObject(bar);
            bSo.FindProperty("barOwner").enumValueIndex = (int)HealthBarOwner.Player;
            bSo.ApplyModifiedProperties();
        }

        private static GameObject InstantiatePlayerHeroModel(Transform parent)
        {
            var heroPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Player_Hero.prefab");
            GameObject heroRoot;
            if (heroPrefab != null)
            {
                heroRoot = (GameObject)PrefabUtility.InstantiatePrefab(heroPrefab);
                if (parent != null) heroRoot.transform.SetParent(parent, false);
            }
            else
            {
                heroRoot = new GameObject("Player_CapybaraHero");
                if (parent != null) heroRoot.transform.SetParent(parent, false);
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
                var uSo = new SerializedObject(unit);
                uSo.FindProperty("isHero").boolValue = true;
                uSo.FindProperty("weaponTransform").objectReferenceValue = null;
                uSo.ApplyModifiedProperties();
                unit.CacheRenderers();

                AttachOverheadHealthBar(heroRoot);
            }

            heroRoot.transform.localPosition = new Vector3(-1.6f, 0.0f, -1.4f);
            heroRoot.transform.localRotation = Quaternion.Euler(0f, 155f, 0f);
            heroRoot.transform.localScale = Vector3.one * 1.15f;
            heroRoot.tag = "Player";

            var combatUnit = heroRoot.GetComponent<CombatUnit3D>();
            if (combatUnit != null) combatUnit.CacheRenderers();

            return heroRoot;
        }
        #endregion

        #region 6. KẾT NỐI HỆ THỐNG QUẢN LÝ & CAMERA ISOMETRIC AAA
        private static void WireSystemsAndManagers(GameObject crystalCoreObj, GameObject heroObj)
        {
            // Kết nối Adventure3DManager
            var managersObj = GameObject.Find("_Managers");
            if (managersObj == null) managersObj = new GameObject("_Managers");

            var adv3D = managersObj.GetComponent<Adventure3DManager>();
            if (adv3D == null) adv3D = managersObj.AddComponent<Adventure3DManager>();

            var advSo = new SerializedObject(adv3D);
            advSo.FindProperty("heroUnit").objectReferenceValue = heroObj.GetComponent<CombatUnit3D>();
            advSo.FindProperty("idleForgePos").vector3Value = new Vector3(-1.6f, 0.0f, -1.4f);
            advSo.FindProperty("idleForgeRot").quaternionValue = Quaternion.Euler(0f, 155f, 0f);
            advSo.FindProperty("heroCombatPos").vector3Value = new Vector3(-1.5f, 0.05f, -0.8f);
            advSo.FindProperty("enemyCombatPos").vector3Value = new Vector3(1.5f, 0.05f, -0.8f);
            advSo.FindProperty("forgeCenter").vector3Value = new Vector3(0f, 0.05f, 0f);
            advSo.ApplyModifiedProperties();

            // Kết nối CrystalDefenseManager
            var defense = managersObj.GetComponent<CrystalDefenseManager>();
            if (defense != null)
            {
                var defSo = new SerializedObject(defense);
                defSo.FindProperty("heroUnit").objectReferenceValue = heroObj.GetComponent<CombatUnit3D>();
                defSo.FindProperty("crystalCoreTransform").objectReferenceValue = crystalCoreObj.transform;
                var enemyCont = GameObject.Find("EnemyContainer");
                if (enemyCont != null) defSo.FindProperty("enemyContainer").objectReferenceValue = enemyCont.transform;
                defSo.ApplyModifiedProperties();
            }

            // Kết nối ClickSystem
            var clickSys = crystalCoreObj.GetComponent<ClickSystem>();
            if (clickSys == null) clickSys = crystalCoreObj.AddComponent<ClickSystem>();
        }

        private static void SetupIsometricCameraAndPostProcessing()
        {
            var camObj = Camera.main != null ? Camera.main.gameObject : GameObject.FindWithTag("MainCamera");
            if (camObj == null)
            {
                camObj = new GameObject("Main Camera");
                camObj.tag = "MainCamera";
                camObj.AddComponent<Camera>();
            }

            var cam = camObj.GetComponent<Camera>();
            camObj.transform.position = new Vector3(2.08f, 5.05f, -7.02f);
            camObj.transform.rotation = Quaternion.Euler(27f, -16f, 0f);
            cam.fieldOfView = 56f;
            cam.nearClipPlane = 0.3f;
            cam.farClipPlane = 100f;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.12f, 0.14f, 0.18f);

            // Nâng cấp Post-Processing Profile (Bloom, Contrast, Vignette)
            var volumeObj = GameObject.Find("GlobalVolume");
            if (volumeObj == null)
            {
                volumeObj = new GameObject("GlobalVolume");
                var vol = volumeObj.AddComponent<Volume>();
                vol.isGlobal = true;
            }

            var volume = volumeObj.GetComponent<Volume>();
            if (volume.profile == null)
            {
                volume.profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>("Assets/Settings/SampleSceneProfile.asset");
            }

            if (volume.profile != null)
            {
                if (!volume.profile.TryGet<Bloom>(out var bloom))
                {
                    bloom = volume.profile.Add<Bloom>(true);
                }
                bloom.threshold.value = 0.65f;
                bloom.intensity.value = 1.45f;
                bloom.scatter.value = 0.72f;

                if (!volume.profile.TryGet<ColorAdjustments>(out var colorAdj))
                {
                    colorAdj = volume.profile.Add<ColorAdjustments>(true);
                }
                colorAdj.contrast.value = 18f;
                colorAdj.saturation.value = 16f;

                EditorUtility.SetDirty(volume.profile);
            }
        }
        #endregion

        #region 7. HỖ TRỢ TIỆN ÍCH & TẠO VẬT LIỆU
        private static void CleanOldHeroAndCores()
        {
            var oldObjects = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include);
            foreach (var obj in oldObjects)
            {
                if (obj == null) continue;
                string n = obj.name;
                if (n == "Player_CapybaraHero" || n == "CrystalCore" || n == "Central_Pedestal" || n == "NexusFloatingIsland")
                {
                    Object.DestroyImmediate(obj);
                }
            }
        }

        private static void RemoveCollider(GameObject go)
        {
            var col = go.GetComponent<Collider>();
            if (col != null) Object.DestroyImmediate(col);
        }

        private static void EnsureFolders()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Materials")) AssetDatabase.CreateFolder("Assets", "Materials");
            if (!AssetDatabase.IsValidFolder("Assets/Data")) AssetDatabase.CreateFolder("Assets", "Data");
        }

        private static Material GetOrCreateLitMaterial(string matName, Color baseColor, float smoothness)
        {
            string path = $"Assets/Materials/{matName}.mat";
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null)
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
                mat = new Material(shader);
                AssetDatabase.CreateAsset(mat, path);
            }

            mat.color = baseColor;
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", baseColor);
            if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", smoothness);
            EditorUtility.SetDirty(mat);
            return mat;
        }

        private static Material GetOrCreateEmissionMaterial(string matName, Color baseColor, Color emissionColor, float intensity)
        {
            string path = $"Assets/Materials/{matName}.mat";
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null)
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
                mat = new Material(shader);
                AssetDatabase.CreateAsset(mat, path);
            }

            mat.color = baseColor;
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", baseColor);
            if (mat.HasProperty("_EmissionColor"))
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", emissionColor * intensity);
            }
            EditorUtility.SetDirty(mat);
            return mat;
        }

        private static void SanitizeSceneTexts()
        {
            var allTmps = Object.FindObjectsByType<TMPro.TextMeshProUGUI>(FindObjectsInactive.Include);
            foreach (var tmp in allTmps)
            {
                if (tmp != null && !string.IsNullOrEmpty(tmp.text))
                {
                    tmp.text = FontSanitizer.Sanitize(tmp.text);
                    EditorUtility.SetDirty(tmp);
                }
            }
        }
        #endregion
    }
}
#endif
