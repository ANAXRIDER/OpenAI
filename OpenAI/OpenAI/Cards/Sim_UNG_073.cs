using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_UNG_073 : SimTemplate //Rockpool Hunter
    {

        //Battlecry: Give a friendly Murloc +1/+1.

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (target != null && target.handcard.card.race == TAG_RACE.MURLOC) p.minionGetBuffed(target, 1, 1);
        }

    }

}