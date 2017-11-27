using System;

namespace Arkivverket.Arkade.Util
{
    public interface IReleaseInfoReader
    {
        Version GetLatestVersion();
    }
}
