using Godot;
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

	public virtual void Shoot() {}
}

public class Slingshot : Weapon
{
	public Slingshot()
	{
		WeaponName = "Slingshot";
	}

	public override void Shoot()
	{
		GD.Print("Pew slingshot");
	}
}

public class Pistol : Weapon
{
	public Pistol()
	{
		WeaponName = "Pistol";
	}

	public override void Shoot()
	{
		GD.Print("Pew Pistol");
	}
}

