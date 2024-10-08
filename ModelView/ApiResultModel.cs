using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelView
{
    public class ApiResultModel<T>
    {
        public T result {  get; set; }
        public int StatusCode {  get; set; }
        public string Message { get; set; }
        public bool success {  get; set; }

    }
}
