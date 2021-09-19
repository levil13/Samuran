using System.Collections.Generic;
using UnityEngine;

public class OutOfViewManager : MonoBehaviour {
  private readonly List<BoxCollider2D> _objectsInView = new List<BoxCollider2D>(); 
  private readonly List<BoxCollider2D> _objectsOutOfView = new List<BoxCollider2D>();
  private Camera _camera;
  private Plane[] _cameraPlanes;

  public delegate void ObjectsOutOfViewCallback(GameObject gameObject);
  public static event ObjectsOutOfViewCallback OnObjectOutOfView;

  private void Awake() {
    LevelManager.OnLevelCreated += OnObjectCreated;
    PlayerManager.OnPlayerCreated += OnObjectCreated;
  }

  void Start() {
    _camera = Camera.main;
  }

  void Update() {
    _cameraPlanes = GeometryUtility.CalculateFrustumPlanes(_camera);

    if (_objectsInView.Count != 0) {
      DetectObjectsOutOfView();
    }

    if (_objectsOutOfView.Count != 0) {
      DetectObjectsInView();
    }
  }

  void OnObjectCreated(BoxCollider2D objectCollider) {
    _objectsOutOfView.Add(objectCollider);
  }

  void DetectObjectsOutOfView() {
    foreach (BoxCollider2D objectCollider in _objectsInView) {
      if (!GeometryUtility.TestPlanesAABB(_cameraPlanes, objectCollider.bounds)) {
        if (OnObjectOutOfView != null) OnObjectOutOfView(objectCollider.gameObject);
        _objectsInView.Remove(objectCollider);
      }
    }
  }

  void DetectObjectsInView() {
    foreach (BoxCollider2D objectCollider in _objectsOutOfView) {
      if (GeometryUtility.TestPlanesAABB(_cameraPlanes, objectCollider.bounds)) {
        _objectsInView.Add(objectCollider);
        _objectsOutOfView.Remove(objectCollider);
      }
    }
  }
}