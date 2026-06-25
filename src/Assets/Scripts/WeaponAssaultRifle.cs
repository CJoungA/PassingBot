using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AmmoEvent : UnityEngine.Events.UnityEvent<int, int> { }

[System.Serializable]
public class MagazineEvent : UnityEngine.Events.UnityEvent<int> { }

public class WeaponAssaultRifle : MonoBehaviour
{
    [HideInInspector]
    public AmmoEvent onAmmoEvent = new AmmoEvent();
    [HideInInspector]
    public MagazineEvent onMagazineEvent = new MagazineEvent();
    [SerializeField]
    private Transform bulletSpawnPoint; //총알생성위치


    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip audioClipTakeOutWeapon; // 무기 장착 사운드
    [SerializeField]
    private AudioClip audioClipFire;
    [SerializeField]
    private AudioClip audioClipReload;


    [Header("Weapon Setting")]
    [SerializeField]
    private WeaponSetting weaponSetting;

    [Header("Fire Effects")]
    [SerializeField]
    private GameObject muzzleFlashEffect; 

    private float lastAttackTime = 0;
    private bool isReload = false;

    private RotateToMouse rotateToMouse;
    private AudioSource audioSource; // 사운드 재생 컴포넌트
    private PlayerAnimatorController animator;
    private ImpactMemoryPool impactMemoryPool;
    private Camera mainCamera;


    public WeaponName WeaponName => weaponSetting.weaponName;
    public int CurrentMagazine => weaponSetting.currentMagazine;
    public int MaxMagazine => weaponSetting.maxMagazine;

    public int AllAmmo => weaponSetting.allAmmo;



    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponentInParent<PlayerAnimatorController>();
        impactMemoryPool = GetComponent<ImpactMemoryPool>();
        mainCamera = Camera.main;

        weaponSetting.currentMagazine = weaponSetting.maxMagazine; // 탄창 수 최대로 설정

        weaponSetting.currentAmmo = weaponSetting.maxAmmo;
    }

    private void OnEnable()
    {
        PlaySound(audioClipTakeOutWeapon); //무기 장착 사운드 재생
        muzzleFlashEffect.SetActive(false);
        onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo); // 무기 탄수 정보를 갱신
        onMagazineEvent.Invoke(weaponSetting.currentMagazine); // 무기의 탄창정보 갱신
    }
    private void PlaySound(AudioClip clip)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void StartWeaponAction(int type = 0)
    {
        if (isReload == true || weaponSetting.allAmmo <= 0)
        {
            return;
        }
        if(type == 0)
        {
            if (weaponSetting.isAutomaticAttack == true)
            {

                StartCoroutine("OnAttackLoop");
            }
            else
            {
                OnAttack();
            }
        }
    }

    public void StopWeaponAction(int type = 0)
    {
        if(type == 0)
        {
            StopCoroutine("OnAttackLoop");
        }
    }

    public void StartReload()
    {
        if (isReload == true) return;

        StopWeaponAction();

        StartCoroutine("OnReload");
    }

    private IEnumerator OnAttackLoop()
    {
        while (true)
        {
            OnAttack();
            yield return null;
        }
    }


    public void OnAttack()
    {
        if(Time.time - lastAttackTime > weaponSetting.attackRate)
        {
            if(animator.MoveSpeed > 2f)
            {
                return;
            }
            
            lastAttackTime = Time.time;

            if (weaponSetting.currentAmmo <= 0)
            {
                StartReload();
                return;
            }
            weaponSetting.currentAmmo--; //탄약 1감소
            Score.shotNum++;
            onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

            animator.Play("Fire", -1, 0);

            StartCoroutine("OnMuzzleFlashEffect");

            PlaySound(audioClipFire);
            
            TwoStepRaycast(); //선을 발사해 목표지점공격

            weaponSetting.allAmmo = weaponSetting.currentAmmo + weaponSetting.currentMagazine;
        }
    }
    private IEnumerator OnMuzzleFlashEffect()
    {
        muzzleFlashEffect.SetActive(true);
        yield return new WaitForSeconds(weaponSetting.attackRate * 0.3f);
        muzzleFlashEffect.SetActive(false);
    }

    private IEnumerator OnReload()
    {
        isReload = true;
        animator.OnReload();
        PlaySound(audioClipReload);

        while (true)
        {
            if(audioSource.isPlaying == false && animator.CurrentAnimationIs("Movement"))
            {
                isReload = false;

                if (weaponSetting.currentMagazine >= weaponSetting.maxAmmo + weaponSetting.currentAmmo)
                {
                    weaponSetting.currentMagazine = weaponSetting.currentMagazine - (weaponSetting.maxAmmo - weaponSetting.currentAmmo);
                    weaponSetting.currentAmmo = weaponSetting.maxAmmo;
                }
                else
                {
                    if (weaponSetting.maxAmmo > weaponSetting.currentMagazine + weaponSetting.currentAmmo)
                    {
                        weaponSetting.currentAmmo = weaponSetting.currentMagazine + weaponSetting.currentAmmo;
                        weaponSetting.currentMagazine = 0;
                    }
                    else
                    {
                        weaponSetting.currentMagazine = weaponSetting.currentMagazine - (weaponSetting.maxAmmo - weaponSetting.currentAmmo);
                        weaponSetting.currentAmmo = weaponSetting.maxAmmo;
                    }
                }


                //weaponSetting.currentMagazine = weaponSetting.currentMagazine - (weaponSetting.maxAmmo - weaponSetting.currentAmmo);
                onMagazineEvent.Invoke(weaponSetting.currentMagazine);

                //weaponSetting.currentAmmo = weaponSetting.maxAmmo;
                onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);//현재 탄 수를 최대로 설정하고 바뀐 탄 수 정보를 UI에 전달
                yield break;
            }
            yield return null;
        }
    }
    private void TwoStepRaycast()
    {
        Ray ray;
        RaycastHit hit;
        Vector3 targetPoint = Vector3.zero;

        ray = mainCamera.ViewportPointToRay(Vector2.one * 0.5f);
        if(Physics.Raycast(ray,out hit, weaponSetting.attackDistance))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.origin + ray.direction * weaponSetting.attackDistance;
        }
        //Debug.DrawRay(ray.origin, ray.direction * weaponSetting.attackDistance, Color.red);

        Vector3 attackDirection = (targetPoint - bulletSpawnPoint.position).normalized;
        if(Physics.Raycast(bulletSpawnPoint.position,attackDirection,out hit, weaponSetting.attackDistance))
        {
            impactMemoryPool.SpawnImpact(hit);
            if (hit.transform.CompareTag("ImpactEnemy"))
            {
                hit.transform.GetComponentInParent<EnemyFSM>().TakeDamage(weaponSetting.damage);
                Score.countBody++;
            }
            else if (hit.transform.CompareTag("ImpactEnemyHead"))
            {
                hit.transform.GetComponentInParent<EnemyFSM>().TakeDamage(weaponSetting.damage * 3);
                Score.countHead++;
            }
            else if (hit.transform.CompareTag("ImpactEnemyArm"))
            {
                hit.transform.GetComponentInParent<EnemyFSM>().TakeDamage((int)Math.Round(weaponSetting.damage * 0.7));
                Score.countArm++;
            }
            else if (hit.transform.CompareTag("ImpactEnemyLeg"))
            {
                hit.transform.GetComponentInParent<EnemyFSM>().TakeDamage((int)Math.Round(weaponSetting.damage * 0.7));
                Score.countLeg++;
            }
        }
        //Debug.DrawRay(bulletSpawnPoint.position, attackDirection * weaponSetting.attackDistance, Color.blue);
    }

}
