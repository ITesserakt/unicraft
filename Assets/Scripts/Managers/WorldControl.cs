using mc2.general;
using UnityEngine;


namespace mc2.managers {
    public class WorldControl : GameManager {
        private Block _block;

        protected internal override void Loading(GameManager manager) {
            base.Loading(manager);
            Messenger<GameObject>.AddListener(GameEvents.BlockUpdate, OnBUpdate);
            //Managers.WGenerator.World.ForEach(i => i.GetComponent<Collider>().enabled = false);
            Status = ManagerStatus.Started;
        }

        private void OnBUpdate(GameObject obj) {
            if (obj.GetComponent<Block>() == null)
                return;
            
            _block = obj.GetComponent<Block>();
            _block.NearestBlocks = Physics.OverlapBox(_block.transform.position, new Vector3(1.5f, 1.5f, 1.5f));
        }

        private void OnDestroy() {
            Messenger<GameObject>.RemoveListener(GameEvents.BlockUpdate, OnBUpdate);
        }

        /*
        private void OnTriggerEnter(Collider other) {
            if (other.GetComponent<Block>() != null) {
                other.GetComponent<Collider>().enabled = true;
            }
        }

        private void OnTriggerExit(Collider other) {
            if (other.GetComponent<Block>() != null)
                other.GetComponent<Collider>().enabled = false;
        }
        */
    }
}