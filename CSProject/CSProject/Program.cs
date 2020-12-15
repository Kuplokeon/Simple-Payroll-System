using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Cryptography.X509Certificates;

namespace CSProject
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Staff> myStaff = new List<Staff>();
            FileReader fr = new FileReader();
            int month = 0, year = 0;

            while (year == 0)
            {
                Console.WriteLine("\nPlease enter the year:");
                try
                {
                    year = Convert.ToInt32(Console.ReadLine());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + " Please try again.");
                }
            }

            while (month == 0)
            {
                Console.WriteLine("\nPlease enter the month:");
                try
                {
                    month = Convert.ToInt32(Console.ReadLine());

                    if (month < 1 || month > 12)
                    {
                        Console.WriteLine("Month must be from 1 to 12. Please try again.");
                        month = 0;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + " Please try again.");
                }
            }

            myStaff = fr.ReadFile();

            for (int i = 0; i < myStaff.Count; i++){

                try
                {
                    Console.WriteLine("\nEnter hours worked for " + myStaff[i].NameOfStaff + ": ");
                    myStaff[i].HoursWorked = Convert.ToInt32(Console.ReadLine());
                    myStaff[i].CalculatePay();

                    Console.WriteLine(myStaff[i].ToString());
                } catch (Exception e) {
                    Console.WriteLine(e.Message);
                    i--;
                }
            }

            PaySlip ps = new PaySlip(month, year);
            ps.GeneratePaySlip(myStaff);
            ps.GenerateSummary(myStaff);

            Console.Read();
        }
    }

    class Staff
    {
        private float hourlyRate;
        private int hWorked;

        public Staff(string name, float rate)
        {
            NameOfStaff = name;
            hourlyRate = rate;
        }

        public float TotalPay
        {
            get;
            protected set;
        }
        public float BasicPay
        {
            get;
            private set;
        }
        public string NameOfStaff
        {
            get;
            private set;
        }

        public int HoursWorked
        {
            get { return hWorked; }
            set
            {
                if (value > 0)
                {
                    hWorked = value;
                } else
                {
                    hWorked = 0;
                }
            }
        }

        public virtual void CalculatePay()
        {
            Console.WriteLine("Calculating Pay...");
            BasicPay = hWorked * hourlyRate;
            TotalPay = BasicPay;
        }

        public override string ToString()
        {
            //return hourlyRate + " " + hWorked;
            //return "\nNameOfStaff = " + NameOfStaff + "\nadminHourlyRate = " + admi
            //return "\NameOfStaff = " + NameOfStaff + "\nmanagerHourlyRate = " + 
            return "\nNameOfStaff = " + NameOfStaff + "\nhourlyRate = " + hourlyRate + "\nhWorked = " + hWorked + "\nBasicPay = " + BasicPay + "\n\nTotalPay = " + TotalPay;
        }
    }

    class Admin:Staff
    {
        private const float overtimeRate = 15.5f;
        private const float adminHourlyRate = 30;

        public float Overtime { get; private set; }

        public Admin(string name) : base(name, adminHourlyRate)
        {
            //
        }

        public override void CalculatePay()
        {
            base.CalculatePay();
            if (HoursWorked > 160)
            {
                Overtime = overtimeRate * (HoursWorked - 160);
            }
        }

        public override string ToString()
        {
            return "\nNameOfStaff = " + NameOfStaff + "\nadminHourlyRate = " + adminHourlyRate + "\nHoursWorked = " + HoursWorked + "\nBasic Pay = " + BasicPay + "\nOvertime = " + Overtime + "\n\nTotalPay = " + TotalPay;
        }
    }

    class Manager:Staff
    {
        private const float managerHourlyRate = 50;

        public int Allowance
        {
            get;
            private set;
        }

        public Manager (string name) : base(name, managerHourlyRate)
        {
            //
        }

        public override void CalculatePay()
        {
            base.CalculatePay();
            Allowance = 1000;
            if (HoursWorked > 160)
            {
                TotalPay += Allowance;
            }
        }

        public override string ToString()
        {
            //return managerHourlyRate + " " + base.ToString();
            return "\nNameOfStaff = " + NameOfStaff + "\nmanagerHourlyRate = " + managerHourlyRate + "\nHoursWorked = " + HoursWorked + "\nBasicPay = " + BasicPay + "\nAllowance = " + Allowance + "\n\nTotalPay = " + TotalPay;
        }
    }

    class FileReader
    {
        public List<Staff> ReadFile()
        {
            List<Staff> myStaff = new List<Staff>();
            string[] result = new string[2];
            string path = "C:/Users/Reece Gaddie/source/repos/CSProject/CSProject/obj/Debug/netcoreapp3.1/staff.txt";
            string[] separator = { ", " };

            if (File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    while (!sr.EndOfStream)
                    {
                        result = sr.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        if (result[1] == "Manager")
                            myStaff.Add(new Manager(result[0]));
                        else if (result[1] == "Admin")
                            myStaff.Add(new Admin(result[0]));
                    }
                    sr.Close();
                }
            } else
            {
                Console.WriteLine("Error: File does not exist");
            }
            return myStaff;
        }
    }

    class PaySlip
    {
        private int month;
        private int year;

        enum MonthsOfYear { JAN = 1, FEB = 2, MAR, APR, MAY, JUN, JUL, AUG, SEP, OCT, NOV, DEC }

        public PaySlip(int payMonth, int payYear)
        {
            month = payMonth;
            year = payYear;
        }

        public void GeneratePaySlip(List<Staff> myStaff)
        {
            string path;

            foreach (Staff i in myStaff)
            {
                path = "C:/Users/Reece Gaddie/source/repos/CSProject/CSProject/obj/Debug/netcoreapp3.1/" + i.NameOfStaff + ".txt";

                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.WriteLine("PAYSLIP FOR " + (MonthsOfYear)month + year);
                    sw.WriteLine("====================");
                    sw.WriteLine("Name of Staff: " + i.NameOfStaff);
                    sw.WriteLine("Hours Worked: " + i.HoursWorked);
                    sw.WriteLine("Basic Pay: " + i.BasicPay);
                    if (i.GetType() == typeof(Manager))
                    {
                        sw.WriteLine("Allowance: " + ((Manager)i).Allowance);
                    } else if (false.GetType() == typeof(Manager))
                    {
                        sw.WriteLine("Overtime: " + ((Admin)i).Overtime);
                    }
                    sw.WriteLine("");
                    sw.WriteLine("====================");
                    sw.WriteLine("Total Pay: " + i.TotalPay);
                    sw.WriteLine("====================");
                    sw.Close();
                }
            }
        }

        public void GenerateSummary (List<Staff> myStaff)
        {
            var result = from i in myStaff
                         where i.HoursWorked < 10
                         orderby i.NameOfStaff ascending
                         select new { i.NameOfStaff, i.HoursWorked };

            string path = "C:/Users/Reece Gaddie/source/repos/CSProject/CSProject/obj/Debug/netcoreapp3.1/summary.txt";

            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.WriteLine("Staff with less than 10 working hours");
                sw.WriteLine("");

                foreach (var i in result)
                    sw.WriteLine("Name of staff: " + i.NameOfStaff + " Hours Worked: " + i.HoursWorked);

                sw.Close();
            }
        }

        public override string ToString()
        {
            return "month = " + month + "year = " + year;
        }
    }
}
