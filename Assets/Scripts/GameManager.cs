using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    //Checkpoints
    public Checkpoint activeCP { get; private set; }
    public UnityEvent onNewCPActivated = new UnityEvent();

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            FullRespawn();
        }
    }

    public void FullRespawn()
    {
        Debug.Log("Respawn");
        if (activeCP != null)
            activeCP.FullRespawn();
    }

    internal void SetActiveCP(Checkpoint checkpoint)
    {
        activeCP = checkpoint;
        onNewCPActivated.Invoke();
    }
}
