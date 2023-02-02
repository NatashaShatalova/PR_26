using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarServiceApp.Utils
{
    public class PageUtils
    {
        public static int skipCount = 0, takeCount = 10, clientCount = 0;

        public class PageNumber
        {
            public int pageNumber { get; set; }

            public PageNumber(int _pageNumber)
            {
                pageNumber = _pageNumber;
            }
        }
    }
}
