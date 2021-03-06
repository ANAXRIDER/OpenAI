using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CFM_312 : SimTemplate //* Jade Chieftain
	{
		// Battlecry: Summon a Jade Golem. Give it Taunt.

        public override void GetBattlecryEffect(Playfield p, Minion m, Minion target, int choice)
        {
            p.callKid(p.getNextJadeGolem(m.own), m.zonepos, m.own, true);
            List<Minion> tmp = m.own ? p.ownMinions : p.enemyMinions;
            int pos = m.zonepos;
            Minion mnn;
            mnn = tmp[Math.Min(pos - 1, 0)];

            //Helpfunctions.Instance.ErrorLog("&&&" + mnn.name);

            if (mnn.playedThisTurn && !mnn.taunt)
            {
                mnn.taunt = true;
                if (mnn.own) p.anzOwnTaunt++;
                else p.anzEnemyTaunt++;
            }


            //List<Minion> tmp = m.own ? p.ownMinions : p.enemyMinions;
            //int pos = m.own ? p.ownMinions.Count : p.enemyMinions.Count;
            //pos--;
            //Minion mnn;
            //for (; pos > -1; pos--)
            //{
            //    mnn = tmp[pos];
            //    if (mnn.playedThisTurn && !mnn.taunt)
            //    {
            //        mnn.taunt = true;
            //        if (mnn.own) p.anzOwnTaunt++;
            //        else p.anzEnemyTaunt++;
            //        break;
            //    }
            //}

        }
    }
}