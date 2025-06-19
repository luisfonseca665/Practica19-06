
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Files_M3;

public static class JsonRetrieval
{
    private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
    {
        MaxDepth = 64,
        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
    };

    public static BankCustomerDTO LoadBankCustomerDTO(string filePath)
    {
        string json = File.ReadAllText(filePath);

        if (string.IsNullOrEmpty(json))
        {
            throw new Exception("No customer found.");
        }
        else
        {
            var customerDTO = JsonSerializer.Deserialize<BankCustomerDTO>(json, _options);
            if (customerDTO == null)
            {
                throw new Exception("Customer could not be deserialized.");
            }
            return customerDTO;
        }
    }

    public static BankCustomer LoadBankCustomer(Bank bank, string filePath, string accountsDirectoryPath, string transactionsDirectoryPath)
    {
        // Load customer data from a file using the provided file path (filename = CustomerId.json)
        var customerDTO = LoadBankCustomerDTO(filePath); // customerDTO: CustomerId, FirstName, LastName, AccountNumbers (list)

        // Use the recovered CustomerId to find the matching customer in Bank.Customers
        var bankCustomer = bank.GetCustomerById(customerDTO.CustomerId);

        // If the customer is not found, create a new bank customer using the recovered data
        if (bankCustomer == null)
        {
            bankCustomer = new BankCustomer(customerDTO.FirstName, customerDTO.LastName, customerDTO.CustomerId, bank);
            bank.AddCustomer(bankCustomer);
        }

        // Ensure the customer object includes all of the customer's accounts that have been backed up to file
        foreach (var accountNumber in customerDTO.AccountNumbers)
        {
            // Check if the account already exists in the customer's accounts
            var existingAccount = bankCustomer.Accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);

            if (existingAccount == null)
            {
                var accountFilePath = Path.Combine(accountsDirectoryPath, $"{accountNumber}.json");
                var recoveredAccount = LoadBankAccount(accountFilePath, transactionsDirectoryPath, bankCustomer);

                if (recoveredAccount != null)
                {
                    bankCustomer.AddAccount(recoveredAccount);
                }
            }
            else
            {
                bankCustomer.AddAccount(existingAccount);
            }
        }

        return bankCustomer;
    }

    public static IEnumerable<BankCustomer> LoadAllCustomers(Bank bank, string directoryPath, string accountsDirectoryPath, string transactionsDirectoryPath)
    {
        List<BankCustomer> customers = new List<BankCustomer>();
        foreach (var filePath in Directory.GetFiles(Path.Combine(directoryPath, "Customers"), "*.json"))
        {
            customers.Add(LoadBankCustomer(bank, filePath, accountsDirectoryPath, transactionsDirectoryPath));
        }
        return customers;
    }

    public static BankAccountDTO LoadBankAccountDTO(string filePath)
    {
        string json = File.ReadAllText(filePath);

        if (string.IsNullOrEmpty(json))
        {
            throw new Exception("No account found.");
        }
        else
        {
            var accountDTO = JsonSerializer.Deserialize<BankAccountDTO>(json, _options);
            if (accountDTO == null)
            {
                throw new Exception("Account could not be deserialized.");
            }
            return accountDTO;
        }

    }

    public static BankAccount LoadBankAccount(string accountFilePath, string transactionsDirectoryPath, BankCustomer customer)
    {
        // get the customer's existing accounts
        IEnumerable<IBankAccount> existingCustomerAccounts = customer.GetAllAccounts();

        // Prepare a transactions file path and a collection for recovered transactions
        string transactionsFilePath = "";
        IEnumerable<Transaction> recoveredTransactions = new List<Transaction>();

        // Load account data from a file using the provided file path (filename = AccountNumber.json)
        var accountDTO = LoadBankAccountDTO(accountFilePath); // accountDTO: AccountNumber, CustomerId, Balance, AccountType, InterestRate

        // Check if the account already exists in the customer's accounts

        bool accountExists = false;

        IBankAccount? returnAccount = null;

        foreach (var existingAccount in existingCustomerAccounts)
        {
            if (existingAccount.AccountNumber == accountDTO.AccountNumber)
            {
                returnAccount = existingAccount;
                accountExists = true;
                break;
            }
        }

        if (accountExists)
        {
            // Before returning the existing account, ensure that it contains the current transaction history
            transactionsFilePath = Path.Combine(transactionsDirectoryPath, $"{accountDTO.AccountNumber}-transactions.json");

            if (File.Exists(transactionsFilePath))
            {
                recoveredTransactions = LoadAllTransactions(transactionsFilePath);
            }

            // Compare the latest transaction date
            int finalExistingAccountTransaction = returnAccount.Transactions.Count() - 1;
            int finalRecoveredTransaction = recoveredTransactions.Count() - 1;

            if (returnAccount.Transactions.ElementAt(finalExistingAccountTransaction).TransactionDate < recoveredTransactions.ElementAt(finalRecoveredTransaction).TransactionDate)
            {
                foreach (var transaction in recoveredTransactions)
                {
                    // Add any missing transactions to the account's transaction history
                    if (!returnAccount.Transactions.Contains(transaction))
                    {
                        returnAccount.AddTransaction(transaction);
                    }
                }
            }

            return (BankAccount)returnAccount;
        }
        else
        {
            // If an existing account wasn't returned, create a new bank account using the recovered data
            var recoveredBankAccount = new BankAccount(customer, customer.CustomerId, accountDTO.Balance, accountDTO.AccountType);

            // Before returning the recovered account, ensure that it contains the current transaction history
            transactionsFilePath = Path.Combine(transactionsDirectoryPath, $"{accountDTO.AccountNumber}-transactions.json");

            recoveredTransactions = LoadAllTransactions(transactionsFilePath);
    
            foreach (var transaction in recoveredTransactions)
            {
                recoveredBankAccount.AddTransaction(transaction);
            }

            return recoveredBankAccount;
        }

        // LoadBankAccount isn't loading transactions for new accounts
        //throw new NotImplementedException("LoadBankAccount not implemented.");
    }

    public static IEnumerable<BankAccount> LoadAllAccounts(string directoryPath, string transactionsDirectoryPath, BankCustomer customer)
    {
        List<BankAccount> accounts = new List<BankAccount>();
        foreach (var filePath in Directory.GetFiles(Path.Combine(directoryPath, "Accounts"), "*.json"))
        {
            accounts.Add(LoadBankAccount(filePath, transactionsDirectoryPath, customer));
        }
        return accounts;
    }

    public static IEnumerable<Transaction> LoadAllTransactions(string filePath)
    {
        string jsonTransaction = File.ReadAllText(filePath);

        if (string.IsNullOrEmpty(jsonTransaction))
        {
            throw new Exception("No transactions loaded.");
        }
        else
        {
            var transactions = JsonSerializer.Deserialize<IEnumerable<Transaction>>(jsonTransaction, _options);
            if (transactions == null)
            {
                throw new Exception("Transactions could not be deserialized.");
            }

            return transactions;
        }
    }

    public static string ReturnAccountFilePath(string directoryPath, int accountNumber)
    {
        string? accountFilePath = null;

        // search the directoryPath directory for a file named with the accountNumber
        foreach (var filePath in Directory.GetFiles(Path.Combine(directoryPath, "Accounts"), "*.json"))
        {
            if (Path.GetFileNameWithoutExtension(filePath) == accountNumber.ToString())
            {
                accountFilePath = filePath;
            }
        }

        if (accountFilePath == null)
        {
            throw new Exception("Account file not found.");
        }

        return accountFilePath;
    }
}