---
trigger: always_on
---

# Antigravity Rules: Tự động chạy và kiểm tra (Auto Run & Test)

Quy định bắt buộc dành cho Antigravity khi thực hiện các tác vụ phát triển trong dự án này:

Luôn tuân thủ nghiêm ngặt các Nguyên tắc Lập trình của Andrej Karpathy được định nghĩa sau:

1. **Suy nghĩ trước khi lập trình**: Nêu rõ các giả định trước khi thực hiện. Nếu yêu cầu không rõ ràng hoặc không được chỉ định đầy đủ, hãy yêu cầu làm rõ thay vì đoán ý định.

2. **Ưu tiên sự đơn giản**: Ưu tiên mã nguồn tối thiểu, sạch sẽ và ngắn gọn. Tránh thiết kế quá phức tạp. Không thêm các tính năng không được yêu cầu, các lớp trừu tượng không cần thiết hoặc các phụ thuộc bổ sung.

3. **Thay đổi có chọn lọc**: CHỈ sửa đổi mã nguồn liên quan trực tiếp đến nhiệm vụ mục tiêu. Tránh các thay đổi gây tác dụng phụ (ví dụ: tái cấu trúc mã không liên quan, định dạng lại định dạng/khoảng trắng, xóa các bình luận hiện có).

4. **Thực thi theo mục tiêu**: Chuyển đổi các nhiệm vụ mơ hồ thành các mục tiêu cụ thể, có thể kiểm thử và xác minh được. Luôn xác minh các thay đổi mã bằng các lệnh xây dựng hoặc kiểm thử trước khi tuyên bố thành công.

## 5. Vòng lặp Tự Động Chạy và Kiểm Tra (Autonomous Verification Loop)
Sau mỗi lần tạo mới hoặc chỉnh sửa mã nguồn (scripts/code), Antigravity **BẮT BUỘC PHẢI TỰ ĐỘNG THỰC HIỆN**:

5. **Kiểm tra biên dịch (Compile Check)**:
   - Tự động gọi công cụ `unityMCP:refresh_unity` để Unity cập nhật script.
   - Tự động gọi `unityMCP:read_console` với bộ lọc lỗi (`types: ["Error", "Exception"]`) để kiểm tra xem mã nguồn mới có phát sinh lỗi biên dịch hay không.
   - Hoặc kiểm tra cú pháp và build qua `dotnet build 3d.slnx` nếu phù hợp.

6. **Tự động chạy Unit/Integration Tests**:
   - Nếu dự án có bộ test suite, tự động kích hoạt `unityMCP:run_tests` (chế độ EditMode hoặc PlayMode).
   - Theo dõi tiến trình qua `unityMCP:get_test_job` và xác nhận kết quả kiểm tra.

7. **Tự sửa lỗi (Self-Healing Loop)**:
   - Nếu phát hiện bất kỳ lỗi biên dịch, ngoại lệ runtime hoặc test bị fail:
     - **Không dừng lại** để đợi người dùng nhắc nhở.
     - Đọc chi tiết log lỗi và stack trace.
     - Tự động sửa lại mã nguồn tương ứng.
     - Tiếp tục chạy lại kiểm tra cho đến khi toàn bộ lỗi được khắc phục triệt để.

## 8. Tiêu chuẩn Mã nguồn & API Unity
- Luôn kiểm tra tính tương thích của API Unity trước khi viết code mới.
- Đảm bảo tuân thủ kiến trúc URP và Input System đã cấu hình trong dự án.
- Không để lại cảnh báo nghiêm trọng (warnings) hoặc lỗi thiếu tham chiếu component (`NullReferenceException`).

