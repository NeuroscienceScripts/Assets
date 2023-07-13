using System.Collections;
using System.Collections.Generic;
using Classes;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class Explosion : MonoBehaviour
{
    public static Explosion Instance { get; private set; }
    [SerializeField] private AudioSource explosionSound;
    public GameObject explosionPrefab;
    public ParticleSystem particle;
    public static bool explosionActivated;

    // Start is called before the first frame update
    void Awake()
    {
        explosionActivated = false;
    }

    // Update is called once per frame
    void Update()
    {
        // If in testing phase
        if (ExperimentController.Instance.phase == 3 & ExperimentController.Instance.stepInPhase >= 3)
        {
            // If a stressful trial & explosion not activated yet 
            if (ExperimentController.Instance.GetTrialInfo().stressTrial && explosionActivated == false)
            {
                // Get player's current position & starting position  
                GridLocation currentNode = NodeExtension.CurrentNode(ExperimentController.Instance.player.transform.position);
                GridLocation startingLocation = ExperimentController.Instance.GetTrialInfo().start;

                // If player starts at A6
                if (startingLocation.x == 6 && startingLocation.y == "A")
                {
                    // If player is at A5
                    if (currentNode.x == 5 && currentNode.y == "A")
                    {
                        // Activate gameObject explosion at A4
                        explosionPrefab.transform.position = new Vector3(0f, 0f, 3f);
                        // Activate particle system in gameObject 
                        particle.Emit(100);
                        if (!explosionSound.isPlaying)
                            explosionSound.Play();
                        // Set explosion to true so doesn't repeat 
                        explosionActivated = true;
                        
                    }
                    // If player is at B7
                    else if (currentNode.x == 7 && currentNode.y == "B")
                    {
                        // Activate explosion at C7
                        explosionPrefab.transform.position = new Vector3(3f, 0f, 1f);
                        particle.Emit(count:100);
                        if (!explosionSound.isPlaying)
                            explosionSound.Play();
                        explosionActivated = true;
                    }
                }

                // // If start at G2 
                // if (startingLocation.x == 2 && startingLocation.y == "G")
                // {
                //     // If player is at G3 
                //     if (currentNode.x == 3 && currentNode.y == "G")
                //     {
                //         // Activate explosion at G3
                //         explosionPrefab.transform.position = new Vector3(-1f, 0f, -3f);
                //         particle.Emit(count:100);
                //         if (!explosionSound.isPlaying)
                //             explosionSound.Play();
                //         explosionActivated = true;
                //     }
                //     // If player is at F1
                //     else if (currentNode.x == 1 && currentNode.y == "F")
                //     {
                //         // Activate explosion at E1
                //         explosionPrefab.transform.position = new Vector3(-3f, 0f, 1f);
                //         particle.Emit(count:100);
                //         if (!explosionSound.isPlaying)
                //             explosionSound.Play();
                //         explosionActivated = true;
                //     }
                //}
                // If start at F1
                if (startingLocation.x == 1 && startingLocation.y == "F")
                {
                    // If player is at G2 
                    if (currentNode.x == 2 && currentNode.y == "G")
                    {
                        // Activate explosion at G3
                        explosionPrefab.transform.position = new Vector3(-1f, 0f, -3f);
                        particle.Emit(count:100);
                        if (!explosionSound.isPlaying)
                            explosionSound.Play();
                        explosionActivated = true;
                    }
                    // If player is at E1
                    else if (currentNode.x == 1 && currentNode.y == "E")
                    {
                        // Activate explosion at E1
                        explosionPrefab.transform.position = new Vector3(-3f, 0f, 1f);
                        particle.Emit(count:100);
                        if (!explosionSound.isPlaying)
                            explosionSound.Play();
                        explosionActivated = true;
                    }
                }
                // // If start at C7
                // if (startingLocation.x == 7 && startingLocation.y == "C")
                // {
                //     // If player is at A5
                //     if (currentNode.x == 5 && currentNode.y == "A")
                //     {
                //         // Activate explosion at A4
                //         explosionPrefab.transform.position = new Vector3(0f, 0f, 3f);
                //         particle.Emit(count:100);
                //         if (!explosionSound.isPlaying)
                //             explosionSound.Play();
                //         explosionActivated = true;
                //     }
                //     // If player is at C5
                //     else if (currentNode.x == 5 && currentNode.y == "C")
                //     {
                //         // Activate explosion at C4
                //         explosionPrefab.transform.position = new Vector3(0f, 0f, 1f);
                //         particle.Emit(count:100);
                //         if (!explosionSound.isPlaying)
                //             explosionSound.Play();
                //         explosionActivated = true;
                //     }
                //     // If player is at E7
                //     else if (currentNode.x == 7 && currentNode.y == "E")
                //     {
                //         // Activate explosion at E6
                //         explosionPrefab.transform.position = new Vector3(2f, 0f, -1f);
                //         particle.Emit(count:100);
                //         if (!explosionSound.isPlaying)
                //             explosionSound.Play();
                //         explosionActivated = true;
                //     }
                // }
                // If start at A1
                if (startingLocation.x == 1 && startingLocation.y == "A")
                {
                    // If player is at A2
                    if (currentNode.x == 2 && currentNode.y == "A")
                    {
                        // Activate explosion at A2
                        explosionPrefab.transform.position = new Vector3(-2f, 0f, 3f);
                        particle.Emit(count:100);
                        if (!explosionSound.isPlaying)
                            explosionSound.Play();
                        explosionActivated = true;
                    }
                }
                // // If start at F6 
                // if (startingLocation.x == 6 && startingLocation.y == "F")
                // {
                //     // If player is at E6
                //     if (currentNode.x == 6 && currentNode.y == "E")
                //     {
                //         // Activate explosion at E6
                //         explosionPrefab.transform.position = new Vector3(2f, 0f, -1f);
                //         particle.Emit(count:100);
                //         if (!explosionSound.isPlaying)
                //             explosionSound.Play();
                //         explosionActivated = true;
                //     }
                //     // If player is at G6
                //     else if (currentNode.x == 6 && currentNode.y == "G")
                //     {
                //         // Activate explosion at G6
                //         explosionPrefab.transform.position = new Vector3(2f, 0f, -3f);
                //         particle.Emit(count:100);
                //         if (!explosionSound.isPlaying)
                //             explosionSound.Play();
                //         explosionActivated = true;
                //     }
                // }
                // If start at E6
                if (startingLocation.x == 6 && startingLocation.y == "E")
                {
                    // If player is at E5
                    if (currentNode.x == 5 && currentNode.y == "E")
                    {
                        // Activate explosion at E4
                        explosionPrefab.transform.position = new Vector3(0f, 0f, -1f);
                        particle.Emit(count:100);
                        if (!explosionSound.isPlaying)
                            explosionSound.Play();
                        explosionActivated = true;
                    }
                    // If player is at D7
                    else if (currentNode.x == 7 && currentNode.y == "D")
                    {
                        // Activate explosion at C7
                        explosionPrefab.transform.position = new Vector3(3f, 0f, 1f);
                        particle.Emit(count:100);
                        if (!explosionSound.isPlaying)
                            explosionSound.Play();
                        explosionActivated = true;
                    }
                    // If player is at G5
                    else if (currentNode.x == 5 && currentNode.y == "G")
                    {
                        // Activate explosion at G6
                        explosionPrefab.transform.position = new Vector3(2f, 0f, -3f);
                        particle.Emit(count:100);
                        if (!explosionSound.isPlaying)
                            explosionSound.Play();
                        explosionActivated = true;
                    }
                }
                // // If start at D1
                // if (startingLocation.x == 1 && startingLocation.y == "D")
                // {
                //     // If player is at C1
                //     if (currentNode.x == 1 && currentNode.y == "C")
                //     {
                //         // Activate explosion at C2
                //         explosionPrefab.transform.position = new Vector3(-2f, 0f, 1f);
                //         particle.Emit(count:100);
                //         if (!explosionSound.isPlaying)
                //             explosionSound.Play();
                //         explosionActivated = true;
                //     }
                //     // If player is at E1
                //     else if (currentNode.x == 1 && currentNode.y == "E")
                //     {
                //         // Activate explosion at E1
                //         explosionPrefab.transform.position = new Vector3(-3f, 0f, -1f);
                //         particle.Emit(count:100);
                //         if (!explosionSound.isPlaying)
                //             explosionSound.Play();
                //         explosionActivated = true;
                //     }
                // }
                // If start at B2
                if (startingLocation.x == 2 && startingLocation.y == "B")
                {
                    // If player is at A2
                    if (currentNode.x == 2 && currentNode.y == "A")
                    {
                        // Activate explosion at A2
                        explosionPrefab.transform.position = new Vector3(-2f, 0f, 3f);
                        particle.Emit(count:100);
                        if (!explosionSound.isPlaying)
                            explosionSound.Play();
                        explosionActivated = true;
                    }
                    // If player is at C2
                    else if (currentNode.x == 2 && currentNode.y == "C")
                    {
                        // Activate explosion at C2
                        explosionPrefab.transform.position = new Vector3(-2f, 0f, 1f);
                        particle.Emit(count:100);
                        if (!explosionSound.isPlaying)
                            explosionSound.Play();
                        explosionActivated = true;
                    }
                }
                // // If start at D4
                // if (startingLocation.x == 4 && startingLocation.y == "D")
                // {
                //     // If player is at C4
                //     if (currentNode.x == 4 && currentNode.y == "C")
                //     {
                //         // Activate explosion at C4
                //         explosionPrefab.transform.position = new Vector3(0f, 0f, 1f);
                //         particle.Emit(count:100);
                //         if (!explosionSound.isPlaying)
                //             explosionSound.Play();
                //         explosionActivated = true;
                //     }
                //     // If player is at E4
                //     else if (currentNode.x == 4 && currentNode.y == "E")
                //     {
                //         // Activate explosion at E4
                //         explosionPrefab.transform.position = new Vector3(0f, 0f, -1f);
                //         particle.Emit(count:100);
                //         if (!explosionSound.isPlaying)
                //             explosionSound.Play();
                //         explosionActivated = true;
                //     }
                // }
                // If start at G5
                if (startingLocation.x == 5 && startingLocation.y == "G")
                {
                    // If player is at G4
                    if (currentNode.x == 4 && currentNode.y == "G")
                    {
                        // Activate explosion at G4
                        explosionPrefab.transform.position = new Vector3(0f, 0f, -3f);
                        particle.Emit(count:100);
                        if (!explosionSound.isPlaying)
                            explosionSound.Play();
                        explosionActivated = true;
                    }
                    // If player is at G6
                    else if (currentNode.x == 6 && currentNode.y == "G")
                    {
                        // Activate explosion at G6
                        explosionPrefab.transform.position = new Vector3(2f, 0f, -3f);
                        particle.Emit(count:100);
                        if (!explosionSound.isPlaying)
                            explosionSound.Play();
                        explosionActivated = true;
                    }
                }
            }
        }
    }
}     