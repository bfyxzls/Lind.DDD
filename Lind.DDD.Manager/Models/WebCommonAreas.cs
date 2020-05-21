using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Manager.Models
{
    /// <summary>
    /// 区域表
    /// </summary>
    public class WebCommonAreas : Lind.DDD.Domain.IEntity, Lind.DDD.TreeHelper.ITree<WebCommonAreas>
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public int CommonAreaGroupID { get; set; }
        public string CommonAreaGroupName { get; set; }
        public string Code { get; set; }
        public string PinYin { get; set; }

        #region ITree<WebCommonArea> 成员

        public WebCommonAreas Father
        {
            get;
            set;
        }

        public IList<WebCommonAreas> Sons
        {
            get;
            set;
        }

        #endregion

        #region ITree 成员

        public int Id
        {
            get { return ID; }
        }

        public int Level
        {
            get
            {
                if (Code == "0")
                {
                    return 0;
                }
                else
                {
                    return Code.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries).Length;
                }
            }
        }

        public bool IsLeaf
        {
            get { return false; }
        }
        [NotMapped]
        public string LinkUrl
        {
            get;
            set;
        }

        #endregion

        #region ITree 成员


        public int? ParentID
        {
            get { return ParentId; }
        }

        #endregion

        #region ITree 成员

        [NotMapped]
        public long Authority
        {
            get;
            set;
        }

        #endregion
    }
}
