
using System;
using System.IO;
using System.Threading.Tasks;

namespace Files_M3;

public class LoadCustomerLogsAsyncParallel
{
    public static async Task ReadCustomerDataAsyncParallel(Bank bank)
    {
        string configDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Config");
        string accountsDirectoryPath = Path.Combine(configDirectoryPath, "Accounts");
        string transactionsDirectoryPath = Path.Combine(configDirectoryPath, "Transactions");

        await JsonRetrievalAsyncParallel.LoadAllCustomersAsyncParallel(bank, configDirectoryPath, accountsDirectoryPath, transactionsDirectoryPath);
    }
}
