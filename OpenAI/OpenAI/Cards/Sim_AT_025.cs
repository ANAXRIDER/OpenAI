using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_AT_025 : SimTemplate //Dark Bargain
    {
        //Destroy 2 random enemy minions. Discard 2 random cards.
        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            if (ownplay)
            {
                Minion choosen2 = p.searchRandomMinion(p.enemyMinions, Playfield.searchmode.searchLowestAttack);
                if (choosen2 != null) p.minionGetDestroyed(choosen2);

                choosen2 = p.searchRandomMinion(p.enemyMinions, Playfield.searchmode.searchLowestAttack);
                if (choosen2 != null) p.minionGetDestroyed(choosen2);
            }
            else
            {
                Minion choosen2 = p.searchRandomMinion(p.ownMinions, Playfield.searchmode.searchHighestAttack);
                if (choosen2 != null) p.minionGetDestroyed(choosen2);

                choosen2 = p.searchRandomMinion(p.ownMinions, Playfield.searchmode.searchHighestAttack);
                if (choosen2 != null) p.minionGetDestroyed(choosen2);
            }

            p.discardACard(ownplay);
            p.discardACard(ownplay);
        }
    }
}