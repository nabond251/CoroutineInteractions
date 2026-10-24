# CoroutineInteractions

Experiments with augmenting async methods to behave as effect producers which an external effect runner can consume, Elm-style.

Calling this a coroutine system may be a bit premature - so far, I don't have any examples of two coroutines interacting back and forth with each other.  Maybe SagaInteractions would have been a better name.

## Play with it

Either run the tests in `CoroutineInteractions.Tests`, or run the main `CoroutineInteractions` project's command line application.  If you opt for the latter, you will also want to have the API in `CoroutineInteractions.Api` running.

## Motives

Consider the async method below:

```cs
public static async Task GreetAsync(ICommandLine commandLine)
{
    await commandLine.WriteLineAsync("Please enter your name.");
    var s = await commandLine.ReadLineAsync();
    await commandLine.WriteLineAsync($"Hello, {s}!");
}
```

Really, nothing very fancy.  Just a simple async method with some external behavior injected via its parameter list.

Oh, and it's testable.

Well of course, just inject a `ICommandLine` spy implementation and assert its method calls.

What if I told you... there is no mock...

Go ahead, peruse my blatantly disorganized pile of code here - you won't find a single mock, anywhere.  (Not because it can't be done in this context, I was just too lazy to put one together.)  Instead, the test goes like the following:

```cs
[Theory]
[InlineData("")]
[InlineData("Mark")]
[InlineData("Nathan")]
public async Task GreetsUserAsync(string username)
{
    var requestChannel = Channel.CreateUnbounded<CommandLineInstruction>();
    var writer = new CommandLineInstructionWriter(requestChannel.Writer);
    var cli = new CoroutineCommandLine(writer);
    var coroutine = CommandLineCoroutines.GreetAsync(cli);
    var sut = requestChannel.Reader.ReadAllAsync().GetAsyncEnumerator();

    Assert.True(await sut.MoveNextAsync());
    sut.Current.AssertWriteLine("Please enter your name.");

    Assert.True(await sut.MoveNextAsync());
    sut.Current.AssertReadLine(username);

    Assert.True(await sut.MoveNextAsync());
    sut.Current.AssertWriteLine($"Hello, {username}!");

    requestChannel.Writer.Complete();
    Assert.False(await sut.MoveNextAsync());
    await coroutine;
}
```

Can't exactly say it follows good AAA unit test practice, but if nothing else, it's very lean, low-ceremony, easy to follow.

## How

The simple, basic idea that makes this machinery possible...  send requests through a channel, along with a response channel of the response type.  Await the response if there's any expected:

```cs
public class CoroutineCommandLine(
    ICommandLineInstructionWriter requestWriter) : ICommandLine
{
    public async Task<string?> ReadLineAsync()
    {
        var readLineRequest = new ReadLine();
        await requestWriter.WriteAsync(readLineRequest);
        return await readLineRequest.Responses.Reader.ReadAsync();
    }

    public async Task WriteLineAsync(string text)
    {
        await requestWriter.WriteAsync(new WriteLine(text));
    }
}
```

## Acks

The examples in the main console program and the test project are from [Mark Seemann](https://x.com/ploeh)'s blog series [Pure Interactions](https://blog.ploeh.dk/2017/07/10/pure-interactions/).

The core channel-passing idea was something I picked up from the Google search LLM when I asked it on a whim to rewrite [Redux-Saga](https://redux-saga.js.org/) in C#.  I'm not sure if Redux-Saga does something exactly like this; it may just be a simple way to implement the JavaScript generator's `yield` behavior in C#.

This is also very similar to how the Android IPC [Messenger](https://developer.android.com/develop/background-work/services/bound-services#Messenger) service works (see [`Message.replyTo`](https://developer.android.com/reference/android/os/Message#replyTo)).
