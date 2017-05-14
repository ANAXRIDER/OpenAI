using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_KAR_006 : SimTemplate //Cloaked Huntress
    {
        // Your Secrets cost (0).
        
        public override void OnAuraStarts(Playfield p, Minion m)
        {
            if (m.own) p.anzOwnCloakedHuntress++;
        }

        public override void OnAuraEnds(Playfield p, Minion m)
        {
            if (m.own) p.anzOwnCloakedHuntress--;
        }
    }
}