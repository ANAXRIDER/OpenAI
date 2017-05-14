using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_OG_083 : SimTemplate //* Twilight Flamecaller
	{
		//Battlecry: Deal 1 damage to all enemy minions
		
        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
			p.allMinionOfASideGetDamage(!own.own, 1);
        }
    }
}