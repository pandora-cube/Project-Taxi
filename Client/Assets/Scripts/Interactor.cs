using System;
using System.Collections.Generic;
using Field;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private Character character;
    
    private Camera _camera;

    private GameObject _lastHitObject;

    private List<InteractOption> _options = new List<InteractOption>();

    private int _currentIndex = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _camera = Camera.main;
    }

    // Update is called once per frame
    private void Update()
    {
        var pos = _camera.ScreenToWorldPoint(Input.mousePosition);
        var forward =  _camera.transform.forward;
        
        if (Physics.Raycast(pos, forward, out RaycastHit hit, 10f, interactLayer))
        {
            if (_lastHitObject != hit.collider.gameObject)
            {
                _currentIndex = 0;
                _lastHitObject = hit.collider.gameObject;
                if (hit.collider.TryGetComponent<IInteractable>(out IInteractable interactable))
                {
                    _options = interactable.GetOptions(character);
                    string text = "";
                    foreach (var option in _options) text += option.ActionName + ", ";
                    Debug.Log(text);
                }
                else
                {
                    _options.Clear();
                }
            }
        }
        else
        {
            _lastHitObject = null;
            _options.Clear();
            _currentIndex = 0;
        }


        if (_options.Count > 0)
        {
            // 상호작용한 옵션 중 선택하기
            float wheelValue = Input.GetAxis("Mouse ScrollWheel");
            if (wheelValue != 0)
            {
                int delta = 0;
                if (wheelValue > 0) delta = -1;
                else if (wheelValue < 0) delta = 1;
                _currentIndex = Math.Clamp(_currentIndex + delta, 0, _options.Count - 1);
                Debug.Log(_options[_currentIndex].ActionName);
            }
            // 선택한 옵션 실행하기
            if (Input.GetKeyDown(KeyCode.F))
            {
                _currentIndex = Math.Clamp(_currentIndex, 0, _options.Count - 1);
                Debug.Log(_options[_currentIndex].ActionName + " 실행");
                _options[_currentIndex].InteractAction?.Invoke(character);
            }
        }
        
    }
}
