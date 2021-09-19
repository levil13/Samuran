using Cinemachine;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class PlayerManager : MonoBehaviour {
    public delegate void PlayerCreatedCallback(BoxCollider2D playerCollider);

    public static event PlayerCreatedCallback OnPlayerCreated;

    public GameObject playerPrefab;
    
    private CinemachineVirtualCamera _playerCamera;

    private void Start() {
        OutOfViewManager.OnObjectOutOfView += OnObjectOutOfView;

        //TODO Remove -5 hardcode to Vector. It is now used because of bad prefab position
        GameObject player = Instantiate(playerPrefab, new Vector3(0, -5, 0), playerPrefab.transform.rotation);
        
        _playerCamera = FindObjectOfType<CinemachineVirtualCamera>();
        _playerCamera.Follow = player.transform;
        if (OnPlayerCreated != null) OnPlayerCreated(player.GetComponent<BoxCollider2D>());
    }
    private void OnObjectOutOfView(GameObject outOfViewObject) {
        if (outOfViewObject.CompareTag("Player")) {
            print("Player dead");
            _playerCamera.VirtualCameraGameObject.SetActive(false);
            Destroy(outOfViewObject);
        }
    }
}