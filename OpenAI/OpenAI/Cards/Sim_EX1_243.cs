using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_EX1_243 : SimTemplate //dustdevil
	{

//    windzorn/, überladung:/ (2)
		public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
            p.changeRecall(own.own, 2);
		}

	}
}