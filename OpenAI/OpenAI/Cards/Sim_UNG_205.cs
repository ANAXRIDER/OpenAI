using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_UNG_205 : SimTemplate //Glacial Shard
    {

        //Battlecry: Freeze an_enemy.

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (!target.own) target.frozen = true;
        }

    }

}