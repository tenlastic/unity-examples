using Newtonsoft.Json.Linq;
using System;

namespace Tenlastic {
    public class BuildService : Service<BuildModel> {

        public static readonly BuildService singleton = new BuildService();

        protected override string GetBaseUrl(JObject jObject) {
            return EnvironmentManager.singleton.environmentObject.buildApiBaseUrl;
        }

        protected override Exception GetException(HttpException ex) {
            return ex;
        }

    }
}
