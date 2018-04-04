using mc2.general;
using UniRx;
using UnityEngine;


namespace mc2.managers {
    public class WorldControl : GameManager {
        private Block _block;

        protected internal override void Loading(GameManager manager) {
            base.Loading(manager);
            MessageBroker.Default
                         .Receive<Messenger>()
                         .Where(msg => msg.Id == GameEvents.BlockUpdate)
                         .Subscribe(msg => OnBUpdate((GameObject) msg.Data[0]));
            //Managers.WGenerator.World.ForEach(i => i.GetComponent<Collider>().enabled = false);
            Status = ManagerStatus.Started;
        }

        private void OnBUpdate(GameObject obj) {
            if (obj.GetComponent<Block>() == null)
                return;
            
            _block = obj.GetComponent<Block>();
            _block.NearestBlocks = Physics.OverlapBox(_block.transform.position, new Vector3(1.5f, 1.5f, 1.5f));
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