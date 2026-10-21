namespace CoroutineInteractions;

public record class Reservation(
    DateTimeOffset Date,
    string? Name,
    string? Email,
    int Quantity);
