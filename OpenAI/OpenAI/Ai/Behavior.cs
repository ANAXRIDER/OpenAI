namespace OpenAI
{
    public abstract class Behavior
    {
        public virtual float GetPlayfieldValue(Playfield p)
        {
            return 0;
        }

        public virtual float GetPlayfieldValueEnemy(Playfield p)
        {
            return GetPlayfieldValue(p);
        }

        public virtual float GetOwnMinionValue(Minion m, Playfield p)
        {
            return 0;
        }

        public virtual float GetEnemyMinionValue(Minion m, Playfield p)
        {
            return 0;
        }
    }
}
