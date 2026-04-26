using TMPro;
using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
    public TextMeshProUGUI magText;
    public AudioSource reloadAudio;
    public AudioSource shootAudio;
    public GameObject muzzleflashSpawnPoint;
    public GameObject muzzleFlashEffect;
    public GameObject bulletDecalPrefab;
    public Camera fpscam;
    public GameObject gunModel;

    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 0.5f;
    private float nextTimeToFire = 0f;

    public float magazineSize = 10;
    private float currentBullets;
    private bool isReloading = false;

    public float spinSpeed = 360f; 

    void Start()
    {
        currentBullets = magazineSize;
        UpdateUI();
    }

    void Update()
    {
        if (isReloading) return;

        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + fireRate;
            if (currentBullets > 0)
            {
                Shoot();
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && currentBullets < magazineSize)
        {
            StartCoroutine(ReloadRoutine());
        }
    }

    void Shoot()
    {
        currentBullets--;
        UpdateUI();

        shootAudio.Play();
        GameObject muzzleflash = Instantiate(muzzleFlashEffect, muzzleflashSpawnPoint.transform.position, muzzleflashSpawnPoint.transform.rotation);
        Destroy(muzzleflash, 0.4f);

        RaycastHit hit;
        if (Physics.Raycast(fpscam.transform.position, fpscam.transform.forward, out hit, range))
        {
            if (hit.transform.CompareTag("Golem"))
            {
                DoDamage(hit.transform.gameObject);
            }

            CreateDecal(hit);
        }
    }

    public void DoDamage(GameObject target)
    {
        target.GetComponent<Enemy_AI>().TakeDamage(damage);
    }

    void CreateDecal(RaycastHit hit)
    {
        GameObject decal = Instantiate(bulletDecalPrefab, hit.point, Quaternion.LookRotation(hit.normal));
        decal.transform.SetParent(hit.transform);
        Destroy(decal, 5f);
    }

    IEnumerator ReloadRoutine()
    {
        isReloading = true;
        reloadAudio.Play();

        float totalRotationGoal = 720f;
        float currentRotation = 0f;
        
        Quaternion startingRotation = gunModel.transform.localRotation;

        while (currentRotation < totalRotationGoal)
        {
            float step = spinSpeed * Time.deltaTime;
            
            if (currentRotation + step > totalRotationGoal)
                step = totalRotationGoal - currentRotation;

            currentRotation += step;
            
            gunModel.transform.Rotate(step, 0, 0, Space.Self);
            
            yield return null;
        }

        gunModel.transform.localRotation = startingRotation;
        currentBullets = magazineSize;
        UpdateUI();
        isReloading = false;
    }

    void UpdateUI()
    {
        magText.text = currentBullets + " / " + magazineSize;
    }
}