using Microsoft.Win32;

//Seb add
//https://github.com/File-New-Project/EarTrumpet/blob/master/EarTrumpet/Services/UserSystemPreferencesService.cs
namespace ComponentFactory.Krypton.Toolkit
{
	/// <summary>
	/// 
	/// </summary>
    public static class UserSystemPreferencesService
    {
		/// <summary>
		/// 
		/// </summary>
        public static bool IsTransparencyEnabled
        { 
            get 
            {
				using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64))
                {
					var subKey = baseKey.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
					return subKey != null ? (int)subKey.GetValue("EnableTransparency", 0) > 0 : false;
				}
            }
        }

		/// <summary>
		/// 
		/// </summary>
        public static bool UseAccentColor
        {
            get
            {
				using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64))
                {
					var subKey = baseKey.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
					return subKey != null ? (int)subKey.GetValue("ColorPrevalence", 0) > 0 : false;
                }
            }
        }
    }
}
