using UnityEngine;

public class ParticleController : MonoBehaviour
{
    public ParticleSystem spaceLeftParticleSystem;
    public ParticleSystem aSpaceParticleSystem;
    public ParticleSystem wParticleSystem;

    void Update()
    {
        // Check and handle the space key and left arrow key combination
        HandleKeyPressCombination(KeyCode.Space, KeyCode.A, spaceLeftParticleSystem);

        // Check and handle the A key and space key combination
        HandleKeyPressCombination(KeyCode.D, KeyCode.Space, aSpaceParticleSystem);

        // Check and handle the W key
        if (Input.GetKey(KeyCode.W))
        {
            // Turn on the W particle system
            if (!wParticleSystem.isPlaying)
            {
                wParticleSystem.Play();
            }
        }
        else if (wParticleSystem.isPlaying)
        {
            // Turn off the W particle system if W key is released
            wParticleSystem.Stop();
        }
    }

    void HandleKeyPressCombination(KeyCode firstKey, KeyCode secondKey, ParticleSystem particleSystem)
    {
        // Check if both keys are pressed
        if (Input.GetKey(firstKey) && Input.GetKey(secondKey))
        {
            // Turn on the particle system
            if (!particleSystem.isPlaying)
            {
                particleSystem.Play();
            }
        }
        else
        {
            // Turn off the particle system
            if (particleSystem.isPlaying)
            {
                particleSystem.Stop();
            }
        }
    }
}
