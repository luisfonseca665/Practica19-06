using System;

namespace Files_M3;

public interface IQuarterlyReportGenerator
{
    void GenerateQuarterlyReport(BankCustomer bankCustomer, int accountNumber, DateOnly reportDate);
}
