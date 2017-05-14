using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_EX1_315 : SimTemplate //summoningportal
	{

//    eure diener kosten (2) weniger, aber nicht weniger als (1).
        public override void OnAuraStarts(Playfield p, Minion own)
		{
            if (own.own) p.anzSummoningPortal++;
		}

        public override void OnAuraEnds(Playfield p, Minion m)
        {
            if (m.own) p.anzSummoningPortal--;
        }


	}
}