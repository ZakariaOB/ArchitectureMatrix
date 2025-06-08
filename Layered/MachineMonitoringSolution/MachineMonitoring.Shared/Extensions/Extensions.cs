namespace MachineMonitoring.Shared.Extensions
{
    public static class Extensions
    {
        /// <summary>
        /// Is null or empty for enumerables
        /// </summary>
        /// <typeparam name="T">Type generic</typeparam>
        /// <param name="enumerable">IEnumerable de T</param>
        /// <returns>True si la liste is null ou vide .</returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                return true;
            }

            // If our IEnumerable is a list for example the count will be better
            ICollection<T> collection = enumerable as ICollection<T>;
            if (collection != null)
            {
                return collection.Count == 0;
            }
            return !enumerable.Any();
        }
    }
}
