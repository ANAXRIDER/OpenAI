using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_OG_255 : SimTemplate //* Doomcaller
	{
		//Battlecry: Give your C'Thun +2/+2 (wherever it is). If it's dead, shuffle it into your deck.
		
		public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			p.anzOgOwnCThunHpBonus += 2;
			p.anzOgOwnCThunAngrBonus += 2;
		}
	}
}