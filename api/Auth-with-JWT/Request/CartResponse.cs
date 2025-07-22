namespace Auth_with_JWT.Request
{
    public class CartResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public double Rating { get; set; } = 4.5;
        public List<string> Features { get; set; } = new() { "Floodlights", "Spectator Area", "Changing Rooms" };
        public string Availability { get; set; }
        public string TimeSlots { get; set; }
        public double PricePerHour { get; set; }
        public string Image { get; set; }
        public string Date { get; set; }
        public int Duration { get; set; }
    }
}
