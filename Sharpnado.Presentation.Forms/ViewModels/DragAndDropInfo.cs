using System.Reflection;

namespace Sharpnado.Presentation.Forms.ViewModels
{
    public class DragAndDropInfo
    {
        public int To { get; set; }

        public int From { get; set; }

        public object Content { get; set; }
    }
}