using mc2.general;
using UnityEngine;


namespace mc2.managers
{
    public class WorldControl : GameManager
    {
        private Component _block;
        
        protected internal override void Loading(GameManager manager)
        {
            base.Loading(manager);
            Messenger<GameObject>.AddListener(GameEvents.BlockUpdate, OnBUpdate); 
            Status = ManagerStatus.Started;
        }

        private void OnBUpdate(GameObject obj)
        {
            if (obj.GetComponent<Block>() == null)
                return;
            _block = obj.GetComponent<Block>();
        }

        private void OnDestroy()
        {
            Messenger<GameObject>.RemoveListener(GameEvents.BlockUpdate, OnBUpdate);
        }
    }
}