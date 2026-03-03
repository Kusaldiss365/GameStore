using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Models
{
    public class Game
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Thumbnail { get; set; } = string.Empty;
        public string ShortDescription { get; set; } = string.Empty;
        public string GameUrl { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public string Platform { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public string Developer { get; set; } = string.Empty;
        public DateOnly? ReleaseDate { get; set; }
        public string FreeToGameProfileUrl { get; set; } = string.Empty;
    }
}
