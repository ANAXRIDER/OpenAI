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

        private bool writelogg = true;
        public void Loggonoff(bool onoff)
        {
            //writelogg = onoff;
        }

        private bool filecreated = false;
        public void CreateNewLogfile()
        {
            filecreated = false;
        }

        private List<string> loggBuffer = new List<string>(Settings.Instance.logBuffer + 1);
        public void Log(string s)
        {
            loggBuffer.Add(s);

            if (loggBuffer.Count > Settings.Instance.logBuffer) FlushLog();
        }

        public void FlushLog()
        {
            if (loggBuffer.Count == 0) return;
            try
            {
                File.AppendAllLines(Settings.Instance.logpath + Settings.Instance.logfile, loggBuffer);
                loggBuffer.Clear();
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
            if (!writelogg) return;
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

        string sendbuffer = "";
        public void ResetBuffer()
        {
            this.sendbuffer = "";
        }

        public void WriteToBuffer(string data)
        {
            this.sendbuffer += data + "\r\n";
        }

        public void WriteBufferToNetwork(string msgtype)
        {
            FishNet.Instance.sendMessage(msgtype + "\r\n" + this.sendbuffer);
        }

        public void WriteBufferToFile()
        {
            bool writed = true;
            this.sendbuffer += "<EoF>";
            //this.ErrorLog("write to crrntbrd file: " + sendbuffer);
            while (writed)
            {
                try
                {
                    if (Settings.Instance.useNetwork) WriteBufferToNetwork("crrntbrd.txt");
                    else System.IO.File.WriteAllText(Settings.Instance.path + "crrntbrd.txt", this.sendbuffer);
                    writed = false;
                }
                catch
                {
                    writed = true;
                }
            }
            this.sendbuffer = "";
        }

        public void WriteBufferToDeckFile()
        {
            bool writed = true;
            this.sendbuffer += "<EoF>";
            while (writed)
            {
                try
                {
                    if (Settings.Instance.useNetwork) WriteBufferToNetwork("curdeck.txt");
                    else System.IO.File.WriteAllText(Settings.Instance.path + "curdeck.txt", this.sendbuffer);
                    writed = false;
                }
                catch
                {
                    writed = true;
                }
            }
            this.sendbuffer = "";
        }

        public void WriteBufferToActionFile()
        {
            bool writed = true;
            this.sendbuffer += "<EoF>";
            //this.ErrorLog("write to action file: "+ sendbuffer);
            while (writed)
            {
                try
                {
                    if (Settings.Instance.useNetwork) WriteBufferToNetwork("actionstodo.txt");
                    else System.IO.File.WriteAllText(Settings.Instance.path + "actionstodo.txt", this.sendbuffer);
                    writed = false;
                }
                catch
                {
                    writed = true;
                }
            }
            this.sendbuffer = "";
        }

        public void WriteBufferToCardDB()
        {
            bool writed = true;
            while (writed)
            {
                try
                {
                    System.IO.File.WriteAllText(Settings.Instance.path + "newCardDB.cs", this.sendbuffer);
                    writed = false;
                }
                catch
                {
                    writed = true;
                }
            }
            this.sendbuffer = "";
        }
    }
}
