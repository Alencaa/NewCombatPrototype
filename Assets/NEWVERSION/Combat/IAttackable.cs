namespace CombatV2.Combat
{
    public interface IAttackable
    {
        void ReceiveHit(HitPayload payload);
    }
}
