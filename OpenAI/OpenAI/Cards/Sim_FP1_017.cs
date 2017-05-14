using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_FP1_017 : SimTemplate //nerubarweblord
	{

//    diener mit kampfschrei/ kosten (2) mehr.
        public override void OnAuraStarts(Playfield p, Minion own)
		{
            p.anzNerubarWeblord++;
		}

        public override void OnAuraEnds(Playfield p, Minion m)
        {
            p.anzNerubarWeblord--;
        }


	}
}