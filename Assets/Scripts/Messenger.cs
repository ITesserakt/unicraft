using UnityEngine;

namespace mc2.general {
    public class Messenger {
        public MonoBehaviour Sender { get; private set; }
        public uint Id { get; private set; }
        public System.Object[] Data { get; private set; }

        public Messenger(MonoBehaviour sender, uint id, params System.Object[] objects) {
            Sender = sender;
            Id = id;
            Data = objects;
        }

        public static Messenger Create(MonoBehaviour sender, uint id, params System.Object[] objects) {
            return new Messenger(sender, id, objects);
        }
    }
}