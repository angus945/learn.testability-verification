using Module.Verification.RuntimeControl;

namespace TinyArena.Verification;

public sealed class RuntimeOperationExecutor<TResult>
{
    private readonly OperationRegistry<TResult> _registry;

    public RuntimeOperationExecutor(OperationRegistry<TResult> registry)
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
    }

    public OperationAdmission Execute(OperationDescriptor descriptor, Func<TResult> execute, Func<TResult, OperationCompletion<TResult>> createCompletion, string failureCode)
    {
        ArgumentNullException.ThrowIfNull(descriptor);
        ArgumentNullException.ThrowIfNull(execute);
        ArgumentNullException.ThrowIfNull(createCompletion);

        if (string.IsNullOrWhiteSpace(failureCode))
        {
            throw new ArgumentException("A failure code is required.", nameof(failureCode));
        }

        OperationAdmission admission = _registry.Admit(descriptor);

        if (admission.Status == OperationAdmissionStatus.Duplicate)
        {
            return admission;
        }

        if (admission.Status != OperationAdmissionStatus.Admitted)
        {
            return admission;
        }

        OperationHandle handle = admission.Handle;

        bool started = _registry.TryMarkRunning(handle);

        if (!started)
        {
            throw new InvalidOperationException("Admitted operation could not transition to Running.");
        }

        TResult result;

        try
        {
            result = execute();
        }
        catch (Exception)
        {
            CompleteFailed(handle, failureCode);
            return admission;
        }

        OperationCompletion<TResult> completion = createCompletion(result);

        bool completed = _registry.TryComplete(handle, completion);

        if (!completed)
        {
            throw new InvalidOperationException("Running operation could not be completed.");
        }

        return admission;
    }

    public OperationRead<TResult> Read(OperationHandle handle)
    {
        return _registry.Read(handle);
    }

    private void CompleteFailed(OperationHandle handle, string failureCode)
    {
        OperationCompletion<TResult> completion = new OperationCompletion<TResult>(OperationState.Failed, failureCode);

        bool completed = _registry.TryComplete(handle, completion);

        if (!completed)
        {
            throw new InvalidOperationException("Failed operation could not be completed.");
        }
    }
}