using System;
using System.IO;

namespace OpenAI
{
    public static class FilePath
    {
        private static string Root = FolderPath.OpenAI;

        public static string CrashTest
        {
            get
            {
                return Path.Combine(FolderPath.Common + "crashtest.txt");
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
