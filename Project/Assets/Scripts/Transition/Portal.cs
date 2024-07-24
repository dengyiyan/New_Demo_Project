using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyProject.Transition
{
    public class Portal : MonoBehaviour
    {
        [SceneName]
        public string sceneTo;
        public string spawnID; // Spawn point ID in the target scene

        public string messageName;
        public string messageText;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            
            if (collision.tag == "Player")
            {
                if (!string.IsNullOrEmpty(sceneTo))
                {
                    EventHandler.CallTransitionEvent(sceneTo, spawnID);
                }
                else
                {
                    EventHandler.CallShowMessageEvent(messageName, messageText);
                }
            }
        }

    }
}
