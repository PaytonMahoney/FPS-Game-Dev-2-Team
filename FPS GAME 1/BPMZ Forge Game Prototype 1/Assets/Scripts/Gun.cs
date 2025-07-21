using System.Collections;
using UnityEngine;


public abstract class Gun : ScriptableObject
{
    public GameObject gunModel;
    [Range(1, 100)] public int shootDMG;
    [Range(5, 1000)] public int shootDistance;
    [Range(0.1f, 5)] public float shootRate;
    public int ammoCurrent;
    [Range(1, 999)] public int ammoMax;

    [Range(1,999)]public int magMax;
    public int magCurrent;
    [Range(0.5f, 5)] public float reloadTime;

    public ParticleSystem hitEffect;
    [Range(0, 1)] public float shootVol;

    [SerializeField] public GameObject projectile;
    public Transform shootPOS;

    public AudioClip reloadSound;
    public AudioClip emptyShotSound;
    public AudioClip shootingSound;

   
    
    public abstract void shoot(LayerMask ignoreLayer, AudioSource audio);
    
       
    public abstract IEnumerator reload(AudioSource audio);

    
}


