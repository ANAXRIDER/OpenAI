using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CS2_034 : SimTemplate //fireblast
	{

//    heldenfähigkeit/\nverursacht 1 schaden.
        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int dmg = 1;
            if (ownplay)
            {
                dmg += p.anzOwnFallenHeros;
                if (p.doublepriest >= 1) dmg *= (2 * p.doublepriest);
                
            }
            else
            {
                dmg += p.anzEnemyFallenHeros;
                if (p.enemydoublepriest >= 1) dmg *= (2 * p.enemydoublepriest);
                
            }
            p.minionGetDamageOrHeal(target, dmg);
        }

	}
}