using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_GVG_046 : SimTemplate //King of Beasts
    {

        //   Taunt Battlecry: Gain +1 Attack for each other Beast you have.

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {

            int bonusattack = 0;
            List<Minion> temp  = (own.own) ? p.ownMinions : p.enemyMinions;
            foreach (Minion m in temp)
            {
                if ((TAG_RACE)m.handcard.card.race == TAG_RACE.BEAST) bonusattack++;
            }
            p.minionGetBuffed(own, bonusattack, 0);

        }


    }

}