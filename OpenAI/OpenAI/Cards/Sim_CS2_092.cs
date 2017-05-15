using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_CS2_092 : SimTemplate//blessing of kings
    {

        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.minionGetBuffed(target, 4, 4);
        }

    }
}
