
using System;
using System.IO;
using System.Text.Json;

namespace Files_M3;

public class LoadCustomerLogs
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
    public static void ReadCustomerData(Bank bank)
    {
        string ConfigDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Config");
        string accountsDirectoryPath = Path.Combine(ConfigDirectoryPath, "Accounts");
        string transactionsDirectoryPath = Path.Combine(ConfigDirectoryPath, "Transactions");

        JsonRetrieval.LoadAllCustomers(bank, ConfigDirectoryPath, accountsDirectoryPath, transactionsDirectoryPath);

    }
}
