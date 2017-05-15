using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_EX1_076 : SimTemplate //pintsizedsummoner
	{

        //todo enemy stuff
//    der erste diener, den ihr in einem zug ausspielt, kostet (1) weniger.
        public override void OnAuraStarts(Playfield p, Minion own)
		{
            if (own.own) p.anzPintSizedSummoner++;
		}

        public override void OnAuraEnds(Playfield p, Minion m)
        {
            if (m.own) p.anzPintSizedSummoner--;
        }

	}
}