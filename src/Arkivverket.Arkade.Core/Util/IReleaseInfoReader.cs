using System;

namespace Arkivverket.Arkade.Core.Util
{
    public interface IReleaseInfoReader
    {
        Version GetLatestVersion();
    }
}
