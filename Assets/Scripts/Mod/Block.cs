using System;
using UnityEngine;

namespace mc2.mod {
    [Serializable]
    public class Block : MonoBehaviour, IItem {
        //For debug
        [SerializeField] private int _id;
        [SerializeField] private string _shortName;
        [SerializeField] private string _fullName;
        [SerializeField] private bool _isHarvest;
        [SerializeField] private Transform _sender;

        private Block() { }

        #region properties

        public int Id {
            get { return _id; }
            internal set { _id = value; }
        }

        public string ShortName {
            get { return _shortName; }
            internal set { _shortName = value; }
        }

        public string FullName {
            get { return _fullName; }
            internal set { _fullName = value; }
        }

        public bool IsHarvest {
            get { return _isHarvest; }
            internal set { _isHarvest = value; }
        }

        public Transform Sender {
            get { return _sender; }
            internal set { _sender = value; }
        }

        #endregion

        public static implicit operator GameObject(Block b) => b.Sender.gameObject;

        public static Block Get(GameObject g) {
            var block = g.GetComponent<Block>();
            if (block)
                return block;

            throw new InvalidOperationException("Игровой объект должен быть блоком");
        }

        public static Block Get(Transform t) => Get(t.gameObject);
        public static Block Get(RaycastHit hit) => Get(hit.transform);
    }
}
