using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;

//Lockbox
namespace ConsoleApp1
{
    //Commands
    //



    class Program
    {
        
        static bool access = false;
        const string passFile = "ContainerPass.bin";
        const string lFile = "Lockbox";
        static string[] fileContents;
        static string[] buffer;
        static string pass;

        /// <summary>
        /// Changes size of [fileContents] using [buffer]
        /// </summary>

        static void changeSize()
        {

        }
        static int getLineCount(string filename)
        {
            int x = 0;
            StreamReader r = new StreamReader(filename);
            while (r.ReadLine() != null) x++;
            return x;
        }
        static void readFromFile(string file)
        {
            int lCount = getLineCount(file);
            fileContents = new string[lCount];
            StreamReader s = new StreamReader(file);
            for (int i = 0; i < lCount; i++)
            {
                string inp = s.ReadLine();
                if (inp == null) throw new IndexOutOfRangeException();
                fileContents[i] = s.ReadLine();
            }

        }
        static void newPassword()
        {
            if (File.Exists(passFile)) File.Delete(passFile);
            StreamWriter sw = new StreamWriter(passFile);
            bool isCorrect = false;
            while (!isCorrect)
            {
                Console.Write("Please input a new password\n:");
                string pass1 = Console.ReadLine();
                Console.Write("Please confirm password\n:");
                if (pass1 == Console.ReadLine())
                {
                    sw.WriteLine(hashString(pass1));
                    sw.Flush();
                    sw.Close();
                    isCorrect = true;
                }
                else
                {
                    Console.WriteLine("Please Try again.\n");
                }
            }
        }
        static void getPassword()
        {
            StreamReader s = new StreamReader(passFile);
            string passHash = s.ReadLine();
            s.Close();
            while (!access)
            {

                Console.Write("Please input your password.\n:");
                string password = Console.ReadLine();

                if (passHash == hashString(password))
                {
                    access = true;
                }
                else
                {
                    Console.WriteLine("Please try again");
                }
            }
            pass = passHash;
        }
        static void decryptAll()
        {
            buffer = new string[fileContents.Length];
            foreach (string i in fileContents) {

            }
        }
        static string decrypt(int[] _in, string key)
        {
            char[] buff = new char[_in.Length];
            buff[buff.Length - 1] = '\0';
            for (int i = 0; i < buff.Length-1; i++)
            {
                int toStrINT = _in[i];
                int hashChar = (int)key[i % key.Length];
                toStrINT = toStrINT / 3;
                toStrINT = (toStrINT - hashChar) / 2;
                buff[i] = (char)toStrINT;
            }
            string _out = new string(buff);
            return _out;
        }
        //ecryption alg (char*2 + hashChar) *3
        //decryprtion alg (char/3 - hashChar) /2

        static int[] encrypt(string _in, string key)
        {
            int[] buff = new int[_in.Length+1];
            buff[buff.Length - 1] = '\0';
            for (int i = 0; i < buff.Length-1; i++)
            {
                char toStr = _in[i];
                int hashChar = (int)key[i % key.Length];
                int toStrINT = (int)toStr;
                toStrINT = (toStrINT * 2) + hashChar;
                toStrINT = toStrINT * 3;
                buff[i] = (char)toStrINT;
            }
            return buff;
        }
        static void writeAll()
        {

        }
        static void Main(string[] args)
        {
            

            
            if (File.Exists(passFile))
            {
                // Retrieve password and compare it to already known hash
                getPassword();
            }
            else
            {
                // Create new file
                newPassword();
            }
            string _inp = "fuck";
            int[] encryptTest = encrypt(_inp, pass);
            Console.WriteLine($"Encrypted");
            foreach (int i in encryptTest)
            {
                Console.Write($"{i.ToString()}:");
            }
            //Console.WriteLine("");
            //string decryptTest = decrypt(encryptTest, pass);
            //Console.WriteLine($"Decrypted {decryptTest}");
            if (!File.Exists(lFile))
            {
                StreamWriter init = new StreamWriter(lFile);
                init.Write(0);
                init.Close();
            }
            while (true) // COMMAND LINE MODE
            {

            }
        }
        /// <summary>
        /// Hashes String in
        /// </summary>
        /// <param name="_in">String to hash</param>
        /// <returns>SHA256 Hashed String</returns>
        private static string hashString(string _in)
        {
            SHA256 x = SHA256.Create();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(_in);
            bs = x.ComputeHash(bs);
            StringBuilder s = new StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            return s.ToString();
        }
    }
}
