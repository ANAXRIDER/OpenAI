using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_AT_063 : SimTemplate //Acidmaw
    {

        //Whenever another minion takes damage, destroy it
        //destroying done in triggerAMinionGotDmg
        public override void OnAuraStarts(Playfield p, Minion m)
        {
            p.anzAcidmaw++;
        }

        public override void OnAuraEnds(Playfield p, Minion m)
        {
            p.anzAcidmaw--;
        }

       

    }
}