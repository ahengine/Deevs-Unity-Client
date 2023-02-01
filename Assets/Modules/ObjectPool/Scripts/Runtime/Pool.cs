﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Patterns
{
    [System.Serializable]
    public class Pool<T> where T : Component
    {
        [SerializeField] Transform _parent;
        [SerializeField] T _prefab;
        private List<T> _items = new List<T>();
        public event Action<T> CreateFunc;

        public T[] ActiveItems
        {
            get
            {
                List<T> _itemesActive = new List<T>();

                for (int i = 0; i < _items.Count; i++)
                    if (_items[i].gameObject.activeSelf)
                        _itemesActive.Add(_items[i]);

                return _itemesActive.ToArray();
            }
        }

        public T Get
        {
            get
            {
                for (int i = 0; i < _items.Count; i++)
                    if (!_items[i].gameObject.activeSelf)
                        return _items[i];

                var item = CreateNewItem();

                return item;
            }
        }

        public T CreateNewItem()
        {
            var item = GameObject.Instantiate(_prefab, _parent);
            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.identity;
            item.name = typeof(T) + "_" + _items.Count;
            _items.Add(item);
            CreateFunc?.Invoke(item);
            return item;
        }

        public T GetActive { get { var t = Get; t.gameObject.SetActive(true); return t; } }
    }
}