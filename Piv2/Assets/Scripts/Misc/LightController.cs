using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
   public Color LightColor1;
   public Color LightColor2;
   private float LightIntensity = 8;
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
      float factor = Mathf.Pow(2, LightIntensity);

      LightColor2 *= factor;
      LightColor1 *= factor;
      while (true)
      {
         AudioManager.Instance.PlaySFX("Alarm");
         lights[0].color = LightColor2;
         lights[1].color = LightColor2;
         yield return new WaitForSecondsRealtime(0.5f);
         lights[0].color = LightColor1;
         lights[1].color = LightColor1;
         yield return new WaitForSecondsRealtime(0.5f);
      }
   }
   
}
