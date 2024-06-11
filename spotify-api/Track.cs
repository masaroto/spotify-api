namespace spotify_api
{
    public class Track
    {
        //public DateOnly Date { get; set; }
        //public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string Id { get; set; }
        public string Name { get; set; }
        public int Popularity { get; set; }

        public Track(string id, string name, int popularity)
        {
            this.Id = id;
            this.Name = name;
            this.Popularity = popularity;
        }
    }
}
