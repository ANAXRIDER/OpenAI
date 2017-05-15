using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_PART_003 : SimTemplate //Rusty Horn
    {

        // Give a minion Taunt.   


        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            target.taunt = true;
        }


    }

}