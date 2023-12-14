using Godot;

public partial class Weapon : Resource
{
    public enum EWeaponType
    {
	    Slingshot,
	    //SingleRayCast,
        Pistol,
	    Blunderbuss,
	    Rifle,
	    WeaponTypeCount
    }

    public enum EWeaponSlot
    {
        Slot1,
        Slot2,
        Slot3,
        Slot4,
        WeaponSlotCount
    }

    [Export]
	public string WeaponName;

    [Export]
    public EWeaponSlot WeaponSlot;

    [Export]
    public EWeaponType WeaponType;

    [Export]
    public int Ammo;

    [Export]
    public int Damage;

    [Export]
    public float AccuracyPentalty;

    [Export]
    public Texture2D FirstPersonSprite;

    [Export]
    public Vector2 FirstPersonSpritePosition;

    [Export]
    public Texture2D PickupSprite;

    //[Export]
    //public int Slot = 0;
}
