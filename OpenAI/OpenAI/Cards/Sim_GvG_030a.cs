using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_GVG_030a : SimTemplate //Attack Mode
	{

        //    +1 Attack.
		public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
            p.minionGetBuffed(own, 1, 0);
		}



	}
}