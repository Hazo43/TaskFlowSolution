using Shared.DTOs.Enums;

namespace Shared.DTOs
{
    public class TaskSpecificationParameter
    {
        public int? ProjectId { get; set; }
        public int? AssignedToId { get; set; }
        public TaskStatusDTO? Status { get; set; }
        public TaskPriorityDTO? Priority { get; set; }
        public string? Search { get; set; }

        // Skip 
        private int _pageIndex { get; set; } = 1;
        public int PageIndex
        {
            get { return _pageIndex; }

            set
            {
       // اللي اتبعتت value اقل من او يساوي صفر رجع الصفحه الاولي لو اكتر رجع قيمه الvalue لو القيمه اللي جايه ف ال
                _pageIndex = (value <= 0) ? 1 : value;
            }
        }

        // Take
        private const int defaultPageSize = 5;
        private const int maxPageSize = 10;

        private int _pageSize = defaultPageSize;
        public int PageSize
        {
            get { return _pageSize; }

            set
            {
                if (value <= 0)
                    _pageSize = defaultPageSize;
                else if (value > maxPageSize)
                    _pageSize = maxPageSize;
                else
                    _pageSize = value;
            }
        }
    }
}
