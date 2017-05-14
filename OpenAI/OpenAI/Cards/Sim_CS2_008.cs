using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_CS2_008 : SimTemplate//moonfire
    {

        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int dmg = (ownplay) ? p.getSpellDamageDamage(1) : p.getEnemySpellDamageDamage(1);
            p.minionGetDamageOrHeal(target, dmg);
            
        }

    }
}
