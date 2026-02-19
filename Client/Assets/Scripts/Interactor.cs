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

    private InteractOption _option = null;

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
                    _option = interactable.GetOption(character);
                    string text = _option.ActionName + ", ";
                    Debug.Log(text);
                }
                else
                {
                    _option = null;
                }
            }
        }
        else
        {
            _lastHitObject = null;
            _option = null;
            _currentIndex = 0;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (_option != null)
            {
                // 바라보는 대상 상호작용
                Debug.Log(_option.ActionName + " 실행");
                _option.InteractAction?.Invoke(character);
            }
        }

        
        if (Input.GetMouseButtonDown(0))
        {
            character.UseHeldItem(_lastHitObject);
        }
    }
}
