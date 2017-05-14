namespace OpenAI
{
    public class SimTemplate
    {
        public virtual void OnSecretPlay(Playfield p, bool ownplay, Minion attacker, Minion target, out int number)
        {
            number = 0;
        }

        public virtual void OnSecretPlay(Playfield p, bool ownplay, Minion target, int number)
        {
        }

        public virtual void OnSecretPlay(Playfield p, bool ownplay, int number)
        {
        }

        public virtual void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
        }

        public virtual void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
        }

        public virtual void OnAuraStarts(Playfield p, Minion m)
        {
        }

        public virtual void OnAuraEnds(Playfield p, Minion m)
        {
        }

        public virtual void OnInspire(Playfield p, Minion m)
        {
        }

        public virtual void OnEnrageStart(Playfield p, Minion m)
        {
        }

        public virtual void OnEnrageStop(Playfield p, Minion m)
        {
        }

        public virtual void OnAMinionGotHealedTrigger(Playfield p, Minion triggerEffectMinion)
        {
        }

        public virtual void OnAHeroGotHealedTrigger(Playfield p, Minion triggerEffectMinion)
        {
        }

        public virtual void OnTurnEndsTrigger(Playfield p, Minion triggerEffectMinion, bool turnEndOfOwner)
        {
        }

        public virtual void OnTurnStartTrigger(Playfield p, Minion triggerEffectMinion, bool turnStartOfOwner)
        {
        }

        public virtual void OnMinionGotDmgTrigger(Playfield p, Minion triggerEffectMinion, bool ownDmgdMinion)
        {
        }

        public virtual void OnMinionDiedTrigger(Playfield p, Minion triggerEffectMinion, Minion diedMinion)
        {
        }

        public virtual void OnMinionIsSummoned(Playfield p, Minion triggerEffectMinion, Minion summonedMinion)
        {
        }

        public virtual void OnMinionWasSummoned(Playfield p, Minion triggerEffectMinion, Minion summonedMinion)
        {
        }

        public virtual void OnDeathrattle(Playfield p, Minion m)
        {
        }

        public virtual void OnCardIsGoingToBePlayed(Playfield p, CardDB.Card c, bool wasOwnCard, Minion triggerEffectMinion, Minion target, int choice)
        {
        }

        public virtual void OnCardWasPlayed(Playfield p, CardDB.Card c, bool wasOwnCard, Minion triggerEffectMinion)
        {
        }

        public virtual void OnCardWasDiscarded(Playfield p, bool wasOwnCard, Minion triggerEffectMinion)
        {
        }

        public virtual void OnCardIsDiscarded(Playfield p, CardDB.Card card, bool own)
        {
        }

        public virtual void OnCardToDecks(Playfield p, bool ownplay, Minion target, int choice)
        {
        }

        public virtual void OnAdaptChoice(Playfield p, bool ownplay, Minion target, CardDB.cardIDEnum choice)
        {
        }
    }
}
