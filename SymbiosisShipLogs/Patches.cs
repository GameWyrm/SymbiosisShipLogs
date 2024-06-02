using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace SymbiosisShipLogs
{
    [HarmonyPatch]
    public class Patches
    {
        [HarmonyPrefix, HarmonyPatch(typeof(OWItem), nameof(OWItem.PickUpItem))]
        public static void OWItem_PickUpItem_Prefix(OWItem __instance)
        {
            if (__instance.gameObject.name == "Flower")
            {
                ShipLogManager manager = GameObject.FindObjectOfType<ShipLogManager>();
                manager.RevealFact("SYM_POOL_FLOWER");
            }
        }

        [HarmonyPrefix, HarmonyPatch(typeof(ShipLogController), nameof(ShipLogController.OnPressInteract))]
        public static void ShipLogController_OnPressInteract_Prefix(ShipLogController __instance)
        {
            Transform altTHIcons = __instance.transform.Find("ShipLogPivot/ShipLogCanvas/MapMode/ScaleRoot/PanRoot/ALT TH_ShipLog");
            if (altTHIcons != null)
            {
                // hide useless outline
                altTHIcons.Find("ALT TH Outline").localScale = Vector3.zero;
                Image shipLogIcon = altTHIcons.transform.Find("ALT TH Revealed").GetComponent<Image>();
                if (PlayerData.GetShipLogFactSave("SYM_MAIN_IDENTIFY").revealOrder > -1)
                {
                    shipLogIcon.sprite = SymbiosisShipLogs.Instance.RevealedPlanet;
                }
                else
                {
                    shipLogIcon.sprite = SymbiosisShipLogs.Instance.HiddenPlanet;
                }
            }
        }
    }
}
