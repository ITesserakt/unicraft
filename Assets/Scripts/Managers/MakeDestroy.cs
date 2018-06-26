using System;
using mc2.general;
using mc2.mod;
using mc2.ui;
using mc2.utils;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace mc2.managers {
    [DontLoadOnStatup]
    public class MakeDestroy : GameManager {
        
        public const float MaxDistance = 6f, MinDistance = 1.05f;
        private const byte Width = WorldGenerator.WidthForChunk;

        private Transform _activeBlock;
        private Camera _camera;
        private GameObject _highLight;
        private GameObject _nameOfObj;

        private MakeDestroy() { }

        protected internal override void Loading() {
            base.Loading();

            _nameOfObj = LoadAndCheckForNull("NameOfObj");
            _highLight = LoadAndCheckForNull("HighLight");
            _camera = Camera.main;
            _activeBlock = GameRegistry.RegisteredBlocks["Dirt"].transform;

            MouseCommands();

            Status = ManagerStatus.Started;
        }

        private void MouseCommands() {

            var updateStream =
                this.UpdateAsObservable()
                    .Select(_ => {
                        RaycastHit hit;
                        var ray = _camera.ScreenPointToRay(Input.mousePosition);
                        var success = Physics.Raycast(ray, out hit, MaxDistance);
                        return new Tuple<RaycastHit, bool>(hit, success);
                    })
                    .Where(tuple => tuple.Item2 && Block.IsBlock(tuple.Item1) && !PauseScreen.IsPause)
                    .Select(tuple => tuple.Item1);

            updateStream.Subscribe(PhantomControl, Debug.LogException);

            updateStream.Where(_ => Input.GetMouseButtonUp(0))
                       .Subscribe(hit =>
                                      MessageBroker.Default.Publish(
                                          Messenger.Create(this, GameEvents.LeftCl, hit)));
            updateStream.Where(_ => Input.GetMouseButtonUp(1))
                       .Subscribe(hit =>
                                      MessageBroker.Default.Publish(
                                          Messenger.Create(
                                              this, GameEvents.RightCl, _activeBlock, hit)));
            updateStream.Where(_ => Input.GetMouseButtonUp(2))
                       .Subscribe(hit =>
                                      MessageBroker.Default.Publish(
                                          Messenger.Create(this, GameEvents.MidCl, hit)));
        }

        protected internal override void OnUpdate() { }

        public void RightClick(Transform arg1, RaycastHit arg2) {
            #region vars

            var pos = arg2.transform.position;
            var x = Mathf.FloorToInt(pos.x / Width);
            var z = Mathf.FloorToInt(pos.z / Width);
            var chTransform = WorldGenerator.GetChunk(x, z)?.transform;
            var generator = Managers.GetManager<WorldGenerator>();

            #endregion

            pos += arg2.normal;
            generator.PutBlock(arg1.gameObject, pos, chTransform);

            MessageBroker.Default.Publish(
                Messenger.Create(this, GameEvents.BlockUpdate, arg2.transform.gameObject));
        }

        public void LeftClick(RaycastHit hit) {
            var block = (Block) hit.transform.gameObject;
            if (block.IsHarvest)
                Destroy(block.gameObject);

            MessageBroker.Default.Publish(
                Messenger.Create(this, GameEvents.BlockUpdate, block.gameObject));
        }

        public void MiddleClick(RaycastHit hit) {
            var block = (Block) hit.transform.gameObject;

            var blockName = block.FullName;
            var namedBlock = _nameOfObj.GetComponent<Text>();

            _activeBlock = GameRegistry.RegisteredBlocks[blockName].transform;
            namedBlock.text = blockName;
        }


        private void PhantomControl(RaycastHit hit) {
            var hitTransf = hit.transform;
            var highLTransf = _highLight.transform;

            if ((Data.Player.transform.position - hitTransf.position).sqrMagnitude > MaxDistance ||
                (Data.Player.transform.position - hitTransf.position).sqrMagnitude < MinDistance)
                _highLight.SetActive(false);

            _highLight.GetComponent<MeshFilter>().mesh = hitTransf.GetComponent<MeshFilter>()?.mesh;

            highLTransf.position = hitTransf.position;
            highLTransf.localScale = hitTransf.localScale + new Vector3(.01f, .01f, .01f);
            _highLight.SetActive(true);
        }
    }
}