using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_GVG_040 : SimTemplate //Siltfin Spiritwalker
    {

        //    Whenever another friendly Murloc dies, draw a card. Overload: (1)

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.changeRecall(own.own, 1);
        }
        // death-effect is handled in playfield -> triggerAMinionDied

    }

}