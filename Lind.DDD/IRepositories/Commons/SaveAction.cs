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
    /// Specifies the kind of save action
    /// </summary>
    public enum SaveAction
    {
        /// <summary>
        /// None
        /// </summary>
        None,

        /// <summary>
        /// Insert
        /// </summary>
        Insert,

        /// <summary>
        /// Update
        /// </summary>
        Update,

        /// <summary>
        /// Delete
        /// </summary>
        Delete
    }
}
