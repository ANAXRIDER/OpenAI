using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_PART_002 : SimTemplate //Time Rewinder
    {

        //   Return a friendly minion to your hand.


        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.minionReturnToHand(target, target.own, 0);
        }


    }

}