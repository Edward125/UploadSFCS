using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Web;
using System.Drawing;
using System.Diagnostics;



namespace UploadSFCS
{
    class Program
    {
        #region 參數

        static string iniFilePath = System.Windows.Forms.Application.StartupPath + @"\UploadSFCS.ini";
        static string webSite = @"http://10.62.201.35/Tester.WebService/WebService.asmx";        
        static  PCB_Web.WebService ws = new PCB_Web.WebService();
     
        //SFCS
        static string PPID = string.Empty;
        static string LINE = string.Empty;
        static string STAGE = "TA";
        static string OPID = "K12345678";
        static bool PassFlag = true;
        static string[] trndata = new string[1]; //上拋SFCS的附件信息,=ERRORCODE
        static string FIXTUREID = string.Empty;
        

        #endregion
        static void Main(string[] args)
        {

            //check ini 
            InitIniFile();
            displayMsg();
            checkfolder_file();
            //check web
            if (!checkWebSite(webSite))
            {
                //error

            }


            Console.WriteLine(webSite);

            if (args.Length == 7)
            {
                PPID = args[0].ToUpper().Trim();
                LINE = args[1].ToUpper().Trim();
                STAGE = args[2].Trim().ToUpper();
                OPID = args[3].Trim().ToUpper();
                if (args[4].ToUpper().Trim() == "PASS")
                {
                    PassFlag = true;
                    trndata[0] = args[0].ToUpper().Trim();
                }
                if (args[4].ToUpper().Trim() == "FAIL")
                {
                    PassFlag = false;
                    trndata[0] = args[5].ToUpper().Trim();
                }
                
                FIXTUREID = args[6].ToUpper().Trim();

                string result = string.Empty;
                result = ws.UploadFixtureID(PPID, STAGE, FIXTUREID);
                result  = ws.Complete (PPID,LINE,STAGE,STAGE,OPID,PassFlag,trndata );
                if (result == "OK")
                {
                    SaveLog(PPID + "->upload OK");
                    Console.WriteLine(DateTime.Now.ToString("yyyyMMddHHmmss") + " " + PPID + "->upload OK");
                }
                else
                {
                    SaveLog(PPID + "->upload NG," + result);
                    Console.WriteLine(DateTime.Now.ToString("yyyyMMddHHmmss") + " " + PPID + "->upload NG," + result);
                }

               // KillProcess();
                KillAllProcess("ksh");
            }
            else 
            {
               // displayMsg();
            }



         // Console.ReadKey();


        }


        private static  void InitIniFile()
        {
            // check file exits
            if (!File.Exists(iniFilePath))
            {
                IniFile.CreateIniFile(iniFilePath);
                IniFile.IniWriteValue("SysConfig", "WebSite", "http://10.62.201.35/Tester.WebService/WebService.asmx", iniFilePath);
            }
            else
            {
                string tempWeb = IniFile.IniReadValue("SysConfig", "WebSite", iniFilePath);
                if (!string.IsNullOrEmpty(tempWeb))
                    webSite = tempWeb;
            }

        }

        private static  bool checkWebSite(string website)
        {
            ws.Url = webSite;
            try
            {
                ws.Discover();
            }
            catch (Exception ex )
            {
                Console.WriteLine( DateTime.Now.ToString ("yyyyMMddHHmmss") + " Connect WebService error.");
                Console.WriteLine(ex.Message);
                Console.WriteLine(DateTime.Now.ToString("yyyyMMddHHmmss") + " pls check the websit or network.");                
                return false;
            }            
            Console.WriteLine(DateTime.Now.ToString("yyyyMMddHHmmss") + " Connect WebService ok.");
            return true;
        }

        public static void SaveLog(string logcontent)
        {
            string appFolder = Application.StartupPath + @"\UploadSFCS";
            string filePath = appFolder + @"\" + @DateTime.Now.ToString("yyyyMMdd") + ".txt";
            string log = DateTime.Now.ToString("yyyyMMddHHmmss") + " " + logcontent + "\r\n";
            //check folder & file
            if (!Directory.Exists(appFolder))
                Directory.CreateDirectory(appFolder);
            if (!File.Exists(filePath))
            {
                FileStream fs = File.Create(filePath);
                fs.Close();
            }
            else
            {
                File.AppendAllText(@filePath, @log);
            }

        }



        public static void checkfolder_file()
        {
            string appFolder = Application.StartupPath + @"\UploadSFCS";
            string filePath = appFolder + @"\" + @DateTime.Now.ToString("yyyyMMdd") + ".txt";
            //string log = DateTime.Now.ToString("yyyyMMddHHmmss") + " " + logcontent + "\r\n";
            //check folder & file
            if (!Directory.Exists(appFolder))
                Directory.CreateDirectory(appFolder);
            if (!File.Exists(filePath))
            {
                FileStream fs = File.Create(filePath);
                fs.Close();
            }
        }

        public static void displayMsg()
        {
            Console.WriteLine("**************************************************************************");
            Console.WriteLine("*    UploadSFCS use command via webservice                               *");
            Console.WriteLine("*    Ver:1.0.0.0, Author:WCD ATE Edward_song@yeah.net                    *");
            Console.WriteLine("**************************************************************************");
            Console.WriteLine("*    Command List:                                                       *");
            Console.WriteLine("*    UploadSFCS.exe PPID  Line Stage OPID PassFlag ErrorCode FixtureID   *");
            Console.WriteLine("*    PPID:the MB ppid or sn,eg:CN12345678901234567890                    *");
            Console.WriteLine("*    Line:pcb line name,eg:AP2                                           *");
            Console.WriteLine("*    Stage:stage code,eg:TA                                              *");
            Console.WriteLine("*    OPID:OP id,eg:D1203ABJ0                                             *");
            Console.WriteLine("*    PassFlage:the MB test result,pass or fail,eg:pass                   *");
            Console.WriteLine("*    ErrorCode:the MB test result code,eg:AT01 or 0000                   *");
            Console.WriteLine("*    FixtureID:the fixture id,eg:15203-1-L                               *");
            Console.WriteLine("**************************************************************************");
        }


        public static void KillProcess()
        {
            System.Diagnostics.Process[] excelProcess = System.Diagnostics.Process.GetProcessesByName("ksh");
            foreach (System.Diagnostics.Process p in excelProcess)
                p.Kill();
        }

        public static void KillAllProcess(string processname)
        {
            System.Diagnostics.Process[] process;
            process = Process.GetProcesses();
            foreach (Process p in process)
            {
                if (p.ProcessName == processname)
                    p.Kill();
            }
        }

    }
}
