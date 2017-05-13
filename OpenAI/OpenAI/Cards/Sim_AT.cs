/*
 * TODO:
 * - Sim_AT_002
 */

namespace OpenAI
{
    /// <summary>
    /// Flame Lance
    ///     Deal $8 damage to a minion.
    /// </summary>
    class Sim_AT_001 : SimTemplate
    {
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int dmg = (ownplay) ? p.getSpellDamageDamage(8) : p.getEnemySpellDamageDamage(8);
            p.minionGetDamageOrHeal(target, dmg);
        }
    }

    /// <summary>
    /// Effigy
    ///     Secret: When a friendly minion dies, summon a random minion with the same Cost.
    /// </summary>
    class Sim_AT_002 : SimTemplate
    {
        public override void onSecretPlay(Playfield p, bool ownplay, int number)
        {
            //TODO SERVER

            //we just revive it
            int posi = ownplay ? p.ownMinions.Count : p.enemyMinions.Count;

            CardDB.Card kid = CardDB.Instance.getCardDataFromID(ownplay ? p.revivingOwnMinion : p.revivingEnemyMinion);
            p.callKid(kid, posi, ownplay);
        }
    }

    /// <summary>
    /// Fallen Hero
    ///     Your Hero Power deals 1 extra damage.
    /// </summary>
    class Sim_AT_003 : SimTemplate
    {
        public override void onAuraStarts(Playfield p, Minion m)
        {
            if (m.own)
            {
                p.anzOwnFallenHeros++;
            }
            else
            {
                p.anzEnemyFallenHeros++;
            }
        }

        public override void onAuraEnds(Playfield p, Minion m)
        {
            if (m.own)
            {
                p.anzOwnFallenHeros--;
            }
            else
            {
                p.anzEnemyFallenHeros--;
            }
        }
    }

    /// <summary>
    /// Arcane Blast
    ///     Deal $2 damage to a minion. This spell gets double bonus from Spell Damage;
    /// </summary>
    class Sim_AT_004 : SimTemplate
    {
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int dmg = (ownplay) ? 2 + p.spellpower : 2 + p.enemyspellpower;
            dmg = (ownplay) ? p.getSpellDamageDamage(dmg) : p.getEnemySpellDamageDamage(dmg);
            p.minionGetDamageOrHeal(target, dmg);
        }
    }

    /// <summary>
    /// Polymorph: Boar
    ///     Transform a minion into a 4/2 Boar with Charge.
    /// </summary>
    class Sim_AT_005 : SimTemplate//
    {
        private CardDB.Card boar = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.AT_005t);

        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.minionTransform(target, boar);
        }

    }

    /// <summary>
    /// Boar
    ///     Charge
    /// </summary>
    class Sim_AT_005t : SimTemplate
    {
    }

    /// <summary>
    /// Dalaran Aspirant
    ///     Inspire: Gain Spell Damage +1.
    /// </summary>
    class Sim_AT_006 : SimTemplate
    {
        public override void onInspire(Playfield p, Minion m)
        {
            m.spellpower++;
            if (m.own)
            {
                p.spellpower++;
            }
            else
            {
                p.enemyspellpower++;
            }
        }
    }
}
