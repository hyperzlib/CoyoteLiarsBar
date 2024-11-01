using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using ConfigurationManager;

namespace CoyoteLiarsBar
{
    [BepInPlugin("top.hyperz.coyote.liarsbar", "Liars Bar Coyote MOD", "1.0.0.0")]
    public class LiarsBarCoyoteMod : BaseUnityPlugin
    {
        internal static ManualLogSource Log;

        public static ConfigEntry<string> ControllerConnectCode { get; set; }

        public static ConfigEntry<int> InitStrength { get; set; }
        
        public static ConfigEntry<int> AddStrengthOnLier { get; set; }
        public static ConfigEntry<int> FireStrengthOnLier { get; set; }
        public static ConfigEntry<int> FireTimeOnLier { get; set; }

        public static ConfigEntry<int> AddStrengthOnDead { get; set; }
        public static ConfigEntry<int> FireStrengthOnDead { get; set; }
        public static ConfigEntry<int> FireTimeOnDead { get; set; }

        public static ConfigEntry<int> SubStrengthOnAware { get; set; }

        void Awake()
        {
            Log = Logger;
            Log.LogInfo("LiarsBarCoyoteMod loaded!");
            // Start();
        }

        void Start()
        {
            Harmony.CreateAndPatchAll(typeof(LiarsBarCoyoteHooks));
            Log.LogInfo("LiarsBarCoyoteMod started!");

            ControllerConnectCode = Config.Bind("1. 控制器设置", "控制器游戏连接码", "all@http://127.0.0.1:8920",
                new ConfigDescription("Coyote Game Hub控制器的游戏链接码，本地使用时无需更改", null, new ConfigurationManagerAttributes{ Order = 2 }));

            InitStrength = Config.Bind("2. 初始电量", "对局开始重设电量", 0,
                new ConfigDescription("每次对局开始重设电量为指定值", null, new ConfigurationManagerAttributes { Order = 1 }));

            AddStrengthOnLier = Config.Bind("3. 被发现时", "增加电量", 3,
                new ConfigDescription("被发现时增加的电量", null, new ConfigurationManagerAttributes { Order = 3 }));
            FireStrengthOnLier = Config.Bind("3. 被发现时", "一键开火电量", 10,
                new ConfigDescription("被发现时一键开火的电量（0为不启用）", null, new ConfigurationManagerAttributes { Order = 2 }));
            FireTimeOnLier = Config.Bind("3. 被发现时", "一键开火时间", 3,
                new ConfigDescription("被发现时一键开火的时间（秒）", null, new ConfigurationManagerAttributes { Order = 1 }));

            AddStrengthOnDead = Config.Bind("4. 死亡时", "增加电量", 5,
                new ConfigDescription("死亡时增加的电量", null, new ConfigurationManagerAttributes { Order = 3 }));
            FireStrengthOnDead = Config.Bind("4. 死亡时", "一键开火电量", 10,
                new ConfigDescription("死亡时一键开火的电量（0为不启用）", null, new ConfigurationManagerAttributes { Order = 2 }));
            FireTimeOnDead = Config.Bind("4. 死亡时", "一键开火时间", 3,
                new ConfigDescription("死亡时一键开火的时间（秒）", null, new ConfigurationManagerAttributes { Order = 1 }));

            SubStrengthOnAware = Config.Bind("5. 发现别人说谎时", "减少电量", 0,
                new ConfigDescription("发现别人说谎时降低的电量", null, new ConfigurationManagerAttributes { Order = 1 }));
        }
    }
}
