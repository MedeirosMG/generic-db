using System;
using System.Collections.Generic;
using System.Text;

namespace MyDB.Infrastructure.BaseController.Model
{
    public class APIResponse<T>
    {
        public T content { get; set; }
        public string message { get; set; }
    }
}
