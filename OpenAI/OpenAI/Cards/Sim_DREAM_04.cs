using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_DREAM_04 : SimTemplate //dream
	{

//    lasst einen diener auf die hand seines besitzers zurückkehren.

		public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
            p.minionReturnToHand(target, target.own, 0);
		}


	}
}