using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CFM_668 : SimTemplate //* Doppelgangster
	{
		// Battlecry: Summon 2 copies of this minion.

        public override void GetBattlecryEffect(Playfield p, Minion m, Minion target, int choice)
        {
            p.callKid(m.handcard.card, m.zonepos, m.own, true);
            p.callKid(m.handcard.card, m.zonepos, m.own, true);
            List<Minion> temp = (m.own) ? p.ownMinions : p.enemyMinions;
            int count = 0;
            foreach (Minion mnn in temp)
            {
                if (mnn.name == CardDB.cardName.doppelgangster && m.entityID != mnn.entityID && mnn.playedThisTurn)
                {
                    mnn.setMinionTominion(m);
                    count++;
                    if (count >= 2) break;
                }
            }
        }
    }
}