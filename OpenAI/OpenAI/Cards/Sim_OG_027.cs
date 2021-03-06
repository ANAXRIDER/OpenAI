using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_OG_027 : SimTemplate //* Evolve
	{
		//Transform your minions into random minions that cost (1) more.
		
		public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{            
            List<Minion> temp = (ownplay) ? p.ownMinions : p.enemyMinions;
            foreach (Minion m in temp )
            {
                p.minionTransform(m, p.getRandomCardForManaMinion(m.handcard.card.cost + 1));
            }
		}
	}
}