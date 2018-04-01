using mc2.general;
using UnityEngine;

namespace mc2.managers {
	class WorldControl : GameManager{

		Component _block;

		protected internal override void Loading(GameManager manager) {
			base.Loading(this);
			Messenger<GameObject>.AddListener(GameEvents.BlockUpdate, OnBUpdate);
			Status = ManagerStatus.Started;
		}

		private void OnDestroy() {
			Messenger<GameObject>.RemoveListener(GameEvents.BlockUpdate, OnBUpdate);
		}

		private void OnBUpdate(GameObject obj) {
			_block = obj.GetComponent<Block>();

		}
	}
}