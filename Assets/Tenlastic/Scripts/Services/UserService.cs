using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace Tenlastic {
    public class UserService : Service<UserModel> {

        public static readonly UserService singleton = new UserService();

        protected override string GetBaseUrl(JObject jObject) {
            return EnvironmentManager.singleton.environmentObject.userApiBaseUrl;
        }

        protected override Exception GetException(HttpException ex) {
            bool isEmailTaken = ex.errors.Any(e => e.name == "UniquenessError" && e.paths.Contains("email"));
            if (isEmailTaken) {
                return new ValidationException("email", "Email address is already taken.");
            }

            bool isUsernameTaken = ex.errors.Any(e => e.name == "UniquenessError" && e.paths.Contains("username"));
            if (isUsernameTaken) {
                return new ValidationException("username", "Username is already taken.");
            }

            return ex;
        }

    }
}

