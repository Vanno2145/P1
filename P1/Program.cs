using System;
using System.Runtime.InteropServices;

class Program
{
    
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern int MessageBox(IntPtr hWnd, string text, string caption, int type); 

    static void Main()
    {
        for (; ; )
        {
            Console.WriteLine("Загадайте число от 0 до 100 и нажмите Enter.");
            Console.ReadLine(); 

            int min = 0;
            int max = 100;
            int guess;
            string response = "";

            
            while (true)
            {
                guess = (min + max) / 2;
                Console.WriteLine($"Компьютер пытается угадать ваше число... Я думаю, что это {guess}");

                Console.WriteLine("Ответьте 'меньше', 'больше' или 'угадал':");
                response = Console.ReadLine().ToLower();

                if (response == "меньше")
                {
                    max = guess - 1;
                }
                else if (response == "больше")
                {
                    min = guess + 1;
                }
                else if (response == "угадал")
                {
                    
                    MessageBox((IntPtr)null, $"Компьютер угадал ваше число! Это {guess}.", "Поздравляем!", 0);
                    break;
                }
                else
                {
                    Console.WriteLine("Неверный ввод! Пожалуйста, введите 'меньше', 'больше' или 'угадал'.");
                }

                if (min > max)
                {
                    MessageBox((IntPtr)null, "Ваше число не в пределах от 0 до 100! Попробуйте еще раз.", "Ошибка!", 0);
                    break;
                }
            }

            Console.WriteLine("Хотите сыграть еще раз? (да/нет):");
            string playAgain = Console.ReadLine().ToLower();

            if (playAgain != "да")
            {
                break;
            }
        }
    }
}
