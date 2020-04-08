using Newtonsoft.Json;

namespace VerificationCheck.Core.Interfaces
{
    public interface ISerializable
    {
        /// <summary>
        /// При добавление текущего класса в бд нужно сериализовывать
        /// </summary>
        void Serialize();
        /// <summary>
        /// При получение текущего класса из бд нужно десериализовать
        /// </summary>
        void Deserialize();

        static JsonSerializerSettings JsonSettings = new JsonSerializerSettings()
        {
            DefaultValueHandling = DefaultValueHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,

        };
    }
}
