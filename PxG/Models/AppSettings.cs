namespace PxG.Models
{
    public class AppSettings
    {
        public string PokemonKey { get; set; } = "";
        public string ReviveKey { get; set; } = "";
        public string ExecuteKey { get; set; } = "";
        public int RevivePositionX { get; set; } = 0;
        public int RevivePositionY { get; set; } = 0;
        public int MedicinePositionX { get; set; } = 0;
        public int MedicinePositionY { get; set; } = 0;
        public string LastSelectedWindow { get; set; } = "";
    }
}
