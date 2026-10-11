# SPECIFICATION REQUIREMENT (GDD) – GAME SPRINT 2026

> **Dự án:** Crystal Forge – 3D Idle Incremental Adventure RPG  
> **Cuộc thi:** Game Sprint 2026 (FPT Polytechnic)  
> **Sinh viên thực hiện:** Phùng Đức Quang | **MSSV:** PH68721 | **Lớp:** GA21301  
> **Chuyên ngành:** Lập trình Game – FPT Polytechnic  
> **Lead Game Designer & Co-Founder ảo:** AI Agent Antigravity  
> **Engine:** Unity 6 (6000.5.9f1) URP | **Nền tảng:** PC / Windows Standalone x64  
> **Trạng thái:** ✅ **Small but complete – Đã phát hành bản build Executable (.EXE)**  

---

### 1. Project Overview

- **Game Title:** Crystal Forge – 3D Idle Incremental Adventure RPG
- **Genre:** Idle Game / Incremental Game (kết hợp Adventure Roguelike phụ trợ)
- **Engine/Platform:** Unity 6 (URP) / PC Windows Standalone x64
- **Elevator Pitch:**  
  Crystal Forge là tựa game 3D Idle Incremental nhịp độ nhanh diễn ra trọn vẹn trên một màn hình duy nhất (Single-screen). Người chơi cùng chú hiệp sĩ Capybara thợ rèn khai thác năng lượng tinh thể từ cụm pha lê ma thuật trên đảo bay cổ đại, tự động hóa chuỗi máy móc công nghiệp và tham gia các chuyến thám hiểm Roguelike ngắn để săn quái vật, mang về các bùa lợi nhân đôi sản lượng lò rèn.
- **Unique Selling Proposition (USP) – Tuân thủ triệt để luật Anti-Clone:**
  1. **Dũng sĩ Capybara 3D Duy Nhất (Single-Hero Model):** Không nhân bản mô hình hay chuyển cảnh. Chú thợ rèn Capybara vừa là linh vật lò rèn, ở trạng thái nhàn rỗi (AFK Idle) tự động tuần tra quanh bệ đá gõ đe cộng hưởng tài nguyên, vừa trực tiếp tiến vào vị trí giao đấu khi mở chế độ Thám Hiểm.
  2. **Đấu trường 3D & Thanh máu Overhead Catch-up Bar:** Quái vật 3D (Slime, Shadow Minion, Stone Golem, Dragon Boss) giao tranh tuần tự sống động. Thanh máu 3D trên đỉnh đầu tự xoay theo camera (Billboarding) và tích hợp thanh trượt trễ (Catch-up Delayed Bar) phong cách game đối kháng tạo cảm giác lực đánh cực mạnh mẽ.
  3. **Vòng lặp Roguelike 3-Pick-1 Skills lồng ghép hữu cơ:** Thám hiểm hầm mỏ trao thưởng lượng lớn Crystal tức thì cùng bùa lợi nhân đôi sản lượng toàn cục ($x2.0\text{ Production}$) trong thời gian thực, xóa bỏ triệt để thế nghẽn kinh tế của dòng game Idle truyền thống.

---

### 2. Core Gameplay & Mechanics

#### 2.1. Core Loop khép kín (4 bước chuẩn: Progress ➔ Auto ➔ Earn ➔ Upgrade)

$$\text{Progress} \longrightarrow \text{Auto} \longrightarrow \text{Earn} \longrightarrow \text{Upgrade} \longrightarrow \text{Progress}$$

- **Progress (Tiến trình & Khai thác khởi đầu):**  
  Người chơi khởi đầu bằng cách chạm/click chuột vào cụm Pha Lê Ma Thuật trung tâm (`Crystal Core`) để thu hoạch `Crystal Energy` theo chỉ số `Tap Power`. Song song đó, khi người chơi treo máy (AFK), chú Capybara tự động đi tuần tra quanh lò rèn và định kỳ mỗi 2.2 giây vung búa nện đe phát ra tia lửa điện sinh thêm tài nguyên thưởng.
- **Auto (Tự động hóa):**  
  Người chơi mở khóa và nâng cấp 3 cỗ máy khai khoáng tự động cổ đại:
  1. *Crystal Drill (Mũi khoan pha lê):* Khoan sâu vào lòng đá khai thác liên tục.
  2. *Mana Furnace (Lò luyện mana):* Nung chảy quặng thô thành năng lượng tinh khiết.
  3. *Auto Extractor (Máy chiết xuất tự động):* Hút năng lượng từ trường không gian xung quanh đảo bay.
- **Earn (Tạo tài nguyên):**  
  Đơn vị tiền tệ chính là **Crystal Energy (Pha Lê Tím)** và chỉ số trọn đời **Lifetime Crystal**. Tài nguyên tự động sinh ra liên tục theo từng frame qua hệ thống `GeneratorSystem`, đồng thời tích lũy ngoại tuyến (Offline Earnings) lên đến 8 giờ với hiệu suất 50% khi người chơi thoát game.
- **Upgrade (Nâng cấp & Cột mốc):**  
  Tái đầu tư Crystal để nâng cấp Lực gõ (`Tap Power`) và cấp bậc của 3 cỗ máy tự động. Việc tích lũy tài nguyên giúp người chơi chinh phục 5 Cột mốc (`Milestones`: Bronze ➔ Silver ➔ Gold ➔ Platinum ➔ Guardian) nhận bùa lợi nhân vĩnh viễn sản lượng toàn cầu (+25%, x2.0 Global Multiplier), mở khóa cỗ máy mới và tiến sâu vào 5 tầng Thám hiểm Adventure.

#### 2.2. Game Layout – Bố cục Single-screen Wireframe (1 Màn Hình Duy Nhất)

Toàn bộ gameplay gói gọn trên **1 màn hình duy nhất**, camera Isometric 3D cố định góc $32^\circ$, không chuyển Scene phức tạp:

```
+-----------------------------------------------------------------------------------+
|  [CRYSTAL FORGE]   Energy: 125.4K   |   +850/s (x1.25)   |   Lifetime: 450.0K     | <- TOP BAR
+-----------------------------------------------------------------------------------+
|  [LEFT: MILESTONES]      |                   [CENTER: 3D DIORAMA]                | [RIGHT: UPGRADES] |
|                          |                                                       |                   |
| - Bronze: [HOAN THANH]   |                 / \                                   | [CARD 1: TAP]     |
|   (Mo khoa Drill)        |                /   \     (Floating Island)            | Tap Power Lv.15   |
|                          |               /     \                                 | Cost: 350 [MUA]   |
| - Silver: [HOAN THANH]   |              |  [Amethyst  |                          |                   |
|   (+25% Production)      |              |   Cluster]  |                          | [CARD 2: DRILL]   |
|                          |              |    [Core]   |                          | Crystal Drill L.10|
| - Gold: [TIEN TRINH 85%] |                 \     /      (Capybara Hero AFK)      | Cost: 405 [MUA]   |
|   [============    ]     |                  \   /       [Patrol & Hammer]        |                   |
|   (Mo khoa Furnace)      |                   \ /                                 | [CARD 3: FURNACE] |
|                          |                                                       | Mana Furnace L.4  |
| - Platinum: [KHOA]       |   -------------------------------------------------   | Cost: 4.3K [MUA]  |
| - Guardian: [KHOA]       |   * Khi mo Thám Hiểm: Overlay hầm mỏ xuất hiện ở đây  |                   |
|                          |   * Capybara giao đấu Quái vật 3D + Overhead HP Bar   | [CARD 4: EXTRACT] |
| [Offline Claim: +5.2K]   |   * Pop-up 3-Pick-1 Roguelike Skills chon tuc thi     | Auto Extractor L.1|
|                          |   * Khong bao gio chuyen Scene hay load map ngoai     | Cost: 25K [MUA]   |
+-----------------------------------------------------------------------------------+-------------------+
|  [⚔️ THAM HIEM ADVENTURE]          |          [💾 LUU GAME]          |          [🔊 AM THANH: ON]   | <- BOTTOM BAR
+-------------------------------------------------------------------------------------------------------+
```

---

### 3. Economy & Upgrade Systems

#### 3.1. Danh mục Tài nguyên (Resources)

1. **Crystal Energy (Pha Lê Tím):** Đơn vị tiền tệ khả dụng dùng để thanh toán mua và nâng cấp toàn bộ hệ thống lò rèn.
2. **Lifetime Crystal (Pha Lê Trọn Đời):** Tổng số tài nguyên người chơi đã thu hoạch từ đầu game đến nay; chỉ số này không bao giờ bị trừ đi khi mua sắm, dùng làm thước đo chuẩn để mở khóa các mốc Milestone.
3. **Production Per Second (+Crystal/s):** Tốc độ tự động sản xuất tinh thể mỗi giây từ toàn bộ hệ thống máy móc cộng dồn.
4. **Tap Power (+Crystal/click):** Lượng tinh thể nhận được tức thì mỗi lần nhấp chuột vào bệ đá hoặc khi Capybara nện búa.

#### 3.2. Hệ thống Nâng cấp cốt lõi (4 Upgrades + 5 Milestones)

Mọi nâng cấp đều tuân thủ công thức toán học lũy tiến hàm mũ:
$$\text{Cost}(\text{level}) = \text{BaseCost} \times (\text{Multiplier})^{\text{level}}$$

| STT | Tên Nâng Cấp (Upgrade) | Chức Năng Cốt Lõi | Base Cost | Multiplier | Sản Lượng Cơ Bản | Điều Kiện Mở Khóa |
| :---: | :--- | :--- | :---: | :---: | :---: | :--- |
| **1** | **Tap Power** (Lực Gõ Pha Lê) | Tăng sản lượng Crystal nhận được trên mỗi cú click/nện đe (+1/Lv). | 25 | 1.18 | +1.0 / click | Mặc định cấp 0 |
| **2** | **Crystal Drill** (Mũi Khoan Pha Lê) | Cỗ máy tự động khai thác vỉa quặng cấp thấp đầu game. | 100 | 1.15 | +1.0 / giây | Đạt mốc Bronze (100 Lifetime) |
| **3** | **Mana Furnace** (Lò Nung Mana) | Lò luyện ma thuật nung chảy quặng thô, tạo bước nhảy vọt kinh tế. | 2,500 | 1.15 | +8.0 / giây | Đạt mốc Gold (25,000 Lifetime) |
| **4** | **Auto Extractor** (Máy Chiết Xuất) | Trụ công nghệ cao hút năng lượng từ trường, tăng trưởng vũ bão cuối game. | 25,000 | 1.15 | +50.0 / giây | Đạt mốc Platinum (250,000 Lifetime) |

#### 3.3. Hệ thống Cột mốc Toàn cầu (Milestones)

- **Bronze (100 Lifetime):** Mở khóa mua máy phát `Crystal Drill`.
- **Silver (2,500 Lifetime):** Thưởng vĩnh viễn nhân hệ số **+25% Global Production (x1.25)**.
- **Gold (25,000 Lifetime):** Mở khóa mua máy phát `Mana Furnace` + Mở Thám hiểm Tầng 2.
- **Platinum (250,000 Lifetime):** Thưởng vĩnh viễn nhân hệ số **x2.0 Global Production** + Mở khóa `Auto Extractor`.
- **Guardian (1,000,000 Lifetime):** Mở khóa Trùm Cuối Tháp Tinh Thể (`Crystal Guardian Boss`).

---

### 4. Game Feel & Feedback

#### 4.1. Visual Feedback (Phản hồi thị giác)

- **Particle Systems (VFX):**
  - `VFX_ClickSparks`: Chùm tia lửa điện ma thuật màu tím và vụn đá vàng tóe ra khi gõ vào khối tinh thể.
  - `VFX_MilestoneBurst`: Pháo hoa ánh kim rực rỡ bùng nổ khi đạt mốc cột mốc tiến trình mới.
  - `Sparkle Particles`: Bụi sao lấp lánh bay lượn huyền ảo quanh đảo bay và cụm tinh thể Amethyst.
- **Tween Animation & Elastic Feel:**
  - `Punch Scale`: Cụm Crystal Core co giãn đàn hồi tức thì khi bị tác động lực, tạo cảm giác đầm tay và thỏa mãn.
  - `Button Juice Bounce`: Toàn bộ các thẻ bài và nút nâng cấp nảy nhẹ khi nhấp chuột mua thành công.
- **Pop-up Floating Text:**
  - Chữ số nhảy màu vàng kim (`+10`, `CRIT!`) phóng to và bay dần lên không trung khi click hoặc nhận thưởng.
- **Overhead 3D Catch-up Health Bar:**
  - Thanh máu 3D trên đỉnh đầu nhân vật xoay mặt theo Camera (Billboard), tích hợp thanh trượt trễ (Catch-up Delayed Bar) đổi màu đỏ/xanh tạo độ giật lực đánh mãn nhãn như game đối kháng.

#### 4.2. Audio Feedback (Phản hồi thính giác – Procedural Audio Synthesizer)

- **Nhạc nền (BGM Ambient Loop):**
  - Bản nhạc nền phong cách Fantasy Lò Rèn ma thuật êm dịu, lặp tuần hoàn (vòng hợp âm Am – F – C – G) mang lại trải nghiệm thư giãn khi treo máy lâu dài.
- **Hiệu ứng âm thanh (SFX):**
  - *Click SFX:* Tiếng gõ lách cách trong trẻo của đá quý và tinh thể khi tương tác.
  - *Upgrade SFX:* Tiếng đe búa nện kim loại giòn giã kèm âm hưởng leng keng tiền vàng khi mua nâng cấp.
  - *Combat SFX:* Tiếng vung búa rít gió, tiếng va đập giáp kim loại và tiếng nổ ma thuật khi kích hoạt chiêu thức Fireball/Lightning.
  - *Milestone & Victory SFX:* Hợp âm kèn ngũ cung ngân vang hào sảng báo hiệu vượt mốc tiến trình và chiến thắng hầm mỏ.

---

### 5. AI Implementation Log

Nhóm phát triển đã tích hợp toàn diện các công cụ AI vào quy trình sản xuất theo đúng tôn chỉ *"AI là trợ thủ tăng tốc, con người giữ quyền quyết định"*:

#### 5.1. AI Art (Midjourney / Stable Diffusion / Gemini Vision)
- **Hạng mục tạo dựng:** Concept 3D Diorama Đảo Nổi Lò Rèn, Cụm Tinh Thể Amethyst đa diện, Tạo hình Dũng sĩ Capybara thợ rèn Chibi đeo balo đá quý, và bộ 4 Quái vật Thám hiểm 3D.
- **Prompt mẫu tiêu biểu:**
  > *"Isometric 3D diorama of a magical floating island forge workshop, a glowing giant amethyst crystal cluster at center emitting purple light, ancient stone runes, anvil and gold trim, cozy cute fantasy style, stylized low poly art, vibrant cinematic lighting, clean game asset, Unreal Engine 5 render style, 8k --ar 16:9"*
  >
  > *"Cute chibi Capybara blacksmith warrior standing heroically, holding a glowing purple crystal hammer and small golden shield, wearing leather adventurer backpack filled with gems, tiny gold crown on head, adorable cartoon stylized 3D character, clean background --ar 1:1"*

#### 5.2. AI Code (Antigravity AI Agent / Claude 3.7 / GPT-4o)
- **Hạng mục tối ưu & lập trình:**
  - Thiết lập kiến trúc Data-driven với hệ thống ScriptableObjects cho Quái vật, Kỹ năng, Sự kiện và Máy phát.
  - Xây dựng thuật toán chiến đấu tuần tự Turn-based kết hợp thanh máu Overhead Catch-up Delayed Bar 3D không bị lỗi scale.
  - Viết công cụ khử lỗi phông chữ tự động `FontSanitizer.cs` xử lý triệt để lỗi ô vuông `□` trên TextMeshPro Unity 6.
- **Prompt mẫu tiêu biểu:**
  > *"Viết class OverheadHealthBar3D kế thừa MonoBehaviour trong Unity 6 URP, hiển thị thanh máu World-Space trên đầu quái vật 3D, tự xoay mặt về Main Camera (Billboard), tích hợp thanh trượt trễ (delayed fill catch-up bar) giảm dần với tốc độ mượt mà khi nhận sát thương, tối ưu không dùng camera.main liên tục trong Update."*

#### 5.3. AI Audio (Procedural Audio Synthesizer / Suno AI)
- **Hạng mục tạo dựng:**
  - Thiết lập thuật toán tổng hợp âm thanh toán học (Procedural Audio Waveform) trực tiếp bằng mã nguồn Unity C# cho toàn bộ SFX tiếng gõ đá pha lê, tiếng leng keng đe búa, tiếng kèn ngũ cung khải hoàn và nhạc nền Fantasy Ambient.
- **Prompt mẫu tiêu biểu:**
  > *"Viết script AudioManager tạo AudioClip bằng thuật toán toán học thời gian thực (Procedural Sine/Square waves) trong C# Unity để giả lập tiếng gõ lách cách của đá quý tần số cao (1200Hz decay nhanh) và tiếng chuông ngũ cung (chords C-E-G-B) khi mở khóa thành tựu mà không cần import file wav bên ngoài."*
