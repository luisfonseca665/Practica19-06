
using System;
using System.IO;
using System.Threading.Tasks;

namespace Files_M3;

public class LoadCustomerLogsAsync
{
    public static async Task ReadCustomerDataAsync(Bank bank)
    {
        string configDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Config");
        string accountsDirectoryPath = Path.Combine(configDirectoryPath, "Accounts");
        string transactionsDirectoryPath = Path.Combine(configDirectoryPath, "Transactions");

        await JsonRetrievalAsync.LoadAllCustomersAsync(bank, configDirectoryPath, accountsDirectoryPath, transactionsDirectoryPath);
    }
}
