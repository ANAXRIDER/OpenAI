using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_OG_094 : SimTemplate //* Power Word Tentacles
	{
		//Give a minion +2/+6.
		
        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.minionGetBuffed(target, 2, 6);
        }
    }
}
