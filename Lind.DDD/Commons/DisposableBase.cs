using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Commons
{
    /// <summary>
    /// 实现IDisposable，对非托管系统进行资源回收
    /// 作者：仓储大叔
    /// </summary>
    public abstract class DisposableBase : IDisposable
    {
        /// <summary>
        /// 标准Dispose，外界可以直接调用它
        /// </summary>
        public void Dispose()
        {
            Logger.LoggerFactory.Instance.Logger_Debug("Dispose");

            this.Dispose(true);////释放托管资源
            GC.SuppressFinalize(this);//请求系统不要调用指定对象的终结器. //该方法在对象头中设置一个位，系统在调用终结器时将检查这个位
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)//_isDisposed为false表示没有进行手动dispose
            {
                //清理托管资源和清理非托管资源
                Finalize(disposing);
            }
            Logger.LoggerFactory.Instance.Logger_Debug("Dispose　complete!");
            _isDisposed = true;
        }

        /// <summary>
        /// 由子类自己去实现自己的Dispose逻辑（清理托管和非托管资源）
        /// </summary>
        /// <param name="disposing"></param>
        protected abstract void Finalize(bool disposing);

        private bool _isDisposed;

        /// <summary>
        /// 是否完成了资源的释放
        /// </summary>
        public bool IsDisposed
        {
            get { return this._isDisposed; }
        }
        /// <summary>
        /// 析构方法－在类被释放前被执行
        /// </summary>
        ~DisposableBase()
        {
            Logger.LoggerFactory.Instance.Logger_Debug("析构方法");

            //执行到这里，托管资源已经被释放
            this.Dispose(false);//释放非托管资源，托管资源由终极器自己完成了
        }
    }
}
