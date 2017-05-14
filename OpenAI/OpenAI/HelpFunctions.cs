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

        public void ToggleLog(bool toggle)
        {
            WriteLog = toggle;
        }

        public void CreateNewLogfile()
        {
            FileCreated = false;
        }

        /*** Cleaned ***/

        private List<string> logBuffer = new List<string>(Settings.Instance.logBuffer + 1);
        public void Log(string s)
        {
            logBuffer.Add(s);

            if (logBuffer.Count > Settings.Instance.logBuffer) FlushLog();
        }

        public void FlushLog()
        {
            if (logBuffer.Count == 0)
                return;

            try
            {
                File.AppendAllLines(Settings.Instance.logpath + Settings.Instance.logfile, logBuffer);
                logBuffer.Clear();
            }
            catch
            {

            }
        }

        public DateTime UnixTimeStampToDateTime(int unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        private List<string> errorLogBuffer = new List<string>(Settings.Instance.logBuffer + 1);
        public void ErrorLog(string s)
        {
            if (!WriteLog)
                return;

            errorLogBuffer.Add(DateTime.Now.ToString("HH:mm:ss: ") + s);

            if (errorLogBuffer.Count > Settings.Instance.logBuffer) FlushErrorLog();
        }

        public void FlushErrorLog()
        {
            if (errorLogBuffer.Count == 0) return;
            try
            {
                File.AppendAllLines(Settings.Instance.logpath + "Logging.txt", errorLogBuffer);
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

        string sendBuffer = "";
        public void ResetBuffer()
        {
            this.sendBuffer = "";
        }

        public void WriteToBuffer(string data)
        {
            this.sendBuffer += data + "\r\n";
        }

        public void WriteBufferToNetwork(string msgtype)
        {
            FishNet.Instance.sendMessage(msgtype + "\r\n" + this.sendBuffer);
        }

        public void WriteBufferToFile()
        {
            bool writed = true;
            this.sendBuffer += "<EoF>";
            //this.ErrorLog("write to crrntbrd file: " + sendbuffer);
            while (writed)
            {
                try
                {
                    if (Settings.Instance.useNetwork) WriteBufferToNetwork("crrntbrd.txt");
                    else System.IO.File.WriteAllText(Settings.Instance.path + "crrntbrd.txt", this.sendBuffer);
                    writed = false;
                }
                catch
                {
                    writed = true;
                }
            }
            this.sendBuffer = "";
        }

        public void WriteBufferToDeckFile()
        {
            bool writed = true;
            this.sendBuffer += "<EoF>";
            while (writed)
            {
                try
                {
                    if (Settings.Instance.useNetwork) WriteBufferToNetwork("curdeck.txt");
                    else System.IO.File.WriteAllText(Settings.Instance.path + "curdeck.txt", this.sendBuffer);
                    writed = false;
                }
                catch
                {
                    writed = true;
                }
            }
            this.sendBuffer = "";
        }

        public void WriteBufferToActionFile()
        {
            bool writed = true;
            this.sendBuffer += "<EoF>";
            //this.ErrorLog("write to action file: "+ sendbuffer);
            while (writed)
            {
                try
                {
                    if (Settings.Instance.useNetwork) WriteBufferToNetwork("actionstodo.txt");
                    else System.IO.File.WriteAllText(Settings.Instance.path + "actionstodo.txt", this.sendBuffer);
                    writed = false;
                }
                catch
                {
                    writed = true;
                }
            }
            this.sendBuffer = "";
        }

        public void WriteBufferToCardDB()
        {
            bool writed = true;
            while (writed)
            {
                try
                {
                    System.IO.File.WriteAllText(Settings.Instance.path + "newCardDB.cs", this.sendBuffer);
                    writed = false;
                }
                catch
                {
                    writed = true;
                }
            }
            this.sendBuffer = "";
        }
    }
}
