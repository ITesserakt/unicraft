using System.Threading.Tasks;
using mc2.general;
using mc2.mod;
using mc2.utils;
using UniRx;
using UnityEngine;


namespace mc2.managers {
    public class WorldControl : GameManager {
        private Block _block;
        
        private WorldControl() {}

        protected internal override void Loading() {
            base.Loading();
            Messenger.SubscribeOnEvent<Messenger>(msg => {
                if (msg.Id == GameEvents.BlockUpdate)
                    OnBUpdate(msg.Data[0] as GameObject);
            });
            Status = ManagerStatus.Started;
        }

        protected internal override void Update_() {}

        private void OnBUpdate(GameObject obj) {
//            _block.NearestBlocks = Physics.OverlapBox(_block.transform.position, new Vector3(1.5f, 1.5f, 1.5f));
        }

        /*internal static void Combine(GameObject combineTo) {
            MeshFilter[] meshFilters = combineTo.GetComponentsInChildren<MeshFilter>();
            CombineInstance[] instances = new CombineInstance[meshFilters.Length];
            var i = 0;
            while (i < meshFilters.Length) {
                instances[i].mesh = meshFilters[i].sharedMesh;
                instances[i].transform = meshFilters[i].transform.localToWorldMatrix;
                meshFilters[i] = null;
                i++;
            }
            
            if (combineTo.transform.GetComponent<MeshFilter>() != null)
                combineTo.transform.GetComponent<MeshFilter>().mesh = new Mesh();
            else 
                combineTo.AddComponent<MeshFilter>().mesh = new Mesh();
            combineTo.transform.GetComponent<MeshFilter>().mesh.CombineMeshes(instances);
            combineTo.transform.gameObject.SetActive(true);
        }*/
    }
}