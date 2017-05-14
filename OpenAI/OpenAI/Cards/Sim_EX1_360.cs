using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_EX1_360 : SimTemplate//Humility
    {

        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.minionSetAngrToOne(target);
        }

    }
}
