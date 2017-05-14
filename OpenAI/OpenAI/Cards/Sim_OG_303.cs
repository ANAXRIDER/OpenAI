using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_OG_303 : SimTemplate //* Cult Sorcerer
	{
        //Spell Damage +1. After you cast a spell, give your C'Thun +1/+1 (wherever it is).
        public override void OnCardIsGoingToBePlayed(Playfield p, CardDB.Card c, bool wasOwnCard, Minion triggerEffectMinion, Minion target, int choice)
        {
            if (triggerEffectMinion.own == wasOwnCard && c.type == CardDB.cardtype.SPELL)
            {
                p.anzOgOwnCThunHpBonus++;
                p.anzOgOwnCThunAngrBonus++;
            }
        }
		
        public override void OnAuraStarts(Playfield p, Minion own)
		{
            if (own.own) p.spellpower++;
            else p.enemyspellpower++;
		}

        public override void OnAuraEnds(Playfield p, Minion m)
        {
            if (m.own) p.spellpower--;
            else p.enemyspellpower--;
        }
	}
}