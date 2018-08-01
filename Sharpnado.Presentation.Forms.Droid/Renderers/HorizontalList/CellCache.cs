using System.Collections.Generic;

using Android.Views;

using Xamarin.Forms;

using View = Android.Views.View;

namespace Sharpnado.Presentation.Forms.Droid.Renderers.HorizontalList
{
    public class CellCache
    {
        private readonly IList<ViewCellHolder> _viewCellCache;
        private readonly int _cacheSize;

        public CellCache(int initialSize, int cacheSize = 0)
        {
            _cacheSize = cacheSize;

            if (cacheSize == -1)
            {
                // Cache disabled
                _viewCellCache = new List<ViewCellHolder>();
                return;
            }

            if (_cacheSize > 0)
            {
                initialSize = initialSize > _cacheSize ? _cacheSize : initialSize;
            }

            _viewCellCache = new List<ViewCellHolder>(initialSize);
            for (int i = 0; i < initialSize; i++)
            {
                _viewCellCache.Add(new ViewCellHolder());
            }
        }

        public int Count => _viewCellCache.Count;

        public void Clear()
        {
            foreach (var holder in _viewCellCache)
            {
                if (holder.CachedView is ViewGroup viewGroup)
                {
                    viewGroup.RemoveAllViews();
                }

                holder.CachedView?.Dispose();
            }

            _viewCellCache.Clear();
        }

        public void IncrementSize()
        {
            if (_cacheSize == -1)
            {
                return;
            }

            if (_cacheSize > 0 && _viewCellCache.Count == _cacheSize)
            {
                return;
            }

            _viewCellCache.Add(new ViewCellHolder());
        }

        public void Remove(int index)
        {
            if (_cacheSize == -1)
            {
                return;
            }

            index = PreProcessIndex(index);

            _viewCellCache.RemoveAt(index);
        }

        public void AddOrUpdateView(int index, View view)
        {
            if (_cacheSize == -1)
            {
                return;
            }

            index = PreProcessIndex(index);

            _viewCellCache[index].CachedView = view;
        }

        public void AddOrUpdateViewCell(int index, ViewCell view)
        {
            if (_cacheSize == -1)
            {
                return;
            }

            index = PreProcessIndex(index);

            _viewCellCache[index].CachedCell = view;
        }

        public bool TryGetView(int index, out View view)
        {
            if (_cacheSize == -1)
            {
                view = null;
                return false;
            }

            index = PreProcessIndex(index);

            view = _viewCellCache[index].CachedView;
            return view != null;
        }

        public bool TryGetCell(int index, out ViewCell cell)
        {
            if (_cacheSize == -1)
            {
                cell = null;
                return false;
            }

            index = PreProcessIndex(index);

            cell = _viewCellCache[index].CachedCell;
            return cell != null;
        }

        public void Swap(int firstIndex, int secondIndex)
        {
            if (_cacheSize == -1)
            {
                return;
            }

            firstIndex = PreProcessIndex(firstIndex);
            secondIndex = PreProcessIndex(secondIndex);

            ViewCellHolder tmp = _viewCellCache[firstIndex];
            _viewCellCache[firstIndex] = _viewCellCache[secondIndex];
            _viewCellCache[secondIndex] = tmp;
        }

        private int PreProcessIndex(int index)
        {
            return _cacheSize > 0 ? index % _cacheSize : index;
        }

        private class ViewCellHolder
        {
            public ViewCell CachedCell { get; set; }

            public View CachedView { get; set; }
        }
    }
}