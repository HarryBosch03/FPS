namespace Code.Scripts.Weapons
{
    public interface IWeapon : IComponent
    {
        bool Shoot { get; set; }
    }
}
