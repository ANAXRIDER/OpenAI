using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_EX1_581 : SimTemplate //sap
	{

//    lasst einen feindlichen diener auf die hand eures gegners zurückkehren.

		public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
            p.minionReturnToHand(target, !ownplay, 0);
		}

	}
}