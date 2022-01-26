using System;
using System.Net;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using RestSharp;
using Serilog;

namespace Arkivverket.Arkade.Core.Util
{
    public class GitHubReleaseInfoReader : IReleaseInfoReader
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);
        private GitHubReleaseInfo _latestReleaseInfo;

        public GitHubReleaseInfoReader()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        private static GitHubReleaseInfo ReadGitHubLatestReleaseInfo()
        {
            var restClient = new RestClient("https://api.github.com/");

            var request = new RestRequest("repos/arkivverket/arkade5/releases/latest");

            RestResponse<GitHubReleaseInfo> gitHubResponse = restClient.ExecuteGetAsync<GitHubReleaseInfo>(request).Result;

            return gitHubResponse.Data;
        }

        public Version GetLatestVersion()
        {
            if (_latestReleaseInfo == null)
                _latestReleaseInfo = ReadGitHubLatestReleaseInfo();

            if (_latestReleaseInfo?.TagName == null)
                throw new Exception("Missing or unexpected data from GitHub");

            if (!Regex.IsMatch(_latestReleaseInfo.TagName, @"^v?\d+.\d+.\d+$"))
                throw new Exception("Unexpected tag-name format");

            string versionNumber = _latestReleaseInfo.TagName.TrimStart('v') + ".0";

            return new Version(versionNumber);
        }

        private class GitHubReleaseInfo
        {
            [JsonPropertyName("tag_name")]
            public string TagName { get; set; }
        }
    }
}
