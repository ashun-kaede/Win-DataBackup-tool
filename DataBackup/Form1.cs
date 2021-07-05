using System;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Timers;

namespace DataBackup
{
    public partial class Form1 : Form
    {
        private static System.Timers.Timer aTimer;
        private Boolean inTimer = false;
        private Boolean SyncStart = false;

        public Form1()
        {
            InitializeComponent();
            init();
        }

        //選擇目錄
        private void button1_Click(object sender, EventArgs e) 
        {
            FolderBrowserDialog pathIn = new FolderBrowserDialog();
            pathIn.ShowDialog();

            //將路徑寫入PathRecord.txt
            if (pathIn.SelectedPath.Length == 0)
            {
                label1.Text = "請選擇檔案 . . .";
                File.Delete("PathRecord.txt");
                using (StreamWriter sw = new StreamWriter("PathRecord.txt"))
                {
                    sw.WriteLine("None");
                }
            }
            else
            {
                label1.Text = pathIn.SelectedPath;
                File.Delete("PathRecord.txt");
                using (StreamWriter sw = new StreamWriter("PathRecord.txt"))
                {
                    sw.WriteLine(label1.Text);
                }
            }
        }

        //同步中按鈕
        private void button2_Click(object sender, EventArgs e)
        {
            
            if (button2.Text == "開啟同步" && label1.Text != "尚未選取")
            {
                button1.Enabled = false;
                SyncStart = true;
                button2.Text = "同步中!";
                listBox1.Items.Add("----------------同步已於 " + DateTime.Now.ToString("hh:mm:ss") + " 開起----------------" + "\n");
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
            }
            else if (button2.Text == "同步中!" && label1.Text != "尚未選取")
            {
                button1.Enabled = true;
                SyncStart = false;
                button2.Text = "開啟同步";
                listBox1.Items.Add("----------------同步已於 " + DateTime.Now.ToString("hh:mm:ss") + " 結束----------------" + "\n");
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
            }
            else
            {
                MessageBox.Show("請先選擇路徑");
            }
        }

        private void init()
        {
            //自動生成Data資料夾
            if (!Directory.Exists("Data"))
            {
                Directory.CreateDirectory("Data");
            }

            // 路徑記錄檔否存在?若不存在則自動生成
            if (!File.Exists("PathRecord.txt"))
                File.Create("PathRecord.txt").Close();

            // 讀取路徑記錄檔，若有資料則寫進按鈕文字
            using (StreamReader sr = new StreamReader("PathRecord.txt"))
            {
                String line;
                line = sr.ReadLine();

                try
                {
                    if (line != "None")
                    {
                        label1.Text = line;
                    }
                }
                catch
                {
                    label1.Text = "請選擇檔案 . . .";
                }
             }

            // Timer設置
            aTimer = new System.Timers.Timer(5000);
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
            aTimer.Start();

            listBox1.Items.Add("< 程式已於 " + DateTime.Now.ToString("hh:mm:ss") + " 開起 >" + "\n");
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            if (!inTimer && SyncStart)
            {
                inTimer = true;
                try
                {
                    string filePath = label1.Text;
                    ArrayList temp = new ArrayList();

                    // 取得被備份資料夾名單
                    if (Directory.Exists(filePath))
                        {
                            temp.Clear();
                            foreach (string x in Directory.GetFiles(filePath))
                            {
                                string fileName = Path.GetFileName(x);
                                temp.Add(fileName);
                            }
                        }

                        // 開始備份
                        foreach (string x in temp)
                        {
                           String  f = Path.Combine("Data", x);
                            if ( SyncStart)
                            {
                                if (!File.Exists(f))
                                {
                                    string sourceFile = Path.Combine(filePath, x);
                                    string destFile = Path.Combine("Data", x);
                                    File.Copy(sourceFile, destFile, true);
                                    this.Invoke(new EventHandler(delegate
                                    {
                                        listBox1.Items.Add(DateTime.Now.ToString("hh:mm:ss") + "   " + x + "  已備份");
                                        listBox1.SelectedIndex = listBox1.Items.Count - 1;
                                    }));
                            }
                        }
                            else
                            {
                                break;
                            }
                        }
                }
                catch
                {
                    Console.Write("file path lost");
                }
            }
            inTimer = false;
        }
    }
 }
