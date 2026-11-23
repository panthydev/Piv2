using UnityEngine;
using UnityEngine.Audio;
public class Soundify : MonoBehaviour
{
  public AudioResource audioContainer;
  public AudioResource dropSoundContainer;
  public GameObject audioSourcePrefab;


  public void PlayAudio()
  {
    GameObject gb = Instantiate(audioSourcePrefab);
    var audioSource = gb.GetComponent<AudioSource>();
    audioSource.resource = audioContainer;
    audioSource.Play();
    Destroy(gb, 6);
  }

  public void PlayDropSound()
  {
    GameObject gb = Instantiate(audioSourcePrefab);
    var audioSource = gb.GetComponent<AudioSource>();
    audioSource.resource = dropSoundContainer;
    audioSource.Play();
    Destroy(gb, 6);
  }
  
  
}
