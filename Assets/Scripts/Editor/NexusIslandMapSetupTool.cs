#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using TMPro;
using CrystalForge.Adventure;
using CrystalForge.Visuals;
using CrystalForge.Utils;

namespace CrystalForge.Editor
{
    public static class NexusIslandMapSetupTool
    {
        [MenuItem("CrystalForge/Build Nexus Crystal Island Map (Exact Concept Art)")]
        public static void BuildNexusIslandMap()
        {
            Debug.Log("<color=#FFD166><b>[CrystalForge]</b> Bắt đầu xây dựng Bản đồ Đảo Tinh Thể Nexus chuẩn theo hình ảnh Concept của Chủ nhân...</color>");

            // 1. DỌN DẸP MÔI TRƯỜNG CŨ TRONG Environment_Diorama
            var env = GameObject.Find("Environment_Diorama");
            if (env == null)
            {
                env = new GameObject("Environment_Diorama");
            }

            // Xóa các object con cũ để tái tạo mới 100% chuẩn ảnh concept
            for (int i = env.transform.childCount - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(env.transform.GetChild(i).gameObject);
            }

            // 2. TẠO VẬT LIỆU CHUẨN STYLIZED CONCEPT ART
            var matStoneCobble = CreateLitMaterial("Mat_NexusCobblestone", new Color(0.60f, 0.56f, 0.68f), 0.25f); // Đá tím xám ấm
            var matStoneDark = CreateLitMaterial("Mat_NexusStoneDark", new Color(0.42f, 0.38f, 0.50f), 0.20f);   // Đá bậc dưới
            var matMossEdge = CreateLitMaterial("Mat_NexusMossEdge", new Color(0.32f, 0.52f, 0.36f), 0.20f);    // Rêu xanh rìa đảo
            var matPedestalWood = CreateLitMaterial("Mat_NexusPedestalWood", new Color(0.48f, 0.28f, 0.16f), 0.35f); // Gỗ bệ trụ
            var matGoldTrim = CreateEmissionMaterial("Mat_NexusGoldRune", new Color(0.95f, 0.75f, 0.30f), new Color(1f, 0.85f, 0.35f), 1.8f); // Chữ Rune phát sáng vàng
            var matRuneStone = CreateLitMaterial("Mat_NexusRuneStone", new Color(0.55f, 0.50f, 0.62f), 0.25f);
            var matBush = CreateLitMaterial("Mat_NexusBush", new Color(0.28f, 0.55f, 0.40f), 0.15f);
            var matFlowerPink = CreateEmissionMaterial("Mat_NexusFlowerPink", new Color(1f, 0.70f, 0.85f), new Color(1f, 0.80f, 0.90f), 1.2f);
            var matFlowerYellow = CreateEmissionMaterial("Mat_NexusFlowerYellow", new Color(1f, 0.90f, 0.40f), new Color(1f, 0.95f, 0.50f), 1.2f);
            var matVineWood = CreateLitMaterial("Mat_NexusVineWood", new Color(0.65f, 0.48f, 0.32f), 0.25f); // Rễ cây uốn lượn ôm trụ
            var matEnergyRing = CreateEmissionMaterial("Mat_NexusEnergyRing", new Color(0.2f, 0.95f, 1f, 0.8f), new Color(0.3f, 1f, 1f), 2.5f); // Vòng hào quang cyan

            // 3. XÂY DỰNG ĐẢO NỔI BẬC ĐÁ TRÒN ĐỒNG TÂM (CONCENTRIC COBBLESTONE ISLAND)
            var islandRoot = new GameObject("NexusFloatingIsland");
            islandRoot.transform.SetParent(env.transform, false);

            // Bậc đáy đảo (Under Island Base & Soil)
            var islandBase = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            islandBase.name = "Island_UnderBase";
            islandBase.transform.SetParent(islandRoot.transform, false);
            islandBase.transform.localPosition = new Vector3(0f, -0.7f, 0f);
            islandBase.transform.localScale = new Vector3(8.6f, 0.7f, 8.6f);
            islandBase.GetComponent<Renderer>().sharedMaterial = matStoneDark;
            RemoveCollider(islandBase);

            // Tầng 1: Vành đá ngoài cùng (Outer Cobble Ring)
            var ring1 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ring1.name = "Island_CobbleRing_1";
            ring1.transform.SetParent(islandRoot.transform, false);
            ring1.transform.localPosition = new Vector3(0f, -0.05f, 0f);
            ring1.transform.localScale = new Vector3(8.2f, 0.15f, 8.2f);
            ring1.GetComponent<Renderer>().sharedMaterial = matStoneCobble;
            RemoveCollider(ring1);

            // Rìa rêu cỏ xanh (Moss Fringe Ring)
            var mossFringe = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            mossFringe.name = "Island_MossFringe";
            mossFringe.transform.SetParent(islandRoot.transform, false);
            mossFringe.transform.localPosition = new Vector3(0f, -0.02f, 0f);
            mossFringe.transform.localScale = new Vector3(8.0f, 0.15f, 8.0f);
            mossFringe.GetComponent<Renderer>().sharedMaterial = matMossEdge;
            RemoveCollider(mossFringe);

            // Tầng 2: Vòng đá bậc giữa có chữ cổ ngữ (Mid Ring with Runes)
            var ring2 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ring2.name = "Island_CobbleRing_2";
            ring2.transform.SetParent(islandRoot.transform, false);
            ring2.transform.localPosition = new Vector3(0f, 0.08f, 0f);
            ring2.transform.localScale = new Vector3(6.5f, 0.12f, 6.5f);
            ring2.GetComponent<Renderer>().sharedMaterial = matStoneCobble;
            RemoveCollider(ring2);

            // Tầng 3: Vòng đá trong cùng sát bệ (Inner Stone Ring)
            var ring3 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ring3.name = "Island_CobbleRing_3";
            ring3.transform.SetParent(islandRoot.transform, false);
            ring3.transform.localPosition = new Vector3(0f, 0.16f, 0f);
            ring3.transform.localScale = new Vector3(4.8f, 0.12f, 4.8f);
            ring3.GetComponent<Renderer>().sharedMaterial = matStoneCobble;
            RemoveCollider(ring3);

            // Tạo các vết ghép phiến đá tròn đồng tâm (Concentric Slabs Detail)
            CreateCobblestoneSlabs(islandRoot.transform, matStoneDark, matGoldTrim);

            // 4. CẦU ĐÁ NHỎ VƯƠN RA BÊN TRÁI (LEFT STONE BRIDGE)
            CreateStoneBridge(islandRoot.transform, matStoneCobble, matStoneDark);

            // 5. CÁC BIA ĐÁ CỔ NGỮ DỰNG ĐỨNG QUANH VIỀN ĐẢO (RUNE MONOLITHS)
            CreateRuneMonoliths(islandRoot.transform, matRuneStone, matGoldTrim);

            // 6. BỤI CÂY TRÒN & HOA DẠI NHỎ XINH (BUSHES & BLOSSOMS)
            CreateBushesAndFlowers(islandRoot.transform, matBush, matFlowerPink, matFlowerYellow);

            // 7. TÁI TẠO BỆ GỖ BÁT GIÁC & RỄ CÂY HOA ÔM TRỤ TINH THỂ (NEXUS PEDESTAL & ROOTS)
            CreateNexusPedestalAndRoots(env.transform, matPedestalWood, matGoldTrim, matVineWood, matFlowerPink, matFlowerYellow, matEnergyRing);

            // 8. ĐẢO NỔI MINI PHÍA XA (DISTANT FLOATING MINI-ISLANDS)
            CreateDistantMiniIslands(env.transform, matStoneDark, matMossEdge, matRuneStone);

            // 9. CẬP NHẬT TRỤ TINH THỂ PHÁT SÁNG VÀNG CAM + CYAN (DUAL GLOW CRYSTAL)
            UpdateCrystalCoreVisual();

            // 10. ĐIỀU CHỈNH BẦU TRỜI & ÁNH SÁNG HOÀNG HÔN TÍM LAM (DREAMY TWILIGHT)
            SetupDreamyTwilightLighting();

            // 11. CẬP NHẬT VỆ BINH CAPYBARA (CAPPY VƯƠNG MIỆN & BÚA MA THUẬT)
            SetupCapybaraHeroCosmetics(matGoldTrim, matEnergyRing);

            // Lưu Scene
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();

            Debug.Log("<color=#00FF88><b>[CrystalForge]</b> Xây dựng hoàn tất 100%! Bản đồ Đảo Tinh Thể Nexus tuyệt đẹp đúng y chang ảnh Concept!</color>");
        }

        private static void CreateCobblestoneSlabs(Transform parent, Material darkMat, Material runeMat)
        {
            var slabsRoot = new GameObject("Cobblestone_RuneAccents");
            slabsRoot.transform.SetParent(parent, false);

            // Vòng rune phát sáng trên các phiến đá (giống trong tranh)
            int runeCount = 12;
            float radius = 2.85f;
            for (int i = 0; i < runeCount; i++)
            {
                float angle = i * (Mathf.PI * 2f / runeCount);
                Vector3 pos = new Vector3(Mathf.Cos(angle) * radius, 0.19f, Mathf.Sin(angle) * radius);

                var runeSlab = GameObject.CreatePrimitive(PrimitiveType.Cube);
                runeSlab.name = $"RuneNode_{i}";
                runeSlab.transform.SetParent(slabsRoot.transform, false);
                runeSlab.transform.localPosition = pos;
                runeSlab.transform.localRotation = Quaternion.Euler(0f, -angle * Mathf.Rad2Deg + 90f, 0f);
                runeSlab.transform.localScale = new Vector3(0.55f, 0.05f, 0.35f);
                runeSlab.GetComponent<Renderer>().sharedMaterial = (i % 2 == 0) ? runeMat : darkMat;
                RemoveCollider(runeSlab);
            }
        }

        private static void CreateStoneBridge(Transform parent, Material cobbleMat, Material darkMat)
        {
            var bridge = new GameObject("Bridge_LeftArch");
            bridge.transform.SetParent(parent, false);
            bridge.transform.localPosition = new Vector3(-4.8f, 0.15f, 1.2f);
            bridge.transform.localRotation = Quaternion.Euler(0f, 40f, 0f);

            // Mặt cầu
            var floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "BridgeFloor";
            floor.transform.SetParent(bridge.transform, false);
            floor.transform.localScale = new Vector3(1.6f, 0.25f, 3.2f);
            floor.GetComponent<Renderer>().sharedMaterial = cobbleMat;
            RemoveCollider(floor);

            // Lan can đá bên trái
            var railL = GameObject.CreatePrimitive(PrimitiveType.Cube);
            railL.name = "Rail_Left";
            railL.transform.SetParent(bridge.transform, false);
            railL.transform.localPosition = new Vector3(-0.75f, 0.35f, 0f);
            railL.transform.localScale = new Vector3(0.25f, 0.5f, 3.2f);
            railL.GetComponent<Renderer>().sharedMaterial = darkMat;
            RemoveCollider(railL);

            // Lan can đá bên phải
            var railR = GameObject.CreatePrimitive(PrimitiveType.Cube);
            railR.name = "Rail_Right";
            railR.transform.SetParent(bridge.transform, false);
            railR.transform.localPosition = new Vector3(0.75f, 0.35f, 0f);
            railR.transform.localScale = new Vector3(0.25f, 0.5f, 3.2f);
            railR.GetComponent<Renderer>().sharedMaterial = darkMat;
            RemoveCollider(railR);
        }

        private static void CreateRuneMonoliths(Transform parent, Material stoneMat, Material runeMat)
        {
            var monoliths = new GameObject("RuneMonoliths_Group");
            monoliths.transform.SetParent(parent, false);

            Vector3[] positions = new Vector3[]
            {
                new Vector3(-3.6f, 0.15f, -1.8f),
                new Vector3(-2.2f, 0.15f, -3.4f),
                new Vector3(2.4f, 0.15f, -3.3f),
                new Vector3(3.8f, 0.15f, -1.5f),
                new Vector3(3.6f, 0.15f, 1.8f),
                new Vector3(-3.2f, 0.15f, 2.6f)
            };

            for (int i = 0; i < positions.Length; i++)
            {
                var m = new GameObject($"RuneMonolith_{i}");
                m.transform.SetParent(monoliths.transform, false);
                m.transform.localPosition = positions[i];

                Vector3 faceCenter = (Vector3.zero - positions[i]).normalized;
                m.transform.localRotation = Quaternion.LookRotation(faceCenter, Vector3.up) * Quaternion.Euler(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f));

                // Thân bia đá
                var body = GameObject.CreatePrimitive(PrimitiveType.Cube);
                body.name = "StoneBody";
                body.transform.SetParent(m.transform, false);
                body.transform.localPosition = new Vector3(0f, 0.55f, 0f);
                body.transform.localScale = new Vector3(0.45f, 1.1f, 0.28f);
                body.GetComponent<Renderer>().sharedMaterial = stoneMat;
                RemoveCollider(body);

                // Chữ Rune phát sáng vàng trên mặt bia đá
                var runeSign = GameObject.CreatePrimitive(PrimitiveType.Cube);
                runeSign.name = "RuneGlow";
                runeSign.transform.SetParent(m.transform, false);
                runeSign.transform.localPosition = new Vector3(0f, 0.6f, 0.15f);
                runeSign.transform.localScale = new Vector3(0.18f, 0.35f, 0.04f);
                runeSign.GetComponent<Renderer>().sharedMaterial = runeMat;
                RemoveCollider(runeSign);
            }
        }

        private static void CreateBushesAndFlowers(Transform parent, Material bushMat, Material flowerPink, Material flowerYellow)
        {
            var flora = new GameObject("Flora_BushesAndFlowers");
            flora.transform.SetParent(parent, false);

            Vector3[] bushPositions = new Vector3[]
            {
                new Vector3(-3.8f, 0.1f, -1.0f),
                new Vector3(-1.2f, 0.1f, -3.6f),
                new Vector3(1.2f, 0.1f, -3.6f),
                new Vector3(3.9f, 0.1f, -0.6f),
                new Vector3(2.8f, 0.1f, 2.8f),
                new Vector3(-2.6f, 0.1f, 3.2f)
            };

            for (int i = 0; i < bushPositions.Length; i++)
            {
                var bush = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                bush.name = $"StylizedBush_{i}";
                bush.transform.SetParent(flora.transform, false);
                bush.transform.localPosition = bushPositions[i] + Vector3.up * 0.25f;
                bush.transform.localScale = new Vector3(0.85f, 0.55f, 0.85f);
                bush.GetComponent<Renderer>().sharedMaterial = bushMat;
                RemoveCollider(bush);

                // Điểm 2-3 bông hoa nhỏ trên bụi cây
                for (int f = 0; f < 2; f++)
                {
                    var fl = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    fl.name = $"Blossom_{f}";
                    fl.transform.SetParent(bush.transform, false);
                    fl.transform.localPosition = new Vector3((f == 0 ? 0.3f : -0.25f), 0.4f, (f == 0 ? 0.2f : -0.2f));
                    fl.transform.localScale = Vector3.one * 0.22f;
                    fl.GetComponent<Renderer>().sharedMaterial = (f == 0) ? flowerPink : flowerYellow;
                    RemoveCollider(fl);
                }
            }
        }

        private static void CreateNexusPedestalAndRoots(Transform env, Material woodMat, Material goldMat, Material rootMat, Material pinkFlower, Material yellowFlower, Material ringMat)
        {
            var pedestalRoot = new GameObject("Nexus_CentralPedestal");
            pedestalRoot.transform.SetParent(env, false);
            pedestalRoot.transform.localPosition = Vector3.zero;

            // Bệ bát giác lớn (Gỗ sồi ấm áp viền vàng)
            var baseOct = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            baseOct.name = "OctagonalBase_Wood";
            baseOct.transform.SetParent(pedestalRoot.transform, false);
            baseOct.transform.localPosition = new Vector3(0f, 0.4f, 0f);
            baseOct.transform.localScale = new Vector3(2.6f, 0.45f, 2.6f);
            baseOct.GetComponent<Renderer>().sharedMaterial = woodMat;
            RemoveCollider(baseOct);

            // Viền vàng kim khắc hoa văn trên bệ
            var goldTrim = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            goldTrim.name = "Pedestal_GoldTrim";
            goldTrim.transform.SetParent(pedestalRoot.transform, false);
            goldTrim.transform.localPosition = new Vector3(0f, 0.85f, 0f);
            goldTrim.transform.localScale = new Vector3(2.4f, 0.08f, 2.4f);
            goldTrim.GetComponent<Renderer>().sharedMaterial = goldMat;
            RemoveCollider(goldTrim);

            // Tạo các DÂY LEO & RỄ CÂY THẦN ôm quanh chân Trụ Tinh Thể (giống hình concept)
            int rootCount = 8;
            float rootRadius = 1.15f;
            for (int i = 0; i < rootCount; i++)
            {
                float angle = i * (Mathf.PI * 2f / rootCount);
                Vector3 rootPos = new Vector3(Mathf.Cos(angle) * rootRadius, 0.95f, Mathf.Sin(angle) * rootRadius);

                var rootSegment = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                rootSegment.name = $"MagicRoot_{i}";
                rootSegment.transform.SetParent(pedestalRoot.transform, false);
                rootSegment.transform.localPosition = rootPos;
                // Nghiêng nhẹ uốn vào thân pha lê
                Vector3 toCenter = (Vector3.zero - rootPos).normalized;
                rootSegment.transform.localRotation = Quaternion.LookRotation(toCenter, Vector3.up) * Quaternion.Euler(25f, 0f, 15f * Mathf.Sin(i));
                rootSegment.transform.localScale = new Vector3(0.18f, 0.65f, 0.18f);
                rootSegment.GetComponent<Renderer>().sharedMaterial = rootMat;
                RemoveCollider(rootSegment);

                // Điểm xuyết hoa đào / hoa tiên nở trên rễ cây
                var fl = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                fl.name = $"RootFlower_{i}";
                fl.transform.SetParent(rootSegment.transform, false);
                fl.transform.localPosition = new Vector3(0f, 0.6f, 0.5f);
                fl.transform.localScale = Vector3.one * 0.45f;
                fl.GetComponent<Renderer>().sharedMaterial = (i % 2 == 0) ? pinkFlower : yellowFlower;
                RemoveCollider(fl);
            }

            // 2 VÒNG HÀO QUANG MA THUẬT XOAY QUANH TRỤ (DUAL MAGIC ENERGY RINGS)
            CreateMagicEnergyRings(pedestalRoot.transform, ringMat);
        }

        private static void CreateMagicEnergyRings(Transform parent, Material ringMat)
        {
            var ringGroup = new GameObject("Nexus_EnergyRings");
            ringGroup.transform.SetParent(parent, false);
            ringGroup.transform.localPosition = new Vector3(0f, 2.2f, 0f);

            // Vòng hào quang 1: Nghiêng góc 20 độ xoay thuận chiều
            var ring1 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ring1.name = "MagicRing_Inner";
            ring1.transform.SetParent(ringGroup.transform, false);
            ring1.transform.localRotation = Quaternion.Euler(22f, 0f, -15f);
            ring1.transform.localScale = new Vector3(2.6f, 0.035f, 2.6f);
            ring1.GetComponent<Renderer>().sharedMaterial = ringMat;
            RemoveCollider(ring1);
            var rot1 = ring1.AddComponent<Rotator>();
            rot1.rotationSpeed = new Vector3(0f, 35f, 0f);

            // Vòng hào quang 2: Nghiêng góc đối diện xoay ngược chiều
            var ring2 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ring2.name = "MagicRing_Outer";
            ring2.transform.SetParent(ringGroup.transform, false);
            ring2.transform.localRotation = Quaternion.Euler(-25f, 0f, 20f);
            ring2.transform.localScale = new Vector3(3.1f, 0.03f, 3.1f);
            ring2.GetComponent<Renderer>().sharedMaterial = ringMat;
            RemoveCollider(ring2);
            var rot2 = ring2.AddComponent<Rotator>();
            rot2.rotationSpeed = new Vector3(0f, -25f, 0f);
        }

        private static void CreateDistantMiniIslands(Transform env, Material stoneMat, Material mossMat, Material runeMat)
        {
            var distantGroup = new GameObject("Distant_MiniIslands");
            distantGroup.transform.SetParent(env, false);

            // Đảo nổi nhỏ 1: Phía sau bên phải
            CreateSingleMiniIsland(distantGroup.transform, "MiniIsland_Right", new Vector3(7.5f, 3.2f, 8.5f), 1.35f, stoneMat, mossMat, runeMat);
            // Đảo nổi nhỏ 2: Phía sau bên trái
            CreateSingleMiniIsland(distantGroup.transform, "MiniIsland_Left", new Vector3(-8.2f, 2.6f, 9.2f), 1.15f, stoneMat, mossMat, runeMat);
        }

        private static void CreateSingleMiniIsland(Transform parent, string name, Vector3 pos, float scale, Material stoneMat, Material mossMat, Material runeMat)
        {
            var island = new GameObject(name);
            island.transform.SetParent(parent, false);
            island.transform.localPosition = pos;
            island.transform.localScale = Vector3.one * scale;

            // Đất đá dưới
            var baseObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            baseObj.transform.SetParent(island.transform, false);
            baseObj.transform.localScale = new Vector3(1.8f, 0.8f, 1.8f);
            baseObj.GetComponent<Renderer>().sharedMaterial = stoneMat;
            RemoveCollider(baseObj);

            // Thảm rêu trên
            var mossObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            mossObj.transform.SetParent(island.transform, false);
            mossObj.transform.localPosition = new Vector3(0f, 0.42f, 0f);
            mossObj.transform.localScale = new Vector3(1.9f, 0.15f, 1.9f);
            mossObj.GetComponent<Renderer>().sharedMaterial = mossMat;
            RemoveCollider(mossObj);

            // Cột đá phế tích nhỏ
            var pillar = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pillar.transform.SetParent(island.transform, false);
            pillar.transform.localPosition = new Vector3(0f, 0.9f, 0f);
            pillar.transform.localScale = new Vector3(0.35f, 0.9f, 0.35f);
            pillar.GetComponent<Renderer>().sharedMaterial = runeMat;
            RemoveCollider(pillar);

            // Nhấp nhô bồng bềnh nhẹ
            var rot = island.AddComponent<Rotator>();
            rot.rotationSpeed = new Vector3(0f, 8f, 0f);
        }

        private static void UpdateCrystalCoreVisual()
        {
            var coreObj = GameObject.Find("CrystalCore");
            if (coreObj == null) return;

            // Đặt chân đế CrystalCore cắm trực tiếp trên bệ gỗ bát giác
            coreObj.transform.position = new Vector3(0f, 0.88f, 0f);

            // Điều chỉnh vị trí thanh máu bay lơ lửng trên đỉnh tinh thể (cao hơn và to rõ dễ nhìn hơn)
            var hpBar = coreObj.transform.Find("OverheadHealthBar");
            if (hpBar != null)
            {
                hpBar.localPosition = new Vector3(0f, 3.5f, 0f);
                hpBar.localScale = Vector3.one * 0.014f;
            }

            // Cập nhật Material pha lê phát sáng vàng mật ong + cyan rực rỡ
            var renderers = coreObj.GetComponentsInChildren<Renderer>();
            var crystalMat = CreateEmissionMaterial("Mat_NexusCrystalCoreDual", new Color(1f, 0.92f, 0.65f), new Color(1f, 0.75f, 0.35f), 2.2f);
            foreach (var r in renderers)
            {
                if (r != null && !r.name.Contains("Shield") && !r.name.Contains("Overhead"))
                {
                    r.sharedMaterial = crystalMat;
                }
            }

            // Đèn ánh sáng ấm áp từ khối pha lê
            var lights = coreObj.GetComponentsInChildren<Light>();
            foreach (var l in lights)
            {
                if (l != null)
                {
                    l.color = new Color(1f, 0.85f, 0.55f);
                    l.intensity = 2.8f;
                    l.range = 8.5f;
                }
            }
        }

        private static void SetupDreamyTwilightLighting()
        {
            // Background tím lam hoàng hôn kỳ ảo (Dreamy Cosmic Twilight)
            if (Camera.main != null)
            {
                Camera.main.clearFlags = CameraClearFlags.SolidColor;
                Camera.main.backgroundColor = new Color(0.12f, 0.08f, 0.22f, 1f); // Deep Cosmic Twilight Purple
                Camera.main.transform.position = new Vector3(0f, 5.2f, -6.8f);
                Camera.main.transform.rotation = Quaternion.Euler(30f, 0f, 0f);
            }

            // Directional Light: Ánh hoàng hôn ấm áp pha tím hồng
            var dirLight = GameObject.Find("Directional Light")?.GetComponent<Light>();
            if (dirLight != null)
            {
                dirLight.color = new Color(1f, 0.92f, 0.85f);
                dirLight.intensity = 1.35f;
                dirLight.transform.rotation = Quaternion.Euler(42f, -35f, 0f);
            }

            // Rim Light: Viền sáng xanh ngọc cyan ma thuật
            var rimLight = GameObject.Find("RimLight_Cyan")?.GetComponent<Light>();
            if (rimLight != null)
            {
                rimLight.color = new Color(0.35f, 0.85f, 1f);
                rimLight.intensity = 0.95f;
            }
        }

        private static void SetupCapybaraHeroCosmetics(Material crownMat, Material hammerMat)
        {
            var heroObj = GameObject.Find("Player_CapybaraHero");
            if (heroObj == null) return;

            // Đặt tag Player
            heroObj.tag = "Player";

            // Đặt Capybara đứng bảo vệ phía trước bên phải Trụ Tinh Thể (giống hình concept)
            heroObj.transform.position = new Vector3(1.65f, 0.2f, -1.0f);
            heroObj.transform.rotation = Quaternion.Euler(0f, -45f, 0f);

            // Xóa HeroCrown cũ nếu có để tránh trùng
            var oldCrown = heroObj.transform.Find("Head/HeroCrown");
            if (oldCrown != null) Object.DestroyImmediate(oldCrown.gameObject);

            // Vương miện vàng đội nghiêng đáng yêu trên đầu Capybara
            var existingCrown = heroObj.transform.Find("Cappy_GoldenCrown");
            if (existingCrown != null) Object.DestroyImmediate(existingCrown.gameObject);

            var crownRoot = new GameObject("Cappy_GoldenCrown");
            crownRoot.transform.SetParent(heroObj.transform, false);
            crownRoot.transform.localPosition = new Vector3(0.08f, 1.15f, 0.05f);
            crownRoot.transform.localRotation = Quaternion.Euler(12f, -10f, -18f); // nghiêng lệch tai như concept
            
            var crownBase = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            crownBase.transform.SetParent(crownRoot.transform, false);
            crownBase.transform.localScale = new Vector3(0.24f, 0.06f, 0.24f);
            crownBase.GetComponent<Renderer>().sharedMaterial = crownMat;
            RemoveCollider(crownBase);

            for (int i = 0; i < 5; i++)
            {
                float angle = i * (360f / 5f) * Mathf.Deg2Rad;
                var spike = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                spike.transform.SetParent(crownRoot.transform, false);
                spike.transform.localPosition = new Vector3(Mathf.Cos(angle) * 0.1f, 0.07f, Mathf.Sin(angle) * 0.1f);
                spike.transform.localScale = new Vector3(0.04f, 0.06f, 0.04f);
                spike.GetComponent<Renderer>().sharedMaterial = crownMat;
                RemoveCollider(spike);
            }

            // Búa Tinh Thể Ma Thuật (Mystic Hammer) trên tay Capybara
            var oldHammer = heroObj.transform.Find("Cappy_MysticHammer");
            if (oldHammer != null) Object.DestroyImmediate(oldHammer.gameObject);

            var hammerRoot = new GameObject("Cappy_MysticHammer");
            hammerRoot.transform.SetParent(heroObj.transform, false);
            hammerRoot.transform.localPosition = new Vector3(-0.45f, 0.45f, 0.35f);
            hammerRoot.transform.localRotation = Quaternion.Euler(15f, 40f, -30f);

            // Cán búa bằng gỗ sồi
            var handle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            handle.transform.SetParent(hammerRoot.transform, false);
            handle.transform.localScale = new Vector3(0.06f, 0.45f, 0.06f);
            var woodMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_NexusWoodPedestal.mat");
            if (woodMat != null) handle.GetComponent<Renderer>().sharedMaterial = woodMat;
            RemoveCollider(handle);

            // Đầu búa bằng pha lê hoa văn phát sáng xanh cyan
            var head = GameObject.CreatePrimitive(PrimitiveType.Cube);
            head.transform.SetParent(hammerRoot.transform, false);
            head.transform.localPosition = new Vector3(0f, 0.38f, 0f);
            head.transform.localScale = new Vector3(0.26f, 0.22f, 0.38f);
            head.GetComponent<Renderer>().sharedMaterial = hammerMat;
            RemoveCollider(head);
        }

        private static Material CreateLitMaterial(string name, Color color, float smoothness)
        {
            Shader s = Shader.Find("Universal Render Pipeline/Lit");
            if (s == null) s = Shader.Find("Standard");
            var mat = new Material(s) { name = name, color = color };
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
            if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", smoothness);
            return mat;
        }

        private static Material CreateEmissionMaterial(string name, Color baseColor, Color emissionColor, float intensity)
        {
            Shader s = Shader.Find("Universal Render Pipeline/Lit");
            if (s == null) s = Shader.Find("Standard");
            var mat = new Material(s) { name = name, color = baseColor };
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", baseColor);
            mat.EnableKeyword("_EMISSION");
            if (mat.HasProperty("_EmissionColor")) mat.SetColor("_EmissionColor", emissionColor * intensity);
            return mat;
        }

        private static void RemoveCollider(GameObject go)
        {
            var col = go.GetComponent<Collider>();
            if (col != null) Object.DestroyImmediate(col);
        }
    }
}
#endif
