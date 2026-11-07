using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    AnimatorOverrideController animatorOverrideController;
    [SerializeField] private AnimationClip idleAnimation;
    [SerializeField] private AnimationClip attackAnimation;
    [SerializeField] private AnimationClip hitAnimation;
    [SerializeField] private AnimationClip dieAnimation;
    [SerializeField] private string idleName = "Idle";
    [SerializeField] private string attackName = "Attack";
    [SerializeField] private string hitName = "Hit";
    [SerializeField] private string dieName = "Die";
    private int idleHash;
    private int attackHash;
    private int hitHash;
    private int dieHash;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        // Convert trigger names to hashes for better performance
        idleHash = Animator.StringToHash(idleName);
        attackHash = Animator.StringToHash(attackName);
        hitHash = Animator.StringToHash(hitName);
        dieHash = Animator.StringToHash(dieName);

        // Assign the animation clips to the animator controller
        animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = animatorOverrideController;
        animatorOverrideController[idleName] = idleAnimation;
        animatorOverrideController[attackName] = attackAnimation;
        animatorOverrideController[hitName] = hitAnimation;
        animatorOverrideController[dieName] = dieAnimation;
    }

    public void TriggerAttack()
    {
        animator.SetTrigger(attackHash);
    }

    public void TriggerHit()
    {
        animator.SetTrigger(hitHash);
    }

    public void TriggerDie()
    {
        animator.SetTrigger(dieHash);
    }
}
