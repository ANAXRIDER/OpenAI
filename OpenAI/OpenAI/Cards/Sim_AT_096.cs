using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_AT_096 : SimTemplate //Clockwork Knight
    {

        //   Battlecry: Give a friendly Mech +1/+1.

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {

            if (target == null) return;
            p.minionGetBuffed(target, 1, 1);
        }


    }

}