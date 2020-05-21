using RedisUtils;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Lind.DDD.RedisUtils
{
    /// <summary>
    /// Redis事务管理者
    /// BUG:sql插入成功,redis失败,这时sql不会被回滚,而redis会回滚;而当sql不成功,redis回滚,这时它们数据是一致的
    /// </summary>
    public class RedisTransactionManager
    {
        /// <summary>
        /// 事务块处理
        /// </summary>
        /// <param name="RedisUtils">当前redis库</param>
        /// <param name="redisAction">Redis事务中的动作</param>
        /// <param name="sqlAction">Sql事务中的动作</param>
        public static void Transaction(
            Action redisAction,
            Action sqlAction = null)
        {
            ITransaction IRT = RedisManager.Instance.GetDatabase().CreateTransaction();
            try
            {
                redisAction();
                if (sqlAction != null)
                {
                    using (var trans = new TransactionScope())
                    {
                        sqlAction();
                        trans.Complete();
                    }
                }
                IRT.Execute();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
