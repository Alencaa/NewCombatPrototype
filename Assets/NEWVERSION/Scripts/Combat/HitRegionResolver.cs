using System.Collections.Generic;
using CombatV2.Combat;

public static class HitRegionResolver
{
    public static HitRegionType ResolveHitRegion(List<HitRegionType> regionsHit, GestureType gesture)
    {
        if (regionsHit == null || regionsHit.Count == 0)
            return HitRegionType.Body;

        if (regionsHit.Count == 1)
            return regionsHit[0];

        switch (gesture)
        {
            case GestureType.SlashDown:
                if (regionsHit.Contains(HitRegionType.Head)) return HitRegionType.Head;
                if (regionsHit.Contains(HitRegionType.Body)) return HitRegionType.Body;
                if (regionsHit.Contains(HitRegionType.Leg)) return HitRegionType.Leg;
                break;

            case GestureType.SlashUp:
                if (regionsHit.Contains(HitRegionType.Leg)) return HitRegionType.Leg;
                if (regionsHit.Contains(HitRegionType.Body)) return HitRegionType.Body;
                if (regionsHit.Contains(HitRegionType.Head)) return HitRegionType.Head;
                break;

            case GestureType.SlashLeft:
            case GestureType.SlashRight:
                if (regionsHit.Contains(HitRegionType.Body)) return HitRegionType.Body;
                if (regionsHit.Contains(HitRegionType.Head)) return HitRegionType.Head;
                if (regionsHit.Contains(HitRegionType.Leg)) return HitRegionType.Leg;
                break;

            // Fallback cho diagonal hoặc unknown
            default:
                if (regionsHit.Contains(HitRegionType.Body)) return HitRegionType.Body;
                break;
        }

        return HitRegionType.Body;
    }
}
