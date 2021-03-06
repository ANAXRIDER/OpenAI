using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_OG_311 : SimTemplate //* A Light in the Darkness
    {
        //Discover a minion. Give it +1/+1.

        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.drawACard(CardDB.cardName.unknown, ownplay, true);
            p.owncards[p.owncards.Count - 1].addattack++;
            p.owncards[p.owncards.Count - 1].addHp++;
        }
    }
}