using OneOf;

namespace CoroutineInteractions;

[GenerateOneOf]
public partial class ReservationsApiInstruction :
    OneOfBase<GetSlots, PostReservation> { }
