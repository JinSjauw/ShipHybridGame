
using System.Diagnostics;
using Audio;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestAudio : MonoBehaviour
{
    public AudioController audioController;

    public void OnPlaySound(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            switch (context.ReadValue<float>())
            {
                case 1:
                    audioController.PlayAudio(AudioID.ST_MUSIC, true, 1.0f);
                    break;
                case -1:
                    audioController.RestartAudio(AudioID.ST_MUSIC, true, .5f);
                    break;
            }
        }

        if (context.canceled)
        {
            audioController.StopAudio(AudioID.ST_MUSIC, false, 0);
        }
    }
}
