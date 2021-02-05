using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayGetUpAnimation : MonoBehaviour
{
    [SerializeField] Animator animator;
    static bool getUpPlayed = false;
    private void Start()
    {
        if (!getUpPlayed)
        {
            getUpPlayed = true;
            animator.SetTrigger("Standup");

            GetComponent<PlayerMovement>().ChangeState(PlayerMovement.PlayerState.Locked);
            StartCoroutine(UnlockAfterStandup());
        }
    }

    IEnumerator UnlockAfterStandup()
    {
        yield return null; //wait one frame until animation starts playing
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"));
        GetComponent<PlayerMovement>().ChangeState(PlayerMovement.PlayerState.Waiting);
    }

}
