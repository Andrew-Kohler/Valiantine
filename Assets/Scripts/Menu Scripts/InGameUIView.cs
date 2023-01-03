using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUIView : View
{
    public override void Initialize()
    {
        //throw new System.NotImplementedException();
    }

    void Update()
    {
        if (GameManager.Instance.isMenu())
        {
            Debug.Log("We should see the menu now");
            ViewManager.Show<PlayerMenuView>(true);
        }
    }
}
