using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private ThirdPersonMovement playerController;

    void Update()
    {
        animator.SetBool("IsMoving", playerController.IsMoving);
    }
}
