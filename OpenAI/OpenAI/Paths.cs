using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAI
{
    public static class PathFile
    {
        public static string ActionsToDo
        {
            get
            {
                return PathFolder.Common + "actionstodo.txt";
            }
        }

        public static string CurrentBoard
        {
            get
            {
                return PathFolder.Common + "crrntbrd.txt";
            }
        }

        public static string CurrentDeck
        {
            get
            {
                return PathFolder.Common + "curdeck.txt";
            }
        }

        public static string CrashTest
        {
            get
            {
                return PathFolder.Common + "crashtest.txt";
            }
        }

        public static string NewCardDB
        {
            get
            {
                return PathFolder.Common + "newCardDB.txt";
            }
        }

        public static string CardDB
        {
            get
            {
                return PathFolder.OpenAI + "_carddb.txt";
            }
        }

        public static string Executable
        {
            get
            {
                return PathFolder.OpenAI + "OpenAIConsole.exe";
            }
        }

        public static string Settings
        {
            get
            {
                return PathFolder.OpenAI + "settings.txt";
            }
        }
        
        public static string Discovery
        {
            get
            {
                return PathFolder.OpenAI + "_discovery.txt";
            }
        }

        public static string Combo
        {
            get
            {
                return PathFolder.OpenAI + "_combo.txt";
            }
        }

        public static string Mulligan
        {
            get
            {
                return PathFolder.OpenAI + "_mulligan.txt";
            }
        }

        public static string Test
        {
            get
            {
                return PathFolder.OpenAI + "test.txt";
            }
        }

        public static string Log
        {
            get
            {
                return PathFolder.OpenAI + "Log.txt";
            }
        }

        public static string ErrorLog
        {
            get
            {
                return PathFolder.Logs + "ErrorLog.txt";
            }
        }
    }

    public static class PathFolder
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

        //

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

        public static string OpenAI
        {
            get
            {
                return Root;
            }
        }
    }
}
