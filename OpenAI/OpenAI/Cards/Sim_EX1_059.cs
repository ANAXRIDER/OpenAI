using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_EX1_059 : SimTemplate //crazedalchemist
	{

//    kampfschrei:/ vertauscht angriff und leben eines dieners.
        //todo: use buffs after that
		public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
            if (target != null) p.minionSwapAngrAndHP(target);
		}

	}
}