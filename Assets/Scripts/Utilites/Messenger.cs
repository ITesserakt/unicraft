using UnityEngine;

namespace mc2.utils {
    public class Messenger {
        public MonoBehaviour Sender { get; private set; }
        public uint Id { get; private set; }
        public object[] Data { get; private set; }

        public Messenger(MonoBehaviour sender, uint id, params object[] objects) {
            Sender = sender;
            Id = id;
            Data = objects;
        }

        public static Messenger Create(MonoBehaviour sender, uint id, params object[] objects) {
            return new Messenger(sender, id, objects);
        }
    }
}