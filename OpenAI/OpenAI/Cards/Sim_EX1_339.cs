using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_EX1_339 : SimTemplate //thoughtsteal
	{

//    kopiert 2 karten aus dem deck eures gegners und fügt sie eurer hand hinzu.
		public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
            p.drawACard(CardDB.cardIDEnum.None, ownplay, true);
            p.drawACard(CardDB.cardIDEnum.None, ownplay, true);
		}

	}
}