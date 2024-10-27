using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinalApi;

namespace ModelView
{
    public static  class ImageExtensions
    {
        public static ImageViewModel toImageViewModel(this Image model)
        {
            return new ImageViewModel
            {
                ID=model.ID,
                src=model.Src

            };
        }

    }
}
