using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_LOE_105 : SimTemplate //explorershat
    {

        //   give a minion +1/+1 and Deathrattle: Add an Explorer's Hat to your hand.

        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.minionGetBuffed(target, 1, 1);
            target.explorershat++;
        }


    }

}