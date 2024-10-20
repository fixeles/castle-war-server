namespace DTO;

public class SaveDataDTO
{
	public bool ForceClientData { get; set; }
	public required string Playtime { get; set; }
	public required string Data { get; set; }
}