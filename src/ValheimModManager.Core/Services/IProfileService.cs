namespace ValheimModManager.Core.Services
{
    public interface IProfileService
    {
        string GetSelectedProfile();
        void SetSelectedProfile(string profileName);
    }
}
