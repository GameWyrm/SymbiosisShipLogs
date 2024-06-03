using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace SymbiosisShipLogs
{
    public class SymbiosisShipLogs : ModBehaviour
    {
        public static SymbiosisShipLogs Instance
        {
            get
            {
                if (instance == null) instance = FindObjectOfType<SymbiosisShipLogs>();
                return instance;
            }
        }

        private static SymbiosisShipLogs instance;

        public Sprite HiddenPlanet;
        public Sprite RevealedPlanet;

        private INewHorizons nh;

        private Dictionary<string, string[]> shipLogObjects = new Dictionary<string, string[]>
        {
            {"Sector/HearthianPlaqueGreen/Plaque/Props_HEA_MuseumPlaque/InteractVolume", ["SYM_GROVE_SIGNS", "SYM_POOL_RUMOR_SIGN_GREEN"]},
            {"Sector/HearthianPlaqueRedFr/Plaque/Props_HEA_MuseumPlaque/InteractVolume", ["SYM_GROVE_SIGNS", "SYM_POOL_RUMOR_SIGN_RED"]},
            {"Sector/HearthianPlaqueBlue/Plaque/Props_HEA_MuseumPlaque/InteractVolume", ["SYM_GROVE_SIGNS", "SYM_POOL_RUMOR_SIGN_BLUE"]},
            {"Sector/HearthianPlaque/Plaque/Props_HEA_MuseumPlaque/InteractVolume", ["SYM_GROVE_SIGNS", "SYM_POOL_RUMOR_SIGN_WHITE"]},
        };

        // unfortunately all these have the name of "ConversationZone" so I can't just get them by path, instead have to use child index
        private Dictionary<int, string[]> shipLogConversationObjects = new Dictionary<int, string[]>
        {
            {24, ["SYM_VILLAGE_SPOKESPERSON", "SYM_MINE_RUMOR"] },
            {28, ["SYM_VILLAGE_MUSEUM", "SYM_GROVE_RUMOR"] },

        };

        private void Awake()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }

        private void Start()
        {
            // Get the New Horizons API and load configs
            nh = ModHelper.Interaction.TryGetModApi<INewHorizons>("xen.NewHorizons");
            nh.LoadConfigs(this);

            nh.GetStarSystemLoadedEvent().AddListener(AddShipLogInteractions);

            RevealedPlanet = GetSpriteFromFile("Symbiosis");
            HiddenPlanet = GetSpriteFromFile("Symbiosis_Hidden");
        }

        private void AddShipLogInteractions(string system)
        {
            if (system != "Jam3") return;
            GameObject altTH = GameObject.Find("ALTTH_Body");
            if (altTH == null) 
            {
                WriteLine("Didn't find the Symbiosis planet!", MessageType.Error);
                return;
            }
            foreach (string s in shipLogObjects.Keys)
            {
                AddShipLogOnInteract addLog = altTH.transform.Find(s).gameObject.GetAddComponent<AddShipLogOnInteract>();
                addLog.logNames = shipLogObjects[s];
            }
            foreach (int i in shipLogConversationObjects.Keys)
            {
                AddShipLogOnInteract addLog = altTH.transform.Find("Sector").GetChild(i).gameObject.GetAddComponent<AddShipLogOnInteract>();
                addLog.logNames = shipLogConversationObjects[i];
                if (i == 24) addLog.requiresFlower = true;
            }
            // Might as well fix flower collider
            GameObject flower = altTH.transform.Find("Sector/Flower").gameObject;
            flower.GetComponent<SphereCollider>().radius = 0.4f;
            GameObject end = GameObject.Find("EndDimension_Body");
            AddShipLogOnInteract endLog = end.transform.Find("Sector/ConversationZone").gameObject.GetAddComponent<AddShipLogOnInteract>();
            endLog.logNames = ["SYM_END_SPOKESPERSON"];
            // Fix unskippable credits BY DESTROYING THEM
            //Destroy(end.transform.Find("Sector/LoadCreditsVolume").gameObject);
        }

        private Sprite GetSpriteFromFile(string fileName)
        {
            try
            {
                string path = Path.Combine([ModHelper.Manifest.ModFolderPath, "planets", fileName + ".png"]);

                byte[] data = null;
                if (File.Exists(path))
                {
                    data = File.ReadAllBytes(path);
                }
                else
                {
                    WriteLine($"Unable to find the texture requested at {path}.", MessageType.Error);
                    return null;
                }
                Texture2D tex = new(512, 512, TextureFormat.RGBA32, false);
                tex.LoadImage(data);

                var rect = new Rect(0, 0, tex.width, tex.height);
                var pivot = new Vector2(0.5f, 0.5f);

                return Sprite.Create(tex, rect, pivot);
            }
            catch (Exception e)
            {
                WriteLine("Unable to load provided texture: " + e.Message, MessageType.Error);
                return null;
            }
        }

        public static void WriteLine(string text, MessageType messageType = MessageType.Message)
        {
            Instance.ModHelper.Console.WriteLine(text, messageType);
        }
    }

}
