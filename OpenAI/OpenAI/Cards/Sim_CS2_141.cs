using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CS2_141 : SimTemplate //ironforgerifleman
	{

//    kampfschrei:/ verursacht 1 schaden.
		public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
            int dmg = 1;
            p.minionGetDamageOrHeal(target, dmg);
		}


	}
}