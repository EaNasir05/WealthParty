using System.Collections;
using UnityEngine;

public class SoundEffectsManager : MonoBehaviour
{
    public static SoundEffectsManager instance;
    [SerializeField] private AudioSource soundEffectObject;
    [SerializeField] private AudioClip buttonClip;
    [SerializeField] private AudioClip victoryClip;
    [SerializeField] private AudioClip nextTurnClip;
    [SerializeField] private AudioClip incomeClip;
    private AudioSource[] regionsOST;
    private AudioSource currentOST;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        regionsOST = GameObject.FindGameObjectWithTag("OSTManager")?.GetComponents<AudioSource>();
    }

    private IEnumerator PlaySFX(AudioClip clip, float volume)
    {
        AudioSource newSource = Instantiate(soundEffectObject, Vector3.zero, Quaternion.identity);
        newSource.clip = clip;
        newSource.volume = volume;
        newSource.Play();
        yield return new WaitForSeconds(newSource.clip.length);
        Destroy(newSource.gameObject);
    }

    public void PlayButtonClip()
    {
        StartCoroutine(PlaySFX(buttonClip, 1));
    }

    public void PlayVictoryClip()
    {
        StartCoroutine(PlaySFX(victoryClip, 1));
    }

    public void PlayNextTurnClip()
    {
        StartCoroutine(PlaySFX(nextTurnClip, (float)0.75));
    }

    public void PlayIncomeClip()
    {
        StartCoroutine(PlaySFX(incomeClip, 1));
    }

    public void PlayRegionOST(int region)
    {
        if (currentOST != null)
        {
            currentOST.Stop();
        }
        currentOST = regionsOST[region];
        currentOST.Play();
    }

    /*public void PlayMainMenuOST()
    {
        if (currentOST != null)
        {
            currentOST.Stop();
            Destroy(currentOST.gameObject);
        }
        AudioSource newSource = Instantiate(soundEffectObject, Vector3.zero, Quaternion.identity);
        newSource.clip = mainMenuOST;
        newSource.volume = 1;
        newSource.loop = true;
        currentOST = newSource;
        newSource.Play();
    }*/

    public void StopCurrentOST()
    {
        if (currentOST != null)
        {
            currentOST.Stop();
        }
        currentOST = null;
    }
}
