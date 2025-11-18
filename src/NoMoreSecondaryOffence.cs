using HarmonyLib;
using MGSC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NoSecondaryObjectiveMission
{
    [HarmonyPatch(typeof(MissionFactory), nameof(MissionFactory.CreateProceduralMission))]
    public static class NoMoreSecondaryOffence
    {

        static bool disable_ritual = Plugin.ConfigGeneral.ModData.GetConfigValue<bool>("Missiongen_Disable_Ritual", false);
        public static void Postfix(ProceduralMissionType missionType, Station station, Faction beneficiary, Faction victim, MissionFactory __instance, ref Mission __result)
        {
            if (disable_ritual) {
                bool reversal = false;
                if (missionType == ProceduralMissionType.Ritual && disable_ritual)
                {
                    missionType = ProceduralMissionType.Sabotage;
                    reversal = true;
                }
                if (reversal) {
                    SpaceTime spaceTime = __instance._state.Get<SpaceTime>();
                    int num = (missionType == ProceduralMissionType.BramfaturaInvasion) ? 1 : MissionSystem.GetObjectiveVariantsCount(beneficiary.FactionType, victim.FactionType, missionType);
                    int num2 = UnityEngine.Random.Range(1, num + 1);
                    int num3 = UnityEngine.Random.Range(Data.Global.MissionMinLifeTimeHours, Data.Global.MissionMaxLifeTimeHours + 1);
                    Mission mission = new Mission
                    {
                        StationId = station.Id,
                        BeneficiaryFactionId = beneficiary.Id,
                        VictimFactionId = victim.Id,
                        CreationTime = spaceTime.Time,
                        ExpireTime = spaceTime.Time.AddHours((double)num3),
                        BramfaturaId = station.BramfaturaId,
                        MissionDifficulty = UnityEngine.Random.Range(1, 6),
                        ProcMissionType = missionType,
                        ProcSubTypeVariant = num2,
                        StoryId = FormatHelper.ToMissonSubTypeId(missionType, beneficiary.FactionType, victim.FactionType, num2),
                        StagesNameId = station.Record.MissionNameTemplateId,
                        WorldStructure = new GameWorldStructure()
                    };
                    ProcMissionTemplate record = Data.ProcMissionTemplates.GetRecord(station.Record.MissionTemplateId, true);
                    if (num == 0)
                    {
                        Debug.LogError(string.Format("Error! Failed create mission, {0}x{1}, of type {2}, no variants!", beneficiary.FactionType, victim.FactionType, mission.ProcMissionType));
                    }
                    __instance.InitProceduralMissionWinCondition(mission, beneficiary, victim);
                    __instance.GenerateProceduralStructure(mission, record, beneficiary, victim.Id, station.Record.StationType);
                    __instance.InitDropPoints(mission, victim);
                    __instance.InitReward(mission, beneficiary);
                    __instance.InitReputation(mission, victim);
                    __result = mission;
                }
            }
        }
    }
}
