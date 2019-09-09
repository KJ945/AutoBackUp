using System;
using System.Configuration;
using System.IO;
using System.Windows.Forms;

namespace AutoBackup
{
    static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
            AutoBackUp();
        }

        private static void AutoBackUp()
        {
            Configuration config = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            string From = ConfigurationManager.AppSettings["FromPath"].ToString();
            string[] Fromarray = From.Split(';');
            string ToDirectory = ConfigurationManager.AppSettings["ToPath"].ToString() + System.DateTime.Now.ToString("yyyyMMddHH") + "\\";
            string logpath = Application.StartupPath + "\\log_" + System.DateTime.Now.ToString("yyyyMMdd") + ".txt";
            string WriteWord = "";
            string Filter = ConfigurationManager.AppSettings["ext"];

            //建立備份目的資料夾
            if (!Directory.Exists(ToDirectory))
            {
                Directory.CreateDirectory(ToDirectory);
            }

            //建立Log檔
            if (!File.Exists(logpath))
            {
                File.Create(logpath).Close();
            }

            try
            {
                for (int i = 0; i < Fromarray.Length; i++)
                {
                    string FromDirectory = Fromarray[i];
                    string[] patterns = Filter.Split('|');

                    foreach (string pt in patterns)
                    {
                        string[] FileList = System.IO.Directory.GetFiles(FromDirectory, pt);

                        foreach (string File in FileList)
                        {
                            System.IO.FileInfo fi = new System.IO.FileInfo(File);
                            fi.CopyTo(ToDirectory + fi.Name, true);
                            WriteWord += System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:fff") + "\t" + fi.Name + "\t\t已成功備份至" + ToDirectory + "\r\n";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteWord += System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:fff") + "\t發生錯誤!!\r\n";
            }

            using (FileStream fs = new FileStream(logpath, FileMode.Append))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.WriteLine(WriteWord);
                }
            }
        }
    }
}
