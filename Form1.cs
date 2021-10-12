using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace 点名器
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        List<ListData> data = new List<ListData>();
        public static string configPath = Application.StartupPath + @"\settings.config";

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "文本文档(*.txt)|*.txt";
            ofd.ShowDialog();
            string s = ofd.FileName;
            listBox2.Items.Add(s);
            data.Add(new ListData(s));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex >= 0)
            {
                data.RemoveAt(listBox2.SelectedIndex);
                listBox2.Items.RemoveAt(listBox2.SelectedIndex);
            }
        }
        int number = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            number = number + 1;
            if (number == 2) timer1.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count == 0) { 
                MessageBox.Show("你还木有选择名单哦~"); 
                return; 
            }
            int count;
            Random rd = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i <= 10; i++)
            {
                count = rd.Next(0, listBox1.Items.Count);
                label1.Text = listBox1.Items[count].ToString();
                number = 1;
                timer1.Enabled = true;
                while (timer1.Enabled == true) Application.DoEvents();
            }
            count = rd.Next(0, listBox1.Items.Count);
            int final = data[listBox2.SelectedIndex].getValid(count);
            data[listBox2.SelectedIndex].addKey(final);
            label1.Text = data[listBox2.SelectedIndex].getName(final);
        }

        private void listBox2_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex < 0) return;
            listBox1.Items.Clear();
            string s = (string)listBox2.Items[listBox2.SelectedIndex];
            if (!File.Exists(s))
            {
                MessageBox.Show("文件" + s + "不存在！");
                return;
            }
            StreamReader sr = new StreamReader(s, txtcodes.GetType123(s));
            string name = sr.ReadLine();
            while (name != null && name != "")
            {
                listBox1.Items.Add(name);
                name = sr.ReadLine();
            }
            sr.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (File.Exists(configPath))
            {
                StreamReader sr = new StreamReader(configPath, txtcodes.GetType123(configPath));
                try
                {
                    listBox2.Items.Clear();
                    int filecnt = int.Parse(sr.ReadLine());
                    for (int i = 0; i < filecnt; i++)
                    {
                        string filename = sr.ReadLine();
                        string hash = sr.ReadLine();
                        string key = sr.ReadLine();
                        if (ListData.getHash(filename) == hash)//确定为目标文件
                        {
                            List<int> keys = new List<int>();
                            var temp = key.Split(' ');
                            foreach (string k in temp)
                                if (!string.IsNullOrEmpty(k))
                                    keys.Add(int.Parse(k));
                            data.Add(new ListData(filename, keys));
                        }
                        else//文件发生改动
                            data.Add(new ListData(filename));
                        listBox2.Items.Add(filename);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                sr.Close();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            StreamWriter sw = new StreamWriter(configPath);
            sw.WriteLine(data.Count);
            for (int i = 0; i < data.Count; i++)
                sw.Write(data[i].toString());
            sw.Close();
        }
    }
}
