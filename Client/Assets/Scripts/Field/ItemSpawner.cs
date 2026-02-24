using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Field
{
    public class ItemSpawner : MonoBehaviour
    {
        public ItemInstance itemInstancePrefab;
        [SerializeField] private BoxCollider boxCollider;
        [SerializeField] private LayerMask layerMask;
        
        [SerializeField] private int maxItems;
        [SerializeField] private float spawnInterval;
        [SerializeField] private Item itemData;
        
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _itemPool = new ObjectPool<ItemInstance>(CreatePoolObject,ActivatePoolObject,DeactivatePoolObject,DestroyPoolObject,defaultCapacity: maxItems,maxSize: maxItems);
            
            List<ItemInstance> pool = new List<ItemInstance>();
            for (int i = 0; i < maxItems; i++)
            {
                var obj = _itemPool.Get();
                pool.Add(obj);
            }

            foreach (var obj in pool)
            {
                _itemPool.Release(obj);
            }
            
            StartCoroutine(SpawnRoutine());
        }

        IEnumerator SpawnRoutine()
        {
            while (true)
            {
                TrySpawnObject();
                
                yield return new WaitForSeconds(spawnInterval);
            }
        }

        void TrySpawnObject()
        {
            var obj = GetObject();
            if (obj == null) return;
            
            Bounds bounds = boxCollider.bounds;
            float randomX =  Random.Range(bounds.min.x, bounds.max.x);
            float randomZ =  Random.Range(bounds.min.z, bounds.max.z);
            Vector3 rayStart = new  Vector3(randomX, bounds.max.y, randomZ);
            if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, bounds.size.y + 1f, layerMask))
            {
                obj.transform.position = hit.point;
            }
        }

        #region ObjectPool

        IObjectPool<ItemInstance> _itemPool;
        
        private ItemInstance CreatePoolObject() => Instantiate(itemInstancePrefab);
        private void ActivatePoolObject(ItemInstance obj)
        {
            obj.gameObject.SetActive(true);
            obj.Init(this,itemData);
        }

        private void DeactivatePoolObject(ItemInstance obj) => obj.gameObject.SetActive(false);
        private void DestroyPoolObject(ItemInstance obj) => Destroy(obj.gameObject);

        /// <summary>
        /// 아이템 오브젝트 스폰 함수
        /// </summary>
        /// <returns></returns>
        public ItemInstance GetObject()
        {
            if (_itemPool.CountInactive == 0)
            {
                return null;
            }
            else
            {
                return _itemPool.Get();
            }
        }
        
        /// <summary>
        /// 아이템 오브젝트 반환 함수
        /// </summary>
        /// <param name="obj"></param>
        public void ReleaseObject(ItemInstance obj)
        {
            if (!obj) return;
            _itemPool.Release(obj);
        }

        #endregion

        
    }
}
