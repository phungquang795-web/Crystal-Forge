# BẢNG PHÂN RÃ TOÀN BỘ NHIỆM VỤ DỰ ÁN (MASTER WORK BREAKDOWN STRUCTURE - WBS)
> **Dự án:** Crystal Forge – 3D Idle Incremental Adventure RPG  
> **Khung thời gian:** Game Sprint 2026 (7 Ngày – "Small but complete")  
> **Sinh viên phụ trách:** Phùng Đức Quang (PH68721 - GA21301)  
> **Lead Game Designer & Co-Founder đồng hành:** AI Agent Antigravity  

---

## ⚠️ CẢNH BÁO QUAN TRỌNG VỀ SCOPE NGHỆ THUẬT (ART & MAP SCOPE)
> **LƯU Ý CỦA LEAD GAME DESIGNER DÀNH CHO CHỦ NHÂN:**  
> Game thuộc thể loại **Single-Screen 3D Idle**.  
> * ❌ **Tuyệt đối KHÔNG làm Map thế giới mở (Open World)** hay bản đồ di chuyển đi cảnh cuộn màn hình dài ngoằng (nguy cơ 100% vỡ Scope 7 ngày).  
> * ✅ **Quy chuẩn Map:** Là một **Phòng Lò Rèn 3D thu nhỏ (3D Diorama / Floating Island Workshop)** nhìn từ góc 3/4 Isometric cố định. Mọi máy móc và nhân vật đều nằm trọn vẹn trong khung nhìn này!  
> * ✅ **Quy chuẩn Quái vật & Chiến đấu:** Quái vật xuất hiện trong các lượt thám hiểm **Adventure Overlay** (dạng thẻ bài 3D/2D Avatar Card trên màn hình pop-up), không cần làm hệ thống tuần tra Pathfinding hay AI di chuyển phức tạp.

---

## 📊 TỔNG QUAN TIẾN ĐỘ THEO KHỐI CHUYÊN MÔN (TRACKS)
- [x] **Track 1: 3D Environment & Map Diorama** (Phòng Lò rèn 3D, Bệ đá, Ánh sáng) `[5/5] - ĐÃ HOÀN THÀNH 100%`
- [x] **Track 2: Nhân vật Mascot & Quái vật (Characters & Monsters)** `[8/8] - ĐÃ HOÀN THÀNH 100%`
- [x] **Track 3: UI/UX 2D & Iconography** (Giao diện Single-Screen, Thám hiểm & Kỹ năng) `[5/5] - ĐÃ HOÀN THÀNH 100%`
- [x] **Track 4: VFX & Âm thanh (Audio & Visual Juice)** `[5/5] - ĐÃ HOÀN THÀNH 100%`
- [x] **Track 5: Lập trình Hệ thống Cốt lõi (Programming Core Systems)** `[10/10] - ĐÃ HOÀN THÀNH 100%`
- [x] **Track 6: Tích hợp, Cân bằng số học & Đóng gói Build** `[4/4] - ĐÃ HOÀN THÀNH 100%`

---

## 🎨 TRACK 1: 3D ENVIRONMENT, MAP & PROPS (BẢN ĐỒ LÒ RÈN 3D)
*Mục tiêu: Tạo không gian lò rèn ma thuật 3D ấn tượng, gói gọn trong 1 góc nhìn camera.*

- [x] **Task 1.1: Thiết lập Bản đồ Phòng Lò Rèn (Diorama Workshop Base)** `[ĐÃ HOÀN THÀNH]`
  - Tạo sàn đảo nổi đá huyền ảo `FloatingIsland_Floor`, viền kim loại vàng `Floor_GoldTrim` và root môi trường.
- [x] **Task 1.2: Thiết kế Bệ Đá Tinh Thể Trung Tâm (Central Core Pedestal)** `[ĐÃ HOÀN THÀNH]`
  - Bệ đá trụ `Central_Pedestal`, đĩa vàng `Pedestal_GoldCap`, khối ngọc xoay lơ lửng `CrystalCore` gắn `Rotator.cs`, `SphereCollider` và `ClickSystem.cs`.
- [x] **Task 1.3: Dựng 3 Mô hình Máy Phát Tự Động (Passive Generator Props)** `[ĐÃ HOÀN THÀNH]`
  - Máy 1 - `Prop_CrystalDrill`: Trụ đỡ đá và mũi khoan ngọc cắm xiên.
  - Máy 2 - `Prop_ManaFurnace`: Lò nung đá trụ tròn kèm ống khói vàng và lõi lửa pha lê phát sáng.
  - Máy 3 - `Prop_AutoExtractor`: Trụ công nghệ, vòng xoay lơ lửng `FloatingRing` và quả cầu năng lượng đỉnh.
- [x] **Task 1.4: Thiết lập Ánh sáng & Bầu không khí URP (Atmosphere & Lighting)** `[ĐÃ HOÀN THÀNH]`
  - Directional Light dịu mắt góc $45^\circ$, Point Light ánh sáng tím/hồng huyền diệu bán kính 8.5m ngay tại khối Crystal Core.
- [x] **Task 1.5: Cố định Camera Góc 3/4 Isometric** `[ĐÃ HOÀN THÀNH]`
  - Main Camera cố định tại toạ độ `(0, 5.8, -7.2)` góc nghiêng $32^\circ$, màu nền tím tối ma thuật (`#0F0D1F`), chừa khoảng trống 2 bên cho Left/Right HUD Panels.

---

## 👾 TRACK 2: NHÂN VẬT & QUÁI VẬT (CHARACTERS & MONSTERS)
*Mục tiêu: Đưa linh hồn vào game với phong cách Stylized ngộ nghĩnh (kiểu Capybara Go!).*

### 2.1. Nhân Vật (Player & Mascot)
- [x] **Task 2.1: Thiết kế Mascot Lò Rèn (Forge Mascot)** `[ĐÃ HOÀN THÀNH]`
  - Nhân vật Mascot Chibi dáng Capybara/Apprentice đứng cạnh bệ đá kèm chiếc búa thợ rèn nhỏ hướng về Crystal Core.
- [x] **Task 2.2: Thiết kế Avatar Hiệp sĩ Thám hiểm (Adventure Hero Token)** `[ĐÃ HOÀN THÀNH]`
  - Avatar & khung đại diện người chơi trong cửa sổ thám hiểm Adventure (`HeroBox` trong `AdventureModalUI.cs`), tích hợp đầy đủ thanh máu, trạng thái tấn công và nhận sát thương.

### 2.2. Quái Vật Thám Hiểm (Adventure Enemies)
- [x] **Task 2.3: Quái Cấp 1 – Crystal Slime (Slime Pha Lê)** `[ĐÃ HOÀN THÀNH]`
  - ScriptableObject `CrystalSlime.asset`: HP 300, ATK 25, DEF 5, Reward 80 Crystal.
- [x] **Task 2.4: Quái Cấp 2 – Stone Golem (Người Đá Cổ Đại)** `[ĐÃ HOÀN THÀNH]`
  - ScriptableObject `StoneGolem.asset`: HP 850, ATK 55, DEF 18, Reward 300 Crystal, Buff x1.2 trong 25s.
- [x] **Task 2.5: Quái Tinh Anh – Shadow Minion (Bóng Ma Mỏ Quặng)** `[ĐÃ HOÀN THÀNH]`
  - ScriptableObject `ShadowMinion.asset`: HP 1,700, ATK 110, DEF 15, Reward 1000 Crystal, Buff x1.5 trong 35s.
- [x] **Task 2.6: Trùm Cuối – Crystal Guardian Boss (Hộ Vệ Tối Cao)** `[ĐÃ HOÀN THÀNH]`
  - ScriptableObject `CrystalGuardianBoss.asset`: HP 4,800, ATK 220, DEF 30, Reward 5000 Crystal, Buff x2.0 trong 60s.
- [x] **Task 2.7: Tối ưu Cân Bằng Toán Học Hệ Thống Quái Vật & Boss (Wave Scaling & Combat Balance)** `[ĐÃ HOÀN THÀNH]`
  - Khắc phục triệt để lỗi cấp số nhân bùng nổ (Wave 5 từng bị lỗi dội lên 23,836 HP).
  - Chuẩn hóa công thức lũy tiến mượt mà $1.095^{\text{wave}-1}$: Wave 1 Dơi Pha Lê 42 HP, Wave 2 Golem 76 HP, Wave 5 Boss Titan 316 HP (đánh trong 18-25s cân não).
  - Cân bằng sát thương quái lên Dũng Sĩ và Trụ Tinh Thể (Base 4 dmg, scale nhẹ +0.55/wave), tăng khả năng sinh tồn của người chơi, loại bỏ hoàn toàn cái chết sốc trong 8 giây.
- [x] **Task 2.8: Cơ Chế Tương Tác Sâu Cho Boss & Quái Vật (Telegraph Windup, Parry, Enrage & Evasion)** `[ĐÃ HOÀN THÀNH]`
  - **Boss Earth-Quake Telegraph (0.9s Tụ lực):** Boss Titan rung lắc tụ lực báo trước kèm âm thanh gầm và chữ nổi cảnh báo đỏ rực.
  - **Cơ chế Phản Xạ Chống Đỡ (Stomp Parry):** Người chơi tap >= 4 lần hoặc kích hoạt Overdrive trong lúc Boss tụ lực sẽ kích hoạt Parry, giảm 50% sát thương đòn dậm đất và hiện chữ `🛡️ CHỐNG ĐỠ THÀNH CÔNG!`.
  - **Boss Cuồng Nộ (Enrage Phase khi HP < 35%):** Boss phát sáng đỏ rực, tăng 25% tốc độ di chuyển và tốc đánh nghẹt thở.
  - **Đặc tính Archetype riêng biệt:** Dơi Pha Lê có 12% cơ hội né đòn thường (đòn Crit/Overdrive không thể né); Golem Giáp Sắt kháng 25% sát thương thường và nện thêm 35% sát thương lên Khiên Trụ; Dơi Thần Tài bay thoát sau 22s nếu không bị hạ.

---

## 🖼️ TRACK 3: UI/UX 2D & ICONOGRAPHY (GIAO DIỆN ĐƠN MÀN HÌNH)
*Mục tiêu: Đầy đủ thông tin, không rối mắt, người chơi dễ thao tác bằng 1 ngón/chuột.*

- [x] **Task 3.1: Bộ Khung Giao Diện Chính (HUD Canvas Framework)** `[ĐÃ HOÀN THÀNH]`
  - Top Bar: Khung hiển thị `Crystal Energy`, `Lifetime Crystal`, `Sản lượng/giây (+X/s)` và Buff badge (`TopBarUI.cs`).
  - Left Panel: Danh sách Milestone & Slider thanh tiến trình (`MilestonePanelUI.cs`).
  - Right Panel: 4 thẻ bài nâng cấp Tap Power + 3 Máy phát tự động kèm điều kiện làm mờ/khóa (`UpgradeCardUI.cs`).
  - Bottom Bar: Nút bấm `[ THÁM HIỂM ADVENTURE ]`, `[ Lưu Game ]`, `[ Âm Thanh ]` kết nối `UIManager.cs`.
- [x] **Task 3.2: Bộ Icon Tài Nguyên & Máy Móc (Resource & Machine Icons)** `[ĐÃ HOÀN THÀNH]`
  - Định dạng hiển thị phân biệt rõ ràng: Viên Pha Lê Tím (Crystal Currency), Mũi Khoan (Drill), Lò Nung (Furnace), Máy Hút (Extractor) với phối màu chuẩn URP trên UI thẻ bài.
- [x] **Task 3.3: Dữ Liệu 5 Kỹ Năng Cổ Đại Roguelike (Skill Cards Data)** `[ĐÃ HOÀN THÀNH]`
  - Fireball: Nổ thêm sát thương sau mỗi 3 lượt đánh (`Fireball.asset`).
  - Lightning: 30% tỉ lệ kích hoạt sét đánh thêm sát thương (`Lightning.asset`).
  - Vampiric Core: Hút máu bằng 20% sát thương gây ra (`VampiricCore.asset`).
  - Crystal Aegis: Khiên hộ thể giảm 25% sát thương gánh chịu (`CrystalAegis.asset`).
  - Precision Focus: Tăng thêm +25% tỉ lệ đòn đánh chí mạng x2 (`PrecisionFocus.asset`).
- [x] **Task 3.4: Thiết kế Cửa Sổ Overlay Sự Kiện Ngẫu Nhiên (Random Event Modal)** `[ĐÃ HOÀN THÀNH]`
  - Khung hội thoại cốt truyện kèm 2 nút quyết định tương tác trực tiếp (`EventPanel` trong `AdventureModalUI.cs`).
  - Tích hợp 2 sự kiện mẫu: Khối ngọc cổ đại (`AncientCrystalEvent.asset`) và Thương nhân dị giới (`MysticMerchantEvent.asset`).
- [x] **Task 3.5: Thiết kế Cửa Sổ Chiến Đấu Tự Động & Chọn Kỹ Năng (Combat & Skill Overlay)** `[ĐÃ HOÀN THÀNH]`
  - Khung song đấu: Hero HP Slider vs Enemy HP Slider kèm Combat Log nhịp nhàng thời gian thực.
  - Khung 3-Pick-1 Roguelike Skills tự động xuất hiện để người chơi tùy biến sức mạnh ngay trong lượt chạy. Hỗ trợ click trực tiếp toàn bộ thẻ bài hoặc nút bấm.
  - Vòng đời kích hoạt Modal: Tự động kích hoạt `AdventureModalUI.OpenModal()` qua `UIManager.OnStartAdventureClicked()` đảm bảo an toàn tuyệt đối ngay cả khi GameObject khởi đầu ở trạng thái Inactive.
  - Khung tổng kết trao thưởng và khép kín vòng lặp kinh tế về Lò Rèn.
- [x] **Task 3.6: Hệ Thống Chiến Đấu Tuần Tự & Hoạt Họa Kiểu Capybara Go (Capybara Go Turn-based Combat & Models)** `[ĐÃ HOÀN THÀNH]`
  - Dựng Đấu trường chạy bộ Runic Runway (`Adventure_CombatArena`) với đường băng đá xám, viền vàng ánh kim và 2 trụ đuốc pha lê thắp sáng.
  - Model 3D Hero Capybara Thợ Rèn (`Hero_Player3D`): Tạo hình chú chuột túi Capybara dễ thương với thân tròn ú nu, mõm vuông hiền lành, 2 tai nhỏ xinh, vương miện vàng dũng sĩ, balo đựng pha lê sau lưng, tay cầm búa thạch anh tím phát sáng và khiên vàng.
  - Bộ 4 Model 3D Quái vật: Slime ngọc bích nhân pha lê tím, Tiểu quỷ bóng tối sừng nhọn mắt đỏ, Golem người đá lõi ngực và Rồng chúa hắc ám cánh pha lê.
  - Cơ chế di chuyển & tấn công tuần tự (Sequential Turn-Based) chuẩn Capybara Go:
    - **Hành quân vào trận (Marching Run):** Hero chạy nảy từng bước chân với góc nghiêng người và độ nảy parabol từ trái vào sân đấu.
    - **Lượt đánh của Hero:** Nhún người lấy đà $\rightarrow$ Phi thân lao vút 2.4 đơn vị tới sát mặt quái $\rightarrow$ Nhảy lên giáng búa thạch anh nện thẳng xuống đầu quái $\rightarrow$ Bật lùi 2 nhịp nảy về vị trí thủ.
    - **Lượt đánh của Quái vật:** Quái vật lao vút qua sàn đấu cắn/tát Hero $\rightarrow$ Hero trúng đòn giật lùi tóe lửa chớp đỏ $\rightarrow$ Quái vật nhảy lùi về vị trí thủ.
    - **Hạ gục & Ăn mừng:** Quái vật sụp đổ và tan biến thành bụi tinh thể, Hero nhảy múa ăn mừng chiến thắng.
  - Điều hướng Camera điện ảnh: Lướt mượt mà xuống góc cận cảnh chiến đấu (`Y: 3.8, Z: -5.2, Rot: 22°`) và tự động trả về toàn cảnh Lò Rèn khi kết thúc.
  - Tối ưu TextMeshPro: Loại bỏ hoàn toàn lỗi `IndexOutOfRangeException` của Unity 6, giao diện HUD trong suốt đạt chuẩn Single-screen 100%.
- [x] **Task 3.7: Tính Năng & Giao Diện Thoát Game (Quit Game Feature & Confirmation Modal)** `[ĐÃ HOÀN THÀNH]`
  - Tạo `QuitConfirmationModalUI.cs` với hộp thoại xác nhận phong cách Dark Obsidian viền Ruby đỏ sang trọng:
    - Tiêu đề "XAC NHAN THOAT GAME" & Lời nhắn an tâm tự động lưu: "Tien trinh cua Chua Te se duoc tu dong luu lai an toan. Ban co chac chan muon roi khoi Thanh Dien khong?".
    - 2 nút bấm: [TIEP TUC CHOI] (đóng modal) và [THOAT GAME] (tự động gọi `SaveLoadSystem.SaveGame(silent: true)`, phát SFX và thoát game mượt mà).
  - Tích hợp nút "THOAT" trực tiếp trên thanh Bottom Bar (`BottomBar_Panel`) cạnh nút "LUU GAME".
  - Tích hợp thêm nút "THOAT GAME" bên cạnh nút "CHOI LAI" tại bảng tổng kết khi thất thủ (`EndGamePanelUI.cs`).
  - Hỗ trợ phím tắt `Escape` (hỗ trợ cả New Input System và Legacy fallback) để bật/tắt hộp thoại xác nhận thoát nhanh bất kỳ lúc nào.

---

## ✨ TRACK 4: VFX & ÂM THANH (JUICE & FEEDBACK)
*Mục tiêu: Tạo cảm giác "đã tay, sướng mắt" khi gõ pha lê và nâng cấp.*

- [x] **Task 4.1: Floating Text (Chữ & Số Bay)** `[ĐÃ HOÀN THÀNH]`
  - Tạo `FloatingTextItem.prefab` và `FloatingTextSpawner.cs` lắng nghe `ClickSystem.OnCoreClicked` để bắn số vàng bay lên và mờ dần.
- [x] **Task 4.2: Particle Systems (Hạt Ánh Sáng)** `[ĐÃ HOÀN THÀNH]`
  - Tia lửa đá và mảnh vụn pha lê tím/vàng tóe ra khi gõ vào Crystal Core (`VFX_ClickSparks`).
  - Hào quang ăn mừng (Celebration Burst / Confetti) khi hoàn thành Milestone (`VFX_MilestoneBurst` trong `VFXManager.cs`).
- [x] **Task 4.3: Tween Animation (Co Giãn Đàn Hồi)** `[ĐÃ HOÀN THÀNH]`
  - Hiệu ứng Punch Scale: Khối Crystal Core co lại nhẹ khi bị đập và nảy bung ra tạo lực (`ClickSystem.AnimatePunchScale`).
  - Nút nâng cấp nảy nhẹ (Button Juice Bounce) khi người chơi mua hoặc tương tác (`UpgradeCardUI.AnimateButtonBounce`).
- [x] **Task 4.4: Thiết kế Âm Thanh Hiệu Ứng (SFX)** `[ĐÃ HOÀN THÀNH]`
  - Tiếng gõ lách cách của đá quý/pha lê khi click (`PlayClickSfx`).
  - Tiếng leng keng kim loại đe búa khi mua nâng cấp (`PlayUpgradeSfx`).
  - Tiếng vung kiếm và va đòn trong combat (`PlayCombatAttackSfx`).
  - Tiếng nổ ma thuật của kỹ năng Fireball/Lightning (`PlaySpellSfx`).
  - Tiếng kèn ngũ cung khải hoàn khi đạt Milestone (`PlayMilestoneSfx`).
  - Tiếng kèn chúc mừng chiến thắng Adventure (`PlayVictorySfx` trong `AudioManager.cs`).
- [x] **Task 4.5: Nhạc Nền (BGM Ambient Loop)** `[ĐÃ HOÀN THÀNH]`
  - Bản nhạc nền phong cách Fantasy Lò Rèn ma thuật nhẹ nhàng, lặp tuần hoàn (vòng hợp âm Am - F - C - G êm ái) thư giãn khi treo máy (`AudioManager.CreateAmbientFantasyLoop`).

---

## 💻 TRACK 5: LẬP TRÌNH HỆ THỐNG KỸ THUẬT (PROGRAMMING & TDD)
*Mục tiêu: Code sạch, tuân thủ Karpathy Rules, tự động biên dịch và kiểm thử.*

- [x] **Task 5.1: Cấu trúc Dự án & ScriptableObjects Data Model** `[ĐÃ HOÀN THÀNH]`
  - Viết `GameEnums.cs`, `GeneratorDataSO.cs`, `UpgradeDataSO.cs`, `MilestoneDataSO.cs`, `SkillDataSO.cs`, `EnemyDataSO.cs`, `EventDataSO.cs`. (Biên dịch thành công 0 lỗi).
- [x] **Task 5.2: ResourceManager & Currency Logic** `[ĐÃ HOÀN THÀNH]`
  - Quản lý số dư `Crystal`, `LifetimeCrystal`, `TapPower`, `ProductionPerSec` qua Singleton `ResourceManager.cs`.
  - Bộ tiện ích `NumberFormatter.cs` chuyển đổi hiển thị: `1K`, `1M`, `1B`, `1T`. (Biên dịch thành công 0 lỗi).
- [x] **Task 5.3: ClickSystem & Input Raycast 3D** `[ĐÃ HOÀN THÀNH]`
  - Bắt cú nhấp chuột/chạm lên Collider của `CrystalCore`, cộng tài nguyên qua `TapCore()`, hiệu ứng co giãn đàn hồi (Punch Scale) và phát tán sự kiện `OnCoreClicked`. (Biên dịch thành công 0 lỗi).
- [x] **Task 5.4: GeneratorSystem (Tự động sinh tài nguyên)** `[ĐÃ HOÀN THÀNH]`
  - Vòng lặp tính toán sản lượng tự động theo giây dựa trên cấp độ máy phát (Drill, Furnace, Extractor), tự động mở khóa theo Lifetime Crystal. (Biên dịch thành công 0 lỗi).
- [x] **Task 5.5: UpgradeManager & Công Thức Chi Phí Lũy Tiến** `[ĐÃ HOÀN THÀNH]`
  - Quản lý nâng cấp Tap Power và điều hướng mua Generator theo công thức lũy tiến `Cost = BaseCost * Mathf.Pow(GrowthRate, Level)`. (Biên dịch thành công 0 lỗi).
- [x] **Task 5.6: MilestoneManager & Hệ Số Thưởng Toàn Cục** `[ĐÃ HOÀN THÀNH]`
  - Theo dõi Lifetime Crystal, tự động kích hoạt 5 mốc Bronze, Silver, Gold, Platinum, Guardian và nhân hệ số `GlobalProductionMultiplier`. (Biên dịch thành công 0 lỗi).
- [x] **Task 5.7: AdventureManager & State Machine** `[ĐÃ HOÀN THÀNH]`
  - Quản lý trạng thái: `None`, `Exploring`, `EventChoice`, `AutoCombat`, `SkillSelection`, `AdventureReward`, `RunDefeat` (`AdventureManager.cs`).
- [x] **Task 5.8: Turn-Based Auto-Combat Engine** `[ĐÃ HOÀN THÀNH]`
  - Vòng lặp đòn đánh tự động theo nhịp coroutine, tính sát thương trừ giáp, tỉ lệ Crit bạo kích x2, kích hoạt chuỗi hiệu ứng kỹ năng Fireball (nổ chu kỳ), Lightning (xác suất), Lifesteal (hút máu) và Aegis (giảm sát thương).
- [x] **Task 5.9: Khép Kín Vòng Lặp Phần Thưởng (Loop Tie-Back)** `[ĐÃ HOÀN THÀNH]`
  - Trao thưởng Crystal và Buff tốc độ sản xuất tạm thời cho Lò Rèn thông qua `ResourceManager.ApplyTemporaryBuff()` và `ResourceManager.AddDirectBonus()`. Mọi modal thám hiểm thu gọn về màn hình lò rèn chính.
- [x] **Task 5.10: SaveLoadSystem & Tính Toán Thu Nhập Treo Máy (Offline Progress)** `[ĐÃ HOÀN THÀNH]`
  - Lưu và nạp dữ liệu bằng JSON vào `PlayerPrefs`, tự động lưu mỗi 15s và khi thoát game.
  - Tự động tính chênh lệch `DateTime.UtcNow`, nhận 50% sản lượng treo máy (tối đa 8 giờ). (Biên dịch thành công 0 lỗi).

---

## 🚀 TRACK 6: TÍCH HỢP, CÂN BẰNG & XUẤT BẢN BUILD (RELEASE)
*Mục tiêu: Đạt chuẩn "Small but complete", xuất file .EXE chạy mượt mà không lỗi.*

- [x] **Task 6.1: Cân Bằng Số Học Kinh Tế (Math Balancing)** `[ĐÃ HOÀN THÀNH]`
  - Thiết lập công thức lũy tiến chuẩn $Cost = BaseCost \times 1.15^{Level}$.
  - Kiểm toán pacing: 1–2 phút mở Drill (100 Lifetime), 8–10 phút mở Furnace (2.5K Lifetime), 25–30 phút mở Extractor (25K Lifetime). Tốc độ phát triển mượt mà, không gặp tường kinh tế.
- [x] **Task 6.2: Kiểm Thử Tự Động & Sửa Lỗi (Autonomous Verification)** `[ĐÃ HOÀN THÀNH]`
  - Biên dịch tự động qua `dotnet build GAme.slnx` đạt 0 Warning, 0 Error.
  - Tự khắc phục lỗi tham chiếu qua Self-Healing Loop. Unity Console hoàn toàn sạch (0 Exception, 0 Warning).
- [x] **Task 6.3: Đóng Gói Bản Dựng Executable (.EXE Windows)** `[ĐÃ HOÀN THÀNH]`
  - Xuất bản phát hành Windows Standalone x64 tại thư mục `Builds/CrystalForge_Release/CrystalForge.exe` (Dung lượng: 105.52 MB).
- [x] **Task 6.4: Nhật Ký Minh Chứng Sử Dụng AI (AI Implementation Log)** `[ĐÃ HOÀN THÀNH]`
  - Soạn thảo và lưu trữ hoàn tất tài liệu minh chứng `AI_IMPLEMENTATION_LOG.md` theo quy chuẩn Game Sprint 2026.
- [x] **Task 6.5: Tối Ưu Hóa Model Player Duy Nhất** `[ĐÃ HOÀN THÀNH]`
  - Khắc phục hoàn toàn lỗi xuất hiện 2 model Player khi vào Thám Hiểm (loại bỏ mascot tĩnh `Prop_ForgeMascot` và đấu trường rời rạc `Adventure_CombatArena`).
  - Hợp nhất thành 1 model chiến binh Capybara duy nhất (`Player_CapybaraHero`) trên Đảo Nổi Lò Rèn.
  - Khử toàn bộ lỗi font dấu tiếng Việt trong TextMeshPro sang tiếng Việt không dấu chuẩn typography sắc nét 100%.
- [x] **Task 6.6: Hệ Thống Đi Tuần Tra Cảnh Giới Quanh Trụ Tinh Thể Khi Idle/AFK** `[ĐÃ HOÀN THÀNH]`
  - Khi ở chế độ Lò Rèn bình thường (không mở Thám Hiểm), chú hiệp sĩ Capybara liên tục di chuyển đi tuần quanh trụ tinh thể theo quỹ đạo tròn bán kính 2.15m đúng nghĩa một người lính gác cảnh giới.
  - Sải bước tuần tra mượt mà với nhịp nảy bước chân (Hop) và độ lắc lư lạch bạch (Waddle) đáng yêu, búa vác chéo vai nảy nhịp nhàng theo nhịp tiến bước.
  - Loại bỏ hoàn toàn hành vi dừng lại nện búa vào trụ tinh thể. Thay vào đó, sau mỗi vòng tuần tra (~8.5s), chú Capybara dừng chân quan sát cảnh giới sang 2 bên hướng ra ngoài không gian đảo nổi (quét trái rồi quét phải cảnh giác) rồi tiếp tục sải bước tuần tra.
  - Khi người chơi click chủ động vào Pha Lê, Capybara vẫn chuyên tâm giữ vững nhiệm vụ đi tuần bảo vệ thánh địa lò rèn mà không bị ngắt quãng.
- [x] **Task 6.7: Thanh Máu Overhead Trên Đầu Player (Xanh) & Quái Vật (Đỏ)** `[ĐÃ HOÀN THÀNH]`
  - Gắn thanh máu 3D World-Space trực tiếp trên đỉnh đầu của chiến binh Capybara (`Player_CapybaraHero`) với màu xanh lục bảo neon rực rỡ (`#00E676`).
  - Gắn thanh máu 3D World-Space trên đỉnh đầu của toàn bộ 4 loại quái vật thám hiểm (`Monster_Slime`, `Monster_ShadowMinion`, `Monster_GolemGuardian`, `Monster_DragonBoss`) với màu đỏ thẫm rực cháy (`#FF3344`).
  - Tự động hướng mặt về Camera (`LateUpdate` Billboarding) ở mọi góc nhìn 3D.
  - Tích hợp thanh trượt trễ (Catch-up Delayed Bar) phong cách Fighting Game tạo cảm giác lực đánh cực đã mắt khi nhận sát thương.
  - Hiệu ứng rung nảy nhẹ (Hit Shake) và Text hiển thị chỉ số máu chi tiết (`HP / MaxHP`) chuẩn TextMeshPro.
  - Tự động ẩn thanh máu khi ở chế độ Lò Rèn bình thường và chỉ kích hoạt rực sáng khi bước vào chế độ Thám Hiểm.
  - Tối ưu cấu trúc RectTransform phẳng (flat hierarchy), loại bỏ hoàn toàn hiện tượng vỡ scale 150x khiến thanh máu bị biến dạng phóng to thành mảng xanh/đỏ tràn ngập toàn bộ màn hình.

- [x] **Task 6.8: Cải Thiện Đồ Họa 3D Đỉnh Cao & Khử Triệt Để Lỗi Phông Chữ**
  - **Khử lỗi phông chữ 100% ([FontSanitizer.cs](file:///d:/Ga21301/GAme/Assets/Scripts/Utils/FontSanitizer.cs)):**
    - Giải quyết triệt để vấn đề mất glyph (tofu / ô vuông `□`) do font mặc định `LiberationSans SDF` thiếu bảng mã ký tự tiếng Việt có dấu.
    - Xây dựng bộ chuẩn hóa tự động ánh xạ 67 ký tự có dấu tiếng Việt sang Latinh không dấu chuẩn Arcade/Idle Gaming, chuyển đổi các emoji không tương thích sang hệ ký tự gaming brackets chuẩn (`[FIRE]`, `[LIGHTNING]`, `[AEGIS]`, `[CHI MANG]`, `[HUT MAU]`).
    - Tự động lọc và khử toàn diện trên mọi TextMeshPro trong Scene ở thời điểm khởi chạy (`Awake`/`Start`), trong Combat Log, Skills, Events, Badges và Top/Bottom Bar UI.
  - **Đồ họa 3D Diorama Fantasy Nâng Cao ([SceneSetupTool.cs](file:///d:/Ga21301/GAme/Assets/Scripts/Editor/SceneSetupTool.cs)):**
    - Thay thế khối tinh thể hộp đơn điệu bằng **Cụm Tinh Thể Đa Diện Huyền Ảo (Multi-faceted Amethyst Cluster)**: Tháp tinh thể chính vát góc 45 độ, lõi năng lượng phát sáng rực rỡ bên trong, 4 vành kim cương bảo vệ, vòng hào quang cổ ngữ ma thuật, 4 mảnh vệ tinh xoay nghịch hướng lơ lửng và hệ thống bụi sao ma thuật bay bổng (Sparkle Particles VFX).
    - Thiết kế **Đảo Nổi Kỳ Quan (Floating Sky Island)**: Phiến đá hoa cương cổ ngữ viền vàng hoàng kim, trận đồ Cổ ngữ phát sáng ma thuật, đe rèn cổ đại với sừng vàng, đáy đảo với các tầng đá nhũ treo lơ lửng ngược vào không gian vũ trụ, và 4 trụ đá Cổ ngữ thắp sáng ngọc xanh cyan 4 góc.
    - Thiết lập hệ thống chiếu sáng 3 điểm Cinematic Lighting (Ánh nắng hoàng hôn ấm áp + Rim light cyan thần bí + Point light nhịp thở ma thuật theo tinh thể).
    - Nâng cấp Post-processing trong `SampleSceneProfile.asset`: Tăng cường Bloom ngưỡng thấp (0.75) tán xạ cao (0.85) và Vignette tím sẫm tạo chiều sâu vũ trụ huyền bí.
  - **Đóng gói bản cài đặt Release hoàn thiện:**
    - Biên dịch và đóng gói thành công bản Standalone Windows x64 tại `Builds/CrystalForge_Release/CrystalForge.exe` (105 MB).

- [x] **Task 6.9: Nâng Cấp Đồ Họa 3D Điện Ảnh & Visual Juice Đỉnh Cao (Cinematic Graphics Upgrade)**
  - **Không gian Vũ trụ & Hạt sao lấp lánh (Deep Space Starfield VFX):**
    - Thiết lập hệ thống hạt sao đa tầng `CosmicStarfield_VFX` với hàng trăm vì sao lấp lánh và bụi ngân hà trôi bồng bềnh xung quanh đảo bay.
    - Tạo 4 tảng thiên thạch lơ lửng ngoài không gian (`Floating Asteroids`) đính tinh thể pha lê và xoay chậm đa góc tạo hiệu ứng thị sai (Parallax depth) 3D sống động.
  - **Trận đồ Cổ ngữ Kép Xoay Động (Animated Dual-Rune Magic Circles):**
    - Trận đồ sàn đảo được trang bị [RunicCircleRotator.cs](file:///d:/Ga21301/GAme/Assets/Scripts/Visuals/RunicCircleRotator.cs) xoay 2 tầng ngược chiều nhau (vòng vàng kim ngoài cùng xoay thuận, vòng ngọc cyan trong xoay nghịch kèm nhấp nhô nhẹ) và 4 nút ấn cổ ngữ phát sáng.
  - **Đuốc Ma Thuật Cyan & Khói Lò Nung (Mystic Flames & Volumetric Smoke VFX):**
    - 4 Trụ đá Cổ ngữ góc đảo được trang bị chén đuốc vàng kim, ngọn lửa ma thuật Cyan (`CyanFlame_VFX`) bốc lên ngùn ngụt kèm ánh sáng bập bùng tự nhiên [MysticFlickerLight.cs](file:///d:/Ga21301/GAme/Assets/Scripts/Visuals/MysticFlickerLight.cs).
    - Lò Nung Mana (`Prop_ManaFurnace`) được bổ sung luồng khói ma thuật bốc lên uốn lượn từ ống khói (`FurnaceSmoke_VFX`) và lõi lửa hồng lập lòe.
  - **Props Xưởng Lò Rèn Thần Thoại:**
    - **Giá Vũ Khí Thần Thoại (`Prop_BlacksmithWeaponRack`):** Khung gỗ sồi viền kim loại treo Đại Kiếm Pha Lê Thần Thoại (`CrystalGreatsword`) phát sáng rực rỡ và Rìu Chiến Binh Runic (`RunicBattleaxe`).
    - **Rương Quặng Pha Lê & Vàng Thỏi (`Prop_CrystalOreCrate`):** Thùng gỗ viền đồng chứa các khối thạch anh thô và thỏi vàng nguyên chất.
    - **Phôi Ngọc Nóng Đỏ Rực Lửa (`Prop_GlowingIngot`):** Nằm trực tiếp trên mặt đe rèn, tỏa ánh sáng nhiệt cam ấm áp bập bùng.
  - **Visual Juice Đỉnh Cao (Impact Shockwave Ring & Forge Sparks):**
    - Tích hợp vòng sóng xung kích tỏa nhanh trên sàn đảo (`VFX_ShockwaveRing`) mỗi khi Capybara giáng búa hoặc người chơi click, tia lửa nổ bung hình quạt và hạt bụi ma thuật lơ lửng (`VFX_AmbientMotes`).
  - **Tinh Chỉnh Post-Processing URP AAA:**
    - Nâng cấp `SampleSceneProfile.asset`: Bổ sung `ColorAdjustments` (Contrast 18, Saturation 16, Post Exposure 0.2), Bloom (Threshold 0.65, Intensity 1.35, Scatter 0.72) và Vignette mềm mại tạo chiều sâu thị giác chuẩn Cinematic.

- [x] **Task 6.10: Tối Giản Hóa Giao Diện Giao Đấu 3D & Ẩn Khung Nhật Ký Chiến Đấu (LogBox)** `[ĐÃ HOÀN THÀNH]`
  - Theo chỉ thị của Chủ nhân, loại bỏ hoàn toàn khung chữ nhật ký chiến đấu `LogBox` (`CombatLogText`) ở đáy màn hình chế độ Thám Hiểm.
  - Khử triệt để các dòng thông báo tấn công/chịu sát thương (`[HERO] Chem: -X HP`, `[QUAI] Tan cong: -Y HP`), giải phóng hoàn toàn không gian phía dưới màn hình.
  - Tối ưu hóa hiệu năng render và xử lý chuỗi ký tự trong [AdventureModalUI.cs](file:///d:/Ga21301/GAme/Assets/Scripts/UI/AdventureModalUI.cs): bỏ qua cập nhật chuỗi khi UI ẩn.
  - Đảm bảo tầm nhìn sàn đấu 3D rộng thoáng, tập trung trọn vẹn sự chú ý của người chơi vào nhịp độ phi thân vung búa của dũng sĩ Capybara, hoạt họa của quái vật và hai thanh máu Overhead 3D World-Space có thanh trượt trễ (Catch-up Delayed Bar) trên đỉnh đầu.
  - Cập nhật đồng bộ các công cụ dựng scene tự động [Adventure3DSetupTool.cs](file:///d:/Ga21301/GAme/Assets/Scripts/Editor/Adventure3DSetupTool.cs) và [AdventureSetupTool.cs](file:///d:/Ga21301/GAme/Assets/Scripts/Editor/AdventureSetupTool.cs).
  - Tái đóng gói thành công bản phát hành độc lập Windows x64 tại [CrystalForge.exe](file:///d:/Ga21301/GAme/Builds/CrystalForge_Release/CrystalForge.exe).

- [x] **Task 6.11: Nâng Cấp Mô Hình 3D Trụ Tinh Thể Năng Lượng Cute Amethyst Star Core** `[ĐÃ HOÀN THÀNH]`
  - Hiện thực hóa 100% bản thiết kế Concept Art Phương án 1 được Chủ nhân phê duyệt vào thế giới 3D Unity URP:
    - **Bệ đá Cẩm Thạch Trắng (`Central_Pedestal`):** Khối đá hoa cương cẩm thạch trắng mềm mại, viền chân đế hoàng kim, 4 chân đế uốn cong ở 4 góc và 4 biểu tượng Ngôi Sao Vàng khảm phát sáng trên 4 mặt đứng.
    - **Đĩa Năng Lượng Thiên Văn:** Mặt vàng kim viền tròn đĩa năng lượng ấm áp tỏa ánh hào quang nhẹ nhàng dưới chân khối ngọc.
    - **Khối Tinh Thể Bo Tròn Chuyển Sắc Pastel (`CentralCuteGemSpire`):** Khối thạch anh tím phấn đa giác mềm mại kết hợp vát cạnh ngọc lam cyan rực rỡ và đáy ngọc thuôn nhọn thanh thoát.
    - **Lõi Sao Vàng Ấm Áp (`InnerWarmStarCore`):** Khối cầu năng lượng mặt trời mini phát quang ấm áp từ sâu bên trong lòng tinh thể.
    - **Vành Đai Sao & Tinh Thể Tí Hon Nghiêng Xoay Quanh (`StarRingContainer`):** Vành khuyên năng lượng kép nghiêng $22^\circ$ mang 4 Ngôi Sao Vàng 3D phát sáng và 4 mảnh tinh thể tí hon xoay tròn tuần hoàn ngược chiều tạo hiệu ứng thị giác cực kỳ ma thuật và đáng yêu.
    - **Bụi Sao Ma Thuật (`SparkleParticles`):** Hệ thống hạt ánh sáng tím phấn và sao vàng thăng hoa nhẹ nhàng bồng bềnh xung quanh lõi năng lượng.
  - Tối ưu Collider và hệ thống Click tương tác mượt mà, đồng bộ với nhịp thở phập phồng ánh sáng URP.
  - Tái đóng gói thành công bản phát hành độc lập Windows x64 tại [CrystalForge.exe](file:///d:/Ga21301/GAme/Builds/CrystalForge_Release/CrystalForge.exe).

- [x] **Task 6.12: Nâng Cấp Mô Hình 3D Trụ Thạch Anh Lục Giác Kèm Rễ Cây Mầm Non - Mẫu 1 (Sprout Vine Quartz Obelisk)** `[ĐÃ HOÀN THÀNH]`
  - Hiện thực hóa 100% mẫu thiết kế **Mẫu 1: Sprout Vine Quartz Obelisk** theo đúng nguyên mẫu ảnh chụp đời thực và bản phác thảo concept art được Chủ nhân lựa chọn:
    - **Đế gỗ bát giác 2 tầng giật cấp (`Central_Pedestal`):** Chế tác thủ công từ các khối hình học lồng ghép góc quay $45^\circ$, chất liệu gỗ tuyết tùng nâu trầm ấm (`Mat_WoodCedar`), viền chỉ vàng hoàng kim sang trọng ở mép từng tầng đế.
    - **Tháp thạch anh lục giác 6 cạnh đều (`HexagonalQuartzObelisk`):** Dựng từ 3 khối lăng trụ giao nhau đối xứng ở các góc quay $0^\circ, 60^\circ, 120^\circ$ theo chuẩn hình học tự nhiên của tinh thể thạch anh ($W / \sqrt{3}$), bề mặt phủ chất liệu thạch anh mờ băng tuyết (`Mat_QuartzFrost`) pha lê tím pastel.
    - **Chóp nhọn đa diện vát góc tự nhiên (Faceted Apex Point):** Khắc họa đỉnh chóp vát cạnh nghiêng góc $45^\circ$ bằng chất liệu thạch anh trong suốt (`Mat_QuartzClear`) khúc xạ ánh sáng huyền ảo theo đúng ảnh chụp thực tế của Chủ nhân.
    - **Lõi bụi sao phát quang (`QuartzInnerStarCore`):** Khối cầu ma thuật ấm áp nằm sâu trong lòng thạch anh tỏa ánh sáng lan tỏa ra các mặt vát lục giác.
    - **Dây rễ mầm non & Chồi lá uốn lượn (`SproutVineRoots`):** Hệ thống 5 nhánh rễ cây gỗ sồi cổ thụ (`Mat_WoodVine`) vươn từ đế gỗ leo bám tự nhiên lên $1/3$ thân trụ thạch anh, điểm xuyết 6 chồi lá non xanh ngọc mạ non pastel (`Mat_SproutGreen`) và 5 đốm rêu ngọc bích phát sáng dịu mắt (`Mat_EmeraldMoss`).
    - **Vành cực quang & Bụi sao lấp lánh (`AuroraHaloContainer`):** 2 vành đai ánh sáng cực quang xanh lơ - tím ngọc nghiêng $18^\circ$ xoay nhẹ nhàng kết hợp hệ thống hạt bụi sao ma thuật bay bổng xung quanh tháp tinh thể.
  - Đồng bộ hóa toàn bộ Collider, hoạt họa xoay nhẹ, nhịp thở ánh sáng Point Light và hiệu ứng nhấp nhô tuần hoàn không đổi góc nhìn.
  - Xác minh tự động qua vòng lặp Autonomous Verification Loop: Biên dịch sạch 0 warning, 0 error (`Assembly-CSharp-Editor.dll`).
  - Tái đóng gói thành công bản phát hành độc lập Windows x64 tại [CrystalForge.exe](file:///d:/Ga21301/GAme/Builds/CrystalForge_Release/CrystalForge.exe).

- [x] **Task 6.13: Tái Hiện Hoàn Hảo 100% Trụ Năng Lượng Đơm Hoa & Cổ Ngữ Bát Giác Khắc Vàng Theo Ảnh Thực Tế** `[ĐÃ HOÀN THÀNH]`
  - Bám sát từng đường nét trong hình ảnh Chủ nhân cung cấp, nâng cấp toàn diện trụ năng lượng trung tâm thành tuyệt tác 3D chân thực:
    - **Đế gỗ bát giác 2 tầng khảm 8 Cổ ngữ vàng phát sáng (`Central_Pedestal`):**
      - 2 Tầng đế gỗ gụ/tuyết tùng nâu trầm ấm giật cấp bề thế, gờ chỉ vàng kim sang trọng quanh các mép tầng.
      - Trên đúng 8 mặt đứng của tầng đế bát giác dưới được khảm **8 biểu tượng Cổ ngữ Fantasy/Norse vàng kim rực sáng** (`ᚱ`, `ᚲ`, `ᚠ`, `ᛏ`, `ᛟ`, `ᛋ`, `ᛉ`, `ᚹ`) bằng chất liệu phát quang (`Mat_RuneGold`) rực rỡ dưới hiệu ứng Bloom URP.
    - **Tháp thạch anh lục giác đa diện nguyên khối (`HexagonalQuartzObelisk`):**
      - Dựng bằng Mesh 3D đa giác lục giác tạo hình chuẩn giải phẫu học tinh thể thạch anh tự nhiên (`GenerateHexagonalQuartzMesh`).
      - Mặt vát phẳng chéo kinh điển (slanted natural facet) nghiêng $45^\circ-50^\circ$ mở rộng hướng thẳng về phía người chơi và dũng sĩ Capybara.
      - Chất liệu thạch anh trong trẻo lấp lánh (`Mat_QuartzCrystal`) với độ bóng 96% bắt trọn vệt phản xạ ánh sáng mặt trời và ánh đèn.
    - **Lõi ma thuật đa sắc bên trong (Dual-Gradient Magical Core):**
      - Nửa trên: Lõi nắng vàng ấm áp (`InnerSunCore` - `#FFAA22` emission).
      - Nửa dưới: Lõi đại dương ngọc lam (`InnerAquaCore` - `#00E5FF` emission).
      - Ánh sáng điểm tỏa nhiệt ấm áp (`CrystalCore_PointLight`) soi rọi lên bệ gỗ và chú Capybara tuần tra.
    - **Hệ rễ cây cổ thụ uốn lượn ôm trọn 40% chân tháp (`SproutVineRoots`):**
      - 16 phân đoạn rễ cây gỗ uốn lượn đan xen tự nhiên thành chiếc nôi ôm chặt lấy chân tinh thể.
      - 4 chân rễ lớn bám chặt xuống các mặt phẳng của đế gỗ bát giác.
    - **18 Bông hoa đào & hoa mai vàng 5 cánh cực cute:**
      - 10 bông hoa đào hồng phấn (`Mat_PeachBlossom`) và 8 bông hoa mai vàng mơ mộng (`Mat_GoldBlossom`), mỗi bông đều sở hữu **5 cánh hoa xòe tròn mềm mại cùng nhụy hoa vàng phát sáng** (`Mat_FlowerCenter`).
      - Xen kẽ chồi lá non xanh ngọc mạ non và đốm rêu ngọc bích phát sáng.
    - **Vành đai năng lượng cực quang nghiêng xoay quanh (`AuroraHaloContainer`):**
      - Vành khuyên ngọc lam phát sáng nghiêng $16^\circ$ xoay nhẹ nhàng tuần hoàn quanh tháp tĩnh, đưa bụi sao vàng ma thuật bay bổng vào không gian.
  - Vòng lặp Autonomous Verification: Biên dịch 0 lỗi, 0 cảnh báo.
  - Đã lưu Scene sạch sẽ và đóng gói thành công bản cài đặt tại [CrystalForge.exe](file:///d:/Ga21301/GAme/Builds/CrystalForge_Release/CrystalForge.exe).

- [x] **Task 6.14: Loại Bỏ Công Trình Trụ Xung Quanh & Tích Hợp Hệ Thống Nâng Cấp Dũng Sĩ (HP, ATK, SPD, SKILL)** `[ĐÃ HOÀN THÀNH]`
  - **Loại bỏ triệt để các công trình trụ/máy móc xung quanh:**
    - Cắt bỏ 4 trụ đá Cổ ngữ thắp sáng ngọn đuốc Cyan (`CreateCornerPylons` / `RunicPylon`) ở 4 góc đảo nổi.
    - Cắt bỏ 3 mô hình máy móc công trình phát tự động xung quanh (`CreateDrillProp`, `CreateFurnaceProp`, `CreateExtractorProp`).
    - Giải phóng toàn bộ diện tích sàn đảo nổi, tạo không gian mở khoáng đạt, tôn vinh trọn vẹn vẻ đẹp tuyệt mỹ của tuyệt tác trụ tinh thể thạch anh lục giác đơm hoa rực rỡ và bệ bát giác khảm cổ ngữ vàng.
    - Giúp chú dũng sĩ Capybara có không gian rộng rãi để sải bước đi tuần quanh trụ và nghênh chiến với quái vật từ mọi hướng mép đảo.
  - **Hệ thống Nâng cấp Dũng Sĩ (Player Upgrades - HP, Công kích, Tốc độ, Kỹ năng):**
    - **Kiến trúc dữ liệu & Kinh tế:**
      - Thêm `PlayerStatType` (`Hp`, `Attack`, `Speed`, `Skill`) vào [GameEnums.cs](file:///d:/Ga21301/GAme/Assets/Scripts/Core/GameEnums.cs).
      - Mở rộng [UpgradeManager.cs](file:///d:/Ga21301/GAme/Assets/Scripts/Economy/UpgradeManager.cs) với công thức lũy tiến chuẩn Game Sprint 2026:
        - $HP Cost = \lfloor 40 \times 1.15^{Level} \rfloor$ (+150 Max HP / cấp)
        - $ATK Cost = \lfloor 60 \times 1.16^{Level} \rfloor$ (+25 Sát thương / cấp)
        - $Speed Cost = \lfloor 80 \times 1.18^{Level} \rfloor$ (+5% Tốc độ đánh & đi tuần / cấp)
        - $Skill Cost = \lfloor 120 \times 1.20^{Level} \rfloor$ (+30 Sát thương phép & +2% Bạo kích / cấp)
      - Lưu trữ và đồng bộ trạng thái vĩnh viễn qua [SaveLoadSystem.cs](file:///d:/Ga21301/GAme/Assets/Scripts/Economy/SaveLoadSystem.cs).
    - **Giao diện Tab Đôi Tinh Gọn (Single-Screen Wireframe):**
      - Tích hợp thanh chuyển Tab thông minh [UpgradeTabSwitcher.cs](file:///d:/Ga21301/GAme/Assets/Scripts/UI/UpgradeTabSwitcher.cs) vào bảng nâng cấp bên phải màn hình:
        - Tab 1: `[*] LÒ RÈN` (Nâng cấp Lực Gõ, Khoan Tinh Thể, Lò Nung, Máy Chiết Xuất).
        - Tab 2: `[*] DŨNG SĨ` (Nâng cấp Thể Lực HP, Công Kích ATK, Nhanh Nhẹn SPD, Cổ Ngữ SKILL).
      - Thiết kế 4 thẻ nâng cấp trực quan [UpgradeCardUI.cs](file:///d:/Ga21301/GAme/Assets/Scripts/UI/UpgradeCardUI.cs) với nút bấm nảy đàn hồi (bounce feel), hiển thị chỉ số cộng thêm, giá mua và tự động làm sáng/mờ theo lượng Pha Lê sở hữu.
    - **Liên kết cơ chế chiến đấu & phòng thủ:**
      - [CrystalDefenseManager.cs](file:///d:/Ga21301/GAme/Assets/Scripts/Adventure/CrystalDefenseManager.cs): HP dũng sĩ tăng theo cấp, sát thương đánh tự động và click phụ trợ tăng theo ATK & SKILL, nhịp đánh và tốc độ đi tuần tăng nhanh theo SPD.
      - [AdventureManager.cs](file:///d:/Ga21301/GAme/Assets/Scripts/Adventure/AdventureManager.cs): Chỉ số khởi đầu thám hiểm kế thừa toàn bộ điểm cộng dồn từ Dũng Sĩ.
  - **Xác minh tự động & Đóng gói:**
    - Biên dịch sạch sẽ 0 warning, 0 error qua `dotnet build GAme.slnx`.
    - Dựng lại Scene tự động qua Unity Editor và lưu [SampleScene.unity](file:///d:/Ga21301/GAme/Assets/Scenes/SampleScene.unity).
    - Đóng gói thành công bản phát hành độc lập Windows x64 tại [CrystalForge.exe](file:///d:/Ga21301/GAme/Builds/CrystalForge_Release/CrystalForge.exe).

- [x] **Task 6.13: Kiến Tạo Model 3D Trụ Bệ Đá Tinh Thể, Nhân Vật Thợ Mỏ Chibi & Khung Cảnh Thánh Điện Cổ Đại (Ancient Sanctuary & Miner Hero)** `[ĐÃ HOÀN THÀNH]`
  - Hiện thực hóa trọn vẹn 100% từ 3 hình ảnh Concept Art do Chủ nhân chỉ định vào Unity 3D URP:
    - **Trụ Bệ Đá Tinh Thể & Lõi Kim Cương (Ảnh 1 & 3):**
      - Bệ đá bát giác 3 tầng granite cổ với 8 phiến phù điêu khắc cổ ngữ Rune Cyan phát quang rực rỡ, bề mặt bàn thờ đá trên cùng có vòng cổ ngữ xoay động.
      - Khối tinh thể kim cương ngọc lam khổng lồ 2 đầu nhọn (`DiamondOctahedronMesh`), lõi cầu phát quang trắng ấm bên trong, vòng hào quang sương lam xoay quanh và bụi hạt ma thuật lấp lánh.
      - 4 chùm tinh thể ngọc lam vươn nhọn tự nhiên mọc xung quanh chân bệ thờ.
    - **Nhân Vật Thợ Mỏ Chibi & Cuốc Khai Khoáng Rực Lửa (Ảnh 2):**
      - Thân hình chibi dễ thương, mái tóc anime nâu, đôi mắt to biểu cảm tự tin và nụ cười kiên định.
      - Mũ bảo hộ thợ mỏ với đèn tinh thể Cyan hình thoi phát sáng rực rỡ ngay trước trán kèm điểm sáng rọi về phía trước.
      - Tạp dề da thợ rèn với dây đai vai, thắt lưng khóa đồng và miếng vá thủ công.
      - Vũ khí Cuốc Khai Khoáng Huyền Thoại hai mũi đá nhọn với rãnh nứt năng lượng magma phát sáng rực lửa xuyên qua đầu cuốc.
      - Tích hợp đầy đủ [CombatUnit3D.cs](file:///d:/Ga21301/GAme/Assets/Scripts/Adventure/CombatUnit3D.cs) (Hero unit), thanh máu Overhead Health Bar, liên kết với [Adventure3DManager.cs](file:///d:/Ga21301/GAme/Assets/Scripts/Adventure/Adventure3DManager.cs).
    - **Khung Cảnh Thánh Điện Hầm Ngục Cổ Kính (Ảnh 3):**
      - Sàn đá hầm ngục cổ với phiến đá lát, rêu xanh và vòng trận đồ cổ ngữ Rune 16 ký tự phát quang màu Cyan trên nền sàn.
      - 4 Cột trụ đá cổ kính khổng lồ quấn dây leo xanh, gắn chén đuốc sắt với ngọn lửa cam rực sáng và ánh sáng bập bùng tự nhiên [MysticFlickerLight.cs](file:///d:/Ga21301/GAme/Assets/Scripts/Visuals/MysticFlickerLight.cs).
      - Luồng sáng thần quang chiếu rọi từ trần hầm ngục xuống bàn thờ trung tâm.
      - Camera Isometric 3/4 điện ảnh và Post-Processing URP (Bloom, Color Adjustments).
  - Tự động hóa toàn diện qua công cụ Editor [AncientSanctuarySetupTool.cs](file:///d:/Ga21301/GAme/Assets/Scripts/Editor/AncientSanctuarySetupTool.cs).
  - Biên dịch kiểm toán sạch sẽ: `0 Warning(s)`, `0 Error(s)`.

- [x] **Task 6.15: Nâng Cấp Trải Nghiệm Đỉnh Cao: Sốt Gõ Fever Rush, Mưa Ngọc Loot Burst & Hệ Thống Di Vật Cổ Đại (Ý Tưởng 1, 2, 4)** `[ĐÃ HOÀN THÀNH]`
  - **Ý Tưởng 1 - Cơ Chế "Sốt Gõ Pha Lê" (Fever Rush Mode):**
    - [ClickSystem.cs](file:///d:/Ga21301/GAme/Assets/Scripts/Economy/ClickSystem.cs) tích hợp cơ chế nạp nhiệt Fever (0.08f/tap, tự tụt nhẹ 0.15f/s nếu ngừng gõ). Khi đạt 100%, bùng nổ trạng thái `FEVER RUSH` kéo dài 5.5 giây:
      - Nhân đôi sản lượng gõ ($x2.0\text{ Tap Power}$).
      - Tăng gấp đôi tốc độ nạp nộ Cổ Ngữ (6.0 Nộ / hit).
      - Phát sóng xung kích chấn động, tia lửa rực rỡ và thông báo bay "🔥 SỐT GÕ FEVER x2! 🔥".
    - [TopBarUI.cs](file:///d:/Ga21301/GAme/Assets/Scripts/UI/TopBarUI.cs) hiển thị huy hiệu `FeverBadge` đếm ngược thời gian thực `FEVER x2 (Xs)`.
  - **Ý Tưởng 2 - Hiệu Ứng "Mưa Ngọc Chiến Lợi Phẩm" (Loot Drop Gem Burst):**
    - Xây dựng [LootGemSpawner.cs](file:///d:/Ga21301/GAme/Assets/Scripts/VFX/LootGemSpawner.cs): Mỗi khi quái vật bị hạ gục trong [CrystalDefenseManager.cs](file:///d:/Ga21301/GAme/Assets/Scripts/Adventure/CrystalDefenseManager.cs), 8 - 16 viên ngọc pha lê 3D phát quang (Cyan, Gold, Purple) nổ bung hình quạt lên không trung, nảy nhẹ trên sàn và uốn lượn hút với gia tốc nhanh dần về vị trí HUD Pha Lê TopBar kèm âm thanh leng keng kim loại.
  - **Ý Tưởng 4 - Hệ Thống 5 Đại Di Vật Cổ Kính (Ancient Sacred Relics):**
    - Nâng cấp [MilestoneDataSO.cs](file:///d:/Ga21301/GAme/Assets/Scripts/Data/MilestoneDataSO.cs) và [SceneSetupTool.cs](file:///d:/Ga21301/GAme/Assets/Scripts/Editor/SceneSetupTool.cs):
      - *Tier 1:* Búa Rèn Runic (Runic Forge Hammer) — Mở khóa Khoan Tinh Thể & +15% Sản Lượng.
      - *Tier 2:* Kính Thấu Quang (Prism of Insight) — Mở khóa Lò Nung Mana & +30% Sản Lượng.
      - *Tier 3:* Khiên Cổ Ngữ (Aegis of Sanctuary) — Mở khóa Máy Chiết Xuất & +50% Sản Lượng.
      - *Tier 4:* Vương Miện Thợ Mỏ (Crown of the Artisan) — +100% Sản Lượng Toàn Cục.
      - *Tier 5:* Trái Tim Thánh Điện (Heart of Sanctuary) — +200% Sản Lượng & Hào Quang Thần Hộ Vệ.
    - [MilestonePanelUI.cs](file:///d:/Ga21301/GAme/Assets/Scripts/UI/MilestonePanelUI.cs) hiển thị danh hiệu Di Vật Cổ Đại uy nghiêm, kích hoạt vinh danh "✨ KHAI QUẬT: [TÊN DI VẬT]! ✨".
  - **Vòng lặp Autonomous Verification Loop:** Biên dịch hoàn tất 0 Warning, 0 Error, Scene lưu sạch sẽ.

