using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_OG_209 : SimTemplate //* Hallazeal the Ascended
	{
        //Whenever your spells deal damage, restore that much Health to your hero.

        public override void OnCardIsGoingToBePlayed(Playfield p, CardDB.Card c, bool wasOwnCard, Minion triggerEffectMinion, Minion target, int choice)
        {
            if (triggerEffectMinion.own == wasOwnCard && c.type == CardDB.cardtype.SPELL)
            {
                //Minion target2 = (wasOwnCard) ? p.ownHero : p.enemyHero;
                //int spellpower = (wasOwnCard) ? p.spellpower : p.enemyspellpower;
                //p.minionGetDamageOrHeal(target2, spellpower);
                Minion target2 = (wasOwnCard) ? p.ownHero : p.enemyHero;
                p.minionGetDamageOrHeal(target2, -p.penman.guessTotalSpellDamage(p, c.name, wasOwnCard));
            }
        }
    }
}