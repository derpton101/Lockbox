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
        const string pFileSuffix = ".pf";
        const string passFile = "ContainerPass.bin";
        const string lFileSuffix = ".lb";
        const string lFile = "Lockbox";
        static string[] fileContents;
        static string[] buffer;
        static string passHash;
        static string pass;
        static string currentOpen;
        const string folder = "lbs\\";

        static int ommitted;

        /// <summary>
        /// Changes size of [fileContents] using [buffer]
        /// </summary>

        static int getLineCount(string filename)
        {
            int x = 0;
            StreamReader r = new StreamReader(filename);
            while (r.ReadLine() != null) x++;
            r.Close();
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
        
        
        static void newPassword(string file)
        {
            if (File.Exists(file)) File.Delete(file);
            StreamWriter sw = new StreamWriter(file);
            bool isCorrect = false;
            while (!isCorrect)
            {
                Console.Write("Please input a new password\n:");
                string pass1 = Console.ReadLine();
                Console.Write("Please confirm password\n:");
                if (pass1 == Console.ReadLine())
                {
                    sw.WriteLine(hashString(pass1+genSalt(file))+':'+ genSalt(file));
                    sw.Flush();
                    sw.Close();
                    isCorrect = true;
                    pass = pass1;
                }
                else
                {
                    Console.WriteLine("Please Try again.\n");
                }
                
            }
            
        }
        static void getPassword(string file)
        {
            StreamReader s = new StreamReader(file);
            string hashSalt = s.ReadLine();
            string[] inter = hashSalt.Split(':');
            string passHash = inter[0];
            string salt = inter[1];
            s.Close();
            string password = "";
            while (!access)
            {

                Console.Write("Please input your password.\n:");
                password = Console.ReadLine();

                if (passHash == hashString(password+salt))
                {
                    access = true;
                }
                else
                {
                    Console.WriteLine("Please try again");
                }
            }
            pass = password;
            Program.passHash = passHash;
        }
        
        static string decrypt(int[] _in, string key)
        {
            char[] buff = new char[_in.Length];
            for (int i = 0; i < buff.Length-1; i++)
            {
                int toStrINT = _in[i];
                int hashChar = (int)key[i % key.Length];
                toStrINT = toStrINT / 3;
                toStrINT = (toStrINT - hashChar) / 2;
                //Console.Write(toStrINT);
                //Console.Write(" ");
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
        static void writeAll(string file)
        {
            File.Delete(file);
            StreamWriter w = new StreamWriter(file);
            if (fileContents == null) 
            {
                w.Close();
                return;
            }
            if (fileContents.Length == 0)
            {
                w.Close();
                return;
            }
            for (int i = 0; i < fileContents.Length; i++)
            {
                
                
                int[] encr = encrypt(fileContents[i], pass);
                int l = encr.Length;
                
                for(int j = 0; j < l; j++)
                {
                    w.Write((encr[j].ToString()));
                    if (j == l - 1) continue;
                    w.Write(":");
                }
                w.Write('\n');
            }
            w.Close();
        }
        
        private bool intHas(int[] arr, int a)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == a) return true;
            }
            return false;
        }
        static void readAll(string file)
        {
            StreamReader r = new StreamReader(file);
            int lines = getLineCount(file);
            int iter = 0;
            fileContents = new string[lines];
            string i = " ";
            
            while (i != null)
            {
                i = r.ReadLine();
                if (i == null) break; // HOW THE FUCK DID THIS GET HERE AND HOW DID WE GET HERE?
                string[] inter = i.Split(':');
                int[] interINT = new int[inter.Length];
                for (int k = 0; k < interINT.Length; k++)
                {
                    interINT[k] = Convert.ToInt32(inter[k]);
                    //Console.Write($"{inter[k]}.{interINT[k]}\n");
                }
                fileContents[iter] = decrypt(interINT, pass);
                iter++;
            }
            r.Close();
        }
        static void Main(string[] args)
        {
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            bool cont = true;
            while (cont)
            {
                Console.Write("Input name of lockbox file\n>>");
                currentOpen = Console.ReadLine();
                if (currentOpen == "quit") break;

                if (File.Exists(folder + currentOpen + pFileSuffix))
                {
                    // Retrieve password and compare it to already known hash
                    getPassword(folder + currentOpen +pFileSuffix);
                }
                else
                {
                    // Create new file
                    newPassword(folder + currentOpen +pFileSuffix);
                }
                //Console.WriteLine("");
                //string decryptTest = decrypt(encryptTest, pass);
                //Console.WriteLine($"Decrypted {decryptTest}");
                if (!File.Exists(folder + currentOpen + lFileSuffix))
                {
                    

                    StreamWriter init = new StreamWriter(folder+currentOpen+lFileSuffix);
                    //init.Write(0);
                    init.Close();
                }

                readAll(folder + currentOpen + lFileSuffix);
                bool cont1 = true;
                while (cont1) // COMMAND LINE MODE
                {
                    Console.Write("\n>");
                    string command = Console.ReadLine();
                    switch (command)
                    {
                        case "newpass":
                            getPassword(folder + currentOpen + pFileSuffix);
                            readAll(folder + currentOpen + lFileSuffix);
                            newPassword(folder + currentOpen + pFileSuffix);
                            writeAll(folder + currentOpen + lFileSuffix);
                            break;
                        case "add":
                            add();
                            break;
                        case "edit":
                            edit();
                            break;
                        case "omit":
                            ommit_CMD();
                            break;
                        case "list":
                            listall();
                            break;
                        case "open":
                            cont1 = false;
                            break;
                        case "quit":
                            cont = false;
                            cont1 = false;
                            break;
                        case "help":
                            Console.WriteLine("newpass\t\t-- set new password");
                            Console.WriteLine("add\t\t-- add new entry");
                            Console.WriteLine("edit\t\t-- edit entry via replace");
                            Console.WriteLine("omit\t\t-- remove entry");
                            Console.WriteLine("list\t\t-- list entries");
                            Console.WriteLine("open\t\t-- open different/new file");
                            Console.WriteLine("quit\t\t-- self explanitory");
                            Console.WriteLine("help\t\t-- prints this");
                            break;
                        default:
                            Console.WriteLine("Command doesn't exist.");
                            break;
                    }
                }
                writeAll(folder + currentOpen + lFileSuffix);
                access = false;
            }
        }
        static bool ommit_CMD()
        {
            if (fileContents == null)
            {
                Console.WriteLine("Empty");
                return true;
            }
            else
            {
                if (fileContents.Length == 0)
                {
                    Console.WriteLine("Empty");
                    return true;
                }
            }
            Console.WriteLine("Select what to ommit from storage.");
            listall();
            int i = Convert.ToInt32(Console.ReadLine());
            while (i == 0)
            {
                Console.WriteLine("Did you enter 0? if yes type \"y\"");
                if (Console.ReadLine() == "y") break;
                Console.Write("Please input a number.\nOMMIT>");
                i = Convert.ToInt32(Console.ReadLine());
            }
            buffer = new string[fileContents.Length-1];
            int iter = 0;
            foreach (string k in fileContents)
            {
                if (isOmmited(k, fileContents)) continue;
                buffer[iter] = k;
                iter++;
            }
            fileContents = new string[buffer.Length];
            for (int k = 0; k < buffer.Length; k++)
            {
                fileContents[k] = buffer[k];
            }
            return false;
        }
        static bool isOmmited(string queryString, string[] arr)
        {

            for (int i = 0; i < arr.Length; i++)
            {
                if (queryString == arr[i])
                {
                    if (i == ommitted) return true;
                    else return false;
                }
            }
            return false;
        }
        static bool listall()
        {
            Console.WriteLine("Listing All");
            int iter = 0;
            if (fileContents == null)
            {
                Console.WriteLine("Empty");
                return true;
            }
            else
            {
                if (fileContents.Length == 0)
                {
                    Console.WriteLine("Empty");
                    return true;
                }
            }
            foreach (string i in fileContents) {
                Console.WriteLine($"[{(iter).ToString()}] \t-- {i}");
                iter++;
            }
            return false;
        }
        static void add()
        {
            Console.Write("Write what to append to list\nADD>");
            string append = Console.ReadLine();
            if (fileContents == null)
            {
                fileContents = new string[1];
                fileContents[0] = append;
            }
            else
            {
                buffer = new string[fileContents.Length];
                for (int i = 0; i < fileContents.Length; i++)
                {
                    buffer[i] = fileContents[i];
                }
                fileContents = new string[buffer.Length + 1];
                for (int i = 0; i < buffer.Length; i++)
                {
                    fileContents[i] = buffer[i];
                }
                fileContents[fileContents.Length-1] = append;
            }
        }
        private static void edit()
        {
            if (listall()) return;
            Console.Write("Select one to replace\nEDIT>");
            int selection = Convert.ToInt32(Console.ReadLine());
            while (selection == 0)
            {
                Console.WriteLine("Did you enter 0? if yes type \"y\"");
                if (Console.ReadLine() == "y") break;
                Console.Write("Please input a number.\nEDIT>");
                selection = Convert.ToInt32(Console.ReadLine());
            }
            Console.Write($"REPLACE[{selection}]>");
            fileContents[selection % fileContents.Length] = Console.ReadLine();
        }
        


        private static string genSalt(string _in)
        {
            SHA512 x = SHA512.Create();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(_in);
            bs = x.ComputeHash(bs);
            StringBuilder s = new StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            return s.ToString();
        }

        private static string hashString(string _in)
        {
            SHA512 x = SHA512.Create();
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
