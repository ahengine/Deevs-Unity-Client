namespace Entities
{
    public interface IDamagable
    {
        bool IsDead { get; }
        bool IsDamaging { get; }
        bool IsDamagable { get; }

        Health Health { get; }

        void DoAddHealth(int value);
        void DoDamage(int damage);

        void DoDamageOnSky(int damage);

        void DoDeath();
    }
}
