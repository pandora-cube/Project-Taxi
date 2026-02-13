using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Field
{
    public class ItemSpawner : MonoBehaviour
    {
        public Item itemPrefab;
        [SerializeField] private BoxCollider boxCollider;
        [SerializeField] private LayerMask layerMask;
        
        [SerializeField] private int maxItems;
        [SerializeField] private float spawnInterval;

        
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _itemPool = new ObjectPool<Item>(CreatePoolObject,ActivatePoolObject,DeactivatePoolObject,DestroyPoolObject,defaultCapacity: maxItems,maxSize: maxItems);
            
            List<Item> pool = new List<Item>();
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

        IObjectPool<Item> _itemPool;
        
        private Item CreatePoolObject() => Instantiate(itemPrefab);
        private void ActivatePoolObject(Item obj)
        {
            obj.gameObject.SetActive(true);
            obj.Init(this);
        }

        private void DeactivatePoolObject(Item obj) => obj.gameObject.SetActive(false);
        private void DestroyPoolObject(Item obj) => Destroy(obj.gameObject);

        public Item GetObject()
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
        
        public void ReleaseObject(Item obj)
        {
            if (!obj) return;
            _itemPool.Release(obj);
        }

        #endregion

        
    }
}
