#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CrystalForge.Editor
{
    public static class UISpriteGenerator
    {
        private const string FOLDER = "Assets/Textures/UI";

        [MenuItem("CrystalForge/Generate High-Res UI Sprites")]
        public static void GenerateAllSprites()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Textures"))
            {
                AssetDatabase.CreateFolder("Assets", "Textures");
            }
            if (!AssetDatabase.IsValidFolder(FOLDER))
            {
                AssetDatabase.CreateFolder("Assets/Textures", "UI");
            }

            // 1. Các khung hình nền & Button (9-slice ready)
            SaveTexture("rounded_box.png", CreateRoundedBox(128, 128, 20), new Vector4(24, 24, 24, 24));
            SaveTexture("pill_frame.png", CreatePillFrame(256, 96, 44), new Vector4(46, 46, 46, 46));

            // 2. Bộ Icon Game Nghệ Thuật 128x128
            SaveTexture("icon_crystal.png", CreateCrystalIcon(128));
            SaveTexture("icon_hammer.png", CreateHammerIcon(128));
            SaveTexture("icon_drill.png", CreateDrillIcon(128));
            SaveTexture("icon_furnace.png", CreateFurnaceIcon(128));
            SaveTexture("icon_extractor.png", CreateExtractorIcon(128));
            SaveTexture("icon_heart.png", CreateHeartIcon(128));
            SaveTexture("icon_sword.png", CreateSwordIcon(128));
            SaveTexture("icon_speed.png", CreateSpeedIcon(128));
            SaveTexture("icon_skill.png", CreateSkillIcon(128));
            SaveTexture("icon_crown.png", CreateCrownIcon(128));
            SaveTexture("icon_trophy.png", CreateTrophyIcon(128));
            SaveTexture("icon_skull.png", CreateSkullIcon(128));
            SaveTexture("icon_star.png", CreateStarIcon(128));
            SaveTexture("icon_sound.png", CreateSoundIcon(128));
            SaveTexture("icon_save.png", CreateSaveIcon(128));

            AssetDatabase.Refresh();
            Debug.Log("<color=#00FF88><b>[UISpriteGenerator]</b> Đã sinh thành công 17 UI Sprites độ nét cao tại Assets/Textures/UI!</color>");
        }

        private static void SaveTexture(string fileName, Texture2D tex, Vector4 border = default)
        {
            string path = Path.Combine(FOLDER, fileName);
            byte[] bytes = tex.EncodeToPNG();
            File.WriteAllBytes(path, bytes);
            Object.DestroyImmediate(tex);

            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.alphaIsTransparency = true;
                importer.filterMode = FilterMode.Bilinear;
                if (border != default)
                {
                    importer.spriteBorder = border;
                }
                importer.SaveAndReimport();
            }
        }

        // --- CÁC HÀM VẼ PROCEDURAL TEXTURE ---

        private static Texture2D CreateRoundedBox(int w, int h, int r)
        {
            var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            Color solid = Color.white;
            Color clear = new Color(1, 1, 1, 0);

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    // Khoảng cách tới góc gần nhất
                    int cx = x < r ? r : (x > w - 1 - r ? w - 1 - r : x);
                    int cy = y < r ? r : (y > h - 1 - r ? h - 1 - r : y);
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cy));

                    if (dist <= r - 1.5f)
                    {
                        tex.SetPixel(x, y, solid);
                    }
                    else if (dist <= r)
                    {
                        float alpha = Mathf.Clamp01(1f - (dist - (r - 1.5f)) / 1.5f);
                        tex.SetPixel(x, y, new Color(1, 1, 1, alpha));
                    }
                    else
                    {
                        tex.SetPixel(x, y, clear);
                    }
                }
            }
            tex.Apply();
            return tex;
        }

        private static Texture2D CreatePillFrame(int w, int h, int r)
        {
            var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            Color solid = Color.white;
            Color clear = new Color(1, 1, 1, 0);

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int cx = x < r ? r : (x > w - 1 - r ? w - 1 - r : x);
                    int cy = h / 2;
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cy));

                    if (dist <= r - 1.5f)
                    {
                        tex.SetPixel(x, y, solid);
                    }
                    else if (dist <= r)
                    {
                        float alpha = Mathf.Clamp01(1f - (dist - (r - 1.5f)) / 1.5f);
                        tex.SetPixel(x, y, new Color(1, 1, 1, alpha));
                    }
                    else
                    {
                        tex.SetPixel(x, y, clear);
                    }
                }
            }
            tex.Apply();
            return tex;
        }

        private static Texture2D CreateCrystalIcon(int size)
        {
            var tex = CreateEmpty(size);
            Vector2 c = new Vector2(size * 0.5f, size * 0.5f);
            float h = size * 0.44f;
            float w = size * 0.32f;

            // Hình thoi octahedron rực sáng
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = Mathf.Abs(x - c.x) / w;
                    float dy = Mathf.Abs(y - c.y) / h;
                    float d = dx + dy;

                    if (d <= 1f)
                    {
                        // Mặt trái sáng hơn mặt phải tạo khối 3D kim cương
                        float facet = x < c.x ? 1.0f : 0.75f;
                        float vertGradient = (float)y / size * 0.3f + 0.7f;
                        Color col = new Color(0.2f * facet, 0.85f * facet * vertGradient, 1.0f * facet, 1f);

                        // Lõi sáng trắng ở giữa
                        if (d < 0.35f)
                        {
                            col = Color.Lerp(col, Color.white, 0.65f);
                        }
                        tex.SetPixel(x, y, col);
                    }
                    else if (d <= 1.08f)
                    {
                        // Viền sáng cyan mờ ngoài
                        tex.SetPixel(x, y, new Color(0.4f, 0.95f, 1f, 0.8f));
                    }
                }
            }
            tex.Apply();
            return tex;
        }

        private static Texture2D CreateHammerIcon(int size)
        {
            var tex = CreateEmpty(size);
            Vector2 c = new Vector2(size * 0.5f, size * 0.5f);

            // Cán búa chéo 45 độ
            for (int i = -35; i <= 30; i++)
            {
                for (int t = -4; t <= 4; t++)
                {
                    int px = Mathf.RoundToInt(c.x + i + t);
                    int py = Mathf.RoundToInt(c.y + i - t);
                    if (Inside(px, py, size)) tex.SetPixel(px, py, new Color(0.7f, 0.45f, 0.2f)); // Màu gỗ
                }
            }

            // Đầu búa kim loại lớn
            Vector2 headCenter = c + new Vector2(16, 16);
            for (int y = -14; y <= 14; y++)
            {
                for (int x = -24; x <= 24; x++)
                {
                    // Xoay 45 độ
                    float rx = (x - y) * 0.707f;
                    float ry = (x + y) * 0.707f;
                    int px = Mathf.RoundToInt(headCenter.x + rx);
                    int py = Mathf.RoundToInt(headCenter.y + ry);

                    if (Inside(px, py, size) && Mathf.Abs(x) <= 20 && Mathf.Abs(y) <= 12)
                    {
                        float shade = (x > 0 ? 0.95f : 0.75f);
                        tex.SetPixel(px, py, new Color(0.35f * shade, 0.85f * shade, 1f * shade));
                    }
                }
            }

            tex.Apply();
            return tex;
        }

        private static Texture2D CreateDrillIcon(int size)
        {
            var tex = CreateEmpty(size);
            Vector2 c = new Vector2(size * 0.5f, size * 0.5f);

            // Thân hình nón tam giác ngược (Mũi khoan xoắn)
            for (int y = 15; y < size - 20; y++)
            {
                float progress = (float)(y - 15) / (size - 35);
                int halfW = Mathf.RoundToInt(progress * (size * 0.36f));

                for (int x = -halfW; x <= halfW; x++)
                {
                    int px = Mathf.RoundToInt(c.x + x);
                    int py = y;
                    if (Inside(px, py, size))
                    {
                        // Rãnh xoắn ốc
                        bool spiral = ((y / 8 + x / 6) % 2 == 0);
                        Color col = spiral ? new Color(1f, 0.85f, 0.2f) : new Color(0.85f, 0.65f, 0.1f);
                        tex.SetPixel(px, py, col);
                    }
                }
            }
            tex.Apply();
            return tex;
        }

        private static Texture2D CreateFurnaceIcon(int size)
        {
            var tex = CreateEmpty(size);
            Vector2 c = new Vector2(size * 0.5f, size * 0.35f);

            // Ngọn lửa ma pháp hình giọt nước đôi
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = (x - size * 0.5f) / (size * 0.35f);
                    float dy = (y - size * 0.2f) / (size * 0.7f);

                    if (dy >= 0 && dy <= 1f)
                    {
                        float flameWidth = (1f - dy) * Mathf.Sqrt(Mathf.Clamp01(dy)) * 2.2f;
                        if (Mathf.Abs(dx) <= flameWidth)
                        {
                            // Gradient từ vàng lõi ra cam đỏ ở rìa
                            float d = Mathf.Abs(dx) / (flameWidth + 0.001f);
                            Color col;
                            if (d < 0.35f) col = new Color(1f, 0.95f, 0.3f); // Vàng sáng
                            else if (d < 0.7f) col = new Color(1f, 0.55f, 0.1f); // Cam
                            else col = new Color(0.95f, 0.2f, 0.1f); // Đỏ rực
                            tex.SetPixel(x, y, col);
                        }
                    }
                }
            }
            tex.Apply();
            return tex;
        }

        private static Texture2D CreateExtractorIcon(int size)
        {
            var tex = CreateEmpty(size);
            Vector2 c = new Vector2(size * 0.5f, size * 0.5f);

            // Vòng xoáy 3 cánh Vortex
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Vector2 v = new Vector2(x - c.x, y - c.y);
                    float r = v.magnitude / (size * 0.44f);
                    if (r <= 1f && r >= 0.15f)
                    {
                        float angle = Mathf.Atan2(v.y, v.x);
                        float spiral = Mathf.Sin(angle * 3f + r * 6f);
                        if (spiral > 0.2f)
                        {
                            Color col = Color.Lerp(new Color(0.2f, 0.9f, 0.6f), new Color(0.3f, 0.6f, 1f), r);
                            tex.SetPixel(x, y, col);
                        }
                    }
                }
            }
            tex.Apply();
            return tex;
        }

        private static Texture2D CreateHeartIcon(int size)
        {
            var tex = CreateEmpty(size);
            Vector2 c = new Vector2(size * 0.5f, size * 0.45f);
            float scale = size * 0.026f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float nx = (x - c.x) / scale;
                    float ny = (y - c.y) / scale;

                    // Phương trình toán học hình trái tim: (x^2 + y^2 - 1)^3 - x^2 * y^3 <= 0
                    float eq = Mathf.Pow(nx * nx * 0.08f + ny * ny * 0.08f - 1f, 3f) - (nx * nx * 0.08f) * Mathf.Pow(ny * 0.28f, 3f);
                    if (eq <= 0f)
                    {
                        // 3D bóng bẩy đỏ ruby
                        float highlight = (nx < -2f && ny > 1f) ? 0.35f : 0f;
                        Color col = new Color(0.95f + highlight, 0.2f + highlight, 0.3f + highlight);
                        tex.SetPixel(x, y, col);
                    }
                }
            }
            tex.Apply();
            return tex;
        }

        private static Texture2D CreateSwordIcon(int size)
        {
            var tex = CreateEmpty(size);
            Vector2 c = new Vector2(size * 0.5f, size * 0.5f);

            // Lưỡi kiếm chỉ thẳng lên trên
            for (int y = 25; y < size - 15; y++)
            {
                int w = y > size - 30 ? Mathf.RoundToInt((size - 15 - y) * 0.6f) : 7;
                for (int x = -w; x <= w; x++)
                {
                    int px = Mathf.RoundToInt(c.x + x);
                    float shade = x < 0 ? 0.95f : 0.75f;
                    if (Inside(px, y, size)) tex.SetPixel(px, y, new Color(0.85f * shade, 0.9f * shade, 1f * shade));
                }
            }

            // Thanh chắn chuôi kiếm (Crossguard)
            for (int y = 24; y <= 30; y++)
            {
                for (int x = -24; x <= 24; x++)
                {
                    int px = Mathf.RoundToInt(c.x + x);
                    if (Inside(px, y, size)) tex.SetPixel(px, y, new Color(1f, 0.8f, 0.2f));
                }
            }

            // Cán kiếm (Hilt)
            for (int y = 10; y < 24; y++)
            {
                for (int x = -4; x <= 4; x++)
                {
                    int px = Mathf.RoundToInt(c.x + x);
                    if (Inside(px, y, size)) tex.SetPixel(px, y, new Color(0.6f, 0.35f, 0.15f));
                }
            }

            tex.Apply();
            return tex;
        }

        private static Texture2D CreateSpeedIcon(int size)
        {
            var tex = CreateEmpty(size);
            Vector2 c = new Vector2(size * 0.5f, size * 0.5f);

            // Tia sét tốc độ chớp nhoáng (Lightning Bolt)
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float py = (float)y / size;
                    float px = (float)x / size;

                    bool inBolt = false;
                    if (py >= 0.5f && py <= 0.88f)
                    {
                        float midX = 0.62f - (py - 0.5f) * 0.45f;
                        if (Mathf.Abs(px - midX) <= 0.12f) inBolt = true;
                    }
                    else if (py >= 0.12f && py < 0.52f)
                    {
                        float midX = 0.44f - (py - 0.12f) * 0.45f;
                        if (Mathf.Abs(px - midX) <= 0.12f) inBolt = true;
                    }

                    if (inBolt)
                    {
                        tex.SetPixel(x, y, new Color(0.35f, 0.9f, 1f));
                    }
                }
            }
            tex.Apply();
            return tex;
        }

        private static Texture2D CreateSkillIcon(int size)
        {
            var tex = CreateEmpty(size);
            Vector2 c = new Vector2(size * 0.5f, size * 0.5f);
            float r = size * 0.38f;

            // Quả cầu ma pháp cổ ngữ tím phát sáng
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), c);
                    if (dist <= r)
                    {
                        float norm = dist / r;
                        Color col = Color.Lerp(Color.white, new Color(0.7f, 0.25f, 1f), norm);
                        tex.SetPixel(x, y, col);
                    }
                    else if (dist <= r + 4f)
                    {
                        tex.SetPixel(x, y, new Color(0.85f, 0.45f, 1f, 0.5f));
                    }
                }
            }
            tex.Apply();
            return tex;
        }

        private static Texture2D CreateCrownIcon(int size)
        {
            var tex = CreateEmpty(size);
            Vector2 c = new Vector2(size * 0.5f, size * 0.4f);

            // Vương miện hoàng gia vàng 3 đỉnh
            for (int y = 20; y < size - 25; y++)
            {
                for (int x = 18; x < size - 18; x++)
                {
                    float nx = (float)(x - 18) / (size - 36);
                    float ny = (float)(y - 20) / (size - 45);

                    // 3 chóp nhọn
                    float peaks = Mathf.Max(
                        1f - Mathf.Abs(nx - 0.1f) * 4f,
                        Mathf.Max(1f - Mathf.Abs(nx - 0.5f) * 3f, 1f - Mathf.Abs(nx - 0.9f) * 4f)
                    );

                    if (ny <= 0.35f || ny <= peaks)
                    {
                        Color col = new Color(1f, 0.82f, 0.2f);
                        if (ny < 0.3f) col = new Color(0.95f, 0.7f, 0.15f);
                        tex.SetPixel(x, y, col);
                    }
                }
            }
            tex.Apply();
            return tex;
        }

        private static Texture2D CreateTrophyIcon(int size)
        {
            var tex = CreateEmpty(size);
            Vector2 c = new Vector2(size * 0.5f, size * 0.55f);

            // Thân cúp vàng + quai
            for (int y = 15; y < size - 20; y++)
            {
                for (int x = 20; x < size - 20; x++)
                {
                    float nx = Mathf.Abs(x - c.x) / (size * 0.32f);
                    float ny = (y - 15f) / (size - 35f);

                    bool inCup = (ny >= 0.4f && nx <= Mathf.Sqrt(ny));
                    bool inStem = (ny >= 0.18f && ny < 0.4f && nx <= 0.16f);
                    bool inBase = (ny < 0.18f && nx <= 0.55f);

                    if (inCup || inStem || inBase)
                    {
                        tex.SetPixel(x, y, new Color(1f, 0.85f, 0.25f));
                    }
                }
            }
            tex.Apply();
            return tex;
        }

        private static Texture2D CreateSkullIcon(int size)
        {
            var tex = CreateEmpty(size);
            Vector2 c = new Vector2(size * 0.5f, size * 0.52f);
            float r = size * 0.35f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), c);
                    bool inHead = dist <= r;
                    bool inJaw = (Mathf.Abs(x - c.x) <= r * 0.55f && y >= c.y - r * 1.15f && y <= c.y);

                    if (inHead || inJaw)
                    {
                        // Hốc mắt
                        float eyeLeft = Vector2.Distance(new Vector2(x, y), c + new Vector2(-12, 4));
                        float eyeRight = Vector2.Distance(new Vector2(x, y), c + new Vector2(12, 4));
                        if (eyeLeft > 9f && eyeRight > 9f)
                        {
                            tex.SetPixel(x, y, new Color(0.95f, 0.35f, 0.35f)); // Đỏ cảnh báo
                        }
                    }
                }
            }
            tex.Apply();
            return tex;
        }

        private static Texture2D CreateStarIcon(int size)
        {
            var tex = CreateEmpty(size);
            Vector2 c = new Vector2(size * 0.5f, size * 0.5f);
            float rOuter = size * 0.44f;
            float rInner = size * 0.18f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Vector2 v = new Vector2(x - c.x, y - c.y);
                    float dist = v.magnitude;
                    float angle = Mathf.Atan2(v.y, v.x) + Mathf.PI * 0.5f;
                    float modAngle = Mathf.Repeat(angle, Mathf.PI * 2f / 5f) - Mathf.PI / 5f;
                    float maxR = rInner / Mathf.Cos(modAngle);

                    if (dist <= rOuter)
                    {
                        // 5 cánh sao
                        float wave = Mathf.Cos(angle * 5f);
                        float limit = Mathf.Lerp(rInner, rOuter, Mathf.Clamp01(wave));
                        if (dist <= limit)
                        {
                            tex.SetPixel(x, y, new Color(1f, 0.88f, 0.25f));
                        }
                    }
                }
            }
            tex.Apply();
            return tex;
        }

        private static Texture2D CreateSoundIcon(int size)
        {
            var tex = CreateEmpty(size);
            Vector2 c = new Vector2(size * 0.44f, size * 0.5f);
            Color cyan = new Color(0.35f, 0.85f, 1.0f);
            Color glow = new Color(0.20f, 0.55f, 0.90f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    // Thân loa hình chữ nhật
                    bool inBox = (x >= c.x - 28 && x <= c.x - 8 && y >= c.y - 14 && y <= c.y + 14);

                    // Phễu loa hình tam giác mở rộng
                    float progress = Mathf.Clamp01((x - (c.x - 8)) / 24f);
                    float halfH = Mathf.Lerp(14f, 32f, progress);
                    bool inHorn = (x >= c.x - 8 && x <= c.x + 16 && Mathf.Abs(y - c.y) <= halfH);

                    if (inBox || inHorn)
                    {
                        tex.SetPixel(x, y, (inBox && x < c.x - 24) ? glow : cyan);
                    }

                    // Sóng âm 1
                    float dist1 = Vector2.Distance(new Vector2(x, y), new Vector2(c.x + 8, c.y));
                    if (dist1 >= 24f && dist1 <= 29f && x > c.x + 18 && Mathf.Abs(y - c.y) <= (x - (c.x + 12)) * 1.1f)
                    {
                        tex.SetPixel(x, y, cyan);
                    }

                    // Sóng âm 2
                    float dist2 = Vector2.Distance(new Vector2(x, y), new Vector2(c.x + 8, c.y));
                    if (dist2 >= 38f && dist2 <= 43f && x > c.x + 28 && Mathf.Abs(y - c.y) <= (x - (c.x + 16)) * 1.15f)
                    {
                        tex.SetPixel(x, y, cyan);
                    }
                }
            }
            tex.Apply();
            return tex;
        }

        private static Texture2D CreateSaveIcon(int size)
        {
            var tex = CreateEmpty(size);
            Vector2 c = new Vector2(size * 0.5f, size * 0.5f);
            Color blue = new Color(0.35f, 0.65f, 1.0f);
            Color dark = new Color(0.12f, 0.22f, 0.45f);
            Color white = new Color(0.95f, 0.98f, 1.0f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    // Vỏ ngoài đĩa mềm bo góc có vát góc trên phải
                    bool inBody = (x >= c.x - 36 && x <= c.x + 36 && y >= c.y - 36 && y <= c.y + 36);
                    bool cutCorner = (x >= c.x + 24 && y >= c.y + 24 && (x - (c.x + 24) + y - (c.y + 24) >= 14));

                    if (inBody && !cutCorner)
                    {
                        tex.SetPixel(x, y, blue);

                        // Cửa trượt kim loại phía trên
                        if (x >= c.x - 22 && x <= c.x + 16 && y >= c.y + 12 && y <= c.y + 36)
                        {
                            bool inShutterHole = (x >= c.x - 12 && x <= c.x - 2 && y >= c.y + 18 && y <= c.y + 30);
                            tex.SetPixel(x, y, inShutterHole ? dark : white);
                        }

                        // Nhãn giấy phía dưới
                        if (x >= c.x - 26 && x <= c.x + 26 && y >= c.y - 30 && y <= c.y + 2)
                        {
                            bool inLine1 = (y == (int)c.y - 8 && x >= c.x - 18 && x <= c.x + 18);
                            bool inLine2 = (y == (int)c.y - 18 && x >= c.x - 18 && x <= c.x + 18);
                            tex.SetPixel(x, y, (inLine1 || inLine2) ? blue : white);
                        }
                    }
                }
            }
            tex.Apply();
            return tex;
        }

        private static Texture2D CreateEmpty(int size)
        {
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Color clear = new Color(0, 0, 0, 0);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    tex.SetPixel(x, y, clear);
                }
            }
            return tex;
        }

        private static bool Inside(int x, int y, int size)
        {
            return x >= 0 && x < size && y >= 0 && y < size;
        }
    }
}
#endif
