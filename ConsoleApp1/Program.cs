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
        static string decrypt(string _in)
        {
            string _out = _in;
            char[] buff = new char[_out.Length];
            buff[buff.Length - 1] = '\0';
            foreach (char i in _out)
            {

            }
            return _out;
        }
        static string encrypt(string _in)
        {
            string _out = _in;
            return _out;
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
            if (!File.Exists(lFile))
            {
                StreamWriter init = new StreamWriter(lFile);
                init.Write(0);
                init.Close();
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
