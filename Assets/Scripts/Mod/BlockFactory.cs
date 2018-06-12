using mc2.general;
using mc2.utils;
using UnityEngine;
using static mc2.mod.GameRegistry;
using System.Linq;

namespace mc2.mod {

    // ReSharper disable once UnusedMember.Global
    public class BlockFactory : IFactory {
        public string ShortName { private get; set; }
        public string FullName { private get; set; }
        public Vector3 Rotation { private get; set; }
        public Mesh Mesh1 { private get; set; }
        public Material[] Materials { private get; set; }
        public bool IsHarvestable { private get; set; }

        public IItem Generate() {

            var g = GameObject.CreatePrimitive(PrimitiveType.Cube);

            var block = g.AddComponent<Block>();
            block.Id = RegId();
            block.Sender = g.transform;
            block.ShortName = RegSName(ShortName + "_block");
            block.FullName = RegFName(FullName);
            block.IsHarvest = IsHarvestable;

            g.GetComponent<Renderer>().sharedMaterials = Materials;
            g.GetComponent<MeshFilter>().sharedMesh = Mesh1;

            g.name = FullName;

            g.transform.Rotate(Rotation);

            RegBlock(g, block);

            return block;
        }

        private BlockFactory() {
            ShortName = null;
            FullName = null;
            IsHarvestable = true;
            Materials = null;
            Mesh1 = null;
            Rotation = default(Vector3);
        }

        public static BlockFactory Setup(string fullName,
                                         string shortName, Material[] mats, Mesh mesh = null, bool isHarvest = true,
                                         Vector3 rotation = default(Vector3)) {
            return new BlockFactory {
                FullName = fullName,
                IsHarvestable = isHarvest,
                Materials = mats,
                Mesh1 = mesh ? mesh : GameObject.Find("Data").GetComponent<Data>().Meshes[0],
                Rotation = rotation,
                ShortName = shortName
            };
        }
    }
}