using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_EX1_250 : SimTemplate //earthelemental
	{

//    spott/, überladung:/ (3)
		public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
            p.changeRecall(own.own, 3);
		}


	}
}