using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CFM_021 : SimTemplate //* Freezing Potion
	{
		// Freeze an enemy.

        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            target.frozen = true;
        }
    }
}
