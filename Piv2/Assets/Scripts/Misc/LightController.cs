using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
   public Color LightColor1;
   public Color LightColor2;
   private float LightIntensity = 2;
   private Light[] lights;


   private void Awake()
   {
      lights = GetComponentsInChildren<Light>();
   }

   private void Start()
   {
      StartAlarm();
   }

   public void StartAlarm()
   {
      StartCoroutine(LightAlarm());
   }

   private IEnumerator LightAlarm()
   {
      LightColor2 *= LightIntensity;
      LightColor1 *= LightIntensity;
      while (true)
      {
         foreach (var light in lights)
         {
            light.color = LightColor2;   
         }
         yield return new WaitForSecondsRealtime(0.5f);
         foreach (var light in lights)
         {
            light.color = LightColor1;   
         }
         yield return new WaitForSecondsRealtime(0.5f);
      }
   }
   
}
