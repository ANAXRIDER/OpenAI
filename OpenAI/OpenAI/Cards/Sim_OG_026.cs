using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_OG_026 : SimTemplate //* Eternal Sentinel
	{
		//Battlecry: Unlock your Overloaded Mana Crystals.
		
		public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
            if (own != null && own.own)
            {
                p.owedRecall = 0;
                p.mana += p.currentRecall;
                p.currentRecall = 0;
            }
            else
            {
                p.enemyRecall = 0;
                p.mana += p.enemyCurrentRecall;
                p.enemyCurrentRecall = 0;
            }
		}
	}
}