using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_EX1_414 : SimTemplate //grommashhellscream
	{

//    ansturm/, wutanfall:/ +6 angriff
        public override void OnEnrageStart(Playfield p, Minion m)
        {
            m.Angr+=6;
        }

        public override void OnEnrageStop(Playfield p, Minion m)
        {
            m.Angr-=6;
        }

	}
}