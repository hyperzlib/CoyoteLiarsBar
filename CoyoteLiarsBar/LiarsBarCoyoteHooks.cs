using System;
using HarmonyLib;
using Mirror;
using CoyoteGameHubSDK;

namespace CoyoteLiarsBar
{
    public class LiarsBarCoyoteHooks
    {
        private static ulong prevCallLierPlayerId = 0;
        private static bool diceGameIsDead = false;

        private static CoyoteGameHubConnector GetApi()
        {
            return new CoyoteGameHubConnector(LiarsBarCoyoteMod.ControllerConnectCode.Value);
        }

        public static void OnGameStart()
        {
            // 当前对局开始
            var api = GetApi();
            var initStrength = LiarsBarCoyoteMod.InitStrength.Value;

            try
            {
                if (initStrength >= 0)
                {
                    api.SetStrength(initStrength);
                }
            }
            catch (Exception ex)
            {
                LiarsBarCoyoteMod.Log.LogError(ex.Message);
            }
        }

        public static void OnPlayerDead()
        {
            // 当前玩家死亡
            var api = GetApi();
            var addStrength = LiarsBarCoyoteMod.AddStrengthOnDead.Value;
            var fireStrength = LiarsBarCoyoteMod.FireStrengthOnDead.Value;
            var fireTime = LiarsBarCoyoteMod.FireTimeOnDead.Value * 1000;

            try
            {
                if (addStrength != 0)
                {
                    api.AddStrength(addStrength);
                }
                if (fireStrength != 0)
                {
                    api.ActionFire(fireStrength, fireTime);
                }
            }
            catch (Exception ex)
            {
                LiarsBarCoyoteMod.Log.LogError(ex.Message);
            }
        }

        public static void OnPlayerLier()
        {
            // 当前玩家被发现
            var api = GetApi();
            var addStrength = LiarsBarCoyoteMod.AddStrengthOnLier.Value;
            var fireStrength = LiarsBarCoyoteMod.FireStrengthOnLier.Value;
            var fireTime = LiarsBarCoyoteMod.FireTimeOnLier.Value * 1000;

            try
            {
                if (addStrength != 0)
                {
                    api.AddStrength(addStrength);
                }
                if (fireStrength != 0)
                {
                    api.ActionFire(fireStrength, fireTime);
                }
            }
            catch (Exception ex)
            {

                LiarsBarCoyoteMod.Log.LogError(ex.Message);
            }
        }

        public static void OnPlayerAware()
        {
            // 当前玩家发现别人说谎
            var api = GetApi();
            var subStrength = LiarsBarCoyoteMod.SubStrengthOnAware.Value;

            try
            {
                if (subStrength != 0)
                {
                    api.SubStrength(subStrength);
                }
            }
            catch (Exception ex)
            {
                LiarsBarCoyoteMod.Log.LogError(ex.Message);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BlorfGamePlay), "UserCode_PlayLiarVoiceRPC", new Type[] { })]
        public static void BlorfGamePlay_UserCode_PlayLiarVoiceRPC(BlorfGamePlay __instance)
        {
            Console.WriteLine("BlorfGamePlay.UserCode_PlayLiarVoiceRPC called!");

            try
            {
                var manager = Traverse.Create(__instance).Field("manager").GetValue<Manager>();
                var currentPlayer = Traverse.Create(__instance).Field("playerStats").GetValue<PlayerStats>();

                prevCallLierPlayerId = currentPlayer.Player_Id;
            }
            catch (Exception e) {
                LiarsBarCoyoteMod.Log.LogError(e);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(DiceGamePlay), "UserCode_PlayLiarVoiceRPC", new Type[] { })]
        public static void DiceGamePlay_UserCode_PlayLiarVoiceRPC(DiceGamePlay __instance)
        {
            Console.WriteLine("DiceGamePlay.UserCode_PlayLiarVoiceRPC called!");

            try
            {
                var manager = Traverse.Create(__instance).Field("manager").GetValue<Manager>();
                var currentPlayer = Traverse.Create(__instance).Field("playerStats").GetValue<PlayerStats>();

                prevCallLierPlayerId = currentPlayer.Player_Id;
            }
            catch (Exception e) {
                LiarsBarCoyoteMod.Log.LogError(e);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BlorfGamePlay), "UserCode_PlayDeadSfx", new Type[] { })]
        public static void BlorfGamePlay_UserCode_PlayDeadSfx(BlorfGamePlay __instance)
        {
            Console.WriteLine("BlorfGamePlay.UserCode_PlayDeadSfx called!");

            try
            {
                var manager = Traverse.Create(__instance).Field("manager").GetValue<Manager>();
                var currentPlayer = Traverse.Create(__instance).Field("playerStats").GetValue<PlayerStats>();
                var localPlayer = manager.GetLocalPlayer();

                Console.WriteLine("Player Dead: {0}", currentPlayer.PlayerName);

                if (localPlayer.Player_Id == currentPlayer.Player_Id)
                {
                    // 当前玩家死亡
                    OnPlayerDead();
                }
            }
            catch (Exception ex)
            {
                LiarsBarCoyoteMod.Log.LogError(ex);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(DiceGamePlay), "UpdateCall", new Type[] { })]
        public static void DiceGamePlay_UpdateCall(DiceGamePlay __instance)
        {
            try
            {
                var manager = Traverse.Create(__instance).Field("manager").GetValue<Manager>();
                var currentPlayer = Traverse.Create(__instance).Field("playerStats").GetValue<PlayerStats>();
                var localPlayer = manager.GetLocalPlayer();

                if (localPlayer.Player_Id == currentPlayer.Player_Id)
                {
                    if (currentPlayer.Dead && !diceGameIsDead)
                    {
                        Console.WriteLine("DiceGamePlay.playerStats.Dead become true [{0}]", currentPlayer.PlayerName);
                        // 当前玩家死亡，一场游戏仅触发一次
                        diceGameIsDead = true;
                        OnPlayerDead();
                    }
                }
            }
            catch (Exception ex)
            {
                LiarsBarCoyoteMod.Log.LogError(ex);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BlorfGamePlay), "UserCode_StartRevolverProcesses__NetworkConnectionToClient__Boolean", new Type[] { typeof(NetworkConnectionToClient), typeof(bool) })]
        public static void BlorfGamePlay_UserCode_StartRevolverProcesses__NetworkConnectionToClient__Boolean(BlorfGamePlay __instance, NetworkConnectionToClient cn, bool playses)
        {
            Console.WriteLine("BlorfGamePlay.UserCode_StartRevolverProcesses__NetworkConnectionToClient__Boolean called!");

            try
            {
                var manager = Traverse.Create(__instance).Field("manager").GetValue<Manager>();
                var currentPlayer = Traverse.Create(__instance).Field("playerStats").GetValue<PlayerStats>();
                var localPlayer = manager.GetLocalPlayer();

                Console.WriteLine("Player is Liar: {0}", currentPlayer.PlayerName);

                if (localPlayer.Player_Id == currentPlayer.Player_Id)
                {
                    // 当前玩家被发现
                    OnPlayerLier();
                }
                else if (prevCallLierPlayerId == localPlayer.Player_Id)
                {

                    // 当前玩家发现别人说谎
                    OnPlayerAware();
                }
            }
            catch (Exception ex)
            {
                LiarsBarCoyoteMod.Log.LogError(ex);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(DiceGamePlay), "PlayDrink", new Type[] { })]
        public static void DiceGamePlay_PlayDrink(DiceGamePlay __instance)
        {
            Console.WriteLine("DiceGamePlay.PlayDrink called!");

            try
            {
                var manager = Traverse.Create(__instance).Field("manager").GetValue<Manager>();
                var currentPlayer = Traverse.Create(__instance).Field("playerStats").GetValue<PlayerStats>();
                var localPlayer = manager.GetLocalPlayer();

                Console.WriteLine("Player is Liar: {0}", currentPlayer.PlayerName);

                if (localPlayer.Player_Id == currentPlayer.Player_Id)
                {
                    // 当前玩家被发现
                    OnPlayerLier();
                }
                else if (prevCallLierPlayerId == localPlayer.Player_Id)
                {

                    // 当前玩家发现别人说谎
                    OnPlayerAware();
                }
            }
            catch (Exception ex)
            {
                LiarsBarCoyoteMod.Log.LogError(ex);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BlorfGamePlayManager), "Start")]
        public static void BlorfGamePlayManager_Start(BlorfGamePlayManager __instance)
        {
            Console.WriteLine("BlorfGamePlayManager.Start called!");

            OnGameStart();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(DiceGamePlayManager), "Start")]
        public static void PostDiceGamePlay(DiceGamePlayManager __instance)
        {
            Console.WriteLine("DiceGamePlayManager.Start called!");

            diceGameIsDead = false;

            OnGameStart();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ChangeSettings), "SetShadow")]
        public static void PostSetShadow(ChangeSettings __instance, int shadowQuality)
        {
            Console.WriteLine("SetShadow called! [{0}]", shadowQuality);
        }
    }
}
