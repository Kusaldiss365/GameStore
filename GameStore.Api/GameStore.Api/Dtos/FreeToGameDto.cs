using System.Text.Json.Serialization;

namespace GameStore.Api.Dtos
{
    public class FreeToGameDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Thumbnail { get; set; } = string.Empty;

        [JsonPropertyName("short_description")]
        public string ShortDescription { get; set; } = string.Empty;

        [JsonPropertyName("game_url")]
        public string GameUrl { get; set; } = string.Empty;

        public string Genre { get; set; } = string.Empty;
        public string Platform { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public string Developer { get; set; } = string.Empty;

        [JsonPropertyName("release_date")]
        public string? ReleaseDate { get; set; }

        [JsonPropertyName("freetogame_profile_url")]
        public string FreeToGameProfileUrl { get; set; } = string.Empty;
    }
}
