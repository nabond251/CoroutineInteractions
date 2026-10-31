using OneOf;

namespace CoroutineInteractions;

[GenerateOneOf]
public partial class InteractorInstruction :
    OneOfBase<Read, Write> { }
