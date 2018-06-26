using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace mc2.mod {
    [Serializable]
    public class Block : MonoBehaviour, IItem {
        //For debug
        [SerializeField] private int _id;
        [SerializeField] private string _shortName;
        [SerializeField] private string _fullName;
        [SerializeField] private bool _isHarvest;

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

        #endregion

        public static implicit operator GameObject(Block b) => b.gameObject;
        public static explicit operator Block(GameObject g) => Get(g);

        private static Block Get(GameObject g) {
            var block = g.GetComponent<Block>();
            if (block)
                return block;

            throw new InvalidOperationException("Игровой объект должен быть блоком");
        }

        public static bool IsBlock(GameObject g) {
            var block = g.GetComponent<Block>();
            return block;
        }

        public static bool IsBlock(Transform t) => IsBlock(t.gameObject);
        public static bool IsBlock(RaycastHit hit) => IsBlock(hit.transform);
    }
}