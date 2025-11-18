using MGSC;
using ModConfigMenu.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NoSecondaryObjectiveMission
{
    // Token: 0x02000006 RID: 6
    public class ModConfigGeneral
    {
        // Token: 0x0600001D RID: 29 RVA: 0x00002840 File Offset: 0x00000A40
        public ModConfigGeneral(string ModName, string ConfigPath)
        {
            this.ModName = ModName;
            this.ModData = new ModConfigData(ConfigPath);
            this.ModData.AddConfigHeader("STRING:General Settings", "general");

            this.ModData.AddConfigValue("general", "about_1", "STRING:<color=#f51b1b>Newly generated mission</color> that are set to be disabled will be replaced by approriate alternative mission.\n");
            this.ModData.AddConfigValue("general", "Missiongen_Disable_Escort", false, "STRING:No More Escort Mission", "STRING:Newly generated Escort mission will be replaced by Infiltration mission.");
            this.ModData.AddConfigValue("general", "Missiongen_Disable_Control", false, "STRING:No More Control Mission", "STRING:Newly generated Control mission will be replaced by Defense mission.");
            this.ModData.AddConfigValue("general", "Missiongen_Disable_Ritual", false, "STRING:No More Ritual/Counterattack Mission", "STRING:Newly generated Ritual/Counterattack mission will be replaced by Sabotage/Control mission. Works with Control mission disabling.");
            this.ModData.AddConfigValue("general", "about_final", "STRING:<color=#f51b1b>The game must be restarted after setting then saving this config to take effect.</color>\n");
            this.ModData.RegisterModConfigData(ModName);
        }

        // Token: 0x04000011 RID: 17
        private string ModName;

        // Token: 0x04000012 RID: 18
        public ModConfigData ModData;

    }
}
