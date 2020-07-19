namespace SwaggerUsage
{
    public class Settings
    {
        public string ProjectName { get; set; }
        public string ExhibitionName { get; set; }
        public SettingsVersion Version { get; set; }
        public string Url { get; set; }
    }

    public class SettingsVersion
    {
        public int Major { get; set; }
        public int Minor { get; set; }
    }
}
