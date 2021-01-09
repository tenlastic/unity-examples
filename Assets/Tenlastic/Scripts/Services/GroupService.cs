using Newtonsoft.Json.Linq;
using System;

namespace Tenlastic {
    public class GroupService : Service<GroupModel> {

        public static readonly GroupService singleton = new GroupService();


        protected override string GetBaseUrl(JObject jObject) {
            return EnvironmentManager.singleton.environmentObject.groupApiBaseUrl;
        }

        protected override Exception GetException(HttpException ex) {
            return ex;
        }

    }
}

