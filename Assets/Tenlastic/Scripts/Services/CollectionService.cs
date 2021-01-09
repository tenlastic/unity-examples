using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace Tenlastic {
    public class CollectionService : Service<CollectionModel> {

        public static readonly CollectionService singleton = new CollectionService();

        protected override string GetBaseUrl(JObject jObject) {
            return EnvironmentManager.singleton.environmentObject.collectionApiBaseUrl;
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

