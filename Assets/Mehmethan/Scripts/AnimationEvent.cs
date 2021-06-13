using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
    public GameObject glass;
    public GameObject handPos;
    public GameObject glassFirstPos;

    public void FinishActions()
   {
       StartCoroutine(GameManager.instance.FinishMethod());
   }

   public void GlassTransfer()
   {
       glass.transform.position = handPos.transform.position;
       glass.transform.SetParent(handPos.transform);
   }

   public void GlassBackTransfer()
   {
       glass.transform.SetParent(null);
       glass.transform.position = glassFirstPos.transform.position;
       glass.transform.rotation = Quaternion.Euler(0,0,0);
   }
}
