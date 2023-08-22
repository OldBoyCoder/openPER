using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;

namespace openPER.Helpers
{
    public class ForumLinkHelper
    {
        private static string baseUrl;
        private static Dictionary<string, string> forumLinks = new Dictionary<string, string>();
        public static IConfiguration config;
        public static void Initialize(IConfiguration Configuration)
        {
            config = Configuration;
            var forumLinkFile = config["ForumLinks"];
            if (forumLinkFile != null)
            {
                var lines = File.ReadAllLines(forumLinkFile);
                foreach (var line in lines)
                {
                    var parts = line.Split("\t");
                    forumLinks.Add(parts[0], parts[1]);
                }
            }
            baseUrl = config["ForumBaseUrl"];
        }
        public static string GetForumLink(string make, string model, string catalogue)
        {
            var key = $"{model}/{catalogue}";
            if (forumLinks.ContainsKey(key))
                return baseUrl + forumLinks[key]+"/";
            if (forumLinks.ContainsKey(model))
                return baseUrl + forumLinks[model] + "/";
            if (make == "F" || make == "C")
                return baseUrl;

            return null;
        }
    }
}
