using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_EX1_587 : SimTemplate //windspeaker
	{

//    kampfschrei:/ verleiht einem befreundeten diener windzorn/.
		public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
            if (target != null) p.minionGetWindfurry(target);
		}


	}
}