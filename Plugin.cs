using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using JetpackPocket.Patches;
using LethalConfig;
using LethalConfig.ConfigItems;
using LethalConfig.ConfigItems.Options;

namespace JetpackPocket
{
    [BepInPlugin(ModInfo.modGUID, ModInfo.modName, ModInfo.modVersion)]
    [BepInDependency("ainavt.lc.lethalconfig")]
    public class JetpackPocketPatchBase : BaseUnityPlugin
    {
        private readonly Harmony harmony = new Harmony(ModInfo.modGUID);
        public static JetpackPocketPatchBase instance;
        public ConfigEntry<bool> JetpackPocketConfigEntry;

        internal ManualLogSource logSource;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }

            logSource = BepInEx.Logging.Logger.CreateLogSource(ModInfo.modGUID);

            JetpackPocketConfigEntry = Config.Bind("General", "Carry multiple two-handed items", false, "Enables carrying multiple two-handed items at the same time while having a jetpack.");
            var checkbox = new BoolCheckBoxConfigItem(JetpackPocketConfigEntry, new BoolCheckBoxOptions
            {
                RequiresRestart = false
            });
            LethalConfigManager.AddConfigItem(checkbox);

            harmony.PatchAll(typeof(JetpackPocketPatchBase));
            harmony.PatchAll(typeof(PlayerControllerBPatch));
            harmony.PatchAll(typeof(NetworkPatch));

            logSource.LogInfo(ModInfo.modName + " (version - " + ModInfo.modVersion + ")" + ": patches applied successfully");
        }
    }
}
