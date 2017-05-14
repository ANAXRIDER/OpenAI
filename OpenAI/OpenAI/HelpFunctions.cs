using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI
{
    public sealed class HelpFunctions
    {
        private static HelpFunctions instance;
        public static HelpFunctions Instance
        {
            get
            {
                return instance ?? (instance = new HelpFunctions());
            }
        }

        private HelpFunctions() { }

        private bool WriteLog { get; set; } = true;
        private bool FileCreated { get; set; } = false;
        private List<string> logBuffer = new List<string>(Settings.Instance.logBuffer + 1);
        private List<string> errorLogBuffer = new List<string>(Settings.Instance.logBuffer + 1);
        private string SendBuffer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toggle"></param>
        public void ToggleLog(bool toggle)
        {
            WriteLog = toggle;
        }

        /// <summary>
        /// 
        /// </summary>
        public void CreateNewLogfile()
        {
            FileCreated = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        public void Log(string s)
        {
            logBuffer.Add(s);

            if (logBuffer.Count > Settings.Instance.logBuffer)
                FlushLog();
        }

        /// <summary>
        /// 
        /// </summary>
        public void FlushLog()
        {
            if (logBuffer.Count == 0)
                return;

            try
            {
                File.AppendAllLines(PathFile.Log, logBuffer);
                logBuffer.Clear();
            }
            catch
            {

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unixTimeStamp"></param>
        /// <returns></returns>
        public DateTime UnixTimeStampToDateTime(int unixTimeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();

            return dateTime;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        public void ErrorLog(string s)
        {
            if (!WriteLog)
                return;

            if(s != string.Empty)
            {
                errorLogBuffer.Add(DateTime.Now.ToString("HH:mm:ss: ") + s);
            }
            else
            {
                errorLogBuffer.Add(s);
            }

            if (errorLogBuffer.Count > Settings.Instance.logBuffer)
                FlushErrorLog();
        }

        /// <summary>
        /// 
        /// </summary>
        public void FlushErrorLog()
        {
            if (errorLogBuffer.Count == 0)
                return;

            try
            {
                File.AppendAllLines(PathFile.ErrorLog, errorLogBuffer);
                errorLogBuffer.Clear();
            }
            catch
            {

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StartFlushingLogBuffers(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() => Instance.FlushLogBuffersAsync(cancellationToken), cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task FlushLogBuffersAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                Instance.FlushLog();
                Instance.FlushErrorLog();
                await Task.Delay(50, cancellationToken);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResetBuffer()
        {
            SendBuffer = "";
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void WriteToBuffer(string data)
        {
            SendBuffer += data + "\r\n";
        }

        public void WriteBufferToNetwork(string msgtype)
        {
            FishNet.Instance.sendMessage(msgtype + "\r\n" + SendBuffer);
        }

        /// <summary>
        /// 
        /// </summary>
        public void WriteBufferToFile()
        {
            bool writed = true;

            SendBuffer += "<EoF>";
            while (writed)
            {
                try
                {
                    if (Settings.Instance.useNetwork)
                    {
                        WriteBufferToNetwork("crrntbrd.txt");
                    }
                    else
                    {
                        File.WriteAllText(PathFile.CurrentBoard, SendBuffer);
                    }
                    writed = false;
                }
                catch
                {
                    writed = true;
                }
            }
            SendBuffer = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        public void WriteBufferToDeckFile()
        {
            bool writed = true;

            SendBuffer += "<EoF>";
            while (writed)
            {
                try
                {
                    if (Settings.Instance.useNetwork)
                    {
                        WriteBufferToNetwork("curdeck.txt");
                    }
                    else
                    {
                        File.WriteAllText(PathFile.CurrentDeck, SendBuffer);
                    }
                    writed = false;
                }
                catch
                {
                    writed = true;
                }
            }
            SendBuffer = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        public void WriteBufferToActionFile()
        {
            bool writed = true;

            SendBuffer += "<EoF>";
            while (writed)
            {
                try
                {
                    if (Settings.Instance.useNetwork)
                    {
                            WriteBufferToNetwork("actionstodo.txt");
                    }
                    else
                    {
                        File.WriteAllText(PathFile.ActionsToDo, SendBuffer);
                    }
                    writed = false;
                }
                catch
                {
                    writed = true;
                }
            }
            SendBuffer = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        public void WriteBufferToCardDB()
        {
            bool writed = true;

            while (writed)
            {
                try
                {
                    File.WriteAllText(PathFile.NewCardDB, SendBuffer);
                    writed = false;
                }
                catch
                {
                    writed = true;
                }
            }
            SendBuffer = string.Empty;
        }
    }
}
