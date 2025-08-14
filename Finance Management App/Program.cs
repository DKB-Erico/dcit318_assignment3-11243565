using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance_Management_App
{
    public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

    public interface ITransactionProcessor
    {
        void Process(Transaction transaction);
    }

    public class BankTransferProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {

            Console.WriteLine($"Processing Bank Transfer of {transaction.Amount:C} for {transaction.Category}");
        }
    }

    public class MobileMoneyProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"Processing Mobile Money transaction of {transaction.Amount:C} for {transaction.Category}");
        }
    }

    public class CryptoWalletProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"Processing Crypto Wallet transaction of {transaction.Amount:C} for {transaction.Category}");
        }
    }

    public class Account
    {
        public string AccountNumber { get; }
        public decimal Balance { get; protected set; }

        public Account(string accountNumber, decimal initialBalance)
        {
            AccountNumber = accountNumber;
            Balance = initialBalance;
        }

        public virtual void ApplyTransaction(Transaction transaction)
        {
            Balance -= transaction.Amount;
        }
    }

    public sealed class SavingsAccount : Account
    {
        public SavingsAccount(string accountNumber, decimal initialBalance) : base(accountNumber, initialBalance) { }

        public override void ApplyTransaction(Transaction transaction)
        {
            if (transaction.Amount > Balance)
            {
                Console.WriteLine("Insufficient funds");
            }
            else
            {
                Balance -= transaction.Amount;
                Console.WriteLine($"Transaction successful for {transaction.Category}. Your new balance is {Balance:C}");
            }

        }
    }

    public class FinanceApp
    {
        private List<Transaction> _transactions = new List<Transaction>();

        public void Run()
        {
            // Instantiate SavingsAccount
            var account = new SavingsAccount("SAC0000000419", 350356);

            // Create three Transactions
            var t1 = new Transaction(001, DateTime.Now, 8000, "Investment");
            var t2 = new Transaction(002, DateTime.Now, 500, "Electricity");
            var t3 = new Transaction(003, DateTime.Now, 150000, "Tokens");

            //Process transactions
            var mobileProcessor = new MobileMoneyProcessor();
            var bankProcessor = new BankTransferProcessor();
            var cryptoProcessor = new CryptoWalletProcessor();

            mobileProcessor.Process(t1);
            bankProcessor.Process(t2);
            cryptoProcessor.Process(t3);

            //Apply transactions to account
            account.ApplyTransaction(t1);
            account.ApplyTransaction(t2);
            account.ApplyTransaction(t3);

            //Add all transactions to list
            _transactions.AddRange(new[] { t1, t2, t3 });
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var app = new FinanceApp();
            app.Run();

            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }
    }
}