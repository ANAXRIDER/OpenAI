using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_EX1_609 : SimTemplate //snipe
	{
        //todo secret
//    geheimnis:/ wenn euer gegner einen diener ausspielt, werden diesem $4 schaden zugefügt.

        public override void OnSecretPlay(Playfield p, bool ownplay, Minion target, int number)
        {
            int dmg = (ownplay) ? p.getSpellDamageDamage(4) : p.getEnemySpellDamageDamage(4);

            p.minionGetDamageOrHeal(target, dmg);
        }

	}

}
