using mc2.general;
using mc2.managers;
using mc2.mod;
using UnityEngine;

namespace Core
{
    [Mod("core", "Core for mc2")]
    public class Main
    {
        public static GameObject Dirt;
		public static GameObject Bedrock;

        public Main()
        {
            Dirt = GameObject.CreatePrimitive(PrimitiveType.Cube);
            new BlockFactory().SimpleFactory(Dirt, new BlockBuilder
            {
                id = 0,
                shortName = "dirt_block",
                fullName = "Dirt",
                mats = new[]
                {
                    Resources.Load<Material>("Dirt")
                },
                mesh = GameObject.Find("Data").GetComponent<Data>().meshes[0]
            });

            Bedrock = GameObject.CreatePrimitive(PrimitiveType.Cube);
			new BlockFactory().SimpleFactory(Bedrock, new BlockBuilder(){
				id = 1,
				shortName = "adminium_block",
				fullName = "Bedrock",
				isHarvest = false,
				mesh = GameObject.Find("Data").GetComponent<Data>().meshes[0],
				mats = new [] {
					Resources.Load<Material>("Bedrock")
				}
			});

            Messenger<RaycastHit>.AddListener(GameEvents.LeftCl, OnLeftClick);
            Messenger<RaycastHit>.AddListener(GameEvents.MidCl, OnMiddleClick);
            Messenger<RaycastHit, Transform>.AddListener(GameEvents.RightCl, OnRightClick);
        }

        private static void OnRightClick(RaycastHit hit, Transform block)
        {
            if (Managers.MkDest.RightClick(block, hit)) ;
        }

        private static void OnMiddleClick(RaycastHit hit)
        {
            if (Managers.MkDest.MiddleClick(hit)) ;
        }

        private static void OnLeftClick(RaycastHit hit)
        {
            if (Managers.MkDest.LeftClick(hit)) ;
        }

    }
}