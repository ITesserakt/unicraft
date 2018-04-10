using mc2.general;
using mc2.managers;
using mc2.mod;
using UniRx;
using UnityEngine;

namespace Core {
    public class Main : IMod {

        public static GameObject Dirt;
        public static GameObject Bedrock;
        public static GameObject Grass;

        public void OnRightClick(RaycastHit hit, Transform block) {
            if (!hit.transform.CompareTag("Player"))
                Managers.MkDest.RightClick(block, hit);
        }

        private static void OnMiddleClick(RaycastHit hit) {
            if (!hit.transform.CompareTag("Player"))
                Managers.MkDest.MiddleClick(hit);
        }

        public void OnLeftClick(RaycastHit hit) {
            if (!hit.transform.CompareTag("Player"))
                Managers.MkDest.LeftClick(hit);
        }

        public void PreLoad() {
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

            var grassFromTemplate = Resources.Load<GameObject>("Grass");
            Grass = GameObject.CreatePrimitive(PrimitiveType.Cube);
            BlockFactory.SimpleFactory(Grass, new BlockBuilder {
                ShortName = "grass_block",
                FullName = "Grass",
                IsHarvest = true,
                Mesh = grassFromTemplate.GetComponent<MeshFilter>().sharedMesh,
                Mats = grassFromTemplate.GetComponent<Renderer>().sharedMaterials,
                Rotation = new Vector3(-90, 0)
            });
        }

        public void Load() {
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
    }
}