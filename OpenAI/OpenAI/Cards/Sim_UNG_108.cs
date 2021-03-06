using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_UNG_108 : SimTemplate //Earthen Scales
    {

        //Give a friendly minion +1/+1, then gain Armor equal to its Attack.

        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            if (target != null)
            {
                p.minionGetBuffed(target, 1, 1);
                p.minionGetArmor((ownplay) ? p.ownHero : p.enemyHero, target.Angr);
            }
        }

    }

}