using UnityEngine;
using UnityEngine.Audio;
public class Soundify : MonoBehaviour
{
  public AudioResource audioContainer;
  public GameObject audioSourcePrefab;


  public void PlayAudio()
  {
    GameObject gb = Instantiate(audioSourcePrefab);
    var audioSource = gb.GetComponent<AudioSource>();
    audioSource.resource = audioContainer;
    audioSource.Play();
    Destroy(gb, 6);
  }
  
  
}
