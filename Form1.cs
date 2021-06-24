using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace 点名器
{
    public partial class Form1 : Form
    {
        static int flag,number;
        static int[] past = new int[17000];
        static bool DoCheck = false;
        public Form1()
        {
            InitializeComponent();
        }
        public static bool Change(int in1)
        {
            if (in1 >=0) return false;
            else return true;
        }
        public bool Checkhave(int num)
        {
            for(int i = 0; i <= listBox1.Items.Count - 1;i++)
                if (past[i] == num) return true;
            return false;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "文本文档(*.txt)|*.txt";
            ofd.ShowDialog();
            string s = ofd.FileName;
            checkedListBox1.Items.Add(s);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (checkedListBox1.SelectedIndex >= 0) checkedListBox1.Items.RemoveAt(checkedListBox1.SelectedIndex);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            foreach (string s in checkedListBox1.CheckedItems)
            {
                if (!File.Exists(s))
                {
                    MessageBox.Show("文件" + s + "不存在！");
                    continue;
                }
                StreamReader sr = new StreamReader(s, txtcodes.GetType123(s));
                string name = sr.ReadLine();
                while (name!=null&&name!="")
                {
                    listBox1.Items.Add(name);
                    name = sr.ReadLine();
                }
                sr.Close();
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            number = number + 1;
            if (number == 2) timer1.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;//忽略文档里面的注释
            XmlReader reader = XmlReader.Create(Application.StartupPath + @"\path.xml", settings);
            xmlDoc.Load(reader);
            XmlNode xn = xmlDoc.SelectSingleNode("Data");
            XmlNodeList xnl = xn.ChildNodes;

            XmlElement xe = (XmlElement)xnl[0];
            XmlNodeList xnl0 = xe.ChildNodes;
            foreach(XmlNode xml in xnl0)
            {
                if(xml.ChildNodes[1].InnerText=="True")
                    checkedListBox1.Items.Add(xml.ChildNodes[0].InnerText, CheckState.Checked);
                else
                    checkedListBox1.Items.Add(xml.ChildNodes[0].InnerText, CheckState.Unchecked);
            }

            xe = (XmlElement)xnl[1];
            if (xe.InnerText != "True") DoCheck = false;
            else DoCheck = true;
            reader.Close();
            button4_Click(sender, e);

            timer2.Enabled = true;
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.AppendChild(xmlDocument.CreateXmlDeclaration("1.0", "utf-8", null));
            XmlElement xmlRoot = xmlDocument.CreateElement("Data");

            XmlComment xmlComment = xmlDocument.CreateComment("名单路径");
            XmlElement xmlChild = xmlDocument.CreateElement("Filepath");
            xmlChild.AppendChild(xmlComment);
            for(int i=0;i<checkedListBox1.Items.Count;i++)
            {
                XmlElement data = xmlDocument.CreateElement("Data" + i.ToString());
                XmlElement data1 = xmlDocument.CreateElement("Path");
                data1.InnerText = (string)checkedListBox1.Items[i];
                XmlElement data2 = xmlDocument.CreateElement("Checked");
                data2.InnerText = checkedListBox1.GetItemChecked(i).ToString();
                data.AppendChild(data1);
                data.AppendChild(data2);
                xmlChild.AppendChild(data);
            }
            xmlRoot.AppendChild(xmlChild);

            XmlElement setting = xmlDocument.CreateElement("Setting");
            setting.InnerText = DoCheck.ToString();
            xmlRoot.AppendChild(setting);

            xmlDocument.AppendChild(xmlRoot);
            xmlDocument.Save("path.xml");

            System.Environment.Exit(0);
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            timer2.Enabled = false;

        }
        Size min = new Size(501, 405);
        Size max = new Size(955, 462);
        private void button5_Click(object sender, EventArgs e)
        {
            Size size = this.Size;
            if (size == min) { this.Size = max; this.TopMost = false; }
            else { this.Size = min;this.TopMost = true; }
        }

        private void checkedListBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            button4_Click(sender, e);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (flag == listBox1.Items.Count)
            {
                MessageBox.Show("都已经轮到过了");
                return;
            }
            if (listBox1.Items.Count == 0) MessageBox.Show("你还木有加载名单哦~");
            else
            {
                int count=0;
                Random rd = new Random(DateTime.Now.Millisecond);
                for (int i = 0; i <= 10; i++)
                {
                    count = rd.Next(0, listBox1.Items.Count);
                    label1.Text=listBox1.Items[count].ToString();
                    number = 1;
                    timer1.Enabled = true;
                    while (timer1.Enabled == true) Application.DoEvents();
                }
                while (Checkhave(count + 1)){
                    Application.DoEvents();
                    count += 1;
                    if (count >= listBox1.Items.Count) count = 0;
                    label1.Text = listBox1.Items[count].ToString();
                }

                past[flag] = count + 1;
                flag += 1;
            }
        }
    }
}
