using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_EX1_057 : SimTemplate //ancientbrewmaster
	{

//    kampfschrei:/ lasst einen befreundeten diener vom schlachtfeld auf eure hand zurückkehren.
		public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
            if (target != null) p.minionReturnToHand(target, target.own, 0);
		}

	}
}