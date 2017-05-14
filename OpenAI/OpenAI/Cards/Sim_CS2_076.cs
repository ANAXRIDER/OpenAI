using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_CS2_076 : SimTemplate//Assassinate
    {

        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.minionGetDestroyed(target);
        }

    }
}
