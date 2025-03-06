using System;
using System.Threading;

class Bank
{
    private decimal balance;
    private readonly object locker = new object();

    public Bank(decimal initialBalance)
    {
        balance = initialBalance;
    }

    public bool Withdraw(decimal amount)
    {
        bool success = false;
        Monitor.Enter(locker);
        try
        {
            if (balance >= amount)
            {
                Console.WriteLine($"{Thread.CurrentThread.Name} снял {amount:C}. Баланс до: {balance:C}");
                Thread.Sleep(100); // Симуляция задержки транзакции
                balance -= amount;
                Console.WriteLine($"Баланс после: {balance:C}");
                success = true;
            }
            else
            {
                Console.WriteLine($"{Thread.CurrentThread.Name}: Недостаточно средств!");
            }
        }
        finally
        {
            Monitor.Exit(locker);
        }
        return success;
    }
}

class Program
{
    static void Main()
    {
        Bank bank = new Bank(500); // Начальный баланс
        Thread[] atms = new Thread[5];

        for (int i = 0; i < atms.Length; i++)
        {
            atms[i] = new Thread(() =>
            {
                Random rand = new Random();
                for (int j = 0; j < 3; j++)
                {
                    bank.Withdraw(rand.Next(50, 200));
                }
            });
            atms[i].Name = $"Банкомат {i + 1}";
            atms[i].Start();
        }

        foreach (var atm in atms)
        {
            atm.Join();
        }
    }
}
