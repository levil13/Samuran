using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {
  public delegate void LevelCreatedCallback(BoxCollider2D levelCollider);
  public static event LevelCreatedCallback OnLevelCreated;

  public GameObject spawnPrefab;
  public List<GameObject> levelsPrefabs;

  private int _currentLevelIndex;
  private float _currentOverallHeight;
  private Grid _levelGrid;

  void Start() {
    OutOfViewManager.OnObjectOutOfView += OnObjectOutOfView;

    _levelGrid = new GameObject("Grid").AddComponent<Grid>();
    InitSpawn();
  }

  private void InitSpawn() {
    InitNewLevel(spawnPrefab);
    InitNewLevel();
  }

  void InitNewLevel(GameObject levelPrefab = null) {
    if (levelPrefab == null) {
      levelPrefab = levelsPrefabs[_currentLevelIndex];
    }
    
    Transform parentTransform = _levelGrid.transform;
    GameObject newLevel = Instantiate(levelPrefab, new Vector3(parentTransform.position.x, _currentOverallHeight), parentTransform.rotation, parentTransform);
    BoxCollider2D newLevelCollider = newLevel.GetComponent<BoxCollider2D>();

    float currentLevelHeight = newLevelCollider.size.y;
    _currentOverallHeight += currentLevelHeight;
    ++_currentLevelIndex;

    if (OnLevelCreated != null) OnLevelCreated(newLevelCollider);
  }

  private void OnObjectOutOfView(GameObject outOfViewObject) {
    if (outOfViewObject.CompareTag("Level") && GameObject.FindWithTag("Spawn") == null) {
      InitNewLevel();
      Destroy(outOfViewObject);
    }

    if (outOfViewObject.CompareTag("Spawn")) {
      InitNewLevel();
      Destroy(outOfViewObject);
    }
  }
}