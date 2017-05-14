using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CFM_655 : SimTemplate //* Toxic Sewer Ooze
	{
		// Battlecry: Remove 1 Durability from your opponent's weapon.

        public override void GetBattlecryEffect(Playfield p, Minion m, Minion target, int choice)
        {
            p.lowerWeaponDurability(1, !m.own);
        }
    }
}