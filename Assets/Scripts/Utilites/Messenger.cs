using System;
using UniRx;
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

        private static Messenger Create(MonoBehaviour sender, uint id, params object[] objects) =>
            new Messenger(sender, id, objects);

        public static IDisposable SubscribeOnEvent<T>(Action<T> onNext, Action<Exception> onError = null,
                                                      Action onCompleted = null) => MessageBroker
                                                                                    .Default.Receive<T>()
                                                                                    .Subscribe(onNext, onError,
                                                                                               onCompleted);

        public static void PublishEvent<T>(T message) {
            MessageBroker.Default.Publish(message);
        }

        public static void PublishEvent(MonoBehaviour sender, uint id, params object[] objects) {
            MessageBroker.Default.Publish(Create(sender, id, objects));
        }

        public static void PublishEvent(MonoBehaviour sender, uint id) {
            MessageBroker.Default.Publish(Create(sender, id, null));
        }
    }
}