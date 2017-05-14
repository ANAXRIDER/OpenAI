using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_DREAM_05 : SimTemplate//Nightmare
    {

        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.minionGetBuffed(target, 4, 4);
            if (ownplay)
            {
                target.destroyOnOwnTurnStart = true;
            }
            else
            {
                target.destroyOnEnemyTurnStart = true;
            }
        }

    }
}
