# KẾ HOẠCH TRIỂN KHAI HỆ THỐNG ESC CON-COST (.NET 8)

Tài liệu này xác định các giai đoạn phát triển hệ thống dựa trên kiến trúc Rev.15 và sơ đồ ERD chuẩn đã thống nhất.

---

## **GIAI ĐOẠN 1: NỀN TẢNG DỮ LIỆU (CORE ENTITIES)**
**Mục tiêu:** Thiết lập khung xương dữ liệu 13 bảng cho toàn hệ thống.

- [ ] **Task 1.1:** Định nghĩa 14 lớp Entity: `CUSTOMER`, `CONTRACTS`, `CONTRACT_ITEMS`, `ADJUST_RECORDS`, `ADJUST_ITEM_DETAILS`, `INDEX_TYPES`, `INDEX_TIMESERIES`, `INSURANCE_RATES`, `SA_USER`, `ESC_SERVICE_REQUEST`, `ADMIN_SETTINGS`, `AUDIT_LOGS`, `SETTING_PERMISSION`, `SETTING_NOTIFICATION`.
- [ ] **Task 1.2:** Cấu hình Fluent API: Thiết lập quan hệ khóa ngoại, kiểu dữ liệu (UUID, BIGINT, DECIMAL) và ràng buộc (Check constraints) cho `bid_rate`, `threshold_rate`.
- [ ] **Task 1.3:** Triển khai Migration và khởi tạo Database trên SQL Server.

---

## **GIAI ĐOẠN 2: HỘI VIÊN & PHÂN QUYỀN (IDENTITY)**
**Mục tiêu:** Xử lý đăng ký khách hàng và luồng phê duyệt của Admin.

- [ ] **Task 2.1:** Xây dựng bảng `CUSTOMER` lưu hồ sơ (company_name, business_license, is_paid) và liên kết `SA_USER` qua `customer_id`.
- [ ] **Task 2.2:** Xây dựng trang Đăng ký (Register) cho khách hàng.
- [ ] **Task 2.3:** Xây dựng Dashboard Admin duyệt hội viên & gửi thông báo tự động qua `SETTING_NOTIFICATION`.

---

## **GIAI ĐOẠN 3: WIZARD NHẬP LIỆU (SETUP WIZARD)**
**Mục tiêu:** Chuyển đổi logic nhập liệu từ bản mẫu HTML sang hệ thống Web hoàn chỉnh.

- [ ] **Task 3.1:** Code Wizard 3 bước: Lưu các trường đặc thù như `advance_amt`, `excluded_amt`, `threshold_days` vào bảng `CONTRACTS`.
- [ ] **Task 3.2:** Phát triển tính năng Map hạng mục biểu giá vào `CONTRACT_ITEMS` theo `group_name` và `index_key`.
- [ ] **Task 3.3:** Tích hợp PDF Auto-fill dựa trên `pdf_mapping_schema` (Phase sau).

---

## **GIAI ĐOẠN 4: BỘ MÁY TÍNH TOÁN (CALCULATION ENGINE)**
**Mục tiêu:** Thực hiện tính toán Kd chính xác và lưu trữ snapshot.

- [ ] **Task 4.1:** Code logic tính toán $Kd = \Sigma(ai \times Ki)$ tại Server-side (.NET 8).
- [ ] **Task 4.2:** Lưu kết quả vào `ADJUST_RECORDS` và chụp ảnh Snapshot (`index0`, `index1`, `wi_ki`) vào `ADJUST_ITEM_DETAILS`.
- [ ] **Task 4.3:** Xử lý các giá trị tài chính: Khấu trừ tạm ứng (`advance_deduct`), tổng tiền điều chỉnh (`gross_adjust`) và tiền thực nhận (`net_adjust`).

---

## **GIAI ĐOẠN 5: BÁO CÁO & YÊU CẦU DỊCH VỤ (SERVICE REQUEST)**
**Mục tiêu:** Đầu ra sản phẩm và luồng tư vấn.

- [ ] **Task 5.1:** Xây dựng logic nút **"Request"**: Chỉ cho phép khi `threshold_met` và `days_met` là True.
- [ ] **Task 5.2:** Phát triển công cụ xuất báo cáo PDF "종합의견서" dựa trên dữ liệu Snapshot.

---

## **GIAI ĐOẠN 6: QUẢN TRỊ DỮ LIỆU GỐC (ADMIN CONTROL)**
**Mục tiêu:** Quản lý các tham số đầu vào hệ thống.

- [ ] **Task 6.1:** UI Admin cập nhật Chỉ số giá tháng (`INDEX_TIMESERIES`) và Tỷ lệ bảo hiểm năm (`INSURANCE_RATES`).
- [ ] **Task 6.2:** Cấu hình ẩn/hiện vùng dữ liệu màu vàng qua `ADMIN_SETTINGS`.
- [ ] **Task 6.3:** Xây dựng Module Quản lý mẫu Email: Thiết kế nội dung HTML và cơ chế Placeholder.

---

**Tài liệu tham chiếu:** [ESC_Database_Architecture_Report.md](file:///d:/DEV_CUONG/PM/esc_concost/BA/ESC_Database_Architecture_Report.md)
