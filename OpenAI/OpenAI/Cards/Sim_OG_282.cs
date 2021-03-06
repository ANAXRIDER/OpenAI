using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_OG_282 : SimTemplate //* Blade of C'Thun
	{
		//Battlecry: Destroy a minion. Add its Attack and Health to C'Thun's (wherever it is).
		
		public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
            if(target != null)
			{
                if (own.own)
                {
                    p.anzOgOwnCThunHpBonus += target.Hp;
                    p.anzOgOwnCThunAngrBonus += target.Angr;
                }
				p.minionGetDestroyed(target);
			}
		}
	}
}