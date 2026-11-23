using System;
using UnityEngine;

public class Volatile : MonoBehaviour
{
   public float ExplosiveForce;
   public float speedThresholdForBoom;
   public float explosionRadius;
   public float upwardsModifier = 10f;
   public GameObject explosionParticleSystemPrefab;

   public Rigidbody rb;

   private void Awake()
   {
      rb = GetComponent<Rigidbody>();
   }

   private void Explode()
   {
      GetComponent<Soundify>().PlayAudio();
      Collider[] colliders = Physics.OverlapSphere(this.transform.position, explosionRadius);

      foreach (var col in colliders)
      {
         if (col.TryGetComponent(typeof(Rigidbody), out var component))
         {
            component.GetComponent<Rigidbody>().AddExplosionForce(ExplosiveForce, transform.position, explosionRadius, upwardsModifier);
         }
      }
   }


   private void OnCollisionEnter(Collision collision)
   {
      if (rb.linearVelocity.magnitude > speedThresholdForBoom)
      { 
         Instantiate(explosionParticleSystemPrefab, transform.position, Quaternion.identity);
         
         Explode();
         Destroy(this.gameObject);
      }
   }
}
