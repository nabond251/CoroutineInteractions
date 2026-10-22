namespace CoroutineInteractions.Api
{
    public class Reservation
    {
        public DateOnly Date { get; set; }

        public string? Name { get; set; }

        public string? Email { get; set; }

        public int Quantity { get; set; }

        public override string ToString()
        {
            return $"""
                Date:       {Date}
                Name:       {Name}
                Email:      {Email}
                Quantity:   {Quantity}
                """;
        }
    }
}
