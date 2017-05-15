using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{

    class Sim_AT_050t : SimTemplate//Lightning Jolt
    {
        //Hero power: 'Deal 2 damage.'

        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            if (target != null)
            {
                int dmg = 2;
                if (ownplay)
                {
                    dmg += p.anzOwnFallenHeros;
                    if (p.doublepriest >= 1) dmg *= (2 * p.doublepriest);

                }
                else
                {
                    dmg += p.anzEnemyFallenHeros;
                    if (p.enemydoublepriest >= 1) dmg *= (2 * p.enemydoublepriest);

                }
                p.minionGetDamageOrHeal(target, dmg);
            }
        }

      

    }

    
}
