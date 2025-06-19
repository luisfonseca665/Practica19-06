using System;

namespace Files_M3;

public class SimulateCustomerAccountActivity
{
    public static BankCustomer SimulateActivityDateRange(DateOnly startDate, DateOnly endDate, BankCustomer bankCustomer)
    {
        // Determine the starting day, month, and year
        int startDay = startDate.Day;
        int startMonth = startDate.Month;
        int startYear = startDate.Year;

        // Determine the ending day, month, and year
        int endDay = endDate.Day;
        int endMonth = endDate.Month;
        int endYear = endDate.Year;

        // Check if the startDate is the first day of the month
        bool isStartDateFirstDayOfMonth = startDay == 1;

        // Check if the endDate is the last day of the month
        bool isEndDateLastDayOfMonth = endDay == DateTime.DaysInMonth(endYear, endMonth);

        // Call SimulateActivityForPeriod if the startDate is not the first day of the month
        if (!isStartDateFirstDayOfMonth)
        {
            DateOnly lastDayOfMonth = new DateOnly(startYear, startMonth, DateTime.DaysInMonth(startYear, startMonth));
            bankCustomer = SimulateActivityForPeriod(startDate, lastDayOfMonth, bankCustomer);
            startDate = lastDayOfMonth.AddDays(1);
        }

        // Need to compare the month and year of the start and end dates
        DateOnly startDayFirstFullMonth = new DateOnly(startDate.Year, startDate.Month, 1);
        DateOnly startDayLastFullMonth = new DateOnly(endYear, endMonth, 1);

        // If the start date for the first month and the start date for the last month are the same, then the date range is exactly one month
        if (startDayFirstFullMonth == startDayLastFullMonth)
        {
            // Call SimulateActivityForPeriod once
            bankCustomer = SimulateActivityForPeriod(startDate, endDate, bankCustomer);
        }
        else
        {
            // Call SimulateActivityForPeriod for each full month in the date range
            DateOnly currentMonth = startDayFirstFullMonth;
            while (currentMonth < startDayLastFullMonth)
            {
                DateOnly lastDayOfMonth = new DateOnly(currentMonth.Year, currentMonth.Month, DateTime.DaysInMonth(currentMonth.Year, currentMonth.Month));
                bankCustomer = SimulateActivityForPeriod(currentMonth, lastDayOfMonth, bankCustomer);
                currentMonth = currentMonth.AddMonths(1);
            }

            // Call SimulateActivityForPeriod for the remaining days in the last month
            bankCustomer = SimulateActivityForPeriod(startDayLastFullMonth, endDate, bankCustomer);
        }

        // Return the updated bankCustomer
        return bankCustomer;
    }

    private static BankCustomer SimulateActivityForPeriod(DateOnly startDate, DateOnly endDate, BankCustomer bankCustomer)
    {
        double[] monthlyExpenses = ReturnMonthlyExpenses();

        double semiMonthlyPaycheck = monthlyExpenses[0];
        double transferToSavings = monthlyExpenses[1];
        double rent = monthlyExpenses[2];
        double entertainment1 = monthlyExpenses[3];
        double entertainment2 = monthlyExpenses[4];
        double entertainment3 = monthlyExpenses[5];
        double entertainment4 = monthlyExpenses[6];
        double monthlyGasElectric = monthlyExpenses[7];
        double monthlyWaterSewer = monthlyExpenses[8];
        double monthlyWasteManagement = monthlyExpenses[9];
        double monthlyHealthClub = monthlyExpenses[10];
        double creditCardBill = monthlyExpenses[11];

        double runningBalance1 = bankCustomer.Accounts[0].Balance;
        double runningBalance2 = bankCustomer.Accounts[1].Balance;

        DateOnly middleOfMonth = new DateOnly(startDate.Year, startDate.Month, 14);
        if (middleOfMonth.DayOfWeek == DayOfWeek.Saturday)
        {
            middleOfMonth = middleOfMonth.AddDays(2);
        }
        else if (middleOfMonth.DayOfWeek == DayOfWeek.Sunday)
        {
            middleOfMonth = middleOfMonth.AddDays(1);
        }

        DateOnly endOfMonth = new DateOnly(endDate.Year, endDate.Month, DateTime.DaysInMonth(endDate.Year, endDate.Month));
        if (endOfMonth.DayOfWeek == DayOfWeek.Saturday)
        {
            endOfMonth = endOfMonth.AddDays(-1);
        }
        else if (endOfMonth.DayOfWeek == DayOfWeek.Sunday)
        {
            endOfMonth = endOfMonth.AddDays(-2);
        }

        if (middleOfMonth >= startDate && middleOfMonth <= endDate)
        {
            bankCustomer.Accounts[0].AddTransaction(new Transaction(middleOfMonth, new TimeOnly(12, 00), runningBalance1, semiMonthlyPaycheck, bankCustomer.Accounts[0].AccountNumber, bankCustomer.Accounts[0].AccountNumber, "Deposit", "Bi-monthly salary deposit"));
            runningBalance1 += semiMonthlyPaycheck;
        }
        if (endOfMonth >= startDate && endOfMonth <= endDate)
        {
            bankCustomer.Accounts[0].AddTransaction(new Transaction(endOfMonth, new TimeOnly(12, 00), runningBalance1, semiMonthlyPaycheck, bankCustomer.Accounts[0].AccountNumber, bankCustomer.Accounts[0].AccountNumber, "Deposit", "Bi-monthly salary deposit"));
            runningBalance1 += semiMonthlyPaycheck;
        }

        if (startDate <= new DateOnly(startDate.Year, startDate.Month, 1) && new DateOnly(startDate.Year, startDate.Month, 1) <= endDate)
        {
            bankCustomer.Accounts[0].AddTransaction(new Transaction(new DateOnly(startDate.Year, startDate.Month, 1), new TimeOnly(12, 00), runningBalance1, transferToSavings, bankCustomer.Accounts[0].AccountNumber, bankCustomer.Accounts[1].AccountNumber, "Transfer", "Transfer checking to savings account"));
            runningBalance1 -= transferToSavings;
            runningBalance2 += transferToSavings;

            bankCustomer.Accounts[0].AddTransaction(new Transaction(new DateOnly(startDate.Year, startDate.Month, 1), new TimeOnly(12, 00), runningBalance1, rent, bankCustomer.Accounts[0].AccountNumber, bankCustomer.Accounts[0].AccountNumber, "Withdraw", "Auto-pay rent"));
            runningBalance1 -= rent;
        }

        DateOnly saturday1 = new DateOnly(startDate.Year, startDate.Month, 1);
        while (saturday1.DayOfWeek != DayOfWeek.Saturday)
        {
            saturday1 = saturday1.AddDays(1);
        }
        DateOnly saturday2 = saturday1.AddDays(7);
        DateOnly saturday3 = saturday2.AddDays(7);
        DateOnly saturday4 = saturday3.AddDays(7);

        if (saturday1 >= startDate && saturday1 <= endDate)
        {
            bankCustomer.Accounts[0].AddTransaction(new Transaction(saturday1, new TimeOnly(21, 00), runningBalance1, entertainment1, bankCustomer.Accounts[0].AccountNumber, bankCustomer.Accounts[0].AccountNumber, "Withdraw", "Debit card purchase"));
            runningBalance1 -= entertainment1;
        }
        if (saturday2 >= startDate && saturday2 <= endDate)
        {
            bankCustomer.Accounts[0].AddTransaction(new Transaction(saturday2, new TimeOnly(21, 00), runningBalance1, entertainment2, bankCustomer.Accounts[0].AccountNumber, bankCustomer.Accounts[0].AccountNumber, "Withdraw", "Debit card purchase"));
            runningBalance1 -= entertainment2;
        }
        if (saturday3 >= startDate && saturday3 <= endDate)
        {
            bankCustomer.Accounts[0].AddTransaction(new Transaction(saturday3, new TimeOnly(21, 00), runningBalance1, entertainment3, bankCustomer.Accounts[0].AccountNumber, bankCustomer.Accounts[0].AccountNumber, "Withdraw", "Debit card purchase"));
            runningBalance1 -= entertainment3;
        }
        if (saturday4 >= startDate && saturday4 <= endDate)
        {
            bankCustomer.Accounts[0].AddTransaction(new Transaction(saturday4, new TimeOnly(21, 00), runningBalance1, entertainment4, bankCustomer.Accounts[0].AccountNumber, bankCustomer.Accounts[0].AccountNumber, "Withdraw", "Debit card purchase"));
            runningBalance1 -= entertainment4;
        }

        DateOnly billDate = new DateOnly(startDate.Year, startDate.Month, 20);
        if (billDate >= startDate && billDate <= endDate)
        {
            bankCustomer.Accounts[0].AddTransaction(new Transaction(billDate, new TimeOnly(12, 00), runningBalance1, monthlyGasElectric, bankCustomer.Accounts[0].AccountNumber, bankCustomer.Accounts[0].AccountNumber, "Withdraw", "Auto-pay gas and electric bill"));
            runningBalance1 -= monthlyGasElectric;

            bankCustomer.Accounts[0].AddTransaction(new Transaction(billDate, new TimeOnly(12, 00), runningBalance1, monthlyWaterSewer, bankCustomer.Accounts[0].AccountNumber, bankCustomer.Accounts[0].AccountNumber, "Withdraw", "Auto-pay water and sewer bill"));
            runningBalance1 -= monthlyWaterSewer;

            bankCustomer.Accounts[0].AddTransaction(new Transaction(billDate, new TimeOnly(12, 00), runningBalance1, monthlyWasteManagement, bankCustomer.Accounts[0].AccountNumber, bankCustomer.Accounts[0].AccountNumber, "Withdraw", "Auto-pay waste management bill"));
            runningBalance1 -= monthlyWasteManagement;

            bankCustomer.Accounts[0].AddTransaction(new Transaction(billDate, new TimeOnly(12, 00), runningBalance1, monthlyHealthClub, bankCustomer.Accounts[0].AccountNumber, bankCustomer.Accounts[0].AccountNumber, "Withdraw", "Auto-pay health club membership"));
            runningBalance1 -= monthlyHealthClub;
        }

        DateOnly monday1 = new DateOnly(startDate.Year, startDate.Month, 1);
        while (monday1.DayOfWeek != DayOfWeek.Monday)
        {
            monday1 = monday1.AddDays(1);
        }
        DateOnly monday2 = monday1.AddDays(7);
        DateOnly monday3 = monday2.AddDays(7);
        DateOnly monday4 = monday3.AddDays(7);

        if (monday1 >= startDate && monday1 <= endDate)
        {
            bankCustomer.Accounts[0].AddTransaction(new Transaction(monday1, new TimeOnly(9, 00), runningBalance1, 400.00, bankCustomer.Accounts[0].AccountNumber, bankCustomer.Accounts[0].AccountNumber, "Withdraw", "Withdraw for expenses"));
            runningBalance1 -= 400.00;
        }
        if (monday2 >= startDate && monday2 <= endDate)
        {
            bankCustomer.Accounts[0].AddTransaction(new Transaction(monday2, new TimeOnly(9, 00), runningBalance1, 400.00, bankCustomer.Accounts[0].AccountNumber, bankCustomer.Accounts[0].AccountNumber, "Withdraw", "Withdraw for expenses"));
            runningBalance1 -= 400.00;
        }
        if (monday3 >= startDate && monday3 <= endDate)
        {
            bankCustomer.Accounts[0].AddTransaction(new Transaction(monday3, new TimeOnly(9, 00), runningBalance1, 400.00, bankCustomer.Accounts[0].AccountNumber, bankCustomer.Accounts[0].AccountNumber, "Withdraw", "Withdraw for expenses"));
            runningBalance1 -= 400.00;
        }
        if (monday4 >= startDate && monday4 <= endDate)
        {
            bankCustomer.Accounts[0].AddTransaction(new Transaction(monday4, new TimeOnly(9, 00), runningBalance1, 400.00, bankCustomer.Accounts[0].AccountNumber, bankCustomer.Accounts[0].AccountNumber, "Withdraw", "Withdraw for expenses"));
            runningBalance1 -= 400.00;
        }

        if (endOfMonth >= startDate && endOfMonth <= endDate)
        {
            bankCustomer.Accounts[0].AddTransaction(new Transaction(endOfMonth, new TimeOnly(12, 00), runningBalance1, creditCardBill, bankCustomer.Accounts[0].AccountNumber, bankCustomer.Accounts[0].AccountNumber, "Withdraw", "Auto-pay credit card bill"));
            runningBalance1 -= creditCardBill;
        }

        DateOnly refundDate = new DateOnly(startDate.Year, startDate.Month, 5);
        if (refundDate >= startDate && refundDate <= endDate)
        {
            bankCustomer.Accounts[1].AddTransaction(new Transaction(refundDate, new TimeOnly(12, 00), runningBalance2, 100.00, bankCustomer.Accounts[1].AccountNumber, bankCustomer.Accounts[0].AccountNumber, "Refund", "Refund for overcharge"));
            runningBalance2 += 100.00;
        }

        DateOnly feeDate1 = new DateOnly(startDate.Year, startDate.Month, 3);
        DateOnly feeDate2 = new DateOnly(startDate.Year, startDate.Month, 10);
        if (feeDate1 >= startDate && feeDate1 <= endDate)
        {
            bankCustomer.Accounts[0].AddTransaction(new Transaction(feeDate1, new TimeOnly(12, 00), runningBalance1, -50.00, bankCustomer.Accounts[0].AccountNumber, bankCustomer.Accounts[0].AccountNumber, "Fee", "Monthly fee"));
            runningBalance1 -= 50.00;
        }
        if (feeDate2 >= startDate && feeDate2 <= endDate)
        {
            bankCustomer.Accounts[0].AddTransaction(new Transaction(feeDate2, new TimeOnly(12, 00), runningBalance1, -50.00, bankCustomer.Accounts[0].AccountNumber, bankCustomer.Accounts[0].AccountNumber, "Fee", "Monthly fee"));
            runningBalance1 -= 50.00;
        }

        return bankCustomer;
    }

    static double[] ReturnMonthlyExpenses()
    {
        Random random = new Random();

        // Generate a salary paycheck amount. Calculate a random salary amount between 2000 and 5000.
        double semiMonthlyPaycheck = random.Next(2000, 5000);

        // Generate a default transfer that's 25% of the salary paycheck amount rounded down to nearest 100.
        double transferToSavings = Math.Floor(semiMonthlyPaycheck * 0.25 / 100) * 100;

        // Generate a rent amount using random value between 800 and 1600 plus 80% of a paycheck.
        double rent = random.Next(800, 1600) + semiMonthlyPaycheck * 0.8;

        // Generate four random entertainment expense amounts between 150 and 220.
        double entertainment1 = random.Next(150, 220);
        double entertainment2 = random.Next(150, 220);
        double entertainment3 = random.Next(150, 220);
        double entertainment4 = random.Next(150, 220);

        // Generate a monthly gas and electric bill using a random number between 100 and 150.
        double monthlyGasElectric = random.Next(100, 150);

        // Generate a monthly water and sewer bill using a random number between 80 and 90.
        double monthlyWaterSewer = random.Next(80, 90);

        // Generate a monthly waste management bill using a random number between 60 and 70.
        double monthlyWasteManagement = random.Next(60, 70);

        // Generate a monthly health club membership bill using a random number between 120 and 160.
        double monthlyHealthClub = random.Next(120, 160);

        // Generate a random credit card bill between 1000 and 1500 plus 40% of a paycheck.
        double creditCardBill = random.Next(1000, 1500) + semiMonthlyPaycheck * 0.4;

        // Create an array with the monthly expenses
        double[] monthlyExpenses = new double[]
        {
            semiMonthlyPaycheck,
            transferToSavings,
            rent,
            entertainment1,
            entertainment2,
            entertainment3,
            entertainment4,
            monthlyGasElectric,
            monthlyWaterSewer,
            monthlyWasteManagement,
            monthlyHealthClub,
            creditCardBill
        };

        return monthlyExpenses;
    }
}
