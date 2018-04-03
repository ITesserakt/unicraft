using mc2.general;
using mc2.managers;
using mc2.mod;
using UniRx;
using UnityEngine;

namespace Core {
    [Mod("core", "Core for mc2")]
    public class Main {
        public static GameObject Dirt;
        public static GameObject Bedrock;

        public Main() {

            Dirt = GameObject.CreatePrimitive(PrimitiveType.Cube);
            BlockFactory.SimpleFactory(Dirt, new BlockBuilder {
                Id = 0,
                ShortName = "dirt_block",
                FullName = "Dirt",
                Mats = new[] {
                    Resources.Load<Material>("Dirt")
                },
                Mesh = GameObject.Find("Data").GetComponent<Data>().meshes[0]
            });

            Bedrock = GameObject.CreatePrimitive(PrimitiveType.Cube);
            BlockFactory.SimpleFactory(Bedrock, new BlockBuilder() {
                Id = 1,
                ShortName = "adminium_block",
                FullName = "Bedrock",
                IsHarvest = false,
                Mesh = GameObject.Find("Data").GetComponent<Data>().meshes[0],
                Mats = new[] {
                    Resources.Load<Material>("Bedrock")
                }
            });

            MessageBroker.Default
                         .Receive<Messenger>()
                         .Where(msg => msg.id == GameEvents.LeftCl)
                         .Subscribe(msg => OnLeftClick((RaycastHit) msg.data));
            MessageBroker.Default
                         .Receive<Messenger>()
                         .Where(msg => msg.id == GameEvents.MidCl)
                         .Subscribe(msg => OnMiddleClick((RaycastHit) msg.data));
            MessageBroker.Default
                         .Receive<Messenger>()
                         .Where(msg => msg.id == GameEvents.RightCl)
                         .Subscribe(msg => OnRightClick(((RightClickArgs) msg.data).Hit,
                                                        ((RightClickArgs) msg.data).Block));
        }

        private static void OnRightClick(RaycastHit hit, Transform block) {
            if (Managers.MkDest.RightClick(block, hit)) ;
        }

        private static void OnMiddleClick(RaycastHit hit) {
            if (Managers.MkDest.MiddleClick(hit)) ;
        }

        private static void OnLeftClick(RaycastHit hit) {
            if (Managers.MkDest.LeftClick(hit)) ;
        }

    }
}