namespace ValheimModManager.Core.Services
{
    public class ProfileService : IProfileService
    {
        private string _selectedProfile;

        public ProfileService()
        {
            _selectedProfile = "default";
        }

        public string GetSelectedProfile()
        {
            return _selectedProfile;
        }

        public void SetSelectedProfile(string profileName)
        {
            _selectedProfile = profileName;
        }
    }
}
