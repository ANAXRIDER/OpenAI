using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_LOE_073 : SimTemplate //Fossilized devilsaur
	{
        //BC: if you control a beast, gain taunt

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {

            List<Minion> temp = (own.own) ? p.ownMinions : p.enemyMinions;
            foreach (Minion m in temp)
            {
                if (m.handcard.card.race == TAG_RACE.BEAST)
                {
                    own.taunt = true;
                    break;
                }
            }
        }

	}

}
