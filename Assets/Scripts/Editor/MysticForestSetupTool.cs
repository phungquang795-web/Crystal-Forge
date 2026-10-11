#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using CrystalForge.Adventure;

namespace CrystalForge.Editor
{
    public static class MysticForestSetupTool
    {
        [MenuItem("CrystalForge/Transform to Mystic Forest Grove")]
        public static void TransformToMysticForest()
        {
            Debug.Log("<color=#00FF88><b>[CrystalForge]</b> Bắt đầu chuyển đổi bản đồ thành KHU RỪNG MA THUẬT (Mystic Forest Grove)...</color>");

            // 1. DỌN DẸP CÁC ĐỒ LÒ RÈN & VŨ TRỤ CŨ
            CleanupOldForgeProps();

            // 2. BIẾN ĐỔI MẶT ĐẤT THÀNH ĐẢO RỪNG RÊU PHONG
            TransformIslandGround();

            // 3. XÂY DỰNG CÁC CÂY CỔ THỤ THẦN THOẠI (ANCIENT MYSTIC TREES)
            BuildAncientForestTrees();

            // 4. XÂY DỰNG NẤM PHÁT SÁNG DẠ QUANG & HOA DẠI RỪNG
            BuildBioluminescentMushroomsAndFlora();

            // 5. TẠO BẦY ĐOM ĐÓM RỪNG THIÊNG (FOREST FIREFLIES)
            BuildForestFireflies();

            // 6. ĐIỀU CHỈNH ÁNH SÁNG & BẦU KHÍ QUYỂN RỪNG ĐÊM HUYỀN BÍ
            SetupForestAtmosphereAndLighting();

            // 7. CẬP NHẬT DANH XƯNG PLAYER: NGƯỜI BẢO VỆ TRỤ TINH THỂ
            UpdateGuardianAndCoreIdentity();

            // 8. CẬP NHẬT GIAO DIỆN UI (BANNER & UPGRADES)
            UpdateForestUI();

            // Lưu Scene
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();

            Debug.Log("<color=#00FF88><b>[CrystalForge]</b> Chuyển đổi thành công 100%! Bản đồ Khu Rừng Ma Thuật đã sẵn sàng!</color>");
        }

        private static void CleanupOldForgeProps()
        {
            var env = GameObject.Find("Environment_Diorama");
            if (env == null) return;

            string[] toDelete = new string[]
            {
                "CosmicStarfield_VFX",
                "FloatingAsteroids_Container",
                "Prop_CrystalAnvil",
                "Prop_BlacksmithWeaponRack",
                "Prop_CrystalOreCrate",
                "ForestFireflies_VFX",
                "MysticForest_TreesContainer",
                "MysticForest_FloraContainer"
            };

            foreach (var name in toDelete)
            {
                var t = env.transform.Find(name);
                if (t != null) Object.DestroyImmediate(t.gameObject);
            }
        }

        private static void TransformIslandGround()
        {
            var env = GameObject.Find("Environment_Diorama");
            if (env == null) return;

            // 1. Sàn đảo chính -> Thảm rêu rừng xanh mướt
            var floor = env.transform.Find("FloatingIsland_Floor");
            if (floor != null)
            {
                var r = floor.GetComponent<Renderer>();
                if (r != null)
                {
                    var mat = CreateForestMaterial("Mat_ForestMossGround", new Color(0.09f, 0.26f, 0.14f), 0.2f);
                    r.sharedMaterial = mat;
                }
            }

            // 2. Viền đảo -> Rêu xanh đậm & rễ cây
            var trim = env.transform.Find("Floor_GoldTrim");
            if (trim != null)
            {
                var r = trim.GetComponent<Renderer>();
                if (r != null)
                {
                    var mat = CreateForestMaterial("Mat_ForestMossTrim", new Color(0.18f, 0.42f, 0.22f), 0.35f);
                    r.sharedMaterial = mat;
                }
            }

            // 3. Vòng tròn cổ ngữ -> Vòng cổ ngữ thiên nhiên phát sáng xanh lục bảo
            var runicCircle = env.transform.Find("Floor_RunicCircle");
            if (runicCircle != null)
            {
                var r = runicCircle.GetComponent<Renderer>();
                if (r != null)
                {
                    var mat = CreateForestEmissionMaterial("Mat_ForestRunicCircle", new Color(0.05f, 0.3f, 0.15f), new Color(0.1f, 1f, 0.5f), 1.8f);
                    r.sharedMaterial = mat;
                }
                foreach (Transform child in runicCircle)
                {
                    var cr = child.GetComponent<Renderer>();
                    if (cr != null)
                    {
                        cr.sharedMaterial = CreateForestEmissionMaterial("Mat_ForestRuneNode", new Color(0.1f, 0.8f, 0.4f), new Color(0.2f, 1f, 0.6f), 2.5f);
                    }
                }
            }

            var innerCircle = env.transform.Find("Floor_InnerGlowCircle");
            if (innerCircle != null)
            {
                var r = innerCircle.GetComponent<Renderer>();
                if (r != null)
                {
                    r.sharedMaterial = CreateForestEmissionMaterial("Mat_ForestInnerRune", new Color(0.05f, 0.35f, 0.2f), new Color(0.1f, 0.9f, 0.5f), 2f);
                }
            }

            // 4. Rễ cây nâng đỡ dưới đáy đảo
            var underRoots = env.transform.Find("IslandUnderRoots");
            if (underRoots != null)
            {
                var woodMat = CreateForestMaterial("Mat_ForestRootsWood", new Color(0.22f, 0.14f, 0.08f), 0.15f);
                foreach (Transform root in underRoots)
                {
                    var r = root.GetComponent<Renderer>();
                    if (r != null) r.sharedMaterial = woodMat;
                }
            }

            // 5. Đài đá nâng Trụ Tinh Thể -> Đá rêu phong cổ đại rừng thiêng
            var pedestal = env.transform.Find("Central_Pedestal");
            if (pedestal != null)
            {
                var stoneMat = CreateForestMaterial("Mat_AncientMossyStone", new Color(0.12f, 0.20f, 0.16f), 0.25f);
                var trimMat = CreateForestEmissionMaterial("Mat_AncientMossyTrim", new Color(0.15f, 0.35f, 0.25f), new Color(0.2f, 0.95f, 0.6f), 1.6f);

                foreach (Transform p in pedestal)
                {
                    var r = p.GetComponent<Renderer>();
                    if (r != null)
                    {
                        if (p.name.Contains("GoldTrim") || p.name.Contains("Rune"))
                        {
                            r.sharedMaterial = trimMat;
                        }
                        else
                        {
                            r.sharedMaterial = stoneMat;
                        }
                    }
                }
            }
        }

        private static void BuildAncientForestTrees()
        {
            var env = GameObject.Find("Environment_Diorama");
            if (env == null) return;

            var treesContainer = new GameObject("MysticForest_TreesContainer");
            treesContainer.transform.SetParent(env.transform, false);

            var trunkMat = CreateForestMaterial("Mat_TreeTrunk", new Color(0.24f, 0.15f, 0.09f), 0.15f);
            var foliageMatA = CreateForestMaterial("Mat_TreeFoliageDeep", new Color(0.08f, 0.38f, 0.18f), 0.2f);
            var foliageMatB = CreateForestMaterial("Mat_TreeFoliageEmerald", new Color(0.12f, 0.52f, 0.25f), 0.25f);

            // Bố trí 5 cây cổ thụ tại các góc mép đảo (tạo chiều sâu rừng mà không che camera)
            CreateStylizedTree(treesContainer.transform, "Tree_BigLeftBack", new Vector3(-4.3f, 0f, 2.2f), 1.25f, trunkMat, foliageMatA, foliageMatB);
            CreateStylizedTree(treesContainer.transform, "Tree_BigRightBack", new Vector3(4.3f, 0f, 2.0f), 1.2f, trunkMat, foliageMatB, foliageMatA);
            CreateStylizedTree(treesContainer.transform, "Tree_CenterBack", new Vector3(0f, 0f, 4.6f), 1.45f, trunkMat, foliageMatA, foliageMatB);
            CreateStylizedTree(treesContainer.transform, "Tree_SmallLeftFront", new Vector3(-4.0f, 0f, -2.5f), 0.9f, trunkMat, foliageMatB, foliageMatA);
            CreateStylizedTree(treesContainer.transform, "Tree_SmallRightFront", new Vector3(4.0f, 0f, -2.4f), 0.95f, trunkMat, foliageMatA, foliageMatB);
        }

        private static void CreateStylizedTree(Transform parent, string name, Vector3 pos, float scale, Material trunkMat, Material folA, Material folB)
        {
            var tree = new GameObject(name);
            tree.transform.SetParent(parent, false);
            tree.transform.localPosition = pos;
            tree.transform.localScale = Vector3.one * scale;

            // Thân cây
            var trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            trunk.name = "Trunk";
            trunk.transform.SetParent(tree.transform, false);
            trunk.transform.localPosition = new Vector3(0f, 1.2f, 0f);
            trunk.transform.localScale = new Vector3(0.35f, 1.2f, 0.35f);
            var colT = trunk.GetComponent<Collider>();
            if (colT != null) Object.DestroyImmediate(colT);
            trunk.GetComponent<Renderer>().sharedMaterial = trunkMat;

            // Tán lá tầng 1 (Dưới)
            var f1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            f1.name = "Foliage_Layer1";
            f1.transform.SetParent(tree.transform, false);
            f1.transform.localPosition = new Vector3(0f, 2.1f, 0f);
            f1.transform.localScale = new Vector3(1.7f, 1.1f, 1.7f);
            var colF1 = f1.GetComponent<Collider>();
            if (colF1 != null) Object.DestroyImmediate(colF1);
            f1.GetComponent<Renderer>().sharedMaterial = folA;

            // Tán lá tầng 2 (Giữa)
            var f2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            f2.name = "Foliage_Layer2";
            f2.transform.SetParent(tree.transform, false);
            f2.transform.localPosition = new Vector3(0f, 2.7f, 0f);
            f2.transform.localScale = new Vector3(1.35f, 0.95f, 1.35f);
            var colF2 = f2.GetComponent<Collider>();
            if (colF2 != null) Object.DestroyImmediate(colF2);
            f2.GetComponent<Renderer>().sharedMaterial = folB;

            // Tán lá tầng 3 (Đỉnh)
            var f3 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            f3.name = "Foliage_Layer3";
            f3.transform.SetParent(tree.transform, false);
            f3.transform.localPosition = new Vector3(0f, 3.2f, 0f);
            f3.transform.localScale = new Vector3(0.95f, 0.8f, 0.95f);
            var colF3 = f3.GetComponent<Collider>();
            if (colF3 != null) Object.DestroyImmediate(colF3);
            f3.GetComponent<Renderer>().sharedMaterial = folA;
        }

        private static void BuildBioluminescentMushroomsAndFlora()
        {
            var env = GameObject.Find("Environment_Diorama");
            if (env == null) return;

            var floraContainer = new GameObject("MysticForest_FloraContainer");
            floraContainer.transform.SetParent(env.transform, false);

            var mushroomCapCyan = CreateForestEmissionMaterial("Mat_MushroomCapCyan", new Color(0.05f, 0.35f, 0.45f), new Color(0.1f, 0.95f, 1f), 2.5f);
            var mushroomCapPink = CreateForestEmissionMaterial("Mat_MushroomCapPink", new Color(0.35f, 0.08f, 0.35f), new Color(1f, 0.35f, 0.85f), 2.2f);
            var mushroomStem = CreateForestMaterial("Mat_MushroomStem", new Color(0.92f, 0.90f, 0.85f), 0.1f);
            var mossStoneMat = CreateForestMaterial("Mat_MossyBoulder", new Color(0.16f, 0.28f, 0.20f), 0.2f);

            // Cụm nấm dạ quang quanh gốc cây trái
            CreateMushroomCluster(floraContainer.transform, "Mushrooms_Left", new Vector3(-3.2f, 0.05f, 1.4f), mushroomStem, mushroomCapCyan, new Color(0.1f, 0.9f, 1f));
            // Cụm nấm dạ quang quanh gốc cây phải
            CreateMushroomCluster(floraContainer.transform, "Mushrooms_Right", new Vector3(3.2f, 0.05f, 1.2f), mushroomStem, mushroomCapPink, new Color(1f, 0.3f, 0.8f));
            // Cụm nấm gần Trụ Tinh Thể
            CreateMushroomCluster(floraContainer.transform, "Mushrooms_NearCore", new Vector3(-1.4f, 0.05f, -1.2f), mushroomStem, mushroomCapCyan, new Color(0.2f, 1f, 0.8f));

            // Tảng đá phủ rêu phong (Mossy Boulders)
            CreateMossyBoulder(floraContainer.transform, "Boulder_1", new Vector3(-3.4f, 0.25f, -0.6f), new Vector3(0.7f, 0.5f, 0.65f), mossStoneMat);
            CreateMossyBoulder(floraContainer.transform, "Boulder_2", new Vector3(3.4f, 0.25f, -0.5f), new Vector3(0.65f, 0.45f, 0.7f), mossStoneMat);
            CreateMossyBoulder(floraContainer.transform, "Boulder_3", new Vector3(1.6f, 0.2f, 2.6f), new Vector3(0.6f, 0.4f, 0.55f), mossStoneMat);
        }

        private static void CreateMushroomCluster(Transform parent, string name, Vector3 pos, Material stemMat, Material capMat, Color glowColor)
        {
            var cluster = new GameObject(name);
            cluster.transform.SetParent(parent, false);
            cluster.transform.localPosition = pos;

            // 3 cây nấm với kích thước và góc nghiêng tự nhiên
            CreateSingleMushroom(cluster.transform, "Mushroom_A", Vector3.zero, 1.0f, stemMat, capMat);
            CreateSingleMushroom(cluster.transform, "Mushroom_B", new Vector3(0.25f, 0f, 0.15f), 0.75f, stemMat, capMat);
            CreateSingleMushroom(cluster.transform, "Mushroom_C", new Vector3(-0.2f, 0f, 0.18f), 0.6f, stemMat, capMat);

            // Đèn hắt dạ quang nhẹ từ cụm nấm
            var lightObj = new GameObject("GlowLight");
            lightObj.transform.SetParent(cluster.transform, false);
            lightObj.transform.localPosition = new Vector3(0f, 0.35f, 0f);
            var pLight = lightObj.AddComponent<Light>();
            pLight.type = LightType.Point;
            pLight.color = glowColor;
            pLight.intensity = 1.1f;
            pLight.range = 1.8f;
        }

        private static void CreateSingleMushroom(Transform parent, string name, Vector3 localPos, float scale, Material stemMat, Material capMat)
        {
            var shroom = new GameObject(name);
            shroom.transform.SetParent(parent, false);
            shroom.transform.localPosition = localPos;
            shroom.transform.localScale = Vector3.one * scale;

            // Thân nấm
            var stem = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            stem.name = "Stem";
            stem.transform.SetParent(shroom.transform, false);
            stem.transform.localPosition = new Vector3(0f, 0.18f, 0f);
            stem.transform.localScale = new Vector3(0.08f, 0.18f, 0.08f);
            var colS = stem.GetComponent<Collider>();
            if (colS != null) Object.DestroyImmediate(colS);
            stem.GetComponent<Renderer>().sharedMaterial = stemMat;

            // Mũ nấm
            var cap = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            cap.name = "Cap";
            cap.transform.SetParent(shroom.transform, false);
            cap.transform.localPosition = new Vector3(0f, 0.34f, 0f);
            cap.transform.localScale = new Vector3(0.32f, 0.18f, 0.32f);
            var colC = cap.GetComponent<Collider>();
            if (colC != null) Object.DestroyImmediate(colC);
            cap.GetComponent<Renderer>().sharedMaterial = capMat;
        }

        private static void CreateMossyBoulder(Transform parent, string name, Vector3 pos, Vector3 scale, Material mat)
        {
            var boulder = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            boulder.name = name;
            boulder.transform.SetParent(parent, false);
            boulder.transform.localPosition = pos;
            boulder.transform.localScale = scale;
            boulder.transform.localRotation = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), 0f);
            var col = boulder.GetComponent<Collider>();
            if (col != null) Object.DestroyImmediate(col);
            boulder.GetComponent<Renderer>().sharedMaterial = mat;
        }

        private static void BuildForestFireflies()
        {
            var env = GameObject.Find("Environment_Diorama");
            if (env == null) return;

            var firefliesObj = new GameObject("ForestFireflies_VFX");
            firefliesObj.transform.SetParent(env.transform, false);
            firefliesObj.transform.localPosition = new Vector3(0f, 1.6f, 0f);

            var ps = firefliesObj.AddComponent<ParticleSystem>();
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            var main = ps.main;
            main.playOnAwake = true;
            main.loop = true;
            main.duration = 6.0f;
            main.startLifetime = new ParticleSystem.MinMaxCurve(3.5f, 6.0f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(0.08f, 0.22f);
            main.startSize = new ParticleSystem.MinMaxCurve(0.04f, 0.09f);
            // Đom đóm vàng chanh & xanh lục dạ quang huyền ảo
            main.startColor = new ParticleSystem.MinMaxGradient(new Color(0.95f, 1f, 0.3f, 0.85f), new Color(0.3f, 1f, 0.6f, 0.85f));
            main.gravityModifier = -0.015f; // Bay bồng bềnh lơ lửng
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.enabled = true;
            emission.rateOverTime = 14f;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.scale = new Vector3(8.0f, 3.2f, 7.0f);

            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient grad = new Gradient();
            grad.SetKeys(
                new GradientColorKey[] { new GradientColorKey(new Color(0.95f, 1f, 0.3f), 0f), new GradientColorKey(new Color(0.2f, 1f, 0.7f), 1f) },
                new GradientAlphaKey[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(0.9f, 0.3f), new GradientAlphaKey(0.9f, 0.7f), new GradientAlphaKey(0f, 1f) }
            );
            colorOverLifetime.color = grad;

            var renderer = ps.GetComponent<ParticleSystemRenderer>();
            renderer.renderMode = ParticleSystemRenderMode.Billboard;

            Shader shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
            if (shader == null) shader = Shader.Find("Sprites/Default");
            if (shader != null)
            {
                renderer.sharedMaterial = new Material(shader) { name = "Mat_FireflyParticle" };
            }

            ps.Play();
        }

        private static void SetupForestAtmosphereAndLighting()
        {
            // 1. Camera Background: Xanh đêm rừng sâu huyền bí
            if (Camera.main != null)
            {
                Camera.main.clearFlags = CameraClearFlags.SolidColor;
                Camera.main.backgroundColor = new Color(0.04f, 0.11f, 0.08f, 1.0f); // Deep Emerald Forest Night
            }

            // 2. Directional Light: Ánh trăng rừng xanh ngọc pha chút nắng ấm
            var dirLightObj = GameObject.Find("Directional Light");
            if (dirLightObj != null)
            {
                var light = dirLightObj.GetComponent<Light>();
                if (light != null)
                {
                    light.color = new Color(0.85f, 0.98f, 0.90f); // Soft Moonlight Green-White
                    light.intensity = 1.15f;
                }
            }

            // 3. Rim Light: Ánh sáng phản chiếu viền xanh ngọc bích
            var rimLightObj = GameObject.Find("RimLight_Cyan");
            if (rimLightObj != null)
            {
                var light = rimLightObj.GetComponent<Light>();
                if (light != null)
                {
                    light.color = new Color(0.15f, 0.95f, 0.55f); // Emerald Rim
                    light.intensity = 0.75f;
                }
            }
        }

        private static void UpdateGuardianAndCoreIdentity()
        {
            // 1. Cập nhật nhãn thanh HP của Player Capybara -> [ 🛡️ NGƯỜI BẢO VỆ ]
            var heroObj = GameObject.Find("Player_CapybaraHero");
            if (heroObj != null)
            {
                var heroBar = heroObj.GetComponentInChildren<OverheadHealthBar3D>(true);
                if (heroBar != null)
                {
                    heroBar.BarOwner = HealthBarOwner.Player;
                    heroBar.SetupBar("NGƯỜI BẢO VỆ", 100, 100, new Color(0.1f, 0.95f, 0.45f));
                }
            }

            // 2. Cập nhật nhãn thanh HP của Trụ Tinh Thể -> [ 💎 TRỤ TINH THỂ RỪNG THIÊNG ]
            var coreObj = GameObject.Find("CrystalCore");
            if (coreObj != null)
            {
                var coreBar = coreObj.GetComponentInChildren<OverheadHealthBar3D>(true);
                if (coreBar != null)
                {
                    coreBar.BarOwner = HealthBarOwner.CrystalCore;
                    coreBar.transform.localPosition = new Vector3(0f, 3.5f, 0f);
                    coreBar.transform.localScale = Vector3.one * 0.014f;
                    coreBar.SetupBar("TRỤ TINH THỂ RỪNG THIÊNG", 100, 100, new Color(0.1f, 0.9f, 1f));
                }
            }
        }

        private static void UpdateForestUI()
        {
            // 1. TopBar Title -> RỪNG TINH THỂ (CRYSTAL GROVE)
            var topBar = GameObject.Find("TopBar_Panel");
            if (topBar != null)
            {
                var titleText = topBar.transform.Find("TitleText")?.GetComponent<TMP_Text>();
                if (titleText != null)
                {
                    titleText.text = "RỪNG TINH THỂ";
                    titleText.color = new Color(0.4f, 1f, 0.65f); // Xanh ngọc
                }
            }

            // 2. BottomBar Banner -> [ 🛡️ BẢO VỆ RỪNG THIÊNG: ĐỢT #1 ]
            var banner = GameObject.Find("StartAdventureBtn");
            if (banner != null)
            {
                var txt = banner.GetComponentInChildren<TMP_Text>(true);
                if (txt != null)
                {
                    txt.text = "[ 🛡️ BẢO VỆ RỪNG THIÊNG: ĐỢT #1 ]";
                }
            }

            // 3. Tab Button & Upgrade Header
            var tabForge = GameObject.Find("TabButton_Forge");
            if (tabForge != null)
            {
                var label = tabForge.GetComponentInChildren<TMP_Text>(true);
                if (label != null) label.text = "[*] VE BINH";
            }

            var upgradeHeader = GameObject.Find("RightPanel_Upgrades/Header")?.GetComponent<TMP_Text>();
            if (upgradeHeader != null)
            {
                upgradeHeader.text = "[+] NANG CAP VE BINH";
                upgradeHeader.color = new Color(0.4f, 1f, 0.65f);
            }
        }

        private static Material CreateForestMaterial(string matName, Color color, float smoothness)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");

            var mat = new Material(shader)
            {
                name = matName,
                color = color
            };
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
            if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", smoothness);
            return mat;
        }

        private static Material CreateForestEmissionMaterial(string matName, Color baseColor, Color emissionColor, float intensity)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");

            var mat = new Material(shader)
            {
                name = matName,
                color = baseColor
            };
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", baseColor);
            mat.EnableKeyword("_EMISSION");
            if (mat.HasProperty("_EmissionColor")) mat.SetColor("_EmissionColor", emissionColor * intensity);
            return mat;
        }
    }
}
#endif
