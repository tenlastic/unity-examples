using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Tenlastic {
    public abstract class Service<TModel> {

        [Serializable]
        public struct FindRequest {
            public int limit;
            public int skip;
            public dynamic where;
        }

        #pragma warning disable 0649
        [Serializable]
        private struct RecordResponse {
            public TModel record;
        }

        [Serializable]
        private struct RecordsResponse {
            public TModel[] records;
        }
        #pragma warning restore 0649

        public delegate void OnCreateRecordDelegate(TModel record);
        public event OnCreateRecordDelegate OnCreateRecord;

        public delegate void OnDeleteRecordDelegate(string _id);
        public event OnDeleteRecordDelegate OnDeleteRecord;

        public delegate void OnUpdateRecordDelegate(TModel record);
        public event OnUpdateRecordDelegate OnUpdateRecord;

        private readonly HttpManager httpManager = HttpManager.singleton;

        public async Task<TModel> CreateRecord(JObject jObject) {
            try {
                RecordResponse response = await httpManager.Request<RecordResponse>(
                    HttpMethod.Post,
                    GetBaseUrl(jObject),
                    jObject
                );

                OnCreateRecord?.Invoke(response.record);

                return response.record;
            } catch (HttpException ex) {
                throw GetException(ex);
            }
        }

        public async Task DeleteRecord(string _id) {
            JObject jObject = new JObject {
                { "_id", _id }
            };

            await httpManager.Request(
                HttpMethod.Delete,
                GetBaseUrl(jObject) + "/" + _id,
                null
            );

            OnDeleteRecord?.Invoke(_id);
        }

        public async Task<TModel> FindRecordById(string _id) {
            JObject jObject = new JObject {
                { "_id", _id }
            };

            RecordResponse response = await httpManager.Request<RecordResponse>(
                HttpMethod.Get,
                GetBaseUrl(jObject) + "/" + _id,
                null
            );

            return response.record;
        }

        public async Task<TModel[]> FindRecords(JObject jObject) {
            RecordsResponse response = await httpManager.Request<RecordsResponse>(
                HttpMethod.Get,
                GetBaseUrl(jObject),
                jObject
            );

            return response.records;
        }

        public async Task<TModel> UpdateRecord(JObject jObject) {
            try {
                RecordResponse response = await httpManager.Request<RecordResponse>(
                    HttpMethod.Put,
                    GetBaseUrl(jObject) + "/" + jObject.GetValue("_id").ToObject<string>(),
                    jObject
                );

                OnUpdateRecord?.Invoke(response.record);

                return response.record;
            } catch (HttpException ex) {
                throw GetException(ex);
            }
        }

        protected abstract string GetBaseUrl(JObject jObject);

        protected virtual Exception GetException(HttpException ex) {
            return ex;
        }

    }
}

