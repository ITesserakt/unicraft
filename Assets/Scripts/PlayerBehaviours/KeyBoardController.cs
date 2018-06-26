using System;
using mc2.ui;
using mc2.utils;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace mc2.player {
	[RequireComponent(typeof(CharacterController))]
	internal sealed class KeyBoardController : MonoBehaviour {
		[SerializeField] private float _speed;

		private IObservable<Unit> UpdateStream { get; set; }
		private bool _isBoost;

		private void Start() {

			UpdateStream = this.UpdateAsObservable().Where(_ => !PauseScreen.IsPause).TakeUntilDestroy(this);

			new MoveCommand(Vector3.forward, KeyCode.W, _speed * 1.1f).Execute(UpdateStream);
			new MoveCommand(Vector3.left, KeyCode.A, _speed).Execute(UpdateStream);
			new MoveCommand(Vector3.back, KeyCode.S, _speed).Execute(UpdateStream);
			new MoveCommand(Vector3.right, KeyCode.D, _speed).Execute(UpdateStream);
			new MoveCommand(Vector3.down, KeyCode.LeftShift, _speed).Execute(UpdateStream);
			new MoveCommand(Vector3.up, KeyCode.Space, _speed).Execute(UpdateStream);
		}

		private class MoveCommand : IObservableCommand<Unit> {
			private Vector3 Way { get; }
			private KeyCode Executor { get; }
			private float Speed { get; }
			private Transform Player { get; }
			private CharacterController CharacterController { get; }

			public MoveCommand(Vector3 way, KeyCode executor, float speed) {
				Way = way;
				Executor = executor;
				Speed = speed;
				Player = Data.Player.transform;
				CharacterController = Player.GetComponent<CharacterController>();
			}

			public void Execute(IObservable<Unit> obs) {
				obs.Where(_ => Input.GetKey(Executor))
				   .Subscribe(_ => {
					   var movement = Way * Speed;
					   movement = Vector3.ClampMagnitude(movement, Speed);
					   movement *= Time.deltaTime;
					   movement = Player.TransformDirection(movement);
					   CharacterController.Move(movement);
				   }, Debug.LogException);
			}
		}

		[Obsolete]
		private class BoostCommand : IObservableCommand<Unit> {
			private float Speed { get; set; }
			private KeyCode Executor { get; }
			internal float Boost { private get; set; }

			private bool _isBoost;

			public BoostCommand(ref float speed, KeyCode executor) {
				Speed = speed;
				Executor = executor;
			}

			public void Execute(IObservable<Unit> obs) {
				obs.Where(_ => Input.GetKey(Executor))
				   .Subscribe(_ => {
					   if (_isBoost) {
						   Speed *= Boost;
						   _isBoost = !_isBoost;
					   }
					   else {
						   Speed /= Boost / 2;
						   _isBoost = !_isBoost;
					   }
				   }, Debug.LogException);
			}

		}
	}
}