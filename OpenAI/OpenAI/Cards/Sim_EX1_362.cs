using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_EX1_362 : SimTemplate //argentprotector
	{

//    kampfschrei:/ verleiht einem befreundeten diener gottesschild/.
		public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
            if (target != null) target.divineshild = true;
		}

	}
}