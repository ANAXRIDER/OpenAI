using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_OG_150 : SimTemplate //* Aberrant Berserker
	{
		//Enrage: +2 Attack.
		
        public override void OnEnrageStart(Playfield p, Minion m)
        {
            m.Angr += 2;
        }

        public override void OnEnrageStop(Playfield p, Minion m)
        {
            m.Angr -= 2;
        }
	}
}