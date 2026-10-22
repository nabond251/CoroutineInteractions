using OneOf;

namespace CoroutineInteractions;

[GenerateOneOf]
public partial class WizardInstruction :
    OneOfBase<CommandLineInstruction, ReservationsApiInstruction> { }
