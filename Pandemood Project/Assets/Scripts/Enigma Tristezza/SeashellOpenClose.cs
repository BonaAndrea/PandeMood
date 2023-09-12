using UnityEngine;

namespace Enigma_Tristezza
{
    public class SeashellOpenClose : MonoBehaviour
    {
        public GameObject upperShell;
        public GameObject Perl;
        public AudioSource AudioOpenSeashell;
        public bool open = false;

        private Animator animator;

        private static readonly int Open = Animator.StringToHash("open");

        // Start is called before the first frame update
        void Start()
        {
            animator = upperShell.GetComponent<Animator>();
            animator.SetBool(Open, false);
            if (Perl != null)
                Perl.GetComponent<InteractableObject>().enabled = false;
        }


        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Bubble"))
            {
                if (!open)
                {
                    open = true;
                    animator.SetBool(Open, true);
                    AudioOpenSeashell.Play();
                    if (Perl!=null)
                        Perl.GetComponent<InteractableObject>().enabled = true;
                }
                // else
                // {
                //     open = false;
                //     animator.SetBool(Open, false);
                //     AudioOpenSeashell.Play();
                //     if (Perl != null)
                //         Perl.GetComponent<InteractableObject>().enabled = false;
                // }
            }
        }
    }
}
