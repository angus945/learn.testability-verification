using Module.Verification.RuntimeControl;

namespace TinyArena.Verification.Tests;

public sealed class RuntimeOperationExecutorTests
{
    [Fact]
    public void Execute_ShouldCompleteSuccessfulOperation()
    {
        OperationRegistry<int> registry = new OperationRegistry<int>("test", 0);
        RuntimeOperationExecutor<int> executor = new RuntimeOperationExecutor<int>(registry);

        OperationDescriptor descriptor = new OperationDescriptor("test.operation");

        Func<int> execute = () => 42;
        Func<int, OperationCompletion<int>> createCompletion = result => new OperationCompletion<int>(OperationState.Succeeded, "test.succeeded", result);

        OperationAdmission admission = executor.Execute(descriptor, execute, createCompletion, "test.failed");

        Assert.Equal(OperationAdmissionStatus.Admitted, admission.Status);

        OperationRead<int> read = executor.Read(admission.Handle);

        Assert.Equal(OperationReadState.Found, read.ReadState);
        Assert.Equal(OperationState.Succeeded, read.State);
        Assert.Equal(42, read.Completion.Result);
    }

    [Fact]
    public void Execute_WhenApplicationThrows_ShouldCompleteAsFailed()
    {
        OperationRegistry<int> registry = new OperationRegistry<int>("test", 0);
        RuntimeOperationExecutor<int> executor = new RuntimeOperationExecutor<int>(registry);

        OperationDescriptor descriptor = new OperationDescriptor("test.operation");

        Func<int> execute = () => throw new InvalidOperationException("Simulated application failure.");
        Func<int, OperationCompletion<int>> createCompletion = result => new OperationCompletion<int>(OperationState.Succeeded, "test.succeeded", result);

        OperationAdmission admission = executor.Execute(descriptor, execute, createCompletion, "test.failed");

        OperationRead<int> read = executor.Read(admission.Handle);

        Assert.Equal(OperationState.Failed, read.State);
        Assert.Equal("test.failed", read.Completion.Code);
    }


}