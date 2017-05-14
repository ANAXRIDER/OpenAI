using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_UNG_057t1 : SimTemplate //Razorpetal
    {

        //Deal $1 damage.

        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int dmg = (ownplay) ? p.getSpellDamageDamage(1) : p.getEnemySpellDamageDamage(1);
            p.minionGetDamageOrHeal(target, dmg);
        }

    }

}