using System.Collections.Generic;
using System.Globalization;

namespace PayrollSystem
{
    // Lớp cơ sở 
    public abstract class Employee
    {
        public const string DefaultDepartment = "Unassigned";

        private string employeeId;
        private string fullName;
        private string department;
        private decimal monthlyBonus; 

        public string EmployeeId { get { return employeeId; } } 
        public string FullName
        {
            get { return fullName; }
            set { fullName = checkText(value, "Ho ten"); }
        }
        public string Department
        {
            get { return department; }
            set { department = checkText(value, "Phong ban"); }
        }
        public decimal MonthlyBonus { get { return monthlyBonus; } }

        // Constructor rút gọn
        protected Employee(string employeeId, string fullName)
            : this(employeeId, fullName, DefaultDepartment) { }

        protected Employee(string employeeId, string fullName, string department)
        {
            this.employeeId = checkText(employeeId, "Ma nhan su");
            FullName = fullName;
            Department = department;
            monthlyBonus = 0;
        }

        // Nạp chồng
        public void addBonus(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Khoan thuong phai lon hon 0");
            monthlyBonus += amount;
        }

        public void addBonus(decimal amount, string reason)
        {
            checkText(reason, "Ly do thuong");
            addBonus(amount);
        }

        public void addBonus(decimal rate, decimal referenceAmount, string reason)
        {
            if (rate <= 0 || rate > 0.5m)
                throw new ArgumentException("Ty le thuong phai trong (0, 0.5]");
            if (referenceAmount <= 0)
                throw new ArgumentException("Gia tri tham chieu phai lon hon 0");
            checkText(reason, "Ly do thuong");
            addBonus(rate * referenceAmount);
        }

        public void resetBonus()
        {
            monthlyBonus = 0; 
        }

        // Hành vi chung
        public abstract decimal calculateGrossPay();
        public abstract string getEmployeeType();

        public virtual void displayPayrollInfo()
        {
            Console.WriteLine($"[{getEmployeeType()}] {EmployeeId} - {FullName} - {Department}");
        }

        // Hàm tiện ích dùng chung
        protected static string checkText(string value, string name)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException(name + " khong duoc rong");
            return value.Trim();
        }

        protected static decimal checkNotNegative(decimal value, string name)
        {
            if (value < 0)
                throw new ArgumentException(name + " khong duoc am");
            return value;
        }

        protected static string money(decimal value)
        {
            return value.ToString("N0", CultureInfo.InvariantCulture);
        }
    }

    // Nhân viên lương cố định
    public class SalariedEmployee : Employee
    {
        private decimal monthlySalary;
        private decimal responsibilityAllowance;

        public decimal MonthlySalary
        {
            get { return monthlySalary; }
            set { monthlySalary = checkNotNegative(value, "Luong thang"); }
        }
        public decimal ResponsibilityAllowance
        {
            get { return responsibilityAllowance; }
            set { responsibilityAllowance = checkNotNegative(value, "Phu cap"); }
        }

        public SalariedEmployee(string employeeId, string fullName, decimal monthlySalary)
            : this(employeeId, fullName, DefaultDepartment, monthlySalary, 0) { }

        public SalariedEmployee(string employeeId, string fullName, string department,
                                decimal monthlySalary, decimal responsibilityAllowance)
            : base(employeeId, fullName, department)
        {
            MonthlySalary = monthlySalary;
            ResponsibilityAllowance = responsibilityAllowance;
        }

        public override decimal calculateGrossPay()
        {
            return monthlySalary + responsibilityAllowance + MonthlyBonus;
        }

        public override string getEmployeeType() { return "Salaried"; }

        public override void displayPayrollInfo()
        {
            base.displayPayrollInfo();
            Console.WriteLine($"  Luong: {money(monthlySalary)}, Phu cap: {money(responsibilityAllowance)}, Thuong: {money(MonthlyBonus)}");
            Console.WriteLine($"  Thu nhap: {money(calculateGrossPay())}");
        }
    }

    // Nhân viên theo giờ
    public class HourlyEmployee : Employee
    {
        public const decimal StandardHours = 160m;
        public const decimal MaxHours = 250m;
        public const decimal OvertimeRate = 1.5m;

        private decimal hourlyRate;
        private decimal workedHours;

        public decimal HourlyRate
        {
            get { return hourlyRate; }
            set { hourlyRate = checkNotNegative(value, "Don gia gio"); }
        }
        public decimal WorkedHours
        {
            get { return workedHours; }
            set
            {
                if (value < 0 || value > MaxHours)
                    throw new ArgumentException("So gio lam phai trong [0, 250]");
                workedHours = value;
            }
        }
        // Giờ vượt ngưỡng được tính từ trạng thái hiện có, không lưu riêng
        public decimal OvertimeHours { get { return Math.Max(0, workedHours - StandardHours); } }

        public HourlyEmployee(string employeeId, string fullName, decimal hourlyRate)
            : this(employeeId, fullName, DefaultDepartment, hourlyRate, 0) { }

        public HourlyEmployee(string employeeId, string fullName, string department,
                              decimal hourlyRate, decimal workedHours)
            : base(employeeId, fullName, department)
        {
            HourlyRate = hourlyRate;
            WorkedHours = workedHours;
        }

        public decimal calculateBasePay()
        {
            decimal regularHours = Math.Min(workedHours, StandardHours);
            return regularHours * hourlyRate + OvertimeHours * hourlyRate * OvertimeRate;
        }

        public override decimal calculateGrossPay()
        {
            return calculateBasePay() + MonthlyBonus;
        }

        public override string getEmployeeType() { return "Hourly"; }

        public override void displayPayrollInfo()
        {
            base.displayPayrollInfo();
            Console.WriteLine($"  Gio thuong: {workedHours - OvertimeHours}, Gio vuot: {OvertimeHours}, Don gia: {money(hourlyRate)}");
            Console.WriteLine($"  Luong gio: {money(calculateBasePay())}, Thuong: {money(MonthlyBonus)}");
            Console.WriteLine($"  Thu nhap: {money(calculateGrossPay())}");
        }
    }

    // Nhân viên kinh doanh
    public class SalesEmployee : Employee
    {
        public const decimal MaxCommissionRate = 0.3m;

        private decimal baseSalary;
        private decimal salesRevenue;
        private decimal commissionRate;

        public decimal BaseSalary
        {
            get { return baseSalary; }
            set { baseSalary = checkNotNegative(value, "Luong co ban"); }
        }
        public decimal SalesRevenue { get { return salesRevenue; } }
        public decimal CommissionRate
        {
            get { return commissionRate; }
            set
            {
                if (value < 0 || value > MaxCommissionRate)
                    throw new ArgumentException("Ty le hoa hong phai trong [0, 0.3]");
                commissionRate = value;
            }
        }

        public SalesEmployee(string employeeId, string fullName, decimal baseSalary)
            : this(employeeId, fullName, DefaultDepartment, baseSalary, 0, 0) { }

        public SalesEmployee(string employeeId, string fullName, string department,
                             decimal baseSalary, decimal salesRevenue, decimal commissionRate)
            : base(employeeId, fullName, department)
        {
            BaseSalary = baseSalary;
            updateSalesRevenue(salesRevenue);
            CommissionRate = commissionRate;
        }

        // Cập nhật doanh số
        public void updateSalesRevenue(decimal revenue)
        {
            salesRevenue = checkNotNegative(revenue, "Doanh so");
        }

        public override decimal calculateGrossPay()
        {
            return baseSalary + salesRevenue * commissionRate + MonthlyBonus;
        }

        public override string getEmployeeType() { return "Sales"; }

        public override void displayPayrollInfo()
        {
            base.displayPayrollInfo();
            Console.WriteLine($"  Luong co ban: {money(baseSalary)}, Hoa hong: {money(salesRevenue * commissionRate)}, Thuong: {money(MonthlyBonus)}");
            Console.WriteLine($"  Thu nhap: {money(calculateGrossPay())}");
        }
    }

    // Bảng lương
    public class Payroll
    {
        private string period;
        private List<Employee> employees = new List<Employee>(); 

        public string Period { get { return period; } }
        public int Count { get { return employees.Count; } }

        public Payroll(string period)
        {
            if (string.IsNullOrWhiteSpace(period))
                throw new ArgumentException("Ky luong khong duoc rong");
            this.period = period.Trim();
        }

        // Trả về false nếu trùng mã
        public bool addEmployee(Employee employee)
        {
            if (employee == null)
                throw new ArgumentException("Nhan su khong duoc null");
            if (findEmployee(employee.EmployeeId) != null)
                return false;
            employees.Add(employee);
            return true;
        }

        public Employee findEmployee(string employeeId)
        {
            foreach (Employee e in employees)
            {
                if (e.EmployeeId == employeeId)
                    return e;
            }
            return null;
        }

        public decimal calculateTotalPayroll()
        {
            decimal total = 0;
            foreach (Employee e in employees)
                total += e.calculateGrossPay(); 
            return total;
        }

        public decimal calculatePayrollByDepartment(string department)
        {
            decimal total = 0;
            foreach (Employee e in employees)
            {
                if (string.Equals(e.Department, department, StringComparison.OrdinalIgnoreCase))
                    total += e.calculateGrossPay();
            }
            return total;
        }

        // Danh sách rỗng => null
        public Employee findHighestPaidEmployee()
        {
            Employee best = null;
            foreach (Employee e in employees)
            {
                if (best == null || e.calculateGrossPay() > best.calculateGrossPay())
                    best = e;
            }
            return best;
        }

        public void displayPayroll()
        {
            Console.WriteLine($"===== BANG LUONG {period} =====");
            if (employees.Count == 0)
            {
                Console.WriteLine("(Danh sach rong)");
                return;
            }
            foreach (Employee e in employees)
                e.displayPayrollInfo(); // đa hình
            Console.WriteLine($"TONG: {calculateTotalPayroll().ToString("N0", CultureInfo.InvariantCulture)}");
        }
    }

    // Chương trình kiểm thử
    class Program
    {
        static int passed = 0, failed = 0;

        static void check(string name, bool ok)
        {
            if (ok) passed++; else failed++;
            Console.WriteLine($"{(ok ? "PASS" : "FAIL")} - {name}");
        }

        static bool throws(Action action)
        {
            try { action(); return false; }
            catch (ArgumentException) { return true; }
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Dữ liệu kiểm thử
            SalariedEmployee e1 = new SalariedEmployee("E001", "Nguyễn Minh An", "Đào tạo", 15000000, 2000000);
            e1.addBonus(1000000);                                        // phiên bản 1

            HourlyEmployee e2 = new HourlyEmployee("E002", "Trần Thu Bình", "Hỗ trợ", 100000, 150);
            e2.addBonus(500000, "Thuong chuyen can");                    // phiên bản 2

            HourlyEmployee e3 = new HourlyEmployee("E003", "Lê Hoàng Chi", "Hỗ trợ", 100000, 170);

            SalesEmployee e4 = new SalesEmployee("E004", "Phạm Quốc Dũng", "Kinh doanh", 8000000, 200000000, 0.05m);
            e4.addBonus(0.02m, 50000000, "Thuong doanh so");             // phiên bản 3

            Payroll payroll = new Payroll("2026-09");
            payroll.addEmployee(e1);
            payroll.addEmployee(e2);
            payroll.addEmployee(e3);
            payroll.addEmployee(e4);
            payroll.displayPayroll();
            Console.WriteLine();

            // 1-4: thu nhập từng người
            check("E001 = 18.000.000", e1.calculateGrossPay() == 18000000m);
            check("E002 = 15.500.000 (khong vuot gio)", e2.calculateGrossPay() == 15500000m);
            check("E003 = 17.500.000 (vuot 10 gio)", e3.calculateGrossPay() == 17500000m);
            check("E004 = 19.000.000", e4.calculateGrossPay() == 19000000m);
            // 5-7: tổng hợp
            check("Tong bang luong = 70.000.000", payroll.calculateTotalPayroll() == 70000000m);
            check("Tong phong Ho tro = 33.000.000", payroll.calculatePayrollByDepartment("Hỗ trợ") == 33000000m);
            check("Nguoi cao nhat la E004", payroll.findHighestPaidEmployee().EmployeeId == "E004");
            // 8: trùng mã
            check("Khong them trung ma", payroll.addEmployee(new SalariedEmployee("E001", "Khac", 1000)) == false
                                         && payroll.Count == 4);
            // 9-10: biên số giờ
            check("Gio = 160 -> 16.000.000", new HourlyEmployee("T1", "A", "X", 100000, 160).calculateGrossPay() == 16000000m);
            check("Gio = 250 hop le, 251 loi",
                  !throws(() => new HourlyEmployee("T2", "A", "X", 100000, 250))
                  && throws(() => new HourlyEmployee("T3", "A", "X", 100000, 251)));
            check("Gio am bi tu choi", throws(() => new HourlyEmployee("T4", "A", "X", 100000, -1)));
            // 11: biên hoa hồng
            check("Hoa hong 0.3 hop le, 0.31 loi",
                  !throws(() => new SalesEmployee("T5", "A", "X", 1000, 1000, 0.3m))
                  && throws(() => new SalesEmployee("T6", "A", "X", 1000, 1000, 0.31m)));
            // 12-14: kiểm tra addBonus
            check("addBonus(0) loi", throws(() => e1.addBonus(0)));
            check("addBonus ty le 0.5 hop le, 0.51 loi",
                  !throws(() => new SalariedEmployee("T7", "A", 1000).addBonus(0.5m, 1000, "ok"))
                  && throws(() => e1.addBonus(0.51m, 1000, "qua cao")));
            check("addBonus ly do rong loi", throws(() => e1.addBonus(1000, "  ")));
            check("addBonus loi khong lam doi tong thuong", e1.MonthlyBonus == 1000000m);
            // 15-16: bất biến Employee
            check("Ma rong bi tu choi", throws(() => new SalariedEmployee("", "A", 1000)));
            check("Luong am bi tu choi", throws(() => new SalariedEmployee("T8", "A", -1)));
            check("Constructor rut gon: phong ban mac dinh",
                  new SalariedEmployee("T9", "A", 1000).Department == "Unassigned");
            // 17: bảng lương rỗng
            Payroll empty = new Payroll("2026-10");
            check("Bang luong rong: tong 0, khong co nguoi cao nhat",
                  empty.calculateTotalPayroll() == 0 && empty.findHighestPaidEmployee() == null);
            empty.displayPayroll();

            Console.WriteLine($"\nKet qua: {passed} dat, {failed} loi");
        }
    }
}
