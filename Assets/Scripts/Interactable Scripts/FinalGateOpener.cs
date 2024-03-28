using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalGateOpener : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject finalBoss;
    private GateInteractable interactable;
    void Start()
    {
        interactable = GetComponent<GateInteractable>();
        if (GameManager.Instance.bossDefeated)
        {
            finalBoss.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(finalBoss == null && GameManager.Instance.bossDefeated == false) // Once the boss is gone, open the gate
        {
            GameManager.Instance.bossDefeated = true;
            interactable.ExternalOpen();
        }
    }
}
