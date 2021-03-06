using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_OG_256 : SimTemplate //* Spawn of N'Zoth
	{
		//Deathive your minions +1/+1.
		
		public override void OnDeathrattle(Playfield p, Minion m)
        {
            List<Minion> temp = (m.own) ? p.ownMinions : p.enemyMinions;
            foreach (Minion mn in temp)
            {
				p.minionGetBuffed(mn, 1, 1);
            }
        }
    }
}