using Field;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    [SerializeField] private LayerMask interactLayer;

    private Camera _camera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _camera = Camera.main;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            var pos = _camera.ScreenToWorldPoint(Input.mousePosition);
            var forward =  _camera.transform.forward;
            Debug.Log("Interact");
            if (Physics.Raycast(pos, forward, out RaycastHit hit, 10f, interactLayer))
            {
                if (hit.collider.TryGetComponent<IInteractable>(out IInteractable interactable))
                {
                    var options = interactable.GetOptions();
                    string text = "";
                    foreach (var option in options) text += option.ActionName + "\n";
                    Debug.Log(text);
                    if (options.Count > 0)
                    {
                        Debug.Log(options[0].ActionName + " 실행");
                        options[0].InteractAction?.Invoke();
                    }
                }
            }
            
        }
        
    }
}
