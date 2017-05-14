using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_EX1_283 : SimTemplate //frostelemental
	{

//    kampfschrei:/ friert/ einen charakter ein.
		public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
            target.frozen = true;
		}


	}
}