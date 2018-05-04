using mc2.utils;
using UnityEngine;

namespace mc2.mod {
    public class BlockFactory : IFactory{
        private string _shortName;
        private string _fullName;

        public Block Generate() {
            var g = GameObject.CreatePrimitive(PrimitiveType.Cube); 
            var block = g.AddComponent<Block>();
            
            block.Id = GameRegistry.RegId();
            block.ShortName = GameRegistry.RegSName(_shortName + "_block");
            block.FullName = GameRegistry.RegFName(_fullName);
            block.IsHarvest = IsHarvest;

            g.GetComponent<Renderer>().sharedMaterials = Mats;
            g.GetComponent<MeshFilter>().sharedMesh = Mesh;

            g.name = _fullName;

            g.transform.Rotate(Rotation);

            GameRegistry.RegBlock(g, block);
            
            return block;
        }

        public BlockFactory(string shortName, string fullName, Mesh mesh = null, Material[] mats = null,
                            bool isHarvest = true, Vector3 rotation = default(Vector3)) {
            _shortName = shortName;
            _fullName = fullName;
            IsHarvest = isHarvest;
            Mats = mats;
            Mesh = mesh ?? GameObject.Find("Data").GetComponent<Data>().Meshes[0];
            Rotation = rotation;
        }

        public Vector3 Rotation { private get; set; }

        public Mesh Mesh { private get; set; }

        public Material[] Mats { private get; set; }

        public bool IsHarvest { private get; set; }
    }
}