using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_EX1_295 : SimTemplate //iceblock
	{
        //todo secret
//    geheimnis:/ wenn euer held tödlichen schaden erleidet, wird dieser verhindert und der held wird immun/ in diesem zug.
        public override void OnSecretPlay(Playfield p, bool ownplay, Minion target, int number)
        {
            target.Hp += number;
            target.immune = true;
        }

	}

}