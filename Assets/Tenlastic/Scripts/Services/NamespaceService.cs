using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace Tenlastic {
    public class NamespaceService : Service<NamespaceModel> {

        public static readonly NamespaceService singleton = new NamespaceService();

        public struct RolesUsername {
            public string[] roles;
            public string username;
        }

        protected override string GetBaseUrl(JObject jObject) {
            return EnvironmentManager.singleton.environmentObject.namespaceApiBaseUrl;
        }

        protected override Exception GetException(HttpException ex) {
            bool isNameTaken = ex.errors.Any(e => e.name == "UniquenessError" && e.paths.Contains("name"));
            if (isNameTaken) {
                return new ValidationException("name", "Name is already taken.");
            }

            return ex;
        }

    }
}

