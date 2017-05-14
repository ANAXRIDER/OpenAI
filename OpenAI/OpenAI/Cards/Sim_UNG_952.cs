using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_UNG_952 : SimTemplate //Spikeridged Steed
    {

        //Give a minion +2/+6 and Taunt. When it dies, summon a Stegodon.

        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.minionGetBuffed(target, 2, 6);
            target.taunt = true;
            target.spikeridgedteed++;
        }

    }

}