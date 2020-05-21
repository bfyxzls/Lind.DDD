#region Copyright and License
/*==============================================================================
 *  Copyright (c) cndotnet.org Corporation.  All rights reserved.
 * ===============================================================================
 * This code and information is provided "as is" without warranty of any kind,
 * either expressed or implied, including but not limited to the implied warranties
 * of merchantability and fitness for a particular purpose.
 * ===============================================================================
 * Licensed under the GNU General Public License (GPL) v2
 * http://www.cndotnet.org/ezsocio
 * ==============================================================================*/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lind.DDD.IRepositories.Commons
{
    /// <summary>
    /// The arguments of saved event
    /// </summary>
    public class SavedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        public virtual Object Data { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SavedEventArgs"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="action">The action.</param>
        public SavedEventArgs(Object data, SaveAction action)
        {
            this.Action = action;
            this.Data = data;
        }

        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        /// <value>The action.</value>
        public SaveAction Action { get; set; }
    }
}
