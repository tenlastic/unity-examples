using System;

namespace Tenlastic {
    [Serializable]
    public class BuildModel {

        [Serializable]
        public struct Entrypoints {
            public string server64;
            public string windows64;
        }

        public string _id;
        public DateTime createdAt;
        public Entrypoints entrypoint;
        public string namespaceId;
        public DateTime publishedAt;
        public string version;
        public DateTime updatedAt;

    }
}
