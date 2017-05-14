/*
 * TODO:
 * - Make structured
 * - cardids of duplicate + avenge
 * - nozdormu (for computing time :D)
 * - faehrtenlesen (tracking)
 * - lehrensucher cho
 * - scharmuetzel kills all :D
 * - deathlord-guessing
 * - kelthuzad dont know which minion died this turn in rl
 */

using System;

namespace OpenAI
{
    public class Action
    {
        public ActionType actionType;
        public Handmanager.Handcard card;
        //public int cardEntitiy;
        public int place; //= target where card/minion is placed
        public Minion own;
        public Minion target;
        public int druidchoice; // 1 left card, 2 right card
        public int tracking; // 1= leftmost card
        public float penalty;

        public CardDB.cardName name = CardDB.cardName.unknown;

        private static Action instance;
        public static Action Instance
        {
            get
            {
                return instance;
            }
        }

        public Action(ActionType type, Handmanager.Handcard hc, Minion ownCardEntity, int place, Minion target, float pen, int choice, int track = 0)
        {
            this.actionType = type;
            this.card = hc;
            this.own = ownCardEntity;
            this.place = place;
            this.target = target;
            this.penalty = pen;
            this.druidchoice = choice;
            this.tracking = track;

        }

        public Action(string s, Playfield p)
        {
            if (s.StartsWith("play "))
            {
                this.actionType = ActionType.PLAY_CARD;

                int cardEnt = Convert.ToInt32(s.Split(new string[] { "id " }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0]);
                int targetEnt = -1;
                if (s.Contains("target ")) targetEnt = Convert.ToInt32(s.Split(new string[] { "target " }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0]);
                int place = 0;
                if (s.Contains("pos ")) place = Convert.ToInt32(s.Split(new string[] { "pos " }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0]);
                int choice = 0;
                if (s.Contains("choice ")) choice = Convert.ToInt32(s.Split(new string[] { "choice " }, StringSplitOptions.RemoveEmptyEntries)[1].Split(' ')[0]);

                this.own = null;

                this.card = new Handmanager.Handcard { entity = cardEnt };

                if (targetEnt >= 0)
                {
                    Minion m = new Minion { entityID = targetEnt };
                    this.target = m;
                }
                else
                {
                    this.target = null;
                }

                this.place = place;
                this.druidchoice = choice;

            }

            if (s.StartsWith("attack "))
            {
                this.actionType = ActionType.ATTACK_WITH_MINION;

                int ownEnt = Convert.ToInt32(s.Split(' ')[1].Split(' ')[0]);
                int targetEnt = Convert.ToInt32(s.Split(' ')[3].Split(' ')[0]);

                this.place = 0;
                this.druidchoice = 0;

                this.card = null;

                Minion m = new Minion {entityID = targetEnt};
                this.target = m;

                Minion o = new Minion {entityID = ownEnt};
                this.own = o;
            }

            if (s.StartsWith("heroattack "))
            {
                this.actionType = ActionType.ATTACK_WITH_HERO;

                int targetEnt = Convert.ToInt32(s.Split(' ')[1].Split(' ')[0]);

                this.place = 0;
                this.druidchoice = 0;

                this.card = null;

                Minion m = new Minion { entityID = targetEnt };
                this.target = m;

                this.own = p.ownHero;

            }

            if (s.StartsWith("useability on target "))
            {
                this.actionType = ActionType.USE_HERO_POWER;

                int targetEnt = Convert.ToInt32(s.Split(' ')[3].Split(' ')[0]);

                this.place = 0;
                this.druidchoice = 0;

                this.card = null;

                Minion m = new Minion { entityID = targetEnt };
                this.target = m;

                this.own = null;

            }

            if (s == "useability")
            {
                this.actionType = ActionType.USE_HERO_POWER;
                this.place = 0;
                this.druidchoice = 0;
                this.card = null;
                this.own = null;
                this.target = null;
            }

            if (s.Contains(" discover "))
            {
                string dc = s.Split(new string[] { " discover " }, StringSplitOptions.RemoveEmptyEntries)[1];
                this.tracking = Convert.ToInt32(dc);
            }

        }

        public Action(Action a)
        {
            this.actionType = a.actionType;
            this.card = a.card;
            this.place = a.place;
            this.own = a.own;
            this.target = a.target;
            this.druidchoice = a.druidchoice;
            this.penalty = a.penalty;
            this.tracking = a.tracking;
        }

        public void print(bool tobuffer = false)
        {
            HelpFunctions help = HelpFunctions.Instance;
            string discover = "";
            if (this.tracking >= 1) discover = " discover " + tracking;
            if (tobuffer)
            {
                if (this.actionType == ActionType.PLAY_CARD)
                {
                    string playaction = "play ";

                    playaction += "id " + this.card.entity;
                    if (this.target != null)
                    {
                        playaction += " target " + this.target.entityID;
                    }

                    if (this.place >= 0)
                    {
                        playaction += " pos " + this.place;
                    }

                    if (this.druidchoice >= 1) playaction += " choice " + this.druidchoice;

                    help.WriteToBuffer(playaction + discover);
                }
                if (this.actionType == ActionType.ATTACK_WITH_MINION && this.target != null)
                {
                    help.WriteToBuffer("attack " + this.own.entityID + " enemy " + this.target.entityID + discover);
                }
                if (this.actionType == ActionType.ATTACK_WITH_HERO && this.target != null)
                {
                    help.WriteToBuffer("heroattack " + this.target.entityID + discover);
                }
                if (this.actionType == ActionType.USE_HERO_POWER)
                {

                    if (this.target != null)
                    {
                        help.WriteToBuffer("useability on target " + this.target.entityID + discover);
                    }
                    else
                    {
                        help.WriteToBuffer("useability" + discover);
                    }
                }
                return;
            }

            //todo sepefeets - consider adding names into actions from the start instead of mapping them here
            string cardname = "";
            string targetname = "";
            Playfield tmpPf = new Playfield();

            if (this.target != null)
            {
                foreach (Minion m in tmpPf.enemyMinions)
                {
                    if (m.entityID == this.target.entityID) targetname = "" + m.name;
                }
                if (tmpPf.enemyHero.entityID == this.target.entityID) targetname = "" + tmpPf.enemyHeroName;
            }
            foreach (Minion m in tmpPf.ownMinions)
            {
                if (this.target != null && m.entityID == this.target.entityID) targetname = "" + m.name;
                if (this.card != null && m.entityID == this.card.entity) cardname = "" + m.name;
                if (this.own != null && m.entityID == this.own.entityID) cardname = "" + m.name;
            }
            if (this.target != null && tmpPf.ownHero.entityID == this.target.entityID) targetname = "" + tmpPf.ownHeroName;
            foreach (Handmanager.Handcard h in tmpPf.owncards)
            {
                if (this.card != null && h.entity == this.card.entity) cardname = "" + h.card.name;
            }


            if (this.actionType == ActionType.PLAY_CARD && this.card != null)
            {
                string playaction = "play ";
                playaction += cardname;
                playaction += " id " + this.card.entity;

                if (this.target != null) playaction += ", target " + targetname + " id " + this.target.entityID;

                if (this.place >= 0) playaction += ", pos " + this.place;

                if (this.druidchoice >= 1) playaction += ", choice " + this.druidchoice;

                help.logg(playaction + discover);
            }
            if (this.actionType == ActionType.ATTACK_WITH_MINION && this.target != null && this.own != null)
            {
                help.logg("attacker: " + cardname + " id " + this.own.entityID + ", enemy: " + targetname + " id " + this.target.entityID + discover);
            }
            if (this.actionType == ActionType.ATTACK_WITH_HERO && this.target != null)
            {
                help.logg("attack with hero, enemy: " + targetname + " id " + this.target.entityID + discover);
            }
            if (this.actionType == ActionType.USE_HERO_POWER)
            {
                help.logg("useability " + discover);
                if (this.target != null)
                {
                    help.logg("on " + (target.own ? "own " : "enemy ") + targetname + " id " + this.target.entityID + discover);
                }
            }
        }
    }    
}
