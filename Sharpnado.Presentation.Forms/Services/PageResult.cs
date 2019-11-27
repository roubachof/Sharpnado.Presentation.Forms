using System.Collections.Generic;

namespace Sharpnado.Presentation.Forms.Services
{
    public struct PageResult<TItem>
    {
        public static readonly PageResult<TItem> Empty = new PageResult<TItem>(0, new List<TItem>());

        public PageResult(int totalCount, IReadOnlyList<TItem> items)
        {
            TotalCount = totalCount;
            Items = items ?? new List<TItem>();
        }

        public int TotalCount { get; }

        public IReadOnlyList<TItem> Items { get; }

        public static bool operator ==(PageResult<TItem> p1, PageResult<TItem> p2)
        {
            return ReferenceEquals(p1.Items, p2.Items) && p1.TotalCount == p2.TotalCount;
        }

        public static bool operator !=(PageResult<TItem> p1, PageResult<TItem> p2)
        {
            return !ReferenceEquals(p1.Items, p2.Items) || p1.TotalCount != p2.TotalCount;
        }

        public bool Equals(PageResult<TItem> other)
        {
            return TotalCount == other.TotalCount && Equals(Items, other.Items);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is PageResult<TItem> result && Equals(result);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (TotalCount * 397) ^ (Items != null ? Items.GetHashCode() : 0);
            }
        }
    }
}
