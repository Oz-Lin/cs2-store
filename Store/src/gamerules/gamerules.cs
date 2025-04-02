using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;
using static Store.Config_Config;

namespace Store;

public static class GameRules
{
    private static CCSGameRulesProxy? _gameRulesProxy;
    private static readonly ConVar _mpHalftime = ConVar.Find("mp_halftime")!;
    private static readonly ConVar _mpMaxrounds = ConVar.Find("mp_maxrounds")!;

    public static bool IgnoreWarmUp()
    {
        if (_gameRulesProxy?.IsValid != true)
        {
            _gameRulesProxy = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules").FirstOrDefault();
        }

        return Config.Credits.IgnoreWarmup && (_gameRulesProxy?.GameRules?.WarmupPeriod ?? false);
    }

    public static bool IsPistolRound()
    {
        if (_gameRulesProxy?.IsValid != true)
        {
            _gameRulesProxy = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules").FirstOrDefault();
        }

        bool isHalftime = _mpHalftime.GetPrimitiveValue<bool>();
        int maxRounds = _mpMaxrounds.GetPrimitiveValue<int>();

        return _gameRulesProxy?.GameRules?.TotalRoundsPlayed == 0 ||
               (isHalftime && maxRounds / 2 == _gameRulesProxy?.GameRules?.TotalRoundsPlayed) ||
               (_gameRulesProxy?.GameRules?.GameRestart ?? false);
    }
}