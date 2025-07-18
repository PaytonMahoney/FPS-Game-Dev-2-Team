using UnityEngine;

[CreateAssetMenu]
public class GunStats : ScriptableObject
{
    public GameObject gunModel;
    [Range(1, 40)] public int shootDMG;
    [Range(5, 1000)] public int shootDistance;
    [Range(0.1f, 3)] public float shootRate;
    public int ammoCurrent;
    [Range(5, 50)] public int ammoMax;
    [Range(0.5f, 3)] public float reloadTime;

    public ParticleSystem hitEffect;
    [Range(0, 1)] public float shootVol;
    
    public AudioClip reloadSound;
    public AudioClip emptyShotSound;
    public AudioClip shootingSound;
}