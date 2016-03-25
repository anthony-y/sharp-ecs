namespace SharpECS
{
    internal static class Util
    {
        public static T CastToType<T>(object objToCast)
        {
            var newObj = (T)objToCast;
            return newObj;
        }

        public static T CastToType<T>(T typeToCast, object objToCast)
        {
            var newObj = (T)objToCast;
            return newObj;
        }

    }
}
