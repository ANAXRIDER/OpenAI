using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_EX1_015 : SimTemplate //noviceengineer
	{

//    kampfschrei:/ zieht eine karte.
		public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
            p.drawACard(CardDB.cardIDEnum.None, own.own);
		}


	}
}