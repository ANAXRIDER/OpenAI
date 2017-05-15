using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_EX1_594 : SimTemplate //vaporize
	{
        //todo secret
//    geheimnis:/ wenn ein diener euren helden angreift, wird er vernichtet.
        public override void OnSecretPlay(Playfield p, bool ownplay, Minion target, int number)
        {
            p.minionGetDestroyed(target);
        }

	}

}