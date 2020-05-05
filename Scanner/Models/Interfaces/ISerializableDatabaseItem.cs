using Newtonsoft.Json;

namespace Scanner.Models.Iterfaces
{
    public interface ISerializableDatabaseItem : IDatabaseItem
    {
        /// <summary>
        /// При добавление текущего класса в базу данных нужно сериализовывать
        /// </summary>
        void Serialize();
        /// <summary>
        /// При получение текущего класса из базу данных нужно десериализовать
        /// </summary>
        void Deserialize();

        static JsonSerializerSettings JsonSettings = new JsonSerializerSettings()
        {
            DefaultValueHandling = DefaultValueHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
        };
    }
}
