using System;
using System.Text;

namespace LWE_Problem
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Введите параметр N:");
            int n = int.Parse(Console.ReadLine());
            Console.WriteLine("Введите модуль q (от n^2 до 2n^2):");
            int q = int.Parse(Console.ReadLine());
            if (q < n * n || q > 2 * n * n)
            {
                return;
            }
            RegevCryptosystem cryptosystem = new RegevCryptosystem(n, q);
            cryptosystem.PrintParams();
            Console.WriteLine("Введите сообщение:");
            string message = Console.ReadLine();
            RegevCryptosystem.VectorIntPair[] encrypted = cryptosystem.Encrypt(message);
            Console.WriteLine("Шифртекст:");
            for (int i = 0; i < encrypted.Length; i++)
            {
                Console.WriteLine("\t{0}. {1}", i + 1, encrypted[i].ToString());
            }
            string decrypted = cryptosystem.Decrypt(encrypted);
            Console.WriteLine("Открытый текст:");
            Console.WriteLine(decrypted);
        }
    }
}
