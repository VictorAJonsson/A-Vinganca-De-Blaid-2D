using System.Collections;
using UnityEngine;

// [RequireComponent(typeof(Animator), typeof(AudioSource))]
[RequireComponent(typeof(Animator))]

public abstract class BaseEnemy : MonoBehaviour
{
    protected Animator animator;
    // protected AudioSource audioSource;
    protected Health health;

    protected bool canAttack = true;

    private SpriteRenderer spriteRenderer;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        // audioSource = GetComponent<AudioSource>();
        health = GetComponent<Health>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        health.OnHurt += PlayHurtAnim;
        health.OnDead += HandleDeath;
    }

    protected abstract void Update();

    private void PlayHurtAnim() => animator.SetTrigger("hurt");
    
    private void HandleDeath()
    {
        canAttack = false;
        // GetComponent<BoxCollider2D>().enabled = false;
        animator.SetTrigger("dead");
        StartCoroutine(FadeOutAndDestroy(3));
    }

    private IEnumerator FadeOutAndDestroy(float duration)
    {
        float elapsedTime = 0f;
        float blinkDuration = 0.1f; // Tempo inicial entre piscadas.

        while (elapsedTime < duration)
        {
            // Alterna a visibilidade do sprite.
            spriteRenderer.enabled = !spriteRenderer.enabled;

            // Aguarda pelo tempo atual de piscar.
            yield return new WaitForSeconds(blinkDuration);

            // Incrementa o tempo passado.
            elapsedTime += blinkDuration;

            // Aumenta o tempo de piscar gradualmente para fazer as piscadas ficarem mais lentas.
            blinkDuration = Mathf.Lerp(0.1f, 0.3f, elapsedTime / duration);
        }

        // Certifica-se de que o sprite está desativado antes de destruir o objeto.
        spriteRenderer.enabled = false;

        // Destroi o objeto.
        Destroy(this.gameObject);
    }
}