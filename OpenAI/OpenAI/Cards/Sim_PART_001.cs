using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_PART_001 : SimTemplate //Armor Plating
    {

        //   Give a minion +1 Health.


        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.minionGetBuffed(target, 0, 1);
        }


    }

}