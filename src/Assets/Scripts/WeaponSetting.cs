public enum WeaponName { AK47 = 0}

[System.Serializable]
public struct WeaponSetting
{
    public WeaponName weaponName;
    public int damage; //무기 공격력
    public int currentMagazine; //현재 탄창 수
    public int maxMagazine; // 최대 탄창 수
    public int currentAmmo; //현재 탄약
    public int maxAmmo; //최대 탄약
    public float attackRate; //공속
    public float attackDistance; //사거리
    public bool isAutomaticAttack; // 연사공격
    public int allAmmo;//현재 모든총알
    public int ammo;//처음 총알


}
