using UnityEngine;

namespace mc2.general {
    public sealed class Block : MonoBehaviour {
        public uint Id { get; internal set; }

        public string ShortName { get; internal set; }

        public string FullName { get; internal set; }

        public bool IsHarvest { get; internal set; }

        public Collider[] NearestBlocks { get; internal set; }
    }

    public class BlockFactory : BlockBuilder {
        public void SimpleFactory(GameObject g, BlockBuilder builder) {
            g.AddComponent<Block> ();

            var block = g.GetComponent<Block> ();
            block.Id = builder.id;
            block.ShortName = builder.shortName;
            block.FullName = builder.fullName;
            block.IsHarvest = builder.isHarvest;
            block.NearestBlocks = Physics.OverlapBox (g.transform.position, new Vector3 (1.5f, 1.5f, 1.5f));

            g.GetComponent<Renderer> ().sharedMaterials = builder.mats;
            g.GetComponent<MeshFilter> ().mesh = builder.mesh;

            g.name = builder.fullName;

            g.transform.Rotate (builder.rotation);
        }
    }

    public class BlockBuilder {
        public uint id { get; protected internal set; }

        public string shortName { get; protected internal set; }

        public string fullName { get; protected internal set; }

        private bool _isHarvest = true;

        public bool isHarvest {
            get {
                return _isHarvest;
            }
            protected internal set {
                _isHarvest = value;
            }
        }

        public Material[] mats { get; protected internal set; }

        public Mesh mesh { get; protected internal set; }

        public Vector3 rotation { get; protected internal set; }
    }
}
