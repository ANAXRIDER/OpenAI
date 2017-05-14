using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_OG_080e : SimTemplate //* Fadeleaf Toxin
	{
		//Give a friendly minion Stealth until your next turn.
		
        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            target.stealth = true;
            target.conceal = true;
        }
    }
}