using System;
using System.IO;
using System.Threading;
using System.Collections;
using System.Text;
using System.Data;
 
using System.Diagnostics;
using System.Globalization;
namespace My
{
	/// <summary>
	/// FunLog ªººK­n´y­z¡C
	/// </summary>
	public class FunLog
	{
		public static DateTime TimeCount = DateTime.Now ;
        
		public FunLog()
		{
			
		}
         

        public static bool GetIsClose(string DateTime, string EndTime)
        {
            bool IsEnd = DateTime.Substring(11).CompareTo(EndTime) != -1;
            bool IsCloseDate = DateTime.Substring(0, 10) == FunLog.GetCloseDate(DateTime);
            return IsEnd && IsCloseDate;
        }

        public static string GetCloseDate(string DateTimes)
        {
            DateTime lTimeS=System.DateTime.Parse(DateTimes);
            DateTime lTime = new DateTime(lTimeS.Year, lTimeS.Month, 1);
            int Year = lTime.Year;
            bool End = true;
            int Wednesday = 0;
            DateTime CloseDate = DateTime.Now;
            while (End)
            {
                if (lTime.DayOfWeek == DayOfWeek.Wednesday) Wednesday++;
                if (Wednesday == 3)
                {
                    CloseDate = lTime;
                    End = false;
                }
                lTime = lTime.AddDays(1);

            }
            return CloseDate.ToString("yyyy/MM/dd");
        }

        public static void WriteLogClear(string log, string FolderPath)
        {
            string Pathfile = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string AllPath = Pathfile.Substring(0, Pathfile.LastIndexOf("\\"));
            FolderPath = AllPath + "\\" + FolderPath;
            FunLog.checkFolderExist(FolderPath);
            string lLogName = FolderPath + "\\" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "_" + DateTime.Now.Day.ToString().PadLeft(2, '0') + ".txt";
            try
            {
                StreamWriter Output = new StreamWriter(lLogName, true, System.Text.Encoding.GetEncoding("BIG5"));
                Output.WriteLine(log);
                Output.Close();
            }
            catch (System.Exception ex)
            {

            }
        }         

        public static ArrayList File2ArrayList(string fileName)
        {
            string Pathfile = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string AllPath = Pathfile.Substring(0, Pathfile.LastIndexOf("\\"));
            fileName = AllPath + "\\" + fileName;
            StreamReader sr;
            try
            {
                sr = new StreamReader(fileName, System.Text.Encoding.GetEncoding("BIG5"));
            }
            catch (System.Exception ex1)
            {
                string err = ex1.Message;
                Thread.Sleep(5000);

                sr = new StreamReader(fileName, System.Text.Encoding.GetEncoding("BIG5"));
            }
            string values = "";
            try
            {
                values = sr.ReadToEnd();
                sr.Close();
            }
            catch (System.Exception ex)
            {

            }
            string[] split={"\r\n"};
            string[] split2 ={","};
            string[] strs = values.Split(split, StringSplitOptions.None);
            ArrayList lList = new ArrayList();
            bool IsTitle = true;
            foreach (string str in strs) {                
                if (str.Trim() != "" && !IsTitle)
                {
                    string[] str2 = str.Split(split2, StringSplitOptions.None);
                    str2[0] = str2[0].Replace("\"", "");
                    lList.Add(str2);
                }
                IsTitle = false;
            }
            return lList;
        }

        public static ArrayList File2ArrayList2(string fileName)
        {
            string Pathfile = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string AllPath = Pathfile.Substring(0, Pathfile.LastIndexOf("\\"));
            fileName = AllPath + "\\" + fileName;
            StreamReader sr;
            try
            {
                sr = new StreamReader(fileName, System.Text.Encoding.GetEncoding("BIG5"));
            }
            catch (System.Exception ex1)
            {
                string err = ex1.Message;
                Thread.Sleep(5000);

                sr = new StreamReader(fileName, System.Text.Encoding.GetEncoding("BIG5"));
            }
            string values = "";
            try
            {
                values = sr.ReadToEnd();
                sr.Close();
            }
            catch (System.Exception ex)
            {

            }
            string[] split = { "\n" };
            string[] split2 = { "\",\"" };
            string[] strs = values.Split(split, StringSplitOptions.None);
            ArrayList lList = new ArrayList();
            bool IsTitle = true;
            foreach (string str in strs)
            {
                if (str.Trim() != "" && !IsTitle)
                {
                    //
                    string[] str2 = str.Split(split2, StringSplitOptions.None);
                    str2[0] = str2[0].Replace("\"", "").Replace(",", "").Replace(",", "").Replace("=", "");
                    lList.Add(str2);
                }
                IsTitle = false;
            }
            return lList;
        }
        public static ArrayList str2ArrayList(string values)
        {
            string[] split = { "\r\n" };
            string[] split2 = { "," };
            string[] strs = values.Split(split, StringSplitOptions.None);
            ArrayList lList = new ArrayList();
            bool IsTitle = true;
            foreach (string str in strs)
            {
                if (str.Trim() != "" && !IsTitle)
                {
                    string[] str2 = str.Split(split2, StringSplitOptions.None);
                    str2[0] = str2[0].Replace("\"", "");
                    if (str2.Length >= 18)
                    {
                        if (str2[17] == "¤@¯ë")
                            lList.Add(str2);
                    }
                }
                IsTitle = false;
            }
            return lList;
        }
        public static void OpenFolder()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("explorer.exe");
            string Pathfile = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string AllPath = Pathfile.Substring(0, Pathfile.LastIndexOf("\\"));
            startInfo.Arguments = AllPath;
            Process.Start(startInfo);
        } 

        public static string ReadFile2(string fileName)
        {
            string Pathfile = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string AllPath = Pathfile.Substring(0, Pathfile.LastIndexOf("\\"));
            fileName = AllPath + "\\" + fileName;
            StreamReader sr;
            try
            {
                sr = new StreamReader(fileName, System.Text.Encoding.GetEncoding("BIG5"));
            }
            catch (System.Exception ex1)
            {
                string err = ex1.Message;
                Thread.Sleep(5000);

                sr = new StreamReader(fileName, System.Text.Encoding.GetEncoding("BIG5"));
            }
            string values = "";
            try
            {
                values = sr.ReadToEnd();
                sr.Close();
            }
            catch (System.Exception ex)
            {

            }
            return values;
        }

        public static string ReadFile(string fileName)
        {
            
            StreamReader sr;
            try
            {
                sr = new StreamReader(fileName, System.Text.Encoding.GetEncoding("BIG5"));
            }
            catch (System.Exception ex1)
            {
                string err = ex1.Message;
                Thread.Sleep(5000);

                sr = new StreamReader(fileName, System.Text.Encoding.GetEncoding("BIG5"));
            }
            string values = "";
            try
            {
                values = sr.ReadToEnd();
                sr.Close();
            }
            catch (System.Exception ex)
            {

            }
            return values;
        }

        public static string SysPath() {
            string Pathfile = System.Reflection.Assembly.GetExecutingAssembly().Location;
            return Pathfile.Substring(0, Pathfile.LastIndexOf("\\"));
        }

        public static string FilePath(string fileName)
        {
            string Pathfile = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string AllPath = Pathfile.Substring(0, Pathfile.LastIndexOf("\\"));
            string Path = AllPath + "\\" + fileName;
            return Path;
        }

        public static void WriteFile(string fileName, string str)
        {
            string Pathfile = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string AllPath = Pathfile.Substring(0, Pathfile.LastIndexOf("\\"));
            string Path = AllPath + "\\" + fileName;
            try
            {
                StreamWriter Output = new StreamWriter(Path,false, System.Text.Encoding.GetEncoding("BIG5"));
                Output.WriteLine(str);
                Output.Close();
            }
            catch (System.Exception ex)
            {
                string xx = ex.Message;
            }
        }

        public static void WriteFileAppend(string fileName, string str)
        {
            string Pathfile = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string AllPath = Pathfile.Substring(0, Pathfile.LastIndexOf("\\"));
            string Path = AllPath + "\\" + fileName;
            try
            {
                StreamWriter Output = new StreamWriter(Path, true, System.Text.Encoding.GetEncoding("BIG5"));
                Output.WriteLine(str);
                Output.Close();
            }
            catch (System.Exception ex)
            {

            }
        }

        public static void WriteLogEachFile(string folderName, string str)
        {
            string Pathfile = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string AllPath = Pathfile.Substring(0, Pathfile.LastIndexOf("\\"));
            FunLog.checkFolderExist(AllPath + "\\" + folderName);
            string Path = AllPath + "\\" + folderName +"\\" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".html";
            try
            {
                StreamWriter Output = new StreamWriter(Path, false, System.Text.Encoding.GetEncoding("BIG5"));
                Output.Write(str);
                Output.Close();
            }
            catch (System.Exception ex)
            {

            }
        }

		public static void WriteLog(string log,string FolderPath){
           
            string Pathfile = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string AllPath = Pathfile.Substring(0, Pathfile.LastIndexOf("\\"));

			FolderPath=AllPath+"\\" + FolderPath;
			FunLog.checkFolderExist(FolderPath);
			string  lLogName=FolderPath+"\\"+DateTime.Now.Month.ToString().PadLeft(2,'0')+"_"+DateTime.Now.Day.ToString().PadLeft(2,'0')+".txt";
            try
            {
                StreamWriter Output = new StreamWriter(lLogName, true, System.Text.Encoding.GetEncoding("BIG5"));
                Output.WriteLine("[" + DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString().PadRight(3,'0') + "]" + log);
                Output.Close();
            }
            catch (System.Exception ex) {
                string ss = ex.Message;
            }
		}

        public static void CopyFile(string sourceFolder,string File, string dstFolder)
        {

            string Pathfile = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string AllPath = Pathfile.Substring(0, Pathfile.LastIndexOf("\\"));

            string FolderPath = AllPath + "\\" + dstFolder;
            FunLog.checkFolderExist(FolderPath);           
            try
            {
                System.IO.File.Move(sourceFolder + "\\" + File, dstFolder + "\\" + File);
            }
            catch (System.Exception ex)
            {
                string ss = ex.Message;
            }
        }

		public static void Write2OneFile(string log,string FolderPath,string SubFileName)
		{
            string Pathfile = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string AllPath = Pathfile.Substring(0, Pathfile.LastIndexOf("\\"));
            FolderPath = AllPath + "\\" + FolderPath +"\\";

            DateTime lTime = DateTime.Now;
			FunLog.checkFolderExistAll(FolderPath);
            string lLogName = FolderPath + lTime.Month.ToString().PadLeft(2, '0') + "_" + lTime.Day.ToString().PadLeft(2, '0') + "_" + lTime.TimeOfDay.TotalSeconds.ToString().PadLeft(7, '0') + SubFileName;
			StreamWriter Output = new StreamWriter(lLogName,true,System.Text.Encoding.GetEncoding("BIG5"));
			Output.Write(log); 			
			Output.Close() ;			
		}
        public static void Write2OneFile(string log, string FilePath)
        {
            try
            {
                FunLog.checkFolderExistAll(FilePath);
                StreamWriter Output = new StreamWriter(FilePath, false, System.Text.Encoding.GetEncoding("BIG5"));
                Output.Write(log);
                Output.Close();
            }
            catch (System.Exception ex)
            {
                string mess = ex.Message;
            }
        }

        public static void Write2OneFileAppend(string log, string FilePath)
        {
            try
            {
                FunLog.checkFolderExistAll(FilePath);
                StreamWriter Output = new StreamWriter(FilePath, true, System.Text.Encoding.GetEncoding("BIG5"));
                Output.WriteLine(log);
                Output.Close();
            }
            catch (System.Exception ex)
            {
                string mess = ex.Message;
            }
        }

		
		public static void Trace(string msg){
			TimeSpan lTime = DateTime.Now-FunLog.TimeCount;
			FunLog.TimeCount=DateTime.Now;
            string Pathfile = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string AllPath = Pathfile.Substring(0, Pathfile.LastIndexOf("\\"));
            FunLog.checkFolderExist(AllPath + "\\Trace");
            string lLogName = AllPath + "\\Trace" + "\\" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "_" + DateTime.Now.Day.ToString().PadLeft(2, '0') + ".txt";
			StreamWriter Output = new StreamWriter(lLogName,true,System.Text.Encoding.GetEncoding("BIG5"));
			Output.WriteLine  ("["+DateTime.Now.ToString()+":"+DateTime.Now.Millisecond.ToString()+"]"+"["+lTime.TotalMilliseconds.ToString()+"]"+msg); 			
			Output.Close() ;		
		}

        public static void checkFolderExistAll(string FolderPath)
        {
            int startint = 0;
            while (FolderPath.IndexOf("\\", startint) > 0)
            {
                string checkPath = FolderPath.Substring(0, FolderPath.IndexOf("\\", startint));
                startint = FolderPath.IndexOf("\\", startint) + 2;
                if (!Directory.Exists(checkPath))
                {
                    try
                    {

                        Directory.CreateDirectory(checkPath);
                    }
                    catch (System.Exception ex) { }
                }
                if (startint > FolderPath.Length)
                {

                    return;
                }
            }

        }

        public static bool checkFileExist(string file ){
            string Pathfile = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string AllPath = Pathfile.Substring(0, Pathfile.LastIndexOf("\\"));
            file = AllPath + "\\" + file;
            return File.Exists(file);
        }

        public static bool checkFileExist2(string Folder, string file)
        {
            string Pathfile = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string AllPath = Pathfile.Substring(0, Pathfile.LastIndexOf("\\"));
            string fileFolder = AllPath + "\\" + Folder;
            checkFolderExistAll(fileFolder);
            file = fileFolder + "\\" + file;
            return File.Exists(file);
        }

        public static bool IsFileExist(string file)
        {           
            return File.Exists(file);
        }

		public static void checkFolderExist(string FolderPath ){
            
			if(!Directory.Exists(FolderPath)){Directory.CreateDirectory(FolderPath);}
		}

        public static string[] GetFolderFileWithoutExtension(string path)
        {
            string Pathfile = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string AllPath = Pathfile.Substring(0, Pathfile.LastIndexOf("\\"));
            AllPath += "\\" + path;
            string[] filePaths = Directory.GetFiles(AllPath);//@"c:\MyDir\"
            if (filePaths.Length > 0)
            {
                for (int i = 0; i < filePaths.Length; i++)
                {
                    filePaths[i] = Path.GetFileNameWithoutExtension(filePaths[i]);
                }
                return filePaths;
            }
            else {
                return null;
            }
        }

        public static string GetFolder() {
            return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }
        public static string GetFileNameWithoutExtension(string path)
        {
            return Path.GetFileNameWithoutExtension(path);
        }
        public static string[] GetFolderFile(string path)
        {
            string Pathfile = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string AllPath = Pathfile.Substring(0, Pathfile.LastIndexOf("\\"));

            AllPath += "\\" + path;
            string[] filePaths = Directory.GetFiles(AllPath);//@"c:\MyDir\"
            if (filePaths.Length > 0)
            {
                for (int i = 0; i < filePaths.Length; i++)
                {
                    filePaths[i] = Path.GetFileName(filePaths[i]);
                }
                return filePaths;
            }
            else
            {
                return null;
            }
        }
        public static string CurentFolder() {
            string Pathfile = System.Reflection.Assembly.GetExecutingAssembly().Location;
            return Pathfile.Substring(0, Pathfile.LastIndexOf("\\")).Replace("\\","/");
        }
        public static void DeleteFile(string file){
            if (checkFileExist(file))
            {
                string Pathfile = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string AllPath = Pathfile.Substring(0, Pathfile.LastIndexOf("\\"));
                file = AllPath + "\\" + file;
                File.Delete(file);
            }
        }

        
        public static void DeleteFile2(string file)
        {
            if (File.Exists(file))
            {                
                File.Delete(file);
            }
        }

        public static void WriteData(string log, string file)
        {
             
            try
            {
                StreamWriter Output = new StreamWriter(file, true, System.Text.Encoding.GetEncoding("BIG5"));
                Output.WriteLine(log);
                Output.Close();
            }
            catch (System.Exception ex)
            {
                string ss = ex.Message;
            }
        }

        public static ArrayList GetDataList(string Folder)
        {
            string[] files = FunLog.GetFolderFile(Folder);
            ArrayList AllList = new ArrayList();
            foreach (string file in files)
            {
                ArrayList lList = File2ArrayList( Folder+"\\" +file);
                AllList.AddRange(lList);
            }
            return AllList;

        }


	}
}
