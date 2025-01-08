using System;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private float jumpForce = 3;
    
    [Header("Propriedades de ataque")]
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private Transform attackPosition;
    [SerializeField] private LayerMask attackLayer;

    private float moveDirection;

    private Rigidbody2D Rigidbody2D;
    private Health health;
    private IsGroundedChecker isGroundedCheker;

    private void Awake()
    {        
        Rigidbody2D = GetComponent<Rigidbody2D>();
        isGroundedCheker = GetComponent<IsGroundedChecker>();
        health = GetComponent<Health>();
        // health.OnHurt += PlayHurtAudio;
        health.OnDead += HandlePlayerDeath;
    }

    private void Start()
    {
        GameManager.Instance.InputManager.OnJump += HandleJump;
    }

    private void Update()
    {
        MovePlayer();
        FlipSpriteAccordingToMoveDirection();
    }

    private void MovePlayer()
    {
        moveDirection = GameManager.Instance.InputManager.Movement;
        transform.Translate(moveDirection * Time.deltaTime * moveSpeed, 0, 0);
    }

    private void FlipSpriteAccordingToMoveDirection()
    {
        if (moveDirection < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (moveDirection > 0)
        {
            transform.localScale = Vector3.one;
        }
    }

    private void HandleJump()
    {
        if (isGroundedCheker.IsGrounded() == false) return;
        Rigidbody2D.linearVelocity += Vector2.up * jumpForce;
        // GameManager.Instance.AudioManager.PlaySFX(SFX.PlayerJump);
    }

    // private void PlayHurtAudio()
    // {
    //     GameManager.Instance.AudioManager.PlaySFX(SFX.PlayerHurt);
    // }

    private void HandlePlayerDeath()
    {
        GetComponent<Collider2D>().enabled = false;
        Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
        // GameManager.Instance.AudioManager.PlaySFX(SFX.PlayerDeath);
        GameManager.Instance.InputManager.DisablePlayerInput();
    }

    private void Attack()
    {
        Collider2D[] hittedEnemies = 
            Physics2D.OverlapCircleAll(attackPosition.position, attackRange, attackLayer);
        
        // GameManager.Instance.AudioManager.PlaySFX(SFX.PlayerAttack);
        
        foreach (Collider2D hittedEnemy in hittedEnemies)
        {
            if (hittedEnemy.TryGetComponent(out Health enemyHealth))
            {
                enemyHealth.TakeDamage();
            }
        }
    }

    // private void PlayWalkSound()
    // {
    //     GameManager.Instance.AudioManager.PlaySFX(SFX.PlayerWalk);
    // }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackPosition.position, attackRange);
    }
}