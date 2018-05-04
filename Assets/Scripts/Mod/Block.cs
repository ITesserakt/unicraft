using System;
using mc2.managers;
using UnityEngine;

namespace mc2.mod {
    public class Block : MonoBehaviour {
        //For debuging
        [SerializeField] private uint _id;
        [SerializeField] private string _shortName;
        [SerializeField] private string _fullName;
        [SerializeField] private bool _isHarvest;
        [SerializeField] private Collider[] _nearestBlocks;

        public uint Id {
            get { return _id; }
            protected internal set { _id = value; }
        }

        public string ShortName {
            get { return _shortName; }
            protected internal set { _shortName = value; }
        }

        public string FullName {
            get { return _fullName; }
            protected internal set { _fullName = value; }
        }

        public bool IsHarvest {
            get { return _isHarvest; }
            protected internal set { _isHarvest = value; }
        }

        [Obsolete("Не работает")]
        public Collider[] NearestBlocks {
            get { return _nearestBlocks; }
            protected internal set { _nearestBlocks = value; }
        }
    }

    public class BlockFactory : BlockBuilder {
        public virtual void SimpleFactory(GameObject g, BlockBuilder builder) {
            var block = g.AddComponent<Block>();
            block.Id = GameRegistry.RegId();
            block.ShortName = builder.ShortName;
            block.FullName = builder.FullName;
            block.IsHarvest = builder.IsHarvest;

            g.GetComponent<Renderer>().sharedMaterials = builder.Mats;
            g.GetComponent<MeshFilter>().sharedMesh = builder.Mesh;

            g.name = builder.FullName;

            g.transform.Rotate(builder.Rotation);

            GameRegistry.RegBlock(g, block);
        }
    }

    public class BlockBuilder {
        private string _shortName;
        private string _fullName;

        public string ShortName {
            get { return _shortName; }
            set {
                _shortName = GameRegistry.RegSName(value);
            }
        }

        public string FullName {
            get { return _fullName; }
            set {
                _fullName = GameRegistry.RegFName(value);
            }
        }

        public bool IsHarvest { get; set; }

        public Material[] Mats { get; set; }

        public Mesh Mesh { get; set; }

        public Vector3 Rotation { get; set; }
    }
}
