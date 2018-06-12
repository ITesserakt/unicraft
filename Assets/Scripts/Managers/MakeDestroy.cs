using mc2.general;
using mc2.mod;
using mc2.ui;
using mc2.utils;
using UnityEngine;
using UnityEngine.UI;

namespace mc2.managers
{
    [DontLoadOnStatup]
    public class MakeDestroy : GameManager
    {
        public const float MaxDistance = 6f, MinDistance = 1.05f;
        private const byte Width = WorldGenerator.WidthForChunk;

        private Transform _activeBlock;
        private Camera _camera;
        private GameObject _highLight;
        private GameObject _nameOfObj;

        private MakeDestroy() { }

        protected internal override void Loading()
        {
            base.Loading();

            _nameOfObj = LoadAndCheckForNull("NameOfObj");
            _highLight = LoadAndCheckForNull("HighLight");
            _camera = Camera.main;

            Status = ManagerStatus.Started;
        }

        protected internal override void Update_()
        {

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

            _activeBlock = GameRegistry.RegisteredBlocks["Dirt"].transform;

            PhantomControl(hit);

            if (Input.GetMouseButtonUp(0)) 
                Messenger.PublishEvent(this, GameEvents.LeftCl, hit);

            else if (Input.GetMouseButtonUp(1))
                Messenger.PublishEvent(this, GameEvents.RightCl, hit, _activeBlock);

            else if (Input.GetMouseButtonUp(2))
                Messenger.PublishEvent(this, GameEvents.MidCl, hit);
        }

        public void RightClick(Transform arg1, RaycastHit arg2)
        {
            #region vars

            var pos = arg2.transform.position;
            var x = Mathf.FloorToInt(pos.x / Width);
            var z = Mathf.FloorToInt(pos.z / Width);
            var chTransform = WorldGenerator.GetChunk(x, z)?.transform;
            var generator = Managers.GetManager<WorldGenerator>();

            #endregion

            pos += arg2.normal;
            if (chTransform == null)
                generator.MakeChunk(x, z);

            var clone = generator.PutBlock(arg1.gameObject, pos, chTransform);

            Messenger.PublishEvent(this, GameEvents.BlockUpdate, clone);
        }

        public void LeftClick(RaycastHit hit)
        {
            var block = Block.Get(hit);
            if (block && block.IsHarvest)
                Destroy(hit.collider.gameObject);

            Messenger.PublishEvent(this, GameEvents.BlockUpdate, hit.transform.gameObject);
        }

        public void MiddleClick(RaycastHit hit)
        {
            var block = Block.Get(hit);
            if (!block)
                return;

            var blockName = block.FullName;
            var namedBlock = _nameOfObj.GetComponent<Text>();

            _activeBlock = GameRegistry.RegisteredBlocks[blockName].transform;
            namedBlock.text = blockName;
        }


        private void PhantomControl(RaycastHit hit)
        {
            var hitTransf = hit.transform;
            var highLTransf = _highLight.transform;

            if ((Data.Player.transform.position - hitTransf.position).sqrMagnitude > MaxDistance ||
                (Data.Player.transform.position - hitTransf.position).sqrMagnitude < MinDistance ||
                !Block.Get(hit))
                _highLight.SetActive(false);

            _highLight.GetComponent<MeshFilter>().mesh = hitTransf.GetComponent<MeshFilter>()?.mesh;

            highLTransf.position = hitTransf.position;
            highLTransf.localScale = hitTransf.localScale + new Vector3(.01f, .01f, .01f);
            _highLight.SetActive(true);
        }
    }
}