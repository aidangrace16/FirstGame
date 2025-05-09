using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MonsterNavChase : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Camera playerCamera;
    public Camera jumpscareCamera;
    public NavMeshAgent agent;

    [Header("Chase Settings")]
    public float detectionRange = 10f;
    public float attackRange    = 2f;
    public LayerMask obstacleMask;
    
    [Header("Look-at Control")]
    public bool onlyChaseWhenNotLookedAt = false;
    [Range(-1f,1f)]
    public float lookDotThreshold = 0.5f; 
    // dot > threshold means player is looking roughly at monster

    [Header("Jumpscare")]
    public float jumpscareDuration = 2f;
    public AudioSource jumpscareAudio;
    public Light jumpscareLight;

    private Animator animator;
    private bool    isAttacking = false;

    void Start()
    {
        agent     = GetComponent<NavMeshAgent>();
        animator  = GetComponent<Animator>();
        if (jumpscareCamera != null)  jumpscareCamera.gameObject.SetActive(false);
        if (jumpscareAudio  == null)  jumpscareAudio = GetComponent<AudioSource>();
        if (jumpscareLight  != null)  jumpscareLight.intensity = 0;
    }

    void Update()
    {
        if (isAttacking) return;

        float dist = Vector3.Distance(transform.position, player.position);

        bool canSee = (dist <= detectionRange) && CanSeePlayer();
        // if we’ve turned on the special “only chase when unseen” mode:
        if (onlyChaseWhenNotLookedAt && IsPlayerLookingAtMonster())
            canSee = false;

        if (canSee)
        {
            if (dist <= attackRange)
            {
                StartCoroutine(StartAttack());
            }
            else
            {
                agent.SetDestination(player.position);
                animator.SetBool("isWalking", true);
            }
        }
        else
        {
            agent.SetDestination(transform.position);
            animator.SetBool("isWalking", false);
        }
    }

    bool IsPlayerLookingAtMonster()
    {
        Vector3 toMonster = (transform.position - playerCamera.transform.position).normalized;
        float dot = Vector3.Dot(playerCamera.transform.forward, toMonster);
        return dot > lookDotThreshold;
    }

    IEnumerator StartAttack()
    {
        isAttacking      = true;
        agent.isStopped  = true;
        animator.SetBool("isWalking", false);
        animator.SetTrigger("attack");

        if (playerCamera != null && jumpscareCamera != null)
        {
            playerCamera.gameObject.SetActive(false);
            jumpscareCamera.gameObject.SetActive(true);
        }

        jumpscareAudio?.Play();
        if (jumpscareLight != null) jumpscareLight.intensity = 2;

        yield return new WaitForSeconds(jumpscareDuration);
        FindObjectOfType<MonsterJumpscare>().TriggerFadeToEnding(1.5f);
    }

    bool CanSeePlayer()
    {
        Vector3 dir    = (player.position - transform.position).normalized;
        float   dist   = Vector3.Distance(transform.position, player.position);
        if (Physics.Raycast(transform.position, dir, out RaycastHit hit, dist, obstacleMask))
            return false;
        return true;
    }
}
