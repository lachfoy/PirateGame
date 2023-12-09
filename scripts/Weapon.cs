using System;

public enum EWeaponType
{
	Slingshot,
	Pistol,
	Blunderbuss,
	Rifle,
	WeaponTypeCount
}

public class Weapon
{
	public bool HasThisWeapon = false;
	public String WeaponName = "generic weapon";
	public int Ammo = 10;
}

public class Slingshot : Weapon
{
	public Slingshot()
	{
		WeaponName = "Slingshot";
	}

}

