using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_FP1_016 : SimTemplate //wailingsoul
	{

//    kampfschrei:/ bringt eure anderen diener zum schweigen/.
		public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
            List<Minion> temp = (own.own) ? p.ownMinions : p.enemyMinions;
            foreach (Minion m in temp)
            {
                p.minionGetSilenced(m);
            }
		}


	}
}