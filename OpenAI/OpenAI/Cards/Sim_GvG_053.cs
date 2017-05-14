using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_GVG_053 : SimTemplate //Shieldmaiden
    {

        //   Battlecry:&lt;/b&gt; Gain 5 Armor.

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.minionGetArmor(own.own ? p.ownHero : p.enemyHero, 5);
        }
    }

}