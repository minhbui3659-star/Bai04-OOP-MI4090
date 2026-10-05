# Bai04-OOP-MI4090

Dự án mô phỏng **Hệ Thống Tính Lương Và Thưởng Nhân Sự (Payroll System)** viết bằng C#. 
Chương trình áp dụng các nguyên lý cốt lõi của Lập trình hướng đối tượng (OOP) bao gồm: Trừu tượng (Abstraction), Kế thừa (Inheritance), Đa hình (Polymorphism) và Nạp chồng (Overloading).

## 🏗️ Cấu trúc hệ thống

* **`Employee`**: Lớp cơ sở trừu tượng (Abstract). Quản lý thông tin chung (Mã, Tên, Phòng ban, Thưởng). Khai báo 3 phiên bản nạp chồng của hàm tính thưởng `addBonus()`.
* **`SalariedEmployee`**: Nhân viên lương cố định (Kế thừa `Employee`). Thu nhập tính dựa trên lương cố định, phụ cấp trách nhiệm và thưởng.
* **`HourlyEmployee`**: Nhân viên theo giờ (Kế thừa `Employee`). Thu nhập tính theo số giờ làm, tự động áp dụng hệ số 1.5 cho số giờ làm thêm vượt mức tiêu chuẩn (160 giờ).
* **`SalesEmployee`**: Nhân viên kinh doanh (Kế thừa `Employee`). Thu nhập bao gồm lương cơ bản, thưởng và tiền hoa hồng (tính theo phần trăm doanh số).
* **`Payroll`**: Lớp bảng lương quản lý danh sách nhân sự. Thực hiện các chức năng thống kê bằng tính đa hình như: tính tổng quỹ lương, tổng lương theo phòng ban, và tìm kiếm nhân viên có lương cao nhất.

## 🧪 Kịch bản kiểm thử (Test Cases)

File `Program02.cs` đi kèm một bộ kiểm thử tự động với hơn 15 test cases (trả về PASS/FAIL) nhằm kiểm chứng:
* **Tính đa hình**: Tính toán chính xác lương cho nhiều loại nhân sự khác nhau trong cùng một danh sách.
* **Kiểm soát ngoại lệ (Exceptions)**: Tự động chặn các dữ liệu sai logic như (lương âm, số giờ làm vượt quá 250h, tỷ lệ hoa hồng vượt 30%, hoặc thêm nhân sự trùng mã ID).
* **Biên dữ liệu**: Hệ thống hoạt động an toàn ngay cả khi bảng lương rỗng.

## 🚀 Hướng dẫn sử dụng

1. Mở file `Program02.cs` bằng IDE hỗ trợ C# (Visual Studio, VS Code, JetBrains Rider,...).
2. Tiến hành biên dịch và chạy (Run) chương trình.
3. Quan sát Terminal/Console để xem chi tiết Bảng lương thống kê và kết quả của toàn bộ các kịch bản kiểm thử.
