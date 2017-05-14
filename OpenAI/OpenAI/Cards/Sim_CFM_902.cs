using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CFM_902 : SimTemplate //* Aya Blackpaw
	{
		// Battlecry and Deathrattle: Summon a Jade Golem.

        public override void GetBattlecryEffect(Playfield p, Minion m, Minion target, int choice)
        {
            p.callKid(p.getNextJadeGolem(m.own), m.zonepos, m.own);
        }

        public override void OnDeathrattle(Playfield p, Minion m)
        {
            p.callKid(p.getNextJadeGolem(m.own), m.zonepos - 1, m.own);
        }
    }
}