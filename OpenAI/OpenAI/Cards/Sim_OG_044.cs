using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_OG_044 : SimTemplate //* Fandral Staghelm
	{
		//Your Choose One cards have both effects combine.

        public override void OnAuraStarts(Playfield p, Minion own)
        {
            if (own.own) p.anzOwnFandralStaghelm++;
        }

        public override void OnAuraEnds(Playfield p, Minion m)
        {
            if (m.own) p.anzOwnFandralStaghelm--;
        }
	}
}