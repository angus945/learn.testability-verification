using Module.Verification.StateSnapshot;

namespace TinyArena.Verification;

public static class SnapshotObservationBarrier
{
    public static string Encode(StateSnapshotReference reference)
    {
        return $"{reference.ChannelId:N}:{reference.CaptureId}";
    }

    public static bool TryDecode(string? value, out StateSnapshotReference reference)
    {
        reference = default;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        string[] parts = value.Split(':');

        if (parts.Length != 2)
        {
            return false;
        }

        if (!Guid.TryParseExact(parts[0], "N", out Guid channelId))
        {
            return false;
        }

        if (!long.TryParse(parts[1], out long captureId))
        {
            return false;
        }

        if (captureId < 1)
        {
            return false;
        }

        reference = new StateSnapshotReference(channelId, captureId);

        return true;
    }
}