using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECSIsBetter
{
    public interface IComponent
    {
        /// <summary>
        /// Lets you do cool stuff like accessing the component owner's other components.
        /// </summary>
        Entity Owner { get; set; }
    }
}
