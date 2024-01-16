using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TheKiwiCoder {

    // The context is a shared object every node has access to.
    // Commonly used components and subsytems should be stored here
    // It will be somewhat specfic to your game exactly what to add here.
    // Feel free to extend this class 
    public class Context {
        public GameObject gameObject;
        public Transform transform;
        public Animator animator;
        public NavMeshAgent agent;
        public Transform whaleBody;
        // Add other game specific systems here

        public static Context CreateFromGameObject(GameObject gameObject) {
            // Fetch all commonly used components
            Context context = new Context();
            context.gameObject = gameObject;
            context.transform = gameObject.transform;
            context.agent = gameObject.GetComponent<NavMeshAgent>();
            context.whaleBody = gameObject.GetComponentInChildren<MeshRenderer>().transform;
            context.animator = gameObject.GetComponentInChildren<Animator>();
            
            // Add whatever else you need here...

            return context;
        }
    }
}