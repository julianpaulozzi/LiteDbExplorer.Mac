using System;
using LiteDB;

namespace LiteDbExplorer.Mac.Converters
{
    public class BsonValueToStringConverter
    {
        public static string Convert(object value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            if (value is BsonValue bsonValue)
            {
                if (bsonValue.IsDocument)
                {
                    return "[Document]";
                }

                if (bsonValue.IsArray)
                {
                    return "[Array]";
                }
                if (bsonValue.IsBinary)
                {
                    return "[Binary]";
                }
                if (bsonValue.IsObjectId)
                {
                    return bsonValue.AsString;
                }
                if (bsonValue.IsDateTime)
                {
                    return bsonValue.AsDateTime.ToString("s");
                }
                if (bsonValue.IsGuid)
                {
                    return bsonValue.AsGuid.ToString();
                }
                if (bsonValue.IsString)
                {
                    return bsonValue.AsString;
                }
                if (bsonValue.IsInt32)
                {
                    return bsonValue.AsInt32.ToString();
                }
                if (bsonValue.IsInt64)
                {
                    return bsonValue.AsInt64.ToString();
                }

                return bsonValue.ToString();
            }

            throw new Exception("Cannot convert non BSON value");
        }

        public static BsonValue ConvertBack(object value)
        {
            if (value == null)
            {
                return null;
            }

            return new BsonValue(value as string);
        }
    }
}
