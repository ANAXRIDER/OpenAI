using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CS2_146 : SimTemplate //southseadeckhand
	{

//    hat ansturm/, während ihr eine waffe angelegt habt.
		public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
            if (own.own)
            {
                if (p.ownWeaponDurability >= 1)
                {
                    p.minionGetCharge(own);
                }
            }
            else
            {
                if (p.enemyWeaponDurability >= 1)
                {
                    p.minionGetCharge(own);
                }
            }
		}

	}
}