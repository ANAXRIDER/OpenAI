﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;

namespace OpenAI
{
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
            Bot b = new Bot();

            while (true)
            {
                System.Threading.Thread.Sleep(10);

                try
                {
                    string data = File.ReadAllText(FilePath.CurrentBoard);
                    //Helpfunctions.Instance.ErrorLog(data);
                    if (data != "" && data != "<EoF>")
                    {
                        data = data.Replace("<EoF>", "");
                        Helpfunctions.Instance.resetBuffer();
                        Helpfunctions.Instance.writeBufferToFile();
                        //readed = true;
                        b.doData(data);
                    }
                }
                catch
                {

                }
                try
                {
                    string data = File.ReadAllText(FilePath.CurrentDeck);
                    //Helpfunctions.Instance.ErrorLog(data);
                    if (data != "" && data != "<EoF>")
                    {
                        data = data.Replace("<EoF>", "");
                        Helpfunctions.Instance.resetBuffer();
                        Helpfunctions.Instance.writeBufferToDeckFile();
                        //readed = true;
                        b.doDeckData(data);
                    }
                }
                catch
                {

                }
            }
        }

        static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            using (StreamWriter sw = File.AppendText(FolderPath.Logs + "CrashLog" + DateTime.Now.ToString("_yyyy-MM-dd_HH-mm-ss") + ".txt"))
            {
                sw.WriteLine(e.ExceptionObject.ToString());
            }
            Console.WriteLine(e.ExceptionObject.ToString());
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("Please report this crash including the CrashLog in your Silverfish\\SilverLogs folder");
            Console.WriteLine("If no CrashLog exists then please post a screenshot instead");
            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();
            throw (Exception)e.ExceptionObject;
        }
    }

    public class Bot
    {
        PenalityManager penman = PenalityManager.Instance;
        DateTime starttime = DateTime.Now;
        Silverfish sf;
        Behavior behave = new BehaviorControl();

        public Bot()
        {
            starttime = DateTime.Now;
            this.sf = Silverfish.Instance;
            sf.SetNewLogFile();

            bool teststuff = true;
            bool printstuff = true;

            if (teststuff)
            {
                //Helpfunctions.Instance.logg("teststuff");
                Playfield p = new Playfield();
                Ai.Instance.autoTester(printstuff);
            }

            if (File.Exists(FilePath.CrashTest))
            {
                testing(1);
            }

            Helpfunctions.Instance.ErrorLog("wait for board...");
        }

        public void doData(string data)
        {
            Ai.Instance.updateTwoTurnSim();
            Ai.Instance.autoTester(false, data);
            Helpfunctions.Instance.resetBuffer();
            Helpfunctions.Instance.writeToBuffer("board " + Ai.Instance.currentCalculatedBoard);
            Helpfunctions.Instance.writeToBuffer("value " + Ai.Instance.bestmoveValue);
            Helpfunctions.Instance.writeToBuffer("discover " + Ai.Instance.bestTracking + "," + Ai.Instance.bestTrackingStatus);

            if (Ai.Instance.bestmove != null)
            {
                Ai.Instance.bestmove.Print(true);
                foreach (Action a in Ai.Instance.bestActions)
                {
                    a.Print(true);
                }
            }
            Helpfunctions.Instance.writeBufferToActionFile();
            Ai.Instance.currentCalculatedBoard = "1";
            Helpfunctions.Instance.ErrorLog("wait for next board...");

            //sf.readActionFile();
        }

        public void doDeckData(string data)
        {
            data = data.Trim();
            //Helpfunctions.Instance.ErrorLog(data);
            string deckname = data.Split(';')[0];
            string ownname = data.Split(';')[1];
            string enemyname = data.Split(';')[2];
            HeroEnum heroname = Hrtprozis.Instance.heroNametoEnum(ownname);
            HeroEnum enemyHeroname = Hrtprozis.Instance.heroNametoEnum(enemyname);

            if (Hrtprozis.Instance.deckName != deckname || heroname != Hrtprozis.Instance.heroname)
            {
                Hrtprozis.Instance.setDeckName(deckname);
                Hrtprozis.Instance.setHeroName(ownname);
                ComboBreaker.Instance.updateInstance();
                Discovery.Instance.UpdateInstance();
                Mulligan.Instance.updateInstance();
            }
            if (Hrtprozis.Instance.deckName != deckname || heroname != Hrtprozis.Instance.heroname || enemyHeroname != Hrtprozis.Instance.enemyHeroname)
            {
                Hrtprozis.Instance.setEnemyHeroName(enemyname);
                Settings.Instance.UpdateInstance();
            }
        }

        public void testing(int start)
        {
            int cardenumcount = Enum.GetNames(typeof(CardDB.cardIDEnum)).Length;
            Random rand = new Random();
            int randcard = 1;

            for (int i = start; i < cardenumcount; i++)
            {
                Handmanager.Instance.anzcards = 1;
                Handmanager.Instance.handCards.Clear();

                Handmanager.Handcard hc = new Handmanager.Handcard //the current card to test
                {
                    manacost = 1,
                    position = 1,
                    entity = 121,
                    card = CardDB.Instance.getCardDataFromID((CardDB.cardIDEnum)i)
                };
                Handmanager.Instance.handCards.Add(hc);
                
                randcard = rand.Next(1, cardenumcount);
                Handmanager.Handcard hc2 = new Handmanager.Handcard //extra random card to test
                {
                    manacost = 1,
                    position = 1,
                    entity = 122,
                    card = CardDB.Instance.getCardDataFromID((CardDB.cardIDEnum)randcard)
                };
                Handmanager.Instance.handCards.Add(hc2);

                randcard = rand.Next(1, cardenumcount);
                Handmanager.Handcard hc3 = new Handmanager.Handcard //extra random card to test
                {
                    manacost = 1,
                    position = 1,
                    entity = 123,
                    card = CardDB.Instance.getCardDataFromID((CardDB.cardIDEnum)randcard)
                };
                Handmanager.Instance.handCards.Add(hc3);

                randcard = rand.Next(1, cardenumcount);
                Handmanager.Handcard hc4 = new Handmanager.Handcard //extra random card to test
                {
                    manacost = 1,
                    position = 1,
                    entity = 124,
                    card = CardDB.Instance.getCardDataFromID((CardDB.cardIDEnum)randcard)
                };
                Handmanager.Instance.handCards.Add(hc4);

                randcard = rand.Next(1, cardenumcount);
                Handmanager.Handcard hc5 = new Handmanager.Handcard //extra random card to test
                {
                    manacost = 1,
                    position = 1,
                    entity = 125,
                    card = CardDB.Instance.getCardDataFromID((CardDB.cardIDEnum)randcard)
                };
                Handmanager.Instance.handCards.Add(hc5);

                randcard = rand.Next(1, cardenumcount);
                Handmanager.Handcard hc6 = new Handmanager.Handcard //extra random card to test
                {
                    manacost = 1,
                    position = 1,
                    entity = 126,
                    card = CardDB.Instance.getCardDataFromID((CardDB.cardIDEnum)randcard)
                };
                Handmanager.Instance.handCards.Add(hc6);

                randcard = rand.Next(1, cardenumcount);
                Handmanager.Handcard hc7 = new Handmanager.Handcard //extra random card to test
                {
                    manacost = 1,
                    position = 1,
                    entity = 127,
                    card = CardDB.Instance.getCardDataFromID((CardDB.cardIDEnum)randcard)
                };
                Handmanager.Instance.handCards.Add(hc7);


                Helpfunctions.Instance.ErrorLog("test " + i + " " + hc.card.name + " " + hc.card.cardIDenum);
                if (hc.card.sim_card == null)
                {
                    Helpfunctions.Instance.logg("cant test " + i + " " + hc.card.name);
                }
                else
                {
                    Ai.Instance.dosomethingclever(this.behave);
                }
            }
        }
    }

    public sealed class Silverfish
    {
        public string versionnumber = "131.0SE + Redfish";
        private bool singleLog;

        Settings sttngs = Settings.Instance;

        List<Minion> ownMinions = new List<Minion>();
        List<Minion> enemyMinions = new List<Minion>();
        List<Handmanager.Handcard> handCards = new List<Handmanager.Handcard>();
        List<string> ownSecretList = new List<string>();

        CardDB.Card heroAbility = new CardDB.Card();
        CardDB.Card enemyAbility = new CardDB.Card();
        
        private int anzOgOwnCThunHpBonus = 0;
        private int anzOgOwnCThunAngrBonus = 0;
        private int anzOgOwnCThunTaunt = 0;

        private static Silverfish instance;

        public static Silverfish Instance
        {
            get
            {
                return instance ?? (instance = new Silverfish());
            }
        }

        private Silverfish()
        {
            Helpfunctions.Instance.ErrorLog("init Silverfish V" + versionnumber);

            PenalityManager.Instance.setCombos();
            Mulligan.Instance.runDebugTest();
            Discovery d = Discovery.Instance; // read the discover list
            Settings.Instance.SetSettings();
        }

        public void SetNewLogFile()
        {
            if (!singleLog)
            {
                Helpfunctions.Instance.createNewLoggfile();
                Helpfunctions.Instance.ErrorLog("#######################################################");
                Helpfunctions.Instance.ErrorLog("fight is logged in: " + FilePath.Log);
                Helpfunctions.Instance.ErrorLog("#######################################################");
            }
        }

        public bool IsCardCreated(Handmanager.Handcard handcard)
        {
            return false;
        }


        public void readActionFile(bool passiveWaiting = false)
        {
            Ai.Instance.nextMoveGuess = new Playfield();
            bool readed = true;
            List<string> alist = new List<string>();
            float value = 0f;
            string boardnumm = "-1";
            int trackingchoice = 0;
            int trackingstate = 0;

            while (readed)
            {
                try
                {
                    string data = "";
                    System.Threading.Thread.Sleep(10);

                    data = File.ReadAllText(FilePath.ActionsToDo);
                    
                    if (data != "" && data != "<EoF>" && data.EndsWith("<EoF>"))
                    {
                        data = data.Replace("<EoF>", "");
                        //Helpfunctions.Instance.ErrorLog(data);
                        Helpfunctions.Instance.resetBuffer();
                        Helpfunctions.Instance.writeBufferToActionFile();
                        alist.AddRange(data.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries));
                        string board = alist[0];
                        if (board.StartsWith("board "))
                        {
                            boardnumm = (board.Split(' ')[1].Split(' ')[0]);
                            alist.RemoveAt(0);
                            /*if (boardnumm != Ai.Instance.currentCalculatedBoard)
                            {
                                if (passiveWaiting)
                                {
                                    System.Threading.Thread.Sleep(10);
                                    return;
                                }
                                continue;
                            }*/
                        }
                        string first = alist[0];
                        if (first.StartsWith("value "))
                        {
                            value = float.Parse((first.Split(' ')[1].Split(' ')[0]));
                            alist.RemoveAt(0);
                        }

                        first = alist[0];

                        if (first.StartsWith("discover "))
                        {
                            string trackingstuff = first.Replace("discover ", "");
                            trackingchoice = Convert.ToInt32(first.Split(',')[0]);
                            trackingstate = Convert.ToInt32(first.Split(',')[1]);
                            alist.RemoveAt(0);
                        }

                        readed = false;
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(10);
                        if (passiveWaiting)
                        {
                            return;
                        }
                    }

                }
                catch
                {
                    System.Threading.Thread.Sleep(10);
                }

            }
            Helpfunctions.Instance.logg("received " + boardnumm + " actions to do:");
            Ai.Instance.currentCalculatedBoard = "0";
            Playfield p = new Playfield();
            List<Action> aclist = new List<Action>();

            foreach (string a in alist)
            {
                aclist.Add(new Action(a, p));
                Helpfunctions.Instance.logg(a);
            }

            Ai.Instance.setBestMoves(aclist, value, trackingchoice, trackingstate);
        }

        public static int getLastAffected(int entityid)
        {
            return 0;
        }

        public static int getCardTarget(int entityid)
        {
            return 0;
        }
    }

    public sealed class Helpfunctions
    {
        private static Helpfunctions instance;

        public static Helpfunctions Instance
        {
            get
            {
                return instance ?? (instance = new Helpfunctions());
            }
        }

        private Helpfunctions()
        {
            //File.WriteAllText(Settings.Instance.logpath + Settings.Instance.logfile, "");
        }
        
        public void loggonoff(bool onoff)
        {
            //writelogg = onoff;
        }

        public void createNewLoggfile()
        {
            //File.WriteAllText(Settings.Instance.logpath + Settings.Instance.logfile, "");
        }

        public void logg(string s)
        {


            /*if (!writelogg) return;
            try
            {
                using (StreamWriter sw = File.AppendText(Settings.Instance.logpath + Settings.Instance.logfile))
                {
                    sw.WriteLine(s);
                }
            }
            catch { }*/
            Console.WriteLine(s);
        }

        public DateTime UnixTimeStampToDateTime(int unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public void ErrorLog(string s)
        {
            //HREngine.API.Utilities.HRLog.Write(s);
            Console.WriteLine(s);
        }


        string sendbuffer = "";
        public void resetBuffer()
        {
            this.sendbuffer = "";
        }

        public void writeToBuffer(string data)
        {
            this.sendbuffer += data + "\r\n";
        }

        public void writeBufferToFile()
        {
            bool writed = true;
            this.sendbuffer += "<EoF>";
            while (writed)
            {
                try
                {
                    File.WriteAllText(FilePath.CurrentBoard, this.sendbuffer);
                    writed = false;
                }
                catch
                {
                    writed = true;
                }
            }
            this.sendbuffer = "";
        }

        public void writeBufferToDeckFile()
        {
            bool writed = true;
            sendbuffer += "<EoF>";

            while (writed)
            {
                try
                {
                    File.WriteAllText(FilePath.CurrentDeck, sendbuffer);
                    writed = false;
                }
                catch
                {
                    writed = true;
                }
            }
            sendbuffer = "";
        }

        public void writeBufferToActionFile()
        {
            bool writed = true;
            sendbuffer += "<EoF>";
            ErrorLog("write to action file: "+ sendbuffer);

            while (writed)
            {
                try
                {
                    File.WriteAllText(FilePath.ActionsToDo, sendbuffer);
                    writed = false;
                }
                catch
                {
                    writed = true;
                }
            }
            this.sendbuffer = "";
        }
   
        public void writeBufferToCardDB()
        {
            bool writed = true;
            while (writed)
            {
                try
                {
                    File.WriteAllText(FilePath.NewCardDB, this.sendbuffer);
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


    // the ai :D
    //please ask/write me if you use this in your project

    

}
