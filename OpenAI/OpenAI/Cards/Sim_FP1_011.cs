using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_FP1_011 : SimTemplate //webspinner
	{

//    todesröcheln:/ fügt eurer hand ein zufälliges wildtier hinzu.
        public override void OnDeathrattle(Playfield p, Minion m)
        {
            p.drawACard(CardDB.cardIDEnum.CS2_120, m.own, true);
        }

	}
}