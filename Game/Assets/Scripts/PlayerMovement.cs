using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
  enum JumpingSide {
    Left,
    Right
  };

  private Boolean _isPlayerFrozen;
  private Boolean _isJumping;
  private Boolean _isDead;
  private JumpingSide _jumpingSide;
  private Rigidbody2D _rb2D;
  private IEnumerator _slideDownCoroutine;
  private ParticleSystem _impactParticleSystem;

  public float jumpForceUp;
  public float jumpForceSide;
  public float fallForce;
  public float slideDownDelay;

  //public Animator animator;

  void Awake() {
    _rb2D = GetComponent<Rigidbody2D>();
    _impactParticleSystem = FindObjectOfType<ParticleSystem>();
  }

  void Update() {
    DetectAndHandleJump();
  }

  void DetectAndHandleJump() {
    if (_isJumping) {
      return;
    }

    if (Input.GetButtonDown("Jump") || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)) {
      SetIsJumping(true);
      if (_isPlayerFrozen) {
        TogglePlayerFreeze(false);
      }
      if (_slideDownCoroutine != null) {
        StopCoroutine(_slideDownCoroutine);
      }
      Jump();
    }
  }

  void SetIsJumping(bool isJumping) {
    _isJumping = isJumping;
  //  animator.SetBool("IsJumping", _isJumping);
  }

  void OnCollisionEnter2D(Collision2D col) {
    SetIsJumping(false);
    if (col.gameObject.CompareTag("Walls")) {
      // EmitImpactParticles();
      UpdateJumpingSide(col.collider);
      TogglePlayerFreeze(true);

      _slideDownCoroutine = SlideDownAfterTime(slideDownDelay);
      StartCoroutine(_slideDownCoroutine);
    }
  }

  void EmitImpactParticles() {
    ParticleSystem.ShapeModule particleShape = _impactParticleSystem.shape;
    if (_jumpingSide == JumpingSide.Left) {
      particleShape.rotation = new Vector3(0, 90, 0);
    } else if (_jumpingSide == JumpingSide.Right) {
      particleShape.rotation = new Vector3(0, -90, 0);
    }
    _impactParticleSystem.Play();
  }

  void TogglePlayerFreeze(Boolean isFrozen) {
    _isPlayerFrozen = isFrozen;
    if (_isPlayerFrozen) {
      _rb2D.constraints = RigidbodyConstraints2D.FreezePosition;
    } else {
      _rb2D.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
  }

  private IEnumerator SlideDownAfterTime(float time) {
    yield return new WaitForSeconds(time);
    TogglePlayerFreeze(false);
    _rb2D.AddForce(transform.up * -1 * fallForce, ForceMode2D.Impulse);
  }

  private void UpdateJumpingSide(Collider2D wallCollider) {
    if (transform.position.x - wallCollider.transform.position.x < 0) {
      _jumpingSide = JumpingSide.Left;
    } else if (transform.position.x - wallCollider.transform.position.x > 0) {
      _jumpingSide = JumpingSide.Right;
    }
  }

  private void Jump() {
    // _impactParticleSystem.Stop();
    float sideForce = jumpForceSide;
    if (_jumpingSide == JumpingSide.Right) {
      sideForce *= -1;
    }
    _rb2D.velocity = Vector3.zero;
    _rb2D.AddForce(new Vector2(sideForce, jumpForceUp), ForceMode2D.Impulse);
  }
}