using UnityEngine;

namespace Game
{
    public class LegAnimationController : MonoBehaviour
    {
        [SerializeField] bool IsFrogLegs;
        [SerializeField] Animator RightLegAnim;
        [SerializeField] Animator LeftLegAnim;
        [SerializeField] AudioClip[] footSteps;
        bool mouseDown = false;
        bool isMoving = false;

        private Game _game;
        
        private void Awake()
        {
            _game = Game.Instance;
        }
        
        private void Start()
        {
            LeftLegAnim.SetBool("Inverse", true);
        }

        private void Update()
        {
            if (IsFrogLegs) return;

            mouseDown = Input.GetMouseButton(0);
            if (mouseDown == isMoving || _game.State != GameState.InGame) return;
            if (mouseDown)
            {
                SetMove(true);
                isMoving = true;
            } else
            {
                SetMove(false);
                isMoving = false;
            }
        }

        public void SetMove(bool move)
        {
            if(move)
            {
                RightLegAnim.speed = 1;
                LeftLegAnim.speed = 1;
                RightLegAnim.SetBool("Move", move);
                LeftLegAnim.SetBool("Move", move);
            } else
            {
                RightLegAnim.speed = 0;
                LeftLegAnim.speed = 0;
            }
        }

        public void Jump()
        {
            if(!IsFrogLegs) return;
            PlayFootstep(1f);
            LeftLegAnim.SetBool("Inverse", false);
            RightLegAnim.SetTrigger("Jump");
            LeftLegAnim.SetTrigger("Jump");

        }

        public void PlayFootstep(float volume)
        {
            SoundFXManager.Instance.PlayRandomSoundClip(footSteps, transform, volume);
        }
    }
}
