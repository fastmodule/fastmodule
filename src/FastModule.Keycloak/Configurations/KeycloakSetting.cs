namespace FastModule.Keycloak.Configurations;


public class AdminApi
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}
public class KeycloakSetting
{
    public string Authority { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    
    public string BaseUrl { get; set; } = string.Empty;
    
    public string Realm { get; set; } = string.Empty;

    public AdminApi? AdminApi { get; set; } = null!;
}
