using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CS2_042 : SimTemplate //fireelemental
	{

//    kampfschrei:/ verursacht 3 schaden.
		public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
            int dmg = 3;
            p.minionGetDamageOrHeal(target, dmg);
           
		}

	}
}