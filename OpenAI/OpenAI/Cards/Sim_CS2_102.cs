using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CS2_102 : SimTemplate //armorup
	{

//    heldenfähigkeit/\nerhaltet 2 rüstung.

		public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
		    p.minionGetArmor(ownplay ? p.ownHero : p.enemyHero, 2);
		}
	}
}