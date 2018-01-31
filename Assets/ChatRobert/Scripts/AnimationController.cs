using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public Animator Animator;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
            SitDown();
        if(Input.GetKeyDown(KeyCode.B))
            SitUp();
        if (Input.GetKeyDown(KeyCode.W))
            Walk();
        if(Input.GetKeyDown(KeyCode.S))
            StopWalk();
        if(Input.GetKeyDown(KeyCode.V))
            Vectory();
    }

    public void SitDown()
    {
        Animator.SetTrigger("SitDown");
        Animator.ResetTrigger("GetUp");
    }

    public void SitUp()
    {
        Animator.SetTrigger("GetUp");
        Animator.ResetTrigger("SitDown");
    }

    public void Walk()
    {
        Animator.SetBool("Walk",true);
    }

    public void StopWalk()
    {
        Animator.SetBool("Walk",false);
    }

    public void Vectory()
    {
        Animator.SetTrigger("Vectory");
    }
}
