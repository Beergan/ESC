# BÁO CÁO PHÂN TÍCH CẤU TRÚC MÃ NGUỒN (SOURCE CODE ANALYSIS)
**Dự án:** ESC CON-COST  |  **Ngày:** 09/05/2026

Tài liệu này hệ thống hóa cấu trúc thư mục và vai trò của từng project trong hệ thống hiện tại, làm căn cứ để triển khai module ESC.

---

## **1. TỔNG QUAN KIẾN TRÚC**
Hệ thống sử dụng kiến trúc **Modular Monolith** kết hợp với **Clean Architecture** (phân tách tầng Abstract, Base, Db và UI).

### **Các Project Cốt lõi (Cross-cutting)**
|**Project**|**Vai trò**|**Nội dung chính**|
| :- | :- | :- |
|**ESC.CONCOST.Abstract**|Tầng trừu tượng|Định nghĩa Interfaces, Base Entities (AuditLog), Constants.|
|**ESC.CONCOST.Base**|Tầng cơ sở|Cung cấp các lớp tiện ích, thực thể dùng chung (SA_USER).|
|**ESC.CONCOST.Db**|Tầng truy cập dữ liệu|DbContext (MSSQL/MySQL), Migrations, Repositories.|
|**ESC.CONCOST.WebHost**|Host ứng dụng|Cấu hình Startup, API Endpoints, Middleware.|
|**ESC.CONCOST.WebApp**|Blazor WASM|Entry point của phía Client, cấu hình Routing chính.|

---

## **2. CẤU TRÚC MODULE (MODULE PATTERN)**
Mỗi module chức năng (Ví dụ: Employee, Setting) được tách thành 3 project độc lập:

1.  **Module X (Backend):** Chứa Services, Handlers (MediatR), Business Logic.
2.  **Module X Core:** Chứa các lớp DTO (Data Transfer Object), Interfaces dùng chung giữa Client-Server.
3.  **Module X Blazor:** Chứa các Razor Components, Pages, UI Logic dành riêng cho module đó.

---

## **3. PHÂN TÍCH VỊ TRÍ TRIỂN KHAI MODULE ESC**

Dựa trên cấu trúc hiện tại, việc triển khai Module ESC (Điều chỉnh giá) cần được phân bổ như sau:

### **A. Tầng Dữ liệu (Database)**
*   **Vị trí:** `src/ESC.CONCOST.Db/Entities/` (Đã tạo).
*   **Hành động:** Định nghĩa 14 Entities theo schema Rev.15. Đăng ký vào `DbMssqlContext`.

### **B. Tầng Nghiệp vụ (Backend)**
*   **Vị trí đề xuất:** `src/ESC.CONCOST.ModuleESC/` (Cần tạo mới).
*   **Nội dung:** Chứa `CalculationService` (tính Kd), `ContractService`, `ExcelService`.

### **C. Tầng Giao diện (Frontend)**
*   **Vị trí đề xuất:** `src/ESC.CONCOST.ModuleESCBlazor/` (Cần tạo mới).
*   **Nội dung:** Chứa các Page cho Wizard, Calculation, Reports.

---

## **4. CÁC ĐIỂM CẦN LƯU Ý KHI CODE**
*   **SA_USER:** Đang nằm ở `ESC.CONCOST.Base`. Cần mở rộng hoặc liên kết với `CUSTOMER` thay vì tạo mới hoàn toàn để tránh phá vỡ Identity hiện có.
*   **AuditLog:** Đang nằm ở `ESC.CONCOST.Abstract`. Cần cập nhật TableName sang `AUDIT_LOGS` để khớp với Rev.15.
*   **Dependency Injection:** Các service mới cần được đăng ký trong `DbMssqlRegister` hoặc một lớp `ModuleRegister` riêng của ESC.

---

**Kết luận:** Hệ thống hiện tại rất chặt chẽ. Việc tạo thêm các project `ModuleESC` và `ModuleESCBlazor` là phương án sạch nhất để không ảnh hưởng đến các module Employee hay Management đang có.
