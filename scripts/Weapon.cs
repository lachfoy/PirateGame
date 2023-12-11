using Godot;

public partial class Weapon : Resource
{
    public enum EWeaponType
    {
	    Slingshot,
	    Pistol,
	    Blunderbuss,
	    Rifle,
	    WeaponTypeCount
    }

    [Export]
    public EWeaponType WeaponType = EWeaponType.WeaponTypeCount;

    [Export]
	public string WeaponName = "generic weapon";

    [Export]
    public int Ammo = 10;

    [Export]
    public int Damage = 10;

    [Export]
    public float AccuracyPentalty = 0.4f;

    //[Export]
    //public int Slot = 0;
}
