using mc2.general;
using UnityEngine;
using UnityEngine.UI;

namespace mc2.managers {
    public class MakeDestroy : GameManager {
        public const float MaxDistance = 6f, MinDistance = 1.05f;
        private const byte Width = WorldGenerator.Width;

        private Transform _block;
        private Camera _camera;
        private GameObject _highLight;
        private GameObject _nameOfObj;

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

        private void Update() {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (!Physics.Raycast(ray, out hit, MaxDistance)) {
                _highLight.SetActive(false);
                return;
            }

            _block = Managers.FindByName(Managers.WGenerator.Voxels, "Dirt").transform;

            PhantomControl(hit);

            if (Input.GetMouseButtonUp(0)) {
                Messenger<RaycastHit>.Broadcast(GameEvents.LeftCl, hit);
            }

            if (Input.GetMouseButtonUp(1)) {
                Messenger<RaycastHit, Transform>.Broadcast(GameEvents.RightCl, hit, _block);
            }

            if (Input.GetMouseButtonUp(2))
                Messenger<RaycastHit>.Broadcast(GameEvents.MidCl, hit);
        }

        public bool RightClick(Transform arg1, RaycastHit arg2) {
            var pos = arg2.transform.position;
            pos += arg2.normal;

            var x = Mathf.FloorToInt(pos.x / Width);
            var z = Mathf.FloorToInt(pos.z / Width);
            var chTransform = Managers.FindByName(Managers.WGenerator.Chunks, "Chunk " + x + ":" + z).transform;

            var clone = Managers.WGenerator.ClonePlace(arg1.gameObject, pos, chTransform);

            Messenger<GameObject>.Broadcast(GameEvents.BlockUpdate, clone);
            return clone != null;
        }

        public bool LeftClick(RaycastHit hit) {
            if (hit.transform.CompareTag(Managers.BlockTags[0]))
                Destroy(hit.collider.gameObject);
            Messenger<GameObject>.Broadcast(GameEvents.BlockUpdate, hit.transform.gameObject);
            return true;
        }

        public bool MiddleClick(RaycastHit hit) {
            if (!hit.transform.CompareTag(Managers.BlockTags[1]) &&
                !hit.transform.CompareTag(Managers.BlockTags[0])) return false;

            _block = Managers.FindById(Managers.WGenerator.Voxels, hit.transform.GetComponent<Block>().Id)
                             .transform;
            var namedBlock = _nameOfObj.GetComponent<Text>();
            namedBlock.text = _block.GetComponent<Block>().FullName;
            return true;
        }


        private void PhantomControl(RaycastHit hit) {
            if (!hit.collider.CompareTag(Managers.BlockTags[0]) && !hit.collider.CompareTag(Managers.BlockTags[1]) &&
                (Managers.Player.transform.position - hit.transform.position).sqrMagnitude > MaxDistance ||
                (Managers.Player.transform.position - hit.transform.position).sqrMagnitude < MinDistance)
                _highLight.SetActive(false);

            _highLight.GetComponent<MeshFilter>().mesh = hit.transform.GetComponent<MeshFilter>().mesh;
            _highLight.transform.position = hit.transform.position;
            _highLight.SetActive(true);
        }
    }
}