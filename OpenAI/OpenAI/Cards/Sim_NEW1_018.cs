using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_NEW1_018 : SimTemplate//bloodsail raider
    {
        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
             own.Angr += p.ownWeaponAttack;
        }

    }
}
