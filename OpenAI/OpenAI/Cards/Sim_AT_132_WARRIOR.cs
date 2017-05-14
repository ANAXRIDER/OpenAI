using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_AT_132_WARRIOR : SimTemplate //armorup
	{

        //    heldenfähigkeit Gain 4 Armor.

		public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
		    p.minionGetArmor(ownplay ? p.ownHero : p.enemyHero, 4);
		}
	}
}