using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_OG_320 : SimTemplate //* Midnight Drake
	{
		// Battlecry: Gain +1 attack for each other card in your hand.
		
		public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
            p.minionGetBuffed(own, (own.own) ? p.owncards.Count : p.enemyAnzCards, 0);
		}
	}
}