using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_LOE_021 : SimTemplate //eyeforaneye
    {
        //todo secret
        //    geheimnis:/ wenn euer held schaden erleidet, wird dem feindlichen helden ebenso viel schaden zugefügt.
        public override void OnSecretPlay(Playfield p, bool ownplay, int number)
        {
            int dmg = 5;
            p.doDmgToRandomEnemyCLIENT2(dmg, true, ownplay);
        }

    }

}