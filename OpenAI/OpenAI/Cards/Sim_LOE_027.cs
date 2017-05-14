using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_LOE_027 : SimTemplate //snipe
	{
        //todo secret
//    geheimnis:/ wenn euer gegner einen diener ausspielt, werden diesem $4 schaden zugefügt.

        public override void OnSecretPlay(Playfield p, bool ownplay, Minion target, int number)
        {
            p.minionGetDestroyed(target);
        }

	}

}
