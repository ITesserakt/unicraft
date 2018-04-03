using UnityEngine;

namespace mc2.general {
    public class Messenger {
        public MonoBehaviour sender { get; private set; }
        public uint id { get; private set; }
        public System.Object data { get; private set; }

        public Messenger(MonoBehaviour sender, uint id, System.Object data) {
            this.sender = sender;
            this.id = id;
            this.data = data;
        }

        public static Messenger Create(MonoBehaviour sender, uint id, System.Object data) {
            return new Messenger(sender, id, data);
        }
    }
}