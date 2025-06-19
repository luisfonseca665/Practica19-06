using System;
using System.IO;
using System.Text.Json;

namespace Files_M3;

public class CreateDataLogs
{
    /*

    // The .csproj file needs to include the following ItemGroup element to copy the Config folder to the output directory

    <ItemGroup>
    <!-- Include all files in the Config folder and copy them to the output directory -->
    <Content Include="Config\**\*">
        <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </Content>
    </ItemGroup>
    
    */

    // create the GenerateCustomerData method
    public static void GenerateCustomerData()
    {
        var approvedCustomers = ApprovedCustomersLoader.LoadApprovedNames();

        string ConfigDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Config");

        foreach (var customer in approvedCustomers)
        {
            var newCustomer = new BankCustomer(customer.FirstName, customer.LastName);
            Console.WriteLine($"Created BankCustomer: {newCustomer.ReturnFullName()}");

            newCustomer.AddAccount(new CheckingAccount(newCustomer, newCustomer.CustomerId, 5000));
            newCustomer.AddAccount(new SavingsAccount(newCustomer, newCustomer.CustomerId, 15000));
            newCustomer.AddAccount(new MoneyMarketAccount(newCustomer, newCustomer.CustomerId, 90000));

            // Get the current date
            var currentDate = DateTime.Now;

            // set a DateOnly endDate using the current date to determine th last day of the previous month
            var endDate = new DateOnly(currentDate.Year, currentDate.Month, 1).AddDays(-1);

            // set a DateOnly startDate that's two years before the endDate
            var startDate = endDate.AddYears(-2);

            // Simulate two years of deposits, withdrawals, and transfers for the customer
            newCustomer = SimulateDepositsWithdrawalsTransfers.SimulateActivityDateRange(startDate, endDate, newCustomer);

            // Save the approved customers to a JSON file
            JsonStorage.SaveBankCustomer(newCustomer, ConfigDirectoryPath);

        }
    }
}
