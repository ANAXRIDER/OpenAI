using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_OG_133 : SimTemplate //* N'Zoth, the Corruptor
    {
        //Battlecry: Summon your Deathrattle minions that died this game.

        CardDB CardDBI = CardDB.Instance;
        CardDB.Card kid = null;

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            int kids = 7 - p.ownMinions.Count;

            if (kids > 0)
            {
                foreach (KeyValuePair<CardDB.cardIDEnum, int> e in Probabilitymaker.Instance.ownGraveyard)
                {
                    kid = CardDBI.getCardDataFromID(e.Key);
                    if (kid.deathrattle)
                    {
                        for (int i = 0; i < e.Value; i++)
                        {
                            p.callKid(kid, own.zonepos, own.own, true);
                            kids--;
                            if (kids < 1) break;
                        }
                        if (kids < 1) break;
                    }
                }
            }

            if (kids > 0 && own.own)
            {
                foreach (GraveYardItem m in p.diedMinions.ToArray()) // toArray() because a knifejuggler could kill a minion due to the summon :D
                {
                    CardDB.Card card = CardDB.Instance.getCardDataFromID(m.cardid);
                    if (card.deathrattle) p.callKid(card, p.ownMinions.Count, m.own);
                }
            }
        }
    }
}