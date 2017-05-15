using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_EX1_084 : SimTemplate //warsongcommander
	{

        //    Your Charge minions have +1 Attack. //new :D

        /*public override void OnMinionIsSummoned(Playfield p, Minion triggerEffectMinion, Minion summonedMinion)
        {
            if (triggerEffectMinion.own == summonedMinion.own && summonedMinion.handcard.card.Attack <= 3 )
            {
                p.minionGetCharge(summonedMinion);
            }
        }*/

        public override void OnAuraStarts(Playfield p, Minion own)
        {
            
            if (own.own)
            {
                p.anzOwnWarsongCommanders++;
                foreach (Minion mnn in p.ownMinions)
                {
                    if(mnn.charge>=1) p.minionGetBuffed(mnn, 1, 0);
                }
            }
            else
            {
                p.anzEnemyWarsongCommanders++;
                foreach (Minion mnn in p.enemyMinions)
                {
                    if (mnn.charge >= 1) p.minionGetBuffed(mnn, 1, 0);
                }
            }
        }

        public override void OnAuraEnds(Playfield p, Minion own)
        {
            if (own.own)
            {
                p.anzOwnWarsongCommanders--;
                foreach (Minion mnn in p.ownMinions)
                {
                    if (mnn.charge >= 1) p.minionGetBuffed(mnn, -1, 0);
                }
            }
            else
            {
                p.anzEnemyWarsongCommanders--;
                foreach (Minion mnn in p.enemyMinions)
                {
                    if (mnn.charge >= 1) p.minionGetBuffed(mnn, -1, 0);
                }
            }
        }
	}
}