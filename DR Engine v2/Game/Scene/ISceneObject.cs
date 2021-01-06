namespace DREngine.Game.Scene
{
    //[JsonConverter(typeof(SceneObjectJsonReader))]
    public interface ISceneObject
    {
        // This can be removed (as it's not used now) but I'll keep it here in case if we decide to switch to this later for compatibility reasons.
        public string Type { get; set; }
    }

    /*
    internal class SceneObjectJsonReader : JsonConverter
    {
        private JsonConverter AsDefaultConverter = new AsDefaultConverter();

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            Debug.Log($"WRITING: {value}");
            JObject.FromObject(value).WriteTo(writer);
            //AsDefaultConverter.WriteJson(writer, value, serializer);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            return AsDefaultConverter.ReadJson(reader, objectType, existingValue, serializer);
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(ISceneObject).IsAssignableFrom(objectType);
        }
    }

    internal class AsDefaultConverter : JsonConverter
    {

        public override bool CanWrite => false;

        public override bool CanRead => false;

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
    */

/*
internal class SceneObjectJsonConverter : JsonConverter//Newtonsoft.Json.Converters.CustomCreationConverter<ISceneObject>
{
    private static DRGame _game;

    public static void InitConversion(DRGame currentGame)
    {
        _game = currentGame;
    }

    public ISceneObject Create(Type objectType, JObject jObject)
    {
        var type = (string)jObject.Property("type");

        switch (type)
        {
            case "Cube":
                return new Cube(_game);
            case "Billboard":
                return new Billboard(_game);
        }

        throw new ApplicationException(String.Format("ISceneObject type {0} is not supported!", type));
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value is ISceneObject sceneObject)
        {
            switch (sceneObject.Type)
            {
                case "Cube":
                    Debug.Log("Serialized a CUBE");
                    WriteJsonSpec<Cube>(writer, (Cube)value, serializer);
                    break;
                case "Billboard":
                    Debug.Log("Serialized a BILLBOARD");
                    WriteJsonSpec<Billboard>(writer, (Billboard)value, serializer);
                    break;
            }
        }
        else
        {
            Debug.LogError($"Tried serializing an object as an ISceneObject when it isn't an ISceneObject: {value}. Will do nothing, expect invalid JSON output.");
            //serializer.Serialize(writer, value);
            //base.WriteJson(writer, value, serializer);
        }
    }

    private void WriteJsonSpec<T>(JsonWriter writer, T value, JsonSerializer serializer)
    {
        //writer.WriteValue("OI BRUV");
        JsonConvert.SerializeObject(value);
        //writer.WriteValue(serializer.Ser);
        //base.WriteJson(writer, value, serializer);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        // Load JObject from stream
        JObject jObject = JObject.Load(reader);

        // Create target object based on JObject
        var target = Create(objectType, jObject);

        // Populate the object properties
        serializer.Populate(jObject.CreateReader(), target);

        return target;
    }

    public override bool CanConvert(Type objectType)
    {
        Debug.Log($"TRIED: {objectType}");
        return typeof(ISceneObject).IsAssignableFrom(objectType);
    }
}
*/
}