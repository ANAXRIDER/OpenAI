using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CS2_004 : SimTemplate //powerwordshield
	{

//    verleiht einem diener +2 leben.\nzieht eine karte.
		public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
            p.minionGetBuffed(target, 0, 2);
            p.drawACard(CardDB.cardIDEnum.None, ownplay);
		}

	}
}