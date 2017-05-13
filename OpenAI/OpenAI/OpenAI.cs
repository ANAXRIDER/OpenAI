using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HSRangerLib;

namespace OpenAI
{
    public sealed class OpenAI
    {
        public string VersionNumber { get; } = "0.0.1";
        public bool WaitingForOpenAI { get; set; } = false;
        public bool StartedExe { get; set; } = false;

        private string BotBehave { get; set; } = "rush";
        private bool SingleLog { get; set; } = false;
        //
        Playfield lastpf;
        Settings sttngs = Settings.Instance;

        public List<Minion> ownMinions = new List<Minion>();
        public List<Minion> enemyMinions = new List<Minion>();
        List<Handmanager.Handcard> handCards = new List<Handmanager.Handcard>();
        int ownPlayerController = 0;
        List<string> ownSecretList = new List<string>();
        int enemySecretCount = 0;
        List<int> enemySecretList = new List<int>();

        int currentMana = 0;
        int ownMaxMana = 0;
        int numOptionPlayedThisTurn = 0;
        int numMinionsPlayedThisTurn = 0;
        int cardsPlayedThisTurn = 0;
        int ownOverload = 0;

        int enemyMaxMana = 0;

        string ownHeroWeapon = "";
        int heroWeaponAttack = 0;
        int heroWeaponDurability = 0;

        string enemyHeroWeapon = "";
        int enemyWeaponAttack = 0;
        int enemyWeaponDurability = 0;

        string heroname = "";
        string enemyHeroname = "";

        CardDB.Card heroAbility = new CardDB.Card();
        bool ownAbilityisReady = false;
        CardDB.Card enemyAbility = new CardDB.Card();

        int anzcards = 0;
        int enemyAnzCards = 0;

        int ownHeroFatigue = 0;
        int enemyHeroFatigue = 0;
        int ownDecksize = 0;
        int enemyDecksize = 0;

        Minion ownHero;
        Minion enemyHero;

        private int anzOgOwnCThunHpBonus = 0;
        private int anzOgOwnCThunAngrBonus = 0;
        private int anzOgOwnCThunTaunt = 0;

        private int OwnCrystalCore = 0;
        private int EnemyCrystalCore = 0;
        private bool ownMinionsCost0 = false;

        // NEW VALUES--

        int numberMinionsDiedThisTurn = 0;//todo need that value
        int ownCurrentOverload = 0;//todo get them! = number of overloaded Manacrystals for CURRENT turn (NOT RECALL_OWED !)
        int enemyOverload = 0;//todo need that value maybe
        int ownDragonConsort = 0;
        int enemyDragonConsort = 0;
        int ownLoathebs = 0;// number of loathebs WE PLAYED (so enemy has the buff)
        int enemyLoathebs = 0;
        int ownMillhouse = 0; // number of millhouse-manastorm WE PLAYED (so enemy has the buff)
        int enemyMillhouse = 0;
        int ownKirintor = 0;
        int ownPrepa = 0;

        // NEW VALUES#TGT#############################################################################################################
        // NEW VALUES#################################################################################################################
        int heroPowerUsesThisTurn = 0;
        int ownHeroPowerUsesThisGame = 0;
        int enemyHeroPowerUsesThisGame = 0;
        int lockandload = 0;
        int Stampede = 0;
        int ownsabo = 0;//number of saboteurplays  of our player (so enemy has the buff)
        int enemysabo = 0;//number of saboteurplays  of enemy player (so we have the buff)
        int ownFenciCoaches = 0; // number of Fencing Coach-debuffs on our player 

        int enemyCursedCardsInHand = 0;

        //LOE stuff###############################################################################################################
        public List<CardDB.cardIDEnum> choiceCards = new List<CardDB.cardIDEnum>(); // here we save all available tracking/discover cards ordered from left to right
        public List<int> choiceCardsEntitys = new List<int>(); //list of entitys same order as choiceCards

        private static HSRangerLib.GameState latestGameState;

        private static OpenAI instance;

        public static OpenAI Instance
        {
            get
            {
                return instance ?? (instance = new OpenAI());
            }
        }

        private OpenAI()
        {
            this.SingleLog = Settings.Instance.writeToSingleFile;
            string path = OpenAIPath.AssemblyDirectory + "SilverLogs" + System.IO.Path.DirectorySeparatorChar;
            Directory.CreateDirectory(path);
            sttngs.setFilePath(OpenAIPath.AssemblyDirectory);

            if (SingleLog)
            {
                sttngs.setLoggPath(OpenAIPath.LogPath + System.IO.Path.DirectorySeparatorChar);
                sttngs.setLoggFile("SilverLog.txt");
                HelpFunctions.Instance.createNewLoggfile();
            }
            else
            {
                sttngs.setLoggPath(path);
            }

            HelpFunctions.Instance.ErrorLog("init Silverfish");
            HelpFunctions.Instance.ErrorLog("setlogpath to:" + path);

            PenalityManager.Instance.setCombos();
            Mulligan m = Mulligan.Instance; // read the mulligan list
            Discovery d = Discovery.Instance; // read the discover list
            Settings.Instance.setSettings();
            if (Settings.Instance.useNetwork) FishNet.Instance.startClient();
            HelpFunctions.Instance.startFlushingLogBuffers();
        }

        public void SetnewLogFile()
        {
            Questmanager.Instance.Reset();
            OwnCrystalCore = 0;
            EnemyCrystalCore = 0;
            ownMinionsCost0 = false;

            HelpFunctions.Instance.flushLogg(); // flush the buffer before creating a new log
            if (!SingleLog)
            {
                sttngs.setLoggFile("SilverLog" + DateTime.Now.ToString("_yyyy-MM-dd_HH-mm-ss") + ".txt");
                HelpFunctions.Instance.createNewLoggfile();
                HelpFunctions.Instance.ErrorLog("#######################################################");
                HelpFunctions.Instance.ErrorLog("fight is logged in: " + sttngs.logpath + sttngs.logfile);
                HelpFunctions.Instance.ErrorLog("#######################################################");
            }
            else
            {
                sttngs.setLoggFile("UILogg.txt");
            }
        }

        public void SetNewGame()
        {
            //todo sepefeets - move stuff here to make things more consistant between HR/HB versions
        }

        public bool UpdateEverything(HSRangerLib.BotBase rangerbot, Behavior botbase, bool queueActions, bool runExtern = false, bool passiveWait = false)
        {
            // data sync workaround for temp buffs - something is wrong
            /*if (lastpf != null && lastpf.playactions[0].actionType == actionEnum.playcard && (lastpf.playactions[0].card.card.name == CardDB.cardName.savageroar ||
                lastpf.playactions[0].card.card.name == CardDB.cardName.bloodlust || PenalityManager.Instance.buffing1TurnDatabase.ContainsKey(lastpf.playactions[0].card.card.name)))
            {
                System.Threading.Thread.Sleep(1500);
            }*/

            //string ename = "" + Ai.Instance.bestmove.card.card.name;

            Ai daum = Ai.Instance;

            if (daum.bestmove != null)// || daum.bestmove.actionType != actionEnum.endturn || daum.bestmove.actionType != actionEnum.useHeroPower)
            //if (daum.bestActions[0].actionType != actionEnum.endturn  && daum.bestActions.Count >= 1 && Playfield.Instance.turnCounter == 0)
            {
                switch (daum.bestmove.actionType)
                {
                    case ActionType.END_TURN:
                        //Helpfunctions.Instance.logg("엔드턴 확인  ");
                        break;
                    case ActionType.ATTACK_WITH_HERO:
                        {
                            if (rangerbot.gameState.TimerState != TurnTimerState.COUNTDOWN)
                            {
                                bool targethasdamageeffect = false;
                                switch (daum.bestmove.target.name)
                                {
                                    case CardDB.cardName.impgangboss:
                                        targethasdamageeffect = true; break;
                                    case CardDB.cardName.dragonegg:
                                        targethasdamageeffect = true; break;
                                    case CardDB.cardName.hoggerdoomofelwynn:
                                        targethasdamageeffect = true; break;
                                    case CardDB.cardName.grimpatron:
                                        targethasdamageeffect = true; break;
                                    default:
                                        break;
                                }

                                if (targethasdamageeffect && !daum.bestmove.target.isHero)
                                {
                                    System.Threading.Thread.Sleep(3500);
                                }

                                if (daum.bestmove.target.hasDeathrattle() && daum.bestmove.own.Angr >= daum.bestmove.target.Hp && !daum.bestmove.target.isHero)
                                {
                                    System.Threading.Thread.Sleep(2200);
                                }

                                if (daum.bestmove.target.taunt && daum.bestmove.own.Angr >= daum.bestmove.target.Hp && !daum.bestmove.target.isHero)
                                {
                                    System.Threading.Thread.Sleep(800);
                                    //Helpfunctions.Instance.logg("Target Taunt detected sleep 800ms");
                                    //Helpfunctions.Instance.logg("Target Taunt detected sleep 800ms");
                                    //Helpfunctions.Instance.ErrorLog("Target Taunt detected sleep 800ms");
                                    //Helpfunctions.Instance.ErrorLog("Target Taunt detected sleep 800ms");
                                }

                                if (this.enemySecretCount >= 1)
                                {
                                    int time = 5000 / this.enemySecretCount;
                                    foreach (SecretItem si in Probabilitymaker.Instance.enemySecrets)
                                    {
                                        if (si.canBe_noblesacrifice)
                                        {
                                            System.Threading.Thread.Sleep(time);
                                            HelpFunctions.Instance.logg("찾는거 덫발견 덫8 time: " + time);
                                            HelpFunctions.Instance.ErrorLog("찾는거 덫발견 덫8 time: " + time);
                                        }
                                        else if (daum.bestmove.target.isHero)
                                        {
                                            if (si.canBe_explosive
                                                //|| si.canBe_icebarrier
                                                || si.canBe_beartrap)
                                            {
                                                System.Threading.Thread.Sleep(time);
                                                HelpFunctions.Instance.logg("찾는거 덫발견 덫9 time: " + time);
                                                HelpFunctions.Instance.ErrorLog("찾는거 덫발견 덫9 time: " + time);
                                            }
                                        }
                                        else if (!daum.bestmove.target.isHero)
                                        {
                                            if (si.canBe_snaketrap)
                                            {
                                                System.Threading.Thread.Sleep(time);
                                                HelpFunctions.Instance.logg("찾는거 덫발견 덫10 time: " + time);
                                                HelpFunctions.Instance.ErrorLog("찾는거 덫발견 덫10 time: " + time);
                                            }
                                            else if (daum.bestmove.own.Angr >= daum.bestmove.target.Hp)
                                            {
                                                if (si.canBe_iceblock)
                                                {
                                                    System.Threading.Thread.Sleep(time);
                                                    HelpFunctions.Instance.logg("찾는거 덫발견 덫13 time: " + time);
                                                    HelpFunctions.Instance.ErrorLog("찾는거 덫발견 덫13 time: " + time);
                                                }
                                            }

                                        }
                                        else if (daum.bestmove.own.Angr >= daum.bestmove.target.Hp && !daum.bestmove.target.isHero)
                                        {
                                            if (si.canBe_effigy
                                                || si.canBe_redemption
                                                || si.canBe_avenge
                                                || si.canBe_duplicate)
                                            {
                                                System.Threading.Thread.Sleep(time);
                                                HelpFunctions.Instance.logg("찾는거 덫발견 덫11 time: " + time);
                                                HelpFunctions.Instance.ErrorLog("찾는거 덫발견 덫11 time: " + time);
                                            }
                                        }
                                    }


                                }





                            }
                            break;
                        }
                    case ActionType.ATTACK_WITH_MINION:
                        {
                            //System.Threading.Thread.Sleep(55);
                            //Helpfunctions.Instance.logg("미니언공격 확인  " + daum.bestmove.own.name + " 으로 공격 " + daum.bestmove.target.name);
                            //Helpfunctions.Instance.ErrorLog("미니언공격 확인  " + daum.bestmove.own.name + " 으로 공격 " + daum.bestmove.target.name);

                            if (rangerbot.gameState.TimerState != TurnTimerState.COUNTDOWN)
                            {
                                bool targethasdamageeffect = false;
                                switch (daum.bestmove.target.name)
                                {
                                    case CardDB.cardName.impgangboss:
                                        targethasdamageeffect = true; break;
                                    case CardDB.cardName.dragonegg:
                                        targethasdamageeffect = true; break;
                                    case CardDB.cardName.hoggerdoomofelwynn:
                                        targethasdamageeffect = true; break;
                                    case CardDB.cardName.grimpatron:
                                        targethasdamageeffect = true; break;
                                    default:
                                        break;
                                }

                                switch (daum.bestmove.own.name)
                                {
                                    case CardDB.cardName.impgangboss:
                                        targethasdamageeffect = true; break;
                                    case CardDB.cardName.dragonegg:
                                        targethasdamageeffect = true; break;
                                    case CardDB.cardName.hoggerdoomofelwynn:
                                        targethasdamageeffect = true; break;
                                    case CardDB.cardName.grimpatron:
                                        targethasdamageeffect = true; break;
                                    default:
                                        break;
                                }

                                if (targethasdamageeffect && !daum.bestmove.target.isHero)
                                {
                                    System.Threading.Thread.Sleep(5200);
                                    //Helpfunctions.Instance.logg("타겟 맞을떄마다 소환합니다 sleep 3500ms");
                                    //Helpfunctions.Instance.logg("타겟 맞을떄마다 소환합니다 3500ms");
                                    //Helpfunctions.Instance.ErrorLog("타겟 맞을떄마다 소환합니다 1200ms");
                                    //Helpfunctions.Instance.ErrorLog("타겟 맞을떄마다 소환합니다 1200ms");
                                }






                                bool hastargetdeathrattle = (daum.bestmove.target.hasDeathrattle() || daum.bestmove.target.deathrattles.Count >= 1 || (daum.bestmove.target.handcard.card.deathrattle && !daum.bestmove.target.silenced)) && !daum.bestmove.target.isHero;
                                //Helpfunctions.Instance.logg("hasdeathrattle " + hastargetdeathrattle);

                                bool hasowndeathrattle = (daum.bestmove.own.hasDeathrattle() || daum.bestmove.own.deathrattles.Count >= 1 || (daum.bestmove.own.handcard.card.deathrattle && !daum.bestmove.own.silenced)) && !daum.bestmove.target.isHero;
                                //Helpfunctions.Instance.logg("hasdeathrattle " + hasowndeathrattle);




                                if (hastargetdeathrattle && daum.bestmove.own.Angr >= daum.bestmove.target.Hp && !daum.bestmove.target.isHero)
                                {
                                    System.Threading.Thread.Sleep(3500);
                                    //Helpfunctions.Instance.logg("Target deathrattle detected sleep 3500ms");
                                    //Helpfunctions.Instance.ErrorLog("Target deathrattle detected sleep 3500ms");
                                }

                                if (hasowndeathrattle && daum.bestmove.own.Hp <= daum.bestmove.target.Angr && !daum.bestmove.target.isHero)
                                {
                                    bool spawnMinions = false;
                                    bool hasDeathrattleBeast = false;
                                    switch (daum.bestmove.own.name)
                                    {
                                        case CardDB.cardName.harvestgolem: spawnMinions = true; break;
                                        case CardDB.cardName.hauntedcreeper: spawnMinions = true; break;
                                        case CardDB.cardName.nerubianegg: spawnMinions = true; break;
                                        case CardDB.cardName.savannahhighmane: spawnMinions = true; hasDeathrattleBeast = true; break;
                                        case CardDB.cardName.sludgebelcher: spawnMinions = true; break;
                                        case CardDB.cardName.thebeast: spawnMinions = true; break;
                                        case CardDB.cardName.cairnebloodhoof: spawnMinions = true; break;
                                        case CardDB.cardName.feugen: spawnMinions = true; break;
                                        case CardDB.cardName.stalagg: spawnMinions = true; break;
                                        case CardDB.cardName.infestedtauren: spawnMinions = true; break;
                                        case CardDB.cardName.infestedwolf: spawnMinions = true; hasDeathrattleBeast = true; break;
                                        case CardDB.cardName.dreadsteed: spawnMinions = true; break;
                                        case CardDB.cardName.voidcaller: spawnMinions = true; break;
                                        case CardDB.cardName.pilotedshredder: spawnMinions = true; break;
                                        case CardDB.cardName.pilotedskygolem: spawnMinions = true; break;
                                        case CardDB.cardName.mountedraptor: spawnMinions = true; break;
                                        case CardDB.cardName.sneedsoldshredder: spawnMinions = true; break;
                                        case CardDB.cardName.anubarak: spawnMinions = true; break;
                                        case CardDB.cardName.possessedvillager: spawnMinions = true; break;
                                        case CardDB.cardName.twilightsummoner: spawnMinions = true; break;
                                        case CardDB.cardName.wobblingrunts: spawnMinions = true; break;
                                        case CardDB.cardName.moatlurker: spawnMinions = true; break;
                                        case CardDB.cardName.kindlygrandmother: spawnMinions = true; hasDeathrattleBeast = true; break;
                                        default:
                                            break;
                                    }
                                    foreach (Minion mnn in this.ownMinions)
                                    {
                                        if (mnn.name == CardDB.cardName.knifejuggler && !mnn.silenced && spawnMinions)
                                        {
                                            System.Threading.Thread.Sleep(1500);
                                            //Helpfunctions.Instance.logg("저글러 own minion's deathrattle detected sleep 1500ms");
                                            //Helpfunctions.Instance.ErrorLog("저글러 own minion's deathrattle detected sleep 1500ms");
                                        }
                                        if (mnn.name == CardDB.cardName.tundrarhino && !mnn.silenced && hasDeathrattleBeast)
                                        {
                                            System.Threading.Thread.Sleep(500);
                                            //Helpfunctions.Instance.logg("라이노 own minion's deathrattle detected sleep 500ms");
                                            //Helpfunctions.Instance.ErrorLog("라이노 own minion's deathrattle detected sleep 500ms");
                                        }
                                    }


                                    //System.Threading.Thread.Sleep(1500);
                                    //Helpfunctions.Instance.logg("own minion's deathrattle detected sleep 1500ms");
                                    //Helpfunctions.Instance.ErrorLog("own minion's deathrattle detected sleep 1500ms");
                                }

                                //if (daum.bestmove.target.taunt && daum.bestmove.own.Angr >= daum.bestmove.target.Hp && !daum.bestmove.target.isHero)
                                //{
                                //    System.Threading.Thread.Sleep(500);
                                //    Helpfunctions.Instance.logg("Target Taunt detected sleep 500ms");
                                //    Helpfunctions.Instance.logg("Target Taunt detected sleep 500ms");
                                //    Helpfunctions.Instance.ErrorLog("Target Taunt detected sleep 500ms");
                                //    Helpfunctions.Instance.ErrorLog("Target Taunt detected sleep 500ms");
                                //}


                                bool hashyena = false;
                                bool cultmaster = false;
                                foreach (Minion mnn in this.ownMinions)
                                {
                                    if (mnn.name == CardDB.cardName.scavenginghyena && !mnn.silenced && mnn.Ready)
                                    {
                                        hashyena = true;
                                    }
                                    if (mnn.name == CardDB.cardName.cultmaster && !mnn.silenced) cultmaster = true;
                                }

                                if (hashyena && daum.bestmove.own.Hp <= daum.bestmove.target.Angr && !daum.bestmove.target.isHero && (TAG_RACE)daum.bestmove.own.handcard.card.race == TAG_RACE.BEAST)
                                {
                                    System.Threading.Thread.Sleep(1500);
                                }

                                if (cultmaster && daum.bestmove.own.Hp <= daum.bestmove.target.Angr && !daum.bestmove.target.isHero)
                                {
                                    System.Threading.Thread.Sleep(2200);
                                }




                                if (this.enemySecretCount >= 1)
                                {
                                    int time = 5000 / this.enemySecretCount;
                                    foreach (SecretItem si in Probabilitymaker.Instance.enemySecrets)
                                    {
                                        if (si.canBe_noblesacrifice
                                       || si.canBe_freezing)
                                        {
                                            System.Threading.Thread.Sleep(time * 4 / 3);
                                            HelpFunctions.Instance.logg("찾는거 덫발견 덫1 슬립 time: " + time * 4 / 3);
                                            HelpFunctions.Instance.ErrorLog("찾는거 덫발견 덫1 time: " + time * 4 / 3);
                                        }

                                        else if (daum.bestmove.target.isHero)
                                        {
                                            if (si.canBe_explosive
                                                || si.canBe_beartrap
                                                //|| si.canBe_icebarrier
                                                || si.canBe_vaporize)
                                            {
                                                System.Threading.Thread.Sleep(time);
                                                HelpFunctions.Instance.logg("찾는거 덫발견 덫2 time: " + time);
                                                HelpFunctions.Instance.ErrorLog("찾는거 덫발견 덫2 time: " + time);
                                            }
                                        }
                                        else if (!daum.bestmove.target.isHero)
                                        {
                                            if (si.canBe_snaketrap)
                                            {
                                                System.Threading.Thread.Sleep(time);
                                                HelpFunctions.Instance.logg("찾는거 덫발견 덫3 time: " + time);
                                                HelpFunctions.Instance.ErrorLog("찾는거 덫발견 덫3 time: " + time);
                                            }
                                            else if (daum.bestmove.own.Angr >= daum.bestmove.target.Hp)
                                            {
                                                if (si.canBe_iceblock)
                                                {
                                                    System.Threading.Thread.Sleep(time);
                                                    HelpFunctions.Instance.logg("찾는거 덫발견 덫13 time: " + time);
                                                    HelpFunctions.Instance.ErrorLog("찾는거 덫발견 덫13 time: " + time);
                                                }
                                            }
                                        }
                                        else if (daum.bestmove.own.Angr >= daum.bestmove.target.Hp)
                                        {
                                            if (si.canBe_effigy
                                                || si.canBe_redemption
                                                || si.canBe_avenge
                                                || si.canBe_duplicate)
                                            {
                                                System.Threading.Thread.Sleep(time);
                                                HelpFunctions.Instance.logg("찾는거 덫발견 덫7 time: " + time);
                                                HelpFunctions.Instance.ErrorLog("찾는거 덫발견 덫7 time: " + time);
                                            }
                                        }
                                    }


                                }
                            }


                            break;
                        }
                    case ActionType.PLAY_CARD:

                        if (rangerbot.gameState.TimerState != TurnTimerState.COUNTDOWN)
                        {
                            if (this.enemySecretCount >= 1)
                            {
                                int time = 5000 / this.enemySecretCount;
                                foreach (SecretItem si in Probabilitymaker.Instance.enemySecrets)
                                {
                                    if (daum.bestmove.card.card.type == CardDB.cardtype.MOB)
                                    {
                                        if (si.canBe_mirrorentity)
                                        {
                                            System.Threading.Thread.Sleep(time);
                                            HelpFunctions.Instance.logg("찾는거 덫발견 덫4 time: " + time);
                                            HelpFunctions.Instance.ErrorLog("찾는거 덫발견 덫4 time: " + time);
                                        }
                                        else if ((si.canBe_snipe)
                                                || (si.canBe_Trial && this.ownMinions.Count >= 3))
                                        {
                                            System.Threading.Thread.Sleep(time);
                                            HelpFunctions.Instance.logg("찾는거 덫발견 덫5 time: " + time);
                                            HelpFunctions.Instance.ErrorLog("찾는거 덫발견 덫5 time: " + time);
                                        }
                                        //else if (daum.bestmove.card.card.Charge)
                                        //{
                                        //    if ((si.canBe_snipe && daum.bestmove.card.card.Health <= 4)
                                        //        || (si.canBe_Trial && this.ownMinions.Count >= 3))
                                        //    {
                                        //        System.Threading.Thread.Sleep(time);
                                        //        Helpfunctions.Instance.logg("찾는거 덫발견 덫5 time: " + time);
                                        //        Helpfunctions.Instance.ErrorLog("찾는거 덫발견 덫5 time: " + time);
                                        //    }
                                        //}
                                    }
                                    else if (daum.bestmove.card.card.type == CardDB.cardtype.SPELL)
                                    {
                                        if (si.canBe_counterspell
                                                || (si.canBe_spellbender && daum.bestmove.target != null && !daum.bestmove.target.isHero)
                                                || si.canBe_cattrick)
                                        {
                                            System.Threading.Thread.Sleep(time);
                                            HelpFunctions.Instance.logg("찾는거 덫발견 덫6 time: " + time);
                                            HelpFunctions.Instance.ErrorLog("찾는거 덫발견 덫6 time: " + time);
                                        }
                                        else if (this.enemyMinions.Count >= 1 &&
                                            (PenalityManager.Instance.DamageAllDatabase.ContainsKey(daum.bestmove.card.card.name)
                                            || PenalityManager.Instance.DamageRandomDatabase.ContainsKey(daum.bestmove.card.card.name)
                                            || PenalityManager.Instance.DamageAllEnemysDatabase.ContainsKey(daum.bestmove.card.card.name)
                                            || PenalityManager.Instance.DamageAllDatabase.ContainsKey(daum.bestmove.card.card.name)
                                            || PenalityManager.Instance.DamageTargetDatabase.ContainsKey(daum.bestmove.card.card.name)
                                            || PenalityManager.Instance.DamageTargetSpecialDatabase.ContainsKey(daum.bestmove.card.card.name)))
                                        {
                                            if (si.canBe_effigy
                                                || si.canBe_redemption
                                                || si.canBe_avenge
                                                || si.canBe_duplicate)
                                            {
                                                System.Threading.Thread.Sleep(time);
                                                HelpFunctions.Instance.logg("찾는거 덫발견 덫7 time: " + time);
                                                HelpFunctions.Instance.ErrorLog("찾는거 덫발견 덫7 time: " + time);
                                            }
                                        }
                                    }
                                }
                            }

                            if (daum.bestmove.card.card.battlecry)
                            {
                                //Helpfunctions.Instance.logg("배틀크라이 발견!!!!!!!");
                                //Helpfunctions.Instance.ErrorLog("배틀크라이 발견!!!!!!!");

                                switch (daum.bestmove.card.card.name)
                                {
                                    case CardDB.cardName.defenderofargus: System.Threading.Thread.Sleep(2200); break;
                                    case CardDB.cardName.kingselekk: System.Threading.Thread.Sleep(8200); break;
                                    case CardDB.cardName.barnes: System.Threading.Thread.Sleep(5200); break;
                                    case CardDB.cardName.tuskarrtotemic: System.Threading.Thread.Sleep(5500); break;
                                    case CardDB.cardName.thecurator: System.Threading.Thread.Sleep(4200); break;
                                    default: break;
                                }
                                System.Threading.Thread.Sleep(2300);
                            }
                            else if (daum.bestmove.card.card.Charge)
                            {
                                System.Threading.Thread.Sleep(2200);
                            }
                            else
                            {
                                switch (daum.bestmove.card.card.name)
                                {
                                    case CardDB.cardName.southseadeckhand:
                                        if (this.heroWeaponAttack >= 1)
                                        {
                                            System.Threading.Thread.Sleep(3200);
                                        }
                                        break;
                                    //case CardDB.cardName.: System.Threading.Thread.Sleep(2200); break;
                                    default: break;
                                }
                            }

                            if (daum.bestmove.card.card.type == CardDB.cardtype.MOB || PenalityManager.Instance.summonMinionSpellsDatabase.ContainsKey(daum.bestmove.card.card.name))
                            {
                                int juggler_councilman_count = 0;
                                bool juggleronfield = false;
                                foreach (Minion mnn in this.ownMinions)
                                {
                                    if (!mnn.silenced && ((mnn.name == CardDB.cardName.darkshirecouncilman && !mnn.playedThisTurn) || mnn.name == CardDB.cardName.knifejuggler))
                                    {
                                        juggler_councilman_count++;
                                        if (mnn.name == CardDB.cardName.knifejuggler) juggleronfield = true;
                                    }
                                }
                                if (juggler_councilman_count >= 1 && this.enemyMinions.Count >= 1)
                                {
                                    int time = 2100 * juggler_councilman_count;
                                    if (juggleronfield) time = time * 13 / 10;
                                    if (daum.bestmove.card.card.name == CardDB.cardName.forbiddenritual)
                                    {
                                        time = time * (Math.Min(7 - this.ownMinions.Count, this.currentMana));
                                        System.Threading.Thread.Sleep(time);
                                        //Helpfunctions.Instance.logg("darkshirecouncilman or knifejuggler effect detected sleep" + time + "ms");
                                        //Helpfunctions.Instance.logg("darkshirecouncilman or knifejuggler effect detected sleep" + time + "ms");
                                        //Helpfunctions.Instance.ErrorLog("darkshirecouncilman or knifejuggler effect detected sleep" + time + "ms");
                                        //Helpfunctions.Instance.ErrorLog("darkshirecouncilman or knifejuggler effect detected sleep" + time + "ms");
                                    }
                                    else if (daum.bestmove.card.card.name == CardDB.cardName.unleashthehounds)
                                    {
                                        time = time * (Math.Min(7 - this.ownMinions.Count, this.enemyMinions.Count));
                                        time = time * 13 / 10;
                                        System.Threading.Thread.Sleep(time);
                                        //Helpfunctions.Instance.logg("darkshirecouncilman or knifejuggler effect detected sleep" + time + "ms");
                                        //Helpfunctions.Instance.logg("darkshirecouncilman or knifejuggler effect detected sleep" + time + "ms");
                                        //Helpfunctions.Instance.ErrorLog("darkshirecouncilman or knifejuggler effect detected sleep" + time + "ms");
                                        //Helpfunctions.Instance.ErrorLog("darkshirecouncilman or knifejuggler effect detected sleep" + time + "ms");
                                    }
                                    else if (PenalityManager.Instance.summonMinionSpellsDatabase.ContainsKey(daum.bestmove.card.card.name))
                                    {
                                        time = time * daum.bestmove.card.card.Summon_Spell_Minion_Count;
                                        System.Threading.Thread.Sleep(time);
                                        HelpFunctions.Instance.logg("darkshirecouncilman or knifejuggler effect detected sleep " + time + "ms" + daum.bestmove.card.card.Summon_Spell_Minion_Count + "enemy minions");
                                        HelpFunctions.Instance.logg("darkshirecouncilman or knifejuggler effect detected sleep " + time + "ms" + daum.bestmove.card.card.Summon_Spell_Minion_Count + "enemy minions");
                                        HelpFunctions.Instance.ErrorLog("darkshirecouncilman or knifejuggler effect detected sleep " + time + "ms" + daum.bestmove.card.card.Summon_Spell_Minion_Count + "enemy minions");
                                        HelpFunctions.Instance.ErrorLog("darkshirecouncilman or knifejuggler effect detected sleep " + time + "ms" + daum.bestmove.card.card.Summon_Spell_Minion_Count + "enemy minions");
                                    }
                                    else
                                    {
                                        System.Threading.Thread.Sleep(time);
                                    }
                                    break;
                                }
                            }


                            if (daum.bestmove.card.card.type == CardDB.cardtype.SPELL)
                            {
                                System.Threading.Thread.Sleep(1200);

                                if (daum.bestmove.target != null)
                                {
                                    bool hastargetdeathrattle = (daum.bestmove.target.hasDeathrattle() || daum.bestmove.target.deathrattles.Count >= 1 || (daum.bestmove.target.handcard.card.deathrattle && !daum.bestmove.target.silenced)) && !daum.bestmove.target.isHero;
                                    if (hastargetdeathrattle)
                                    {
                                        System.Threading.Thread.Sleep(3500);
                                    }

                                    if (PenalityManager.Instance.NeedSleepBecauseTimingMinionsDB.ContainsKey(daum.bestmove.target.name))
                                    {
                                        int timing = PenalityManager.Instance.NeedSleepBecauseTimingMinionsDB[daum.bestmove.target.name] * 1050;
                                        System.Threading.Thread.Sleep(timing);
                                    }

                                }
                                if (daum.bestmove.card.card.name == CardDB.cardName.callofthewild)
                                {
                                    System.Threading.Thread.Sleep(2200);
                                }
                                //적시바 나 데드리샷

                                if (daum.bestmove.card.card.name == CardDB.cardName.deadlyshot)
                                {
                                    foreach (Minion mnn in this.enemyMinions)
                                    {
                                        if (mnn.name == CardDB.cardName.sylvanaswindrunner && !mnn.silenced)
                                        {
                                            System.Threading.Thread.Sleep(4500);
                                        }
                                    }
                                }
                                if (daum.bestmove.card.card.name == CardDB.cardName.quickshot && this.anzcards == 1)
                                {
                                    int pow = 3200;
                                    System.Threading.Thread.Sleep(pow);
                                }
                                if (daum.bestmove.card.card.name == CardDB.cardName.lightningstorm)
                                {
                                    int pow = 2200;
                                    if (this.enemyMinions.Count >= 1) pow = pow * this.enemyMinions.Count * 3 / 4;
                                    System.Threading.Thread.Sleep(pow);
                                }
                                if (daum.bestmove.card.card.name == CardDB.cardName.lightningbolt)
                                {
                                    System.Threading.Thread.Sleep(1500);
                                }
                                if (daum.bestmove.card.card.name == CardDB.cardName.feralspirit)
                                {
                                    System.Threading.Thread.Sleep(4500);
                                }
                                if (daum.bestmove.card.card.name == CardDB.cardName.rockbiterweapon)
                                {
                                    System.Threading.Thread.Sleep(1500);
                                }
                                if (daum.bestmove.card.card.name == CardDB.cardName.maelstromportal)
                                {
                                    System.Threading.Thread.Sleep(2500);
                                }
                                if (daum.bestmove.card.card.name == CardDB.cardName.hex)
                                {
                                    System.Threading.Thread.Sleep(1500);
                                }
                                if (daum.bestmove.card.card.name == CardDB.cardName.bloodlust)
                                {
                                    System.Threading.Thread.Sleep(1500);
                                }

                                switch (daum.bestmove.card.card.name)
                                {
                                    case CardDB.cardName.upgrade: System.Threading.Thread.Sleep(1500); break;
                                    default: break;
                                }
                            }

                            //shaman 1cost trogg
                            bool hastrogg = false;
                            foreach (Minion mnn in this.ownMinions)
                            {
                                if (mnn.name == CardDB.cardName.tunneltrogg && !mnn.silenced && mnn.Ready)
                                {
                                    hastrogg = true;
                                }
                            }
                            if (hastrogg && daum.bestmove.card.card.overload >= 1) //overload
                            {
                                System.Threading.Thread.Sleep(1500);
                            }


                            if (PenalityManager.Instance.cardDrawBattleCryDatabase.ContainsKey(daum.bestmove.card.card.name)) //small sleep drawcards. sometimes just turn end
                            {
                                System.Threading.Thread.Sleep(800);
                            }

                            if (PenalityManager.Instance.AdaptDatabase.ContainsKey(daum.bestmove.card.card.name) ||
                                PenalityManager.Instance.discoverCards.ContainsKey(daum.bestmove.card.card.name)) //small sleep adapt/discover cards.
                            {
                                System.Threading.Thread.Sleep(1200);
                            }

                        }



                        break;
                    case ActionType.USE_HERO_POWER:
                        //if (this.enemySecretCount >= 1)
                        //{
                        //    int time = 6000 / this.enemySecretCount;
                        //    foreach (SecretItem si in Probabilitymaker.Instance.enemySecrets)
                        //    {
                        //        if (si.canBe_Dart)
                        //        {
                        //            System.Threading.Thread.Sleep(time);
                        //            Helpfunctions.Instance.logg("찾는거 덫발견 덫21 time: " + time);
                        //            Helpfunctions.Instance.ErrorLog("찾는거 덫발견 덫21 time: " + time);
                        //        }
                        //    }
                        //}
                        //if (heroAbility.name == CardDB.cardName.shapeshift || heroAbility.name == CardDB.cardName.direshapeshift) System.Threading.Thread.Sleep(1200);
                        //if (rangerbot.gameState.TimerState != TurnTimerState.COUNTDOWN) System.Threading.Thread.Sleep(1800);
                        break;
                    default:
                        break;
                }
            }


            HelpFunctions.Instance.ErrorLog("updateEverything");
            latestGameState = rangerbot.gameState;

            this.UpdateBehaveString(botbase);

            Entity ownPlayer = rangerbot.FriendPlayer;
            Entity enemyPlayer = rangerbot.EnemyPlayer;
            ownPlayerController = ownPlayer.ControllerId;//ownPlayer.GetHero().GetControllerId()

            Hrtprozis.Instance.clearAll();
            Handmanager.Instance.clearAll();

            // create hero + minion data
            GetHerostuff(rangerbot);
            GetMinions(rangerbot);
            GetHandcards(rangerbot);
            GetDecks(rangerbot);
            CorrectSpellpower(rangerbot);

            Hrtprozis.Instance.setOwnPlayer(ownPlayerController);
            Handmanager.Instance.setOwnPlayer(ownPlayerController);

            this.numOptionPlayedThisTurn = 0;
            this.numOptionPlayedThisTurn += this.cardsPlayedThisTurn + this.ownHero.numAttacksThisTurn;
            foreach (Minion m in this.ownMinions)
            {
                if (m.Hp >= 1) this.numOptionPlayedThisTurn += m.numAttacksThisTurn;
            }

            Hrtprozis.Instance.updatePlayer(this.ownMaxMana, this.currentMana, this.cardsPlayedThisTurn, this.numMinionsPlayedThisTurn, this.numOptionPlayedThisTurn, this.ownOverload, ownHero.entityID, enemyHero.entityID, this.numberMinionsDiedThisTurn, this.ownCurrentOverload, this.enemyOverload, this.heroPowerUsesThisTurn, this.lockandload, this.Stampede);
            Hrtprozis.Instance.setPlayereffects(this.ownDragonConsort, this.enemyDragonConsort, this.ownLoathebs, this.enemyLoathebs, this.ownMillhouse, this.enemyMillhouse, this.ownKirintor, this.ownPrepa, this.ownsabo, this.enemysabo, this.ownFenciCoaches, this.enemyCursedCardsInHand);
            Hrtprozis.Instance.updateSecretStuff(this.ownSecretList, this.enemySecretCount);


            Hrtprozis.Instance.updateOwnHero(this.ownHeroWeapon, this.heroWeaponAttack, this.heroWeaponDurability, this.heroname, this.heroAbility, this.ownAbilityisReady, this.ownHero, this.ownHeroPowerUsesThisGame);
            Hrtprozis.Instance.updateEnemyHero(this.enemyHeroWeapon, this.enemyWeaponAttack, this.enemyWeaponDurability, this.enemyHeroname, this.enemyMaxMana, this.enemyAbility, this.enemyHero, this.enemyHeroPowerUsesThisGame);

            Hrtprozis.Instance.updateMinions(this.ownMinions, this.enemyMinions);
            Handmanager.Instance.setHandcards(this.handCards, this.anzcards, this.enemyAnzCards, this.choiceCards);

            Hrtprozis.Instance.updateFatigueStats(this.ownDecksize, this.ownHeroFatigue, this.enemyDecksize, this.enemyHeroFatigue);
            Hrtprozis.Instance.updateJadeGolemsInfo(ownPlayer.GetTagValue((int)GAME_TAG.JADE_GOLEM), enemyPlayer.GetTagValue((int)GAME_TAG.JADE_GOLEM));
            Probabilitymaker.Instance.getEnemySecretGuesses(this.enemySecretList, Hrtprozis.Instance.heroNametoEnum(this.enemyHeroname));

            //Hrtprozis.Instance.updateCrystalCore(OwnCrystalCore, EnemyCrystalCore);
            Hrtprozis.Instance.updateOwnMinionsCost0(this.ownMinionsCost0);

            //learnmode :D

            Playfield p = new Playfield();


            if (!queueActions)
            {
                if (lastpf != null)
                {
                    if (lastpf.isEqualf(p))
                    {
                        return false;
                    }

                    //board changed we update secrets!
                    //if(Ai.Instance.nextMoveGuess!=null) Probabilitymaker.Instance.updateSecretList(Ai.Instance.nextMoveGuess.enemySecretList);
                    Probabilitymaker.Instance.updateSecretList(p, lastpf);
                    Bot.Instance.dontmultiactioncount = 0;
                    Bot.Instance.POWERFULSINGLEACTION = 0;
                    Bot.Instance.DoMultipleThingsAtATime = true;
                }
            }
            else
            {
                //queue stuff 
                if (lastpf != null)
                {
                    if (lastpf.isEqualf(p))
                    {
                        ((Bot)rangerbot).ShouldSendActions = false;  // let the bot know we haven't updated any actions
                        return false;
                    }

                    //board changed we update secrets!
                    //if(Ai.Instance.nextMoveGuess!=null) Probabilitymaker.Instance.updateSecretList(Ai.Instance.nextMoveGuess.enemySecretList);
                    Probabilitymaker.Instance.updateSecretList(p, lastpf);
                    Bot.Instance.dontmultiactioncount = 1;
                    Bot.Instance.POWERFULSINGLEACTION = 0;
                    Bot.Instance.DoMultipleThingsAtATime = false;
                }

            }

            lastpf = p;
            p = new Playfield();//secrets have updated :D
            // calculate stuff

            /*foreach (Handmanager.Handcard hc in p.owncards)
            {
                Helpfunctions.Instance.ErrorLog("hc playfield" + hc.manacost + " " + hc.getManaCost(p));
            }*/

            if (queueActions)
            {
                // Detect errors in HearthRanger execution of our last set of actions and try to fix it so we don't
                // have to re-calculate the entire turn.
                Bot currentBot = (Bot)rangerbot;
                if (currentBot.NumActionsSent > currentBot.NumExecsReceived && !p.isEqualf(Ai.Instance.nextMoveGuess))
                {
                    HelpFunctions.Instance.ErrorLog("HR action queue did not complete!");
                    HelpFunctions.Instance.logg("board state out-of-sync due to action queue!");

                    //if (Ai.Instance.restoreBestMoves(p, currentBot.queuedMoveGuesses))
                    //{
                    //    Helpfunctions.Instance.logg("rolled back state to replay queued actions.");
                    //    Helpfunctions.Instance.ErrorLog("#queue-rollback#");
                    //}
                }
            }
            if (p.mana > Ai.Instance.nextMoveGuess.mana && p.ownMaxMana > Ai.Instance.nextMoveGuess.ownMaxMana && Ai.Instance.bestActions.Count > 0)
            {
                HelpFunctions.Instance.logg("You may have roped last turn!");
                //Helpfunctions.Instance.logg("Mana: " + p.mana + ">" + Ai.Instance.nextMoveGuess.mana);
                //Helpfunctions.Instance.logg("Max Mana: " + p.ownMaxMana + ">" + Ai.Instance.nextMoveGuess.ownMaxMana);
                //Helpfunctions.Instance.logg("Actions left: " + Ai.Instance.bestActions.Count);
            }

            HelpFunctions.Instance.ErrorLog("calculating stuff... " + DateTime.Now.ToString("HH:mm:ss.ffff"));
            if (runExtern)
            {
                HelpFunctions.Instance.logg("recalc-check###########");
                //p.printBoard();
                //Ai.Instance.nextMoveGuess.printBoard();
                if (p.isEqual(Ai.Instance.nextMoveGuess, true))
                {
                    PrintStuff(p, false);
                    Ai.Instance.doNextCalcedMove();
                }
                else
                {
                    List<Handmanager.Handcard> newcards = p.getNewHandCards(Ai.Instance.nextMoveGuess);
                    foreach (var card in newcards)
                    {
                        if (!IsCardCreated(card)) Hrtprozis.Instance.removeCardFromTurnDeck(card.card.cardIDenum);
                    }

                    PrintStuff(p, true);
                    ReadActionFile(passiveWait);
                }
            }
            else
            {
                PrintStuff(p, false);
                Ai.Instance.dosomethingclever(botbase);
            }

            HelpFunctions.Instance.ErrorLog("calculating ended! " + DateTime.Now.ToString("HH:mm:ss.ffff"));

            return true;
        }

        public bool IsCardCreated(Handmanager.Handcard handcard)
        {
            foreach (var card in latestGameState.GameEntityList)
            {
                if (card.EntityId == handcard.entity)
                {
                    if (card.CreatorId != 0) return true;
                    else return false;
                }
            }
            return false;
        }


        private void GetHerostuff(HSRangerLib.BotBase rangerbot)
        {

            //TODO GET HERO POWER USES!!!!!!
            //heroPowerUsesThisTurn = 0;
            //ownHeroPowerUsesThisGame = 0;
            //enemyHeroPowerUsesThisGame = 0;

            //reset playerbuffs (thx to xytri)
            this.enemyMillhouse = 0;
            this.enemyLoathebs = 0;
            this.ownDragonConsort = 0;
            this.ownKirintor = 0;
            this.ownPrepa = 0;
            this.lockandload = 0;
            this.Stampede = 0;
            this.enemysabo = 0;
            this.ownFenciCoaches = 0;
            this.ownMillhouse = 0;
            this.ownLoathebs = 0;
            this.enemyDragonConsort = 0;
            this.ownsabo = 0;


            Dictionary<int, Entity> allEntitys = new Dictionary<int, Entity>();

            foreach (var item in rangerbot.gameState.GameEntityList)
            {
                allEntitys.Add(item.EntityId, item);
            }

            Entity ownhero = rangerbot.FriendHero;
            Entity enemyhero = rangerbot.EnemyHero;
            Entity ownHeroAbility = rangerbot.FriendHeroPower;

            //player stuff#########################
            //this.currentMana =ownPlayer.GetTag(HRGameTag.RESOURCES) - ownPlayer.GetTag(HRGameTag.RESOURCES_USED) + ownPlayer.GetTag(HRGameTag.TEMP_RESOURCES);
            this.currentMana = rangerbot.gameState.CurrentMana;
            this.ownMaxMana = rangerbot.gameState.LocalMaxMana;
            this.enemyMaxMana = rangerbot.gameState.RemoteMaxMana;
            //enemySecretCount = rangerbot.EnemySecrets.Count;
            //enemySecretCount = 0;
            //count enemy secrets
            enemySecretList.Clear();

            foreach (var item in rangerbot.EnemySecrets)
            {
                if (item.GetTagValue((int)GAME_TAG.QUEST) >= 1)
                {
                    Questmanager.Instance.updateQuestStuff(item.CardId, item.GetTagValue((int)GAME_TAG.QUEST_PROGRESS), item.GetTagValue((int)GAME_TAG.QUEST_PROGRESS_TOTAL), false);
                    continue;
                }
                enemySecretList.Add(item.EntityId);
            }
            enemySecretCount = enemySecretList.Count;

            this.ownSecretList.Clear();
            foreach (var item in rangerbot.FriendSecrets)
            {
                if (item.GetTagValue((int)GAME_TAG.QUEST) >= 1)
                {
                    Questmanager.Instance.updateQuestStuff(item.CardId, item.GetTagValue((int)GAME_TAG.QUEST_PROGRESS), item.GetTagValue((int)GAME_TAG.QUEST_PROGRESS_TOTAL), true);
                    continue;
                }
                this.ownSecretList.Add(item.CardId);
            }

            this.numMinionsPlayedThisTurn = rangerbot.gameState.NumMinionsPlayedThisTurn;
            this.cardsPlayedThisTurn = rangerbot.gameState.NumCardsPlayedThisTurn;


            //get weapon stuff
            this.ownHeroWeapon = "";
            this.heroWeaponAttack = 0;
            this.heroWeaponDurability = 0;

            this.ownHeroFatigue = ownhero.Fatigue;
            this.enemyHeroFatigue = enemyhero.Fatigue;

            this.ownDecksize = rangerbot.gameState.LocalDeckRemain;
            this.enemyDecksize = rangerbot.gameState.RemoteDeckRemain;


            //own hero stuff###########################
            int heroAtk = ownhero.ATK;
            int heroHp = ownhero.Health - ownhero.Damage;
            int heroDefence = ownhero.Armor;
            this.heroname = Hrtprozis.Instance.heroIDtoName(ownhero.CardId);

            bool heroImmuneToDamageWhileAttacking = false;
            bool herofrozen = ownhero.IsFrozen;
            int heroNumAttacksThisTurn = ownhero.NumAttacksThisTurn;
            bool heroHasWindfury = ownhero.HasWindfury;
            bool heroImmune = (ownhero.IsImmune);

            //Helpfunctions.Instance.ErrorLog(ownhero.GetName() + " ready params ex: " + exausted + " " + heroAtk + " " + numberofattacks + " " + herofrozen);


            if (rangerbot.FriendWeapon != null)
            {
                Entity weapon = rangerbot.FriendWeapon;
                this.ownHeroWeapon = CardDB.Instance.getCardDataFromID(CardDB.Instance.cardIdstringToEnum(rangerbot.FriendWeapon.CardId)).name.ToString();
                this.heroWeaponAttack = weapon.ATK;
                this.heroWeaponDurability = weapon.Durability - weapon.Damage;//weapon.GetDurability();
                heroImmuneToDamageWhileAttacking = false;
                if (this.ownHeroWeapon == "gladiatorslongbow")
                {
                    heroImmuneToDamageWhileAttacking = true;
                }
                if (this.ownHeroWeapon == "doomhammer")
                {
                    heroHasWindfury = true;
                }

                //Helpfunctions.Instance.ErrorLog("weapon: " + ownHeroWeapon + " " + heroWeaponAttack + " " + heroWeaponDurability);

            }



            //enemy hero stuff###############################################################
            this.enemyHeroname = Hrtprozis.Instance.heroIDtoName(enemyhero.CardId);

            int enemyAtk = enemyhero.ATK;
            int enemyHp = enemyhero.Health - enemyhero.Damage;
            int enemyDefence = enemyhero.Armor;
            bool enemyfrozen = enemyhero.IsFrozen;
            bool enemyHeroImmune = (enemyhero.IsImmune);

            this.enemyHeroWeapon = "";
            this.enemyWeaponAttack = 0;
            this.enemyWeaponDurability = 0;
            if (rangerbot.EnemyWeapon != null)
            {
                Entity weapon = rangerbot.EnemyWeapon;
                this.enemyHeroWeapon = CardDB.Instance.getCardDataFromID(CardDB.Instance.cardIdstringToEnum(weapon.CardId)).name.ToString();
                this.enemyWeaponAttack = weapon.ATK;
                this.enemyWeaponDurability = weapon.Durability - weapon.Damage;
            }


            //own hero power stuff###########################################################

            this.heroAbility = CardDB.Instance.getCardDataFromID(CardDB.Instance.cardIdstringToEnum(ownHeroAbility.CardId));
            this.ownAbilityisReady = (ownHeroAbility.IsExhausted) ? false : true; // if exhausted, ability is NOT ready

            //only because hearthranger desnt give me the data ;_; use the tag HEROPOWER_ACTIVATIONS_THIS_TURN instead! (of own player)
            //this.heroPowerUsesThisTurn = 10000;
            //if (this.ownAbilityisReady) this.heroPowerUsesThisTurn = 0;
            this.heroPowerUsesThisTurn = rangerbot.gameState.HeroPowerActivationsThisTurn;
            this.ownHeroPowerUsesThisGame = rangerbot.gameState.NumTimesHeroPowerUsedThisGame;

            this.enemyAbility = CardDB.Instance.getCardDataFromID(CardDB.Instance.cardIdstringToEnum(rangerbot.EnemyHeroPower.CardId));

            //generate Heros
            this.ownHero = new Minion();
            this.enemyHero = new Minion();
            this.ownHero.isHero = true;
            this.enemyHero.isHero = true;
            this.ownHero.own = true;
            this.enemyHero.own = false;
            this.ownHero.maxHp = ownhero.Health;
            this.enemyHero.maxHp = enemyhero.Health;
            this.ownHero.entityID = ownhero.EntityId;
            this.enemyHero.entityID = enemyhero.EntityId;

            this.ownHero.Angr = heroAtk;
            this.ownHero.Hp = heroHp;
            this.ownHero.armor = heroDefence;
            this.ownHero.frozen = herofrozen;
            this.ownHero.immuneWhileAttacking = heroImmuneToDamageWhileAttacking;
            this.ownHero.immune = heroImmune;
            this.ownHero.numAttacksThisTurn = heroNumAttacksThisTurn;
            this.ownHero.windfury = heroHasWindfury;

            this.enemyHero.Angr = enemyAtk;
            this.enemyHero.Hp = enemyHp;
            this.enemyHero.frozen = enemyfrozen;
            this.enemyHero.armor = enemyDefence;
            this.enemyHero.immune = enemyHeroImmune;
            this.enemyHero.Ready = false;

            this.ownHero.updateReadyness();


            //load enchantments of the heros
            List<miniEnch> miniEnchlist = new List<miniEnch>();
            foreach (Entity ent in allEntitys.Values)
            {
                if (ent.Attached == this.ownHero.entityID && ent.Zone == HSRangerLib.TAG_ZONE.PLAY)
                {
                    CardDB.cardIDEnum id = CardDB.Instance.cardIdstringToEnum(ent.CardId);
                    int controler = ent.ControllerId;
                    int creator = ent.CreatorId;
                    miniEnchlist.Add(new miniEnch(id, creator, controler));
                }

            }

            this.ownHero.loadEnchantments(miniEnchlist, ownhero.ControllerId);

            miniEnchlist.Clear();

            foreach (Entity ent in allEntitys.Values)
            {
                if (ent.Attached == this.enemyHero.entityID && ent.Zone == HSRangerLib.TAG_ZONE.PLAY)
                {
                    CardDB.cardIDEnum id = CardDB.Instance.cardIdstringToEnum(ent.CardId);
                    int controler = ent.ControllerId;
                    int creator = ent.CreatorId;
                    miniEnchlist.Add(new miniEnch(id, creator, controler));
                }

            }

            this.enemyHero.loadEnchantments(miniEnchlist, enemyhero.ControllerId);
            //fastmode weapon correction:
            if (ownHero.Angr < this.heroWeaponAttack) ownHero.Angr = this.heroWeaponAttack;
            if (enemyHero.Angr < this.enemyWeaponAttack) enemyHero.Angr = this.enemyWeaponAttack;

            this.ownOverload = rangerbot.gameState.RecallOwnedNum;//was at the start, but copied it over here :D , its german for overload :D
            //Reading new values:###################################################################################################
            //ToDo:

            this.numberMinionsDiedThisTurn = rangerbot.gameState.NumMinionsKilledThisTurn;

            //this should work (hope i didnt oversee a value :D)

            this.ownCurrentOverload = rangerbot.gameState.RecalledCrystalsOwedNextTurn;// ownhero.GetTag(HRGameTag.RECALL);
            this.enemyOverload = 0;// enemyhero.GetTag(HRGameTag.RECALL_OWED);

            //count buffs off !!players!! (players and not heros) (like preparation, kirintor-buff and stuff)
            // hope this works, dont own these cards to test where its attached

            int owncontrollerblubb = ownhero.ControllerId + 1; // controller = 1 or 2, but entity with 1 is the board -> +1
            int enemycontrollerblubb = enemyhero.ControllerId + 1;// controller = 1 or 2, but entity with 1 is the board -> +1

            //will not work in Hearthranger!


            foreach (Entity ent in allEntitys.Values)
            {
                if (ent.Attached == owncontrollerblubb && ent.Zone == HSRangerLib.TAG_ZONE.PLAY) //1==play
                {
                    CardDB.cardIDEnum id = CardDB.Instance.cardIdstringToEnum(ent.CardId);
                    if (id == CardDB.cardIDEnum.NEW1_029t) this.enemyMillhouse++;//CHANGED!!!!
                    if (id == CardDB.cardIDEnum.FP1_030e) this.enemyLoathebs++; //CHANGED!!!!
                    if (id == CardDB.cardIDEnum.BRM_018e) this.ownDragonConsort++;
                    if (id == CardDB.cardIDEnum.EX1_612o) this.ownKirintor++;
                    if (id == CardDB.cardIDEnum.EX1_145o) this.ownPrepa++;
                    if (id == CardDB.cardIDEnum.AT_061e) this.lockandload++;
                    if (id == CardDB.cardIDEnum.UNG_916e) this.Stampede++;
                    if (id == CardDB.cardIDEnum.AT_086e) this.enemysabo++;
                    if (id == CardDB.cardIDEnum.AT_115e) this.ownFenciCoaches++;

                }

                if (ent.Attached == enemycontrollerblubb && ent.Zone == HSRangerLib.TAG_ZONE.PLAY) //1==play
                {
                    CardDB.cardIDEnum id = CardDB.Instance.cardIdstringToEnum(ent.CardId);
                    if (id == CardDB.cardIDEnum.NEW1_029t) this.ownMillhouse++; //CHANGED!!!! (enemy has the buff-> we played millhouse)
                    if (id == CardDB.cardIDEnum.FP1_030e) this.ownLoathebs++; //CHANGED!!!!
                    if (id == CardDB.cardIDEnum.BRM_018e) this.enemyDragonConsort++;
                    // not needef for enemy, because its lasting only for his turn
                    //if (id == CardDB.cardIDEnum.EX1_612o) this.enemyKirintor++;
                    //if (id == CardDB.cardIDEnum.EX1_145o) this.enemyPrepa++;
                    if (id == CardDB.cardIDEnum.AT_086e) this.ownsabo++;
                }

            }
            this.lockandload = (rangerbot.gameState.LocalPlayerLockAndLoad) ? 1 : 0;
            //this.Stampede = (rangerbot.gameState.LocalPlayerLockAndLoad) ? 1 : 0;

            //saboteur test:
            if (ownHeroAbility.Cost >= 3) HelpFunctions.Instance.ErrorLog("heroabilitymana " + ownHeroAbility.Cost);
            if (this.enemysabo == 0 && ownHeroAbility.Cost >= 3) this.enemysabo++;
            if (this.enemysabo == 1 && ownHeroAbility.Cost >= 8) this.enemysabo++;

            //TODO test Bolvar Fordragon but it will be on his card :D
            //Reading new values end################################

        }

        private void GetMinions(HSRangerLib.BotBase rangerbot)
        {
            Dictionary<int, Entity> allEntitys = new Dictionary<int, Entity>();

            //TEST....................
            /*
            Helpfunctions.Instance.ErrorLog("# all");
            foreach (var item in rangerbot.gameState.GameEntityList)
            {
                allEntitys.Add(item.EntityId, item);
                Helpfunctions.Instance.ErrorLog(item.CardId + " e " + item.EntityId + " a " + item.Attached + " controler " + item.ControllerId + " creator " + item.CreatorId + " zone " + item.Zone + " zp " + item.ZonePosition);
                List<Entity> ents = item.Attachments;
                foreach (var item1 in ents)
                {
                    Helpfunctions.Instance.ErrorLog("#" + item1.CardId + " e " + item1.EntityId + " a " + item1.Attached + " controler " + item1.ControllerId + " creator " + item1.CreatorId + " zone " + item1.Zone);
                }
            }*/

            ownMinions.Clear();
            enemyMinions.Clear();
            Entity ownPlayer = rangerbot.FriendHero;
            Entity enemyPlayer = rangerbot.EnemyHero;

            // ALL minions on Playfield:
            List<Entity> list = new List<Entity>();

            foreach (var item in rangerbot.FriendMinion)
            {
                list.Add(item);
            }

            foreach (var item in rangerbot.EnemyMinion)
            {
                list.Add(item);
            }


            List<Entity> enchantments = new List<Entity>();


            foreach (Entity item in list)
            {
                Entity entity = item;
                int zp = entity.ZonePosition;

                if ((TAG_CARDTYPE)entity.CardType == TAG_CARDTYPE.MINION && zp >= 1)
                {
                    //Helpfunctions.Instance.ErrorLog("zonepos " + zp);
                    CardDB.Card c = CardDB.Instance.getCardDataFromID(CardDB.Instance.cardIdstringToEnum(entity.CardId));
                    Minion m = new Minion();
                    m.name = c.name;
                    m.handcard.card = c;
                    m.Angr = entity.ATK;
                    m.maxHp = entity.Health;
                    m.Hp = m.maxHp - entity.Damage;
                    if (m.Hp <= 0) continue;
                    m.wounded = false;
                    if (m.maxHp > m.Hp) m.wounded = true;


                    m.exhausted = entity.IsExhausted;

                    m.taunt = (entity.HasTaunt);

                    m.numAttacksThisTurn = entity.NumAttacksThisTurn;

                    int temp = entity.NumTurnsInPlay;
                    m.playedThisTurn = (temp == 0) ? true : false;

                    m.windfury = (entity.HasWindfury);

                    m.frozen = (entity.IsFrozen);

                    m.divineshild = (entity.HasDivineShield);

                    m.stealth = (entity.IsStealthed);

                    m.poisonous = (entity.IsPoisonous);

                    m.immune = (entity.IsImmune);

                    m.silenced = entity.IsSilenced;

                    m.spellpower = entity.SpellPower;

                    m.charge = 0;

                    if (!m.silenced && m.name == CardDB.cardName.southseadeckhand && entity.HasCharge) m.charge = 1;
                    if (!m.silenced && m.handcard.card.Charge) m.charge = 1;
                    if (m.charge == 0 && entity.HasCharge) m.charge = 1;
                    m.zonepos = zp;

                    m.entityID = entity.EntityId;

                    if (m.name == CardDB.cardName.unknown) HelpFunctions.Instance.ErrorLog("unknown card error");

                    HelpFunctions.Instance.ErrorLog(m.entityID + " ." + entity.CardId + ". " + m.name + " ready params ex: " + m.exhausted + " charge: " + m.charge + " attcksthisturn: " + m.numAttacksThisTurn + " playedthisturn " + m.playedThisTurn);
                    //Helpfunctions.Instance.ErrorLog("spellpower check " + entitiy.SpellPowerAttack + " " + entitiy.SpellPowerHealing + " " + entitiy.SpellPower);


                    List<miniEnch> enchs = new List<miniEnch>();
                    /*foreach (Entity ent in allEntitys.Values)
                    {
                        if (ent.Attached == m.entitiyID && ent.Zone == HSRangerLib.TAG_ZONE.PLAY)
                        {
                            CardDB.cardIDEnum id = CardDB.Instance.cardIdstringToEnum(ent.CardId);
                            int creator = ent.CreatorId;
                            int controler = ent.ControllerId;
                            enchs.Add(new miniEnch(id, creator, controler));
                        }

                    }*/

                    foreach (Entity ent in item.Attachments)
                    {
                        CardDB.cardIDEnum id = CardDB.Instance.cardIdstringToEnum(ent.CardId);
                        int creator = ent.CreatorId;
                        int controler = ent.ControllerId;
                        enchs.Add(new miniEnch(id, creator, controler));

                    }

                    m.loadEnchantments(enchs, entity.ControllerId);




                    m.Ready = false; // if exhausted, he is NOT ready

                    m.updateReadyness();


                    if (entity.ControllerId == this.ownPlayerController) // OWN minion
                    {
                        m.own = true;
                        this.ownMinions.Add(m);
                    }
                    else
                    {
                        m.own = false;
                        this.enemyMinions.Add(m);
                    }

                }
                // minions added

                /*
                if (entitiy.GetCardType() == HRCardType.WEAPON)
                {
                    //Helpfunctions.Instance.ErrorLog("found weapon!");
                    if (entitiy.GetControllerId() == this.ownPlayerController) // OWN weapon
                    {
                        this.ownHeroWeapon = CardDB.Instance.getCardDataFromID(CardDB.Instance.cardIdstringToEnum(entitiy.GetCardId())).name.ToString();
                        this.heroWeaponAttack = entitiy.GetATK();
                        this.heroWeaponDurability = entitiy.GetDurability();
                        //this.heroImmuneToDamageWhileAttacking = false;


                    }
                    else
                    {
                        this.enemyHeroWeapon = CardDB.Instance.getCardDataFromID(CardDB.Instance.cardIdstringToEnum(entitiy.GetCardId())).name.ToString();
                        this.enemyWeaponAttack = entitiy.GetATK();
                        this.enemyWeaponDurability = entitiy.GetDurability();
                    }
                }

                if (entitiy.GetCardType() == HRCardType.ENCHANTMENT)
                {

                    enchantments.Add(entitiy);
                }
                 */


            }

            /*foreach (HRCard item in list)
            {
                foreach (HREntity e in item.GetEntity().GetEnchantments())
                {
                    enchantments.Add(e);
                }
            }


            // add enchantments to minions
            setEnchantments(enchantments);*/
        }

        private void CorrectSpellpower(HSRangerLib.BotBase rangerbot)
        {
            int ownspellpower = rangerbot.gameState.LocalPlayerSpellPower;
            int spellpowerfield = 0;
            int numberDalaranAspirant = 0;
            foreach (Minion mnn in this.ownMinions)
            {
                if (mnn.name == CardDB.cardName.dalaranaspirant) numberDalaranAspirant++;
                spellpowerfield += mnn.spellpower;
            }
            int missingSpellpower = ownspellpower - spellpowerfield;
            if (missingSpellpower != 0)
            {
                HelpFunctions.Instance.ErrorLog("spellpower correction: " + ownspellpower + " " + spellpowerfield + " " + numberDalaranAspirant);
            }
            if (missingSpellpower >= 1 && numberDalaranAspirant >= 1)
            {
                //give all dalaran aspirants the "same amount" of spellpower
                for (int i = 0; i < missingSpellpower; i++)
                {
                    Minion dalaranAspriant = null;
                    int spellpower = ownspellpower;

                    foreach (Minion mnn in this.ownMinions)
                    {
                        if (mnn.name == CardDB.cardName.dalaranaspirant)
                        {
                            if (spellpower >= mnn.spellpower)
                            {
                                spellpower = mnn.spellpower;
                                dalaranAspriant = mnn;
                            }
                        }
                    }
                    dalaranAspriant.spellpower++;
                }

            }
        }

        private void SetEnchantments(List<Entity> enchantments)
        {
            /*
            foreach (HREntity bhu in enchantments)
            {
                //create enchantment
                Enchantment ench = CardDB.getEnchantmentFromCardID(CardDB.Instance.cardIdstringToEnum(bhu.GetCardId()));
                ench.creator = bhu.GetCreatorId();
                ench.controllerOfCreator = bhu.GetControllerId();
                ench.cantBeDispelled = false;
                //if (bhu.c) ench.cantBeDispelled = true;

                foreach (Minion m in this.ownMinions)
                {
                    if (m.entitiyID == bhu.GetAttached())
                    {
                        m.enchantments.Add(ench);
                        //Helpfunctions.Instance.ErrorLog("add enchantment " +bhu.GetCardId()+" to: " + m.entitiyID);
                    }

                }

                foreach (Minion m in this.enemyMinions)
                {
                    if (m.entitiyID == bhu.GetAttached())
                    {
                        m.enchantments.Add(ench);
                    }

                }

            }
            */
        }

        private void GetHandcards(HSRangerLib.BotBase rangerbot)
        {
            handCards.Clear();
            this.anzcards = 0;
            this.enemyAnzCards = 0;
            List<Entity> list = rangerbot.FriendHand;

            int elementalLastturn = 0;
            foreach (Entity item in list)
            {

                Entity entitiy = item;

                if (entitiy.ControllerId == this.ownPlayerController && entitiy.ZonePosition >= 1) // own handcard
                {
                    CardDB.Card c = CardDB.Instance.getCardDataFromID(CardDB.Instance.cardIdstringToEnum(entitiy.CardId));

                    //c.cost = entitiy.GetCost();
                    //c.entityID = entitiy.GetEntityId();

                    Handmanager.Handcard hc = new Handmanager.Handcard();
                    hc.card = c;
                    hc.position = entitiy.ZonePosition;
                    hc.entity = entitiy.EntityId;
                    hc.manacost = entitiy.Cost;
                    hc.addattack = 0;
                    //Helpfunctions.Instance.ErrorLog("hc "+ entitiy.ZonePosition + " ." + entitiy.CardId + ". " + entitiy.Cost + "  " + c.name);
                    int attackchange = entitiy.ATK - c.Attack;
                    int hpchange = entitiy.Health - c.Health;
                    hc.addattack = attackchange;
                    hc.addHp = hpchange;
                    hc.elemPoweredUp = entitiy.GetTagValue((int)GAME_TAG.ELEMENTAL_POWERED_UP);
                    if (hc.elemPoweredUp > 0) elementalLastturn = 1;
                    if (entitiy.HasTagValue((int)GAME_TAG.POWERED_UP)) hc.powerup = true;

                    handCards.Add(hc);
                    this.anzcards++;
                }


            }
            if (elementalLastturn > 0) Hrtprozis.Instance.updateElementals(elementalLastturn);

            Dictionary<int, Entity> allEntitys = new Dictionary<int, Entity>();

            foreach (var item in rangerbot.gameState.GameEntityList)
            {
                allEntitys.Add(item.EntityId, item);
            }


            foreach (Entity ent in allEntitys.Values)
            {
                if (ent.ControllerId != this.ownPlayerController && ent.ZonePosition >= 1 && ent.Zone == HSRangerLib.TAG_ZONE.HAND) // enemy handcard
                {
                    this.enemyAnzCards++;

                    //dont know if we can read this so ;D
                    if (CardDB.Instance.cardIdstringToEnum(ent.CardId) == CardDB.cardIDEnum.LOE_007t) this.enemyCursedCardsInHand++;
                }
            }

            //search for choice-cards in HR:
            this.choiceCards.Clear();
            this.choiceCardsEntitys.Clear();
            foreach (Entity ent in allEntitys.Values)
            {
                if (ent.ControllerId == this.ownPlayerController && ent.Zone == HSRangerLib.TAG_ZONE.SETASIDE) // choice cards are in zone setaside (but thats not all D:)
                {
                    if (ent.CardState == ActorStateType.CARD_SELECTABLE) //in HR these cards (setaside + card_selectable) are choice/tracking/discover-cards
                    {
                        this.choiceCards.Add(CardDB.Instance.cardIdstringToEnum(ent.CardId));
                        this.choiceCardsEntitys.Add(ent.EntityId);
                    }
                }
            }

        }





        private void GetDecks(HSRangerLib.BotBase rangerbot)
        {
            Dictionary<int, Entity> allEntitys = new Dictionary<int, Entity>();

            // add json.net nuget package to use this debug code
            //string path = SilverFishBotPath.AssemblyDirectory + System.IO.Path.DirectorySeparatorChar + "HRERRORLogs" + System.IO.Path.DirectorySeparatorChar;
            //System.IO.Directory.CreateDirectory(path);
            //string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(rangerbot.gameState.GameEntityList, Newtonsoft.Json.Formatting.Indented);
            //System.IO.File.WriteAllText(path + "HRErrorLog" + DateTime.Now.ToString("_yyyy-MM-dd_HH-mm-ss") + ".txt", jsonData);

            foreach (Entity item in rangerbot.gameState.GameEntityList)
            {
                allEntitys.Add(item.EntityId, item);
            }

            Dictionary<CardDB.cardIDEnum, int> graveyardtest = new Dictionary<CardDB.cardIDEnum, int>();

            foreach (var any in rangerbot.gameState.MyDeckCards)
            {
                HelpFunctions.Instance.logg("Value " + any.Value + "Key " + any.Key);
            }

            int owncontroler = rangerbot.gameState.LocalControllerId;
            int enemycontroler = rangerbot.gameState.RemoteControllerId;
            List<CardDB.cardIDEnum> ownCards = new List<CardDB.cardIDEnum>();
            List<CardDB.cardIDEnum> enemyCards = new List<CardDB.cardIDEnum>();
            List<GraveYardItem> graveYard = new List<GraveYardItem>();

            foreach (Entity ent in allEntitys.Values)
            {
                if ((TAG_ZONE)ent.Zone == TAG_ZONE.GRAVEYARD) HelpFunctions.Instance.ErrorLog("ent.Zone" + ent.Zone + "ent.id" + ent.EntityId);
                //Helpfunctions.Instance.logg("Zone=" + ent.Zone + " id=" + ent.EntityId + ent.CardState);
                //Helpfunctions.Instance.ErrorLog("Zone=" + ent.Zone + " id=" + ent.EntityId  + ent.CardState );
                if (ent.Zone == HSRangerLib.TAG_ZONE.SECRET && ent.ControllerId == enemycontroler) continue; // cant know enemy secrets :D
                if (ent.Zone == HSRangerLib.TAG_ZONE.DECK) continue;
                CardDB.cardIDEnum cardid = CardDB.Instance.cardIdstringToEnum(ent.CardId);

                //string owner = "own";
                //if (ent.GetControllerId() == enemycontroler) owner = "enemy";
                //if (ent.GetControllerId() == enemycontroler && ent.GetZone() == HRCardZone.HAND) Helpfunctions.Instance.logg("enemy card in hand: " + "cardindeck: " + cardid + " " + ent.GetName());
                //if (cardid != CardDB.cardIDEnum.None) Helpfunctions.Instance.logg("cardindeck: " + cardid + " " + ent.GetName() + " " + ent.GetZone() + " " + owner + " " + ent.GetCardType());
                if (cardid != CardDB.cardIDEnum.None)
                {
                    if (ent.Zone == HSRangerLib.TAG_ZONE.GRAVEYARD)
                    {
                        GraveYardItem gyi = new GraveYardItem(cardid, ent.EntityId, ent.ControllerId == owncontroler);
                        graveYard.Add(gyi);
                    }

                    int creator = ent.CreatorId;
                    if (creator != 0 && creator != owncontroler && creator != enemycontroler) continue; //if creator is someone else, it was not played

                    if (ent.ControllerId == owncontroler) //or controler?
                    {
                        if (ent.Zone == HSRangerLib.TAG_ZONE.GRAVEYARD)
                        {
                            ownCards.Add(cardid);
                            if (cardid == CardDB.cardIDEnum.UNG_067t1) OwnCrystalCore = 5;
                            if (cardid == CardDB.cardIDEnum.UNG_116t) ownMinionsCost0 = true;
                        }

                        if (ent.Zone == HSRangerLib.TAG_ZONE.PLAY)
                        {
                            if (cardid == CardDB.cardIDEnum.UNG_067t1) OwnCrystalCore = 5;
                            if (cardid == CardDB.cardIDEnum.UNG_116t) ownMinionsCost0 = true;
                        }

                    }
                    else
                    {
                        if (ent.Zone == HSRangerLib.TAG_ZONE.GRAVEYARD)
                        {
                            enemyCards.Add(cardid);
                            if (cardid == CardDB.cardIDEnum.UNG_067t1) EnemyCrystalCore = 5;
                        }

                        if (ent.Zone == HSRangerLib.TAG_ZONE.PLAY)
                        {
                            if (cardid == CardDB.cardIDEnum.UNG_067t1)
                            {
                                EnemyCrystalCore = 5;
                                HelpFunctions.Instance.logg("ENEMYCRYSTALCOREFOUND");
                                HelpFunctions.Instance.ErrorLog("ENEMYCRYSTALCOREFOUND");
                            }
                        }

                    }
                }
            }

            Probabilitymaker.Instance.setOwnCards(ownCards);
            Probabilitymaker.Instance.setEnemyCards(enemyCards);
            bool isTurnStart = false;
            if (Ai.Instance.nextMoveGuess.mana == -100)
            {
                isTurnStart = true;
                Ai.Instance.updateTwoTurnSim();
            }
            Probabilitymaker.Instance.setGraveYard(graveYard, isTurnStart);

        }

        private void UpdateBehaveString(Behavior botbase)
        {
            this.BotBehave = "rush";
            if (botbase is BehaviorFace) this.BotBehave = "face";
            if (botbase is BehaviorControl) this.BotBehave = "control";
            if (botbase is BehaviorMana) this.BotBehave = "mana";
            if (botbase is BehaviorAggroWarlock) this.BotBehave = "aggrowarlock";
            if (botbase is BehaviorAggroshaman) this.BotBehave = "aggroshaman";
            this.BotBehave += " " + Ai.Instance.maxwide;
            this.BotBehave += " face " + ComboBreaker.Instance.attackFaceHP;
            if (Settings.Instance.secondTurnAmount > 0)
            {
                if (Ai.Instance.nextMoveGuess.mana == -100)
                {
                    Ai.Instance.updateTwoTurnSim();
                }
                this.BotBehave += " twoturnsim " + Settings.Instance.secondTurnAmount + " ntss " + Settings.Instance.nextTurnDeep + " " + Settings.Instance.nextTurnMaxWide + " " + Settings.Instance.nextTurnTotalBoards;
            }

            if (Settings.Instance.playarround)
            {
                this.BotBehave += " playaround";
                this.BotBehave += " " + Settings.Instance.playaroundprob + " " + Settings.Instance.playaroundprob2;
            }

            this.BotBehave += " ets " + Settings.Instance.enemyTurnMaxWide;

            if (Settings.Instance.simEnemySecondTurn)
            {
                this.BotBehave += " ets2 " + Settings.Instance.enemyTurnMaxWideSecondTime;
                this.BotBehave += " ents " + Settings.Instance.enemySecondTurnMaxWide;
            }

            if (Settings.Instance.useSecretsPlayArround)
            {
                this.BotBehave += " secret";
            }

            if (Settings.Instance.secondweight != 0.5f)
            {
                this.BotBehave += " weight " + (int)(Settings.Instance.secondweight * 100f);
            }

            if (Settings.Instance.simulatePlacement)
            {
                this.BotBehave += " plcmnt";
            }


        }

        public static int GetLastAffected(int entityid)
        {

            if (latestGameState != null)
            {
                foreach (var item in latestGameState.GameEntityList)
                {
                    if (item.LastAffectedById == entityid)
                    {
                        return item.EntityId;
                    }
                }
            }

            return 0;
        }

        public static int GetCardTarget(int entityid)
        {

            if (latestGameState != null)
            {
                foreach (var item in latestGameState.GameEntityList)
                {
                    if (item.EntityId == entityid)
                    {
                        return item.CardTargetId;
                    }
                }
            }


            return 0;
        }

        //public void testExternal()
        //{
        //    BoardTester bt = new BoardTester("");
        //    this.currentMana = Hrtprozis.Instance.currentMana;
        //    this.ownMaxMana = Hrtprozis.Instance.ownMaxMana;
        //    this.enemyMaxMana = Hrtprozis.Instance.enemyMaxMana;
        //    printstuff(true);
        //    readActionFile();
        //}

        private void PrintStuff(Playfield p, bool runEx)
        {
            string dtimes = DateTime.Now.ToString("HH:mm:ss:ffff");
            String completeBoardString = p.getCompleteBoardForSimulating(this.BotBehave, this.VersionNumber, dtimes);

            HelpFunctions.Instance.logg(completeBoardString);

            if (runEx)
            {
                Ai.Instance.currentCalculatedBoard = dtimes;
                HelpFunctions.Instance.ResetBuffer();
                if (!Settings.Instance.useNetwork)
                {
                    HelpFunctions.Instance.WriteBufferToActionFile();
                    HelpFunctions.Instance.ResetBuffer();
                }

                HelpFunctions.Instance.WriteToBuffer(completeBoardString);
                HelpFunctions.Instance.writeBufferToFile();
            }

        }

        public bool ReadActionFile(bool passiveWaiting = false)
        {
            bool readed = true;
            List<string> alist = new List<string>();
            float value = 0f;
            string boardnumm = "-1";
            this.WaitingForOpenAI = true;
            int trackingchoice = 0;
            int trackingstate = 0;
            bool network = Settings.Instance.useNetwork;

            while (readed)
            {
                try
                {
                    string data = "";
                    System.Threading.Thread.Sleep(5);
                    if (network)
                    {
                        KeyValuePair<string, string> msg = FishNet.Instance.readMessage();
                        if (msg.Key != "actionstodo.txt")
                        {
                            HelpFunctions.Instance.ErrorLog("[Program] Ignoring Message: " + msg.Key);
                            continue;
                        }
                        HelpFunctions.Instance.ErrorLog("[Program] Message Type: " + msg.Key);
                        data = msg.Value;
                    }
                    else
                    {
                        data = System.IO.File.ReadAllText(Settings.Instance.path + "actionstodo.txt");
                    }
                    //if (data == "") Helpfunctions.Instance.ErrorLog($"[Program] Message Data: empty");
                    //if (data == "<EoF>" && data.EndsWith("<EoF>")) Helpfunctions.Instance.ErrorLog($"[Program] Message Data: <EoF>");
                    //if (!data.EndsWith("<EoF>")) Helpfunctions.Instance.ErrorLog($"[Program] Message Data: missing <EoF>");

                    if (data != "" && data != "<EoF>" && data.EndsWith("<EoF>"))
                    {
                        //Helpfunctions.Instance.ErrorLog($"[Program] Message Data:\r\n{data}");
                        data = data.Replace("<EoF>", "");
                        //Helpfunctions.Instance.ErrorLog(data);
                        if (!network)
                        {
                            HelpFunctions.Instance.ResetBuffer();
                            HelpFunctions.Instance.WriteBufferToActionFile();
                        }
                        alist.AddRange(data.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries));
                        string board = alist[0];
                        if (board.StartsWith("board "))
                        {
                            boardnumm = (board.Split(' ')[1].Split(' ')[0]);
                            alist.RemoveAt(0);
                            if (boardnumm != Ai.Instance.currentCalculatedBoard)
                            {
                                if (passiveWaiting)
                                {
                                    System.Threading.Thread.Sleep(10);
                                    return false;
                                }
                                continue;
                            }
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
                            trackingchoice = Convert.ToInt32(trackingstuff.Split(',')[0]);
                            trackingstate = Convert.ToInt32(trackingstuff.Split(',')[1]);
                            alist.RemoveAt(0);
                        }
                        readed = false;
                    }
                    else
                    {
                        if (passiveWaiting)
                        {
                            return false;
                        }
                    }

                }
                catch
                {
                    System.Threading.Thread.Sleep(10);
                }
            }
            this.WaitingForOpenAI = false;
            HelpFunctions.Instance.logg("received " + boardnumm + " actions to do: (currtime = " + DateTime.Now.ToString("HH:mm:ss.ffff") + ")");
            Ai.Instance.currentCalculatedBoard = "0";
            Playfield p = new Playfield();
            List<Action> aclist = new List<Action>();

            foreach (string a in alist)
            {
                aclist.Add(new Action(a, p));
                HelpFunctions.Instance.logg(a);
            }

            Ai.Instance.setBestMoves(aclist, value, trackingchoice, trackingstate);

            return true;
        }


    }
}
