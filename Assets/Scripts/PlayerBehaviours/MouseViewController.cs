using System;
using mc2.ui;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace mc2.player {
    internal sealed class MouseViewController : MonoBehaviour {
        private enum RotAxes {
            Both = 0,
            OnlyX = 1,
            OnlyY = 2
        }

        [SerializeField] private RotAxes _axes = RotAxes.Both;
        [SerializeField] private float _sensitivity = 5;
        [SerializeField] private float _minY = -70;
        [SerializeField] private float _maxY = 80;

        private Quaternion _cameraRot;
        private Transform _camera;
        private Transform _player;

        private void Start() {
            _camera = transform;
            _cameraRot = _camera.localRotation;

            _player = _camera.parent;

            var updateStream = this.UpdateAsObservable().Where(_ => !PauseScreen.IsPause).TakeUntilDestroy(this);

            MouseLook(updateStream);
        }

        private void MouseLook(IObservable<Unit> upd) {
            var axeX = upd.Select(_ => Input.GetAxis("Mouse X") * _sensitivity);
            var axeY = upd.Select(_ => Input.GetAxis("Mouse Y") * _sensitivity);

            axeX.Where(_ => _axes == RotAxes.OnlyX)
                .Subscribe(delta => _camera.Rotate(0, delta, 0));

            axeY.Where(_ => _axes == RotAxes.OnlyY)
                .Subscribe(delta => {
                    _cameraRot *= Quaternion.Euler(-delta, 0, 0);
                    _cameraRot = ClampRotation(_cameraRot, _minY, _maxY);
                    _camera.localRotation = _cameraRot;
                });

            upd.Where(_ => _axes == RotAxes.Both)
               .Select(_ => new Vector2(Input.GetAxis("Mouse Y") * _sensitivity,
                                        Input.GetAxis("Mouse X") * _sensitivity))
               .Subscribe(vector => {
                   _cameraRot *= Quaternion.Euler(-vector.x, 0, 0);
                   _cameraRot = ClampRotation(_cameraRot, _minY, _maxY);
                   _camera.localRotation = _cameraRot;

                   _player.localRotation *= Quaternion.Euler(0, vector.y, 0);
               });
        }

        private static Quaternion ClampRotation(Quaternion q, float min, float max) {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            var angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

            angleX = Mathf.Clamp(angleX, min, max);

            q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }
    }
}