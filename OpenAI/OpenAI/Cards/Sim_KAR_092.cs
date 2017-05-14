using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_KAR_092 : SimTemplate //Medivh's Valet
    {
        // Battlecry: If you control a Secret, deal 3 damage.

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (target != null && ((own.own) ? p.ownSecretsIDList.Count : p.enemySecretList.Count) >= 1)
            {
                p.minionGetDamageOrHeal(target, 3);
            }
        }
    }
}