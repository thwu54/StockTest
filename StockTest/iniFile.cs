using System;
using System.Runtime.InteropServices;
using System.Text;

namespace My
{
	/// <summary>
	/// iniFile ªººK­n´y­z¡C
	/// </summary>
	public class iniFile
	{
		public string filename;
		[DllImport("kernel32")]private static extern long WritePrivateProfileString(string section, string key, string val, string filepatch);
		[DllImport("kernel32")]private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size,string filePath);

		//«غc¤l
		public iniFile(string filename)
		{
			this.filename = filename;
		}

		public static void iniWriteValue(string Section, string key, string Value)
		{
			try
			{
                string Path=System.Reflection.Assembly.GetExecutingAssembly().Location;
                Path = Path.Replace(".exe", ".config");

                WritePrivateProfileString(Section, key, Value, Path);
			}
			catch(Exception e)
			{}
		}

        public static void iniWriteValue(string FileName, string Section, string key, string Value)
        {
            try
            {
                string Path = System.Reflection.Assembly.GetExecutingAssembly().Location;
                Path = Path.Substring(0, Path.LastIndexOf("\\")) + FileName; 
                WritePrivateProfileString(Section, key, Value, Path);
            }
            catch (Exception e)
            { }
        }

		public static string iniReadValue(string Section, string Key)
		{
			StringBuilder Value = new StringBuilder(255);
			try
			{
                string Path = System.Reflection.Assembly.GetExecutingAssembly().Location;
                Path = Path.Replace(".exe", ".config");
                int i = GetPrivateProfileString(Section, Key, "", Value, 255, Path);
				return Value.ToString();
			}
			catch(Exception e)
			{
				return "";
			}
		}

        public static string iniReadValue(string FileName,string Section, string Key)
        {
            StringBuilder Value = new StringBuilder(255);
            try
            {
                string Path = System.Reflection.Assembly.GetExecutingAssembly().Location;
                Path = Path.Substring(0, Path.LastIndexOf("\\")) + FileName;        
                int i = GetPrivateProfileString(Section, Key, "", Value, 255, Path);
                return Value.ToString();
            }
            catch (Exception e)
            {
                return "";
            }
        }

	}
}
