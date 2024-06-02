using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SymbiosisShipLogs
{
    public class AddShipLogOnInteract : MonoBehaviour
    {
        public string[] logNames;
        public bool requiresFlower;

        private InteractReceiver interactReceiver;

        private void Start()
        {
            interactReceiver = GetComponent<InteractReceiver>();
            if (interactReceiver == null)
            {
                SymbiosisShipLogs.WriteLine($"No interact receiver found on {gameObject.name}!", OWML.Common.MessageType.Error);
                return;
            }
            interactReceiver.OnPressInteract += OnPressInteract;
        }

        private void OnPressInteract()
        {
            ShipLogManager manager = GameObject.FindObjectOfType<ShipLogManager>();
            foreach (string str in logNames)
            {
                manager.RevealFact(str);
            }
            if (requiresFlower && DialogueConditionManager.SharedInstance.GetConditionState("VEIL_LIFTED"))
            {
                manager.RevealFact("SYM_END_RUMOR");
            }
        }
    }
}
