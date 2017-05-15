using System;
using System.IO;

namespace OpenAI
{
    public static class FilePath
    {
        /*** ROOT FOLDER ***/

        public static string CardDB => Path.Combine(FolderPath.OpenAI + "_carddb.txt");

        public static string Exe => Path.Combine(FolderPath.OpenAI + "OpenAIConsole.exe");

        public static string MullTest => Path.Combine(FolderPath.OpenAI + "mulltest.txt");

        public static string Settings => Path.Combine(FolderPath.OpenAI + "settings.txt");

        public static string Test => Path.Combine(FolderPath.OpenAI + "test.txt");

        /*** LOG FOLDER ***/

        public static string ErrorLog => Path.Combine(FolderPath.Logs + "ErrorLog.txt");

        public static string Log => Path.Combine(FolderPath.Logs + "Log.txt");

        /*** COMMON FOLDER ***/

        public static string ActionsToDo => Path.Combine(FolderPath.Common + "actionstodo.txt");

        public static string CrashTest => Path.Combine(FolderPath.Common + "crashtest.txt");

        public static string CurrentBoard => Path.Combine(FolderPath.Common + "crrntbrd.txt");

        public static string CurrentDeck => Path.Combine(FolderPath.Common + "curdeck.txt");

        public static string NewCardDB => Path.Combine(FolderPath.Common + "newCardDB.cs");
    }

    public static class FolderPath
    {
        private static string Root
        {
            get
            {
                string codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                string temp = Path.GetDirectoryName(path) + Path.DirectorySeparatorChar;

                return temp;
            }
        }

        public static string OpenAI => Root;

        public static string Common
        {
            get
            {
                string temp = Root + "Common" + Path.DirectorySeparatorChar;
                if (Directory.Exists(temp) == false)
                {
                    Directory.CreateDirectory(temp);
                }

                return temp;
            }
        }

        public static string Logs
        {
            get
            {
                string temp = Root + "Logs" + Path.DirectorySeparatorChar;
                if (Directory.Exists(temp) == false)
                {
                    Directory.CreateDirectory(temp);
                }

                return temp;
            }
        }
    }
}
