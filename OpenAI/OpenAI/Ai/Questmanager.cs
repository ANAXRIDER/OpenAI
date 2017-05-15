using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    public class QuestManager
    {
        public class QuestItem
        {
            public Dictionary<CardDB.cardName, int> mobsTurn = new Dictionary<CardDB.cardName, int>();
            public CardDB.cardIDEnum Id = CardDB.cardIDEnum.None;
            public int questProgress = 0;
            public int maxProgress = 1000;

            public void Copy(QuestItem q)
            {
                this.Id = q.Id;
                this.questProgress = q.questProgress;
                this.maxProgress = q.maxProgress;
                if (Id == CardDB.cardIDEnum.UNG_067)
                {
                    this.mobsTurn.Clear();
                    foreach (var n in q.mobsTurn) this.mobsTurn.Add(n.Key, n.Value);
                }
            }

            public void Reset()
            {
                this.Id = CardDB.cardIDEnum.None;
                this.questProgress = 0;
                this.maxProgress = 1000;
                this.mobsTurn.Clear();
            }


            public void MinionWasPlayed(Minion m)
            {
                switch (Id)
                {
                    case CardDB.cardIDEnum.UNG_934: if (m.taunt) questProgress++; break;
                    case CardDB.cardIDEnum.UNG_920: if (m.handcard.card.cost == 1) questProgress++; break;
                    case CardDB.cardIDEnum.UNG_067:
                        if (mobsTurn.ContainsKey(m.name)) mobsTurn[m.name]++;
                        else mobsTurn.Add(m.name, 1);
                        int total = mobsTurn[m.name] + QuestManager.Instance.GetPlayedCardFromHand(m.name);
                        if (total > questProgress) questProgress++;
                        break;
                }
            }

            public void MinionWasSummoned(Minion m)
            {
                switch (Id)
                {
                    case CardDB.cardIDEnum.UNG_116: if (m.Angr >= 5) questProgress++; break;
                    case CardDB.cardIDEnum.UNG_940: if (m.handcard.card.deathrattle) questProgress++; break;
                    case CardDB.cardIDEnum.UNG_942: if ((TAG_RACE)m.handcard.card.race == TAG_RACE.MURLOC) questProgress++; break;
                }
            }

            public void SpellWasPlayed(Minion target, int qId)
            {
                switch (Id)
                {
                    case CardDB.cardIDEnum.UNG_954: if (target != null && target.own && !target.isHero) questProgress++; break;
                    case CardDB.cardIDEnum.UNG_028: if (qId > 67) questProgress++; break;
                }
            }

            public void WasDiscarded(int num)
            {
                switch (Id)
                {
                    case CardDB.cardIDEnum.UNG_829: questProgress += num; break;
                }
            }

            public CardDB.cardIDEnum Reward()
            {
                switch (Id)
                {
                    case CardDB.cardIDEnum.UNG_028: return CardDB.cardIDEnum.UNG_028t; //-Quest: Cast 6 spells that didn't start in your deck. Reward: Time Warp.
                    case CardDB.cardIDEnum.UNG_067: return CardDB.cardIDEnum.UNG_067t1; //-Quest: Play four minions with the same name. Reward: Crystal Core.
                    case CardDB.cardIDEnum.UNG_116: return CardDB.cardIDEnum.UNG_116; //-Quest: Summon 5 minions with 5 or more Attack. Reward: Barnabus.
                    case CardDB.cardIDEnum.UNG_829: return CardDB.cardIDEnum.UNG_829t1; //-Quest: Discard 6 cards. Reward: Nether Portal.
                    case CardDB.cardIDEnum.UNG_920: return CardDB.cardIDEnum.UNG_920t1; //-Quest: Play seven 1-Cost minions. Reward: Queen Carnassa.
                    case CardDB.cardIDEnum.UNG_934: return CardDB.cardIDEnum.UNG_934t1; //-Quest: Play 7 Taunt minions. Reward: Sulfuras.
                    case CardDB.cardIDEnum.UNG_940: return CardDB.cardIDEnum.UNG_940t8; //-Quest: Summon 7 Deathrattle minions. Reward: Amara, Warden of Hope.
                    case CardDB.cardIDEnum.UNG_942: return CardDB.cardIDEnum.UNG_942t; //-Quest: Summon 10 Murlocs. Reward: Megafin.
                    case CardDB.cardIDEnum.UNG_954: return CardDB.cardIDEnum.UNG_954t1; //-Quest: Cast 6 spells on your minions. Reward: Galvadon.
                }
                return CardDB.cardIDEnum.None;
            }
        }

        private StringBuilder sb = new StringBuilder("", 500);
        public QuestItem ownQuest = new QuestItem();
        public QuestItem enemyQuest = new QuestItem();
        public Dictionary<CardDB.cardName, int> mobsGame = new Dictionary<CardDB.cardName, int>();
        private CardDB.cardName nextMobName = CardDB.cardName.unknown;
        private int nextMobId = 0;
        private int prevMobId = 0;
        Helpfunctions help;

        private static QuestManager _instance;
        public static QuestManager Instance
        {
            get
            {
                return _instance ?? (_instance = new QuestManager());
            }
        }

        private QuestManager()
        {
            this.help = Helpfunctions.Instance;
        }


        public void UpdateQuestProgress(string questID, int curProgr, int maxProgr, bool ownplay)
        {
            QuestItem tmp = new QuestItem() { Id = CardDB.Instance.cardIdstringToEnum(questID), questProgress = curProgr, maxProgress = maxProgr };
            if (ownplay) this.ownQuest = tmp;
            else this.enemyQuest = tmp;
        }

        public int GetPlayedCardFromHand(CardDB.cardName name)
        {
            if (mobsGame.ContainsKey(name))
                return mobsGame[name];

            return 0;
        }

        public void Reset()
        {
            sb.Clear();
            mobsGame.Clear();
            ownQuest = new QuestItem();
            enemyQuest = new QuestItem();
            nextMobName = CardDB.cardName.unknown;
            nextMobId = 0;
            prevMobId = 0;
        }

        public string GetQuestsString()
        {
            sb.Clear();
            sb.Append("quests: ");
            sb.Append(ownQuest.Id).Append(" ").Append(ownQuest.questProgress).Append(" ").Append(ownQuest.maxProgress).Append(" ");
            sb.Append(enemyQuest.Id).Append(" ").Append(enemyQuest.questProgress).Append(" ").Append(enemyQuest.maxProgress);
            return sb.ToString();
        }

    }
}
