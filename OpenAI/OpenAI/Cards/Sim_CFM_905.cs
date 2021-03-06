using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CFM_905 : SimTemplate //* Small-Time Recruits
	{
		// Draw three 1-Cost minions from your deck.

        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            if (ownplay)
            {
                CardDB.Card c;
                int count = 0;
                foreach (KeyValuePair<CardDB.cardIDEnum, int> cid in Hrtprozis.Instance.turnDeck)
                {
                    c = CardDB.Instance.getCardDataFromID(cid.Key);
                    if (c.cost == 1 && c.type == CardDB.cardtype.MOB)
                    {
                        for (int i = 0; i < cid.Value; i++)
                        {
                            p.drawACard(c.name, true);
                            count++;
                            if (count > 2) break;
                        }
						if (count > 2) break;
                    }
                }
            }
        }
    }
}