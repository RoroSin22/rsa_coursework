using System.Numerics;
using System.Security.Cryptography;


public static class PrimeGenerator
{
    public static BigInteger GenerateRandomPrime()
    {
        RandomNumberGenerator.Create();
        BigInteger candidate = new BigInteger(RandomNumberGenerator.GetInt32(300));

        if (candidate % 2 == 0)
            candidate++;

        while (!IsProbablePrime(candidate, 10)) 
            candidate += 2; 

        return candidate;
    }

    private static bool IsProbablePrime(BigInteger candidate, int iterations)
    {
        if (candidate <= 1) 
            return false;
        if (candidate <= 3) 
            return true;
        if (candidate % 2 == 0) 
            return false;

        BigInteger d = candidate - 1;
        int s = 0;
        while (d % 2 == 0)
        {
            d /= 2;
            s++;
        }

        for (int i = 0; i < iterations; i++)
        {
            BigInteger a = new BigInteger(RandomNumberGenerator.GetInt32(2, 298)); 
            a = a % (candidate - 1) + 1; 


            BigInteger x = BigInteger.ModPow(a, d, candidate);
            if (x == 1 || x == candidate - 1) 
                continue;

            for (int r = 1; r < s; r++)
            {
                x = BigInteger.ModPow(x, 2, candidate);

                if (x == candidate - 1) 
                    break;

                if (r == s - 1) 
                    return false;
            }

            return false;
        }

        return true;
    }
}

public class RSA
{
public static BigInteger Pow_(BigInteger value, BigInteger exponent)
    {
    BigInteger originalValue = value;
    while (exponent-- > 1)
        value = BigInteger.Multiply(value, originalValue);
    return value;
    }

    private static List<string> RSA_Endoce(string s, BigInteger e, BigInteger n)
    {
        List<string> result = new List<string>();

        BigInteger bi;

        for (int i = 0; i < s.Length; i++)
        {
            int index = (char)s[i];

            bi = new BigInteger(index);
            bi = Pow_(bi, e);

            bi = bi % n;

            result.Add(bi.ToString());
        }

        return result;
    }

    private static string RSA_Dedoce(List<string> input, BigInteger d, BigInteger n)
    {
        string result = "";

        BigInteger bi;

        foreach (string item in input)
        {
            
            bi = new BigInteger(Convert.ToDouble(item));
            bi = Pow_(bi, d);


            bi = bi % n;

            result += ((char) bi).ToString();
        }

        return result;
    }


    private static BigInteger[] Generate_pq()
    {
        BigInteger[] pq = new BigInteger[2];

        pq[0] = PrimeGenerator.GenerateRandomPrime();

        do
            pq[1] = PrimeGenerator.GenerateRandomPrime();
        while (pq[0] == pq[1]);

        return pq;
    }

    private static BigInteger Calculate_n(BigInteger[] pq)
    {
        return pq[0] * pq[1];
    }

    private static BigInteger Calculate_m(BigInteger[] pq)
    {
        return (pq[0] - 1) * (pq[1] - 1);
    }

    private static BigInteger Calculate_e(BigInteger m)
    {
        BigInteger e = m - 1;

        for (BigInteger i = 2; i <= m; i++)
        {
            if ((m % i == 0) && (e % i == 0)) 
            {
                e--;
                i = 1;
            }
        }

        return e;
    }

    private static BigInteger Calculate_d(BigInteger e, BigInteger m)
    {
        BigInteger d = e / 2;

        while (true)
        {
            if(((d * e) % m == 1) && (d != e))
                return d;
            d++;
        }
    }

    public static void Incode()
    {
        Console.WriteLine("Введите путь файлу, который необходимо зашировать");
        string path_original = Console.ReadLine();

        StreamReader sr = new StreamReader(path_original);
        
        string s = "";
        while (!sr.EndOfStream)
            s += sr.ReadLine();


        sr.Close();

        Console.WriteLine("Введите путь к папке, где будут создан зашифрованный файл");
        string path_end = Console.ReadLine() + "crypted.txt";

        Console.WriteLine("Вы хотите сгенерировать ключ шифрования или использовать готовый?");
        Console.WriteLine(@"Введите ""0"" для готового ключа");
        Console.WriteLine(@"Введите ""1"" для генерации ключа");

        string answer = "";
        while (answer != "0" & answer != "1")
            answer = Console.ReadLine();

        BigInteger e;
        BigInteger n;
        
        if (answer == "1")
        {   
            Console.WriteLine("Введите путь к папке, где будут созданы файлы с ключами");
            string path_key = Console.ReadLine();
            string path_open = path_key + "open.txt";
            string path_close = path_key + "close.txt";

            BigInteger[] pq = Generate_pq();
            n = Calculate_n(pq);
            BigInteger m = Calculate_m(pq);
            e = Calculate_e(m);
            BigInteger d = Calculate_d(e, m);

            StreamWriter open = new StreamWriter(path_open);
            open.WriteLine(e);
            open.WriteLine(n);
            open.Close();

            StreamWriter close = new StreamWriter(path_close);
            close.WriteLine(d);
            close.WriteLine(n);
            close.Close();
        }
        else
        {
            Console.WriteLine("Введите путь файлу с открытым ключом шифрования");
            string path_open = Console.ReadLine();

            StreamReader open = new StreamReader(path_open);
            e =  BigInteger.Parse(open.ReadLine());
            n = BigInteger.Parse(open.ReadLine());
            open.Close();
        }

        List<string> result = RSA_Endoce(s, e, n);

        StreamWriter sw = new StreamWriter(path_end);
        foreach (string item in result)
            sw.WriteLine(item);
        sw.Close();
    }

    public static void Decode()
    {
        Console.WriteLine("Введите путь файлу, который необходимо расшифровать");
        string path_crypted = Console.ReadLine();

        Console.WriteLine("Введите путь файлу с закрытым ключом шифрования");
        string path_close = Console.ReadLine();

        Console.WriteLine("Введите путь к папке, где будут создан дешифрованный файл");
        string path_end = Console.ReadLine() + "original.txt";

        StreamReader close = new StreamReader(path_close);
        BigInteger d = BigInteger.Parse(close.ReadLine());
        BigInteger n = BigInteger.Parse(close.ReadLine());
        close.Close();

        List<string> input = new List<string>();

        StreamReader sr = new StreamReader(path_crypted);

        while (!sr.EndOfStream)
            input.Add(sr.ReadLine());

        sr.Close();

        string result = RSA_Dedoce(input, d, n);

        StreamWriter sw = new StreamWriter(path_end);
        sw.WriteLine(result);
        sw.Close();
    }

    public static void Interface()
    {
        Console.WriteLine("Вы хотите зашифровать или расшифровать файл?");
        Console.WriteLine(@"Введите ""0"" для шифрования");
        Console.WriteLine(@"Введите ""1"" для дешифрования");

        string answer = "";
        while (answer != "0" & answer != "1")
            answer = Console.ReadLine();

        if (answer == "0")
            Incode();
        else
            Decode();

        Console.WriteLine("Процесс завершен");
    }
}

class Program
{
    public static void Main()
    {
        RSA.Interface();
    }

}