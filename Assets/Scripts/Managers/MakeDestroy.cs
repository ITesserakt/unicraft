using mc2.mod;
using mc2.ui;
using mc2.utils;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace mc2.managers {
    [DontLoadOnStatup]
    public class MakeDestroy : GameManager {
        public const float MaxDistance = 6f, MinDistance = 1.05f;
        private const byte Width = WorldGenerator.Width;

        private Transform _block;
        private Camera _camera;
        private GameObject _highLight;
        private GameObject _nameOfObj;

        private MakeDestroy() { }

        protected internal override void Loading(GameManager manager) {
            base.Loading(this);

            _nameOfObj = GameObject.Find("NameOfObj");
            if (!IsLoad(_nameOfObj, "NameOfObj")) return;

            _highLight = GameObject.Find("HighLight");
            if (!IsLoad(_highLight, "HighLight")) return;

            _camera = Camera.main;
            if (!IsLoad(_camera, "Camera")) return;

            Status = ManagerStatus.Started;
        }

        protected internal override void Update_() {

            if (PauseScreen.IsPause.Value) {
                _highLight.SetActive(false);
                return;
            }

            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (!Physics.Raycast(ray, out hit, MaxDistance)) {
                _highLight.SetActive(false);
                return;
            }

            _block = GameRegistry.RegisteredBlocks["Dirt"].transform;

            PhantomControl(hit);

            if (Input.GetMouseButton(0))
                MessageBroker.Default.Publish(Messenger.Create(this, GameEvents.LeftCl, hit));

            if (Input.GetMouseButton(1))
                MessageBroker.Default.Publish(Messenger.Create(this, GameEvents.RightCl, hit, _block));

            if (Input.GetMouseButton(2))
                MessageBroker.Default.Publish(Messenger.Create(this, GameEvents.MidCl, hit));
        }

        public bool RightClick(Transform arg1, RaycastHit arg2) {
            var pos = arg2.transform.position;
            pos += arg2.normal;

            var x = Mathf.FloorToInt(pos.x / Width);
            var z = Mathf.FloorToInt(pos.z / Width);
            Transform chTransform = null;
            try {
                chTransform = Managers.FindByName(((WorldGenerator) Managers.StartSequence[1]).Chunks,
                                                  "Chunk " + x + ":" + z).transform;
            }
            catch { /**/}

            var clone =
                ((WorldGenerator) Managers.StartSequence[1]).CloneTo(arg1.gameObject, pos, chTransform);

            MessageBroker.Default
                         .Publish(Messenger.Create(this, GameEvents.BlockUpdate, clone));
            return clone != null;
        }

        public bool LeftClick(RaycastHit hit) {
            if (hit.transform.CompareTag(Managers.BlockTags[0]))
                Destroy(hit.collider.gameObject);
            MessageBroker.Default
                         .Publish(Messenger.Create(this, GameEvents.BlockUpdate, hit.transform.gameObject));
            return true;
        }

        public bool MiddleClick(RaycastHit hit) {
            if (!hit.transform.CompareTag(Managers.BlockTags[1]) &&
                !hit.transform.CompareTag(Managers.BlockTags[0])) return false;

            _block = GameRegistry.RegisteredBlocks[hit.transform.GetComponent<Block>().FullName].transform;
            var namedBlock = _nameOfObj.GetComponent<Text>();
            namedBlock.text = _block.GetComponent<Block>().FullName;
            return true;
        }


        private void PhantomControl(RaycastHit hit) {
            if (!hit.collider.CompareTag(Managers.BlockTags[0]) && !hit.collider.CompareTag(Managers.BlockTags[1]) &&
                (Managers.Player.transform.position - hit.transform.position).sqrMagnitude > MaxDistance ||
                (Managers.Player.transform.position - hit.transform.position).sqrMagnitude < MinDistance)
                _highLight.SetActive(false);

            if (hit.transform.GetComponent<MeshFilter>() != null)
                _highLight.GetComponent<MeshFilter>().mesh = hit.transform.GetComponent<MeshFilter>().mesh;
            _highLight.transform.position = hit.transform.position;
            _highLight.SetActive(true);
        }
    }
}