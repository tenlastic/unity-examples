using Newtonsoft.Json.Linq;
using System;

namespace Tenlastic {
    public class GameServerService : Service<GameServerModel> {

        public static readonly GameServerService singleton = new GameServerService();

        protected override string GetBaseUrl(JObject jObject) {
            return EnvironmentManager.singleton.environmentObject.gameServerApiBaseUrl;
        }

        protected override Exception GetException(HttpException ex) {
            return ex;
        }

    }
}

