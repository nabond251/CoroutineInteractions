using OneOf;

namespace CoroutineInteractions;

[GenerateOneOf]
public partial class InteractorInstruction :
    OneOfBase<YieldDecompressor, YieldParser, Read, Write> { }
