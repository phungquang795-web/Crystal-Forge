# GAME DESIGN DOCUMENT (GDD) – CRYSTAL FORGE
> **Dự án:** Game Sprint 2026 | **Đơn vị:** Cao đẳng FPT Polytechnic  
> **Sinh viên thực hiện:** Phùng Đức Quang | **MSSV:** PH68721 | **Lớp:** GA21301  
> **Vai trò đồ án:** Lead Game Designer & Co-Founder  
> **Engine:** Unity 6 (6000.5.9f1) | **Render Pipeline:** Universal Render Pipeline (URP)  
> **Nền tảng mục tiêu:** PC / Windows Standalone x64 | **Định dạng:** Single-Screen 3D Diorama  
> **Trạng thái dự án:** ✅ **RELEASE READY (Đã đóng gói Executable .EXE)**  

---

## 💎 1. TỔNG QUAN DỰ ÁN & ĐIỂM KHÁC BIỆT CỐT LÕI (HIGH CONCEPT & USP)

### 1.1. High Concept
**Crystal Forge** là tựa game **Idle / Incremental 3D** nhịp độ nhanh kết hợp phụ trợ **Adventure Roguelike** phong cách *Capybara Go!*. Người chơi vào vai người quản lý Lò Rèn Pha Lê huyền bí đặt trên một hòn đảo bay cổ đại giữa ngân hà:
* **Tương tác thủ công (Manual Click):** Chạm vào *Crystal Core* (Cụm Tinh Thể Đa Diện phát quang) để thu hoạch *Crystal Energy*. Chú thợ rèn **Capybara 3D** sẽ lập tức vung búa gõ đe cộng hưởng phát ra tài nguyên.
* **Tự động hóa sản xuất (Automation):** Mua sắm và nâng cấp 3 cỗ máy khai khoáng cổ đại (*Crystal Drill*, *Mana Furnace*, *Auto Extractor*) để sinh tài nguyên thụ động theo giây (Idle/s).
* **Tuần tra nhàn rỗi (AFK Patrol Loop):** Chú Capybara tự động đi tuần tra nhún nhảy quanh lò rèn, định kỳ nện búa vào đe rèn sinh thêm tài nguyên thưởng ngay cả khi người chơi không bấm chuột.
* **Thám hiểm Đấu Trường 3D (Roguelike Adventure):** Sử dụng sức mạnh lò rèn dấn thân vào hầm mỏ 5 tầng: đối mặt sự kiện ngẫu nhiên (Event), chiến đấu theo lượt với dàn Quái vật 3D (Slime, Shadow Minion, Golem, Dragon Boss), lựa chọn 1 trong 3 kỹ năng cổ đại (3-Pick-1 Skill) để nhận thưởng Crystal khổng lồ cùng Buff nhân đôi sản lượng Lò Rèn.

### 1.2. Điểm khác biệt cốt lõi (USP - Anti-Clone)
1. **Model Dũng Sĩ Capybara 3D Duy Nhất (Single-Hero Model):** Không chuyển cảnh, không nhân bản model phụ. Chú Capybara thợ rèn vừa là linh vật lò rèn, vừa trực tiếp di chuyển vào vị trí giao chiến khi bước vào chế độ Thám Hiểm.
2. **Hệ thống Đấu Trường 3D & Thanh Máu Overhead Catch-Up Bar:** Toàn bộ quái vật và dũng sĩ đều hiển thị dưới dạng model 3D chuyển động sống động với thanh máu gắn trên đầu (Overhead Health Bar) hỗ trợ thanh trượt trễ (Catch-up Delayed Bar) phong cách game đối kháng.
3. **Trải nghiệm Đơn Màn Hình Liền Mạch (Single-Screen 100%):** Mọi chỉ số, tương tác 3D, danh sách nâng cấp, cột mốc tiến trình và modal thám hiểm đều gói gọn trên 1 Camera Isometric duy nhất, giữ nguyên vẹn dòng chảy tài nguyên và không tải lại Scene.

---

## ⚖️ 2. ĐỐI CHIẾU 10 NGUYÊN TẮC CỨNG (SCOPE & COMPLIANCE VERIFICATION)

| STT | Nguyên Tắc Cứng | Yêu Cầu Directive | Hiện Trạng Thực Tế Tại Codebase Crystal Forge | Đánh Giá |
| :---: | :--- | :--- | :--- | :---: |
| **1** | **Thể loại duy nhất** | Bắt buộc là Idle / Incremental Game | Vòng lặp khai thác tinh thể thụ động kết hợp mini-loop Adventure Roguelike | ✅ ĐẠT 100% |
| **2** | **Thời hạn phát triển** | Tối đa 7 ngày từ Prototype $\rightarrow$ Release | Hoàn thành toàn bộ tính năng và build xong file `.exe` trong đúng 7 ngày | ✅ ĐẠT 100% |
| **3** | **Triết lý Scope** | "Small but complete" – Không dở dang | 100% tính năng hoạt động trọn vẹn, không có placeholder hay lỗi runtime | ✅ ĐẠT 100% |
| **4** | **Giao diện UI/UX** | Single-screen Wireframe (1 màn hình) | Toàn bộ diễn ra trên `SampleScene.unity` qua Camera góc 3/4 và Canvas Overlay | ✅ ĐẠT 100% |
| **5** | **Core Loop 4 bước** | Progress $\rightarrow$ Auto $\rightarrow$ Earn $\rightarrow$ Upgrade | Chu trình khép kín: Khai thác $\rightarrow$ Máy phát $\rightarrow$ Thu nhập $\rightarrow$ Nâng cấp Lò rèn | ✅ ĐẠT 100% |
| **6** | **Hệ thống kinh tế** | 3 đến 5 nâng cấp có công thức lũy tiến | Gồm 1 nâng cấp Lực Gõ (Tap Power) + 3 Máy phát tự động (Drill, Furnace, Extractor) | ✅ ĐẠT 100% |
| **7** | **Luật Anti-Clone** | Có ít nhất 1 điểm khác biệt (USP) | Cơ chế Capybara Hero 3D tuần tra Lò rèn & Đấu trường thám hiểm Roguelike 3D | ✅ ĐẠT 100% |
| **8** | **Human-Led UI/UX** | Single-screen bố cục trực quan | Bố cục 4 phân khu (Header, Left Milestones, Center 3D, Right Upgrades) | ✅ ĐẠT 100% |
| **9** | **Chuyển đổi TDD** | Kiến trúc dữ liệu ScriptableObject & Managers | Hệ thống 8 Manager Classes độc lập, cấu hình qua ScriptableObject Data | ✅ ĐẠT 100% |
| **10**| **Out-of-Scope** | Cấm triệt để: Open World, Multi, Cloud, IAP | Đã loại trừ 100% mọi yếu tố vi phạm, chỉ dùng Local Storage an toàn | ✅ ĐẠT 100% |

---

## 🔄 3. VÒNG LẶP CỐT LÕI (CORE LOOPS)

### 3.1. Vòng lặp Chính: Kinh Tế Lò Rèn Nhàn Rỗi (Primary Idle Economy Loop)
$$\begin{matrix}
\text{[CLICK / AFK PATROL]} & \longrightarrow & \text{[TÍCH LŨY CRYSTAL]} & \longrightarrow & \text{[MUA / NÂNG CẤP]} \\
\uparrow & & & & \downarrow \\
\text{[MỞ KHÓA CỘT MỐC]} & \longleftarrow & \text{[TĂNG PRODUCTION/S]} & \longleftarrow & \text{[CỖ MÁY KHAI KHOÁNG]}
\end{matrix}$$

1. **Progress (Khai thác thủ công & AFK):** Người chơi click vào cụm Crystal Core (+TapPower Crystal) hoặc để chú Capybara tự động tuần tra gõ đe rèn (+25% TapPower).
2. **Auto (Tự động hóa):** Mua các máy phát tự động khi tích lũy đủ Crystal.
3. **Earn (Thu nhập liên tục):** Hệ thống tích lũy tài nguyên theo từng frame qua `GeneratorSystem`, hỗ trợ nhận thưởng tích lũy Offline tối đa 8 giờ.
4. **Upgrade (Nâng cấp & Cột mốc):** Tái đầu tư vào Lực gõ và Máy phát để vượt ngưỡng Cột mốc (Milestones), kích hoạt các bùa lợi nhân sản lượng toàn cầu (Global Multiplier).

### 3.2. Vòng lặp Phụ: Thám Hiểm Hầm Mỏ Roguelike (Secondary Adventure Loop)
$$\text{Start Adventure} \longrightarrow \text{Tầng 1..5} \longrightarrow \left[ \begin{matrix} \text{Sự kiện Bí Ẩn (Event Choice)} \\ \text{hoặc} \\ \text{Đấu Trường 3D (Turn Combat)} \end{matrix} \right] \longrightarrow \text{3-Pick-1 Skill} \longrightarrow \text{Reward: Crystal + Buff} \longrightarrow \text{Bứt Phá Lò Rèn}$$

* **Tính liên kết hữu cơ:**
  - Sản lượng Idle cung cấp tài nguyên nâng cấp các mốc Milestone để mở khóa độ sâu thám hiểm.
  - Thám hiểm trả về lượng lớn Crystal tức thì kèm **Buff nhân đôi sản lượng lò rèn (x2.0 Production)** trong thời gian thực, phá vỡ thế tắc nghẽn (bottleneck) kinh tế.

---

## 📈 4. TOÁN HỌC KINH TẾ & CÂN BẰNG NÂNG CẤP (ECONOMY BALANCING)

### 4.1. Công thức chi phí nâng cấp lũy tiến
Mọi nâng cấp trong trò chơi tuân thủ nghiêm ngặt công thức hàm mũ chuẩn quốc tế:
$$\text{Cost}(\text{level}) = \text{BaseCost} \times (\text{Multiplier})^{\text{level}}$$

Trong đó:
* $\text{BaseCost}$: Giá khởi điểm ở cấp độ 0.
* $\text{Multiplier}$: Hệ số tăng giá sau mỗi cấp (dao động từ $1.15$ đến $1.18$).
* $\text{level}$: Cấp độ hiện tại của cỗ máy / kỹ năng.

### 4.2. Bảng cân bằng thông số thực tế trong Game
| Mã Nâng Cấp | Tên Cỗ Máy / Kỹ Năng | Base Cost | Hệ Số Multiplier | Sản Lượng Cơ Bản | Điều Kiện Mở Khóa | Vai Trò Thiết Kế |
| :--- | :--- | :---: | :---: | :---: | :---: | :--- |
| `tap_power` | **Luc Go Pha Le** | 25 | 1.18 | +1.0 Crystal / Click | Cấp 0 (Mặc định) | Tạo sự cuốn hút và cảm giác quyền năng tương tác ban đầu |
| `gen_drill` | **Crystal Drill** | 100 | 1.15 | +1.0 Crystal / s | Đạt mốc Bronze (100 Lifetime) | Bắt đầu chu trình tự động hóa nhàn rỗi đầu game |
| `gen_furnace` | **Mana Furnace** | 2,500 | 1.15 | +8.0 Crystal / s | Đạt mốc Gold (25,000 Lifetime) | Tạo bước nhảy vọt kinh tế trung hạn |
| `gen_extractor` | **Auto Extractor** | 25,000 | 1.15 | +50.0 Crystal / s | Đạt mốc Platinum (250,000 Lifetime) | Động cơ tăng trưởng tài nguyên cực lớn cuối game |

### 4.3. Công thức thu nhập Offline (Offline Progress Formula)
$$\text{OfflineSeconds} = \min\left( \text{CurrentTime} - \text{LastSaveTime},\; 28,800 \text{ giây} \right)$$
$$\text{OfflineEarnings} = \text{ProductionPerSec} \times \text{OfflineSeconds} \times 0.5$$
*(Giới hạn tối đa 8 tiếng với hiệu suất 50% nhằm thúc đẩy người chơi quay trở lại game).*

---

## 🏆 5. HỆ THỐNG CỘT MỐC TIẾN TRÌNH (MILESTONE SYSTEM)

| Cột Mốc | Ngưỡng Lifetime Crystal | Phần Thưởng Mở Khóa | Giá Trị Thực Chiến |
| :---: | :---: | :--- | :--- |
| **🥉 Bronze** | **100** | Mở khóa mua máy phát `Crystal Drill` | Kích hoạt vòng lặp Idle tự động đầu tiên |
| **🥈 Silver** | **2,500** | Thưởng vĩnh viễn **+25% Global Production** | Bứt tốc tích lũy qua giai đoạn đầu game |
| **🥇 Gold** | **25,000** | Mở khóa mua máy phát `Mana Furnace` | Mở rộng quy mô công xưởng lò rèn |
| **💎 Platinum** | **250,000** | Mở khóa mua máy phát `Auto Extractor` | Tối ưu hóa chuỗi sản xuất công nghiệp |
| **👑 Guardian** | **1,000,000** | Thưởng vĩnh viễn **x2.0 Global Production** | Hoàn thành đại nghiệp Thợ Rèn Huyền Thoại |

---

## ⚔️ 6. THIẾT KẾ ĐẤU TRƯỜNG 3D & ADVENTURE ROGUELIKE

### 6.1. Dàn Quái Vật 3D Đa Dạng (Enemy 3D Roster)
| Quái Vật | Tầng Xuất Hiện | HP Cơ Bản | ATK | DEF | Phần Thưởng Crystal | Bùa Lợi Sản Lượng (Buff) |
| :--- | :---: | :---: | :---: | :---: | :---: | :---: |
| **Crystal Slime** | Tầng 1 | 300 | 25 | 5 | 500 | +20% Production (45s) |
| **Shadow Minion** | Tầng 2 - 3 | 750 | 50 | 12 | 1,800 | +35% Production (60s) |
| **Golem Guardian**| Tầng 3 - 4 | 1,600 | 85 | 25 | 4,500 | +50% Production (60s) |
| **Dragon Boss** | Tầng 5 (Trùm) | 3,500 | 140 | 40 | 15,000 | **x2.0 Production (90s)** |

### 6.2. Cơ chế Thanh Máu Trên Đầu (Overhead Health Bar)
* **Billboarding Thông Minh:** Tự động xoay mặt về góc nhìn Camera (`LateUpdate`), đảm bảo người chơi luôn quan sát rõ lượng máu ở mọi góc quay 3D.
* **Catch-up Delayed Bar:** Thanh trượt phụ màu đỏ/trắng trượt giảm từ từ phía sau thanh máu chính, tạo cảm giác đòn đánh đầy uy lực chuẩn phong cách game đối kháng arcade.
* **Rung chấn (Hit Shake):** Rung nhẹ khi nhận đòn kết hợp text hiển thị số máu chi tiết (`HP / MaxHP`).
* **Mã màu phân biệt:** Màu xanh ngọc lục bảo (`#00FF88`) cho Hero Capybara, màu đỏ nham thạch (`#FF3344`) cho Quái vật.

### 6.3. Bể Kỹ Năng Chọn 1 trong 3 (3-Pick-1 Roguelike Skills)
1. `[LUA] Fireball`: Gây thêm $+60$ sát thương lửa bộc phát sau mỗi 3 lượt đánh.
2. `[SET] Lightning`: $30\%$ tỉ lệ kích hoạt sét giật gây thêm $+45$ sát thương xuyên giáp.
3. `[HUT MAU] Vampiric Core`: Hồi phục lượng máu bằng $20\%$ sát thương gây ra.
4. `[KHIEU] Crystal Aegis`: Giáp hộ thể tinh thể, giảm vĩnh viễn $25\%$ sát thương nhận vào.
5. `[CHI MANG] Precision Focus`: Tăng thêm $+25\%$ tỉ lệ đánh chí mạng ($x2.0$ sát thương).

---

## 🎨 7. ĐỒ HỌA 3D DIORAMA & CHIẾU SÁNG CINEMATIC

### 7.1. Cụm Tinh Thể Đa Diện Thần Thoại (Amethyst Crystal Core)
* **Tháp Tinh Thể Spire:** Đặt tại tâm tọa độ $(0, 1.65, 0)$, xoay góc 45° tạo các mặt phản quang sắc cạnh.
* **Lõi Năng Lượng Inner Core:** Khối cầu phát sáng rực rỡ bên trong với Emission Shader URP cực mạnh.
* **Vành Tinh Thể Facet Diamonds:** 4 khối kim cương vát góc bao quanh tháp chính.
* **Vành Cổ Ngữ Runic Halo Ring:** Vòng hào quang lam ngọc xoay đồng trục.
* **Mảnh Vệ Tinh (Satellite Shards):** 4 mảnh tinh thể bay lơ lửng, tự động xoay nghịch hướng liên tục qua script `CrystalCoreVisual.cs`.
* **Bụi Sao Ma Thuật (Sparkle Particles VFX):** Hệ thống hạt ánh sáng tím - vàng kim bốc lên liên tục vào không gian.

### 7.2. Đảo Nổi Huyền Bí (Floating Sky Island Diorama)
* **Sàn Đảo Hoa Cương:** Phiến đá obsidian sẫm màu $(8.8 \times 0.4 \times 7.8)$ với viền kim loại vàng hoàng kim vát cạnh (`Floor_GoldTrim`).
* **Trận Đồ Ma Thuật Khắc Sàn:** Vòng tròn cổ ngữ dát vàng kết hợp vòng hào quang ngọc lam phát sáng.
* **Đáy Đảo Treo Ngược (Laputa Under-Roots):** 7 cụm đá nhũ xếp tầng thon nhọn cắm thẳng xuống khoảng không vô tận (độ sâu từ $-0.6m$ đến $-2.4m$, đảm bảo không bao giờ xuyên sàn).
* **4 Trụ Cổ Ngữ (Corner Runic Pylons):** Dựng tại 4 góc hòn đảo, thắp sáng khối cầu ngọc lam Cyan.
* **Đe Rèn Thần Thoại (Prop_CrystalAnvil):** Khối đe hắc diện thạch với 2 sừng đe hoàng kim nằm trên bệ đá trung tâm.

### 7.3. Chiếu Sáng 3 Điểm (3-Point Cinematic Lighting) & URP Post-Processing
* **Key Light:** Ánh sáng mặt trời hoàng hôn ấm áp $(1.0, 0.95, 0.90)$ tạo bóng đổ mịn (Soft Shadows).
* **Rim Light (Cyan):** Đèn viền xanh lam $(0.25, 0.55, 1.0)$ chiếu ngược từ phía sau, tách bạch viền nhân vật và cảnh vật 3D trên nền không gian tối.
* **Crystal Point Light:** Nguồn sáng tím $(0.92, 0.3, 1.0)$ nhịp thở (breathing light) phát ra từ tâm cụm tinh thể.
* **URP Post-Processing Profile:**
  - **Bloom:** Ngưỡng thấp $0.75$, cường độ $0.85$, độ tán xạ $0.65$ tạo độ phát quang lộng lẫy cho pha lê và đòn đánh.
  - **Vignette:** Tím đen sẫm $(0.08, 0.04, 0.12)$, cường độ $0.32$ tạo chiều sâu vũ trụ tập trung vào tâm màn hình.

---

## 🔤 8. HỆ THỐNG PHÔNG CHỮ & TYPOGRAPHY SẮC NÉT (FONT SANITIZATION)

* **Vấn đề đã xử lý:** Font mặc định `LiberationSans SDF` không hỗ trợ bảng mã tiếng Việt có dấu và các emoji, gây lỗi ô vuông (`□`).
* **Kiến trúc giải pháp ([FontSanitizer.cs](file:///d:/Ga21301/GAme/Assets/Scripts/Utils/FontSanitizer.cs)):**
  - Khử toàn bộ dấu tiếng Việt sang bảng Latinh chuẩn (ví dụ: `THÁM HIỂM` $\rightarrow$ `THAM HIEM`, `NÂNG CẤP LÒ RÈN` $\rightarrow$ `NANG CAP LO REN`, `LỰC GÕ` $\rightarrow$ `LUC GO`).
  - Chuyển đổi emoji lạ sang gaming brackets chuẩn: `[FIRE]`, `[LIGHTNING]`, `[AEGIS]`, `[CHI MANG]`, `[HUT MAU]`, `[*]`, `[+]`, `[X]`.
  - Hàm quét tự động `UIManager.SanitizeAllSceneTexts()` thực thi ngay tại `Awake()` và `Start()`, tự động chuẩn hóa mọi TextMeshPro tĩnh và động trên toàn bộ Scene.
  - Kết quả: **Hiển thị sắc nét 100%, không còn bất kỳ một ký tự lỗi ô vuông nào trên toàn bộ giao diện và nhật ký chiến đấu.**

---

## 🖥️ 9. THIẾT KẾ BỐ CỤC ĐƠN MÀN HÌNH (SINGLE-SCREEN WIREFRAME)

```
+---------------------------------------------------------------------------------------------------+
| CRYSTAL FORGE                    494 (+1.0/s)                               Luc go: +1.0 | Tong: 594 |
+-------------------------------+-----------------------------------+-------------------------------+
|  [*] COT MOC TIEN TRINH       |        [CENTER 3D DIORAMA]        |      [+] NANG CAP LO REN      |
|                               |                                   |                               |
|  Silver Milestone             |    ✨ Multi-Faceted Crystal Core  |  Luc Go Pha Le                |
|  [████████░░░░░░░░] 24%       |       (Floating & Rotating)       |  Cap 4 (+5.0/Click)   [MUA 48]|
|  427 / 2.50K                  |    💫 Sparkle Particles & Shards  |                               |
|  Thuong: +25% Global Prod     |                                   |  Crystal Drill                |
|                               |    🦫 Capybara Hero (AFK Patrol)  |  Cap 1 (+1.0/s)      [MUA 114]|
|  HUY HIEU DA DAT              |       (Walking Circle & Striking) |                               |
|  [*] Bronze (100)             |                                   |  Mana Furnace                 |
|                               |    🔨 Obsidian Anvil with Gold    |  Can 25.00K Lifetime          |
|                               |    🗿 Floating Island with Roots  |                               |
|                               |    🔮 Runic Pylons & Cyan Orbs    |  Auto Extractor               |
|                               |                                   |  Can 250.00K Lifetime         |
+-------------------------------+-----------------------------------+-------------------------------+
|        [ Am Thanh ]           |      [ THAM HIEM ADVENTURE ]      |          [ Luu Game ]         |
+---------------------------------------------------------------------------------------------------+
| >>> [OVERLAY MODAL] THAM HIEM MO QUANG (TANG 1/5) - 3D TURN COMBAT / EVENT / 3-PICK-1 SKILL       |
+---------------------------------------------------------------------------------------------------+
```

---

## 🏛️ 10. KIẾN TRÚC KỸ THUẬT UNITY (TECHNICAL ARCHITECTURE)

```
                            ┌────────────────────────┐
                            │      GameManager       │
                            └───────────┬────────────┘
                                        │
     ┌──────────────────┬───────────────┼───────────────┬──────────────────┐
     ▼                  ▼               ▼               ▼                  ▼
┌──────────────┐ ┌──────────────┐ ┌──────────────┐ ┌──────────────┐ ┌──────────────┐
│ResourceManager│ │GeneratorSystem│ │UpgradeManager│ │MilestoneMgr  │ │SaveLoadSystem│
└──────────────┘ └──────────────┘ └──────────────┘ └──────────────┘ └──────────────┘
         │                                                      │
         └──────────────────────┬───────────────────────────────┘
                                ▼
                    ┌────────────────────────┐
                    │       UIManager        │
                    │  (FontSanitizer Auto)  │
                    └───────────┬────────────┘
                                │
        ┌───────────────────────┴───────────────────────┐
        ▼                                               ▼
┌────────────────────────┐                    ┌────────────────────────┐
│   Adventure3DManager   │                    │    AdventureModalUI    │
│  (Hero Patrol & State) │◄──────────────────►│  (Overlays & Turn Log) │
└───────────┬────────────┘                    └────────────────────────┘
            │
            ├───────────────────────────────────────────┐
            ▼                                           ▼
┌────────────────────────┐                    ┌────────────────────────┐
│     CombatUnit3D       │                    │   OverheadHealthBar    │
│ (Hero & Monster Units) │                    │ (Billboard & Catch-up) │
└────────────────────────┘                    └────────────────────────┘
```

### Bảng Danh Mục Script Trọng Yếu:
1. `Assets/Scripts/Core/ResourceManager.cs`: Quản lý ví tiền, tài nguyên tích lũy, tốc độ sản xuất toàn cầu.
2. `Assets/Scripts/Economy/GeneratorSystem.cs`: Điều phối các máy phát tự động sinh tài nguyên theo nhịp deltaTime.
3. `Assets/Scripts/Economy/UpgradeManager.cs`: Tính toán giá nâng cấp theo hàm số mũ và kích hoạt nâng cấp.
4. `Assets/Scripts/Economy/MilestoneManager.cs`: Theo dõi tiến trình Lifetime Crystal và mở khóa huy hiệu, bùa lợi.
5. `Assets/Scripts/Adventure/Adventure3DManager.cs`: Điều khiển Capybara tuần tra AFK quanh lò rèn và điều phối đấu trường 3D.
6. `Assets/Scripts/Adventure/CombatUnit3D.cs`: Xử lý chuyển động nhún nhảy, vung búa, nhận đòn, animation di chuyển của Hero và Quái.
7. `Assets/Scripts/Adventure/OverheadHealthBar.cs`: Thanh máu 3D trên đầu với Billboarding, Delayed Bar, Hit Shake.
8. `Assets/Scripts/Visuals/CrystalCoreVisual.cs`: Xoay đảo chiều các mảnh vệ tinh, hiệu ứng nhịp thở đèn Point Light.
9. `Assets/Scripts/Utils/FontSanitizer.cs`: Bộ lọc chuẩn hóa phông chữ, khử sạch lỗi thiếu glyph tiếng Việt và emoji.
10. `Assets/Scripts/Audio/AudioManager.cs`: Bộ tổng hợp âm thanh thủ tục (Procedural Synth) tạo hiệu ứng búa, chuông, chiến đấu.
11. `Assets/Scripts/VFX/VFXManager.cs`: Hệ thống sinh tia lửa hạt và text số nhảy (Floating Damage/Gain Numbers).
12. `Assets/Scripts/Core/SaveLoadSystem.cs`: Lưu trữ và khôi phục dữ liệu cục bộ an toàn kèm tính toán thu nhập Offline.

---

## 📅 11. NHẬT KÝ TIẾN ĐỘ THỰC HIỆN GAME SPRINT (7 NGÀY HOÀN THIỆN)

* **Ngày 1 (Core Prototype):** Xây dựng `ResourceManager`, cơ chế Click cụm Crystal Core, giao diện Top Bar theo dõi tài nguyên.
* **Ngày 2 (Economy & Upgrades):** Cài đặt công thức lũy tiến, hoàn thiện bảng Right Panel với 4 cỗ máy nâng cấp, hiển thị giá tự động cập nhật.
* **Ngày 3 (Milestones & Offline Save):** Xây dựng Left Panel Cột mốc tiến trình, hệ thống `SaveLoadSystem` mã hóa dữ liệu cục bộ và tính thưởng Offline tối đa 8 tiếng.
* **Ngày 4 (Mini-loop Adventure):** Tích hợp Modal Thám hiểm Roguelike, hệ thống sự kiện ngẫu nhiên (Event Choices) và bùa lợi tạm thời.
* **Ngày 5 (3D Combat & 3-Pick-1 Skills):** Xây dựng Đấu trường theo lượt 3D, bể kỹ năng Roguelike chọn 1 trong 3, liên kết thưởng ngược lại Lò rèn.
* **Ngày 6 (Hero Model Duy Nhất & Thanh Máu Overhead):** Thiết lập chú Capybara tuần tra AFK quanh lò rèn phong cách *Capybara Go!*, dựng 4 model Quái vật 3D, gắn thanh máu trên đầu với hiệu ứng Catch-up trễ.
* **Ngày 7 (Polishing, Font Fix & Release Build):**
  - Cải thiện toàn diện đồ họa 3D Diorama Đảo Nổi, cụm tinh thể đa diện, hiệu ứng ánh sáng 3 điểm và Post-processing Bloom/Vignette.
  - Khử triệt để 100% lỗi phông chữ TextMeshPro bằng `FontSanitizer.cs`.
  - Tự động chạy chu trình kiểm tra biên dịch không lỗi (0 Warning, 0 Error).
  - Đóng gói bản cài đặt Standalone Windows Executable hoàn thiện tại `Builds/CrystalForge_Release/CrystalForge.exe` (105 MB).

---

> **XÁC NHẬN CỦA CO-FOUNDER & LEAD GAME DESIGNER:**  
> Dự án **Crystal Forge** đã hoàn thành 100% mục tiêu đề ra cho Game Sprint 2026, tuân thủ tuyệt đối 10 Điều Ràng Buộc Cứng và triết lý *"Small but complete"*. Sản phẩm sẵn sàng xuất xưởng và trình chiếu tại buổi bảo vệ đồ án!
