using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using HSRangerLib;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI
{
    public static class OpenAIPath
    {
        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                string temp = System.IO.Path.GetDirectoryName(path) + System.IO.Path.DirectorySeparatorChar;

                return temp;
            }
        }

        public static  string SettingsPath
        {
            get{
                string temp = AssemblyDirectory + System.IO.Path.DirectorySeparatorChar + "Common" + System.IO.Path.DirectorySeparatorChar;
                if (System.IO.Directory.Exists(temp) == false)
                {
                    System.IO.Directory.CreateDirectory(temp);
                }

                return temp;
            }
        }

        public static string LogPath
        {
            get
            {
                string temp = AssemblyDirectory + System.IO.Path.DirectorySeparatorChar + "Logs" + System.IO.Path.DirectorySeparatorChar;
                if (System.IO.Directory.Exists(temp) == false)
                {
                    System.IO.Directory.CreateDirectory(temp);
                }

                return temp;
            }
        }
    }

    public class Bot : BotBase
    {
        private static Bot instance;

        public static Bot Instance
        {
            get
            {
                return instance ?? (instance = new Bot());
            }
        }
        public override string Description
        {
            get
            {
                return "OpenAI Version V" + OpenAI.Instance.VersionNumber + " )" + Environment.NewLine;
            }
        }

        public bool doMultipleThingsAtATime = true;
        public int dontmultiactioncount = 0;
        public int POWERFULSINGLEACTION = 0;

        //private int stopAfterWins = 30;
        private int concedeLvl = 5; // the rank, till you want to concede
        DateTime starttime = DateTime.Now;
        OpenAI sf;

        public Behavior behave = new BehaviorControl();

        //stuff for attack queueing :D
        public int numExecsReceived = 0;
        public int numActionsSent = 0;
        public bool shouldSendActions = true;
        public List<Playfield> queuedMoveGuesses = new List<Playfield>();
        
        private bool deckChanged = false;
        private bool shouldSendFakeAction = false;

        int discovercounter = 0;

        CardDB.cardIDEnum lastplayedcard = CardDB.cardIDEnum.None;
        int targetentity = 0;

        //
        bool isgoingtoconcede = false;
        int wins = 0;
        int loses = 0;

        public Bot()
        {
            base.HasBestMoveAI = true;

            starttime = DateTime.Now;

            Settings set = Settings.Instance;
            this.sf = OpenAI.Instance;
            behave = set.behave;
            sf.setnewLoggFile();
            CardDB cdb = CardDB.Instance;
            if (cdb.installedWrong)
            {
                HelpFunctions.Instance.ErrorLog("cant find CardDB");
                return;
            }

            bool teststuff = false; // set to true, to run a testfile (requires test.txt file in folder where _cardDB.txt file is located)
            bool printstuff = false; // if true, the best board of the tested file is printet stepp by stepp

            HelpFunctions.Instance.ErrorLog("----------------------------");
            HelpFunctions.Instance.ErrorLog("you are now running uai V" + sf.VersionNumber);
            HelpFunctions.Instance.ErrorLog("----------------------------");
            //Helpfunctions.Instance.ErrorLog("test... " + Settings.Instance.logpath + Settings.Instance.logfile);
            if (set.useExternalProcess) HelpFunctions.Instance.ErrorLog("YOU USE SILVER.EXE FOR CALCULATION, MAKE SURE YOU STARTED IT!");
            if (set.useExternalProcess) HelpFunctions.Instance.ErrorLog("SILVER.EXE IS LOCATED IN: " + Settings.Instance.path);
            
            if (!sf.startedexe && set.useExternalProcess && (!set.useNetwork || (set.useNetwork && set.netAddress == "127.0.0.1")))
            {
                sf.startedexe = true;
                Task.Run(() => startExeAsync());
            }


            if (teststuff)//run autotester for developpers
            {
                Ai.Instance.autoTester(printstuff);
            }

            this.doMultipleThingsAtATime = Settings.Instance.speedy;

            this.doMultipleThingsAtATime = true; // for easier debugging+bug fixing in the first weeks after update
            //will be false until xytrix fixes it (@xytrix end the action list, after playing a tracking/discover card)
        }

        private void startExeAsync()
        {
            System.Diagnostics.Process[] pname = System.Diagnostics.Process.GetProcessesByName("Redfish");
            string directory = Settings.Instance.path + "Redfish.exe";
            bool hasToOpen = true;

            if (pname.Length >= 1)
            {

                for (int i = 0; i < pname.Length; i++)
                {

                    string fullPath = pname[i].Modules[0].FileName;
                    if (fullPath == directory) hasToOpen = false;
                }
            }

            if (hasToOpen)
            {
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo(directory);
                startInfo.WorkingDirectory = Settings.Instance.path;
                System.Diagnostics.Process.Start(startInfo);
            }

            sf.startedexe = false; //reset it in case user closes exe
        }

        /// <summary>
        /// HRanger Code
        /// invoke when game enter mulligan
        /// </summary>
        /// <param name="e">
        ///     e.card_list -- mulligan card list
        ///     e.replace_list -- toggle card list (output)
        /// </param>
        public override void OnGameMulligan(GameMulliganEventArgs e)
        {
            if (e.handled || e.card_list.Count == 0) // if count==0 then HR is conceding
            {
                return;
            }

            //set e.handled to true, 
            //then bot will toggle cards by e.replace_list 
            //and will not use internal mulligan logic anymore.
            e.handled = true;

            if (Settings.Instance.learnmode)
            {
                e.handled = false;
                return;
            }

            var list = e.card_list;

            Entity enemyPlayer = base.EnemyHero;
            Entity ownPlayer = base.FriendHero;
            string enemName = Hrtprozis.Instance.heroIDtoName(enemyPlayer.CardId);
            string ownName = Hrtprozis.Instance.heroIDtoName(ownPlayer.CardId);

            // reload settings
            HeroEnum heroname = Hrtprozis.Instance.heroNametoEnum(ownName);
            HeroEnum enemyHeroname = Hrtprozis.Instance.heroNametoEnum(enemName);
            if (deckChanged || heroname != Hrtprozis.Instance.heroname)
            {
                if (heroname != Hrtprozis.Instance.heroname)
                {
                    HelpFunctions.Instance.ErrorLog("New Class: \"" + Hrtprozis.Instance.heroEnumtoCommonName(heroname) + "\", Old Class: \"" + Hrtprozis.Instance.heroEnumtoCommonName(Hrtprozis.Instance.heroname) + "\"");
                }
                Hrtprozis.Instance.setHeroName(ownName);
                ComboBreaker.Instance.updateInstance();
                Discovery.Instance.updateInstance();
                Mulligan.Instance.updateInstance();
                deckChanged = false;
            }
            if (deckChanged || heroname != Hrtprozis.Instance.heroname || enemyHeroname != Hrtprozis.Instance.enemyHeroname)
            {
                Hrtprozis.Instance.setEnemyHeroName(enemName);
                if (enemyHeroname != Hrtprozis.Instance.enemyHeroname)
                {
                    HelpFunctions.Instance.ErrorLog("New Enemy Class: \"" + Hrtprozis.Instance.heroEnumtoCommonName(enemyHeroname) + "\", Old Class: \"" + Hrtprozis.Instance.heroEnumtoCommonName(Hrtprozis.Instance.enemyHeroname) + "\"");
                }

                behave = Settings.Instance.updateInstance();
            }
            
            sf.setnewLoggFile();
            Settings.Instance.loggCleanPath();
            Mulligan.Instance.loggCleanPath();
            Discovery.Instance.loggCleanPath();
            ComboBreaker.Instance.loggCleanPath();



            if (Hrtprozis.Instance.startDeck.Count > 0)
            {
                string deckcards = "Deck: ";
                foreach (KeyValuePair<CardDB.cardIDEnum, int> card in Hrtprozis.Instance.startDeck)
                {
                    deckcards += card.Key;
                    if (card.Value > 1) deckcards += "," + card.Value;
                    deckcards += ";";
                }
                HelpFunctions.Instance.logg(deckcards);
            }

            //reload external process settings too
            HelpFunctions.Instance.resetBuffer();
            HelpFunctions.Instance.writeToBuffer(Hrtprozis.Instance.deckName + ";" + ownName + ";" + enemName + ";");
            HelpFunctions.Instance.writeBufferToDeckFile();

            if (Mulligan.Instance.hasmulliganrules(ownName, enemName))
            {
                bool hascoin = false;
                List<Mulligan.CardIDEntity> celist = new List<Mulligan.CardIDEntity>();

                foreach (var item in list)
                {
                    HelpFunctions.Instance.ErrorLog("cards on hand for mulligan: " + item.CardId);
                    if (item.CardId != "GAME_005")// dont mulligan coin
                    {
                        celist.Add(new Mulligan.CardIDEntity(item.CardId, item.EntityId));
                    }
                    else
                    {
                        hascoin = true;
                    }

                }
                if (celist.Count >= 4) hascoin = true;
                List<int> mullentities = Mulligan.Instance.whatShouldIMulligan(celist, ownName, enemName, hascoin);
                foreach (var item in list)
                {
                    if (mullentities.Contains(item.EntityId))
                    {
                        HelpFunctions.Instance.ErrorLog("Rejecting Mulligan Card " + HSRangerLib.CardDefDB.Instance.GetCardEnglishName(item.CardId) + " because of your rules");
                        //toggle this card
                        e.replace_list.Add(item);
                    }
                }

            }
            else
            {
                foreach (var item in list)
                {
                    if (item.Cost >= 4)
                    {
                        HelpFunctions.Instance.ErrorLog("Rejecting Mulligan Card " + HSRangerLib.CardDefDB.Instance.GetCardEnglishName(item.CardId) + " because it cost is >= 4.");

                        e.replace_list.Add(item);

                    }
                    if (item.CardId == "EX1_308" || item.CardId == "EX1_622" || item.CardId == "EX1_005")
                    {
                        HelpFunctions.Instance.ErrorLog("Rejecting Mulligan Card " + HSRangerLib.CardDefDB.Instance.GetCardEnglishName(item.CardId) + " because it is soulfire or shadow word: death");
                        e.replace_list.Add(item);
                    }
                }
            }

            Ai.Instance.bestmoveValue = 0; // not concede
            //Helpfunctions.Instance.logg("Ai.Instance.bestmoveValue " + Ai.Instance.bestmoveValue);

            if (Mulligan.Instance.loserLoserLoser)
            {
                if (!autoconcede())
                {
                    concedeVSenemy(ownName, enemName);
                }

                //set concede flag
                e.concede = this.isgoingtoconcede;
            }
        }

        /// <summary>
        /// invoke when drafting arena cards (including hero draft)
        /// </summary>
        /// <param name="e"></param>
        public override void OnGameArenaDraft(GameArenaDraftEventArgs e)
        {
            //must set e.handled to true if you handle draft in this function.
            e.handled = false;


            //if (e.is_hero_choices)
            //{
            //    //choose hero here
            //    e.draft_pick_id = GetBestHeroCardId(e);

            //    return;
            //}

        }

        //private int CountDeckCardNum(int cost,bool is_minion, bool is_spell,List<HSRangerLib.GameArenaDraftEventArgs.DeckCard> deck)
        //{
        //    int num = 0;

        //    foreach (var item in deck)
        //    {
        //        CardDef def = CardDefDB.Instance.GetCardDef(item.card_id);

        //        if (def.Cost == cost)
        //        {
        //            if (is_minion)
        //            {
        //                if (def.CardType == TAG_CARDTYPE.MINION)
        //                {
        //                    num += item.num;
        //                }
        //            }

        //            if (is_spell)
        //            {
        //                if (def.CardType == TAG_CARDTYPE.ABILITY ||
        //                    def.CardType == TAG_CARDTYPE.ENCHANTMENT)
        //                {
        //                    num += item.num;
        //                }
        //            }
        //        }
        //    }

        //    return num;
        //}

        //private int f(string hero_id)
        //{
        //    CardDef def = CardDefDB.Instance.GetCardDef(hero_id);

        //    //No.1 choice (Your best choice)
        //    if (def.Class == TAG_CLASS.DRUID)
        //    {
        //        return 1;
        //    }

        //    //No.2 choice
        //    if (def.Class == TAG_CLASS.HUNTER)
        //    {
        //        return 2;
        //    }

        //    //No.3 choice
        //    if (def.Class == TAG_CLASS.MAGE)
        //    {
        //        return 3;
        //    }

        //    //No.4 choice
        //    if (def.Class == TAG_CLASS.PALADIN)
        //    {
        //        return 4; 
        //    }

        //    //No.5 choice
        //    if (def.Class == TAG_CLASS.PRIEST)
        //    {
        //        return 5;
        //    }

        //    //No.6 choice
        //    if (def.Class == TAG_CLASS.ROGUE)
        //    {
        //        return 6;
        //    }
        //    //No.7 choice
        //    if (def.Class == TAG_CLASS.SHAMAN)
        //    {
        //        return 7;
        //    }
        //    //No.8 choice
        //    if (def.Class == TAG_CLASS.WARLOCK)
        //    {
        //        return 8;
        //    }
        //    //No.9 choice
        //    if (def.Class == TAG_CLASS.WARRIOR)
        //    {
        //        return 9;
        //    }

        //    return 100;
        //}

        //private string GetBestHeroCardId(GameArenaDraftEventArgs e)
        //{
        //    string best_hero_id = "";
        //    foreach (var card_id in e.draft_choices.OrderBy( hero => GetHeroPriority(hero)))
        //    {
        //        best_hero_id = card_id;
        //        break;
        //    }

        //    return best_hero_id;
        //}
    






        /// <summary>
        /// invoke when game starts.
        /// </summary>
        /// <param name="e">e.deck_list -- all cards id in the deck.</param>
        public override void OnGameStart(GameStartEventArgs e)
        {
            // reset instance vars
            numExecsReceived = 0;
            numActionsSent = 0;

            if (Hrtprozis.Instance.deckName != e.deck_name)
            {
                HelpFunctions.Instance.ErrorLog("New Deck: \"" + e.deck_name + "\", Old Deck: \"" + Hrtprozis.Instance.deckName + "\"");
                deckChanged = true;
                Hrtprozis.Instance.setDeckName(e.deck_name);
            }
            else
            {
                deckChanged = false;
            }

            Hrtprozis.Instance.clearDecks();
            foreach (var card in e.deck_list)
            {
                Hrtprozis.Instance.addCardToDecks(CardDB.Instance.cardIdstringToEnum(card.card_id), card.num);
            }

        }

        /// <summary>
        /// invoke when game ends.
        /// </summary>
        /// <param name="e"></param>
        public override void OnGameOver(GameOverEventArgs e)
        {
            if (e.win)
            {
                HandleWining();
            }else if (e.loss || e.concede)
            {
                HandleLosing(e.concede);
            }
        }

        private HSRangerLib.BotAction CreateRangerConcedeAction()
        {
            HSRangerLib.BotAction ranger_action = new HSRangerLib.BotAction();
            ranger_action.Actor = base.FriendHero;
            ranger_action.Type = BotActionType.CONCEDE;

            return ranger_action;
        }

        private HSRangerLib.BotActionType GetRangerActionType(Entity actor, Entity target, actionEnum sf_action_type)
        {
            
            if (sf_action_type == actionEnum.endturn)
            {
                if (POWERFULSINGLEACTION >= 1) POWERFULSINGLEACTION = 0;
                return BotActionType.END_TURN;
            }

            if (sf_action_type == actionEnum.useHeroPower)
            {
                return BotActionType.CAST_ABILITY;
            }

            if (sf_action_type == actionEnum.attackWithHero)
            {
                return BotActionType.HERO_ATTACK;
            }

            if (sf_action_type == actionEnum.attackWithMinion)
            {
                if (actor.Zone == HSRangerLib.TAG_ZONE.HAND && actor.IsMinion)
                {
                    return BotActionType.CAST_MINION;// that should not occour >_>
                }else if (actor.Zone == HSRangerLib.TAG_ZONE.PLAY && actor.IsMinion)
                {
                    return BotActionType.MINION_ATTACK;
                }
            }

            if (sf_action_type == actionEnum.playcard)
            {
                if (actor.Zone == HSRangerLib.TAG_ZONE.HAND)
                {
                    if (actor.IsMinion)
                    {
                        return BotActionType.CAST_MINION;
                    }else if (actor.IsWeapon)
                    {
                        return BotActionType.CAST_WEAPON;
                    }else
                    {
                        return BotActionType.CAST_SPELL;
                    }                    
                }else if (actor.Zone == HSRangerLib.TAG_ZONE.PLAY)
                {
                    if (actor.IsMinion)
                    {
                        return BotActionType.MINION_ATTACK;
                    }else if (actor.IsWeapon)
                    {
                        return BotActionType.HERO_ATTACK;
                    }
                }
            }

            if (target != null)
            {
                HelpFunctions.Instance.ErrorLog("GetActionType: wrong action type! " +
                                            sf_action_type.ToString() + ": " + HSRangerLib.CardDefDB.Instance.GetCardEnglishName(actor.CardId)
                                                         + " target: " + HSRangerLib.CardDefDB.Instance.GetCardEnglishName(target.CardId));
            }else
            {
                HelpFunctions.Instance.ErrorLog("GetActionType: wrong action type! " +
                                            sf_action_type.ToString() + ": " + HSRangerLib.CardDefDB.Instance.GetCardEnglishName(actor.CardId)
                                                         + " target none.");
            }

            if (POWERFULSINGLEACTION >= 1) POWERFULSINGLEACTION = 0;
            return BotActionType.END_TURN;
        }

        private HSRangerLib.BotAction ConvertToRangerAction(Action moveTodo)
        {
            HSRangerLib.BotAction ranger_action = new HSRangerLib.BotAction();
            Ai daum = Ai.Instance;
            
            switch (moveTodo.actionType)
            {
                case actionEnum.endturn:
                    break;
                case actionEnum.playcard:
                    ranger_action.Actor = getCardWithNumber(moveTodo.card.entity);

                    lastplayedcard = CardDB.Instance.cardIdstringToEnum(ranger_action.Actor.CardId);
                    if (daum.bestmove.target != null) targetentity = daum.bestmove.target.entityID;
                    HelpFunctions.Instance.ErrorLog("lastplayedcard " + lastplayedcard.ToString());
                    if (targetentity >= 1) HelpFunctions.Instance.ErrorLog("lastplayedcardtarget " + targetentity);
                    Hrtprozis.Instance.updateLastPlayedCard(lastplayedcard, targetentity);
                    Ai.Instance.playedlastcard = lastplayedcard;

                    if (daum.bestmove.actionType == actionEnum.playcard && daum.bestmove != null)
                    {
                        if (daum.IsPlayRandomEffect(daum.bestmove.card.card, daum.oldMoveGuess, daum.nextMoveGuess))
                        {
                            this.doMultipleThingsAtATime = false;
                            this.dontmultiactioncount++;
                            //Helpfunctions.Instance.ErrorLog("doMultipleThingsAtATime " + doMultipleThingsAtATime + " because IsPlayRandomEffect 찾는거");

                        }
                        else this.doMultipleThingsAtATime = true;


                        //if (daum.bestmove.card.card.name == CardDB.cardName.barnes) POWERFULSINGLEACTION++;

                        switch (daum.bestmove.card.card.name)
                        {
                            case CardDB.cardName.defenderofargus:
                            case CardDB.cardName.direwolfalpha:
                            case CardDB.cardName.darkpeddler:
                            case CardDB.cardName.quickshot:
                            case CardDB.cardName.kingselekk:
                            case CardDB.cardName.barnes:
                            case CardDB.cardName.tuskarrtotemic:
                            case CardDB.cardName.flametonguetotem:
                            case CardDB.cardName.leeroyjenkins:
                                //Helpfunctions.Instance.logg("찾는거 " + daum.bestmove.card.card.name + " 드로우카드 찾는거");
                                //Helpfunctions.Instance.ErrorLog("찾는거 " + daum.bestmove.card.card.name + " 드로우카드 찾는거");
                                //case CardDB.cardName.defenderofargus:
                                this.doMultipleThingsAtATime = false;
                                this.dontmultiactioncount++; break;
                            default: break;
                        }

                        if (PenalityManager.Instance.cardDrawBattleCryDatabase.ContainsKey(daum.bestmove.card.card.name)
                            || PenalityManager.Instance.AdaptDatabase.ContainsKey(daum.bestmove.card.card.name)
                            || PenalityManager.Instance.discoverCards.ContainsKey(daum.bestmove.card.card.name))
                        {
                            this.doMultipleThingsAtATime = false;
                            this.dontmultiactioncount++; break;                            
                        }
                        if (PenalityManager.Instance.AdaptDatabase.ContainsKey(daum.bestmove.card.card.name)
                            || PenalityManager.Instance.discoverCards.ContainsKey(daum.bestmove.card.card.name)
                            || PenalityManager.Instance.randomEffects.ContainsKey(daum.bestmove.card.card.name)) this.POWERFULSINGLEACTION++;


                        //charge
                        if (daum.bestmove.card.card.Charge)
                        {
                            this.doMultipleThingsAtATime = false;
                            this.dontmultiactioncount++; break;

                        }
                        else
                        {
                            //special charge
                            switch (daum.bestmove.card.card.name)
                            {
                                case CardDB.cardName.leathercladhogleader:
                                    if (Playfield.Instance.EnemyCards.Count >= 6)
                                    {
                                        this.doMultipleThingsAtATime = false;
                                        this.dontmultiactioncount++; break;
                                    }
                                    else break;
                                case CardDB.cardName.southseadeckhand:
                                    break;
                                case CardDB.cardName.spikedhogrider:
                                    if (Playfield.Instance.enemyMinions.Find(a => a.taunt) != null)
                                    {
                                        this.doMultipleThingsAtATime = false;
                                        this.dontmultiactioncount++; break;
                                    }
                                    else break;
                                case CardDB.cardName.alexstraszaschampion:
                                    if (Playfield.Instance.owncards.Find(a => a.card.race == TAG_RACE.DRAGON) != null)
                                    {
                                        this.doMultipleThingsAtATime = false;
                                        this.dontmultiactioncount++; break;
                                    }
                                    else break;
                                case CardDB.cardName.tanarishogchopper:
                                    if (Playfield.Instance.EnemyCards.Count == 6)
                                    {
                                        this.doMultipleThingsAtATime = false;
                                        this.dontmultiactioncount++; break;
                                    }
                                    else break;
                                case CardDB.cardName.armoredwarhorse:
                                    this.doMultipleThingsAtATime = false;
                                    this.dontmultiactioncount++; break;
                                default: break;
                            }
                        }


                        bool hasjuggler = false;
                        foreach (Minion m in Playfield.Instance.ownMinions)
                        {
                            if (m.name == CardDB.cardName.knifejuggler && !m.silenced) hasjuggler = true;
                        }
                        if (hasjuggler && (daum.bestmove.card.card.type == CardDB.cardtype.MOB || PenalityManager.Instance.summonMinionSpellsDatabase.ContainsKey(daum.bestmove.card.card.name)))
                        {
                            bool hasdamageeffectminion = false;
                            foreach (Minion m in Playfield.Instance.enemyMinions)
                            {
                                if (m.name == CardDB.cardName.impgangboss ||
                                    m.name == CardDB.cardName.dragonegg ||
                                    m.name == CardDB.cardName.hoggerdoomofelwynn ||
                                    m.name == CardDB.cardName.grimpatron) hasdamageeffectminion = true;
                                if (!m.silenced && (m.handcard.card.deathrattle || m.hasDeathrattle())) hasdamageeffectminion = true;
                            }
                            if (hasdamageeffectminion) this.POWERFULSINGLEACTION++;
                            HelpFunctions.Instance.logg("찾는거 저글러 몹" + daum.bestmove.card.card.name);
                            HelpFunctions.Instance.logg("찾는거 저글러 몹" + daum.bestmove.card.card.name);
                            HelpFunctions.Instance.ErrorLog("찾는거 저글러 몹" + daum.bestmove.card.card.name);
                            HelpFunctions.Instance.ErrorLog("찾는거 저글러 몹" + daum.bestmove.card.card.name);
                        }

                        
                        


                        if (daum.bestmove.card.card.type == CardDB.cardtype.SPELL)
                        {

                            if (daum.bestmove.card.card.name == CardDB.cardName.jadeidol &&
                                (daum.bestmove.druidchoice == 2 || Playfield.Instance.ownMinions.Find(a => a.name == CardDB.cardName.fandralstaghelm && !a.silenced) != null))
                            {
                                Hrtprozis.Instance.AddTurnDeck(CardDB.cardIDEnum.CFM_602, 3);
                            }
                            bool hasenemydeathrattle = false;
                            foreach (Entity mnn in EnemyMinion)
                            {
                                if (mnn.HasDeathrattle) hasenemydeathrattle = true;
                            }

                            bool Random_Spell_But_Can_Kill_Deathrattle_Card = false;
                            switch (daum.bestmove.card.card.name)
                            {
                                case CardDB.cardName.arcanemissiles:
                                case CardDB.cardName.forkedlightning:
                                case CardDB.cardName.brawl:
                                case CardDB.cardName.bouncingblade:      
                                case CardDB.cardName.darkbargain:
                                case CardDB.cardName.avengingwrath:
                                case CardDB.cardName.multishot:
                                case CardDB.cardName.fistofjaraxxus:
                                case CardDB.cardName.deadlyshot: 
                                case CardDB.cardName.sabotage:
                                case CardDB.cardName.spreadingmadness:
                                case CardDB.cardName.flamecannon: 
                                case CardDB.cardName.cleave: Random_Spell_But_Can_Kill_Deathrattle_Card = true;
                                    this.dontmultiactioncount++;
                                    break;
                                default: break;
                            }

                            bool hastargetdeathrattle = false;
                            if (daum.bestmove.target != null) hastargetdeathrattle = (daum.bestmove.target.hasDeathrattle() || daum.bestmove.target.deathrattles.Count >= 1 || (daum.bestmove.target.handcard.card.deathrattle && !daum.bestmove.target.silenced)) && !daum.bestmove.target.isHero;

                            bool targethasdamageeffect = false;
                            if (daum.bestmove.target != null)
                            {
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
                            }
                            bool hasdamageeffectminion = false;
                            foreach (Minion m in Playfield.Instance.enemyMinions)
                            {
                                if (m.name == CardDB.cardName.impgangboss ||
                                    m.name == CardDB.cardName.dragonegg ||
                                    m.name == CardDB.cardName.hoggerdoomofelwynn ||
                                    m.name == CardDB.cardName.grimpatron) hasdamageeffectminion = true;
                            }                            
                            if ((daum.bestmove.target != null && (hastargetdeathrattle || targethasdamageeffect) || 
                                ((hasenemydeathrattle || hasdamageeffectminion) && (Random_Spell_But_Can_Kill_Deathrattle_Card || PenalityManager.Instance.DamageAllDatabase.ContainsKey(daum.bestmove.card.card.name) || PenalityManager.Instance.DamageAllEnemysDatabase.ContainsKey(daum.bestmove.card.card.name)))))
                            {
                                this.dontmultiactioncount++;
                            }

                            //switch (daum.bestmove.card.card.name)
                            //{
                            //    case CardDB.cardName.hex:
                            //    case CardDB.cardName.jadeidol:
                            //        this.POWERFULSINGLEACTION++; break;
                            //    default: break;
                            //}                                           
                        }

                        foreach (Minion m in Playfield.Instance.ownMinions)
                        {
                            if (m.name == CardDB.cardName.fandralstaghelm && !m.silenced)
                            {
                                this.doMultipleThingsAtATime = false;
                                this.dontmultiactioncount++;
                            }
                        }

                    }

                    //if (moveTodo.card.card.type == CardDB.cardtype.MOB || moveTodo.card.card.name == CardDB.cardName.forbiddenritual)
                    //{
                    //    foreach (Minion mnn in Playfield.Instance.ownMinions)
                    //    {
                    //        if (!mnn.silenced && (mnn.name == CardDB.cardName.darkshirecouncilman || mnn.name == CardDB.cardName.knifejuggler))
                    //        {
                    //            System.Threading.Thread.Sleep(500);
                    //            Helpfunctions.Instance.logg("darkshirecouncilman or knifejuggler 미리 effect detected sleep 500ms");
                    //            Helpfunctions.Instance.logg("darkshirecouncilman or knifejuggler 미리 effect detected sleep 500ms");
                    //            Helpfunctions.Instance.ErrorLog("darkshirecouncilman or knifejuggler 미리 effect detected sleep 500ms");
                    //            Helpfunctions.Instance.ErrorLog("darkshirecouncilman or knifejuggler 미리 effect detected sleep 500ms");
                    //        }
                    //    }
                    //}

                    //if (moveTodo.card.card.type == CardDB.cardtype.MOB)
                    //{
                    //    System.Threading.Thread.Sleep(200);
                    //}

                    //switch (moveTodo.card.card.name)
                    //{
                    //    case CardDB.cardName.defenderofargus:
                    //        if (this.EnemySecrets.Count >= 1) System.Threading.Thread.Sleep(1800);
                    //        System.Threading.Thread.Sleep(4500); 
                    //        break;
                    //    case CardDB.cardName.abusivesergeant:
                    //        System.Threading.Thread.Sleep(900); break;
                    //    case CardDB.cardName.darkirondwarf:
                    //        System.Threading.Thread.Sleep(900); break;
                    //    case CardDB.cardName.direwolfalpha:
                    //        if (this.EnemySecrets.Count >= 1) System.Threading.Thread.Sleep(1200);
                    //        System.Threading.Thread.Sleep(2500);
                    //        break;
                    //    case CardDB.cardName.flametonguetotem:
                    //        if (this.EnemySecrets.Count >= 1) System.Threading.Thread.Sleep(1200);
                    //        System.Threading.Thread.Sleep(2500);
                    //        break;
                    //    default:
                    //        break;
                    //}

                    if (ranger_action.Actor == null) return null;  // missing entity likely because new spawned minion
                    break;
                case actionEnum.attackWithHero:
                    ranger_action.Actor = base.FriendHero;
                    //System.Threading.Thread.Sleep(1100);

                    //foreach (Minion m in Playfield.Instance.ownMinions)
                    //{
                    //    if (m.name == CardDB.cardName.southseadeckhand && (m.playedThisTurn || m.Ready ))
                    //    {
                    //        this.doMultipleThingsAtATime = false;
                    //        this.dontmultiactioncount++;
                    //    }
                    //}

                    break;
                case actionEnum.useHeroPower:
                    ranger_action.Actor = base.FriendHeroPower;
                    break;
                case actionEnum.attackWithMinion:
                    ranger_action.Actor = getEntityWithNumber(moveTodo.own.entityID);

                    if (daum.bestmove.own.name == CardDB.cardName.viciousfledgling && daum.bestmove.target.isHero && !daum.bestmove.target.own)
                    {
                        lastplayedcard = CardDB.Instance.cardIdstringToEnum(ranger_action.Actor.CardId);
                        if (daum.bestmove.target != null) targetentity = daum.bestmove.own.entityID;
                        HelpFunctions.Instance.ErrorLog("lastplayedcard " + lastplayedcard.ToString());
                        if (targetentity >= 1) HelpFunctions.Instance.ErrorLog("lastplayedcardtarget " + targetentity);
                        Hrtprozis.Instance.updateLastPlayedCard(lastplayedcard, targetentity);
                        Ai.Instance.playedlastcard = lastplayedcard;
                    }

                    //foreach (Minion m in Playfield.Instance.ownMinions)
                    //{
                    //    if (m.name == CardDB.cardName.flametonguetotem || m.name == CardDB.cardName.direwolfalpha || (m.name == CardDB.cardName.frothingberserker && m.Ready && !m.frozen)
                    //        || m.name == CardDB.cardName.southseadeckhand && m.Ready)
                    //    {
                    //        this.doMultipleThingsAtATime = false;
                    //        this.dontmultiactioncount++;
                    //        this.POWERFULSINGLEACTION++;
                    //    }
                    //}

                    if (ranger_action.Actor == null) return null;  // missing entity likely because new spawned minion
                    break;
                default:
                    break;
            }

            if (moveTodo.target != null)
            {
                ranger_action.Target = getEntityWithNumber(moveTodo.target.entityID);
                if (ranger_action.Target == null) return null;  // missing entity likely because new spawned minion
            }


             ranger_action.Type = GetRangerActionType(ranger_action.Actor, ranger_action.Target, moveTodo.actionType);

             if (moveTodo.druidchoice >= 1)
             {
                 ranger_action.Choice = moveTodo.druidchoice;//1=leftcard, 2= rightcard
             }

             ranger_action.Index = moveTodo.place;
             if (moveTodo.place >= 1) ranger_action.Index = moveTodo.place - 1;

             if (moveTodo.target != null)
             {
                 //ranger stuff :D
                 ranger_action.ID = moveTodo.actionType.ToString() + ": " + HSRangerLib.CardDefDB.Instance.GetCardEnglishName(ranger_action.Actor.CardId);

                 HelpFunctions.Instance.ErrorLog(moveTodo.actionType.ToString() + ": " + HSRangerLib.CardDefDB.Instance.GetCardEnglishName(ranger_action.Actor.CardId)
                                                  + " target: " + HSRangerLib.CardDefDB.Instance.GetCardEnglishName(ranger_action.Target.CardId));
                 HelpFunctions.Instance.logg(moveTodo.actionType.ToString() + ": " + HSRangerLib.CardDefDB.Instance.GetCardEnglishName(ranger_action.Actor.CardId)
                                                  + " target: " + HSRangerLib.CardDefDB.Instance.GetCardEnglishName(ranger_action.Target.CardId)
                                                  + " choice: " + moveTodo.druidchoice + " place" + moveTodo.place);


             }
             else
             {
                 //ranger stuff :D
                 ranger_action.ID = moveTodo.actionType.ToString() + ": " + HSRangerLib.CardDefDB.Instance.GetCardEnglishName(ranger_action.Actor.CardId);

                 HelpFunctions.Instance.ErrorLog(moveTodo.actionType.ToString() + ": " + HSRangerLib.CardDefDB.Instance.GetCardEnglishName(ranger_action.Actor.CardId)
                                                  + " target nothing");
                 HelpFunctions.Instance.logg(moveTodo.actionType.ToString() + ": " + HSRangerLib.CardDefDB.Instance.GetCardEnglishName(ranger_action.Actor.CardId)
                                                  + " choice: " + moveTodo.druidchoice + " place" + moveTodo.place);
             }





            if (moveTodo.actionType == actionEnum.attackWithMinion)
            {
                bool hashyena = false;
                bool cultmaster = false;
                foreach (Minion mnn in Playfield.Instance.ownMinions)
                {
                    if (mnn.name == CardDB.cardName.scavenginghyena && !mnn.silenced && mnn.Ready)
                    {
                        hashyena = true;
                    }
                    if (mnn.name == CardDB.cardName.cultmaster && !mnn.silenced) cultmaster = true;
                }

                if (hashyena && daum.bestmove.own.Hp <= daum.bestmove.target.Angr && !daum.bestmove.target.isHero && (TAG_RACE)daum.bestmove.own.handcard.card.race == TAG_RACE.BEAST)
                {
                    this.doMultipleThingsAtATime = false;
                    this.dontmultiactioncount++;
                    POWERFULSINGLEACTION++;
                }

                else if (cultmaster && daum.bestmove.own.Hp <= daum.bestmove.target.Angr && !daum.bestmove.target.isHero)
                {
                    this.doMultipleThingsAtATime = false;
                    this.dontmultiactioncount++;
                    POWERFULSINGLEACTION++;
                }

                else if (daum.bestmove.own.Angr >= daum.bestmove.target.Hp && !daum.bestmove.target.divineshild && daum.bestmove.own.name == CardDB.cardName.finjatheflyingstar)
                {
                    this.doMultipleThingsAtATime = false;
                    this.dontmultiactioncount++;
                    POWERFULSINGLEACTION++;
                }

                else if ((daum.bestmove.own.Angr >= daum.bestmove.target.Hp || daum.bestmove.own.poisonous ) && !daum.bestmove.target.divineshild && (daum.bestmove.target.name == CardDB.cardName.murlocwarleader || daum.bestmove.target.name == CardDB.cardName.southseacaptain))
                {
                    this.doMultipleThingsAtATime = false;
                    this.dontmultiactioncount++;
                    POWERFULSINGLEACTION++;
                }


                else if (moveTodo.target != null && !ranger_action.Target.IsHero)
                {
                    if (ranger_action.Target.HasDeathrattle && !ranger_action.Target.HasDivineShield) // target deathrattle
                    {
                        if ((ranger_action.Target.Health - ranger_action.Target.Damage) <= ranger_action.Actor.ATK || 
                            ranger_action.Target.IsPoisonous || ranger_action.Actor.IsPoisonous)
                        {
                            this.doMultipleThingsAtATime = false;
                            this.dontmultiactioncount++;
                        }
                    }
                    if (ranger_action.Actor.HasDeathrattle && !ranger_action.Actor.HasDivineShield) // actor deathrattle
                    {
                        if ((ranger_action.Actor.Health - ranger_action.Actor.Damage) <= ranger_action.Target.ATK ||
                            ranger_action.Target.IsPoisonous || ranger_action.Actor.IsPoisonous)
                        {
                            this.doMultipleThingsAtATime = false;
                            this.dontmultiactioncount++;
                        }
                    }
                }
                else if (POWERFULSINGLEACTION >= 1)
                {
                    this.doMultipleThingsAtATime = false;
                }
                else 
                {
                    dontmultiactioncount = 0;
                    this.doMultipleThingsAtATime = true;
                }
            }


            if (this.EnemySecrets.Count >= 1)
            {
                if (moveTodo.actionType == actionEnum.attackWithHero)
                {
                    foreach (SecretItem si in Probabilitymaker.Instance.enemySecrets)
                    {
                        if (si.canBe_noblesacrifice)
                        {
                            this.doMultipleThingsAtATime = false;
                            POWERFULSINGLEACTION++;
                        }
                        else if (daum.bestmove.target.isHero)
                        {
                            if (si.canBe_explosive
                                //|| si.canBe_icebarrier
                                || si.canBe_beartrap)
                            {
                                this.doMultipleThingsAtATime = false;
                                POWERFULSINGLEACTION++;
                            }
                        }
                        else if (!daum.bestmove.target.isHero)
                        {
                            if (si.canBe_snaketrap)
                            {
                                this.doMultipleThingsAtATime = false;
                                POWERFULSINGLEACTION++;
                            }
                            else if (daum.bestmove.own.Angr >= daum.bestmove.target.Hp)
                            {
                                if (si.canBe_iceblock)
                                {
                                    this.doMultipleThingsAtATime = false;
                                    POWERFULSINGLEACTION++;
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
                                this.doMultipleThingsAtATime = false;
                                POWERFULSINGLEACTION++;
                            }
                        }
                    }
                }

                else if (moveTodo.actionType == actionEnum.attackWithMinion)
                {
                    foreach (SecretItem si in Probabilitymaker.Instance.enemySecrets)
                    {
                        if (si.canBe_noblesacrifice
                       || si.canBe_freezing)
                        {
                            this.doMultipleThingsAtATime = false;
                            POWERFULSINGLEACTION++;
                        }

                        else if (daum.bestmove.target.isHero)
                        {
                            if (si.canBe_explosive
                                || si.canBe_beartrap
                                //|| si.canBe_icebarrier
                                || si.canBe_vaporize)
                            {
                                this.doMultipleThingsAtATime = false;
                                POWERFULSINGLEACTION++;
                            }
                        }
                        else if (!daum.bestmove.target.isHero)
                        {
                            if (si.canBe_snaketrap)
                            {
                                this.doMultipleThingsAtATime = false;
                                POWERFULSINGLEACTION++;
                            }
                            else if (daum.bestmove.own.Angr >= daum.bestmove.target.Hp)
                            {
                                if (si.canBe_iceblock)
                                {
                                    this.doMultipleThingsAtATime = false;
                                    POWERFULSINGLEACTION++;
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
                                this.doMultipleThingsAtATime = false;
                                POWERFULSINGLEACTION++;
                            }
                        }
                    }


                }

                if (moveTodo.actionType == actionEnum.playcard)
                {
                    foreach (SecretItem si in Probabilitymaker.Instance.enemySecrets)
                    {
                        if (daum.bestmove.card.card.type == CardDB.cardtype.MOB)
                        {
                            if (si.canBe_mirrorentity)
                            {
                                this.doMultipleThingsAtATime = false;
                                POWERFULSINGLEACTION++;
                            }
                            else if ((si.canBe_snipe)
                                    || (si.canBe_Trial && Playfield.Instance.ownMinions.Count >= 3))
                            {
                                this.doMultipleThingsAtATime = false;
                                POWERFULSINGLEACTION++;
                            }
                        }
                        else if (daum.bestmove.card.card.type == CardDB.cardtype.SPELL)
                        {
                            if (si.canBe_counterspell
                                    || (si.canBe_spellbender && daum.bestmove.target != null && !daum.bestmove.target.isHero)
                                    || si.canBe_cattrick)
                            {
                                this.doMultipleThingsAtATime = false;
                                POWERFULSINGLEACTION++;
                            }
                            else if (Playfield.Instance.enemyMinions.Count >= 1 &&
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
                                    this.doMultipleThingsAtATime = false;
                                    POWERFULSINGLEACTION++;
                                }
                            }
                        }
                    }
                }

            }


            if (moveTodo.actionType == actionEnum.attackWithMinion && ranger_action.Target.IsHero && this.EnemyMinion.Count == 0)
            {
                this.doMultipleThingsAtATime = true;
                this.dontmultiactioncount = 0;
            }
            if (POWERFULSINGLEACTION >= 1) 
            {
                this.doMultipleThingsAtATime = false;
                dontmultiactioncount++;
            }

            return ranger_action;
        }

        /// <summary>
        /// if uses extern a.i.,
        /// invoke when hearthranger did all the actions.
        /// </summary>
        /// <param name="e"></param>
        public override void OnQueryBestMove(QueryBestMoveEventArgs e)
        {

            //don't forget to set HasBestMoveAI property to true in class constructor.
            //or Hearthranger will never query best move !
            //base.HasBestMoveAI = true;
            e.handled = true;
            HSRangerLib.BotAction ranger_action;

            //if (discovercounter == 1) System.Threading.Thread.Sleep(4200);

            try
            {
                HelpFunctions.Instance.ErrorLog("start things...");
                //HR-only fix for being too fast
                //IsProcessingPowers not good enough so always sleep
                //System.Threading.Thread.Sleep(200);
                //todo find better solution
                //better test... we checked if isprocessing is true.. after that, we wait little time and test it again.
                if (this.gameState.IsProcessingPowers || this.gameState.IsBlockingServer || this.gameState.IsBusy || this.gameState.IsMulliganBlockingPowers)
                {
                    HelpFunctions.Instance.logg("HR is too fast...");
                    HelpFunctions.Instance.ErrorLog("HR is too fast...");
                    if (this.gameState.IsProcessingPowers) HelpFunctions.Instance.logg("IsProcessingPowers");
                    if (this.gameState.IsBlockingServer) HelpFunctions.Instance.logg("IsBlockingServer");
                    if (this.gameState.IsBusy) HelpFunctions.Instance.logg("IsBusy");
                    if (this.gameState.IsMulliganBlockingPowers) HelpFunctions.Instance.logg("IsMulliganBlockingPowers");
                }

                HelpFunctions.Instance.ErrorLog("proc check done...");


                //we are conceding
                if (this.isgoingtoconcede)
                {
                    if (HSRangerLib.RangerBotSettings.CurrentSettingsGameType == HSRangerLib.enGameType.The_Arena)
                    {
                        this.isgoingtoconcede = false;
                    }
                    else
                    {
                        ranger_action = CreateRangerConcedeAction();
                        e.action_list.Add(ranger_action);
                        return;
                    }
                }
                if (Settings.Instance.learnmode)
                {
                    e.handled = false;
                    return;
                }

                HelpFunctions.Instance.ErrorLog("update everything...");
                bool templearn = sf.updateEverything(this, behave, doMultipleThingsAtATime, Settings.Instance.useExternalProcess, false); // cant use passive waiting (in this mode i return nothing)
                if (templearn == true) Settings.Instance.printlearnmode = true;

                // actions-queue-stuff
                //  AI has requested to ignore this update, so return without setting any actions.
                if (!shouldSendActions)
                {
                    //Helpfunctions.Instance.ErrorLog("shouldsendactionsblah");
                    shouldSendActions = true;  // unpause ourselves for next time
                    return;
                }


                if (Settings.Instance.learnmode)
                {
                    if (Settings.Instance.printlearnmode)
                    {
                        Ai.Instance.simmulateWholeTurnandPrint();
                    }
                    Settings.Instance.printlearnmode = false;

                    e.handled = false;
                    return;
                }

                //if (Settings.Instance.enemyConcede) Helpfunctions.Instance.ErrorLog("bestmoveVal:" + Ai.Instance.bestmoveValue);

                //if (Ai.Instance.bestmoveValue <= Settings.Instance.enemyConcedeValue && Settings.Instance.enemyConcede)
                //{
                //    e.action_list.Add(CreateRangerConcedeAction());
                //    return;
                //}

                if (Handmanager.Instance.getNumberChoices() >= 1)
                //if (Silverfish.Instance.choiceCards.Count >= 1 && e.action_list.Count == 0 && discovercounter == 0)
                {
                    //detect which choice
                    doMultipleThingsAtATime = false;
                    this.dontmultiactioncount++;

                    //Hrtprozis.Instance.updateLastPlayedCard(lastplayedcard, targetentity);
                    Ai.Instance.playedlastcard = Playfield.Instance.LastPlayedCard;

                    int trackingchoice = Ai.Instance.bestTracking;
                    //int trackingchoice = Ai.Instance.bestTracking;
                    if (Ai.Instance.bestTrackingStatus == 4) HelpFunctions.Instance.logg("dll discovering adapt best choice" + trackingchoice);
                    if (Ai.Instance.bestTrackingStatus == 3) HelpFunctions.Instance.logg("dll discovering using user choice..." + trackingchoice);
                    if (Ai.Instance.bestTrackingStatus == 0) HelpFunctions.Instance.logg("dll discovering using optimal choice..." + trackingchoice);
                    if (Ai.Instance.bestTrackingStatus == 1) HelpFunctions.Instance.logg("dll discovering using suboptimal choice..." + trackingchoice);
                    if (Ai.Instance.bestTrackingStatus == 2) HelpFunctions.Instance.logg("dll discovering using random choice..." + trackingchoice);
                    if (trackingchoice >= 1) trackingchoice = OpenAI.Instance.choiceCardsEntitys[trackingchoice - 1];
                    //there is a tracking/discover effect ongoing! (not druid choice)
                    BotAction trackingaction = new HSRangerLib.BotAction();
                    trackingaction.Actor = this.getEntityWithNumber(trackingchoice);


                    foreach (var item in OpenAI.Instance.choiceCards)
                    {
                        HelpFunctions.Instance.logg("" + item.ToString() + " " + HSRangerLib.CardDefDB.Instance.GetCardEnglishName(item.ToString()));
                    }


                    if (trackingaction.Actor != null)
                    {
                        //DEBUG stuff
                        HelpFunctions.Instance.logg("discovering choice entity" + trackingchoice + " card " + HSRangerLib.CardDefDB.Instance.GetCardEnglishName(trackingaction.Actor.CardId));
                        //Helpfunctions.Instance.logg("actor: cardid " + trackingaction.Actor.CardId + " entity " + trackingaction.Actor.EntityId);

                        if (trackingaction != null)
                        {
                            e.action_list.Add(trackingaction);
                            System.Threading.Thread.Sleep(3200);
                            return;
                        }
                    }
                    //string filename = "silvererror" + DateTime.Now.ToString("_yyyy-MM-dd_HH-mm-ss") + ".xml";
                    //Helpfunctions.Instance.logg("create errorfile " +  filename);
                    //this.gameState.SaveToXMLFile(filename);
                }


                else if (!doMultipleThingsAtATime || this.dontmultiactioncount >= 1)
                {
                    discovercounter = 0;
                    //this is used if you cant queue actions (so ai is just sending one action at a time)
                    Action moveTodo = Ai.Instance.bestmove;
                    //Helpfunctions.Instance.ErrorLog("dontmultiactioncount " + dontmultiactioncount);
                    if (moveTodo == null || moveTodo.actionType == actionEnum.endturn)
                    {
                        //simply clear action list, hearthranger bot will endturn if no action can do.
                        e.action_list.Clear();

                        if (Settings.Instance.enemyConcede) HelpFunctions.Instance.ErrorLog("bestmoveVal:" + Ai.Instance.bestmoveValue);

                        if (Ai.Instance.bestmoveValue <= Settings.Instance.enemyConcedeValue && Settings.Instance.enemyConcede)
                        {
                            e.action_list.Add(CreateRangerConcedeAction());
                            return;
                        }

                        BotAction endturnmove = new HSRangerLib.BotAction();
                        endturnmove.Type = BotActionType.END_TURN;
                        HelpFunctions.Instance.ErrorLog("end turn action");
                        e.action_list.Add(endturnmove);
                        if (POWERFULSINGLEACTION >= 1 || dontmultiactioncount >= 1)
                        {
                            //Helpfunctions.Instance.ErrorLog("찾는거종료1" + POWERFULSINGLEACTION);
                            //Helpfunctions.Instance.logg("찾는거종료1" + POWERFULSINGLEACTION);
                            //Helpfunctions.Instance.ErrorLog("찾는거종료1" + dontmultiactioncount);
                            //Helpfunctions.Instance.logg("찾는거종료1" + dontmultiactioncount);
                            //Helpfunctions.Instance.ErrorLog("찾는거종료1 doMultipleThingsAtATime " + doMultipleThingsAtATime);
                            //Helpfunctions.Instance.logg("찾는거종료1 doMultipleThingsAtATime " + doMultipleThingsAtATime);
                            POWERFULSINGLEACTION = 0;
                            dontmultiactioncount = 0;
                            doMultipleThingsAtATime = true;
                        }
                        doMultipleThingsAtATime = true;
                        return;
                    }
                    else
                    {
                        shouldSendFakeAction = true;
                    }


                    HelpFunctions.Instance.ErrorLog("play action");
                    moveTodo.print();
                    e.action_list.Add(ConvertToRangerAction(moveTodo));
                }
                else
                {//##########################################################################
                 //this is used if you can queue multiple actions
                 //thanks to xytrix
                    discovercounter = 0;
                    this.queuedMoveGuesses.Clear();
                    this.queuedMoveGuesses.Add(new Playfield());  // prior to any changes, in case HR fails to execute any actions
                    bool hasMoreActions = false;
                    do
                    {
                        HelpFunctions.Instance.ErrorLog("play action..." + (e.action_list.Count() + 1));
                        Action moveTodo = Ai.Instance.bestmove;

                        if (!hasMoreActions && (moveTodo == null || moveTodo.actionType == actionEnum.endturn))
                        {
                            if (Settings.Instance.enemyConcede) HelpFunctions.Instance.ErrorLog("bestmoveVal:" + Ai.Instance.bestmoveValue);

                            if (Ai.Instance.bestmoveValue <= Settings.Instance.enemyConcedeValue && Settings.Instance.enemyConcede)
                            {
                                e.action_list.Add(CreateRangerConcedeAction());
                                return;
                            }
                            HelpFunctions.Instance.ErrorLog("enturn");
                            //simply clear action list, hearthranger bot will endturn if no action can do.
                            BotAction endturnmove = new HSRangerLib.BotAction();
                            endturnmove.Type = BotActionType.END_TURN;
                            e.action_list.Add(endturnmove);
                            hasMoreActions = false;
                        }
                        else
                        {
                            HelpFunctions.Instance.ErrorLog("play action");
                            moveTodo.print();

                            BotAction nextMove = ConvertToRangerAction(moveTodo);
                            if (nextMove == null) return;  // Prevent exceptions for expected errors like missing entityID for new spawned minions

                            e.action_list.Add(nextMove);
                            this.queuedMoveGuesses.Add(new Playfield(Ai.Instance.nextMoveGuess));
                            if (nextMove.Type == BotActionType.CAST_ABILITY || nextMove.Type == BotActionType.CAST_MINION || nextMove.Type == BotActionType.CAST_SPELL || nextMove.Type == BotActionType.CAST_WEAPON) System.Threading.Thread.Sleep(45); // to avoid misplay
                            else System.Threading.Thread.Sleep(5);
                            hasMoreActions = canQueueNextActions();
                            if (hasMoreActions) Ai.Instance.doNextCalcedMove();

                            if (Settings.Instance.enemyConcede) HelpFunctions.Instance.ErrorLog("bestmoveVal:" + Ai.Instance.bestmoveValue);

                            if (Ai.Instance.bestmoveValue <= Settings.Instance.enemyConcedeValue && Settings.Instance.enemyConcede)
                            {
                                e.action_list.Add(CreateRangerConcedeAction());
                                return;
                            }

                        }
                    }
                    while (hasMoreActions);

                    numActionsSent = e.action_list.Count();
                    HelpFunctions.Instance.ErrorLog("sending HR " + numActionsSent + " queued actions");
                    numExecsReceived = 0;
                    if (Settings.Instance.enemyConcede) HelpFunctions.Instance.ErrorLog("bestmoveVal:" + Ai.Instance.bestmoveValue);

                    if (Ai.Instance.bestmoveValue <= Settings.Instance.enemyConcedeValue && Settings.Instance.enemyConcede)
                    {
                        e.action_list.Add(CreateRangerConcedeAction());
                        return;
                    }
                }//##########################################################################
                if (Settings.Instance.enemyConcede) HelpFunctions.Instance.ErrorLog("bestmoveVal:" + Ai.Instance.bestmoveValue);

                if (Ai.Instance.bestmoveValue <= Settings.Instance.enemyConcedeValue && Settings.Instance.enemyConcede)
                {
                    e.action_list.Add(CreateRangerConcedeAction());
                    return;
                }
            }
            catch (Exception Exception)
            {
                using (StreamWriter sw = File.AppendText(Settings.Instance.logpath + "CrashLog" + DateTime.Now.ToString("_yyyy-MM-dd_HH-mm-ss") + ".txt"))
                {
                    sw.WriteLine(Exception.ToString());
                }
                HelpFunctions.Instance.logg("\r\nDLL Crashed! " + DateTime.Now.ToString("_yyyy-MM-dd_HH-mm-ss") + "\r\nStackTrace ---" + Exception.ToString() + "\r\n\r\n");
                HelpFunctions.Instance.ErrorLog("\r\nDLL Crashed! " + DateTime.Now.ToString("_yyyy-MM-dd_HH-mm-ss") + "\r\nStackTrace ---" + Exception.ToString() + "\r\n\r\n");
                HelpFunctions.Instance.flushLogg();
                HelpFunctions.Instance.flushErrorLog();

                if (Settings.Instance.learnmode)
                {
                    e.action_list.Clear();
                }
                throw;
            }
            return;
        }


        public override void OnActionDone(ActionDoneEventArgs e)
        {
            //do nothing here

            //queue stuff
            numExecsReceived++;

            switch (e.done_result)
            {
                case ActionDoneEventArgs.ActionResult.Executed:
                    HelpFunctions.Instance.ErrorLog("HR action " + numExecsReceived + " done <executed>: " + e.action_id); break;
                case ActionDoneEventArgs.ActionResult.SourceInvalid:
                    HelpFunctions.Instance.ErrorLog("HR action " + numExecsReceived + " done <invalid_source>: " + e.action_id); break;
                case ActionDoneEventArgs.ActionResult.TargetInvalid:
                    HelpFunctions.Instance.ErrorLog("HR action " + numExecsReceived + " done <invalid_target>: " + e.action_id); break;
                default:
                    HelpFunctions.Instance.ErrorLog("HR action " + numExecsReceived + " done <default>: " + e.action_id + " " + e.ToString()); break;
            }

        }


        private bool canQueueNextActions()
        {
            if (!Ai.Instance.canQueueNextMoves()) return false;

            // HearthRanger will re-query bestmove after a targeted minion buff. So even though we can queue moves after,
            // there's no point because we'll just print error messages when HearthRanger ignores them.
            if (Ai.Instance.bestmove.actionType == actionEnum.playcard)
            {
                CardDB.cardName card = Ai.Instance.bestmove.card.card.name;

                if (card == CardDB.cardName.abusivesergeant
                    || card == CardDB.cardName.darkirondwarf
                    || card == CardDB.cardName.crueltaskmaster
                    || card == CardDB.cardName.screwjankclunker
                    || card == CardDB.cardName.lancecarrier
                    || card == CardDB.cardName.clockworkknight
                    || card == CardDB.cardName.shatteredsuncleric
                    || card == CardDB.cardName.houndmaster
                    || card == CardDB.cardName.templeenforcer
                    || card == CardDB.cardName.wildwalker
                    || card == CardDB.cardName.defenderofargus
                    || card == CardDB.cardName.direwolfalpha
                    || card == CardDB.cardName.flametonguetotem
                    || card == CardDB.cardName.darkpeddler
                    || card == CardDB.cardName.kingselekk
                    || card == CardDB.cardName.animalcompanion
                    || card == CardDB.cardName.barnes
                    || card == CardDB.cardName.tuskarrtotemic
                    || card == CardDB.cardName.callofthewild
                    || card == CardDB.cardName.quickshot
                    || card == CardDB.cardName.unleashthehounds
                    || card == CardDB.cardName.southseadeckhand)
                {
                    return false;
                }

                if (Ai.Instance.bestmove.card.card.type == CardDB.cardtype.MOB || PenalityManager.Instance.summonMinionSpellsDatabase.ContainsKey(Ai.Instance.bestmove.card.card.name))
                {
                    foreach (Minion mnn in Playfield.Instance.ownMinions)
                    {
                        if (!mnn.silenced && (mnn.name == CardDB.cardName.darkshirecouncilman || mnn.name == CardDB.cardName.knifejuggler))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }


        int lossedtodo = 0;
        int KeepConcede = 0;
        int oldwin = 0;
        private bool autoconcede()
        {
            if (HSRangerLib.RangerBotSettings.CurrentSettingsGameType == HSRangerLib.enGameType.The_Arena) return false;
            if (HSRangerLib.RangerBotSettings.CurrentSettingsGameType == HSRangerLib.enGameType.Play_Ranked) return false;
            int totalwin = this.wins;
            int totallose = this.loses;
            /*if ((totalwin + totallose - KeepConcede) != 0)
            {
                Helpfunctions.Instance.ErrorLog("#info: win:" + totalwin + " concede:" + KeepConcede + " lose:" + (totallose - KeepConcede) + " real winrate:" + (totalwin * 100 / (totalwin + totallose - KeepConcede)));
            }*/



            int curlvl = gameState.CurrentRank;

            if (curlvl > this.concedeLvl)
            {
                this.lossedtodo = 0;
                return false;
            }

            if (this.oldwin != totalwin)
            {
                this.oldwin = totalwin;
                if (this.lossedtodo > 0)
                {
                    this.lossedtodo--;
                }
                HelpFunctions.Instance.ErrorLog("not today!! (you won a game)");
                this.isgoingtoconcede = true;
                return true;
            }

            if (this.lossedtodo > 0)
            {
                this.lossedtodo--;
                HelpFunctions.Instance.ErrorLog("not today!");
                this.isgoingtoconcede = true;
                return true;
            }

            if (curlvl < this.concedeLvl)
            {
                this.lossedtodo = 3;
                HelpFunctions.Instance.ErrorLog("your rank is " + curlvl + " targeted rank is " + this.concedeLvl + " -> concede!");
                HelpFunctions.Instance.ErrorLog("not today!!!");
                this.isgoingtoconcede = true;
                return true;
            }
            return false;
        }

        private bool concedeVSenemy(string ownh, string enemyh)
        {
            if (HSRangerLib.RangerBotSettings.CurrentSettingsGameType == HSRangerLib.enGameType.The_Arena) return false;
            if (HSRangerLib.RangerBotSettings.CurrentSettingsGameType == HSRangerLib.enGameType.Play_Ranked) return false;

            if (Mulligan.Instance.shouldConcede(Hrtprozis.Instance.heroNametoEnum(ownh), Hrtprozis.Instance.heroNametoEnum(enemyh)))
            {
                HelpFunctions.Instance.ErrorLog("not today!!!!");
                this.isgoingtoconcede = true;
                return true;
            }
            return false;
        }

        private void HandleWining()
        {
            this.wins++;
            if (this.isgoingtoconcede)
            {
                this.isgoingtoconcede = false;
            }
            int totalwin = this.wins;
            int totallose = this.loses;
            if ((totalwin + totallose - KeepConcede) != 0)
            {
                HelpFunctions.Instance.ErrorLog("#info: win:" + totalwin + " concede:" + KeepConcede + " lose:" + (totallose - KeepConcede) + " real winrate:" + (totalwin * 100 / (totalwin + totallose - KeepConcede)));
            }
            else
            {
                HelpFunctions.Instance.ErrorLog("#info: win:" + totalwin + " concede:" + KeepConcede + " lose:" + (totallose - KeepConcede) + " real winrate: 100");
            }
            HelpFunctions.Instance.logg("Match Won!");
        }

        private void HandleLosing(bool is_concede)
        {
            this.loses++;
            if (is_concede)
            {
                this.isgoingtoconcede = false;
                this.KeepConcede++;
            }
            this.isgoingtoconcede = false;
            int totalwin = this.wins;
            int totallose = this.loses;
            if ((totalwin + totallose - KeepConcede) != 0)
            {
                HelpFunctions.Instance.ErrorLog("#info: win:" + totalwin + " concede:" + KeepConcede + " lose:" + (totallose - KeepConcede) + " real winrate:" + (totalwin * 100 / (totalwin + totallose - KeepConcede)));
            }
            else
            {
                HelpFunctions.Instance.ErrorLog("#info: win:" + totalwin + " concede:" + KeepConcede + " lose:" + (totallose - KeepConcede) + " real winrate: 100");
            }
            HelpFunctions.Instance.logg("Match Lost :(");

        }

        private Entity getEntityWithNumber(int number)
        {
            foreach (Entity e in gameState.GameEntityList)
            {
                if (number == e.EntityId) return e;
            }
            return null;
        }

        private Entity getCardWithNumber(int number)
        {
            foreach (Entity e in base.FriendHand)
            {
                if (number == e.EntityId) return e;
            }
            return null;
        }
    }
}
