using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_OG_151 : SimTemplate //* Tentacle of N'Zoth
	{
		//Deathrattle: Deal 1 damage to all minions.
        public override void OnDeathrattle(Playfield p, Minion m)
        {
            p.allMinionsGetDamage(1);
        }
	}
}