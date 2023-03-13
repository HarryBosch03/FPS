namespace Code.Scripts.Weapons
{
    public interface IWeapon : IComponent
    {
        bool enabled { get; }
        bool Shoot { get; set; }
        void Equip();
        void Holster();
    }
}