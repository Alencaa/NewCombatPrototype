namespace CombatV2.Combat
{
    public interface IBlockable
    {
        bool TryBlock(HitPayload payload);
        bool TryParry(HitPayload payload);
    }
}
