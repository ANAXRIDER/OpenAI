using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_KAR_057 : SimTemplate //Ivory Knight
    {
        // Battlecry: Discover a spell. Restore Health to your hero equal to its Cost.

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.drawACard(CardDB.cardIDEnum.None, own.own, true);

            if (own.own)
            {
                p.minionGetDamageOrHeal(p.ownHero, 5, true);//assume heal 5
            }
            else
            {
                p.minionGetDamageOrHeal(p.enemyHero, 5, true);//assume heal 5
            }
        }
    }
}