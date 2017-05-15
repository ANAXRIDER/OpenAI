using System;
using System.IO;

namespace OpenAI
{
    public static class FilePath
    {
        public static string Exe
        {
            get
            {
                return Path.Combine(FolderPath.OpenAI + "OpenAIConsole.exe");
            }
        }

        public static string CardDB
        {
            get
            {
                return Path.Combine(FolderPath.OpenAI + "_carddb.txt");
            }
        }

        public static string Settings
        {
            get
            {
                return Path.Combine(FolderPath.OpenAI + "settings.txt");
            }
        }

        public static string ErrorLog
        {
            get
            {
                return Path.Combine(FolderPath.Logs + "ErrorLog.txt");
            }
        }

        public static string Log
        {
            get
            {
                return Path.Combine(FolderPath.Logs + "Log.txt");
            }
        }

        public static string CrashTest
        {
            get
            {
                return Path.Combine(FolderPath.Common + "crashtest.txt");
            }
        }

        public static string ActionsToDo
        {
            get
            {
                return Path.Combine(FolderPath.Common + "actionstodo.txt");
            }
        }

        public static string CurrentBoard
        {
            get
            {
                return Path.Combine(FolderPath.Common + "crrntbrd.txt");
            }
        }

        public static string CurrentDeck
        {
            get
            {
                return Path.Combine(FolderPath.Common + "curdeck.txt");
            }
        }
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

        public static string OpenAI
        {
            get
            {
                return Root;
            }
        }

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
