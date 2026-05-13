# BÁO CÁO PHÂN TÍCH KIẾN TRÚC & HƯỚNG DẪN TRIỂN KHAI (ARCHITECTURAL ANALYSIS)
**Chuyên gia:** Senior Software Architect / Code Reviewer  
**Dự án:** ESC CON-COST System  |  **Ngày:** 09/05/2026

---

## **1. TỔNG QUAN KIẾN TRÚC HỆ THỐNG**
Dựa trên việc rà soát mã nguồn tại `/src`, hệ thống đang vận hành theo mô hình **Modular Monolith** kết hợp với các nguyên lý của **Clean Architecture**.

*   **Tầng Presentation (UI):** Blazor WebAssembly (`ModuleXBlazor`). Giao diện được cấu trúc theo dạng component-based, sử dụng các Page để điều hướng.
*   **Tầng Application/Domain Logic:** Phân tách theo Module độc lập (`ModuleX`). Sử dụng **MediatR** (Handlers) để tách biệt Command và Query, giúp code dễ đọc và dễ test.
*   **Tầng Data Access:** Sử dụng EF Core với MSSQL. **Repository Pattern** được triển khai thông qua `IRepository<T>` và `BaseRepository<T>`.
*   **Tầng Host (WebHost):** Đóng vai trò là Server trung tâm, quản lý DI (Dependency Injection), Middleware và Routing.

---

## **2. PHÂN TÍCH MODULE & CHỨC NĂNG HIỆN CÓ**
Hệ thống hiện có các module chính:
1.  **Module Management:** Xử lý các nghiệp vụ cốt lõi về hệ thống, phân quyền (Permissions) và tài khoản (Identity).
2.  **Module Employee:** Quản lý thông tin nhân sự.
3.  **Module Setting:** Quản lý cấu hình toàn cục.

**Mối liên quan đến module ESC:** 
Module ESC sẽ cần tương tác mật thiết với `SA_USER` (Auth) và hệ thống `AuditLog` hiện có để đảm bảo tính minh bạch trong tính toán bù giá.

---

## **3. PHÂN TÍCH DATABASE & MODEL**
Hệ thống hiện đang sử dụng **ASP.NET Core Identity** cho `SA_USER`. 

### **Các thay đổi rủi ro cần lưu ý:**
*   **SA_USER:** Hiện đang lưu cả thông tin cá nhân và thông tin công ty. Theo Rev.15, ta cần tách thành `SA_USER` (chỉ lưu Auth) và `CUSTOMER` (lưu Profile). 
*   **AuditLog:** Hiện đang dùng bảng `SYSTEM_AUDIT_LOG`. Việc đổi tên sang `AUDIT_LOGS` cần được thực hiện qua Migration cẩn thận để không mất dữ liệu cũ.

---

## **4. QUY CHUẨN CODE (CODING CONVENTION)**
Để đảm bảo tính đồng nhất, Module ESC cần tuân thủ:
*   **Naming:** 
    *   Entity: `Entity[Name].cs` (ví dụ: `EntityContract.cs`).
    *   Handler: `QryHandler[Name].cs` (Query) và `CmdHandler[Name].cs` (Command).
*   **Pattern:** Sử dụng **Fluent API** để cấu hình quan hệ bảng trong file `ModelBuilderExt.cs`.
*   **Dependency Injection:** Khai báo Service trong các lớp `ModuleRegister` của từng module.

---

## **5. ĐỀ XUẤT HƯỚNG TRIỂN KHAI MODULE ESC (KHÔNG PHÁ VỠ HỆ THỐNG)**

### **A. Cấu trúc Project mới**
Để đảm bảo tính cô lập (Isolation), chúng ta nên tạo các project mới:
1.  **`ESC.CONCOST.ModuleESC`**: Chứa logic nghiệp vụ (Service tính Kd, Handlers).
2.  **`ESC.CONCOST.ModuleESCBlazor`**: Chứa giao diện Wizard, Báo cáo và Dashboard khách hàng.

### **B. Xử lý Identity & Auth**
*   **Không xóa** các trường cũ trong `SA_USER` mà chỉ **thêm mới** (Add columns) và tạo liên kết (Relationship) sang bảng `CUSTOMER`. Điều này đảm bảo module Employee vẫn hoạt động bình thường.

### **C. Triển khai Database**
*   Tất cả Entity mới sẽ nằm trong `ESC.CONCOST.Db/Entities/`.
*   Sử dụng `ModelBuilder.Entity<T>()` trong Context để thiết lập các ràng buộc dữ liệu (Constraints).

---

## **6. RỦI RO & PHƯƠNG ÁN DỰ PHÒNG**
*   **Rủi ro hiệu năng:** Tính toán Kd cần truy vấn chuỗi thời gian lớn. 
    *   *Giải pháp:* Sử dụng Caching cho bảng `INDEX_TIMESERIES`.
*   **Rủi ro logic:** Trùng lặp code xử lý giữa các module.
    *   *Giải pháp:* Sử dụng `ESC.CONCOST.Abstract` để định nghĩa các Interface chung.

---

## **7. CHECKLIST TRƯỚC KHI BẮT ĐẦU CODE**
- [ ] Rà soát lại `DbMssqlContext` để đảm bảo không có Migration cũ bị lỗi.
- [ ] Tạo project template cho `ModuleESC` và `ModuleESCBlazor`.
- [ ] Cấu hình DI cho module mới trong `Program.cs` của WebHost và WebApp.
- [ ] Thực hiện Task 1.1: Tạo các Entity Class với namespace chuẩn.

---
**Người lập báo cáo:** Senior Software Architect AI
