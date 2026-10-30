namespace EtherGizmos.Shipyard.Extensions;

public static class TypeExtensions
{
    extension(Type @this)
    {
        public Type AsElementType()
        {
            //1. Check if the type itself is a generic IList<>
            if (@this.IsGenericType && @this.GetGenericTypeDefinition() == typeof(IList<>))
            {
                return @this.GetGenericArguments()[0];
            }

            //2. Check if it implements a generic IList<> (e.g., List<T>)
            foreach (var interfaceType in @this.GetInterfaces())
            {
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IList<>))
                {
                    return interfaceType.GetGenericArguments()[0];
                }
            }

            //3. Return the original type if it's not an IList
            return @this;
        }
    }
}
