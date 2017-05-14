using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_OG_322 : SimTemplate //* Blackwater Pirate
    {
        //Your weapons cost (2) less.
        
        public override void OnAuraStarts(Playfield p, Minion own)
        {
            if (own.own) p.anzBlackwaterPirate++;
        }

        public override void OnAuraEnds(Playfield p, Minion m)
        {
            if (m.own) p.anzBlackwaterPirate--;
        }
    }
}