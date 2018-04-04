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
        public static void SimpleFactory(GameObject g, BlockBuilder builder) {
            g.AddComponent<Block>();

            var block = g.GetComponent<Block>();
            block.Id = GameRegistry.RegId();
            block.ShortName = builder.ShortName;
            block.FullName = builder.FullName;
            block.IsHarvest = builder.IsHarvest;
            block.NearestBlocks = Physics.OverlapBox(g.transform.position, new Vector3(1.5f, 1.5f, 1.5f));

            g.GetComponent<Renderer>().sharedMaterials = builder.Mats;
            g.GetComponent<MeshFilter>().mesh = builder.Mesh;

            g.name = builder.FullName;

            g.transform.Rotate(builder.Rotation);
        }
    }

    public class BlockBuilder {

        private bool _isHarvest = true;
        private string _shortName;
        private string _fullName;

        public string ShortName {
            get { return _shortName; }
            protected internal set {
                GameRegistry.RegSName(value);
                _shortName = value;
            }
        }

        public string FullName {
            get { return _fullName; }
            protected internal set {
                GameRegistry.RegFName(value);
                _fullName = value;
            }
        }

        public bool IsHarvest {
            get { return _isHarvest; }
            protected internal set { _isHarvest = value; }
        }

        public Material[] Mats { get; protected internal set; }

        public Mesh Mesh { get; protected internal set; }

        public Vector3 Rotation { get; protected internal set; }
    }
}
