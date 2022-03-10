using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace 点名器
{
    internal class ListData
    {
        private string filehash;
        private List<int> keys;
        private List<string> names;
        private string filename;
        public ListData(string filename)//作为新文件输入
        {
            this.filename = filename;
            filehash = getHash(filename);
            names = new List<string>();
            keys = new List<int>();
            StreamReader sr = new StreamReader(filename, txtcodes.GetType123(filename));
            while (!sr.EndOfStream)
            {
                string s = sr.ReadLine();
                if (!string.IsNullOrEmpty(s))
                    names.Add(s);
            }
            sr.Close();
            for (int i = 0; i < names.Count() / 2; i++)
                keys.Add(-1);
        }
        public ListData(string filename, List<int> pkeys)//作为旧文件输入
        {
            this.filename = filename;
            filehash = getHash(filename);
            names = new List<string>();
            StreamReader sr = new StreamReader(filename, txtcodes.GetType123(filename));
            while (!sr.EndOfStream)
            {
                string s = sr.ReadLine();
                if (!string.IsNullOrEmpty(s))
                    names.Add(s);
            }
            sr.Close();
            keys = pkeys;
        }
        public static string getHash(string filename)
        {
            using (HashAlgorithm hash = HashAlgorithm.Create())
            {
                using (FileStream file1 = new FileStream(filename, FileMode.Open))
                {
                    byte[] hashByte1 = hash.ComputeHash(file1);
                    return BitConverter.ToString(hashByte1);
                }
            }
        }
        public string getName(int index)
        {
            return names[index];
        }
        public int getValid(int index)
        {
            while (keys.Contains(index))
                index = (index + 1) % names.Count;
            return index;
        }
        public void addKey(int key)
        {
            keys.RemoveAt(0);
            keys.Add(key);
        }
        public string toString()
        {
            string output = filename + "\n" + filehash + "\n";
            for (int i = 0; i < keys.Count; i++)
                output += keys[i].ToString() + " ";
            return output + "\n";
        }
    }
}
