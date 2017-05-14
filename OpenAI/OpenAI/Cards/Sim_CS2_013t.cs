using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CS2_013t : SimTemplate //excessmana
	{

//    zieht eine karte. i&gt;(ihr könnt nur 10 mana in eurer leiste haben.)/i&gt;
		public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
            p.drawACard(CardDB.cardIDEnum.None, ownplay);
		}

	}
}