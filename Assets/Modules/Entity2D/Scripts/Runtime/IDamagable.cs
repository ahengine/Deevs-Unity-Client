namespace Entities
{
    public interface IDamagable
    {
        bool IsDead { get; }

        Health Health { get; }

        void DoAddHealth(int value);
        void DoDamage(int damage);
        void DoDeath();
    }
}
