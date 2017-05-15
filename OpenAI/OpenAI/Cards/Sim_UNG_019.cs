using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_UNG_019 : SimTemplate //Air Elemental
    {

        //Can't be targeted by spells or Hero Powers.

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            own.cantBeTargetedBySpellsOrHeroPowers = true;
        }

    }

}