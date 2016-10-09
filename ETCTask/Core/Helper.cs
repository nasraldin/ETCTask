using System.Collections.Generic;

namespace ETCTask.Core
{
    public static class Helper
    {
        public static IEnumerable<IEnumerable<T>> Count<T>(
            this IEnumerable<T> product, int countSize)
        {
            using (var enumerator = product.GetEnumerator())
            {
                while (enumerator.MoveNext())
                    yield return CountElements(enumerator, countSize - 1);
            }
        }

        private static IEnumerable<T> CountElements<T>(
            IEnumerator<T> product, int batchSize)
        {
            yield return product.Current;
            for (var i = 0; (i < batchSize) && product.MoveNext(); i++)
                yield return product.Current;
        }
    }
}