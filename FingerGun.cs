using MelonLoader;
using BTD_Mod_Helper;
using FingerGun;
using HarmonyLib;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using BTD_Mod_Helper;
using MelonLoader;
using BTD_Mod_Helper.Api.ModOptions;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Simulation.Bloons;
using Il2CppAssets.Scripts.Unity.Bridge;
using System;

[assembly: MelonInfo(typeof(FingerGun.FingerGun), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace FingerGun
{
  public class FingerGun : BloonsTD6Mod
  {
    public override void OnApplicationStart()
    {
      ModHelper.Msg<FingerGun>("FingerGun loaded!");
    }

    public static readonly ModSettingDouble ClickRadius = new(0d)
    {
      displayName = "Click Radius",
      description = "Controls the size of the circle in which bloons will be popped when you click.\n0 = bloons will be popped at a single point.",
      min = 0d,
      max = 20d,
      slider = true,
    };

    public static readonly ModSettingInt ClickDamage = new(1)
    {
      displayName = "Click Damage",
      description = "Controls how many layers of damage each click does.",
      min = 1,
      max = 99999,
    };

    public static readonly ModSettingInt ClickPierce = new(1)
    {
      displayName = "Click Pierce",
      description = "Controls how many bloons each click can pop.",
      min = 1,
      max = 99,
    };

    public static readonly ModSettingBool ClickCamo = new(true)
    {
      displayName = "Clicks Pop Camo",
    };

    public static readonly ModSettingBool ClickLead = new(false)
    {
      displayName = "Clicks Pop Lead",
    };

  }

  [HarmonyPatch(typeof(InputManager), nameof(InputManager.CursorDown))]
  internal class InputManager_CursorDown
  {
    [HarmonyPostfix]
    internal static void Postfix(InputManager __instance)
    {
      int pierce = FingerGun.ClickPierce;
      BloonToSimulation[] bloonsims = InGame.instance.bridge.GetAllBloons().ToArray();
      foreach (BloonToSimulation sim in bloonsims)
      {
        Bloon b = sim.GetBloon();
        if (
          (FingerGun.ClickCamo || !b.bloonModel.IsCamoBloon()) &&
          (FingerGun.ClickLead || !b.bloonModel.bloonProperties.HasFlag(Il2Cpp.BloonProperties.Lead)) &&
          Math.Sqrt(
          Math.Pow(b.Position.X - __instance.cursorPositionWorld.x, 2) +
          Math.Pow(b.Position.Y - __instance.cursorPositionWorld.y, 2)) < b.radius + FingerGun.ClickRadius)
        {
          b.Damage(FingerGun.ClickDamage, null, true, true, true);
          if(--pierce <= 0) break;
        }
      }
    }
  }
}

