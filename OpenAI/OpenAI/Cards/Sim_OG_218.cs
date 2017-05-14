using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_OG_218 : SimTemplate //* Bloodhoof Brave
	{
		//Taunt. Enrage:+3 Attack.
		
        public override void OnEnrageStart(Playfield p, Minion m)
        {
            m.Angr += 3;
        }

        public override void OnEnrageStop(Playfield p, Minion m)
        {
            m.Angr -= 3;
        }
	}
}