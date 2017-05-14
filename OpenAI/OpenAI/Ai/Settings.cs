using System;
using System.IO;

namespace OpenAI
{
    internal sealed class Settings
    {
        public Behavior SetSettings()
        {
            return ReadSettings();
        }

        public Behavior SetDefaultSettings()
        {
            this.EnfaceHp = 15;             // hp of enemy when your hero is allowed to attack the enemy face with his weapon
            this.MaxWide = 3000;            // numer of boards which are taken to the next deep-lvl
            this.TwotsAmount = 0;           // number of boards where the next turn is simulated
            this.SimEnemySecondTurn = true; // if he simulates the next players-turn, he also simulates the enemys respons

            this.PlayAround = false;    //play around some enemys aoe-spells?
            //these two probs are >= 0 and <= 100
            this.PlayAroundProb = 50;   //probability where the enemy plays the aoe-spell, but your minions will not die through it
            this.PlayAroundProb2 = 80;  // probability where the enemy plays the aoe-spell, and your minions can die!

            this.EnemyTurnMaxWide = 40;             // bords calculated in enemys-first-turn in first AI step (lower than enemySecondTurnMaxWide)
            this.EnemyTurnMaxWideSecondTime = 200;  // bords calculated in enemys-first-turn BUT in the second AI step (higher than enemyTurnMaxWide)
            this.EnemySecondTurnMaxWide = 20;       // number of enemy-board calculated in enemys second TURN

            this.NextTurnDeep = 6;          //maximum combo-deep in your second turn (dont change this!)
            this.NextTurnMaxWide = 20;      //maximum boards calculated in one second-turn-"combo-step"
            this.NextTurnTotalBoards = 200; //maximum boards calculated in second turn simulation

            this.useSecretsPlayArround = false; // playing arround enemys secrets

            this.alpha = 50; // weight of the second turn in calculation (0<= alpha <= 100)

            this.simulatePlacement = false;         // set this true, and ai will simulate all placements, whether you have a alpha/flametongue/argus
            this.behave = new BehaviorControl();    //select the behavior of the ai: control, rush, face (new) or mana (very experimental, dont use that :D)

            this.useExternalProcess = false;    // use silver.exe for calculations a lot faster than turning it off (true = recomended)
            this.passiveWaiting = false;        // process will wait passive for silver.exe to finish

            this.speedy = false; // send multiple actions together to HR
            
            this.useNetwork = false;        // use networking to communicate with silver.exe instead of a file
            this.netAddress = "127.0.0.1";  // address where the bot is running
            this.tcpPort = 14804;           // TCP port to connect on

            this.logBuffer = 100; // max log messages to buffer before writing to disk

            this.SecretStandard = true;
            this.enemyherovaluemultiply = 100;
            //###########################################################

            ApplySettings();

            return behave;
        }

        public void ApplySettings()
        {
            this.SetWeights(alpha);

            Mulligan.Instance.setAutoConcede(Settings.Instance.concede);
            HelpFunctions.Instance.ErrorLog("[Settings] set enemy-face-hp to: " + this.EnfaceHp);
            ComboBreaker.Instance.attackFaceHP = this.EnfaceHp;
            Ai.Instance.setMaxWide(this.MaxWide);
            HelpFunctions.Instance.ErrorLog("[Settings] set maxwide to: " + this.MaxWide);

            //Ai.Instance.setTwoTurnSimulation(false, this.twotsamount);
            HelpFunctions.Instance.ErrorLog("[Settings] calculate the second turn of the " + this.TwotsAmount + " best boards");
            if (this.TwotsAmount >= 1)
            {
                if (this.SimEnemySecondTurn) HelpFunctions.Instance.ErrorLog("[Settings] simulates the enemy turn on your second turn");
            }

            if (this.useSecretsPlayArround)
            {
                HelpFunctions.Instance.ErrorLog("[Settings] playing arround secrets is " + this.useSecretsPlayArround);
            }
            Ai.Instance.setPlayAround();

            if (this.writeToSingleFile) HelpFunctions.Instance.ErrorLog("[Settings] write log to single file");
        }

        //

        public int EnfaceHp { get; set; } = 15;

        public int MaxWide { get; set; } = 3000;
        public int TwotsAmount { get; set; }
        public int SecondTurnAmount { get; set; } = 256;

        public bool SimulateEnemysTurn { get; set; } = true;
        public bool SimEnemySecondTurn { get; set; } = true; //todo sepefeets - wasn't this dead code too?

        public bool PlayAround { get; set; }
        public int PlayAroundProb { get; set; } = 50;
        public int PlayAroundProb2 { get; set; } = 80; //todo - and this

        public int EnemyTurnMaxWide { get; set; } = 20;
        public int EnemyTurnMaxWideSecondTime { get; set; } = 200;
        public int EnemySecondTurnMaxWide { get; set; } = 20;

        public int NextTurnDeep { get; set; } = 6;
        public int NextTurnMaxWide { get; set; } = 20;
        public int NextTurnTotalBoards { get; set; } = 50;

        //

        public bool useSecretsPlayArround;

        public int alpha = 50;
        public float firstweight = 0.5f;
        public float secondweight = 0.5f;

        public bool simulatePlacement = true;

        public bool useExternalProcess;
        public bool passiveWaiting; //todo sepefeets - dead code
        public bool speedy;

        public bool concede = false;
        public bool enemyConcede;
        public int enemyConcedeValue = -1200;

        public bool useNetwork = false;
        public string netAddress = "127.0.0.1";
        public int tcpPort = 14804;

        public int logBuffer = 100;

        public bool SecretStandard = true;
        public int enemyherovaluemultiply = 100;

        public string logpath = "";
        public string logfile = "Logg.txt";

        public bool writeToSingleFile = false;

        public bool learnmode = false;
        public bool printlearnmode = true;

        public int numberOfThreads = Environment.ProcessorCount;

        private string ownClass = "";
        private string enemyClass = "";
        private string deckName = "";
        private string cleanPath = "";

        public Behavior behave = new BehaviorControl();


        private static Settings instance;
        public static Settings Instance
        {
            get
            {
                return instance ?? (instance = new Settings());
            }
        }

        private Settings() { }

        public Behavior UpdateInstance()
        {
            ownClass = Hrtprozis.Instance.heroEnumtoCommonName(Hrtprozis.Instance.heroname);
            enemyClass = Hrtprozis.Instance.heroEnumtoCommonName(Hrtprozis.Instance.enemyHeroname);
            deckName = Hrtprozis.Instance.deckName;
            lock(instance)
            {
                return ReadSettings();
            }
        }

        public void LogCleanPath()
        {
            HelpFunctions.Instance.Log(cleanPath);
        }

        public void SetWeights(int alpha)
        {
            float a = ((float)alpha) / 100f;
            this.firstweight = 1f - a;
            this.secondweight = a;
            HelpFunctions.Instance.ErrorLog("[Settings] current alpha is " + this.secondweight);
        }

        public void SetLogPath(string path)
        {
            this.logpath = path;
        }

        public void SetLogFile(string path)
        {
            this.logfile = path;
        }

        public Behavior ReadSettings() //takes same path as carddb
        {
            string[] lines = new string[] { };

            string path = OpenAIPath.SettingsPath;
            string cleanpath = "Silverfish" + Path.DirectorySeparatorChar;
            string datapath = path + "Data" + Path.DirectorySeparatorChar;
            string cleandatapath = cleanpath + "Data" + Path.DirectorySeparatorChar;
            string classpath = datapath + ownClass + Path.DirectorySeparatorChar;
            string cleanclasspath = cleandatapath + ownClass + Path.DirectorySeparatorChar;
            string deckpath = classpath + deckName + Path.DirectorySeparatorChar;
            string cleandeckpath = cleanclasspath + deckName + Path.DirectorySeparatorChar;
            string enemyfilestring = "settings-" + enemyClass + ".txt";
            const string filestring = "settings.txt";
            bool enemysettings = false;

            // if we have a deckName then we have a real ownClass too, not the default "druid"
            if (deckName != "" && File.Exists(deckpath + enemyfilestring))
            {
                enemysettings = true;
                path = deckpath;
                cleanPath = cleandeckpath + enemyfilestring;
            }
            else if (deckName != "" && File.Exists(deckpath + filestring))
            {
                path = deckpath;
                cleanPath = cleandeckpath + filestring;
            }
            else if (deckName != "" && File.Exists(classpath + enemyfilestring))
            {
                enemysettings = true;
                path = classpath;
                cleanPath = cleanclasspath + enemyfilestring;
            }
            else if (deckName != "" && File.Exists(classpath + filestring))
            {
                path = classpath;
                cleanPath = cleanclasspath + filestring;
            }
            else if (deckName != "" && File.Exists(datapath + enemyfilestring))
            {
                enemysettings = true;
                path = datapath;
                cleanPath = cleandatapath + enemyfilestring;
            }
            else if (deckName != "" && File.Exists(datapath + filestring))
            {
                path = datapath;
                cleanPath = cleandatapath + filestring;
            }
            else if (File.Exists(path + enemyfilestring))
            {
                enemysettings = true;
                cleanPath = cleanpath + enemyfilestring;
            }
            else if (File.Exists(path + filestring))
            {
                cleanPath = cleanpath + filestring;
            }
            else
            {
                HelpFunctions.Instance.Log("[Settings] cant find base settings.txt, using default settings");
                return SetDefaultSettings();
            }
            HelpFunctions.Instance.ErrorLog("[Settings] read " + cleanPath);


            const string readerror = " read error. Continuing without user-defined rules.";
            if (enemysettings)
            {
                try
                {
                    lines = File.ReadAllLines(path + enemyfilestring);
                }
                catch
                {
                    HelpFunctions.Instance.ErrorLog(enemyfilestring + readerror);
                    return SetDefaultSettings();
                }
            }
            else
            {
                try
                {
                    lines = File.ReadAllLines(path + filestring);
                }
                catch
                {
                    HelpFunctions.Instance.Log(filestring + readerror);
                    return SetDefaultSettings();
                }
            }

            foreach (string ss in lines)
            {
                string s = ss.Replace(" ", "");
                if (s.Contains(";")) s = s.Split(';')[0];
                if (s.Contains("#")) s = s.Split('#')[0];
                if (s.Contains("//")) s = s.Split(new string[]{"//"}, StringSplitOptions.RemoveEmptyEntries)[0];
                if (s.Contains(",")) s = s.Split(',')[0];
                if (s == "" || s == " ") continue;
                s = s.ToLower();
                const string ignoring = "[Settings] ignoring the setting ";

                string searchword = "maxwide=";
                if (s.StartsWith(searchword))
                {
                    string a = s.Replace(searchword,"");
                    try
                    {
                        this.MaxWide = Convert.ToInt32(a);
                    }
                    catch
                    {
                        HelpFunctions.Instance.ErrorLog(ignoring + searchword);
                    }
                }

                searchword = "twotsamount=";
                if (s.StartsWith(searchword))
                {
                    string a = s.Replace(searchword, "");
                    try
                    {
                        this.TwotsAmount = Convert.ToInt32(a);
                    }
                    catch
                    {
                        HelpFunctions.Instance.ErrorLog(ignoring + searchword);
                    }
                }

                searchword = "simenemysecondturn=";
                if (s.StartsWith(searchword))
                {
                    string a = s.Replace(searchword, "");
                    try
                    {
                        this.SimEnemySecondTurn = Convert.ToBoolean(a);
                    }
                    catch
                    {
                        HelpFunctions.Instance.ErrorLog(ignoring + searchword);
                    }
                }

                searchword = "enfacehp=";
                if (s.StartsWith(searchword))
                {
                    string a = s.Replace(searchword, "");
                    try
                    {
                        this.EnfaceHp = Convert.ToInt32(a);
                    }
                    catch
                    {
                        HelpFunctions.Instance.ErrorLog(ignoring + searchword);
                    }
                }

                searchword = "playarround=";
                if (s.StartsWith(searchword))
                {
                    string a = s.Replace(searchword, "");
                    try
                    {
                        this.PlayAround = Convert.ToBoolean(a);
                    }
                    catch
                    {
                        HelpFunctions.Instance.ErrorLog(ignoring + searchword);
                    }
                }

                searchword = "playaroundprob=";
                if (s.StartsWith(searchword))
                {
                    string a = s.Replace(searchword, "");
                    try
                    {
                        this.PlayAroundProb = Convert.ToInt32(a);
                    }
                    catch
                    {
                        HelpFunctions.Instance.ErrorLog(ignoring + searchword);
                    }
                }

                searchword = "playaroundprob2=";
                if (s.StartsWith(searchword))
                {
                    string a = s.Replace(searchword, "");
                    try
                    {
                        this.PlayAroundProb2 = Convert.ToInt32(a);
                    }
                    catch
                    {
                        HelpFunctions.Instance.ErrorLog(ignoring + searchword);
                    }
                }

                searchword = "enemyturnmaxwide=";
                if (s.StartsWith(searchword))
                {
                    string a = s.Replace(searchword, "");
                    try
                    {
                        this.EnemyTurnMaxWide = Convert.ToInt32(a);
                    }
                    catch
                    {
                        HelpFunctions.Instance.ErrorLog(ignoring + searchword);
                    }
                }

                searchword = "enemyturnmaxwidesecondtime=";
                if (s.StartsWith(searchword))
                {
                    string a = s.Replace(searchword, "");
                    try
                    {
                        this.EnemyTurnMaxWideSecondTime = Convert.ToInt32(a);
                    }
                    catch
                    {
                        HelpFunctions.Instance.ErrorLog(ignoring + searchword);
                    }
                }

                searchword = "enemysecondturnmaxwide=";
                if (s.StartsWith(searchword))
                {
                    string a = s.Replace(searchword, "");
                    try
                    {
                        this.EnemySecondTurnMaxWide = Convert.ToInt32(a);
                    }
                    catch
                    {
                        HelpFunctions.Instance.ErrorLog(ignoring + searchword);
                    }
                }
               
                searchword = "nextturndeep=";
                if (s.StartsWith(searchword))
                {
                    string a = s.Replace(searchword, "");
                    try
                    {
                        this.NextTurnDeep = Convert.ToInt32(a);
                    }
                    catch
                    {
                        HelpFunctions.Instance.ErrorLog(ignoring + searchword);
                    }
                }

                searchword = "nextturnmaxwide=";
                if (s.StartsWith(searchword))
                {
                    string a = s.Replace(searchword, "");
                    try
                    {
                        this.NextTurnMaxWide = Convert.ToInt32(a);
                    }
                    catch
                    {
                        HelpFunctions.Instance.ErrorLog(ignoring + searchword);
                    }
                }

                searchword = "nextturntotalboards=";
                if (s.StartsWith(searchword))
                {
                    string a = s.Replace(searchword, "");
                    try
                    {
                        this.NextTurnTotalBoards = Convert.ToInt32(a);
                    }
                    catch
                    {
                        HelpFunctions.Instance.ErrorLog(ignoring + searchword);
                    }
                }

                searchword = "usesecretsplayarround=";
                if (s.StartsWith(searchword))
                {
                    string a = s.Replace(searchword, "");
                    try
                    {
                        this.useSecretsPlayArround = Convert.ToBoolean(a);
                    }
                    catch
                    {
                        HelpFunctions.Instance.ErrorLog(ignoring + searchword);
                    }
                }

                searchword = "alpha=";
                if (s.StartsWith(searchword))
                {
                    string a = s.Replace(searchword, "");
                    try
                    {
                        this.alpha = Convert.ToInt32(a);
                    }
                    catch
                    {
                        HelpFunctions.Instance.ErrorLog(ignoring + searchword);
                    }
                }

                searchword = "simulateplacement=";
                if (s.StartsWith(searchword))
                {
                    string a = s.Replace(searchword, "");
                    try
                    {
                        this.simulatePlacement = Convert.ToBoolean(a);
                    }
                    catch
                    {
                        HelpFunctions.Instance.ErrorLog(ignoring + searchword);
                    }
                }

                searchword = "useexternalprocess=";
                if (s.StartsWith(searchword))
                {
                    string a = s.Replace(searchword, "");
                    try
                    {
                        this.useExternalProcess = Convert.ToBoolean(a);
                    }
                    catch
                    {
                        HelpFunctions.Instance.ErrorLog(ignoring + searchword);
                    }
                }

                searchword = "passivewaiting=";
                if (s.StartsWith(searchword))
                {
                    string a = s.Replace(searchword, "");
                    try
                    {
                        this.passiveWaiting = Convert.ToBoolean(a);
                    }
                    catch
                    {
                        HelpFunctions.Instance.ErrorLog(ignoring + searchword);
                    }
                }

                searchword = "behave=";
                if (s.StartsWith(searchword))
                {
                    string a = s.Replace(searchword, "");
                    if (a.StartsWith("control")) behave = new BehaviorControl();
                    if (a.StartsWith("rush")) behave = new BehaviorRush();
                    if (a.StartsWith("mana")) behave = new BehaviorMana();
                    if (a.StartsWith("face")) behave = new BehaviorFace();
					if (a.StartsWith("aggrowarlock")) behave = new BehaviorAggroWarlock();
                    if (a.StartsWith("aggroshaman")) behave = new BehaviorAggroshaman();
                }

                searchword = "concedeonbadboard=";
                if (s.StartsWith(searchword))
                {
                    string a = s.Replace(searchword, "");
                    try
                    {
                        this.enemyConcede = Convert.ToBoolean(a);
                    }
                    catch
                    {
                        HelpFunctions.Instance.ErrorLog(ignoring + searchword);
                    }
                }

                searchword = "concedeonboardvalue=";
                if (s.StartsWith(searchword))
                {
                    string a = s.Replace(searchword, "");
                    try
                    {
                        this.enemyConcedeValue = Convert.ToInt32(a);
                    }
                    catch
                    {
                        HelpFunctions.Instance.ErrorLog(ignoring + searchword);
                    }
                }

                searchword = "speed=";
                if (s.StartsWith(searchword))
                {
                    string a = s.Replace(searchword, "");
                    try
                    {
                        this.speedy = Convert.ToBoolean(a);
                    }
                    catch
                    {
                        HelpFunctions.Instance.ErrorLog(ignoring + searchword);
                    }
                }

                searchword = "usenetwork=";
                if (s.StartsWith(searchword))
                {
                    string a = s.Replace(searchword, "");
                    try
                    {
                        this.useNetwork = Convert.ToBoolean(a);
                    }
                    catch
                    {
                        HelpFunctions.Instance.ErrorLog(ignoring + searchword);
                    }
                }

                searchword = "netaddress=";
                if (s.StartsWith(searchword))
                {
                    string a = s.Replace(searchword, "");
                    try
                    {
                        this.netAddress = a;
                    }
                    catch
                    {
                        HelpFunctions.Instance.ErrorLog(ignoring + searchword);
                    }
                }

                searchword = "tcpport=";
                if (s.StartsWith(searchword))
                {
                    string a = s.Replace(searchword, "");
                    try
                    {
                        this.tcpPort = Convert.ToInt32(a);
                    }
                    catch
                    {
                        HelpFunctions.Instance.ErrorLog(ignoring + searchword);
                    }
                }

                //always enabled now so ignore user setting
                /*
                searchword = "logbuffer=";
                if (s.StartsWith(searchword))
                {
                    string a = s.Replace(searchword, "");
                    try
                    {
                        this.logBuffer = Convert.ToInt32(a);
                    }
                    catch
                    {
                        Helpfunctions.Instance.ErrorLog(ignoring + searchword);
                    }
                }
                */

                searchword = "secretstandard =";
                if (s.StartsWith(searchword))
                {
                    string a = s.Replace(searchword, "");
                    try
                    {
                        this.SecretStandard = Convert.ToBoolean(a);
                    }
                    catch
                    {
                        HelpFunctions.Instance.ErrorLog(ignoring + searchword);
                    }
                }

                searchword = "enemyherovaluemultiply=";
                if (s.StartsWith(searchword))
                {
                    string a = s.Replace(searchword, "");
                    try
                    {
                        this.enemyherovaluemultiply = Convert.ToInt32(a);
                    }
                    catch
                    {
                        HelpFunctions.Instance.ErrorLog(ignoring + searchword);
                    }
                }


            }
            //foreach ended----------

            ApplySettings();


            return behave;
        }

    }
}
 