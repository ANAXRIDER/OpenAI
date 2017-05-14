using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_PART_006 : SimTemplate //Reversing Switch
    {

        //   Swap a minion's Attack and Health.


        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.minionSwapAngrAndHP(target);
        }


    }

}