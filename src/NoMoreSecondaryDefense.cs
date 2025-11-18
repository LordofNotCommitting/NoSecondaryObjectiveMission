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
    [HarmonyPatch(typeof(MissionFactory), nameof(MissionFactory.CreateProceduralReverseMission))]
    public static class NoMoreSecondaryDefense
    {

        static bool disable_escort = Plugin.ConfigGeneral.ModData.GetConfigValue<bool>("Missiongen_Disable_Escort", false);

        static bool disable_control = Plugin.ConfigGeneral.ModData.GetConfigValue<bool>("Missiongen_Disable_Control", false);
        public static void Postfix(Mission originalMission, MissionFactory __instance, ref Mission __result)
        {
            if (disable_escort || disable_control) {
                //Plugin.Logger.Log("--- disabling check!");

                SpaceTime spaceTime = __instance._state.Get<SpaceTime>();
                Factions factions = __instance._state.Get<Factions>();
                Stations stations = __instance._state.Get<Stations>();
                ProceduralMissionType reverseMissionType = Data.ProcMissions.Get(originalMission.ProcMissionType).ReverseMissionType;

                bool reversal = false;
                if (reverseMissionType == ProceduralMissionType.Escort && disable_escort) {
                    reverseMissionType = ProceduralMissionType.Infiltration;
                    reversal = true;
                }

                if (reverseMissionType == ProceduralMissionType.Control && disable_control)
                {
                    reverseMissionType = ProceduralMissionType.Defense;
                    reversal = true;
                }
                if (reversal) {
                    Faction faction = factions.Get(originalMission.VictimFactionId, true);
                    Faction faction2 = factions.Get(originalMission.BeneficiaryFactionId, true);
                    int objectiveVariantsCount = MissionSystem.GetObjectiveVariantsCount(faction.FactionType, faction2.FactionType, reverseMissionType);
                    int num = UnityEngine.Random.Range(1, objectiveVariantsCount + 1);
                    int num2 = UnityEngine.Random.Range(Data.Global.MissionMinLifeTimeHours, Data.Global.MissionMaxLifeTimeHours + 1);
                    Station station = stations.Get(originalMission.StationId, true);
                    Mission mission = new Mission
                    {
                        StationId = station.Id,
                        BeneficiaryFactionId = originalMission.VictimFactionId,
                        VictimFactionId = originalMission.BeneficiaryFactionId,
                        CreationTime = spaceTime.Time,
                        ExpireTime = spaceTime.Time.AddHours((double)num2),
                        BramfaturaId = station.BramfaturaId,
                        MissionDifficulty = UnityEngine.Random.Range(1, 6),
                        ProcMissionType = reverseMissionType,
                        ProcSubTypeVariant = num,
                        StoryId = FormatHelper.ToMissonSubTypeId(reverseMissionType, faction.FactionType, faction2.FactionType, num),
                        StagesNameId = station.Record.MissionNameTemplateId,
                        WorldStructure = new GameWorldStructure()
                    };
                    ProcMissionTemplate record = Data.ProcMissionTemplates.GetRecord(station.Record.MissionTemplateId, true);
                    if (objectiveVariantsCount == 0)
                    {
                        Debug.LogError(string.Format("Error! Failed create mission, {0}x{1}, of type {2}, no variants!", faction.FactionType, faction2.FactionType, mission.ProcMissionType));
                    }
                    __instance.InitProceduralMissionWinCondition(mission, faction, faction2);
                    __instance.GenerateProceduralStructure(mission, record, faction, faction2.Id, station.Record.StationType);
                    __instance.InitDropPoints(mission, faction2);
                    __instance.InitReward(mission, faction);
                    __instance.InitReputation(mission, faction2);
                    __result = mission;
                }
            }
        }
    }
}
