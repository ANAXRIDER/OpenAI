using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_NEW1_014 : SimTemplate //masterofdisguise
	{

//    kampfschrei:/ verleiht einem befreundeten diener verstohlenheit/.
		public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
            if (target != null) target.stealth = true;
		}


	}
}