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
                ShortName = "dirt_block",
                FullName = "Dirt",
                Mats = new[] {
                    Resources.Load<Material>("Dirt")
                },
                Mesh = GameObject.Find("Data").GetComponent<Data>().meshes[0],
                IsHarvest = true
            });

            Bedrock = GameObject.CreatePrimitive(PrimitiveType.Cube);
            BlockFactory.SimpleFactory(Bedrock, new BlockBuilder() {
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
                         .Where(msg => msg.Id == GameEvents.LeftCl)
                         .Subscribe(msg => OnLeftClick((RaycastHit) msg.Data[0]));
            MessageBroker.Default
                         .Receive<Messenger>()
                         .Where(msg => msg.Id == GameEvents.MidCl)
                         .Subscribe(msg => OnMiddleClick((RaycastHit) msg.Data[0]));
            MessageBroker.Default
                         .Receive<Messenger>()
                         .Where(msg => msg.Id == GameEvents.RightCl)
                         .Subscribe(msg => OnRightClick((RaycastHit) msg.Data[0], (Transform) msg.Data[1]));
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