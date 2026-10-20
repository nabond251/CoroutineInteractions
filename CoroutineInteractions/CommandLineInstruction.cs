using OneOf;

namespace CoroutineInteractions;

[GenerateOneOf]
public partial class CommandLineInstruction :
    OneOfBase<ReadLine, WriteLine> { }
