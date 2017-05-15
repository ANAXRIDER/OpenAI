using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_OG_080 : SimTemplate //* Xaril, Poisoned Mind
	{
		//Battlecry and Deathrattle: Add a random Toxin card to your hand.
		
        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.drawACard(CardDB.cardName.unknown, own.own, true);
        }

        public override void OnDeathrattle(Playfield p, Minion m)
        {
            p.drawACard(CardDB.cardName.unknown, m.own, true);
        }
    }
}